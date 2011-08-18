using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Load;
using CrystalDecisions.CrystalReports.Engine;

namespace Canguro.View.Reports
{
    public partial class ReportsWindow : Form
    {
        private Model.Model model;
        Canguro.Controller.CommandServices services;
        List<ReportOptions> reportOptions;
        bool reportSelected;

        List<ReportData> joints = new List<ReportData>();
        List<ReportData> lines = new List<ReportData>();
        List<ReportData> loadCases = new List<ReportData>();
        List<ReportData> jointForces = new List<ReportData>();
        List<ReportData> groundDisplacements = new List<ReportData>();
        List<ReportData> lineConcentratedLoads = new List<ReportData>();
        List<ReportData> lineDistributedLoads = new List<ReportData>();
        List<ReportData> materials = new List<ReportData>();
        List<ReportData> sections = new List<ReportData>();
        List<ReportData> jointDisplacements = new List<ReportData>();
        List<ReportData> jointReactions = new List<ReportData>();
        List<ReportData> jointAccelerations = new List<ReportData>();
        List<ReportData> jointVelocities = new List<ReportData>();
        List<ReportData> baseReactions = new List<ReportData>();
        List<ReportData> elementJointForces = new List<ReportData>();
        List<ReportData> constraints = null; // Created by wrapper.
        List<ReportData> assembledJointMasses = new List<ReportData>();
        List<ReportData> modalLPR = new List<ReportData>();
        List<ReportData> modalPMR = new List<ReportData>();
        List<ReportData> modalPF = new List<ReportData>();
        List<ReportData> modalPeriods = new List<ReportData>();
        List<ReportData> responseSpectrumInfo = new List<ReportData>();
        List<ReportData> designOptions = new List<ReportData>();
        List<ReportData> steelDesign = new List<ReportData>();
        List<ReportData> steelDesignPMM = new List<ReportData>();
        List<ReportData> steelDesignShear = new List<ReportData>();
        List<ReportData> concreteBeamDesign = new List<ReportData>();
        List<ReportData> concreteColumnDesign = new List<ReportData>();
        List<ReportData> materialAmounts = new List<ReportData>();

        private static bool isOpen = false;
        public static bool IsOpen
        {
            get { return isOpen; }
        }

        private void ReportsWindow_Load(object sender, EventArgs e)
        {
            isOpen = true;
        }

        private void ReportsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            isOpen = false;
            disposeReports();
            model.ChangeModel();
        }

        private void disposeReports()
        {
            joints = null;
            lines = null;
            loadCases = null;
            jointForces = null;
            groundDisplacements = null;
            lineConcentratedLoads = null;
            lineDistributedLoads = null;
            materials = null;
            sections = null;
            jointDisplacements = null;
            jointReactions = null;
            jointAccelerations = null;
            jointVelocities = null;
            baseReactions = null;
            elementJointForces = null;
            assembledJointMasses = null;
            modalLPR = null;
            modalPMR = null;
            modalPF = null;
            modalPeriods = null;
            responseSpectrumInfo = null;
            designOptions = null;
            steelDesign = null;
            steelDesignPMM = null;
            steelDesignShear = null;
            concreteBeamDesign = null;
            concreteColumnDesign = null;
            reportOptions = null;
            materialAmounts = null;

            crystalReportViewer1.Dispose();
            DataReport report = crystalReportViewer1.ReportSource as DataReport;
            List<byte> empty = new List<byte>();
            if (report != null)
            {
                foreach (ReportDocument sub in report.Subreports)
                    if (sub != null)
                    {
                        //sub.SetDataSource(empty);
                        sub.Close();
                        sub.Dispose();
                    }
                report.Close();
                report.Dispose();
            }

            Dispose();
        }

        public ReportsWindow(Canguro.Controller.CommandServices services, List<ReportOptions> options, bool onlySelected)
        {
            this.services = services;
            this.model = services.Model;
            reportOptions = options;
            reportSelected = onlySelected;
            InitializeComponent();

            services.ReportProgress(10);
            if (reportOptions.Contains(ReportOptions.Joints))
                getJoints();
            services.ReportProgress(15);
            if (reportOptions.Contains(ReportOptions.Lines))
                getLines();
            services.ReportProgress(20);
            if (reportOptions.Contains(ReportOptions.Constraints))
                constraints = ConstraintWrapper.GetAllConstraints(model);
            if (reportOptions.Contains(ReportOptions.LoadCases))
                getLoadCases();
            services.ReportProgress(25);
            if (reportOptions.Contains(ReportOptions.JointForces) || reportOptions.Contains(ReportOptions.GroundDisplacements))
                getJointLoads();
            services.ReportProgress(30);
            if (reportOptions.Contains(ReportOptions.LineConcentratedForces) || reportOptions.Contains(ReportOptions.LineDistributedLoads))
                getLineLoads();
            services.ReportProgress(35);
            getMaterials();
            getSections();
            getJointDisplacements();
            services.ReportProgress(40);
            getJointMasses(model.Results);
            getDesignOptions();
            getDesignResults();
            services.ReportProgress(45);
        }
        List<int> subReportLengths;

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            try
            {
                DataReport report = new DataReport();
                //                report.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\RuntimeData\\GeneralReport.rpt");
                subReportLengths = new List<int>();
                int i = 0;
                if (reportOptions.Contains(ReportOptions.Joints) && joints.Count > 0)
                    formatSubreport(report.Subreports[i++], joints, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.Lines) && lines.Count > 0)
                    formatSubreport(report.Subreports[i++], lines, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.Constraints) && constraints != null && constraints.Count > 0)
                    formatSubreport(report.Subreports[i++], constraints, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.LoadCases) && loadCases.Count > 0)
                    formatSubreport(report.Subreports[i++], loadCases, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.JointForces) && jointForces.Count > 0)
                    formatSubreport(report.Subreports[i++], jointForces, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.GroundDisplacements) && groundDisplacements.Count > 0)
                    formatSubreport(report.Subreports[i++], groundDisplacements, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.LineConcentratedForces) && lineConcentratedLoads.Count > 0)
                    formatSubreport(report.Subreports[i++], lineConcentratedLoads, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.LineDistributedLoads) && lineDistributedLoads.Count > 0)
                    formatSubreport(report.Subreports[i++], lineDistributedLoads, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.Materials) && materials.Count > 0)
                    formatSubreport(report.Subreports[i++], materials, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.MaterialAmounts) && materialAmounts != null && materialAmounts.Count > 0)
                    formatSubreport(report.Subreports[i++], materialAmounts, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.Sections) && sections.Count > 0)
                    formatSubreport(report.Subreports[i++], sections, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.JointDisplacements) && jointDisplacements.Count > 0)
                    formatSubreport(report.Subreports[i++], jointDisplacements, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.JointAccelerations) && jointAccelerations.Count > 0)
                    formatSubreport(report.Subreports[i++], jointAccelerations, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.JointVelocities) && jointVelocities.Count > 0)
                    formatSubreport(report.Subreports[i++], jointVelocities, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.BaseReactions) && baseReactions.Count > 0)
                    formatSubreport(report.Subreports[i++], baseReactions, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.ElementJointForces) && elementJointForces.Count > 0)
                    formatSubreport(report.Subreports[i++], elementJointForces, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.AssembledJointMasses) && assembledJointMasses.Count > 0)
                    formatSubreport(report.Subreports[i++], assembledJointMasses, 50 + 50 * i / reportOptions.Count);
                // Begin modal options
                if (reportOptions.Contains(ReportOptions.ModalLoadParticipationRatios) && modalLPR.Count > 0)
                    formatSubreport(report.Subreports[i++], modalLPR, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.ModalParticipatingMassRatios) && modalPMR.Count > 0)
                    formatSubreport(report.Subreports[i++], modalPMR, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.ModalParticipationFactors) && modalPF.Count > 0)
                    formatSubreport(report.Subreports[i++], modalPF, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.ModalPeriodsAndFrequencies) && modalPeriods.Count > 0)
                    formatSubreport(report.Subreports[i++], modalPeriods, 50 + 50 * i / reportOptions.Count);
                // End modal options
                if (reportOptions.Contains(ReportOptions.ResponseSpectrumInfo) && responseSpectrumInfo.Count > 0)
                    formatSubreport(report.Subreports[i++], responseSpectrumInfo, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.JointReactions) && jointReactions.Count > 0)
                    formatSubreport(report.Subreports[i++], jointReactions, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.DesignOptions) && designOptions.Count > 0)
                    formatSubreport(report.Subreports[i++], designOptions, 50 + 50 * i / reportOptions.Count);
                // Begin design options
                if (reportOptions.Contains(ReportOptions.SteelDesign) && steelDesign.Count > 0)
                    formatSubreport(report.Subreports[i++], steelDesign, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.SteelDesignPMM) && steelDesignPMM.Count > 0)
                    formatSubreport(report.Subreports[i++], steelDesignPMM, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.SteelDesignShear) && steelDesignShear.Count > 0)
                    formatSubreport(report.Subreports[i++], steelDesignShear, 50 + 50 * i / reportOptions.Count);
                // End design options
                if (reportOptions.Contains(ReportOptions.ConcreteColumnDesign) && concreteColumnDesign.Count > 0)
                    formatSubreport(report.Subreports[i++], concreteColumnDesign, 50 + 50 * i / reportOptions.Count);
                if (reportOptions.Contains(ReportOptions.ConcreteBeamDesign) && concreteBeamDesign.Count > 0)
                    formatSubreport(report.Subreports[i++], concreteBeamDesign, 50 + 50 * i / reportOptions.Count);

                for (int j = i + 3; j < report.ReportDefinition.Sections.Count - 2; j++)
                    report.ReportDefinition.Sections[j].SectionFormat.EnableSuppress = true;

                //for (; i < report.Subreports.Count; i++)
                //{
                //    foreach (Section sec in report.Subreports[i].ReportDefinition.Sections)
                //        sec.SectionFormat.EnableSuppress = true;
                //}

                headerGenerator(report);
                tocGenerator(report);
                crystalReportViewer1.ReportSource = report;
                services.ReportProgress(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private const int margin = 100;
        private const int padding = 100;

        private void formatSubreport(ReportDocument report, List<ReportData> list, int progress)
        {
            if (list != null && list.Count > 0)
            {
                subReportLengths.Add(list.Count);
                report.SetDataSource(list);
                ReportData sample = list[0];
                List<PropertyDescriptor> properties = sample.Properties;
                int count = (properties.Count > 10) ? 10 : properties.Count;
                TextObject title = report.ReportDefinition.ReportObjects["TitleText"] as TextObject;
                if (title != null)
                    title.Text = Culture.Get(sample.GetType().ToString().Replace(".", "")).ToUpper();
                FieldObject data;
                int left = margin;
                for (int i = 0; i < 10; i++)
                {
                    string name = string.Format("Text{0}", i + 1);
                    title = report.ReportDefinition.ReportObjects[name] as TextObject;
                    name = string.Format("Data{0}1", i + 1);
                    data = report.ReportDefinition.ReportObjects[name] as FieldObject;

                    if (i < count)
                    {
                        Model.ModelAttributes.GridPositionAttribute position =
                            ((Model.ModelAttributes.GridPositionAttribute)properties[i].Attributes[typeof(Model.ModelAttributes.GridPositionAttribute)]);

                        if (title != null)
                        {
                            title.Text = properties[i].DisplayName;
                            title.Left = left;
                            title.Width = position.Width;
                        }
                        if (data != null)
                        {
                            data.Left = left;
                            data.Width = position.Width;
                        }
                        left += position.Width + padding;
                    }
                    else
                    {
                        if (title != null)
                        {
                            title.Left = 0;
                            title.Width = 0;
                            title.Text = "";
                        }
                        if (data != null)
                        {
                            data.Left = 0;
                            data.Width = 0;
                        }
                    }
                }
            }
            services.ReportProgress((uint)progress);
        }

        /// <summary>
        /// Build a report header
        /// </summary>
        /// <param name="report"></param>
        private void headerGenerator(GeneralReport report)
        {
            TextObject title = report.ReportDefinition.ReportObjects["ReportFileNameTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportFileNameText"] as TextObject;
            if (title != null)
                title.Text = System.IO.Path.GetFileNameWithoutExtension(model.CurrentPath).ToUpper();

            title = report.ReportDefinition.ReportObjects["ReportDateTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportUnitSystemTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportUnitSystemText"] as TextObject;
            if (title != null)
                title.Text = model.UnitSystem.Name;
        }

        /// <summary>
        /// Build a report header
        /// </summary>
        /// <param name="report"></param>
        private void headerGenerator(DataReport report)
        {
            TextObject title = report.ReportDefinition.ReportObjects["ReportFileNameTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportFileNameText"] as TextObject;
            if (title != null)
                title.Text = (string.IsNullOrEmpty(model.CurrentPath)) ? Culture.Get("defaultModelName") :
                  System.IO.Path.GetFileName(model.CurrentPath);

            title = report.ReportDefinition.ReportObjects["ReportDateTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportUnitSystemTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportUnitSystemText"] as TextObject;
            if (title != null)
                title.Text = model.UnitSystem.Name;

            title = report.ReportDefinition.ReportObjects["ReportAuthorTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            title = report.ReportDefinition.ReportObjects["ReportAuthorText"] as TextObject;
            if (title != null)
                title.Text = model.Summary.Author.Equals("") ? "" : model.Summary.Author;

            title = report.ReportDefinition.ReportObjects["ReportNameTitleText"] as TextObject;
            if (title != null)
                title.Text = model.Summary.Title.Equals("") ? title.Text : model.Summary.Title;

        }
        /// <summary>
        /// Build a ToC
        /// </summary>
        /// <param name="report"></param>
        private void tocGenerator(DataReport report)
        {
            TextObject title = report.ReportDefinition.ReportObjects["ReportToCTitleText"] as TextObject;
            if (title != null)
                title.Text = Culture.Get(title.Name);

            FormulaFieldDefinition formula = report.DataDefinition.FormulaFields["FormulaIndex"] as FormulaFieldDefinition;
            FormulaFieldDefinition lines = report.DataDefinition.FormulaFields["FormulaLines"] as FormulaFieldDefinition;

            int numOfpage = 2;
            int availableLines = 62;

            if (formula != null && lines != null)
            {
                StringBuilder text = new StringBuilder();
                StringBuilder numbers = new StringBuilder();
                text.Append("'");

                numbers.Append("'");
                for (int subreport = 0; subreport < reportOptions.Count; )
                {
                    string label = ((TextObject)report.Subreports[subreport].ReportDefinition.ReportObjects["TitleText"]).Text;
                    if (label.Equals("Title"))
                    {
                        numOfpage = subreport;
                        break;
                    }
                    int length = subReportLengths[subreport] + 5;

                    if (length < availableLines)
                    {
                        numbers.Append("   " + numOfpage.ToString() + " ' + Chr(10) + '");
                        availableLines = availableLines - length;
                    }
                    else
                    {
                        if (availableLines != 62)
                        {
                            numOfpage += 1;
                            numbers.Append("   " + numOfpage.ToString() + " ' + Chr(10) + '");
                            availableLines = 62;
                        }
                        else
                            numbers.Append("   " + numOfpage.ToString() + " ' + Chr(10) + '");

                        while (length > 62)
                        {
                            length -= 62;
                            numOfpage++;
                        }
                        availableLines -= length;
                    }
                    //Chr(10) brakes lines in crystal reports
                    label = label.PadRight(170, '.').Replace("..", ". ");
                    text.Append(label + "' + Chr(10) + '");
                    subreport++;
                }
                text.Append("'");
                numbers.Append("'");
                formula.Text = text.ToString();
                lines.Text = numbers.ToString();
            }
        }

        private void getJoints()
        {
            joints.Clear();
            foreach (Joint j in model.JointList)
                if (j != null && (!reportSelected || j.IsSelected))
                    joints.Add(new JointWrapper(j));
        }

        private void getLines()
        {
            lines.Clear();
            foreach (LineElement l in model.LineList)
                if (l != null && (!reportSelected || l.IsSelected))
                    lines.Add(new LineWrapper(l));
        }

        private void getLoadCases()
        {
            foreach (LoadCase lCase in model.LoadCases.Values)
                if (lCase != null)
                    loadCases.Add(new LoadCaseWrapper(lCase));
        }

        private void getJointLoads()
        {
            foreach (LoadCase lCase in model.LoadCases.Values)
                if (lCase != null)
                    foreach (Joint joint in model.JointList)
                        if (joint != null && joint.Loads != null && joint.Loads[lCase] != null && (!reportSelected || joint.IsSelected))
                            foreach (Canguro.Model.Load.Load load in joint.Loads[lCase])
                            {
                                if (load is Canguro.Model.Load.ForceLoad)
                                    jointForces.Add(new JointForcesWrapper((Canguro.Model.Load.ForceLoad)load, joint, lCase));
                                else if (load is Canguro.Model.Load.GroundDisplacementLoad)
                                    groundDisplacements.Add(new GroundDisplacementWrapper((Canguro.Model.Load.GroundDisplacementLoad)load, joint, lCase));
                            }
        }

        private void getLineLoads()
        {
            foreach (LoadCase lCase in model.LoadCases.Values)
                if (lCase != null)
                    foreach (LineElement line in model.LineList)
                        if (line != null && line.Loads != null && line.Loads[lCase] != null && (!reportSelected || line.IsSelected))
                            foreach (Canguro.Model.Load.Load load in line.Loads[lCase])
                            {
                                if (load is Canguro.Model.Load.ConcentratedSpanLoad)
                                    lineConcentratedLoads.Add(new LineConcentratedForcesWrapper((Canguro.Model.Load.ConcentratedSpanLoad)load, line, lCase));
                                else if (load is Canguro.Model.Load.DistributedSpanLoad)
                                    lineDistributedLoads.Add(new LineDistributedForcesWrapper((Canguro.Model.Load.DistributedSpanLoad)load, line, lCase));
                            }
        }

        private void getMaterials()
        {
            if (reportOptions.Contains(ReportOptions.MaterialAmounts))
                materialAmounts = MaterialAmountWrapper.GetAmountsPerSection(model);
        }

        private void getSections()
        {
            List<Canguro.Model.Material.Material> mList = new List<Canguro.Model.Material.Material>();
            List<Canguro.Model.Section.FrameSection> sList = new List<Canguro.Model.Section.FrameSection>();
            foreach (LineElement line in model.LineList)
                if (line != null && line.Properties is StraightFrameProps && (!reportSelected || line.IsSelected))
                    if (!sList.Contains(((StraightFrameProps)line.Properties).Section))
                        sList.Add(((StraightFrameProps)line.Properties).Section);
            foreach (Canguro.Model.Section.FrameSection sec in sList)
                if (!mList.Contains(sec.Material))
                    mList.Add(sec.Material);

            if (reportOptions.Contains(ReportOptions.Sections))
                foreach (Canguro.Model.Section.FrameSection sec in sList)
                    sections.Add(new SectionWrapper(sec));

            if (reportOptions.Contains(ReportOptions.Materials))
                foreach (Canguro.Model.Material.Material mat in mList)
                    materials.Add(new MaterialWrapper(mat));
        }


        private void getJointDisplacements()
        {
            Canguro.Model.Results.Results results = model.Results;

            if (results != null && results.AnalysisID > 0 && results.JointDisplacements != null)
                foreach (Model.Results.ResultsCase rCase in results.ResultsCases)
                    if (rCase != null)
                        getResults(rCase, results);

            bool addModalLPR = reportOptions.Contains(ReportOptions.ModalLoadParticipationRatios);
            if (results.ModalLPR != null && addModalLPR)
                for (int i = 0; i < 3; i++)
                    modalLPR.Add(new ModalLPRWrapper(results, i));
        }

        private void getResults(Model.Results.ResultsCase rCase, Model.Results.Results results)
        {
            if (rCase == null) return;
            results.ActiveCase = rCase;
            float[,] d = results.JointDisplacements;
            bool addDisplacements = reportOptions.Contains(ReportOptions.JointDisplacements);
            bool addReactions = reportOptions.Contains(ReportOptions.JointReactions);
            bool addAccel = reportOptions.Contains(ReportOptions.JointAccelerations);
            bool addVelocities = reportOptions.Contains(ReportOptions.JointVelocities);
            bool addElementJointForces = reportOptions.Contains(ReportOptions.ElementJointForces);
            bool addModalPMR = reportOptions.Contains(ReportOptions.ModalParticipatingMassRatios);
            bool addModalPF = reportOptions.Contains(ReportOptions.ModalParticipationFactors);
            bool addModalPeriods = reportOptions.Contains(ReportOptions.ModalPeriodsAndFrequencies);

            foreach (Joint j in model.JointList)
                if (j != null && (!reportSelected || j.IsSelected))
                {
                    if (addDisplacements)
                        jointDisplacements.Add(new JointDisplacementsWrapper(j.Id, results));
                    if (j.DoF.IsRestrained && addReactions)
                        jointReactions.Add(new JointReactionsWrapper(j, results));
                    if (results.JointAccelerations.GetLength(0) > j.Id && addAccel)
                        jointAccelerations.Add(new JointAccelerationsWrapper(j.Id, results));
                    if (results.JointVelocities.GetLength(0) > j.Id && addVelocities)
                        jointVelocities.Add(new JointVelocitiesWrapper(j.Id, results));

                }
            foreach (LineElement line in model.LineList)
                if (line != null && addElementJointForces && (!reportSelected || line.IsSelected))
                {
                    elementJointForces.Add(new ElementJointForcesWrapper(line, line.I, results));
                    elementJointForces.Add(new ElementJointForcesWrapper(line, line.J, results));
                }
            if (results.BaseReactions != null && reportOptions.Contains(ReportOptions.BaseReactions))
                baseReactions.Add(new BaseReactionsWrapper(results));
            if (results.ResponseSpectrumMI != null && reportOptions.Contains(ReportOptions.ResponseSpectrumInfo))
                responseSpectrumInfo.Add(new ResponseSpectrumMIWrapper(results));



            if (results.ModalPMR != null && addModalPMR)
                modalPMR.Add(new ModalPMRWrapper(results));

            if (results.ModalPF != null && addModalPF)
                modalPF.Add(new ModalPFWrapper(results));

            if (results.ModalPeriods != null && addModalPeriods)
                modalPeriods.Add(new ModalPeriodsWrapper(results));
        }

        private void getJointMasses(Model.Results.Results results)
        {
            if (results.AssembledJointMasses != null && reportOptions.Contains(ReportOptions.AssembledJointMasses))
                foreach (Joint j in model.JointList)
                    if (j != null && (!reportSelected || j.IsSelected))
                        assembledJointMasses.Add(new AssembledJointMassesWrapper(j, results));
        }

        private void getDesignOptions()
        {
            if (reportOptions.Contains(ReportOptions.DesignOptions))
            {
                List<Model.Design.DesignOptions> design = new List<Canguro.Model.Design.DesignOptions>();
                if (model.SteelDesignOptions is Model.Design.SteelDesignOptions)
                    design.Add(model.SteelDesignOptions);
                if (model.ConcreteDesignOptions is Model.Design.ConcreteDesignOptions)
                    design.Add(model.ConcreteDesignOptions);

                foreach (Model.Design.DesignOptions opt in design)
                {
                    foreach (System.ComponentModel.PropertyDescriptor prop in opt.GetProperties())
                        if (prop.IsBrowsable)
                        {
                            object obj = prop.GetValue(opt);
                            if (obj is System.Collections.IEnumerable)
                            {
                                designOptions.Add(new DesignOptionsWrapper(opt, prop.DisplayName, ""));
                                foreach (object o in (System.Collections.IEnumerable)obj)
                                    designOptions.Add(new DesignOptionsWrapper(opt, "", o.ToString()));
                            }
                            else
                                designOptions.Add(new DesignOptionsWrapper(opt, prop.DisplayName, obj.ToString()));
                        }
                }
            }
        }

        private void getDesignResults()
        {
            Model.Results.Results results = model.Results;
            bool addSteel = reportOptions.Contains(ReportOptions.SteelDesign);
            bool addSteelPMM = reportOptions.Contains(ReportOptions.SteelDesignPMM);
            bool addSteelShear = reportOptions.Contains(ReportOptions.SteelDesignShear);
            bool addConcBeam = reportOptions.Contains(ReportOptions.ConcreteBeamDesign);
            bool addConcCol = reportOptions.Contains(ReportOptions.ConcreteColumnDesign);
            bool addDesign = addConcBeam || addConcCol || addSteel || addSteelPMM || addSteelShear;

            if (results != null && results.AnalysisID != 0 && addDesign)
            {
                foreach (LineElement element in model.LineList)
                {
                    if (element != null && element.Properties is StraightFrameProps && (!reportSelected || element.IsSelected))
                    {
                        StraightFrameProps frame = (StraightFrameProps)element.Properties;
                        if (addSteel && frame.Section.Material.DesignProperties is Model.Material.SteelDesignProps &&
                            results.DesignSteelSummary != null && results.DesignSteelSummary[element.Id] != null)
                            steelDesign.Add(new SteelDesignWrapper(element, results.DesignSteelSummary[element.Id]));

                        if (addSteelPMM && results.DesignSteelPMMDetails != null && results.DesignSteelPMMDetails.Length > element.Id &&
                            results.DesignSteelPMMDetails[element.Id] != null)
                            steelDesignPMM.Add(new SteelDesignPMMWrapper(element, results.DesignSteelPMMDetails[element.Id]));

                        if (addSteelShear && results.DesignSteelShearDetails != null && results.DesignSteelShearDetails.Length > element.Id &&
                            results.DesignSteelShearDetails[element.Id] != null)
                            steelDesignShear.Add(new SteelDesignShearWrapper(element, results.DesignSteelShearDetails[element.Id]));

                        if (frame.Section.Material.DesignProperties is Model.Material.ConcreteDesignProps)
                        {
                            if (addConcBeam && results.DesignConcreteBeam != null && results.DesignConcreteBeam.Length > element.Id
                                && results.DesignConcreteBeam[element.Id] != null)
                                concreteBeamDesign.Add(new ConcreteBeamDesignWrapper(element, results.DesignConcreteBeam[element.Id]));
                            if (addConcCol && results.DesignConcreteColumn != null && results.DesignConcreteColumn.Length > element.Id
                                && results.DesignConcreteColumn[element.Id] != null)
                                concreteColumnDesign.Add(new ConcreteColumnDesignWrapper(element, results.DesignConcreteColumn[element.Id]));
                        }
                    }
                }
            }
        }

        private void ReportsWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Space)
            {
                crystalReportViewer1.ShowNextPage();
            }
            else if (e.KeyData == Keys.Left)
            {
                crystalReportViewer1.ShowPreviousPage();
            }
            else if (e.KeyData == Keys.PageUp)
            {
                crystalReportViewer1.ShowLastPage();
            }
            else if (e.KeyData == Keys.PageDown)
            {
                crystalReportViewer1.ShowFirstPage();
            }
            else
            {
                //For futher details call me
            }
        }

        private void crystalReportViewer1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Space)
            {
                crystalReportViewer1.ShowNextPage();
            }
            else if (e.KeyData == Keys.Left)
            {
                crystalReportViewer1.ShowPreviousPage();
            }
            else if (e.KeyData == Keys.PageUp)
            {
                crystalReportViewer1.ShowLastPage();
            }
            else if (e.KeyData == Keys.PageDown)
            {
                crystalReportViewer1.ShowFirstPage();
            }
            else
            {
                //For further details call me
            }
        }

    } // class
} // namespace