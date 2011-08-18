namespace Canguro.Controller.Grid
{
    partial class JointDOFControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JointDOFControl));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.comboT1 = new System.Windows.Forms.ComboBox();
            this.spring1 = new System.Windows.Forms.TextBox();
            this.comboT2 = new System.Windows.Forms.ComboBox();
            this.spring2 = new System.Windows.Forms.TextBox();
            this.comboT3 = new System.Windows.Forms.ComboBox();
            this.spring3 = new System.Windows.Forms.TextBox();
            this.comboR1 = new System.Windows.Forms.ComboBox();
            this.spring4 = new System.Windows.Forms.TextBox();
            this.comboR2 = new System.Windows.Forms.ComboBox();
            this.spring5 = new System.Windows.Forms.TextBox();
            this.comboR3 = new System.Windows.Forms.ComboBox();
            this.spring6 = new System.Windows.Forms.TextBox();
            this.statusText = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.comboT1);
            this.flowLayoutPanel1.Controls.Add(this.spring1);
            this.flowLayoutPanel1.Controls.Add(this.comboT2);
            this.flowLayoutPanel1.Controls.Add(this.spring2);
            this.flowLayoutPanel1.Controls.Add(this.comboT3);
            this.flowLayoutPanel1.Controls.Add(this.spring3);
            this.flowLayoutPanel1.Controls.Add(this.comboR1);
            this.flowLayoutPanel1.Controls.Add(this.spring4);
            this.flowLayoutPanel1.Controls.Add(this.comboR2);
            this.flowLayoutPanel1.Controls.Add(this.spring5);
            this.flowLayoutPanel1.Controls.Add(this.comboR3);
            this.flowLayoutPanel1.Controls.Add(this.spring6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // comboT1
            // 
            this.comboT1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboT1.DropDownWidth = 80;
            resources.ApplyResources(this.comboT1, "comboT1");
            this.comboT1.FormattingEnabled = true;
            this.comboT1.Items.AddRange(new object[] {
            resources.GetString("comboT1.Items"),
            resources.GetString("comboT1.Items1"),
            resources.GetString("comboT1.Items2")});
            this.comboT1.Name = "comboT1";
            this.comboT1.Tag = "translation dofLocalAxisStr 1";
            this.comboT1.Enter += new System.EventHandler(this.combo_Enter);
            this.comboT1.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            this.comboT1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // spring1
            // 
            resources.ApplyResources(this.spring1, "spring1");
            this.spring1.Name = "spring1";
            this.spring1.Tag = "dofSpringConstantStr translation dofLocalAxisStr 1)";
            this.spring1.Enter += new System.EventHandler(this.spring_Enter);
            this.spring1.TextChanged += new System.EventHandler(this.spring_TextChanged);
            this.spring1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // comboT2
            // 
            this.comboT2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboT2.DropDownWidth = 80;
            resources.ApplyResources(this.comboT2, "comboT2");
            this.comboT2.FormattingEnabled = true;
            this.comboT2.Items.AddRange(new object[] {
            resources.GetString("comboT2.Items"),
            resources.GetString("comboT2.Items1"),
            resources.GetString("comboT2.Items2")});
            this.comboT2.Name = "comboT2";
            this.comboT2.Tag = "translation dofLocalAxisStr 2";
            this.comboT2.Enter += new System.EventHandler(this.combo_Enter);
            this.comboT2.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            this.comboT2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // spring2
            // 
            resources.ApplyResources(this.spring2, "spring2");
            this.spring2.Name = "spring2";
            this.spring2.Tag = "dofSpringConstantStr translation dofLocalAxisStr 2)";
            this.spring2.Enter += new System.EventHandler(this.spring_Enter);
            this.spring2.TextChanged += new System.EventHandler(this.spring_TextChanged);
            this.spring2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // comboT3
            // 
            this.comboT3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboT3.DropDownWidth = 80;
            resources.ApplyResources(this.comboT3, "comboT3");
            this.comboT3.FormattingEnabled = true;
            this.comboT3.Items.AddRange(new object[] {
            resources.GetString("comboT3.Items"),
            resources.GetString("comboT3.Items1"),
            resources.GetString("comboT3.Items2")});
            this.comboT3.Name = "comboT3";
            this.comboT3.Tag = "translation dofLocalAxisStr 3";
            this.comboT3.Enter += new System.EventHandler(this.combo_Enter);
            this.comboT3.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            this.comboT3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // spring3
            // 
            resources.ApplyResources(this.spring3, "spring3");
            this.spring3.Name = "spring3";
            this.spring3.Tag = "dofSpringConstantStr translation dofLocalAxisStr 3)";
            this.spring3.Enter += new System.EventHandler(this.spring_Enter);
            this.spring3.TextChanged += new System.EventHandler(this.spring_TextChanged);
            this.spring3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // comboR1
            // 
            this.comboR1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboR1.DropDownWidth = 80;
            resources.ApplyResources(this.comboR1, "comboR1");
            this.comboR1.FormattingEnabled = true;
            this.comboR1.Items.AddRange(new object[] {
            resources.GetString("comboR1.Items"),
            resources.GetString("comboR1.Items1"),
            resources.GetString("comboR1.Items2")});
            this.comboR1.Name = "comboR1";
            this.comboR1.Tag = "rotation dofLocalAxisStr 1";
            this.comboR1.Enter += new System.EventHandler(this.combo_Enter);
            this.comboR1.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            this.comboR1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // spring4
            // 
            resources.ApplyResources(this.spring4, "spring4");
            this.spring4.Name = "spring4";
            this.spring4.Tag = "dofSpringConstantStr rotation dofLocalAxisStr 1)";
            this.spring4.Enter += new System.EventHandler(this.spring_Enter);
            this.spring4.TextChanged += new System.EventHandler(this.spring_TextChanged);
            this.spring4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // comboR2
            // 
            this.comboR2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboR2.DropDownWidth = 80;
            resources.ApplyResources(this.comboR2, "comboR2");
            this.comboR2.FormattingEnabled = true;
            this.comboR2.Items.AddRange(new object[] {
            resources.GetString("comboR2.Items"),
            resources.GetString("comboR2.Items1"),
            resources.GetString("comboR2.Items2")});
            this.comboR2.Name = "comboR2";
            this.comboR2.Tag = "rotation dofLocalAxisStr 2";
            this.comboR2.Enter += new System.EventHandler(this.combo_Enter);
            this.comboR2.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            this.comboR2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // spring5
            // 
            resources.ApplyResources(this.spring5, "spring5");
            this.spring5.Name = "spring5";
            this.spring5.Tag = "dofSpringConstantStr rotation dofLocalAxisStr 2)";
            this.spring5.Enter += new System.EventHandler(this.spring_Enter);
            this.spring5.TextChanged += new System.EventHandler(this.spring_TextChanged);
            this.spring5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // comboR3
            // 
            this.comboR3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboR3.DropDownWidth = 80;
            resources.ApplyResources(this.comboR3, "comboR3");
            this.comboR3.Items.AddRange(new object[] {
            resources.GetString("comboR3.Items"),
            resources.GetString("comboR3.Items1"),
            resources.GetString("comboR3.Items2")});
            this.comboR3.Name = "comboR3";
            this.comboR3.Tag = "rotation dofLocalAxisStr 3";
            this.comboR3.Enter += new System.EventHandler(this.combo_Enter);
            this.comboR3.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            this.comboR3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // spring6
            // 
            resources.ApplyResources(this.spring6, "spring6");
            this.spring6.Name = "spring6";
            this.spring6.Tag = "dofSpringConstantStr rotation dofLocalAxisStr 3)";
            this.spring6.Enter += new System.EventHandler(this.spring_Enter);
            this.spring6.TextChanged += new System.EventHandler(this.spring_TextChanged);
            this.spring6.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            // 
            // statusText
            // 
            resources.ApplyResources(this.statusText, "statusText");
            this.statusText.Name = "statusText";
            // 
            // JointDOFControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "JointDOFControl";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JointDOFControl_KeyDown);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox comboT1;
        private System.Windows.Forms.ComboBox comboT2;
        private System.Windows.Forms.ComboBox comboT3;
        private System.Windows.Forms.ComboBox comboR1;
        private System.Windows.Forms.ComboBox comboR2;
        private System.Windows.Forms.ComboBox comboR3;
        private System.Windows.Forms.TextBox spring1;
        private System.Windows.Forms.TextBox spring2;
        private System.Windows.Forms.TextBox spring3;
        private System.Windows.Forms.TextBox spring4;
        private System.Windows.Forms.TextBox spring5;
        private System.Windows.Forms.TextBox spring6;
        private System.Windows.Forms.Label statusText;






    }
}
