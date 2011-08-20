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
using System.Text;
using System.Xml;

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
                
                // Verify model consistency for analysis (e.g. graph connectivity)
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

                    // Serialize the model
                    string modelPath = System.IO.Path.GetTempFileName();
                    AnalysisCmd.FixPDelta(services.Model.AbstractCases);
                    Stream stream = File.Create(modelPath);
                    new Canguro.Model.Serializer.Serializer(services.Model).Serialize(stream, false);
                    stream.Close();

                    System.Windows.Forms.Cursor.Current = cursor;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                    // Export to s2k
                    Export(modelPath, dstFile);

                    System.Windows.Forms.Cursor.Current = cursor;
                }
                else // Can't export
                {
                    if (!isConnected)
                        message = Culture.Get("structureIsDisconnectedWrn");
                    System.Windows.Forms.MessageBox.Show(message, Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(Culture.Get("ErrorExporting"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public override bool AllowCancel()
        {
            if (!Canguro.Model.Model.Instance.IsLocked)
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

        /// <summary>
        /// xml file to convert
        /// </summary> 
        /// <param name="fileName"></param>
        private static void Export(string srcFileName, string dstFileName)
        {
            // Output file
            TextWriter s2kDoc = new StreamWriter(dstFileName, false, Encoding.UTF8);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(srcFileName);

            //Get all nodes in xml file
            XmlNodeList nodes = xmlDoc.SelectSingleNode("XmlExportedFile").ChildNodes;
            foreach (XmlNode node in nodes)
                translateXML(node, s2kDoc);

            s2kDoc.WriteLine("END TABLE DATA");

            s2kDoc.Close();
        }

        private static void translateXML(XmlNode node, TextWriter s2kDoc)
        {
            string nameTable = node.Name.Replace("_", " ").ToUpper();
            if (nameTable.StartsWith("T-"))
                return;

            s2kDoc.WriteLine("TABLE:  \"" + nameTable + "\"");
            if (node.ChildNodes.Count == 0)
            {
                XmlAttributeCollection attibutes = node.Attributes;
                StringBuilder attBuilder = new StringBuilder();
                foreach (XmlAttribute att in attibutes)
                {
                    string value = att.Value;
                    attBuilder.Append("   " + att.Name + "=\"" + EncodeValue(ref value) + "\"");
                }

                s2kDoc.WriteLine(attBuilder.ToString());
            }
            else
            {
                XmlNodeList childs = node.ChildNodes;
                foreach (XmlNode son in childs)
                {
                    XmlAttributeCollection attibutes = son.Attributes;
                    StringBuilder attBuilder = new StringBuilder();
                    foreach (XmlAttribute att in attibutes)
                    {
                        string value = att.Value;
                        attBuilder.Append("   " + att.Name + "=\"" + EncodeValue(ref value) + "\"");
                    }

                    s2kDoc.WriteLine(attBuilder.ToString());
                }
            }
            s2kDoc.WriteLine(" ");
        }

        private static StringBuilder buffStr = new StringBuilder();
        private static string EncodeValue(ref string text)
        {
            buffStr.Remove(0, buffStr.Length);
            foreach (char c in text)
            {
                if (c == '\'')
                    buffStr.Append("&apos;");
                else if (c == '"')
                    buffStr.Append("&quot;");
                else if ((c <= 0x1f && c != 0x9 && c != 0x10 && c != 0x13) || c > 127)
                    buffStr.Append(string.Format("&#x{0:x};", (int)c));
                else
                    buffStr.Append(c);
            }
            return buffStr.ToString();
        }
    }
}
