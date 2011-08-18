namespace Canguro.Commands.Forms
{
    partial class CommandToolbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandToolbox));
            this.comboList = new System.Windows.Forms.ComboBox();
            this.panelOkCancel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.properties = new System.Windows.Forms.PropertyGrid();
            this.panelOkCancel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboList
            // 
            resources.ApplyResources(this.comboList, "comboList");
            this.comboList.FormattingEnabled = true;
            this.comboList.Name = "comboList";
            // 
            // panelOkCancel
            // 
            this.panelOkCancel.Controls.Add(this.cancelButton);
            this.panelOkCancel.Controls.Add(this.okButton);
            resources.ApplyResources(this.panelOkCancel, "panelOkCancel");
            this.panelOkCancel.Name = "panelOkCancel";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // properties
            // 
            this.properties.CommandsVisibleIfAvailable = false;
            resources.ApplyResources(this.properties, "properties");
            this.properties.Name = "properties";
            this.properties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.properties.ToolbarVisible = false;
            // 
            // CommandToolbox
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.properties);
            this.Controls.Add(this.panelOkCancel);
            this.Controls.Add(this.comboList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommandToolbox";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CommandToolbox_FormClosing);
            this.panelOkCancel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboList;
        private System.Windows.Forms.Panel panelOkCancel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PropertyGrid properties;

    }
}