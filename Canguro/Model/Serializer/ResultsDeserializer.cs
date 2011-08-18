using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Canguro.Model.Results;

namespace Canguro.Model.Serializer
{
    class ResultsDeserializer
    {
        private Results.Results results;
        private Model model;
        private static char[] comma = ",".ToCharArray();

        public ResultsDeserializer(Canguro.Model.Model model)
        {
            this.model = model;
            results = null;
        }

        public void Deserialize(XmlNode xml)
        {
            try
            {
                if ("T-Results".Equals(xml.Name))
                {
                    int id = int.Parse(Deserializer.readAttribute(xml, "ID"));
                    results = new Canguro.Model.Results.Results(id);
                    results.Finished = bool.Parse(Deserializer.readAttribute(xml, "Finished", true.ToString()));
                    model.Results = results;
                }
                if (results != null && results.AnalysisID > 0)
                {
                    readResultCases(xml.SelectSingleNode("T-Result_Cases"));
                    readResultProgress(xml.SelectSingleNode("T-Result_Progress"));
                    results.Init();
                    readJointDisplacements(xml.SelectSingleNode("Joint_Displacements"));
                    readJointReactions(xml.SelectSingleNode("Joint_Reactions"));
                    readJointVelocities(xml.SelectSingleNode("Joint_Velocities_-_Relative"));
                    readJointAccelerations(xml.SelectSingleNode("Joint_Accelerations_-_Relative"));
                    readJointMasses(xml.SelectSingleNode("Assembled_Joint_Masses"));
                    readElementJointForces(xml.SelectSingleNode("Element_Joint_Forces_-_Frames"));
                    readBaseReactions(xml.SelectSingleNode("Base_Reactions"));
                    readModalPeriods(xml.SelectSingleNode("Modal_Periods_And_Frequencies"));
                    readModalLoadParticipation(xml.SelectSingleNode("Modal_Load_Participation_Ratios"));
                    readModalParticipationMass(xml.SelectSingleNode("Modal_Participating_Mass_Ratios"));
                    readModalParticipationFactors(xml.SelectSingleNode("Modal_Participation_Factors"));
                    readResponseSpectrumModalInfo(xml.SelectSingleNode("Response_Spectrum_Modal_Information"));
                    readDesignSteelSummary(xml.SelectSingleNode("T-Design_Steel_Summary"));
                    readDesignSteelPMM(xml.SelectSingleNode("T-Design_Steel_PMM"));
                    readDesignSteelShear(xml.SelectSingleNode("T-Design_Steel_Shear"));
                    readDesignConcreteColumn(xml.SelectSingleNode("T-Design_Concrete_Column"));
                    readDesignConcreteBeam(xml.SelectSingleNode("T-Design_Concrete_Beam"));
                    if (results.ResultsCases.Count > 0)
                        results.ActiveCase = results.ResultsCases[0];
                }
            }
            catch (Exception ex)
            {
                model.IsLocked = false;
                System.Windows.Forms.MessageBox.Show("Cannot load results");
            }
        }

        private void readResultCases(XmlNode xml)
        {
            if ("T-Result_Cases".Equals(xml.Name))
                foreach (XmlNode child in xml.ChildNodes)
                    if ("Result_Case".Equals(child.Name))
                    {
                        ResultsCase rc;
                        int id = int.Parse(Deserializer.readAttribute(child, "ID"));
                        string fullPath = Deserializer.readAttribute(child, "FullPath");
                        rc = new ResultsCase(id, fullPath);
                        rc.IsLoaded = bool.Parse(Deserializer.readAttribute(child, "IsLoaded", "True"));
                        results.ResultsCases.Add(rc);
                    }
        }

        private void readResultProgress(XmlNode xml)
        {
            System.Text.Encoding coder = System.Text.ASCIIEncoding.Default;
            if (xml != null && "T-Result_Progress".Equals(xml.Name))
            {
                uint totalFin = uint.Parse(Deserializer.readAttribute(xml, "TotalProgress", "0"));
                bool started = bool.Parse(Deserializer.readAttribute(xml, "Started", true.ToString()));
                DownloadProgress progress = new DownloadProgress(totalFin, started);
                progress.DecryptionKey = coder.GetBytes(Deserializer.readAttribute(xml, "Key"));
                progress.DecryptionVector = coder.GetBytes(Deserializer.readAttribute(xml, "Vector"));
                progress.Design = new DownloadProps();
                progress.Design.CaseName = Deserializer.readAttribute(xml, "DesignCase");
                progress.Design.FileName = Deserializer.readAttribute(xml, "DesignFile");
                progress.Design.Finished = bool.Parse(Deserializer.readAttribute(xml, "DesignFinished"));
                progress.Design.Percentage = uint.Parse(Deserializer.readAttribute(xml, "DesignPercentage"));
                progress.Model = Deserializer.readAttribute(xml, "Model");
                progress.Summary = new DownloadProps();
                progress.Summary.CaseName = Deserializer.readAttribute(xml, "SumCase");
                progress.Summary.FileName = Deserializer.readAttribute(xml, "SumFile");
                progress.Summary.Finished = bool.Parse(Deserializer.readAttribute(xml, "SumFinished"));
                progress.Summary.Percentage = uint.Parse(Deserializer.readAttribute(xml, "SumPercentage"));
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if (child != null && "Item".Equals(child.Name))
                    {
                        DownloadProps item = new DownloadProps();
                        item.CaseName = Deserializer.readAttribute(child, "Case");
                        item.FileName = Deserializer.readAttribute(child, "File");
                        item.Finished = bool.Parse(Deserializer.readAttribute(child, "Finished"));
                        item.Percentage = uint.Parse(Deserializer.readAttribute(child, "Percentage"));
                        progress.Items.AddLast(item);
                    }
                }
                results.Downloaded = progress;
            }
        }

        private void readJointDisplacements(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                int count = model.JointList.Count;
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case"));
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        if (i < count)
                        {
                            results.ActiveCase = results.ResultsCases[rCase];
                            string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                            for (int k = 0; k < str.Length; k++)
                                results.JointDisplacements[i, k] = float.Parse(str[k]);
                        }
                    }
                }
            }
        }

        private void readJointReactions(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                int count = model.JointList.Count;
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        if (i < count)
                        {
                            results.ActiveCase = results.ResultsCases[rCase];
                            string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                            for (int k = 0; k < str.Length; k++)
                                results.JointReactions[i, k] = float.Parse(str[k]);
                        }
                    }
                }
            }
        }

        private void readJointVelocities(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                int count = model.JointList.Count;
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        if (i < count)
                        {
                            results.ActiveCase = results.ResultsCases[rCase];
                            string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                            for (int k = 0; k < str.Length; k++)
                                results.JointVelocities[i, k] = float.Parse(str[k]);
                        }
                    }
                }
            }

        }

        private void readJointAccelerations(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                int count = model.JointList.Count;
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        if (i < count)
                        {
                            results.ActiveCase = results.ResultsCases[rCase];
                            string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                            for (int k = 0; k < str.Length; k++)
                                results.JointAccelerations[i, k] = float.Parse(str[k]);
                        }
                    }
                }
            }
        }

        private void readJointMasses(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                int count = model.JointList.Count;
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        if (i < count)
                        {
                            results.ActiveCase = results.ResultsCases[rCase];
                            string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                            for (int k = 0; k < str.Length; k++)
                                results.AssembledJointMasses[i, k] = float.Parse(str[k]);
                        }
                    }
                }
            }
//                readDataTags(xml, results.AssembledJointMasses);
        }

        private void readElementJointForces(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                int count = model.LineList.Count;
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        if (i < count)
                        {
                            int j = int.Parse(Deserializer.readAttribute(child, "J"));
                            results.ActiveCase = results.ResultsCases[rCase];
                            string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                            for (int k = 0; k < str.Length; k++)
                                results.ElementJointForces[i, j, k] = float.Parse(str[k]);
                        }
                    }
                }
            }
        }

        private void readBaseReactions(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        uint i = uint.Parse(Deserializer.readAttribute(child, "I"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        results.BaseReactions[i] = float.Parse(Deserializer.readAttribute(child, "Data"));
                    }
                }
            }
        }

        private void readModalPeriods(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        uint i = uint.Parse(Deserializer.readAttribute(child, "I"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        if (results.ModalPeriods == null)
                            results.ModalPeriods = new float[3];
                        results.ModalPeriods[i] = float.Parse(Deserializer.readAttribute(child, "Data"));
                    }
                }
            }
        }

        private void readModalLoadParticipation(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        string caseName = ResultsPath.Format(Deserializer.readAttribute(child, "CaseName"));

                        // Create list if it doesn't exist
                        if (!results.ModalLPR.ContainsKey(caseName))
                            results.ModalLPR.Add(caseName, new List<ModalLPRRow>());
                        List<ModalLPRRow> row = results.ModalLPR[caseName];
                        
                        // Now get the data
                        ModalLPRRow res = new ModalLPRRow(
                            Deserializer.readAttribute(child, "Item"),
                            Deserializer.readAttribute(child, "Type"),
                            float.Parse(Deserializer.readAttribute(child, "StaticVal")),
                            float.Parse(Deserializer.readAttribute(child, "DynamicVal")));
                        row.Add(res);
                    }
                }
            }
        }

        private void readModalParticipationMass(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case"));
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        if (results.ModalPMR == null)
                            results.ModalPMR = new float[13];
                        results.ModalPMR[i] = float.Parse(Deserializer.readAttribute(child, "Data"));
                    }
                }
            }
        }

        private void readModalParticipationFactors(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        uint i = uint.Parse(Deserializer.readAttribute(child, "I"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        if (results.ModalPF == null)
                            results.ModalPF = new float[9];
                        results.ModalPF[i] = float.Parse(Deserializer.readAttribute(child, "Data"));
                    }
                }
            }
        }

        private void readResponseSpectrumModalInfo(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "I"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                        if (results.ResponseSpectrumMI == null)
                            results.ResponseSpectrumMI = new float[xml.ChildNodes.Count, str.Length];
                        for (int k = 0; k < str.Length; k++)
                            results.ResponseSpectrumMI[i, k] = float.Parse(str[k]);
                    }
                }
            }
        }

        #region Design
        private void readDesignSteelSummary(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "id", results.ActiveCase.Name));
                        results.ActiveCase = results.ResultsCases[rCase];
                        SteelDesignSummary res = results.DesignSteelSummary[i];

                        res.DesignData = Deserializer.readAttribute(child, "data").Split(comma);
                        res.ErrMsg = Deserializer.readAttribute(child, "error", res.ErrMsg);
                        res.Ratio = float.Parse(Deserializer.readAttribute(child, "ratio", res.Ratio.ToString()));
                        res.Status = Deserializer.readAttribute(child, "status", res.Status);
                        res.WarnMsg = Deserializer.readAttribute(child, "warning", res.WarnMsg);
                    }
                }
            }
        }

        private void readDesignSteelPMM(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "id", results.ActiveCase.Name));
                        results.ActiveCase = results.ResultsCases[rCase];
                        SteelDesignPMMDetails res = results.DesignSteelPMMDetails[i];

                        res.DesignData = Deserializer.readAttribute(child, "data").Split(comma);
                        res.ErrMsg = Deserializer.readAttribute(child, "error", res.ErrMsg);
                        res.Status = Deserializer.readAttribute(child, "status", res.Status);
                        res.WarnMsg = Deserializer.readAttribute(child, "warning", res.WarnMsg);

                        res.PRatio = float.Parse(Deserializer.readAttribute(child, "pratio"));
                        res.MMajRatio = float.Parse(Deserializer.readAttribute(child, "mmajratio"));
                        res.MMinRatio = float.Parse(Deserializer.readAttribute(child, "mminratio"));
                        res.TotalRatio = float.Parse(Deserializer.readAttribute(child, "totalratio"));
                    }
                }
            }

        }

        private void readDesignSteelShear(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "id"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        SteelDesignShearDetails res = results.DesignSteelShearDetails[i];

                        res.DesignData = Deserializer.readAttribute(child, "data").Split(comma);
                        res.ErrMsg = Deserializer.readAttribute(child, "error", res.ErrMsg);
                        res.Status = Deserializer.readAttribute(child, "status", res.Status);
                        res.WarnMsg = Deserializer.readAttribute(child, "warning", res.WarnMsg);

                        res.VMajorRatio = float.Parse(Deserializer.readAttribute(child, "vmajratio"));
                        res.VMinorRatio = float.Parse(Deserializer.readAttribute(child, "vminratio"));
                    }
                }
            }

        }

        private void readDesignConcreteColumn(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "id"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        ConcreteColumnDesign res = results.DesignConcreteColumn[i];

                        res.DesignData = Deserializer.readAttribute(child, "data").Split(comma);
                        res.ErrMsg = Deserializer.readAttribute(child, "error", res.ErrMsg);
                        res.Status = Deserializer.readAttribute(child, "status", res.Status);
                        res.WarnMsg = Deserializer.readAttribute(child, "warning", res.WarnMsg);

                        res.PMMRatio = Deserializer.readAttribute(child, "pmmratio");
                        res.PMMArea = float.Parse(Deserializer.readAttribute(child, "pmmarea"));
                        res.VMajRebar = float.Parse(Deserializer.readAttribute(child, "vmajrebar"));
                        res.VMinRebar = float.Parse(Deserializer.readAttribute(child, "vminrebar"));
                    }
                }
            }

        }

        private void readDesignConcreteBeam(XmlNode xml)
        {
            if (xml != null && xml.ChildNodes.Count > 0)
            {
                foreach (XmlNode child in xml.ChildNodes)
                {
                    if ("Data".Equals(child.Name))
                    {
                        int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                        int i = int.Parse(Deserializer.readAttribute(child, "id"));
                        results.ActiveCase = results.ResultsCases[rCase];
                        ConcreteBeamDesign res = results.DesignConcreteBeam[i];

                        res.DesignData = Deserializer.readAttribute(child, "data").Split(comma);
                        res.ErrMsg = Deserializer.readAttribute(child, "error", res.ErrMsg);
                        res.Status = Deserializer.readAttribute(child, "status", res.Status);
                        res.WarnMsg = Deserializer.readAttribute(child, "warning", res.WarnMsg);

                        res.FBotArea = float.Parse(Deserializer.readAttribute(child, "fbotarea"));
                        res.FTopArea = float.Parse(Deserializer.readAttribute(child, "ftoparea"));
                        res.VRebar = float.Parse(Deserializer.readAttribute(child, "vrebar"));
                    }
                }
            }

        }
        #endregion

        private void readDataTags(XmlNode xml, float[,] data)
        {
            foreach (XmlNode child in xml.ChildNodes)
            {
                if ("Data".Equals(child.Name))
                {
                    int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                    int i = int.Parse(Deserializer.readAttribute(child, "I"));
                    results.ActiveCase = results.ResultsCases[rCase];
                    string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                    for (int k = 0; k < str.Length; k++)
                        data[i, k] = float.Parse(str[k]);
                }
            }
        }

        private void readDataTags(XmlNode xml, float[, ,] data)
        {
            foreach (XmlNode child in xml.ChildNodes)
            {
                if ("Data".Equals(child.Name))
                {
                    int rCase = int.Parse(Deserializer.readAttribute(child, "Case")) ;
                    int i = int.Parse(Deserializer.readAttribute(child, "I"));
                    int j = int.Parse(Deserializer.readAttribute(child, "J"));
                    results.ActiveCase = results.ResultsCases[rCase];
                    string[] str = Deserializer.readAttribute(child, "Data").Split(comma);
                    for (int k = 0; k < str.Length; k++)
                        data[i, j, k] = float.Parse(str[k]);
                }
            }
        }
    }
}
