using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.View.Reports;

namespace Canguro.Commands.Forms
{
    public partial class ReportOptionsDialog : Form
    {
        Dictionary<string, ReportOptions> allReports;
        Canguro.Model.Model model;

        public ReportOptionsDialog(Canguro.Model.Model model)
        {
            this.model = model;
            InitializeComponent();
            allReports = new Dictionary<string, ReportOptions>();

            allReports.Add(Culture.Get(typeof(JointWrapper).ToString().Replace(".", "")), ReportOptions.Joints);
            allReports.Add(Culture.Get(typeof(LineWrapper).ToString().Replace(".", "")), ReportOptions.Lines);
            allReports.Add(Culture.Get(typeof(LoadCaseWrapper).ToString().Replace(".", "")), ReportOptions.LoadCases);
            allReports.Add(Culture.Get(typeof(JointForcesWrapper).ToString().Replace(".", "")), ReportOptions.JointForces);
            allReports.Add(Culture.Get(typeof(GroundDisplacementWrapper).ToString().Replace(".", "")), ReportOptions.GroundDisplacements);
            allReports.Add(Culture.Get(typeof(LineConcentratedForcesWrapper).ToString().Replace(".", "")), ReportOptions.LineConcentratedForces);
            allReports.Add(Culture.Get(typeof(LineDistributedForcesWrapper).ToString().Replace(".", "")), ReportOptions.LineDistributedLoads);
            allReports.Add(Culture.Get(typeof(ConstraintWrapper).ToString().Replace(".", "")), ReportOptions.Constraints);
            allReports.Add(Culture.Get(typeof(MaterialWrapper).ToString().Replace(".", "")), ReportOptions.Materials);
            allReports.Add(Culture.Get(typeof(MaterialAmountWrapper).ToString().Replace(".", "")), ReportOptions.MaterialAmounts);
            allReports.Add(Culture.Get(typeof(SectionWrapper).ToString().Replace(".", "")), ReportOptions.Sections);
            allReports.Add(Culture.Get(typeof(JointDisplacementsWrapper).ToString().Replace(".", "")), ReportOptions.JointDisplacements);
            allReports.Add(Culture.Get(typeof(BaseReactionsWrapper).ToString().Replace(".", "")), ReportOptions.BaseReactions);
            allReports.Add(Culture.Get(typeof(ElementJointForcesWrapper).ToString().Replace(".", "")), ReportOptions.ElementJointForces);
            allReports.Add(Culture.Get(typeof(AssembledJointMassesWrapper).ToString().Replace(".", "")), ReportOptions.AssembledJointMasses);
            //Begin Modal Tables
            allReports.Add(Culture.Get(typeof(ModalLPRWrapper).ToString().Replace(".", "")), ReportOptions.ModalLoadParticipationRatios);
            allReports.Add(Culture.Get(typeof(ModalPMRWrapper).ToString().Replace(".", "")), ReportOptions.ModalParticipatingMassRatios);
            allReports.Add(Culture.Get(typeof(ModalPFWrapper).ToString().Replace(".", "")), ReportOptions.ModalParticipationFactors);
            allReports.Add(Culture.Get(typeof(ModalPeriodsWrapper).ToString().Replace(".", "")), ReportOptions.ModalPeriodsAndFrequencies);
            //End Modal Tables
            allReports.Add(Culture.Get(typeof(ResponseSpectrumMIWrapper).ToString().Replace(".", "")), ReportOptions.ResponseSpectrumInfo);
            allReports.Add(Culture.Get(typeof(JointReactionsWrapper).ToString().Replace(".", "")), ReportOptions.JointReactions);
            allReports.Add(Culture.Get(typeof(JointAccelerationsWrapper).ToString().Replace(".", "")), ReportOptions.JointAccelerations);
            allReports.Add(Culture.Get(typeof(JointVelocitiesWrapper).ToString().Replace(".", "")), ReportOptions.JointVelocities);
            //Begin Design Tables
            allReports.Add(Culture.Get(typeof(DesignOptionsWrapper).ToString().Replace(".", "")), ReportOptions.DesignOptions);
            allReports.Add(Culture.Get(typeof(SteelDesignWrapper).ToString().Replace(".", "")), ReportOptions.SteelDesign);
            allReports.Add(Culture.Get(typeof(SteelDesignPMMWrapper).ToString().Replace(".", "")), ReportOptions.SteelDesignPMM);
            allReports.Add(Culture.Get(typeof(SteelDesignShearWrapper).ToString().Replace(".", "")), ReportOptions.SteelDesignShear);
            allReports.Add(Culture.Get(typeof(ConcreteColumnDesignWrapper).ToString().Replace(".", "")), ReportOptions.ConcreteColumnDesign);
            allReports.Add(Culture.Get(typeof(ConcreteBeamDesignWrapper).ToString().Replace(".", "")), ReportOptions.ConcreteBeamDesign);
            //End Design Tables

            enableOptions();
        }

        private void enableOptions()
        {
            Dictionary<ReportOptions, bool> enabledOptions = new Dictionary<ReportOptions, bool>();

            bool notEmptyModel = (model.Summary.NumJoints > 0);
            bool resultsArrived = (model.Results != null && model.Results.Finished);
            bool haveDesignOptions = false;
            Canguro.Model.Design.DesignOptions[] desopt = new Canguro.Model.Design.DesignOptions[] { model.SteelDesignOptions, model.ConcreteDesignOptions, model.ColdFormedDesignOptions, model.AluminumDesignOptions };
            foreach (Canguro.Model.Design.DesignOptions opt in desopt)
                if (!(opt is Canguro.Model.Design.NoDesign)) {
                    haveDesignOptions = true;
                    break;
                }
            bool steelDesign = model.SteelDesignOptions.ToString().Contains("Sin") || model.SteelDesignOptions.ToString().Contains("No") ? false : true;
            bool concreteDesign = model.ConcreteDesignOptions.ToString().Contains("Sin") || model.ConcreteDesignOptions.ToString().Contains("No") ? false : true;

            enabledOptions.Add(ReportOptions.Joints, notEmptyModel);
            enabledOptions.Add(ReportOptions.Lines, notEmptyModel);
            enabledOptions.Add(ReportOptions.LoadCases, model.LoadCases.Count > 0);
            enabledOptions.Add(ReportOptions.JointForces, notEmptyModel);
            enabledOptions.Add(ReportOptions.GroundDisplacements, notEmptyModel);
            enabledOptions.Add(ReportOptions.LineConcentratedForces, notEmptyModel);
            enabledOptions.Add(ReportOptions.LineDistributedLoads, notEmptyModel);
            enabledOptions.Add(ReportOptions.Materials, notEmptyModel);
            enabledOptions.Add(ReportOptions.Constraints, model.ConstraintList.Count > 0);
            enabledOptions.Add(ReportOptions.MaterialAmounts, notEmptyModel);
            enabledOptions.Add(ReportOptions.Sections, notEmptyModel);
            enabledOptions.Add(ReportOptions.JointDisplacements, resultsArrived);
            enabledOptions.Add(ReportOptions.BaseReactions, resultsArrived);
            enabledOptions.Add(ReportOptions.ElementJointForces, resultsArrived);
            enabledOptions.Add(ReportOptions.AssembledJointMasses, resultsArrived);
            // Begin modal tables
            enabledOptions.Add(ReportOptions.ModalLoadParticipationRatios, model.Results.ModalLPR != null && model.Results.ModalLPR.Count > 0);
            enabledOptions.Add(ReportOptions.ModalParticipatingMassRatios, model.Results.ModalLPR != null && model.Results.ModalLPR.Count > 0);
            enabledOptions.Add(ReportOptions.ModalParticipationFactors, model.Results.ModalLPR != null && model.Results.ModalLPR.Count > 0);
            enabledOptions.Add(ReportOptions.ModalPeriodsAndFrequencies, model.Results.ModalLPR != null && model.Results.ModalLPR.Count > 0);
            // End modal tables
            enabledOptions.Add(ReportOptions.ResponseSpectrumInfo, model.Results.ResponseSpectrumMI != null);
            enabledOptions.Add(ReportOptions.JointReactions, resultsArrived);
            enabledOptions.Add(ReportOptions.JointAccelerations, model.Results.JointAccelerations != null);
            enabledOptions.Add(ReportOptions.JointVelocities, model.Results.JointVelocities != null);
            // Begin design
            enabledOptions.Add(ReportOptions.DesignOptions, haveDesignOptions);
            enabledOptions.Add(ReportOptions.SteelDesign, model.Results.DesignSteelSummary != null && model.Results.DesignSteelSummary.Length > 0 && steelDesign);
            enabledOptions.Add(ReportOptions.SteelDesignPMM, model.Results.DesignSteelPMMDetails != null && model.Results.DesignSteelPMMDetails.Length > 0 && steelDesign);
            enabledOptions.Add(ReportOptions.SteelDesignShear, model.Results.DesignSteelShearDetails != null && model.Results.DesignSteelShearDetails.Length > 0 && steelDesign);
            enabledOptions.Add(ReportOptions.ConcreteColumnDesign, model.Results.DesignConcreteColumn != null && model.Results.DesignConcreteColumn.Length > 0 && concreteDesign);
            enabledOptions.Add(ReportOptions.ConcreteBeamDesign, model.Results.DesignConcreteBeam != null && model.Results.DesignConcreteBeam.Length > 0 && concreteDesign);

            foreach (string str in allReports.Keys)
                if (enabledOptions[allReports[str]])
                    optionsCheckedListBox.Items.Add(str);
        }

        private void ReportOptionsDialog_Load(object sender, EventArgs e)
        {
            authorTextBox.Text = model.Summary.Author;
            titleTextBox.Text = model.Summary.Title;
            UpdateDialog();
        }

        public List<ReportOptions> GetSelectedOptions()
        {
            List<ReportOptions> options = new List<ReportOptions>();
            foreach (string opt in optionsCheckedListBox.CheckedItems)
                options.Add(allReports[opt]);
            return options;
        }

        private void UpdateDialog()
        {
            List<ReportOptions> selected = GetSelectedOptions();
            checkAllLink.Enabled = (selected.Count != optionsCheckedListBox.Items.Count);
            checkNoneLink.Enabled = (selected.Count > 0);
        }

        /// <summary>
        /// Gets the option to report only the selected objects.
        /// </summary>
        public bool OnlySelected
        {
            get { return onlySelectedCheckBox.Checked; }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            model.Summary.Author = authorTextBox.Text;
            model.Summary.Title = titleTextBox.Text;
        }

        private void checkAllLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < optionsCheckedListBox.Items.Count; i++)
                optionsCheckedListBox.SetItemChecked(i, true);
            UpdateDialog();
        }

        private void checkNoneLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < optionsCheckedListBox.Items.Count; i++)
                optionsCheckedListBox.SetItemChecked(i, false);
            UpdateDialog();
        }

        private void optionsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDialog();
        }
    }
}