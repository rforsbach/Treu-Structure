namespace Canguro.Controller.Grid
{
    partial class EndOffsetsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EndOffsetsControl));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.offILabel = new System.Windows.Forms.Label();
            this.offJLabel = new System.Windows.Forms.Label();
            this.factorLabel = new System.Windows.Forms.Label();
            this.offITextBox = new System.Windows.Forms.TextBox();
            this.offJTextBox = new System.Windows.Forms.TextBox();
            this.factorTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // offILabel
            // 
            resources.ApplyResources(this.offILabel, "offILabel");
            this.offILabel.Name = "offILabel";
            // 
            // offJLabel
            // 
            resources.ApplyResources(this.offJLabel, "offJLabel");
            this.offJLabel.Name = "offJLabel";
            // 
            // factorLabel
            // 
            resources.ApplyResources(this.factorLabel, "factorLabel");
            this.factorLabel.Name = "factorLabel";
            // 
            // offITextBox
            // 
            resources.ApplyResources(this.offITextBox, "offITextBox");
            this.offITextBox.Name = "offITextBox";
            this.offITextBox.TextChanged += new System.EventHandler(this.offITextBox_TextChanged);
            this.offITextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.offITextBox_KeyDown);
            // 
            // offJTextBox
            // 
            resources.ApplyResources(this.offJTextBox, "offJTextBox");
            this.offJTextBox.Name = "offJTextBox";
            this.offJTextBox.TextChanged += new System.EventHandler(this.offITextBox_TextChanged);
            this.offJTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.offITextBox_KeyDown);
            // 
            // factorTextBox
            // 
            resources.ApplyResources(this.factorTextBox, "factorTextBox");
            this.factorTextBox.Name = "factorTextBox";
            this.factorTextBox.TextChanged += new System.EventHandler(this.offITextBox_TextChanged);
            this.factorTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.offITextBox_KeyDown);
            // 
            // EndOffsetsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.factorTextBox);
            this.Controls.Add(this.offJTextBox);
            this.Controls.Add(this.offITextBox);
            this.Controls.Add(this.factorLabel);
            this.Controls.Add(this.offJLabel);
            this.Controls.Add(this.offILabel);
            this.DoubleBuffered = true;
            this.Name = "EndOffsetsControl";
            this.Load += new System.EventHandler(this.EndOffsetsControl_Load);
            this.Click += new System.EventHandler(this.button_Clicked);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label offILabel;
        private System.Windows.Forms.Label offJLabel;
        private System.Windows.Forms.Label factorLabel;
        private System.Windows.Forms.TextBox offITextBox;
        private System.Windows.Forms.TextBox offJTextBox;
        private System.Windows.Forms.TextBox factorTextBox;

    }
}
