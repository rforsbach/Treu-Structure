using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Load;
using Canguro.Model.Design;

namespace Canguro.Commands.Model
{
    public partial class AnalysisOptionsDialog : Form
    {
        Canguro.Controller.CommandServices services;
        AnalysisCase modalCase = null;
        AnalysisCase pDeltaCase = null;
        List<AnalysisCase> responseCases = new List<AnalysisCase>();
        private const int NormalWidth = 402;
        private const int ExtendedWidth = 739;

        public AnalysisOptionsDialog(Canguro.Controller.CommandServices services)
        {
            this.services = services;
            InitializeComponent();
            Init();
            UpdateDialog();
        }

        private void addAnalysisCaseButton_Click(object sender, EventArgs e)
        {
            services.Run(new Commands.Load.AddLoadCaseCmd());
            UpdateDialog();
        }

        private void editAnalysisCaseButton_Click(object sender, EventArgs e)
        {
            Canguro.Model.Load.AbstractCase aCase = analysisCasesCheckedListBox.SelectedItem 
                as Canguro.Model.Load.AbstractCase;
            if (aCase != null)
            {
                try
                {
                    services.GetProperties(Culture.Get("edit"), aCase, false);
                    UpdateDialog();
                }
                catch (Canguro.Controller.CancelCommandException) { }
            }
        }

        private void selectAllAnalysisCasesButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < analysisCasesCheckedListBox.Items.Count; i++)
            {
                analysisCasesCheckedListBox.SetItemChecked(i, true);
                ((Canguro.Model.Load.AbstractCase)analysisCasesCheckedListBox.Items[i]).IsActive = true;
            }
        }

        private void deselectAllAnalysisCasesButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < analysisCasesCheckedListBox.Items.Count; i++)
            {
                analysisCasesCheckedListBox.SetItemChecked(i, false);
                ((Canguro.Model.Load.AbstractCase)analysisCasesCheckedListBox.Items[i]).IsActive = false;
            }
        }

        private void analysisCasesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!updatingDialog)
            {
                AbstractCase aCase = (AbstractCase)analysisCasesCheckedListBox.Items[e.Index];
                aCase.IsActive = (e.NewValue == CheckState.Checked);

                Canguro.Model.Model.Instance.RepairAbstractCases(aCase);
                UpdateDialog();
            }
        }

        public void Init()
        {
            try
            {
                updatingDialog = true;
                Canguro.Model.Model model = services.Model;
                int responseSpectrumCases = 0;
                foreach (AbstractCase aCase in model.AbstractCases)
                {
                    if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is ModalCaseProps)
                    {
                        modalCase = (AnalysisCase)aCase;
                        modalAnalysisCheckBox.Checked = aCase.IsActive;
                    }
                    else if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is PDeltaCaseProps)
                    {
                        pDeltaCase = (AnalysisCase)aCase;
                        pDeltaCheckBox.Checked = aCase.IsActive;
                    }
                    else if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is ResponseSpectrumCaseProps)
                    {
                        responseCases.Add((AnalysisCase)aCase);
                        if (aCase.IsActive)
                            responseSpectrumCases++;

                        switch (responseSpectrumCases)
                        {
                            case 1:
                            case 2:
                                responseSpectrumCheckBox.CheckState = CheckState.Indeterminate;
                                break;
                            case 3:
                                responseSpectrumCheckBox.CheckState = CheckState.Checked;
                                break;
                            default:
                                responseSpectrumCheckBox.CheckState = CheckState.Unchecked;
                                break;
                        }
                    }
                }

                responseSpectrumFunctionsComboBox.Items.Clear();
                foreach (ResponseSpectrum rs in model.ResponseSpectra)
                    responseSpectrumFunctionsComboBox.Items.Add(rs);

                steelDesignComboBox.Items.Add(NoDesign.Instance);
                concreteDesignComboBox.Items.Add(NoDesign.Instance);
                foreach (DesignOptions option in model.DesignOptions)
                {
                    if (option is SteelDesignOptions)
                        steelDesignComboBox.Items.Add(option);
                    else if (option is ConcreteDesignOptions)
                        concreteDesignComboBox.Items.Add(option);
                }
                steelDesignComboBox.SelectedItem = model.SteelDesignOptions;
                concreteDesignComboBox.SelectedItem = model.ConcreteDesignOptions;

                bool steelActive = false;
                bool concreteActive = false;
                foreach (Canguro.Model.LineElement e in model.LineList)
                {
                    if (e != null && e.Properties is Canguro.Model.StraightFrameProps &&
                        ((Canguro.Model.StraightFrameProps)e.Properties).Section.Material.DesignProperties is
                        Canguro.Model.Material.SteelDesignProps)
                        steelActive = true;
                    else if (e != null && e.Properties is Canguro.Model.StraightFrameProps &&
                        ((Canguro.Model.StraightFrameProps)e.Properties).Section.Material.DesignProperties is
                        Canguro.Model.Material.ConcreteDesignProps)
                        concreteActive = true;
                }
                steelDesignComboBox.Enabled = steelActive;
                concreteDesignComboBox.Enabled = concreteActive;
            }
            finally
            {
                updatingDialog = false;
            }

            this.Width = NormalWidth;
        }

        private bool updatingDialog = false;
        public void UpdateDialog()
        {
            try
            {
                updatingDialog = true;
                int responseSpectrumCases = 0;
                Canguro.Model.Model model = services.Model;
                analysisCasesCheckedListBox.Items.Clear();

                foreach (AbstractCase aCase in model.AbstractCases)
                {
                    if (!(aCase is AnalysisCase) || (!(((AnalysisCase)aCase).Properties is ModalCaseProps) &&
                        (!(((AnalysisCase)aCase).Properties is ResponseSpectrumCaseProps)) &&
                        (!(((AnalysisCase)aCase).Properties is PDeltaCaseProps))))
                        analysisCasesCheckedListBox.Items.Add(aCase);

                    if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is ResponseSpectrumCaseProps)
                    {
                        if (aCase.IsActive)
                            responseSpectrumCases++;

                        switch (responseSpectrumCases)
                        {
                            case 0:
                                responseSpectrumCheckBox.CheckState = CheckState.Unchecked;
                                break;
                            case 1:
                            case 2:
                                responseSpectrumCheckBox.CheckState = CheckState.Indeterminate;
                                break;
                            default:
                                responseSpectrumCheckBox.CheckState = CheckState.Checked;
                                responseSpectrumFactorUpDown.Value = Convert.ToDecimal(((ResponseSpectrumCaseProps)((AnalysisCase)aCase).Properties).ScaleFactor);
                                dampingFactorUpDown.Value = Convert.ToDecimal(((ResponseSpectrumCaseProps)((AnalysisCase)aCase).Properties).ModalDamping);
                                break;
                        }
                    }

                    if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is ModalCaseProps)
                        modalAnalysisCheckBox.Checked = aCase.IsActive;

                    if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is PDeltaCaseProps)
                        pDeltaCheckBox.Checked = aCase.IsActive;
                }

                for (int i = 0; i < analysisCasesCheckedListBox.Items.Count; i++)
                    analysisCasesCheckedListBox.SetItemChecked(i,
                        ((AbstractCase)analysisCasesCheckedListBox.Items[i]).IsActive);

                if (modalCase != null)
                    modesNumericUpDown.Value = ((ModalCaseProps)modalCase.Properties).MaxModes;
                if (responseCases.Count > 0 && responseCases[0].IsActive)
                {
                    ResponseSpectrumCaseProps rsProps = (ResponseSpectrumCaseProps)responseCases[0].Properties;
                    //responseSpectrumFunctionsComboBox.SelectedText = rsProps.ResponseSpectrumFunction.ToString();
                    if (responseSpectrumFunctionsComboBox.SelectedValue == null ||
                        !responseSpectrumFunctionsComboBox.SelectedValue.ToString().Equals(rsProps.ResponseSpectrumFunction.ToString()))
                    {
                        foreach (object obj in responseSpectrumFunctionsComboBox.Items)
                            if (rsProps != null && obj.ToString().Equals(rsProps.ResponseSpectrumFunction.ToString()))
                                responseSpectrumFunctionsComboBox.SelectedItem = obj;
                    }
                    viewSpectrumLink.Enabled = true;
                }
                else
                    viewSpectrumLink.Enabled = false;

                modesNumericUpDown.Enabled = modalAnalysisCheckBox.Checked;
                //responseSpectrumCheckBox.Enabled = modalAnalysisCheckBox.Checked;
                responseSpectrumFunctionsComboBox.Enabled = responseSpectrumCheckBox.Checked;
                responseSpectrumFactorUpDown.Enabled = responseSpectrumCheckBox.Checked;
                dampingFactorLabel.Enabled = responseSpectrumCheckBox.Checked;
                dampingFactorUpDown.Enabled = responseSpectrumCheckBox.Checked;

                editSteelDesignLinkLabel.Enabled = steelDesignComboBox.Enabled && model.SteelDesignOptions is SteelDesignOptions;
                steelCombosLinkLabel.Enabled = editSteelDesignLinkLabel.Enabled;
                editConcreteDesignLinkLabel.Enabled = concreteDesignComboBox.Enabled && model.ConcreteDesignOptions is ConcreteDesignOptions;
                concreteCombosLinkLabel.Enabled = editConcreteDesignLinkLabel.Enabled;

                if (model.SteelDesignOptions is SteelDesignOptions && model.SteelDesignOptions.DesignCombinations.Count == 0)
                    currentDesignOptions = model.SteelDesignOptions;
                else if (model.ConcreteDesignOptions is ConcreteDesignOptions && model.ConcreteDesignOptions.DesignCombinations.Count == 0)
                    currentDesignOptions = model.ConcreteDesignOptions;
                else
                    currentDesignOptions = null;

                AnalyzeButton.Text = (currentDesignOptions == null) ? Culture.Get("analyze") : Culture.Get("next");
                AcceptButton = AnalyzeButton;
            }
            finally
            {
                updatingDialog = false;
            }

            Update();
        }
          
        private void modalAnalysisCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!updatingDialog)
            {
                if (modalCase == null && modalAnalysisCheckBox.Checked)
                {
                    modalCase = new AnalysisCase(Culture.Get("defaultModalCase"), new ModalCaseProps());
                    services.Model.AbstractCases.Add(modalCase);
                }
                if (modalCase != null)
                {
                    modalCase.IsActive = modalAnalysisCheckBox.Checked;
                    Canguro.Model.Model.Instance.RepairAbstractCases(modalCase);
                }

                UpdateDialog();
            }
        }

        private void responseSpectrumCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!updatingDialog)
            {
                if (responseCases.Count == 0 && responseSpectrumCheckBox.Checked && services.Model.ResponseSpectra.Count > 0)
                {
                    if (modalCase == null)
                    {
                        modalCase = new AnalysisCase(Culture.Get("defaultModalCase"), new ModalCaseProps());
                        services.Model.AbstractCases.Add(modalCase);
                    }

                    ResponseSpectrumCaseProps props = new ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues.UX);
                    props.ModalAnalysisCase = modalCase;
                    responseCases.Add(new AnalysisCase(Culture.Get("defaultResponseCase") + " X", props));
                    responseCases[0].IsActive = false;
                    services.Model.AbstractCases.Add(responseCases[0]);

                    props = new ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues.UY);
                    props.ModalAnalysisCase = modalCase;
                    responseCases.Add(new AnalysisCase(Culture.Get("defaultResponseCase") + " Y", props));
                    services.Model.AbstractCases.Add(responseCases[1]);

                    props = new ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues.UZ);
                    props.ModalAnalysisCase = modalCase;
                    responseCases.Add(new AnalysisCase(Culture.Get("defaultResponseCase") + " Z", props));
                    services.Model.AbstractCases.Add(responseCases[2]);
                }
                foreach (AbstractCase responseCase in responseCases)
                {
                    responseCase.IsActive = responseSpectrumCheckBox.Checked;
                    Canguro.Model.Model.Instance.RepairAbstractCases(responseCase);
                }

                UpdateDialog();
            }
        }

        private void responseSpectrumFactorUpDown_ValueChanged(object sender, EventArgs e)
        {
            foreach (AnalysisCase responseCase in responseCases)
                ((ResponseSpectrumCaseProps)responseCase.Properties).ScaleFactor = Convert.ToSingle(responseSpectrumFactorUpDown.Value);
        }

        private void dampingFactorUpDown_ValueChanged(object sender, EventArgs e)
        {
            foreach (AnalysisCase responseCase in responseCases)
                ((ResponseSpectrumCaseProps)responseCase.Properties).ModalDamping = Convert.ToSingle(dampingFactorUpDown.Value);
        }

        private void modesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (modalCase != null)
                ((ModalCaseProps)modalCase.Properties).MaxModes = (uint)modesNumericUpDown.Value;
        }

        private void responseSpectrumFunctionsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (AnalysisCase responseCase in responseCases)
                ((ResponseSpectrumCaseProps)responseCase.Properties).ResponseSpectrumFunction = (ResponseSpectrum)responseSpectrumFunctionsComboBox.SelectedItem;
        }

        private void editSteelDesignLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DesignOptions option = steelDesignComboBox.SelectedItem as DesignOptions;
            if (option != null && !(option is NoDesign))
            {
                DesignOptions clone = (DesignOptions)option.Clone();
                DialogResult result = new Canguro.Commands.Forms.EditDesignOptionsDialog(clone).ShowDialog(this);
                if (result == DialogResult.OK)
                    option.CopyFrom(clone);
            }
        }

        private void editConcreteDesignLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DesignOptions option = concreteDesignComboBox.SelectedItem as DesignOptions;
            if (option != null && !(option is NoDesign))
            {
                DesignOptions clone = (DesignOptions)option.Clone();
                DialogResult result = new Canguro.Commands.Forms.EditDesignOptionsDialog(clone).ShowDialog(this);
                if (result == DialogResult.OK)
                    option.CopyFrom(clone);
            }
        }

        private void steelDesignComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            services.Model.SteelDesignOptions = (DesignOptions)steelDesignComboBox.SelectedItem;
            UpdateDialog();
        }

        private void concreteDesignComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            services.Model.ConcreteDesignOptions = (DesignOptions)concreteDesignComboBox.SelectedItem;
            UpdateDialog();
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (currentDesignOptions == null) // All options are set
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                wizardControl.SelectTab(1);
                updateDesignCombosPage();
                DialogResult = DialogResult.None;
            }
        }

        private void AnalysisOptionsDialog_Load(object sender, EventArgs e)
        {
            wizardControl.HideTabs = true;
        }

        #region Design Combos

        private DesignOptions currentDesignOptions;

        private void updateDesignCombosPage()
        {
            designNameLabel.Text = currentDesignOptions.ToString();
            selectedCombosListBox.Items.Clear();
            allCombosListBox.Items.Clear();
            foreach (AbstractCase aCase in services.Model.AbstractCases)
                if (aCase is LoadCombination)
                {
                    if (currentDesignOptions.DesignCombinations.Contains((LoadCombination)aCase))
                        selectedCombosListBox.Items.Add(aCase);
                    else
                        allCombosListBox.Items.Add(aCase);
                }

            editComboLinkLabel.Enabled = (allCombosListBox.SelectedItem != null || selectedCombosListBox.SelectedItem != null);
            AcceptButton = analyzeButton2;
            if (currentDesignOptions is SteelDesignOptions && services.Model.ConcreteDesignOptions is ConcreteDesignOptions)
                analyzeButton2.Text = Culture.Get("next");
            else
                analyzeButton2.Text = Culture.Get("analyze");
        }

        private void allCombosListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allCombosListBox.SelectedItem != null)
                selectedCombosListBox.SelectedItem = null;

            editComboLinkLabel.Enabled = (allCombosListBox.SelectedItem != null || selectedCombosListBox.SelectedItem != null);
        }

        private void selectedCombosListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedCombosListBox.SelectedItem != null)
                allCombosListBox.SelectedItem = null;

            editComboLinkLabel.Enabled = (allCombosListBox.SelectedItem != null || selectedCombosListBox.SelectedItem != null);
        }

        private void addAllCombosButton_Click(object sender, EventArgs e)
        {
            foreach (object obj in allCombosListBox.Items)
                currentDesignOptions.DesignCombinations.Add((LoadCombination)obj);
            updateDesignCombosPage();
        }

        private void addComboButton_Click(object sender, EventArgs e)
        {
            if (allCombosListBox.SelectedItems != null)
            {
                foreach (LoadCombination item in allCombosListBox.SelectedItems)
                    currentDesignOptions.DesignCombinations.Add(item);
                updateDesignCombosPage();
            }
        }

        private void removeComboButton_Click(object sender, EventArgs e)
        {
            if (selectedCombosListBox.SelectedItems != null)
            {
                foreach (LoadCombination item in selectedCombosListBox.SelectedItems)
                    currentDesignOptions.DesignCombinations.Remove(item);
                updateDesignCombosPage();
            }
        }

        private void removeAllCombosButton_Click(object sender, EventArgs e)
        {
            currentDesignOptions.DesignCombinations.Clear();
            updateDesignCombosPage();
        }

        private void addComboLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Canguro.Commands.Forms.LoadCombinationsDialog dlg = new Canguro.Commands.Forms.LoadCombinationsDialog(services.Model, new LoadCombination(Culture.Get("defaultLoadComboName")));
            dlg.ShowDialog(this);
            updateDesignCombosPage();
        }

        private void editComboLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadCombination combo = (allCombosListBox.SelectedItem != null) ? (LoadCombination)allCombosListBox.SelectedItem :
                (selectedCombosListBox.SelectedItem != null) ? (LoadCombination)selectedCombosListBox.SelectedItem : null;
            if (combo != null)
            {
                Canguro.Commands.Forms.LoadCombinationsDialog dlg = new Canguro.Commands.Forms.LoadCombinationsDialog(services.Model, combo);
                dlg.ShowDialog(this);
                updateDesignCombosPage();
            }
        }

        private void addDefaultsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            currentDesignOptions.AddDefaultCombos();
            updateDesignCombosPage();
        }

        private void analyzeButton2_Click(object sender, EventArgs e)
        {
            if (currentDesignOptions is ConcreteDesignOptions)
                currentDesignOptions = null;
            else if (currentDesignOptions is SteelDesignOptions)
                currentDesignOptions = (services.Model.ConcreteDesignOptions is ConcreteDesignOptions) ?
                    services.Model.ConcreteDesignOptions : null;

            AnalyzeButton_Click(sender, e);
        }

        private void applyButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
        }

        private void cancelButton2_Click(object sender, EventArgs e)
        {
            wizardControl.SelectTab(0);
            UpdateDialog();
            DialogResult = DialogResult.None;
        }

        #endregion

        private void applyButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void steelCombosLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            currentDesignOptions = services.Model.SteelDesignOptions;
            wizardControl.SelectTab(1);
            updateDesignCombosPage();
        }

        private void concreteCombosLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            currentDesignOptions = services.Model.ConcreteDesignOptions;
            wizardControl.SelectTab(1);
            updateDesignCombosPage();
        }

        private void viewSpectrumLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (responseSpectrumFunctionsComboBox.SelectedItem != null)
            {
                ResponseSpectrum rs = (ResponseSpectrum)responseSpectrumFunctionsComboBox.SelectedItem;
                new Canguro.Commands.Forms.ViewSpectrumDialog(rs).ShowDialog();
            }            
        }

        private void pDeltaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!updatingDialog)
            {
                if (pDeltaCase == null && pDeltaCheckBox.Checked)
                {
                    PDeltaCaseProps props = new PDeltaCaseProps();
                    pDeltaCase = new AnalysisCase(Culture.Get("defaultPDeltaCase"), props);
                    
                    services.Model.AbstractCases.Add(pDeltaCase);
                }
                if (pDeltaCase != null)
                {
                    pDeltaCase.IsActive = pDeltaCheckBox.Checked;
                    Canguro.Model.Model.Instance.RepairAbstractCases(pDeltaCase);
                }

                UpdateDialog();
            }
        }

        private void analysisCasesCheckedListBox_DoubleClick(object sender, EventArgs e)
        {
            // TODO: Editar LC
        }
    }
}