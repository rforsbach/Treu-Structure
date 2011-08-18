using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Load;

namespace Canguro.Commands.Forms
{
    public partial class LoadCombinationsDialog : Form
    {
        LoadCombination currentCombo;
        Canguro.Model.Model model;
        private readonly bool oneCombo;
        private bool updatingGridView = false;
        private List<ACaseFactorWrapper> ACaseFactors = new List<ACaseFactorWrapper>();
        private Dictionary<string, LoadCombination.CombinationType> combinationTypes = new Dictionary<string,LoadCombination.CombinationType>();

        public LoadCombinationsDialog(Canguro.Model.Model model, LoadCombination combo)
        {
            this.currentCombo = combo;
            this.model = model;
            InitializeComponent();
            wizardControl.HideTabs = true;
            oneCombo = (combo != null);
        }

        public LoadCombinationsDialog(Canguro.Model.Model model)
            : this(model, null)
        {
        }

        public LoadCombinationsDialog()
            : this(Canguro.Model.Model.Instance)
        {
        }

        private void LoadCombinationsDialog_Load(object sender, EventArgs e)
        {
            updateTypeComboBox();
            if (currentCombo == null)
            {
                wizardControl.SelectTab(0);
                updateComboListBox();
                updateMainPage();
            }
            else
            {
                wizardControl.SelectTab(1);
                updateEditPage(true);
            }
        }

        private void wizardControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (wizardControl.SelectedIndex)
            {
                case 0: updateMainPage(); break;
                case 1: updateEditPage(true); break;
            }
        }

        private void comboListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentCombo = (LoadCombination)comboListBox.SelectedItem;
            updateMainPage();
        }

        private void updateDialog()
        {
        }

        private void updateMainPage()
        {
            addButton.Enabled = true;
            editButton.Enabled = (currentCombo != null);
            deleteButton.Enabled = (currentCombo != null);
            foreach (AbstractCase aCase in model.AbstractCases)
                if (aCase is AnalysisCase && ((AnalysisCase)aCase).Properties is ResponseSpectrumCaseProps)
                {
                    responseSpectrumCheckBox.Checked = aCase.IsActive;
                    break;
                }
        }

        private void updateEditPage(bool updateName)
        {
            if (currentCombo == null)
                return;

            try
            {
                updatingGridView = true;
                if (updateName)
                {
                    nameTextBox.Text = currentCombo.Name;
                    
                    typeComboBox.SelectedItem = "CombinationType" + currentCombo.Type.ToString();
                }

                ACaseFactors.Clear();
                for (int i = 0; i < currentCombo.Cases.Count; i++)
                    ACaseFactors.Add(new ACaseFactorWrapper(currentCombo.Cases, i));


                ACaseFactorWrapper[] array = ACaseFactors.ToArray();
                if (array.Length > 0)
                    casesGridView.DataSource = array;
                else
                    casesGridView.DataSource = null;

                casesListBox.Items.Clear();
                foreach (AbstractCase acase in model.AbstractCases)
                    if (acase != null && !(acase is LoadCombination) && !Contains(currentCombo.Cases, acase) &&
                        !(acase is AnalysisCase && ((AnalysisCase)acase).Properties is ModalCaseProps))
                        casesListBox.Items.Add(acase);
            }
            finally
            {
                updatingGridView = false;
            }
        }

        private bool Contains(IList<AbstractCaseFactor> factors, AbstractCase acase)
        {
            foreach (AbstractCaseFactor fac in factors)
                if (acase.Name.Equals(fac.Case.Name))
                    return true;

            return false;
        }

        private void updateComboListBox()
        {
            comboListBox.Items.Clear();
            foreach (AbstractCase acase in model.AbstractCases)
                if (acase != null && acase is LoadCombination)
                    comboListBox.Items.Add(acase);
            if (currentCombo != null && comboListBox.Items.Contains(currentCombo))
                comboListBox.SelectedItem = currentCombo;
        }

        private void updateCasesListBox()
        {
            comboListBox.Items.Clear();
            foreach (AbstractCase acase in model.AbstractCases)
                if (acase != null && acase is LoadCombination)
                    comboListBox.Items.Add(acase);
            if (currentCombo != null && comboListBox.Items.Contains(currentCombo))
                comboListBox.SelectedItem = currentCombo;
        }

        private void updateTypeComboBox()
        {
            combinationTypes.Clear();
            typeComboBox.Items.Clear();
            foreach (LoadCombination.CombinationType val in Enum.GetValues(typeof(LoadCombination.CombinationType)))
            {
                string name = Culture.Get("CombinationType" + val.ToString());
                combinationTypes.Add(name, val);
                typeComboBox.Items.Add(name);
            }
            typeComboBox.SelectedIndex = 0;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            currentCombo = new LoadCombination(Culture.Get("defaultLoadComboName"));
            wizardControl.SelectTab(1);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (currentCombo != null)
                wizardControl.SelectTab(1);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (currentCombo != null && model.AbstractCases.Contains(currentCombo))
            {
                model.AbstractCases.Remove(currentCombo);
                updateComboListBox();
                updateMainPage();
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (oneCombo)
                DialogResult = DialogResult.Cancel;
            else
            {
                currentCombo = null;
                wizardControl.SelectTab(0);
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            bool needUpdateComboList = false;
            string name = nameTextBox.Text;
            if (!currentCombo.Name.Equals(name))
            {
                currentCombo.Name = name;
                needUpdateComboList = true;
            }

            if (!model.AbstractCases.Contains(currentCombo))
            {
                model.AbstractCases.Add(currentCombo);
                needUpdateComboList = true;
            }

            if (needUpdateComboList)
                updateComboListBox();

            currentCombo.Type = combinationTypes[typeComboBox.SelectedItem.ToString()];

            if (oneCombo)
                DialogResult = DialogResult.OK;
            else
                wizardControl.SelectTab(0);
        }

        private void addAllCasesButton_Click(object sender, EventArgs e)
        {
            foreach (object obj in casesListBox.Items)
                currentCombo.Cases.Add(new AbstractCaseFactor((AbstractCase)obj));
            updateEditPage(false);
        }

        private void addCaseButton_Click(object sender, EventArgs e)
        {
            foreach (object item in casesListBox.SelectedItems)
            {
                AbstractCase obj = item as AbstractCase;
                if (obj != null)
                    currentCombo.Cases.Add(new AbstractCaseFactor(obj));
            }
            updateEditPage(false);
        }

        private void removeCaseButton_Click(object sender, EventArgs e)
        {
            List<int> indices = new List<int>();
            foreach (DataGridViewCell cell in casesGridView.SelectedCells)
                indices.Add(cell.RowIndex);
            indices.Sort();

            for (int i = indices.Count; i > 0; i--)
                if (currentCombo.Cases.Count >= i)
                    currentCombo.Cases.RemoveAt(indices[i - 1]);
            
            updateEditPage(false);
        }

        private void removeAllCasesButton_Click(object sender, EventArgs e)
        {
            currentCombo.Cases.Clear();
            updateEditPage(false);
        }

        private void casesGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (casesGridView.CurrentCell.ColumnIndex == 1 && e.Control != null)
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress += new KeyPressEventHandler(gridFactorEdit_KeyPress);
                tb.LostFocus += new EventHandler(gridFactorEdit_LostFocus);
            }
        }

        void gridFactorEdit_LostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                float fac;

                if (float.TryParse(((TextBox)sender).Text, out fac))
                {
                    AbstractCaseFactor cFactor = (AbstractCaseFactor)currentCombo.Cases[casesGridView.CurrentCell.RowIndex];
                    cFactor.Factor = fac;
                    currentCombo.Cases[casesGridView.CurrentCell.RowIndex] = cFactor;
                }
            }
        }

        void gridFactorEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-' && (((TextBox)sender).SelectionStart > 0 || 
                (((TextBox)sender).Text.Contains("-") && !((TextBox)sender).SelectedText.Contains("-"))))
                e.Handled = true;
            else if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains(".") && !((TextBox)sender).SelectedText.Contains("."))
                e.Handled = true;
            else if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '.' || e.KeyChar == '-'))
                e.Handled = true;
        }

        public class ACaseFactorWrapper
        {
            private IList<AbstractCaseFactor> factors;
            private int index;

            public ACaseFactorWrapper(IList<AbstractCaseFactor> factors, int index)
            {
                this.factors = factors;
                this.index = index;
            }

            public string Case
            {
                get { return factors[index].Case.Name; }
            }

            public float Factor
            {
                get { return factors[index].Factor; }
                set { factors[index] = new AbstractCaseFactor(factors[index].Case, value); }
            }
        }

        private void responseSpectrumCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AnalysisCase modalCase = null;
            List<AnalysisCase> responseCases = new List<AnalysisCase>();
            foreach (AbstractCase ac in model.AbstractCases)
                if (ac is AnalysisCase && ((AnalysisCase)ac).Properties is ResponseSpectrumCaseProps)
                    responseCases.Add((AnalysisCase)ac);
                else if (ac is AnalysisCase && ((AnalysisCase)ac).Properties is ModalCaseProps)
                    modalCase = (AnalysisCase)ac;

            if (modalCase == null)
            {
                modalCase = new AnalysisCase(Culture.Get("defaultModalCase"), new ModalCaseProps());
                model.AbstractCases.Add(modalCase);
            }
            if (modalCase != null)
            {
                if (responseCases.Count == 0 && responseSpectrumCheckBox.Checked && model.ResponseSpectra.Count > 0)
                {
                    ResponseSpectrumCaseProps props = new ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues.UX);
                    props.ModalAnalysisCase = modalCase;
                    responseCases.Add(new AnalysisCase(Culture.Get("defaultResponseCase") + " X", props));
                    model.AbstractCases.Add(responseCases[0]);

                    props = new ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues.UY);
                    props.ModalAnalysisCase = modalCase;
                    responseCases.Add(new AnalysisCase(Culture.Get("defaultResponseCase") + " Y", props));
                    model.AbstractCases.Add(responseCases[1]);

                    props = new ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues.UZ);
                    props.ModalAnalysisCase = modalCase;
                    responseCases.Add(new AnalysisCase(Culture.Get("defaultResponseCase") + " Z", props));
                    model.AbstractCases.Add(responseCases[2]);
                }
                foreach (AbstractCase responseCase in responseCases)
                    responseCase.IsActive = responseSpectrumCheckBox.Checked;
            }
        }

        private void casesGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (updatingGridView)
                e.ThrowException = false;
        }
    }
}