namespace Canguro.Commands.Model
{
    partial class AnalysisOptionsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisOptionsDialog));
            this.AnalyzeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.wizardControl = new Canguro.Commands.Forms.WizardControl();
            this.mainTabPage = new System.Windows.Forms.TabPage();
            this.pDeltaCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dampingFactorUpDown = new System.Windows.Forms.NumericUpDown();
            this.dampingFactorLabel = new System.Windows.Forms.Label();
            this.viewSpectrumLink = new System.Windows.Forms.LinkLabel();
            this.responseSpectrumFactorUpDown = new System.Windows.Forms.NumericUpDown();
            this.spectrumFactorLabel = new System.Windows.Forms.Label();
            this.responseSpectrumFunctionsComboBox = new System.Windows.Forms.ComboBox();
            this.responseSpectrumCheckBox = new System.Windows.Forms.CheckBox();
            this.modesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.modalAnalysisCheckBox = new System.Windows.Forms.CheckBox();
            this.analysisCasesGroupBox = new System.Windows.Forms.GroupBox();
            this.deselectAllAnalysisCasesButton = new System.Windows.Forms.Button();
            this.analysisCasesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.selectAllAnalysisCasesButton = new System.Windows.Forms.Button();
            this.addAnalysisCaseButton = new System.Windows.Forms.Button();
            this.editAnalysisCaseButton = new System.Windows.Forms.Button();
            this.designGroupBox = new System.Windows.Forms.GroupBox();
            this.concreteCombosLinkLabel = new System.Windows.Forms.LinkLabel();
            this.steelCombosLinkLabel = new System.Windows.Forms.LinkLabel();
            this.concreteDesignLabel = new System.Windows.Forms.Label();
            this.SteelDesignLabel = new System.Windows.Forms.Label();
            this.editConcreteDesignLinkLabel = new System.Windows.Forms.LinkLabel();
            this.editSteelDesignLinkLabel = new System.Windows.Forms.LinkLabel();
            this.concreteDesignComboBox = new System.Windows.Forms.ComboBox();
            this.steelDesignComboBox = new System.Windows.Forms.ComboBox();
            this.designCombosTabPage = new System.Windows.Forms.TabPage();
            this.applyButton2 = new System.Windows.Forms.Button();
            this.cancelButton2 = new System.Windows.Forms.Button();
            this.analyzeButton2 = new System.Windows.Forms.Button();
            this.addDefaultsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.editComboLinkLabel = new System.Windows.Forms.LinkLabel();
            this.addComboLinkLabel = new System.Windows.Forms.LinkLabel();
            this.removeAllCombosButton = new System.Windows.Forms.Button();
            this.removeComboButton = new System.Windows.Forms.Button();
            this.addComboButton = new System.Windows.Forms.Button();
            this.addAllCombosButton = new System.Windows.Forms.Button();
            this.selectedCombosListBox = new System.Windows.Forms.ListBox();
            this.allCombosListBox = new System.Windows.Forms.ListBox();
            this.designCombosLabel = new System.Windows.Forms.Label();
            this.designNameLabel = new System.Windows.Forms.Label();
            this.cachedGeneralReport1 = new Canguro.RuntimeData.CachedGeneralReport();
            this.wizardControl.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dampingFactorUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.responseSpectrumFactorUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modesNumericUpDown)).BeginInit();
            this.analysisCasesGroupBox.SuspendLayout();
            this.designGroupBox.SuspendLayout();
            this.designCombosTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnalyzeButton
            // 
            resources.ApplyResources(this.AnalyzeButton, "AnalyzeButton");
            this.AnalyzeButton.Name = "AnalyzeButton";
            this.AnalyzeButton.UseVisualStyleBackColor = true;
            this.AnalyzeButton.Click += new System.EventHandler(this.AnalyzeButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // wizardControl
            // 
            this.wizardControl.Controls.Add(this.mainTabPage);
            this.wizardControl.Controls.Add(this.designCombosTabPage);
            resources.ApplyResources(this.wizardControl, "wizardControl");
            this.wizardControl.Name = "wizardControl";
            this.wizardControl.SelectedIndex = 0;
            // 
            // mainTabPage
            // 
            this.mainTabPage.Controls.Add(this.pDeltaCheckBox);
            this.mainTabPage.Controls.Add(this.applyButton);
            this.mainTabPage.Controls.Add(this.cancelButton);
            this.mainTabPage.Controls.Add(this.groupBox1);
            this.mainTabPage.Controls.Add(this.analysisCasesGroupBox);
            this.mainTabPage.Controls.Add(this.designGroupBox);
            this.mainTabPage.Controls.Add(this.AnalyzeButton);
            resources.ApplyResources(this.mainTabPage, "mainTabPage");
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.UseVisualStyleBackColor = true;
            // 
            // pDeltaCheckBox
            // 
            resources.ApplyResources(this.pDeltaCheckBox, "pDeltaCheckBox");
            this.pDeltaCheckBox.Name = "pDeltaCheckBox";
            this.pDeltaCheckBox.UseVisualStyleBackColor = true;
            this.pDeltaCheckBox.CheckedChanged += new System.EventHandler(this.pDeltaCheckBox_CheckedChanged);
            // 
            // applyButton
            // 
            this.applyButton.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.Name = "applyButton";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dampingFactorUpDown);
            this.groupBox1.Controls.Add(this.dampingFactorLabel);
            this.groupBox1.Controls.Add(this.viewSpectrumLink);
            this.groupBox1.Controls.Add(this.responseSpectrumFactorUpDown);
            this.groupBox1.Controls.Add(this.spectrumFactorLabel);
            this.groupBox1.Controls.Add(this.responseSpectrumFunctionsComboBox);
            this.groupBox1.Controls.Add(this.responseSpectrumCheckBox);
            this.groupBox1.Controls.Add(this.modesNumericUpDown);
            this.groupBox1.Controls.Add(this.modalAnalysisCheckBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // dampingFactorUpDown
            // 
            this.dampingFactorUpDown.DecimalPlaces = 2;
            this.dampingFactorUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.dampingFactorUpDown, "dampingFactorUpDown");
            this.dampingFactorUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.dampingFactorUpDown.Name = "dampingFactorUpDown";
            this.dampingFactorUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.dampingFactorUpDown.ValueChanged += new System.EventHandler(this.dampingFactorUpDown_ValueChanged);
            // 
            // dampingFactorLabel
            // 
            resources.ApplyResources(this.dampingFactorLabel, "dampingFactorLabel");
            this.dampingFactorLabel.Name = "dampingFactorLabel";
            // 
            // viewSpectrumLink
            // 
            resources.ApplyResources(this.viewSpectrumLink, "viewSpectrumLink");
            this.viewSpectrumLink.Name = "viewSpectrumLink";
            this.viewSpectrumLink.TabStop = true;
            this.viewSpectrumLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.viewSpectrumLink_LinkClicked);
            // 
            // responseSpectrumFactorUpDown
            // 
            this.responseSpectrumFactorUpDown.DecimalPlaces = 2;
            this.responseSpectrumFactorUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            resources.ApplyResources(this.responseSpectrumFactorUpDown, "responseSpectrumFactorUpDown");
            this.responseSpectrumFactorUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.responseSpectrumFactorUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.responseSpectrumFactorUpDown.Name = "responseSpectrumFactorUpDown";
            this.responseSpectrumFactorUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.responseSpectrumFactorUpDown.ValueChanged += new System.EventHandler(this.responseSpectrumFactorUpDown_ValueChanged);
            // 
            // spectrumFactorLabel
            // 
            resources.ApplyResources(this.spectrumFactorLabel, "spectrumFactorLabel");
            this.spectrumFactorLabel.Name = "spectrumFactorLabel";
            // 
            // responseSpectrumFunctionsComboBox
            // 
            this.responseSpectrumFunctionsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.responseSpectrumFunctionsComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.responseSpectrumFunctionsComboBox, "responseSpectrumFunctionsComboBox");
            this.responseSpectrumFunctionsComboBox.Name = "responseSpectrumFunctionsComboBox";
            this.responseSpectrumFunctionsComboBox.SelectedIndexChanged += new System.EventHandler(this.responseSpectrumFunctionsComboBox_SelectedIndexChanged);
            // 
            // responseSpectrumCheckBox
            // 
            resources.ApplyResources(this.responseSpectrumCheckBox, "responseSpectrumCheckBox");
            this.responseSpectrumCheckBox.Name = "responseSpectrumCheckBox";
            this.responseSpectrumCheckBox.UseVisualStyleBackColor = true;
            this.responseSpectrumCheckBox.CheckedChanged += new System.EventHandler(this.responseSpectrumCheckBox_CheckedChanged);
            // 
            // modesNumericUpDown
            // 
            resources.ApplyResources(this.modesNumericUpDown, "modesNumericUpDown");
            this.modesNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modesNumericUpDown.Name = "modesNumericUpDown";
            this.modesNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modesNumericUpDown.ValueChanged += new System.EventHandler(this.modesNumericUpDown_ValueChanged);
            // 
            // modalAnalysisCheckBox
            // 
            resources.ApplyResources(this.modalAnalysisCheckBox, "modalAnalysisCheckBox");
            this.modalAnalysisCheckBox.Name = "modalAnalysisCheckBox";
            this.modalAnalysisCheckBox.UseVisualStyleBackColor = true;
            this.modalAnalysisCheckBox.CheckedChanged += new System.EventHandler(this.modalAnalysisCheckBox_CheckedChanged);
            // 
            // analysisCasesGroupBox
            // 
            this.analysisCasesGroupBox.Controls.Add(this.deselectAllAnalysisCasesButton);
            this.analysisCasesGroupBox.Controls.Add(this.analysisCasesCheckedListBox);
            this.analysisCasesGroupBox.Controls.Add(this.selectAllAnalysisCasesButton);
            this.analysisCasesGroupBox.Controls.Add(this.addAnalysisCaseButton);
            this.analysisCasesGroupBox.Controls.Add(this.editAnalysisCaseButton);
            resources.ApplyResources(this.analysisCasesGroupBox, "analysisCasesGroupBox");
            this.analysisCasesGroupBox.Name = "analysisCasesGroupBox";
            this.analysisCasesGroupBox.TabStop = false;
            // 
            // deselectAllAnalysisCasesButton
            // 
            resources.ApplyResources(this.deselectAllAnalysisCasesButton, "deselectAllAnalysisCasesButton");
            this.deselectAllAnalysisCasesButton.Name = "deselectAllAnalysisCasesButton";
            this.deselectAllAnalysisCasesButton.UseVisualStyleBackColor = true;
            this.deselectAllAnalysisCasesButton.Click += new System.EventHandler(this.deselectAllAnalysisCasesButton_Click);
            // 
            // analysisCasesCheckedListBox
            // 
            this.analysisCasesCheckedListBox.FormattingEnabled = true;
            resources.ApplyResources(this.analysisCasesCheckedListBox, "analysisCasesCheckedListBox");
            this.analysisCasesCheckedListBox.Name = "analysisCasesCheckedListBox";
            this.analysisCasesCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.analysisCasesCheckedListBox_ItemCheck);
            this.analysisCasesCheckedListBox.DoubleClick += new System.EventHandler(this.analysisCasesCheckedListBox_DoubleClick);
            // 
            // selectAllAnalysisCasesButton
            // 
            resources.ApplyResources(this.selectAllAnalysisCasesButton, "selectAllAnalysisCasesButton");
            this.selectAllAnalysisCasesButton.Name = "selectAllAnalysisCasesButton";
            this.selectAllAnalysisCasesButton.UseVisualStyleBackColor = true;
            this.selectAllAnalysisCasesButton.Click += new System.EventHandler(this.selectAllAnalysisCasesButton_Click);
            // 
            // addAnalysisCaseButton
            // 
            resources.ApplyResources(this.addAnalysisCaseButton, "addAnalysisCaseButton");
            this.addAnalysisCaseButton.Name = "addAnalysisCaseButton";
            this.addAnalysisCaseButton.UseVisualStyleBackColor = true;
            this.addAnalysisCaseButton.Click += new System.EventHandler(this.addAnalysisCaseButton_Click);
            // 
            // editAnalysisCaseButton
            // 
            resources.ApplyResources(this.editAnalysisCaseButton, "editAnalysisCaseButton");
            this.editAnalysisCaseButton.Name = "editAnalysisCaseButton";
            this.editAnalysisCaseButton.UseVisualStyleBackColor = true;
            this.editAnalysisCaseButton.Click += new System.EventHandler(this.editAnalysisCaseButton_Click);
            // 
            // designGroupBox
            // 
            this.designGroupBox.Controls.Add(this.concreteCombosLinkLabel);
            this.designGroupBox.Controls.Add(this.steelCombosLinkLabel);
            this.designGroupBox.Controls.Add(this.concreteDesignLabel);
            this.designGroupBox.Controls.Add(this.SteelDesignLabel);
            this.designGroupBox.Controls.Add(this.editConcreteDesignLinkLabel);
            this.designGroupBox.Controls.Add(this.editSteelDesignLinkLabel);
            this.designGroupBox.Controls.Add(this.concreteDesignComboBox);
            this.designGroupBox.Controls.Add(this.steelDesignComboBox);
            resources.ApplyResources(this.designGroupBox, "designGroupBox");
            this.designGroupBox.Name = "designGroupBox";
            this.designGroupBox.TabStop = false;
            // 
            // concreteCombosLinkLabel
            // 
            resources.ApplyResources(this.concreteCombosLinkLabel, "concreteCombosLinkLabel");
            this.concreteCombosLinkLabel.Name = "concreteCombosLinkLabel";
            this.concreteCombosLinkLabel.TabStop = true;
            this.concreteCombosLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.concreteCombosLinkLabel_LinkClicked);
            // 
            // steelCombosLinkLabel
            // 
            resources.ApplyResources(this.steelCombosLinkLabel, "steelCombosLinkLabel");
            this.steelCombosLinkLabel.Name = "steelCombosLinkLabel";
            this.steelCombosLinkLabel.TabStop = true;
            this.steelCombosLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.steelCombosLinkLabel_LinkClicked);
            // 
            // concreteDesignLabel
            // 
            resources.ApplyResources(this.concreteDesignLabel, "concreteDesignLabel");
            this.concreteDesignLabel.Name = "concreteDesignLabel";
            // 
            // SteelDesignLabel
            // 
            resources.ApplyResources(this.SteelDesignLabel, "SteelDesignLabel");
            this.SteelDesignLabel.Name = "SteelDesignLabel";
            // 
            // editConcreteDesignLinkLabel
            // 
            resources.ApplyResources(this.editConcreteDesignLinkLabel, "editConcreteDesignLinkLabel");
            this.editConcreteDesignLinkLabel.Name = "editConcreteDesignLinkLabel";
            this.editConcreteDesignLinkLabel.TabStop = true;
            this.editConcreteDesignLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.editConcreteDesignLinkLabel_LinkClicked);
            // 
            // editSteelDesignLinkLabel
            // 
            resources.ApplyResources(this.editSteelDesignLinkLabel, "editSteelDesignLinkLabel");
            this.editSteelDesignLinkLabel.Name = "editSteelDesignLinkLabel";
            this.editSteelDesignLinkLabel.TabStop = true;
            this.editSteelDesignLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.editSteelDesignLinkLabel_LinkClicked);
            // 
            // concreteDesignComboBox
            // 
            this.concreteDesignComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.concreteDesignComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.concreteDesignComboBox, "concreteDesignComboBox");
            this.concreteDesignComboBox.Name = "concreteDesignComboBox";
            this.concreteDesignComboBox.SelectedIndexChanged += new System.EventHandler(this.concreteDesignComboBox_SelectedIndexChanged);
            // 
            // steelDesignComboBox
            // 
            this.steelDesignComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.steelDesignComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.steelDesignComboBox, "steelDesignComboBox");
            this.steelDesignComboBox.Name = "steelDesignComboBox";
            this.steelDesignComboBox.SelectedIndexChanged += new System.EventHandler(this.steelDesignComboBox_SelectedIndexChanged);
            // 
            // designCombosTabPage
            // 
            this.designCombosTabPage.Controls.Add(this.applyButton2);
            this.designCombosTabPage.Controls.Add(this.cancelButton2);
            this.designCombosTabPage.Controls.Add(this.analyzeButton2);
            this.designCombosTabPage.Controls.Add(this.addDefaultsLinkLabel);
            this.designCombosTabPage.Controls.Add(this.editComboLinkLabel);
            this.designCombosTabPage.Controls.Add(this.addComboLinkLabel);
            this.designCombosTabPage.Controls.Add(this.removeAllCombosButton);
            this.designCombosTabPage.Controls.Add(this.removeComboButton);
            this.designCombosTabPage.Controls.Add(this.addComboButton);
            this.designCombosTabPage.Controls.Add(this.addAllCombosButton);
            this.designCombosTabPage.Controls.Add(this.selectedCombosListBox);
            this.designCombosTabPage.Controls.Add(this.allCombosListBox);
            this.designCombosTabPage.Controls.Add(this.designCombosLabel);
            this.designCombosTabPage.Controls.Add(this.designNameLabel);
            resources.ApplyResources(this.designCombosTabPage, "designCombosTabPage");
            this.designCombosTabPage.Name = "designCombosTabPage";
            this.designCombosTabPage.UseVisualStyleBackColor = true;
            // 
            // applyButton2
            // 
            this.applyButton2.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            resources.ApplyResources(this.applyButton2, "applyButton2");
            this.applyButton2.Name = "applyButton2";
            this.applyButton2.UseVisualStyleBackColor = true;
            this.applyButton2.Click += new System.EventHandler(this.applyButton2_Click);
            // 
            // cancelButton2
            // 
            this.cancelButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton2, "cancelButton2");
            this.cancelButton2.Name = "cancelButton2";
            this.cancelButton2.UseVisualStyleBackColor = true;
            this.cancelButton2.Click += new System.EventHandler(this.cancelButton2_Click);
            // 
            // analyzeButton2
            // 
            this.analyzeButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.analyzeButton2, "analyzeButton2");
            this.analyzeButton2.Name = "analyzeButton2";
            this.analyzeButton2.UseVisualStyleBackColor = true;
            this.analyzeButton2.Click += new System.EventHandler(this.analyzeButton2_Click);
            // 
            // addDefaultsLinkLabel
            // 
            resources.ApplyResources(this.addDefaultsLinkLabel, "addDefaultsLinkLabel");
            this.addDefaultsLinkLabel.Name = "addDefaultsLinkLabel";
            this.addDefaultsLinkLabel.TabStop = true;
            this.addDefaultsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.addDefaultsLinkLabel_LinkClicked);
            // 
            // editComboLinkLabel
            // 
            resources.ApplyResources(this.editComboLinkLabel, "editComboLinkLabel");
            this.editComboLinkLabel.Name = "editComboLinkLabel";
            this.editComboLinkLabel.TabStop = true;
            this.editComboLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.editComboLinkLabel_LinkClicked);
            // 
            // addComboLinkLabel
            // 
            resources.ApplyResources(this.addComboLinkLabel, "addComboLinkLabel");
            this.addComboLinkLabel.Name = "addComboLinkLabel";
            this.addComboLinkLabel.TabStop = true;
            this.addComboLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.addComboLinkLabel_LinkClicked);
            // 
            // removeAllCombosButton
            // 
            resources.ApplyResources(this.removeAllCombosButton, "removeAllCombosButton");
            this.removeAllCombosButton.Name = "removeAllCombosButton";
            this.removeAllCombosButton.UseVisualStyleBackColor = true;
            this.removeAllCombosButton.Click += new System.EventHandler(this.removeAllCombosButton_Click);
            // 
            // removeComboButton
            // 
            resources.ApplyResources(this.removeComboButton, "removeComboButton");
            this.removeComboButton.Name = "removeComboButton";
            this.removeComboButton.UseVisualStyleBackColor = true;
            this.removeComboButton.Click += new System.EventHandler(this.removeComboButton_Click);
            // 
            // addComboButton
            // 
            resources.ApplyResources(this.addComboButton, "addComboButton");
            this.addComboButton.Name = "addComboButton";
            this.addComboButton.UseVisualStyleBackColor = true;
            this.addComboButton.Click += new System.EventHandler(this.addComboButton_Click);
            // 
            // addAllCombosButton
            // 
            resources.ApplyResources(this.addAllCombosButton, "addAllCombosButton");
            this.addAllCombosButton.Name = "addAllCombosButton";
            this.addAllCombosButton.UseVisualStyleBackColor = true;
            this.addAllCombosButton.Click += new System.EventHandler(this.addAllCombosButton_Click);
            // 
            // selectedCombosListBox
            // 
            this.selectedCombosListBox.FormattingEnabled = true;
            resources.ApplyResources(this.selectedCombosListBox, "selectedCombosListBox");
            this.selectedCombosListBox.Name = "selectedCombosListBox";
            this.selectedCombosListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedCombosListBox.SelectedIndexChanged += new System.EventHandler(this.selectedCombosListBox_SelectedIndexChanged);
            // 
            // allCombosListBox
            // 
            this.allCombosListBox.FormattingEnabled = true;
            resources.ApplyResources(this.allCombosListBox, "allCombosListBox");
            this.allCombosListBox.Name = "allCombosListBox";
            this.allCombosListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.allCombosListBox.SelectedIndexChanged += new System.EventHandler(this.allCombosListBox_SelectedIndexChanged);
            // 
            // designCombosLabel
            // 
            resources.ApplyResources(this.designCombosLabel, "designCombosLabel");
            this.designCombosLabel.Name = "designCombosLabel";
            // 
            // designNameLabel
            // 
            resources.ApplyResources(this.designNameLabel, "designNameLabel");
            this.designNameLabel.Name = "designNameLabel";
            // 
            // AnalysisOptionsDialog
            // 
            this.AcceptButton = this.AnalyzeButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.wizardControl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnalysisOptionsDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.AnalysisOptionsDialog_Load);
            this.wizardControl.ResumeLayout(false);
            this.mainTabPage.ResumeLayout(false);
            this.mainTabPage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dampingFactorUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.responseSpectrumFactorUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modesNumericUpDown)).EndInit();
            this.analysisCasesGroupBox.ResumeLayout(false);
            this.designGroupBox.ResumeLayout(false);
            this.designGroupBox.PerformLayout();
            this.designCombosTabPage.ResumeLayout(false);
            this.designCombosTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox responseSpectrumCheckBox;
        private System.Windows.Forms.NumericUpDown modesNumericUpDown;
        private System.Windows.Forms.CheckBox modalAnalysisCheckBox;
        private System.Windows.Forms.ComboBox responseSpectrumFunctionsComboBox;
        private System.Windows.Forms.CheckedListBox analysisCasesCheckedListBox;
        private System.Windows.Forms.Button addAnalysisCaseButton;
        private System.Windows.Forms.Button editAnalysisCaseButton;
        private System.Windows.Forms.Button selectAllAnalysisCasesButton;
        private System.Windows.Forms.Button deselectAllAnalysisCasesButton;
        private System.Windows.Forms.GroupBox analysisCasesGroupBox;
        private System.Windows.Forms.Button AnalyzeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox designGroupBox;
        private System.Windows.Forms.LinkLabel editSteelDesignLinkLabel;
        private System.Windows.Forms.ComboBox concreteDesignComboBox;
        private System.Windows.Forms.ComboBox steelDesignComboBox;
        private System.Windows.Forms.LinkLabel editConcreteDesignLinkLabel;
        private System.Windows.Forms.Label concreteDesignLabel;
        private System.Windows.Forms.Label SteelDesignLabel;
        private Canguro.Commands.Forms.WizardControl wizardControl;
        private System.Windows.Forms.TabPage mainTabPage;
        private System.Windows.Forms.TabPage designCombosTabPage;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label designCombosLabel;
        private System.Windows.Forms.Label designNameLabel;
        private System.Windows.Forms.ListBox selectedCombosListBox;
        private System.Windows.Forms.ListBox allCombosListBox;
        private System.Windows.Forms.Button removeAllCombosButton;
        private System.Windows.Forms.Button removeComboButton;
        private System.Windows.Forms.Button addComboButton;
        private System.Windows.Forms.Button addAllCombosButton;
        private System.Windows.Forms.LinkLabel addComboLinkLabel;
        private System.Windows.Forms.LinkLabel addDefaultsLinkLabel;
        private System.Windows.Forms.LinkLabel editComboLinkLabel;
        private System.Windows.Forms.Button applyButton2;
        private System.Windows.Forms.Button cancelButton2;
        private System.Windows.Forms.Button analyzeButton2;
        private System.Windows.Forms.LinkLabel concreteCombosLinkLabel;
        private System.Windows.Forms.LinkLabel steelCombosLinkLabel;
        private System.Windows.Forms.Label spectrumFactorLabel;
        private System.Windows.Forms.NumericUpDown responseSpectrumFactorUpDown;
        private System.Windows.Forms.LinkLabel viewSpectrumLink;
        private System.Windows.Forms.CheckBox pDeltaCheckBox;
        private System.Windows.Forms.NumericUpDown dampingFactorUpDown;
        private System.Windows.Forms.Label dampingFactorLabel;
        private RuntimeData.CachedGeneralReport cachedGeneralReport1;
    }
}