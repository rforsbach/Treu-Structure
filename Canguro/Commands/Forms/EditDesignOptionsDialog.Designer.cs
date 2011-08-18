namespace Canguro.Commands.Forms
{
    partial class EditDesignOptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditDesignOptionsDialog));
            this.cancelButtom = new System.Windows.Forms.Button();
            this.acceptButton = new System.Windows.Forms.Button();
            this.editPanel = new System.Windows.Forms.Panel();
            this.editPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.defaultsButton = new System.Windows.Forms.Button();
            this.editPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButtom
            // 
            this.cancelButtom.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButtom, "cancelButtom");
            this.cancelButtom.Name = "cancelButtom";
            this.cancelButtom.UseVisualStyleBackColor = true;
            // 
            // acceptButton
            // 
            this.acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.acceptButton, "acceptButton");
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.UseVisualStyleBackColor = true;
            // 
            // editPanel
            // 
            this.editPanel.Controls.Add(this.editPropertyGrid);
            resources.ApplyResources(this.editPanel, "editPanel");
            this.editPanel.Name = "editPanel";
            // 
            // editPropertyGrid
            // 
            resources.ApplyResources(this.editPropertyGrid, "editPropertyGrid");
            this.editPropertyGrid.Name = "editPropertyGrid";
            this.editPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.editPropertyGrid.ToolbarVisible = false;
            // 
            // defaultsButton
            // 
            resources.ApplyResources(this.defaultsButton, "defaultsButton");
            this.defaultsButton.Name = "defaultsButton";
            this.defaultsButton.UseVisualStyleBackColor = true;
            this.defaultsButton.Click += new System.EventHandler(this.defaultsButton_Click);
            // 
            // EditDesignOptionsDialog
            // 
            this.AcceptButton = this.acceptButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButtom;
            this.Controls.Add(this.defaultsButton);
            this.Controls.Add(this.editPanel);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.cancelButtom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditDesignOptionsDialog";
            this.ShowInTaskbar = false;
            this.editPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButtom;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Panel editPanel;
        private System.Windows.Forms.PropertyGrid editPropertyGrid;
        private System.Windows.Forms.Button defaultsButton;
    }
}