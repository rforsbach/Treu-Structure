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
    public partial class EditLoadCaseDialog : Form
    {
        private LoadCase load;
        private Dictionary<string, LoadCase.LoadCaseType> caseTypes = new Dictionary<string, LoadCase.LoadCaseType>();

        public EditLoadCaseDialog(LoadCase loadCase)
        {
            load = loadCase;
            InitializeComponent();

            foreach (LoadCase.LoadCaseType type in Enum.GetValues(typeof(LoadCase.LoadCaseType)))
                caseTypes.Add(Culture.Get(type.ToString()), type);

            foreach (string type in caseTypes.Keys)
                typeComboBox.Items.Add(type);
        }

        private void EditLoadCaseDialog_Load(object sender, EventArgs e)
        {
            nameTextBox.Text = load.Name;
            swFactorNumericUpDown.Value = (decimal)load.SelfWeight;

            foreach (object type in typeComboBox.Items)
                if (caseTypes[type.ToString()] == load.CaseType)
                {
                    typeComboBox.SelectedItem = type;
                    break;
                }
            UpdateDialog();
        }

        private void UpdateDialog()
        {
            //LoadCase.LoadCaseType selectedType = load.CaseType;
            //bool selectAutoLoad = selectedType == LoadCase.LoadCaseType.Quake || selectedType == LoadCase.LoadCaseType.Wind;
            //autoLoadLabel.Visible = selectAutoLoad;
            //autoLoadComboBox.Visible = selectAutoLoad;
            //if (selectAutoLoad)
            //{
            //    autoLoadComboBox.Items.Clear();
            //    autoLoadComboBox.Items.AddRange(AllAutoLoads[selectedType].ToArray());
            //    autoLoadComboBox.SelectedItem = load.AutoLoad;
            //}

            //bool selectedAutoLoad = (load.AutoLoad != null && !(load.AutoLoad is NoAutoLoad));
            //autoLoadNameLabel.Visible = selectedAutoLoad;
            //autoLoadPropertyGrid.Visible = selectedAutoLoad;
            //if (selectAutoLoad)
            //{
            //    Size size = new Size(430, Size.Height);
            //    MaximumSize = size;
            //    Size = size;
            //    MinimumSize = size;
            //    cancelButton.Location = new Point(335, cancelButton.Location.Y);
            //    okButton.Location =  new Point(254, cancelButton.Location.Y);
            //    autoLoadNameLabel.Text = load.AutoLoad;
            //    autoLoadPropertyGrid.SelectedObject = load.AutoLoad;
            //}
            //else
            //{
            //    Size size = new Size(200, Size.Height);
            //    MaximumSize = size;
            //    Size = size;
            //    MinimumSize = size;
            //    cancelButton.Location = new Point(99, cancelButton.Location.Y);
            //    okButton.Location = new Point(18, cancelButton.Location.Y);
            //}
        }

        private Dictionary<LoadCase.LoadCaseType, List<AutoLoad>> allAutoLoads = null;
        private Dictionary<LoadCase.LoadCaseType, List<AutoLoad>> AllAutoLoads
        {
            get
            {
                if (allAutoLoads == null)
                {
                    AutoLoad al = null; // load.AutoLoad;
                    allAutoLoads = new Dictionary<LoadCase.LoadCaseType, List<AutoLoad>>();
                    allAutoLoads.Add(LoadCase.LoadCaseType.Quake, new List<AutoLoad>());
                    allAutoLoads.Add(LoadCase.LoadCaseType.Wind, new List<AutoLoad>());
                    allAutoLoads[LoadCase.LoadCaseType.Quake].Add(new NoAutoLoad());
                    allAutoLoads[LoadCase.LoadCaseType.Quake].Add((al is AutoUBC97QuakeLoad) ? al : new AutoUBC97QuakeLoad());
                    allAutoLoads[LoadCase.LoadCaseType.Wind].Add(new NoAutoLoad());
                    allAutoLoads[LoadCase.LoadCaseType.Wind].Add((al is AutoUBC97WindLoad) ? al : new AutoUBC97WindLoad());
                    allAutoLoads[LoadCase.LoadCaseType.Wind].Add((al is AutoRCDFWindLoad) ? al : new AutoRCDFWindLoad());
                }
                return allAutoLoads;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            load.Name = nameTextBox.Text;
            load.SelfWeight = Convert.ToSingle(swFactorNumericUpDown.Value);
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            load.CaseType = caseTypes[typeComboBox.SelectedItem.ToString()];
            UpdateDialog();
        }

        private void autoLoadComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            load.AutoLoad = autoLoadComboBox.SelectedItem.ToString();
            UpdateDialog();
        }
    }
}