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

                        string modelPath = System.IO.Path.GetTempFileName();
                        System.Diagnostics.Debug.WriteLine(modelPath);
                        FixPDelta(services.Model.AbstractCases);

                        Stream stream = File.Create(modelPath);
                        new Canguro.Model.Serializer.Serializer(services.Model).Serialize(stream, false);
                        stream.Close();

                        System.Windows.Forms.Cursor.Current = cursor;

                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                        // TODO: ANALYZE STRUCTURE!!!
                        //analysisID = ws.Analyze(userNameURL, passwordURL, host, serial, file, analysisOptions, modelSize, quotation);
                            
                        System.Windows.Forms.Cursor.Current = cursor;

                        services.Model.Results = new Canguro.Model.Results.Results(0);

                        // TODO: GET RESULTS
                        services.ReportProgress(5);
                    }
                    else // Can't analyze
                    {
                        if (!isConnected)
                            message = Culture.Get("structureIsDisconnectedWrn");

                        System.Windows.Forms.MessageBox.Show(message, Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show(Culture.Get("ErrorAnalyzing"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
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
