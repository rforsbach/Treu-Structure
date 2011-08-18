namespace Canguro.Controller.Grid
{
    partial class JointConstraintControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JointConstraintControl));
            this.constraintComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // constraintComboBox
            // 
            resources.ApplyResources(this.constraintComboBox, "constraintComboBox");
            this.constraintComboBox.FormattingEnabled = true;
            this.constraintComboBox.Name = "constraintComboBox";
            this.constraintComboBox.SelectedIndexChanged += new System.EventHandler(this.constraintComboBox_SelectedIndexChanged);
            // 
            // JointConstraintControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.constraintComboBox);
            this.DoubleBuffered = true;
            this.Name = "JointConstraintControl";
            this.Load += new System.EventHandler(this.JointConstraintControl_Load);
            this.Click += new System.EventHandler(this.button_Clicked);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointConstraintControl_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox constraintComboBox;


    }
}
