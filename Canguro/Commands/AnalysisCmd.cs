using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Canguro.Model;
using Canguro.Utility;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to analyze the model. Opens the Analysis Dialog
    /// </summary>
    public class AnalysisCmd : Canguro.Commands.ModelCommand
    {
        private bool gettingResults = false;

        /// <summary>
        /// Gets the Locale dependent title of the Command under the key "analyzeTitle"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("analyzeTitle");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Opens the AnalysisOptionsDialog, creates the export file, sends it to the Server and waits for it to have results.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            gettingResults = false;
            System.Windows.Forms.DialogResult result = services.ShowDialog(new Canguro.Commands.Model.AnalysisOptionsDialog(services));
            string message = "";
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                services.Model.Undo.Rollback();
            }
            else if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursor.Current;
                    bool isConnected;
                    bool canAnalyze = false;
                    JoinCmd.RepairJoints(services.Model);
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
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                        CanguroServer.Analysis ws = new CanguroServer.Analysis();
                        //(new ExportMDBCmd()).Run(services);
                        string modelPath = System.IO.Path.GetTempFileName();
                        System.Diagnostics.Debug.WriteLine(modelPath);
                        FixPDelta(services.Model.AbstractCases);
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

                        int analysisID = 0;
                        int numJoints = 0;
                        foreach (Canguro.Model.Joint j in services.Model.JointList)
                            if (j != null)
                                numJoints++;

                        float modelSize = numJoints * (1f + analysisOptions.Length * 0.25f);
                        string modelDescription = "ModelName=" + Path.GetFileNameWithoutExtension(services.Model.CurrentPath) + "|Joints=" + numJoints + "|Design=" + (!string.IsNullOrEmpty(analysisOptions));
                        string userNameURL = System.Web.HttpUtility.UrlEncode(services.UserCredentials.UserName);
                        string passwordURL = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Password);
                        string description = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Description);
                        string serial = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Serial);
                        string host = System.Web.HttpUtility.UrlEncode(System.Windows.Forms.SystemInformation.ComputerName);

                        CanguroServer.Quotation quotation = ws.Quote(userNameURL, passwordURL, host, serial, modelDescription);
//                      CanguroServer.Quotation quotation = ws.Quote(userNameURL, passwordURL, modelDescription, description, serial);

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
                        while (analysisID == 0)
                        {
                            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                            analysisID = ws.Analyze(userNameURL, passwordURL, host, serial, file, analysisOptions, modelSize, quotation);
//                          analysisID = ws.Analyze(userNameURL, passwordURL, file, analysisOptions, modelSize, quotation, description, serial);
                            
                            System.Windows.Forms.Cursor.Current = cursor;

                            if (analysisID == 0)
                            {
                                Forms.LoginDialog ld = new Canguro.Commands.Forms.LoginDialog(services);
                                if (services.ShowDialog(ld) == System.Windows.Forms.DialogResult.Cancel)
                                    return; // Authentication failed
                            }
                        }

                        services.Model.Results = new Canguro.Model.Results.Results(analysisID);

                        // Pass control to GetResults
                        gettingResults = true;
                        (new GetResultsCmd()).Run(services);
                    }
                    else // Can't analyze
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
                    System.Windows.Forms.MessageBox.Show(Culture.Get("ErrorAnalyzing"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            //else if (result == System.Windows.Forms.DialogResult.Cancel)
            //    throw new Canguro.Controller.CancelCommandException();
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

        public static void FixPDelta(IList<Canguro.Model.Load.AbstractCase> cases)
        {
            foreach (Canguro.Model.Load.AbstractCase ac in cases)
                if (ac is Canguro.Model.Load.AnalysisCase &&
                    ((Canguro.Model.Load.AnalysisCase)ac).Properties is Canguro.Model.Load.PDeltaCaseProps)
                    ((Canguro.Model.Load.PDeltaCaseProps)((Canguro.Model.Load.AnalysisCase)ac).Properties).ReloadDefaultLoads();
        }
    }
}
