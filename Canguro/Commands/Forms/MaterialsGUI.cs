using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Material;
using Canguro.Model.Section;

namespace Canguro.Commands.Forms
{
    public partial class MaterialsGUI : Form
    {
        Material material;
        List<MaterialTypeProps> typeList = new List<MaterialTypeProps>();
        List<MaterialDesignProps> designPropList = new List<MaterialDesignProps>();
        private readonly bool oneMaterial; // True if a material is provided.

        public MaterialsGUI(Material mat)
        {
            oneMaterial = (mat != null);
            InitializeComponent();

            material = mat;
            IsotropicTypeProps isotropic;
            if (mat != null && mat.TypeProperties is IsotropicTypeProps)
                isotropic = (IsotropicTypeProps)mat.TypeProperties;
            else
                isotropic = MaterialManager.Instance.DefaultSteel.TypeProperties as IsotropicTypeProps;
            if (isotropic == null)
                isotropic = new IsotropicTypeProps(1.999E+11F, 0.3F, 0.0000117F);
            typeList.Add(isotropic);

            UniaxialTypeProps uniaxial;
            if (mat != null && mat.TypeProperties is UniaxialTypeProps)
                uniaxial = (UniaxialTypeProps)mat.TypeProperties;
            else
                uniaxial = MaterialManager.Instance.DefaultRebar.TypeProperties as UniaxialTypeProps;
            if (uniaxial == null)
                uniaxial = new UniaxialTypeProps(1.999E+11F, 0.0000117F);
            typeList.Add(uniaxial);

            designPropList.Add(new NoDesignProps());
            designPropList.Add(new SteelDesignProps());
            designPropList.Add(MaterialManager.Instance.DefaultConcrete.DesignProperties);
            //designPropList.Add(new AluminumDesignProps());
            designPropList.Add(new RebarDesignProps());
            //designPropList.Add(new ColdFormedDesignProps());
        }
            
        public MaterialsGUI() : this(null)
        {
        }

        public Material Material
        {
            get { return material; }
        }

        private void MaterialsGUI_Load(object sender, EventArgs e)
        {
            wizardControl.HideTabs = true;

            typeComboBox.Items.AddRange(typeList.ToArray());
            designComboBox.Items.AddRange(designPropList.ToArray());
            UpdateMaterialList();
            if (oneMaterial)
                wizardControl.SelectTab(1);
            else
                wizardControl.SelectTab(0);
        }

        public void UpdatePage1()
        {
            object selected = materialsListBox.SelectedItem;
            if (selected != null && selected is Material)
                material = (Material)selected;

            editButton.Enabled = (material != null && !material.IsLocked);
            deleteButton.Enabled = (material != null && !material.IsLocked);
            addButton.Enabled = true;
        }

        public void UpdatePage2()
        {
            if (material != null)
            {
                material.Name = material.Name; // Checks that material name is valid
                nameTextBox.Text = material.Name;
                densityTextBox.Text = material.Density.ToString();
                densityLabel.Text = Culture.Get("Density") + " (" + Canguro.Model.Model.Instance.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Density) + ")";
                MaterialTypeProps typeProps = material.TypeProperties;
                MaterialDesignProps designProps = material.DesignProperties;

                for (int i = 0; i < typeList.Count; i++)
                    if (typeList[i].ToString().Equals(typeProps.ToString()))
                        typeComboBox.SelectedIndex = i;

                typePropertyGrid.SelectedObject = typeProps;

                for (int i = 0; i < designPropList.Count; i++)
                    if (designPropList[i].ToString().Equals(designProps.ToString()))
                        designComboBox.SelectedIndex = i;
                designPropertyGrid.SelectedObject = designProps;
            }
            else
                wizardControl.SelectTab(0);
        }

        public void UpdateMaterial()
        {
            material.Name = nameTextBox.Text;
            float value;
            if (float.TryParse(densityTextBox.Text, out value))
                material.Density = value;
            material.TypeProperties = (MaterialTypeProps)typePropertyGrid.SelectedObject;
            material.DesignProperties = (MaterialDesignProps)designPropertyGrid.SelectedObject;
        }

        public void UpdateMaterialList()
        {
            materialsListBox.Items.Clear();
            foreach (Material mat in MaterialManager.Instance.Materials)
                materialsListBox.Items.Add(mat);
            materialsListBox.SelectedItem = material;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            UpdateMaterial();
            if (MaterialManager.Instance.Materials[material.Name] == null)
                MaterialManager.Instance.Materials[material.Name] = material;
            UpdateMaterialList();

            if (oneMaterial)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void designComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            designPropertyGrid.SelectedObject = designComboBox.SelectedItem;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            typePropertyGrid.SelectedObject = typeComboBox.SelectedItem;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (material != null)
                material = new Material(material);
            else
                material = new Material();
            wizardControl.SelectTab(1);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (material != null && !material.IsLocked)
                wizardControl.SelectTab(1);
        }

        private void materialsListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            material = (Material)materialsListBox.SelectedItem;
            if (material != null && !material.IsLocked)
                wizardControl.SelectTab(1);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (material != null && !material.IsLocked)
            {
                if (materialIsUsed())
                {
                    MessageBox.Show(Culture.Get("materialInUseError"), Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    deleteButton.Enabled = false;
                }
                else
                {
                    MaterialManager.Instance.Materials[material.Name] = null;
                    material = null;
                    UpdateMaterialList();
                    UpdatePage1();
                }
            }
        }

        private bool materialIsUsed()
        {
            foreach (Section sec in Canguro.Model.Model.Instance.Sections)
                if (sec != null && sec.Material == material)
                    return true;
            return false;
        }

        private void materialsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            material = (Material)materialsListBox.SelectedItem;
            UpdatePage1();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            material = null;
            if (oneMaterial)
                DialogResult = DialogResult.Cancel;
            else
                wizardControl.SelectTab(0);
        }

        private void wizardControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (wizardControl.SelectedIndex)
            {
                case 0:
                    UpdatePage1();
                    AcceptButton = okButton;
                    //CancelButton = cancelButton;
                    break;
                case 1:
                    UpdatePage2();
                    AcceptButton = applyButton;
                    //CancelButton = backButton;
                    break;
            }
        }
    }
}