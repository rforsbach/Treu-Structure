namespace Canguro.Controller.Grid
{
    partial class CardinalPointControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardinalPointControl));
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.White;
            this.button10.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button10.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button10.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.button10.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.button10, "button10");
            this.button10.Name = "button10";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button11_MouseMove);
            this.button10.Click += new System.EventHandler(this.button_Clicked);
            this.button10.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CardinalPointControl_KeyDown);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.White;
            this.button11.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button11.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button11.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.button11.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.button11, "button11");
            this.button11.Name = "button11";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button11_MouseMove);
            this.button11.Click += new System.EventHandler(this.button_Clicked);
            this.button11.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CardinalPointControl_KeyDown);
            // 
            // CardinalPointControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::Canguro.Properties.Resources.CardinalPointBack;
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.DoubleBuffered = true;
            this.Name = "CardinalPointControl";
            this.Load += new System.EventHandler(this.CardinalPointControl_Load);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CardinalPointControl_PreviewKeyDown);
            this.Click += new System.EventHandler(this.button_Clicked);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button11_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CardinalPointControl_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.ToolTip toolTip1;

    }
}
