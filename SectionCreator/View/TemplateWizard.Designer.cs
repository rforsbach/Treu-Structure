namespace Canguro.SectionCreator
{
    partial class TemplateWizard
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateWizard));
            this.wizardControl = new Canguro.Commands.Forms.WizardControl();
            this.tab0Welcome = new System.Windows.Forms.TabPage();
            this.cancelButton = new System.Windows.Forms.Button();
            this.newSectionFromTemplateButton = new System.Windows.Forms.Button();
            this.newTemplateButton = new System.Windows.Forms.Button();
            this.newSectionButton = new System.Windows.Forms.Button();
            this.tab1Variables = new System.Windows.Forms.TabPage();
            this.okVariablesButton = new System.Windows.Forms.Button();
            this.cancelVariablesButton = new System.Windows.Forms.Button();
            this.variablesHelpLabel = new System.Windows.Forms.Label();
            this.variablesGridView = new System.Windows.Forms.DataGridView();
            this.Variables = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefaultValues = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.templateVariableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tab2Template = new System.Windows.Forms.TabPage();
            this.createButton = new System.Windows.Forms.Button();
            this.saveTemplateButton = new System.Windows.Forms.Button();
            this.cancelTemplateButton = new System.Windows.Forms.Button();
            this.templatePanel = new Canguro.SectionCreator.TemplatePanel();
            this.tab3FromTemplate = new System.Windows.Forms.TabPage();
            this.fromTemplatePanel = new Canguro.SectionCreator.TemplatePanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.variableValuesGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.addPointButton = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.wizardControl.SuspendLayout();
            this.tab0Welcome.SuspendLayout();
            this.tab1Variables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.variablesGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.templateVariableBindingSource)).BeginInit();
            this.tab2Template.SuspendLayout();
            this.tab3FromTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.variableValuesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardControl
            // 
            this.wizardControl.Controls.Add(this.tab0Welcome);
            this.wizardControl.Controls.Add(this.tab1Variables);
            this.wizardControl.Controls.Add(this.tab2Template);
            this.wizardControl.Controls.Add(this.tab3FromTemplate);
            this.wizardControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl.Location = new System.Drawing.Point(0, 0);
            this.wizardControl.Name = "wizardControl";
            this.wizardControl.SelectedIndex = 0;
            this.wizardControl.Size = new System.Drawing.Size(430, 267);
            this.wizardControl.TabIndex = 0;
            this.wizardControl.SelectedIndexChanged += new System.EventHandler(this.wizardControl_SelectedIndexChanged);
            // 
            // tab0Welcome
            // 
            this.tab0Welcome.Controls.Add(this.cancelButton);
            this.tab0Welcome.Controls.Add(this.newSectionFromTemplateButton);
            this.tab0Welcome.Controls.Add(this.newTemplateButton);
            this.tab0Welcome.Controls.Add(this.newSectionButton);
            this.tab0Welcome.Location = new System.Drawing.Point(4, 23);
            this.tab0Welcome.Name = "tab0Welcome";
            this.tab0Welcome.Padding = new System.Windows.Forms.Padding(3);
            this.tab0Welcome.Size = new System.Drawing.Size(422, 240);
            this.tab0Welcome.TabIndex = 0;
            this.tab0Welcome.Text = "Welcome";
            this.tab0Welcome.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(8, 174);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(200, 50);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // newSectionFromTemplateButton
            // 
            this.newSectionFromTemplateButton.Location = new System.Drawing.Point(8, 118);
            this.newSectionFromTemplateButton.Name = "newSectionFromTemplateButton";
            this.newSectionFromTemplateButton.Size = new System.Drawing.Size(200, 50);
            this.newSectionFromTemplateButton.TabIndex = 2;
            this.newSectionFromTemplateButton.Text = "New Section from Template";
            this.newSectionFromTemplateButton.UseVisualStyleBackColor = true;
            this.newSectionFromTemplateButton.Click += new System.EventHandler(this.newSectionFromTemplateButton_Click);
            // 
            // newTemplateButton
            // 
            this.newTemplateButton.Location = new System.Drawing.Point(8, 62);
            this.newTemplateButton.Name = "newTemplateButton";
            this.newTemplateButton.Size = new System.Drawing.Size(200, 50);
            this.newTemplateButton.TabIndex = 1;
            this.newTemplateButton.Text = "New Template";
            this.newTemplateButton.UseVisualStyleBackColor = true;
            this.newTemplateButton.Click += new System.EventHandler(this.newTemplateButton_Click);
            // 
            // newSectionButton
            // 
            this.newSectionButton.Location = new System.Drawing.Point(8, 6);
            this.newSectionButton.Name = "newSectionButton";
            this.newSectionButton.Size = new System.Drawing.Size(200, 50);
            this.newSectionButton.TabIndex = 0;
            this.newSectionButton.Text = "New Section";
            this.newSectionButton.UseVisualStyleBackColor = true;
            this.newSectionButton.Click += new System.EventHandler(this.newSectionButton_Click);
            // 
            // tab1Variables
            // 
            this.tab1Variables.Controls.Add(this.okVariablesButton);
            this.tab1Variables.Controls.Add(this.cancelVariablesButton);
            this.tab1Variables.Controls.Add(this.variablesHelpLabel);
            this.tab1Variables.Controls.Add(this.variablesGridView);
            this.tab1Variables.Location = new System.Drawing.Point(4, 23);
            this.tab1Variables.Name = "tab1Variables";
            this.tab1Variables.Padding = new System.Windows.Forms.Padding(3);
            this.tab1Variables.Size = new System.Drawing.Size(422, 240);
            this.tab1Variables.TabIndex = 3;
            this.tab1Variables.Text = "Variables";
            this.tab1Variables.UseVisualStyleBackColor = true;
            // 
            // okVariablesButton
            // 
            this.okVariablesButton.Location = new System.Drawing.Point(258, 228);
            this.okVariablesButton.Name = "okVariablesButton";
            this.okVariablesButton.Size = new System.Drawing.Size(75, 23);
            this.okVariablesButton.TabIndex = 5;
            this.okVariablesButton.Text = "Next >>";
            this.okVariablesButton.UseVisualStyleBackColor = true;
            this.okVariablesButton.Click += new System.EventHandler(this.okVariablesButton_Click);
            // 
            // cancelVariablesButton
            // 
            this.cancelVariablesButton.Location = new System.Drawing.Point(339, 228);
            this.cancelVariablesButton.Name = "cancelVariablesButton";
            this.cancelVariablesButton.Size = new System.Drawing.Size(75, 23);
            this.cancelVariablesButton.TabIndex = 4;
            this.cancelVariablesButton.Text = "Cancel";
            this.cancelVariablesButton.UseVisualStyleBackColor = true;
            this.cancelVariablesButton.Click += new System.EventHandler(this.cancelVariablesButton_Click);
            // 
            // variablesHelpLabel
            // 
            this.variablesHelpLabel.Location = new System.Drawing.Point(217, 6);
            this.variablesHelpLabel.Name = "variablesHelpLabel";
            this.variablesHelpLabel.Size = new System.Drawing.Size(171, 71);
            this.variablesHelpLabel.TabIndex = 1;
            this.variablesHelpLabel.Text = "Please add variables and default values to use in the template.";
            // 
            // variablesGridView
            // 
            this.variablesGridView.AutoGenerateColumns = false;
            this.variablesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.variablesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Variables,
            this.DefaultValues,
            this.nameDataGridViewTextBoxColumn,
            this.valueDataGridViewTextBoxColumn});
            this.variablesGridView.DataSource = this.templateVariableBindingSource;
            this.variablesGridView.Location = new System.Drawing.Point(8, 6);
            this.variablesGridView.Name = "variablesGridView";
            this.variablesGridView.RowHeadersVisible = false;
            this.variablesGridView.Size = new System.Drawing.Size(203, 226);
            this.variablesGridView.TabIndex = 0;
            // 
            // Variables
            // 
            this.Variables.HeaderText = "Variables";
            this.Variables.Name = "Variables";
            // 
            // DefaultValues
            // 
            this.DefaultValues.HeaderText = "Default Values";
            this.DefaultValues.Name = "DefaultValues";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // valueDataGridViewTextBoxColumn
            // 
            this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
            this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
            // 
            // templateVariableBindingSource
            // 
            this.templateVariableBindingSource.DataSource = typeof(Canguro.SectionCreator.TemplateVariable);
            // 
            // tab2Template
            // 
            this.tab2Template.Controls.Add(this.button5);
            this.tab2Template.Controls.Add(this.button4);
            this.tab2Template.Controls.Add(this.button3);
            this.tab2Template.Controls.Add(this.listBox1);
            this.tab2Template.Controls.Add(this.addPointButton);
            this.tab2Template.Controls.Add(this.textBox1);
            this.tab2Template.Controls.Add(this.createButton);
            this.tab2Template.Controls.Add(this.saveTemplateButton);
            this.tab2Template.Controls.Add(this.cancelTemplateButton);
            this.tab2Template.Controls.Add(this.templatePanel);
            this.tab2Template.Location = new System.Drawing.Point(4, 23);
            this.tab2Template.Name = "tab2Template";
            this.tab2Template.Padding = new System.Windows.Forms.Padding(3);
            this.tab2Template.Size = new System.Drawing.Size(422, 240);
            this.tab2Template.TabIndex = 1;
            this.tab2Template.Text = "Template";
            this.tab2Template.UseVisualStyleBackColor = true;
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(163, 228);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(89, 23);
            this.createButton.TabIndex = 4;
            this.createButton.Text = "Create Section";
            this.createButton.UseVisualStyleBackColor = true;
            // 
            // saveTemplateButton
            // 
            this.saveTemplateButton.Location = new System.Drawing.Point(258, 228);
            this.saveTemplateButton.Name = "saveTemplateButton";
            this.saveTemplateButton.Size = new System.Drawing.Size(75, 23);
            this.saveTemplateButton.TabIndex = 3;
            this.saveTemplateButton.Text = "Save";
            this.saveTemplateButton.UseVisualStyleBackColor = true;
            // 
            // cancelTemplateButton
            // 
            this.cancelTemplateButton.Location = new System.Drawing.Point(339, 228);
            this.cancelTemplateButton.Name = "cancelTemplateButton";
            this.cancelTemplateButton.Size = new System.Drawing.Size(75, 23);
            this.cancelTemplateButton.TabIndex = 2;
            this.cancelTemplateButton.Text = "Cancel";
            this.cancelTemplateButton.UseVisualStyleBackColor = true;
            this.cancelTemplateButton.Click += new System.EventHandler(this.cancelTemplateButton_Click);
            // 
            // templatePanel
            // 
            this.templatePanel.Location = new System.Drawing.Point(197, 7);
            this.templatePanel.Name = "templatePanel";
            this.templatePanel.Size = new System.Drawing.Size(217, 215);
            this.templatePanel.TabIndex = 1;
            // 
            // tab3FromTemplate
            // 
            this.tab3FromTemplate.Controls.Add(this.fromTemplatePanel);
            this.tab3FromTemplate.Controls.Add(this.button1);
            this.tab3FromTemplate.Controls.Add(this.button2);
            this.tab3FromTemplate.Controls.Add(this.variableValuesGridView);
            this.tab3FromTemplate.Location = new System.Drawing.Point(4, 23);
            this.tab3FromTemplate.Name = "tab3FromTemplate";
            this.tab3FromTemplate.Padding = new System.Windows.Forms.Padding(3);
            this.tab3FromTemplate.Size = new System.Drawing.Size(422, 240);
            this.tab3FromTemplate.TabIndex = 2;
            this.tab3FromTemplate.Text = "From Template";
            this.tab3FromTemplate.UseVisualStyleBackColor = true;
            // 
            // fromTemplatePanel
            // 
            this.fromTemplatePanel.Location = new System.Drawing.Point(197, 6);
            this.fromTemplatePanel.Name = "fromTemplatePanel";
            this.fromTemplatePanel.Size = new System.Drawing.Size(217, 215);
            this.fromTemplatePanel.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(258, 228);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(339, 228);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // variableValuesGridView
            // 
            this.variableValuesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.variableValuesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.variableValuesGridView.Location = new System.Drawing.Point(8, 6);
            this.variableValuesGridView.Name = "variableValuesGridView";
            this.variableValuesGridView.RowHeadersVisible = false;
            this.variableValuesGridView.Size = new System.Drawing.Size(183, 214);
            this.variableValuesGridView.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Variables";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 90;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Values";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 90;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(148, 20);
            this.textBox1.TabIndex = 5;
            // 
            // addPointButton
            // 
            this.addPointButton.Location = new System.Drawing.Point(160, 5);
            this.addPointButton.Name = "addPointButton";
            this.addPointButton.Size = new System.Drawing.Size(23, 23);
            this.addPointButton.TabIndex = 6;
            this.addPointButton.Text = "+";
            this.addPointButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.addPointButton.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 33);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(148, 186);
            this.listBox1.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(160, 34);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "-";
            this.button3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(160, 63);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(23, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "<";
            this.button4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(160, 92);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(23, 23);
            this.button5.TabIndex = 10;
            this.button5.Text = ">";
            this.button5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button5.UseVisualStyleBackColor = true;
            // 
            // TemplateWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 267);
            this.Controls.Add(this.wizardControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateWizard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Section";
            this.Load += new System.EventHandler(this.TemplateWizard_Load);
            this.wizardControl.ResumeLayout(false);
            this.tab0Welcome.ResumeLayout(false);
            this.tab1Variables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.variablesGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.templateVariableBindingSource)).EndInit();
            this.tab2Template.ResumeLayout(false);
            this.tab2Template.PerformLayout();
            this.tab3FromTemplate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.variableValuesGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Canguro.Commands.Forms.WizardControl wizardControl;
        private System.Windows.Forms.TabPage tab0Welcome;
        private System.Windows.Forms.TabPage tab1Variables;
        private System.Windows.Forms.TabPage tab2Template;
        private System.Windows.Forms.TabPage tab3FromTemplate;
        private System.Windows.Forms.Button newSectionFromTemplateButton;
        private System.Windows.Forms.Button newTemplateButton;
        private System.Windows.Forms.Button newSectionButton;
        private System.Windows.Forms.Button saveTemplateButton;
        private System.Windows.Forms.Button cancelTemplateButton;
        private TemplatePanel templatePanel;
        private System.Windows.Forms.Label variablesHelpLabel;
        private System.Windows.Forms.DataGridView variablesGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variables;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefaultValues;
        private System.Windows.Forms.Button okVariablesButton;
        private System.Windows.Forms.Button cancelVariablesButton;
        private System.Windows.Forms.DataGridView variableValuesGridView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private TemplatePanel fromTemplatePanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource templateVariableBindingSource;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button addPointButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
    }
}