namespace Canguro.Commands.Forms
{
    partial class ConstraintsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConstraintsDialog));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.wizardControl = new Canguro.Commands.Forms.WizardControl();
            this.mainPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.constraintsListBox = new System.Windows.Forms.ListBox();
            this.materialsToolStrip = new System.Windows.Forms.ToolStrip();
            this.addButton = new System.Windows.Forms.ToolStripButton();
            this.editButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.editPage = new System.Windows.Forms.TabPage();
            this.backButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.wizardControl.SuspendLayout();
            this.mainPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.materialsToolStrip.SuspendLayout();
            this.editPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            resources.ApplyResources(this.BottomToolStripPanel, "BottomToolStripPanel");
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // LeftToolStripPanel
            // 
            resources.ApplyResources(this.LeftToolStripPanel, "LeftToolStripPanel");
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // RightToolStripPanel
            // 
            resources.ApplyResources(this.RightToolStripPanel, "RightToolStripPanel");
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // TopToolStripPanel
            // 
            resources.ApplyResources(this.TopToolStripPanel, "TopToolStripPanel");
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // wizardControl
            // 
            this.wizardControl.Controls.Add(this.mainPage);
            this.wizardControl.Controls.Add(this.editPage);
            resources.ApplyResources(this.wizardControl, "wizardControl");
            this.wizardControl.Multiline = true;
            this.wizardControl.Name = "wizardControl";
            this.wizardControl.SelectedIndex = 0;
            this.wizardControl.TabStop = false;
            this.wizardControl.SelectedIndexChanged += new System.EventHandler(this.wizardControl_SelectedIndexChanged);
            // 
            // mainPage
            // 
            this.mainPage.Controls.Add(this.panel1);
            this.mainPage.Controls.Add(this.cancelButton);
            this.mainPage.Controls.Add(this.okButton);
            resources.ApplyResources(this.mainPage, "mainPage");
            this.mainPage.Name = "mainPage";
            this.mainPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.constraintsListBox);
            this.panel1.Controls.Add(this.materialsToolStrip);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // constraintsListBox
            // 
            this.constraintsListBox.BackColor = System.Drawing.Color.White;
            this.constraintsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.constraintsListBox, "constraintsListBox");
            this.constraintsListBox.FormattingEnabled = true;
            this.constraintsListBox.Name = "constraintsListBox";
            this.constraintsListBox.SelectedIndexChanged += new System.EventHandler(this.constraintsListBox_SelectedIndexChanged);
            this.constraintsListBox.DoubleClick += new System.EventHandler(this.constraintsListBox_DoubleClick);
            // 
            // materialsToolStrip
            // 
            this.materialsToolStrip.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.materialsToolStrip, "materialsToolStrip");
            this.materialsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.materialsToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.materialsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addButton,
            this.editButton,
            this.deleteButton});
            this.materialsToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.materialsToolStrip.Name = "materialsToolStrip";
            this.materialsToolStrip.Stretch = true;
            this.materialsToolStrip.TabStop = true;
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Image = global::Canguro.Properties.Resources.add32;
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Image = global::Canguro.Properties.Resources.edit32;
            this.editButton.Name = "editButton";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Image = global::Canguro.Properties.Resources.delete32;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // editPage
            // 
            this.editPage.Controls.Add(this.backButton);
            this.editPage.Controls.Add(this.applyButton);
            this.editPage.Controls.Add(this.nameTextBox);
            this.editPage.Controls.Add(this.nameLabel);
            resources.ApplyResources(this.editPage, "editPage");
            this.editPage.Name = "editPage";
            this.editPage.UseVisualStyleBackColor = true;
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
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // ConstraintsDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.BottomToolStripPanel);
            this.Controls.Add(this.wizardControl);
            this.Controls.Add(this.LeftToolStripPanel);
            this.Controls.Add(this.RightToolStripPanel);
            this.Controls.Add(this.TopToolStripPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConstraintsDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ConstraintsDialog_Load);
            this.wizardControl.ResumeLayout(false);
            this.mainPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.materialsToolStrip.ResumeLayout(false);
            this.materialsToolStrip.PerformLayout();
            this.editPage.ResumeLayout(false);
            this.editPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox constraintsListBox;
        private System.Windows.Forms.ToolStrip materialsToolStrip;
        private System.Windows.Forms.ToolStripButton addButton;
        private System.Windows.Forms.ToolStripButton editButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TabPage editPage;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Button okButton;
        private WizardControl wizardControl;
        private System.Windows.Forms.TabPage mainPage;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
    }
}