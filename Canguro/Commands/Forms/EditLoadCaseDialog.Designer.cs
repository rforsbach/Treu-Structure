namespace Canguro.Commands.Forms
{
    partial class EditLoadCaseDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditLoadCaseDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.autoLoadPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.swFactorLabel = new System.Windows.Forms.Label();
            this.swFactorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.typeLabel = new System.Windows.Forms.Label();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.autoLoadComboBox = new System.Windows.Forms.ComboBox();
            this.autoLoadLabel = new System.Windows.Forms.Label();
            this.autoLoadNameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.swFactorNumericUpDown)).BeginInit();
            this.SuspendLayout();
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
            // autoLoadPropertyGrid
            // 
            resources.ApplyResources(this.autoLoadPropertyGrid, "autoLoadPropertyGrid");
            this.autoLoadPropertyGrid.Name = "autoLoadPropertyGrid";
            this.autoLoadPropertyGrid.TabStop = false;
            this.autoLoadPropertyGrid.ToolbarVisible = false;
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
            // swFactorLabel
            // 
            resources.ApplyResources(this.swFactorLabel, "swFactorLabel");
            this.swFactorLabel.Name = "swFactorLabel";
            // 
            // swFactorNumericUpDown
            // 
            this.swFactorNumericUpDown.DecimalPlaces = 3;
            this.swFactorNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.swFactorNumericUpDown, "swFactorNumericUpDown");
            this.swFactorNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.swFactorNumericUpDown.Name = "swFactorNumericUpDown";
            this.swFactorNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // typeLabel
            // 
            resources.ApplyResources(this.typeLabel, "typeLabel");
            this.typeLabel.Name = "typeLabel";
            // 
            // typeComboBox
            // 
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.typeComboBox, "typeComboBox");
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            // 
            // autoLoadComboBox
            // 
            this.autoLoadComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.autoLoadComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.autoLoadComboBox, "autoLoadComboBox");
            this.autoLoadComboBox.Name = "autoLoadComboBox";
            this.autoLoadComboBox.SelectedIndexChanged += new System.EventHandler(this.autoLoadComboBox_SelectedIndexChanged);
            // 
            // autoLoadLabel
            // 
            resources.ApplyResources(this.autoLoadLabel, "autoLoadLabel");
            this.autoLoadLabel.Name = "autoLoadLabel";
            // 
            // autoLoadNameLabel
            // 
            resources.ApplyResources(this.autoLoadNameLabel, "autoLoadNameLabel");
            this.autoLoadNameLabel.Name = "autoLoadNameLabel";
            // 
            // EditLoadCaseDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.autoLoadNameLabel);
            this.Controls.Add(this.autoLoadLabel);
            this.Controls.Add(this.typeComboBox);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.swFactorNumericUpDown);
            this.Controls.Add(this.swFactorLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.autoLoadPropertyGrid);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.autoLoadComboBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditLoadCaseDialog";
            this.Load += new System.EventHandler(this.EditLoadCaseDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.swFactorNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PropertyGrid autoLoadPropertyGrid;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label swFactorLabel;
        private System.Windows.Forms.NumericUpDown swFactorNumericUpDown;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.ComboBox autoLoadComboBox;
        private System.Windows.Forms.Label autoLoadLabel;
        private System.Windows.Forms.Label autoLoadNameLabel;
    }
}