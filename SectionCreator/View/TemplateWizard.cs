using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.SectionCreator.Commands;

namespace Canguro.SectionCreator
{
    partial class TemplateWizard : Form
    {
        Model model;
        Controller controller;
        SectionTemplate currentTemplate = null;

        public TemplateWizard(Model model, Controller controller)
        {
            this.model = model;
            this.controller = controller;
            InitializeComponent();
        }

        private void TemplateWizard_Load(object sender, EventArgs e)
        {
            wizardControl.HideTabs = true;

            // Temp
            newSectionButton_Click(this, EventArgs.Empty);
            // End Temp
        }

        private void newSectionButton_Click(object sender, EventArgs e)
        {
            if (model.Modified)
            {
                DialogResult dr = MessageBox.Show(Culture.Get("askSaveChangesAndExit"), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    return;
                else if (dr == DialogResult.Yes)
                    new SaveCommand().Run(controller, model);
            }
            model.Reset();
            DialogResult = DialogResult.OK;
        }

        private void newTemplateButton_Click(object sender, EventArgs e)
        {
            currentTemplate = new SectionTemplate();
            wizardControl.SelectTab(tab1Variables);
        }

        private void newSectionFromTemplateButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelTemplateButton_Click(object sender, EventArgs e)
        {
            currentTemplate = null;
            wizardControl.SelectTab(tab0Welcome);

        }

        private void wizardControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (wizardControl.SelectedIndex)
            {
                case 1:
                    UpdateVariablesTab();
                    break;
                case 2:
                    UpdateTemplateTab();
                    break;
                case 3:
                    UpdateFromTemplateTab();
                    break;
            }
        }

        private void UpdateVariablesTab()
        {
            variablesGridView.DataSource = null;
            variablesGridView.Rows.Clear();
            if (currentTemplate.Variables.Count > 0)
            {
                variablesGridView.Rows.Add(currentTemplate.Variables.Count);
                for (int i = 0; i < currentTemplate.Variables.Count; i++)
                {
                    variablesGridView[0, i + 1].Value = currentTemplate.Variables[i].Name;
                    variablesGridView[1, i + 1].Value = currentTemplate.Variables[i].Value;
                }
            }
            CancelButton = cancelVariablesButton;
            AcceptButton = okVariablesButton;
        }

        private void UpdateTemplateTab()
        {
            CancelButton = cancelTemplateButton;
            AcceptButton = saveTemplateButton;
        }

        private void UpdateFromTemplateTab()
        {
        }

        private void cancelVariablesButton_Click(object sender, EventArgs e)
        {
            currentTemplate = null;
            wizardControl.SelectTab(tab0Welcome);
            DialogResult = DialogResult.None;
        }

        private void okVariablesButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in variablesGridView.Rows)
                {
                    object nam = row.Cells[0].Value;
                    object val = row.Cells[1].Value;
                    if (nam != null && !string.IsNullOrEmpty(nam.ToString()) && val != null && !string.IsNullOrEmpty(val.ToString()))
                        currentTemplate.Variables.Add(new TemplateVariable(nam.ToString(), double.Parse(val.ToString())));
                }
                wizardControl.SelectTab(tab2Template);
            }
            catch (Exception)
            {

            }
            DialogResult = DialogResult.None;
        }
    }
}