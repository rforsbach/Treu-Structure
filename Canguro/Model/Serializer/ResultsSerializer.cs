using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Canguro.Model.Results;

namespace Canguro.Model.Serializer
{
    class ResultsSerializer
    {
        private Results.Results results;
        //private List<ResultsCase> resultCases;

        public ResultsSerializer(Canguro.Model.Model model)
        {
            results = model.Results;
        }

        public void Serialize(XmlWriter xml)
        {
            xml.WriteStartElement("T-Results");
            xml.WriteAttributeString("ID", results.AnalysisID.ToString());
            xml.WriteAttributeString("Finished", results.Finished.ToString());
            if (results != null && results.AnalysisID > 0 && results.Finished)
            {
                writeResultCases(xml);
                writeResultProgress(xml);
                writeJointDisplacements(xml);
                writeJointReactions(xml);
                writeJointVelocities(xml);
                writeJointAccelerations(xml);
                writeJointMasses(xml);
                writeElementJointForces(xml);
                writeBaseReactions(xml);
                writeModalPeriods(xml);
                writeModalLoadParticipation(xml);
                writeModalParticipationMass(xml);
                writeModalParticipationFactors(xml);
                writeResponseSpectrumModalInfo(xml);
                writeDesignSteelSummary(xml);
                writeDesignSteelPMM(xml);
                writeDesignSteelShear(xml);
                writeDesignConcreteColumn(xml);
                writeDesignConcreteBeam(xml);
            }
            xml.WriteEndElement();
        }

        private void writeResultCases(XmlWriter xml)
        {
            xml.WriteStartElement("T-Result_Cases");            
            foreach (ResultsCase rc in results.ResultsCases)
                writeResultCase(xml, rc);
            xml.WriteEndElement();
        }

        private void writeResultCase(XmlWriter xml, ResultsCase rCase)
        {
            xml.WriteStartElement("Result_Case");
            xml.WriteAttributeString("ID", rCase.Id.ToString());
            xml.WriteAttributeString("FullPath", rCase.FullPath);
            xml.WriteAttributeString("IsLoaded", rCase.IsLoaded.ToString());
            xml.WriteEndElement();
        }

        private void writeResultProgress(XmlWriter xml)
        {
            DownloadProgress progress = results.Downloaded;
            System.Text.Encoding coder = System.Text.ASCIIEncoding.Default;
            xml.WriteStartElement("T-Result_Progress");
            xml.WriteAttributeString("TotalProgress", progress.TotalProgress.ToString());
            xml.WriteAttributeString("Started", progress.Started.ToString());
            xml.WriteAttributeString("Key", coder.GetString(progress.DecryptionKey));
            xml.WriteAttributeString("Vector", coder.GetString(progress.DecryptionVector));
            xml.WriteAttributeString("DesignCase", progress.Design.CaseName);
            xml.WriteAttributeString("DesignFile", progress.Design.FileName);
            xml.WriteAttributeString("DesignFinished", progress.Design.Finished.ToString());
            xml.WriteAttributeString("DesignPercentage", progress.Design.Percentage.ToString());
            xml.WriteAttributeString("Model", progress.Model);
            xml.WriteAttributeString("SumCase", progress.Summary.CaseName);
            xml.WriteAttributeString("SumFile", progress.Summary.FileName);
            xml.WriteAttributeString("SumFinished", progress.Summary.Finished.ToString());
            xml.WriteAttributeString("SumPercentage", progress.Summary.Percentage.ToString());
            foreach (DownloadProps item in progress.Items)
            {
                xml.WriteStartElement("Item");
                xml.WriteAttributeString("Case", item.CaseName);
                xml.WriteAttributeString("File", item.FileName);
                xml.WriteAttributeString("Finished", item.Finished.ToString());
                xml.WriteAttributeString("Percentage", item.Percentage.ToString());
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        private void writeJointDisplacements(XmlWriter xml)
        {
            if (results.JointDisplacements == null || results.JointDisplacements.GetLength(0) == 0)
                return;
            xml.WriteStartElement("Joint_Displacements");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                writeDataTags(xml, results.JointDisplacements);
            }
            xml.WriteEndElement();
        }

        private void writeJointReactions(XmlWriter xml)
        {
            if (results.ResultsCases == null || results.ResultsCases.Count == 0)
                return;
            xml.WriteStartElement("Joint_Reactions");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                writeDataTags(xml, results.JointReactions);
            }
            xml.WriteEndElement();
        }

        private void writeJointVelocities(XmlWriter xml)
        {
            if (results.JointVelocities == null || results.JointVelocities.GetLength(0) == 0)
                return;
            xml.WriteStartElement("Joint_Velocities_-_Relative");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                writeDataTags(xml, results.JointVelocities);
            }
            xml.WriteEndElement();
        }

        private void writeJointAccelerations(XmlWriter xml)
        {
            if (results.JointAccelerations == null || results.JointAccelerations.GetLength(0) == 0)
                return;
            xml.WriteStartElement("Joint_Accelerations_-_Relative");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                writeDataTags(xml, results.JointAccelerations);
            }
            xml.WriteEndElement();
        }

        private void writeJointMasses(XmlWriter xml)
        {
            if (results.AssembledJointMasses == null || results.AssembledJointMasses.GetLength(0) == 0)
                return;
            xml.WriteStartElement("Assembled_Joint_Masses");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                writeDataTags(xml, results.AssembledJointMasses);
            }
            xml.WriteEndElement();
        }

        private void writeElementJointForces(XmlWriter xml)
        {
            if (results.ElementJointForces == null || results.ElementJointForces.GetLength(0) == 0)
                return;
            xml.WriteStartElement("Element_Joint_Forces_-_Frames");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                writeDataTags(xml, results.ElementJointForces);
            }
            xml.WriteEndElement();
        }

        private void writeBaseReactions(XmlWriter xml)
        {
            if (results.BaseReactions == null || results.BaseReactions.Length == 0)
                return;
            xml.WriteStartElement("Base_Reactions");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                for (int i = 0; i < results.BaseReactions.Length; i++)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("I", i.ToString());
                    xml.WriteAttributeString("Data", results.BaseReactions[i].ToString());
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        private void writeModalPeriods(XmlWriter xml)
        {
            xml.WriteStartElement("Modal_Periods_And_Frequencies");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                if (results.ModalPeriods != null) {
                    for (int i = 0; i < results.ModalPeriods.Length; i++) {
                        xml.WriteStartElement("Data");
                        xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                        xml.WriteAttributeString("I", i.ToString());
                        xml.WriteAttributeString("Data", results.ModalPeriods[i].ToString());
                        xml.WriteEndElement();
                    }
                }
            }
            xml.WriteEndElement();
        }

        private void writeModalLoadParticipation(XmlWriter xml)
        {
            xml.WriteStartElement("Modal_Load_Participation_Ratios");
            if (results.ModalLPR != null && results.ModalLPR.Count > 0) {
                foreach (string caseName in results.ModalLPR.Keys) {
                    foreach (ModalLPRRow res in results.ModalLPR[caseName]) {
                        xml.WriteStartElement("Data");
                        xml.WriteAttributeString("CaseName", caseName);
                        xml.WriteAttributeString("Item", res.Item);
                        xml.WriteAttributeString("Type", res.ItemType);
                        xml.WriteAttributeString("StaticVal", res.StaticVal.ToString());
                        xml.WriteAttributeString("DynamicVal", res.DynamicVal.ToString());
                        xml.WriteEndElement();
                    }
                }
            }
            xml.WriteEndElement();
        }

        private void writeModalParticipationMass(XmlWriter xml)
        {
            xml.WriteStartElement("Modal_Participating_Mass_Ratios");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                if (results.ModalPMR != null) {
                    for (int i = 0; i < results.ModalPMR.Length; i++) {
                        xml.WriteStartElement("Data");
                        xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                        xml.WriteAttributeString("I", i.ToString());
                        xml.WriteAttributeString("Data", results.ModalPMR[i].ToString());
                        xml.WriteEndElement();
                    }
                }
            }
            xml.WriteEndElement();
        }

        private void writeModalParticipationFactors(XmlWriter xml)
        {
            xml.WriteStartElement("Modal_Participation_Factors");
            foreach (ResultsCase rCase in results.ResultsCases)
            {
                results.ActiveCase = rCase;
                if (results.ModalPF != null) {
                    for (int i = 0; i < results.ModalPF.Length; i++) {
                        xml.WriteStartElement("Data");
                        xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                        xml.WriteAttributeString("I", i.ToString());
                        xml.WriteAttributeString("Data", results.ModalPF[i].ToString());
                        xml.WriteEndElement();
                    }
                }
            }
            xml.WriteEndElement();
        }

        private void writeResponseSpectrumModalInfo(XmlWriter xml)
        {
            if (results.ResponseSpectrumMI == null || results.ResponseSpectrumMI.GetLength(0) == 0)
                return;
            xml.WriteStartElement("Response_Spectrum_Modal_Information");
                writeDataTags(xml, results.ResponseSpectrumMI);
            xml.WriteEndElement();
        }

#region Design
        private void writeDesignSteelSummary(XmlWriter xml)
        {
            xml.WriteStartElement("T-Design_Steel_Summary");
            for (int i = 0; i < results.DesignSteelSummary.Length; i++)
            {
                SteelDesignSummary res = results.DesignSteelSummary[i];
                if (!string.IsNullOrEmpty(res.Status) && res.DesignData != null)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("id", i.ToString());
                    xml.WriteAttributeString("data", string.Join(",", res.DesignData));
                    xml.WriteAttributeString("error", res.ErrMsg);
                    xml.WriteAttributeString("ratio", res.Ratio.ToString());
                    xml.WriteAttributeString("status", res.Status);
                    xml.WriteAttributeString("warning", res.WarnMsg);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        private void writeDesignSteelPMM(XmlWriter xml)
        {
            xml.WriteStartElement("T-Design_Steel_PMM");
            for (int i = 0; i < results.DesignSteelPMMDetails.Length; i++)
            {
                SteelDesignPMMDetails res = results.DesignSteelPMMDetails[i];
                if (!string.IsNullOrEmpty(res.Status) && res.DesignData != null)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("id", i.ToString());
                    xml.WriteAttributeString("data", string.Join(",", res.DesignData));
                    xml.WriteAttributeString("error", res.ErrMsg);
                    xml.WriteAttributeString("pratio", res.PRatio.ToString());
                    xml.WriteAttributeString("mmajratio", res.MMajRatio.ToString());
                    xml.WriteAttributeString("mminratio", res.MMinRatio.ToString());
                    xml.WriteAttributeString("totalratio", res.TotalRatio.ToString());
                    xml.WriteAttributeString("status", res.Status);
                    xml.WriteAttributeString("warning", res.WarnMsg);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        private void writeDesignSteelShear(XmlWriter xml)
        {
            xml.WriteStartElement("T-Design_Steel_Shear");
            for (int i = 0; i < results.DesignSteelShearDetails.Length; i++)
            {
                SteelDesignShearDetails res = results.DesignSteelShearDetails[i];
                if (!string.IsNullOrEmpty(res.Status) && res.DesignData != null)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("id", i.ToString());
                    xml.WriteAttributeString("data", string.Join(",", res.DesignData));
                    xml.WriteAttributeString("error", res.ErrMsg);
                    xml.WriteAttributeString("vmajratio", res.VMajorRatio.ToString());
                    xml.WriteAttributeString("vminratio", res.VMinorRatio.ToString());
                    xml.WriteAttributeString("status", res.Status);
                    xml.WriteAttributeString("warning", res.WarnMsg);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        private void writeDesignConcreteColumn(XmlWriter xml)
        {
            xml.WriteStartElement("T-Design_Concrete_Column");
            for (int i = 0; i < results.DesignConcreteColumn.Length; i++)
            {
                ConcreteColumnDesign res = results.DesignConcreteColumn[i];
                if (res.DesignData != null)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("id", i.ToString());
                    xml.WriteAttributeString("data", string.Join(",", res.DesignData));
                    xml.WriteAttributeString("error", res.ErrMsg);
                    xml.WriteAttributeString("pmmratio", res.PMMRatio.ToString());
                    xml.WriteAttributeString("pmmarea", res.PMMArea.ToString());
                    xml.WriteAttributeString("vmajrebar", res.VMajRebar.ToString());
                    xml.WriteAttributeString("vminrebar", res.VMinRebar.ToString());
                    xml.WriteAttributeString("status", res.Status);
                    xml.WriteAttributeString("warning", res.WarnMsg);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        private void writeDesignConcreteBeam(XmlWriter xml)
        {
            xml.WriteStartElement("T-Design_Concrete_Beam");
            for (int i = 0; i < results.DesignConcreteBeam.Length; i++)
            {
                ConcreteBeamDesign res = results.DesignConcreteBeam[i];
                if (res.FBotArea > 0)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("id", i.ToString());
                    xml.WriteAttributeString("data", (res.DesignData == null) ? "" : string.Join(",", res.DesignData));
                    xml.WriteAttributeString("error", res.ErrMsg);
                    xml.WriteAttributeString("fbotarea", res.FBotArea.ToString());
                    xml.WriteAttributeString("ftoparea", res.FTopArea.ToString());
                    xml.WriteAttributeString("vrebar", res.VRebar.ToString());
                    xml.WriteAttributeString("status", res.Status);
                    xml.WriteAttributeString("warning", res.WarnMsg);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }
#endregion

        private void writeDataTags(XmlWriter xml, float[,] data)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                xml.WriteStartElement("Data");
                xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                xml.WriteAttributeString("I", i.ToString());
                xml.WriteAttributeString("Data", getDataString(data, i));
                xml.WriteEndElement();
            }
        }

        private void writeDataTags(XmlWriter xml, float[,,] data)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteAttributeString("Case", results.ActiveCase.Id.ToString());
                    xml.WriteAttributeString("I", i.ToString());
                    xml.WriteAttributeString("J", j.ToString());
                    xml.WriteAttributeString("Data", getDataString(data, i, j));
                    xml.WriteEndElement();
                }
            }
        }

        private string getDataString(float[,] data, int index)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < data.GetLength(1); i++)
                str.Append("," + data[index, i]);
            if (str.Length > 0)
                return str.Remove(0, 1).ToString();
            else
                return "";
        }

        private string getDataString(float[,,] data, int index, int index2)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < data.GetLength(2); i++)
                str.Append("," + data[index, index2, i]);
            if (str.Length > 0)
                return str.Remove(0, 1).ToString();
            else
                return "";
        }

        // Specific Design Codes (Commented)
        //    private void writeDesignLrfd99Summary(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 1 - Summary Data - AISC-Lrfd99");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelSummary)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignLrfd99PMM(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 2 - PMM Details - AISC-Lrfd99");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelPMMDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignLrfd99Shear(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 3 - Shear Details - AISC-Lrfd99");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelShearDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignAsd01Summary(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 1 - Summary Data - AISC-Asd01");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelSummary)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignAsd01PMM(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 2 - PMM Details - AISC-Asd01");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelPMMDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignAsd01Shear(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 3 - Shear Details - AISC-Asd01");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelShearDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97AsdSummary(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 1 - Summary Data - Ubc97-Asd");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelSummary)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97AsdPMM(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 2 - PMM Details - Ubc97-Asd");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelPMMDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97AsdShear(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 3 - Shear Details - Ubc97-Asd");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelShearDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignRcdfColumn(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Concrete Design 1 - Column Summary Data - Mexican Rcdf 2001");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignConcreteColumn)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignRcdfBeam(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Concrete Design 2 - Beam Summary Data - Mexican Rcdf 2001");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignConcreteBeam)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97Column(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Concrete Design 1 - Column Summary Data - Ubc97");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignConcreteColumn)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97Beam(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Concrete Design 2 - Beam Summary Data - Ubc97");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignConcreteBeam)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97LrfdSummary(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 1 - Summary Data - Ubc97-Lrfd");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelSummary)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97LrfdPMM(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 2 - PMM Details - Ubc97-Lrfd");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelPMMDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignUbc97LrfdShear(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Steel Design 3 - Shear Details - Ubc97-Lrfd");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignSteelShearDetails)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignACIColumn(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Concrete Design 1 - Column Summary Data - ACI 318-02");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignConcreteColumn)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }

        //    private void writeDesignACIBeam(XmlWriter xml)
        //    {
        //        xml.WriteStartElement("Concrete Design 2 - Beam Summary Data - ACI 318-02");
        //        foreach (ResultsCase rCase in resultCases)
        //        {
        //            results.ActiveCase = rCase;
        //            foreach (float[,] res in results.DesignConcreteBeam)
        //            {
        //            }
        //        }
        //        xml.WriteEndElement();
        //    }
    }
}
