using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;

namespace Canguro.Commands.Forms
{
    public partial class ConstraintsDialog : Form
    {
        Canguro.Model.Model model;
        Canguro.Model.Constraint current;

        public ConstraintsDialog(Canguro.Model.Model model)
        {
            this.model = model;
            InitializeComponent();
        }

        private void UpdateDialog()
        {
            constraintsListBox.Items.Clear();
            foreach (Canguro.Model.Constraint cons in model.ConstraintList)
                constraintsListBox.Items.Add(cons);
            if (current != null)
                constraintsListBox.SelectedItem = current;
        }

        private void ConstraintsDialog_Load(object sender, EventArgs e)
        {
            wizardControl.HideTabs = true;
            UpdateDialog();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            current = new Canguro.Model.Constraint();
            wizardControl.SelectTab(1);
        }

        private void constraintsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            current = (Canguro.Model.Constraint)constraintsListBox.SelectedItem;
            editButton.Enabled = (current != null);
            deleteButton.Enabled = (current != null);
        }

        private void constraintsListBox_DoubleClick(object sender, EventArgs e)
        {
            editButton_Click(sender, e);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (current != null)
                wizardControl.SelectTab(1);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (current != null)
            {
                model.ConstraintList.Remove(current);
                current = null;
                UpdateDialog();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (wizardControl.SelectedIndex == 1)
            {
                current.Name = nameTextBox.Text;
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }

        private void wizardControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (wizardControl.SelectedIndex)
            {
                case 0:
                    UpdateDialog();
                    CancelButton = cancelButton;
                    AcceptButton = okButton;
                    break;
                case 1:
                    nameTextBox.Text = current.Name;
                    CancelButton = backButton;
                    AcceptButton = applyButton;
                    break;
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            wizardControl.SelectTab(0);
            DialogResult = DialogResult.None;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (!model.ConstraintList.Contains(current))
                model.ConstraintList.Add(current);
            current.Name = nameTextBox.Text;
            wizardControl.SelectTab(0);
        }
    }
}