using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.IO;
using System.Net;
using System.Security.Cryptography;

using Canguro.Model.Results;
using ICSharpCode.SharpZipLib.Zip;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to retrieve analysis results from the Server.
    /// </summary>
    class GetResultsCmd : Canguro.Commands.ModelCommand
    {
        ParameterizedThreadStart loadFileDelegate;
        RijndaelManaged decryptor = new RijndaelManaged();
        private const int minimumTimeInterval = 1;
        private const int maximumTimeInterval = 15;

        public override string Title
        {
            get
            {
                return Culture.Get("getResultsCommandTitle");
            }
        }

        public override void Run(Canguro.Controller.CommandServices services)
        {
            try
            {
                int analysisID = services.Model.Results.AnalysisID;
                if (analysisID != 0)
                {
                    if (string.IsNullOrEmpty(services.UserCredentials.DisplayName))
                        if (services.ShowDialog(new Forms.LoginDialog(services)) == System.Windows.Forms.DialogResult.Cancel)
                            return;

                    string usr = System.Web.HttpUtility.UrlEncode(services.UserCredentials.UserName);
                    string psw = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Password);
                    string description = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Description);
                    string serial = System.Web.HttpUtility.UrlEncode(services.UserCredentials.Serial);
                    //string usr = services.UserCredentials.UserName;
                    //string psw = services.UserCredentials.Password;

                    services.Model.IsLocked = true;

                    if (services.Model.Results.Downloaded == null || !services.Model.Results.Downloaded.Started)
                    {
                        CanguroServer.AnalysisResult res;
                        CanguroServer.Analysis ws = new CanguroServer.Analysis();
                        DateTime lastCheck;

                        // Start progress bar
                        uint waitingProgress = 0;
                        services.ReportProgress(waitingProgress++);

                        // Calculate the time interval to check tha analysis state
                        int timeInterval = services.Model.JointList.Count / 1000;
                        timeInterval = (timeInterval < minimumTimeInterval) ? minimumTimeInterval : ((timeInterval > maximumTimeInterval) ? maximumTimeInterval : timeInterval);

                        res = ws.GetResults(usr, psw, analysisID);
//                      res = ws.GetResults(usr, psw, analysisID, description, serial);

                        lastCheck = DateTime.Now;

                        while (res.State != Canguro.CanguroServer.AnalysisState.Finished && !Cancel)
                        {
                            wait(timeInterval * 10000);

                            TimeSpan ts = DateTime.Now.Subtract(lastCheck);
                            if (ts.Seconds >= timeInterval)
                            {
                                res = ws.GetResults(usr, psw, analysisID);
//                              res = ws.GetResults(usr, psw, analysisID, description, serial);

                                lastCheck = DateTime.Now;

                                switch (res.State)
                                {
                                    case Canguro.CanguroServer.AnalysisState.Queued:
                                        services.ReportProgress((uint)(res.StateInfo * 0.333), Culture.Get("strQueuedProgress"), (uint)res.StateInfo);
                                        break;
                                    case Canguro.CanguroServer.AnalysisState.Analyzing:
                                        services.ReportProgress((uint)(0.333f + res.StateInfo * 0.333), Culture.Get("strAnalyzingProgress"), (uint)res.StateInfo);
                                        break;
                                    case Canguro.CanguroServer.AnalysisState.Error:
                                        throw new Exception();
                                        break;
                                }
                            }
                        }

                        services.Model.Results.Downloaded.Start(res.Manifest);
                    }

                    loadResults(services);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
                Console.WriteLine(ex);
            }

        }

        private void loadResults(Controller.CommandServices services)
        {
            // Set decryptor key
            decryptor.Key = services.Model.Results.Downloaded.DecryptionKey;
            decryptor.IV = services.Model.Results.Downloaded.DecryptionVector;

            populateResultsWithFile(services, services.Model.Results.Downloaded.Summary);

            // Load every other .tsr in a new thread
            bool firstCase = true;
            foreach (DownloadProps fileProps in services.Model.Results.Downloaded.Items)
            {
                populateResultsWithFile(services, fileProps);

                if (firstCase && services.Model.Results.ResultsCases.Count > 0)
                {
                    firstCase = false;
                    services.Model.Results.ActiveCase = services.Model.Results.ResultsCases[fileProps.CaseName];
                    services.Model.ChangeModel();
                }
            }

            populateResultsWithFile(services, services.Model.Results.Downloaded.Design);

            // Reset decryptor
            decryptor.Clear();

            // Finished downloading results
            services.Model.Results.Finished = true;

            // Trigger final ResultsArrived and ModelChanged events to take the newly acquired results into account
            services.Model.NewResults();
            services.Model.ChangeModel();
        }

        private void populateResultsWithFile(Controller.CommandServices services, DownloadProps props)
        {
            if (props == null || props.Finished) return;

            // Used to retrieve entire input
            StringBuilder sb = new StringBuilder();

            // Load summary in a new thread
            loadFileDelegate = new ParameterizedThreadStart(Utility.AnalysisUtils.loadFileFromWeb);
            Thread t = new Thread(loadFileDelegate);
            t.Start(new Canguro.Utility.AnalysisUtils.LoadFileThreadParams(services, (DownloadProps)props.Clone(), sb, decryptor));

            while (t.IsAlive)
            {
                wait(150);
                //t.Join(150);
            }

            // Set the downloaded Results Case as the active one (when loading a case file)
            ResultsCase activeCase = services.Model.Results.ActiveCase;
            
            if (props != services.Model.Results.Downloaded.Summary && props != services.Model.Results.Downloaded.Design)
                services.Model.Results.ActiveCase = services.Model.Results.ResultsCases[props.CaseName];

            // Load the results into the Results object
            parseFile(Utility.AnalysisUtils.DecodeStream(sb), services.Model.Results);
            
            services.Model.Results.ActiveCase = activeCase;

            props.Finished = true;
        }

        private string combineResultsCaseName(string resCase, string stepType, string stepNum)
        {
            return ResultsPath.Combine(ResultsPath.Format(resCase), ResultsPath.Combine(ResultsPath.Format(stepType), ResultsPath.Format(stepNum)));
            //string name = resCase;
            //if (!string.IsNullOrEmpty(stepType))
            //    name += "~" + stepType;
            //if (!string.IsNullOrEmpty(stepNum))
            //    name += "~" + stepNum;
            //return name;

        }

        private float parseResultsFloat(string value)
        {
            float ret = 0f;
            if (!float.TryParse(value, out ret))
            {
                double d;
                if (double.TryParse(value, out d))
                {
                    if (d > float.MaxValue)
                        ret = float.MaxValue;
                    else if (d < float.MinValue)
                        ret = float.MinValue;
                    else
                        ret = 0;
                }
                else
                    ret = 0;
            }

            if (float.IsPositiveInfinity(ret))
                ret = float.MaxValue;
            else if (float.IsNegativeInfinity(ret))
                ret = float.MinValue;
            else if (float.IsNaN(ret))
                ret = 0f;

            return ret;
        }

        private int parseResultsInt(string value)
        {
            int ret = 0;
            if (!int.TryParse(value, out ret))
            {
                double d;
                if (double.TryParse(value, out d))
                {
                    if (d > int.MaxValue)
                        ret = int.MaxValue;
                    else if (d < int.MinValue)
                        ret = int.MinValue;
                    else
                        ret = 0;
                }
                else
                    ret = 0;
            }

            return ret;
        }

        private void parseFile(string file, Canguro.Model.Results.Results results)
        {
            string[] fileChunks = file.Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int i, j;
            dataTables table;
            bool isCaseData = false;
            
            foreach (string fileChunk in fileChunks)
            {
                List<string[]> chunk = getDataChunk(fileChunk, out table);
                if (chunk == null) continue;

                switch (table)
                {
                    #region Summary data
                    case dataTables.ResultsCases:
                        foreach (string[] rc in chunk)
                            results.ResultsCases.Add(ResultsPath.Format(rc[0]));
                        results.Init();
                        break;
                    case dataTables.AssembledJointMasses:
                        for (i = 0; i < chunk.Count; i++)
                            for (j = 0; j < 6; j++) {
                                if (chunk[i][j + 1].ToString() == "") chunk[i][j + 1] = "0.0";
                                if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                                results.AssembledJointMasses[parseResultsInt(chunk[i][0]), j] =
                                    parseResultsFloat(chunk[i][j + 1]);
                            }
                        break;
                    case dataTables.ModalLoadParticipationRatios:
                        for (i = 0; i < chunk.Count; i++) {
                            // Get ModalLPR List from ResultsCase
                            string caseName = ResultsPath.Format(chunk[i][0]);
                            if (results.ResultsCases.FindPath(caseName) != null) {
                                // Create list if it doesn't exist
                                if (!results.ModalLPR.ContainsKey(caseName))
                                    results.ModalLPR.Add(caseName, new List<ModalLPRRow>());
                                List<ModalLPRRow> row = results.ModalLPR[caseName];

                                // Add the row to the list
                                if (row != null) {
                                    if (chunk[i][3].ToString() == "") chunk[i][3] = "0.0";
                                    if (chunk[i][4].ToString() == "") chunk[i][4] = "0.0";
                                    row.Add(new ModalLPRRow(chunk[i][1].Trim(), chunk[i][2].Trim(),
                                        parseResultsFloat(chunk[i][3]), parseResultsFloat(chunk[i][4])));
                                }
                            }
                        }
                        break;
                    
                    case dataTables.ModalParticipatingMassRatios:
                        for (i = 0; i < chunk.Count; i++) {
                            // Get ModalLPR List from ResultsCase
                            results.ActiveCase = results.ResultsCases[combineResultsCaseName(chunk[i][0], chunk[i][1], chunk[i][2])];
                            if (results.ActiveCase != null) {
                                results.ModalPMR = new float[13];
                                for (j = 0; j < 13; j++) {
                                    if (chunk[i][j + 3].ToString() == "") chunk[i][j + 3] = "0.0";
                                    results.ModalPMR[j] = parseResultsFloat(chunk[i][j + 3]);
                                }
                            }
                        }
                        break;
                    case dataTables.ModalParticipationFactors:
                        for (i = 0; i < chunk.Count; i++) {
                            // Get ModalLPR List from ResultsCase
                            results.ActiveCase = results.ResultsCases[combineResultsCaseName(chunk[i][0], chunk[i][1], chunk[i][2])];
                            if (results.ActiveCase != null) {
                                results.ModalPF = new float[9];
                                for (j = 0; j < 9; j++) {
                                    if (chunk[i][j + 3].ToString() == "") chunk[i][j + 3] = "0.0";
                                    results.ModalPF[j] = parseResultsFloat(chunk[i][j + 3]);
                                }
                            }
                        }
                        break;
                    case dataTables.ModalPeriodsAndFrequencies:
                        for (i = 0; i < chunk.Count; i++) {
                            // Get ModalLPR List from ResultsCase
                            results.ActiveCase = results.ResultsCases[combineResultsCaseName(chunk[i][0], chunk[i][1], chunk[i][2])];
                            if (results.ActiveCase != null) {
                                results.ModalPeriods = new float[3];
                                for (j = 0; j < 3; j++) {
                                    if (chunk[i][j + 3].ToString() == "") chunk[i][j + 3] = "0.0";
                                    results.ModalPeriods[j] = parseResultsFloat(chunk[i][j + 3]);
                                }
                            }
                        }
                        break;
                    case dataTables.ResponseSpectrumModalInformation:
                        if (chunk.Count == 0) break;

                        results.ResponseSpectrumMI = new float[chunk.Count, chunk[0].Length - 3];

                        for (i = 0; i < chunk.Count; i++) {
                            // Get ModalLPR List from ResultsCase
                            //string spectrumCaseName = combineResultsCaseName(chunk[i][0], chunk[i][1], chunk[i][2]);
                            if (results.ActiveCase != null) {
                                for (j = 0; j < 8; j++) {
                                    if (chunk[i][j + 3].ToString() == "") chunk[i][j + 3] = "0.0";
                                    results.ResponseSpectrumMI[i, j] = parseResultsFloat(chunk[i][j + 3]);
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Case based data
                    case dataTables.JointDisplacements:
                        for (i = 0; i < chunk.Count; i++)
                            for (j = 0; j < 6; j++) {
                                if (chunk[i][j + 1].ToString() == "") chunk[i][j + 1] = "0.0";
                                if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                                results.JointDisplacements[parseResultsInt(chunk[i][0]), j] =
                                    parseResultsFloat(chunk[i][j + 1]);
                            }
                        isCaseData = true;
                        break;
                    case dataTables.JointVelocitiesRelative:
                        for (i = 0; i < chunk.Count; i++)
                            for (j = 0; j < 6; j++) {
                                if (chunk[i][j + 1].ToString() == "") chunk[i][j + 1] = "0.0";
                                if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                                results.JointVelocities[parseResultsInt(chunk[i][0]), j] =
                                    parseResultsFloat(chunk[i][j + 1]);
                            }
                        isCaseData = true;
                        break;
                    case dataTables.JointAccelerationsRelative:
                        for (i = 0; i < chunk.Count; i++)
                            for (j = 0; j < 6; j++) {
                                if (chunk[i][j + 1].ToString() == "") chunk[i][j + 1] = "0.0";
                                if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                                results.JointAccelerations[parseResultsInt(chunk[i][0]), j] =
                                    parseResultsFloat(chunk[i][j + 1]);
                            }
                        isCaseData = true;
                        break;
                    case dataTables.JointReactions:
                        for (i = 0; i < chunk.Count; i++)
                            for (j = 0; j < 6; j++) {
                                if (chunk[i][j + 1].ToString() == "") chunk[i][j + 1] = "0.0";
                                if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                                results.JointReactions[parseResultsInt(chunk[i][0]), j] =
                                    parseResultsFloat(chunk[i][j + 1]);
                            }
                        isCaseData = true;
                        break;
                    case dataTables.BaseReactions:
                        if (chunk.Count > 0)
                            for (j = 0; j < 18; j++) {
                                if (chunk[0][j].ToString() == "") chunk[0][j] = "0.0";
                                results.BaseReactions[j] = parseResultsFloat(chunk[0][j]);
                            }
                        isCaseData = true;
                        break;
                    case dataTables.ElementJointForcesFrames:
                        for (i = 0; i < chunk.Count; i++)
                            for (j = 0; j < 6; j++) {
                                if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                                if (chunk[i][1].ToString() == "") chunk[i][1] = "0";
                                int lineIndex = parseResultsInt(chunk[i][0]);
                                int jointIndexInFrame = getJointIndexInFrame(lineIndex, parseResultsInt(chunk[i][1]));
                                if (jointIndexInFrame < 2) {
                                    if (chunk[i][j + 2].ToString() == "") chunk[i][j + 2] = "0.0";
                                    results.ElementJointForces[lineIndex, jointIndexInFrame, j] =
                                        parseResultsFloat(chunk[i][j + 2]);
                                }
                            }
                        isCaseData = true;
                        break;
                    #endregion

                    #region Design data
                    case dataTables.SteelDesignSummary:
                        for (i = 0; i < chunk.Count; i++) {
                            int numLine = parseResultsInt(chunk[i][0]);
                            if (chunk[i][1].ToString() == "") chunk[i][1] = "0";
                            results.DesignSteelSummary[numLine].Ratio = parseResultsFloat(chunk[i][1]);
                            results.DesignSteelSummary[numLine].ErrMsg = chunk[i][2];
                            results.DesignSteelSummary[numLine].WarnMsg = chunk[i][3];
                            results.DesignSteelSummary[numLine].Status = chunk[i][4];

                            results.DesignSteelSummary[numLine].DesignData = new string[chunk[i].Length - 5];
                            for (j = 5; j < chunk[i].Length; j++)
                                results.DesignSteelSummary[numLine].DesignData[j - 5] = chunk[i][j];
                        }
                        break;
                    case dataTables.SteelDesignPMMDetails:
                        for (i = 0; i < chunk.Count; i++) {
                            if (chunk[i][0].ToString() == "") chunk[i][0] = "0";
                            int numLine = parseResultsInt(chunk[i][0]);
                            if (chunk[i][1].ToString() == "") chunk[i][1] = "0.0";
                            results.DesignSteelPMMDetails[numLine].TotalRatio = parseResultsFloat(chunk[i][1]);
                            if (chunk[i][2].ToString() == "") chunk[i][2] = "0.0";
                            results.DesignSteelPMMDetails[numLine].PRatio = parseResultsFloat(chunk[i][2]);
                            if (chunk[i][3].ToString() == "") chunk[i][3] = "0.0";
                            results.DesignSteelPMMDetails[numLine].MMajRatio = parseResultsFloat(chunk[i][3]);
                            if (chunk[i][4].ToString() == "") chunk[i][4] = "0.0";
                            results.DesignSteelPMMDetails[numLine].MMinRatio = parseResultsFloat(chunk[i][4]);
                            results.DesignSteelPMMDetails[numLine].ErrMsg = chunk[i][5];
                            results.DesignSteelPMMDetails[numLine].WarnMsg = chunk[i][6];
                            results.DesignSteelPMMDetails[numLine].Status = chunk[i][7];

                            results.DesignSteelPMMDetails[numLine].DesignData = new string[chunk[i].Length - 8];
                            for (j = 8; j < chunk[i].Length; j++)
                                results.DesignSteelPMMDetails[numLine].DesignData[j - 8] = chunk[i][j];
                        }
                        break;
                    case dataTables.SteelDesignShearDetails:
                        for (i = 0; i < chunk.Count; i++) {
                            int numLine = parseResultsInt(chunk[i][0]);
                            if (chunk[i][1].ToString() == "") chunk[i][1] = "0.0";
                            results.DesignSteelShearDetails[numLine].VMajorRatio = parseResultsFloat(chunk[i][1]);
                            if (chunk[i][2].ToString() == "") chunk[i][2] = "0.0";
                            results.DesignSteelShearDetails[numLine].VMinorRatio = parseResultsFloat(chunk[i][2]);
                            results.DesignSteelShearDetails[numLine].ErrMsg = chunk[i][3];
                            results.DesignSteelShearDetails[numLine].WarnMsg = chunk[i][4];
                            results.DesignSteelShearDetails[numLine].Status = chunk[i][5];

                            results.DesignSteelShearDetails[numLine].DesignData = new string[chunk[i].Length - 6];
                            for (j = 6; j < chunk[i].Length; j++)
                                results.DesignSteelShearDetails[numLine].DesignData[j - 6] = chunk[i][j];
                        }
                        break;
                    case dataTables.ConcreteDesignColumnSummary:
                        for (i = 0; i < chunk.Count; i++) {
                            int numLine = parseResultsInt(chunk[i][0]);
                            results.DesignConcreteColumn[numLine].Status = chunk[i][1];
                            if (chunk[i][2].ToString() == "") chunk[i][2] = "0.0";
                            results.DesignConcreteColumn[numLine].PMMArea = parseResultsFloat(chunk[i][2]);
                            if (chunk[i][3].ToString() == "") chunk[i][3] = "0.0";
                            results.DesignConcreteColumn[numLine].PMMRatio = chunk[i][3];
                            if (chunk[i][4].ToString() == "") chunk[i][4] = "0.0";
                            results.DesignConcreteColumn[numLine].VMajRebar = parseResultsFloat(chunk[i][4]);
                            if (chunk[i][5].ToString() == "") chunk[i][5] = "0.0";
                            results.DesignConcreteColumn[numLine].VMinRebar = parseResultsFloat(chunk[i][5]);
                            results.DesignConcreteColumn[numLine].ErrMsg = chunk[i][6];
                            results.DesignConcreteColumn[numLine].WarnMsg = chunk[i][7];

                            results.DesignConcreteColumn[numLine].DesignData = new string[chunk[i].Length - 8];
                            for (j = 8; j < chunk[i].Length; j++)
                                results.DesignConcreteColumn[numLine].DesignData[j - 8] = chunk[i][j];
                        }
                        break;
                    case dataTables.ConcreteDesignBeamSummary:
                        for (i = 0; i < chunk.Count; i++) {
                            int numLine = parseResultsInt(chunk[i][0]);
                            results.DesignConcreteBeam[numLine].Status = chunk[i][1];
                            if (chunk[i][2].ToString() == "") chunk[i][2] = "0.0";
                            results.DesignConcreteBeam[numLine].FTopArea = parseResultsFloat(chunk[i][2]);
                            if (chunk[i][3].ToString() == "") chunk[i][3] = "0.0";
                            results.DesignConcreteBeam[numLine].FBotArea = parseResultsFloat(chunk[i][3]);
                            if (chunk[i][4].ToString() == "") chunk[i][4] = "0.0";
                            results.DesignConcreteBeam[numLine].VRebar = parseResultsFloat(chunk[i][4]);
                            results.DesignConcreteBeam[numLine].ErrMsg = chunk[i][5];
                            results.DesignConcreteBeam[numLine].WarnMsg = chunk[i][6];

                            results.DesignConcreteColumn[numLine].DesignData = new string[chunk[i].Length - 7];
                            for (j = 7; j < chunk[i].Length; j++)
                                results.DesignConcreteColumn[numLine].DesignData[j - 7] = chunk[i][j];
                        }
                        break;
                    #endregion

                    // If the data is not recognized, ignore it
                    default:
                        break;// throw new FileLoadException();
                }

            }

            if (isCaseData && results.ActiveCase != null)
                results.ActiveCase.IsLoaded = true;
        }

        private int getJointIndexInFrame(int lineId, int jointId)
        {
            Canguro.Model.LineElement line = Canguro.Model.Model.Instance.LineList[lineId];

            if (line.I.Id == jointId)
                return 0;
            else if (line.J.Id == jointId)
                return 1;
            else
                return 2;
        }

        enum dataTables
        {
            NoTable,
            ResultsCases,
            AssembledJointMasses,
            ModalPeriodsAndFrequencies,
            ModalLoadParticipationRatios,
            ModalParticipatingMassRatios,
            ModalParticipationFactors,
            ResponseSpectrumModalInformation,
            JointDisplacements,
            JointReactions,
            JointVelocitiesRelative,
            JointAccelerationsRelative,
            ElementJointForcesFrames,
            BaseReactions,
            SteelDesignSummary,
            SteelDesignPMMDetails,
            SteelDesignShearDetails,
            ConcreteDesignColumnSummary,
            ConcreteDesignBeamSummary
        }

        private List<string[]> getDataChunk (string chunk, out dataTables table)
        {
            List<string[]> data = new List<string[]>();
            StringReader sr = new StringReader(chunk);
            string line = sr.ReadLine();
            string tableName = line.Replace("]", "").Replace("-", "");
            table = dataTables.NoTable;
            
            try
            {
                table = (dataTables)Enum.Parse(typeof(dataTables), tableName, true);
            }
            catch (ArgumentException)
            {
                if (tableName.Contains("SteelDesign"))
                {
                    if (tableName.Contains("Summary"))
                        table = dataTables.SteelDesignSummary;
                    else if (tableName.Contains("PMM"))
                        table = dataTables.SteelDesignPMMDetails;
                    else if (tableName.Contains("Shear"))
                        table = dataTables.SteelDesignShearDetails;
                }
                else if (tableName.Contains("ConcreteDesign"))
                {
                    if (tableName.Contains("Column"))
                        table = dataTables.ConcreteDesignColumnSummary;
                    else if (tableName.Contains("Beam"))
                        table = dataTables.ConcreteDesignBeamSummary;
                }
            }

            if (table == dataTables.NoTable)
                return null;

            while ((line = sr.ReadLine()) != null)
                data.Add(line.Split('\t'));

            return data;
        }

        public override bool AllowCancel()
        {
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

        private void wait(int milliseconds)
        {
            // if ModelCmd has been cancelled or ModelCmd is no longer registered on the Controller
            // Cancel the command and stop the waiting
            if ((Controller.Controller.Instance.ModelCommand == null) || (Cancel))
                return;

            Utility.NativeHelperMethods.WaitInMainThread(milliseconds);
        }
    }
}
