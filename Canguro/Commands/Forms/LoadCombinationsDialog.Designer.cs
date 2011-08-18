namespace Canguro.Commands.Forms
{
    partial class LoadCombinationsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadCombinationsDialog));
            this.wizardControl = new Canguro.Commands.Forms.WizardControl();
            this.mainPage = new System.Windows.Forms.TabPage();
            this.responseSpectrumCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboListBox = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addButton = new System.Windows.Forms.ToolStripButton();
            this.editButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.editTabPage = new System.Windows.Forms.TabPage();
            this.backButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.casesGridView = new System.Windows.Forms.DataGridView();
            this.removeAllCasesButton = new System.Windows.Forms.Button();
            this.removeCaseButton = new System.Windows.Forms.Button();
            this.addCaseButton = new System.Windows.Forms.Button();
            this.addAllCasesButton = new System.Windows.Forms.Button();
            this.casesListBox = new System.Windows.Forms.ListBox();
            this.typeLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.wizardControl.SuspendLayout();
            this.mainPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.editTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.casesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardControl
            // 
            this.wizardControl.Controls.Add(this.mainPage);
            this.wizardControl.Controls.Add(this.editTabPage);
            resources.ApplyResources(this.wizardControl, "wizardControl");
            this.wizardControl.Multiline = true;
            this.wizardControl.Name = "wizardControl";
            this.wizardControl.SelectedIndex = 0;
            this.wizardControl.SelectedIndexChanged += new System.EventHandler(this.wizardControl_SelectedIndexChanged);
            // 
            // mainPage
            // 
            this.mainPage.Controls.Add(this.responseSpectrumCheckBox);
            this.mainPage.Controls.Add(this.okButton);
            this.mainPage.Controls.Add(this.panel1);
            this.mainPage.Controls.Add(this.cancelButton);
            resources.ApplyResources(this.mainPage, "mainPage");
            this.mainPage.Name = "mainPage";
            this.mainPage.UseVisualStyleBackColor = true;
            // 
            // responseSpectrumCheckBox
            // 
            resources.ApplyResources(this.responseSpectrumCheckBox, "responseSpectrumCheckBox");
            this.responseSpectrumCheckBox.Name = "responseSpectrumCheckBox";
            this.responseSpectrumCheckBox.UseVisualStyleBackColor = true;
            this.responseSpectrumCheckBox.CheckedChanged += new System.EventHandler(this.responseSpectrumCheckBox_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.comboListBox);
            this.panel1.Controls.Add(this.toolStrip1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // comboListBox
            // 
            this.comboListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.comboListBox, "comboListBox");
            this.comboListBox.FormattingEnabled = true;
            this.comboListBox.Name = "comboListBox";
            this.comboListBox.SelectedIndexChanged += new System.EventHandler(this.comboListBox_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addButton,
            this.editButton,
            this.deleteButton});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // addButton
            // 
            this.addButton.Image = global::Canguro.Properties.Resources.add32;
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Image = global::Canguro.Properties.Resources.edit32;
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Name = "editButton";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Image = global::Canguro.Properties.Resources.delete32;
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // editTabPage
            // 
            this.editTabPage.Controls.Add(this.backButton);
            this.editTabPage.Controls.Add(this.applyButton);
            this.editTabPage.Controls.Add(this.casesGridView);
            this.editTabPage.Controls.Add(this.removeAllCasesButton);
            this.editTabPage.Controls.Add(this.removeCaseButton);
            this.editTabPage.Controls.Add(this.addCaseButton);
            this.editTabPage.Controls.Add(this.addAllCasesButton);
            this.editTabPage.Controls.Add(this.casesListBox);
            this.editTabPage.Controls.Add(this.typeLabel);
            this.editTabPage.Controls.Add(this.nameTextBox);
            this.editTabPage.Controls.Add(this.typeComboBox);
            this.editTabPage.Controls.Add(this.nameLabel);
            resources.ApplyResources(this.editTabPage, "editTabPage");
            this.editTabPage.Name = "editTabPage";
            this.editTabPage.UseVisualStyleBackColor = true;
            // 
            // backButton
            // 
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.Name = "backButton";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // applyButton
            // 
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.Name = "applyButton";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // casesGridView
            // 
            this.casesGridView.AllowUserToAddRows = false;
            this.casesGridView.AllowUserToDeleteRows = false;
            this.casesGridView.AllowUserToResizeRows = false;
            this.casesGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.casesGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.casesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.casesGridView, "casesGridView");
            this.casesGridView.Name = "casesGridView";
            this.casesGridView.RowHeadersVisible = false;
            this.casesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.casesGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.casesGridView_EditingControlShowing);
            this.casesGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.casesGridView_DataError);
            // 
            // removeAllCasesButton
            // 
            resources.ApplyResources(this.removeAllCasesButton, "removeAllCasesButton");
            this.removeAllCasesButton.Name = "removeAllCasesButton";
            this.removeAllCasesButton.UseVisualStyleBackColor = true;
            this.removeAllCasesButton.Click += new System.EventHandler(this.removeAllCasesButton_Click);
            // 
            // removeCaseButton
            // 
            resources.ApplyResources(this.removeCaseButton, "removeCaseButton");
            this.removeCaseButton.Name = "removeCaseButton";
            this.removeCaseButton.UseVisualStyleBackColor = true;
            this.removeCaseButton.Click += new System.EventHandler(this.removeCaseButton_Click);
            // 
            // addCaseButton
            // 
            resources.ApplyResources(this.addCaseButton, "addCaseButton");
            this.addCaseButton.Name = "addCaseButton";
            this.addCaseButton.UseVisualStyleBackColor = true;
            this.addCaseButton.Click += new System.EventHandler(this.addCaseButton_Click);
            // 
            // addAllCasesButton
            // 
            resources.ApplyResources(this.addAllCasesButton, "addAllCasesButton");
            this.addAllCasesButton.Name = "addAllCasesButton";
            this.addAllCasesButton.UseVisualStyleBackColor = true;
            this.addAllCasesButton.Click += new System.EventHandler(this.addAllCasesButton_Click);
            // 
            // casesListBox
            // 
            this.casesListBox.FormattingEnabled = true;
            resources.ApplyResources(this.casesListBox, "casesListBox");
            this.casesListBox.Name = "casesListBox";
            this.casesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // typeLabel
            // 
            resources.ApplyResources(this.typeLabel, "typeLabel");
            this.typeLabel.Name = "typeLabel";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            // 
            // typeComboBox
            // 
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.typeComboBox, "typeComboBox");
            this.typeComboBox.Name = "typeComboBox";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // LoadCombinationsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.wizardControl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadCombinationsDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.LoadCombinationsDialog_Load);
            this.wizardControl.ResumeLayout(false);
            this.mainPage.ResumeLayout(false);
            this.mainPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.editTabPage.ResumeLayout(false);
            this.editTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.casesGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private WizardControl wizardControl;
        private System.Windows.Forms.TabPage mainPage;
        private System.Windows.Forms.TabPage editTabPage;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addButton;
        private System.Windows.Forms.ToolStripButton editButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ListBox comboListBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button removeAllCasesButton;
        private System.Windows.Forms.Button removeCaseButton;
        private System.Windows.Forms.Button addCaseButton;
        private System.Windows.Forms.Button addAllCasesButton;
        private System.Windows.Forms.ListBox casesListBox;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.DataGridView casesGridView;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox responseSpectrumCheckBox;
    }
}