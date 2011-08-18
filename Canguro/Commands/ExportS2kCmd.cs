using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Canguro.Model;
using Canguro.Model.Results;
using Canguro.Utility;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Canguro.Commands.Model
{
    class ExportS2kCmd : Canguro.Commands.ModelCommand
    {
        private bool gettingResults = false;
        RijndaelManaged decryptor = new RijndaelManaged();

        /// <summary>
        /// Gets the Locale dependent title of the Command under the key "export2skTitle"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("export2skTitle");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Opens the SaveTo dialog, creates the xml file, sends it to the Server and waits for it to have the manifest for the exported s2k.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            gettingResults = false;
            try
            {
                string message = "";
                System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursor.Current;
                bool isConnected;
                bool canAnalyze = false;
                new Canguro.Commands.Model.UnselectCmd().Run(services);
                if (!(canAnalyze = AnalysisUtils.CanAnalyze(services.Model, ref message, out isConnected)))
                {
                    if (!isConnected)
                    {
                        if (System.Windows.Forms.MessageBox.Show(message, Culture.Get("error"),
                            System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {

                            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                            new JoinCmd().Run(services);
                            canAnalyze = AnalysisUtils.CanAnalyze(services.Model, ref message, out isConnected);

                            System.Windows.Forms.Cursor.Current = cursor;
                        }
                        else
                            return;
                    }
                }
                if (canAnalyze)
                {
                    System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                    dlg.Filter = "S2K File (*.s2k)|*.s2k";
                    dlg.DefaultExt = "s2k";
                    dlg.AddExtension = true;
                    dlg.Title = Culture.Get("ExportS2KTitle");
                    if (!string.IsNullOrEmpty(services.Model.CurrentPath))
                        dlg.FileName = Path.Combine(Path.GetDirectoryName(services.Model.CurrentPath), Path.GetFileNameWithoutExtension(services.Model.CurrentPath)) + ".s2k";
                    else
                        dlg.FileName = Culture.Get("defaultModelName");
                    System.Windows.Forms.DialogResult result = (dlg).ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                    string dstFile = dlg.FileName;

                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                    CanguroServer.Analysis ws = new CanguroServer.Analysis();
                    //(new ExportMDBCmd()).Run(services);
                    string modelPath = System.IO.Path.GetTempFileName();
                    System.Diagnostics.Debug.WriteLine(modelPath);
                    AnalysisCmd.FixPDelta(services.Model.AbstractCases);
                    // Change a xml file insted  mdb file
                    //(new CreateXMLCmd()).Export(services.Model, modelPath);
                    //new ExportMDBCmd().Export(services.Model, modelPath);
                    Stream stream = File.Create(modelPath);
                    new Canguro.Model.Serializer.Serializer(services.Model).Serialize(stream, false);
                    stream.Close();

                    byte[] file = AnalysisUtils.GetCompressedModel(modelPath /*"tmp"*/);
                    //byte[] file = File.ReadAllBytes("tmp.mdb");                                             

                    string analysisOptions = "";
                    if (!(services.Model.SteelDesignOptions is Canguro.Model.Design.NoDesign))
                        analysisOptions += "S";
                    if (!(services.Model.ConcreteDesignOptions is Canguro.Model.Design.NoDesign))
                        analysisOptions += "C";
                    if (!(services.Model.ColdFormedDesignOptions is Canguro.Model.Design.NoDesign))
                        analysisOptions += "O";
                    if (!(services.Model.AluminumDesignOptions is Canguro.Model.Design.NoDesign))
                        analysisOptions += "A";

                    CanguroServer.AnalysisResult ar = null;
                    int numJoints = 0;
                    foreach (Canguro.Model.Joint j in services.Model.JointList)
                        if (j != null)
                            numJoints++;

                    float modelSize = numJoints * (1f + analysisOptions.Length * 0.25f);
                    string modelDescription = "Export=s2k|ModelName=" + Path.GetFileNameWithoutExtension(services.Model.CurrentPath) + "|Joints=" + numJoints + "|Design=" + (!string.IsNullOrEmpty(analysisOptions));
                    string userNameURL = System.Web.HttpUtility.UrlEncode(services.UserCredentials.UserName);
                    string passwordURL = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Password);
                    string description = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Description);
                    string serial = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Serial);
                    string host = System.Web.HttpUtility.UrlEncode(System.Windows.Forms.SystemInformation.ComputerName);

                    int maxRetry = 3;
                    int retry = 0;
                    CanguroServer.Quotation quotation = null;
                    while (retry < maxRetry)
                    {
                        try
                        {
                            quotation = ws.Quote(userNameURL, passwordURL, host, serial, modelDescription);
                            //                  CanguroServer.Quotation quotation = ws.Quote(userNameURL, passwordURL, modelDescription, description, serial);

                            System.Windows.Forms.Cursor.Current = cursor;

                            if (quotation.Cost > 0)
                            {
                                if (!quotation.CanAfford)
                                {
                                    System.Windows.Forms.MessageBox.Show(Culture.Get("strCannotAffordService").Replace("*cost*", quotation.Cost.ToString()), Culture.Get("ActionNotPossibleTitle"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                                    return;

                                    //Commands.Forms.AddCredit addCredit = new Canguro.Commands.Forms.AddCredit();
                                    //services.ShowDialog(addCredit);
                                    //return;
                                }

                                System.Windows.Forms.DialogResult acceptCharge = System.Windows.Forms.MessageBox.Show(Culture.Get("strAskToAcceptCharge") + " " + quotation.Cost + " " + Culture.Get("strMoney"), Culture.Get("strAskToAcceptChargeTitle"), System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Question);
                                if (acceptCharge == System.Windows.Forms.DialogResult.Cancel)
                                    return;
                            }

                            retry = 0;
                            while (retry < maxRetry || ar == null)
                            {
                                try
                                {
                                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                                    ar = ws.ExportToS2K(userNameURL, passwordURL, host, serial, file, analysisOptions, modelSize, quotation);
                                    //ar = ws.ExportToS2K(userNameURL, passwordURL, file, analysisOptions, modelSize, quotation, description, serial);

                                    System.Windows.Forms.Cursor.Current = cursor;

                                    if (ar == null)
                                    {
                                        Forms.LoginDialog ld = new Canguro.Commands.Forms.LoginDialog(services);
                                        if (services.ShowDialog(ld) == System.Windows.Forms.DialogResult.Cancel)
                                            return; // Authentication failed
                                    }
                                    else
                                        retry = maxRetry;
                                }
                                catch (Exception ex)
                                {
                                    retry++;
                                    if (retry < maxRetry)
                                        System.Diagnostics.Debug.WriteLine(ex.Message);
                                    else
                                        throw ex;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            retry++;
                            if (retry < maxRetry)
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                            else
                                throw ex;
                        }
                    }

                    // Pass control to GetResults
                    gettingResults = true;
                    getS2kFile(ar, dstFile, services);
                }
                else // Can't export
                {
                    if (!isConnected)
                        message = Culture.Get("structureIsDisconnectedWrn");
                    System.Windows.Forms.MessageBox.Show(message, Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            catch (System.Net.WebException)
            {
                System.Windows.Forms.MessageBox.Show(Culture.Get("ErrorInWebService"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(Culture.Get("ErrorExporting"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void getS2kFile(CanguroServer.AnalysisResult ar, string dstFile, Controller.CommandServices services)
        {
            if (ar.State != Canguro.CanguroServer.AnalysisState.Finished || ar.Manifest == null)
                throw new Exception("Cannot download export due to errors in the returned AnalysisResult object");

            Canguro.Model.Results.DownloadProps props = new Canguro.Model.Results.DownloadProps(ar.Manifest.Summary);

            if (props == null)
                throw new Exception("Cannot download export due to errors in the returned AnalysisResult object");

            // Used to retrieve entire input
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Set decryptor key
            decryptor.Key = ar.Manifest.DecryptionKey;
            decryptor.IV = ar.Manifest.DecryptionVector;

            // Load summary in a new thread
            ParameterizedThreadStart loadFileDelegate = new ParameterizedThreadStart(Utility.AnalysisUtils.loadFileFromWeb);
            Thread t = new Thread(loadFileDelegate);
            t.Start(new Canguro.Utility.AnalysisUtils.LoadFileThreadParams(services, (DownloadProps)props.Clone(), sb, decryptor));

            while (t.IsAlive)
            {
                wait(150);
                //t.Join(150);
            }

            decryptor.Clear();

            // Now save the stream to a file
            StreamWriter sw = new StreamWriter(File.Create(dstFile));
            sw.Write(sb.ToString());
            sw.Close();
        }

        private void wait(int milliseconds)
        {
            // if ModelCmd has been cancelled or ModelCmd is no longer registered on the Controller
            // Cancel the command and stop the waiting
            if ((Controller.Controller.Instance.ModelCommand == null) || (Cancel))
                return;

            Utility.NativeHelperMethods.WaitInMainThread(milliseconds);
        }

        public override bool AllowCancel()
        {
            if (!gettingResults || !Canguro.Model.Model.Instance.IsLocked)
                return true;

            System.Windows.Forms.DialogResult r =
                System.Windows.Forms.MessageBox.Show(
                Culture.Get("looseAnalysisQuestionStr"),
                Culture.Get("analysisResultsQuestionTitle"),
                System.Windows.Forms.MessageBoxButtons.YesNo,
                System.Windows.Forms.MessageBoxIcon.Warning,
                System.Windows.Forms.MessageBoxDefaultButton.Button2);

            if (r == System.Windows.Forms.DialogResult.Yes)
                Canguro.Model.Model.Instance.IsLocked = false;

            return (r == System.Windows.Forms.DialogResult.Yes);
        }
    }
}
