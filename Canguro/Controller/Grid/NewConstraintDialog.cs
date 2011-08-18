using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public partial class NewConstraintDialog : Form
    {
        Canguro.Model.Constraint constraint = null;

        public NewConstraintDialog()
        {
            InitializeComponent();
            okButton.Enabled = (nameTextBox.Text.Length > 0);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (nameTextBox.Text.Length > 0)
            {
                constraint = new Canguro.Model.Constraint(nameTextBox.Text);
                Canguro.Model.Model.Instance.ConstraintList.Add(constraint);
            }
        }

        public Canguro.Model.Constraint Constraint
        {
            get
            {
                return constraint;
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = (nameTextBox.Text.Length > 0);
        }
    }
}