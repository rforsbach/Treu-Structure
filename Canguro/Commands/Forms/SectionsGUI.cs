using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Section;
using Canguro.Model.Material;

namespace Canguro.Commands.Forms
{
    public partial class SectionsGUI : Form
    {
        private FrameSection currentSection;
        private Dictionary<string, FrameSection> steelSections;
        private Dictionary<string, FrameSection> concreteSections;
        private Dictionary<string, FrameSection> allSections;
        private List<ConcreteSectionProps> concreteSectionProps;
        private Dictionary<string, string> shapeNames;
        private Material currentMaterial;
        private readonly bool oneSection; // True if a material is provided.
        private Canguro.Model.Model model;

        public SectionsGUI(FrameSection section)
        {
            oneSection = (section != null);
            InitializeComponent();

            steelSections = new Dictionary<string, FrameSection>();
            concreteSections = new Dictionary<string, FrameSection>();
            allSections = new Dictionary<string, FrameSection>();
            concreteSectionProps = new List<ConcreteSectionProps>();
            shapeNames = new Dictionary<string, string>();

            shapeNames.Add("R", Culture.Get("RectangularSection"));
            shapeNames.Add("RN", Culture.Get("CircleSection"));
//            shapeNames.Add("2L", Culture.Get("DoubleAngle"));
            shapeNames.Add("C", Culture.Get("Channel"));
            shapeNames.Add("I", Culture.Get("I_WideFlange"));
            shapeNames.Add("B", Culture.Get("Box_Tube"));
            shapeNames.Add("P", Culture.Get("Pipe"));
            shapeNames.Add("L", Culture.Get("Angle"));
            shapeNames.Add("T", Culture.Get("Tee"));
            shapeNames.Add("G", Culture.Get("General"));
            model = Canguro.Model.Model.Instance;

            coverTopLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            coverBottomLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            roTopLeftLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            roTopRightLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            roBottomLeftLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            roBottomRightLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            sqCoverLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            sqReinforcementsLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            areaLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Area) + ")";
            JLabel.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.AreaInertia) + ")";
            sa33Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Area) + ")";
            sa22Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Area) + ")";
            i33Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.AreaInertia) + ")";
            i22Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.AreaInertia) + ")";
            s33Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.ShearModulus) + ")";
            s22Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.ShearModulus) + ")";
            z33Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.ShearModulus) + ")";
            z22Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.ShearModulus) + ")";
            r33Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
            r22Label.Text += " (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
        }

        public SectionsGUI()
            : this(null)
        {
        }

        public FrameSection Section
        {
            get { return currentSection; }
        }


        private void SectionsGUI_Load(object sender, EventArgs e)
        {
            wizardControl.HideTabs = true;

            ConcreteColumnSectionProps cProps = new ConcreteColumnSectionProps();
            concreteSectionProps.Add(cProps);
            concreteSectionProps.Add(new ConcreteBeamSectionProps());

            sqColRebarSizeComboBox.DataSource = new List<string>(BarSizes.Instance.Keys);

            if (oneSection)
                wizardControl.SelectTab(4);
            else
                wizardControl.SelectTab(0);
            UpdatePage1();

            UpdateMaterialsComboBox();
        }

        private void UpdateShapeComboBox()
        {
            shapeComboBox.Items.Clear();

            foreach (string shape in shapeNames.Values)
                shapeComboBox.Items.Add(shape);

            shapeComboBox.Enabled = true;

            if (shapeComboBox.Items.Count > 0)
                shapeComboBox.SelectedIndex = 0;
        }

        private void UpdateMaterialsComboBox()
        {
            materialComboBox.Items.Clear();
            int i = 0;
            List<string> validMaterials = new List<string>(new string[] { "SteelDesignProps", "ConcreteDesignProps" });
            foreach (Material mat in MaterialManager.Instance.Materials)
            {
                if (validMaterials.Contains(mat.DesignProperties.GetType().Name))
                {
                    materialComboBox.Items.Add(mat.Name);
                    if (mat == currentMaterial)
                        materialComboBox.SelectedIndex = i;
                    i++;
                }
            }

            if (materialComboBox.SelectedItem == null)
                materialComboBox.SelectedIndex = 0;
        }

        private void UpdatePage1()
        {
            if (sectionsTreeView.TopNode != Canguro.Model.Model.Instance.Sections.Tree)
            {
                sectionsTreeView.Nodes.Clear();
                sectionsTreeView.Nodes.Add(Canguro.Model.Model.Instance.Sections.Tree);
            }
            sectionsTreeView.ExpandAll();
            currentSection = (sectionsTreeView.SelectedNode != null) ? (FrameSection)sectionsTreeView.SelectedNode.Tag : null;
            if (currentSection != null)
            {
                editButton.Enabled = true;
                deleteButton.Enabled = true;
                propertiesButton.Enabled = true;
            }
            else
            {
                editButton.Enabled = false;
                deleteButton.Enabled = false;
                propertiesButton.Enabled = false;
            }
            addButton.Enabled = true;
            sectionsTreeView.Update();
        }

        private void UpdatePage2()
        {
            if (currentSection != null && currentSection.Material != null)
            {
                currentMaterial = currentSection.Material;
                nameTextBox.Text = currentSection.Name;
                if (!materialComboBox.Items.Contains(currentSection.Material.Name))
                    materialComboBox.Items.Add(currentSection.Material.Name);
                materialComboBox.SelectedItem = currentSection.Material.Name;
                if (shapeNames.ContainsKey(currentSection.Shape))
                    shapeComboBox.SelectedItem = shapeNames[currentSection.Shape];
                else
                    shapeComboBox.SelectedItem = -1;
                shapeComboBox.Enabled = false;
            }
            else
            {
                UpdateShapeComboBox();
                nameTextBox.Text = "";
                nextButton2.Enabled = false;
            }
        }

        private void UpdatePage3()
        {
            if (currentSection == null) // || !(currentSection is Rectangular || currentSection is Circle))
                wizardControl.SelectTab(0);
            else
            {
                currentMaterial = currentSection.Material;
                if (currentMaterial.DesignProperties is ConcreteDesignProps)
                {
                    columnRadioButton.Visible = true;
                    beamRadioButton.Visible = true;
                    if (currentSection.ConcreteProperties is ConcreteBeamSectionProps)
                        beamRadioButton.Checked = true;
                    else if (currentSection.ConcreteProperties is ConcreteColumnSectionProps)
                        columnRadioButton.Checked = true;

                    if (currentSection is Rectangular)
                        beamRadioButton.Enabled = true;
                    else
                        beamRadioButton.Enabled = false;
                    nextButton3.Text = Culture.Get("next") + " >>";
                }
                else if (currentMaterial.DesignProperties is SteelDesignProps)
                {
                    columnRadioButton.Visible = false;
                    beamRadioButton.Visible = false;
                    nextButton3.Text = Culture.Get("apply");
                }
                else
                    wizardControl.SelectTab(3);

                t3TextBox.Text = currentSection.T3.ToString();
                t2TextBox.Text = currentSection.T2.ToString();
                tfTextBox.Text = currentSection.Tf.ToString();
                twTextBox.Text = currentSection.Tw.ToString();
                disTextBox.Text = currentSection.Dis.ToString();
                if (currentSection is Rectangular)
                {
                    t3Label.Text = "H (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
                    t3TextBox.Text = currentSection.T3.ToString();
                    t2Label.Text = "B (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
                    t2TextBox.Text = currentSection.T2.ToString();
                    t2Label.Visible = true;
                    t2TextBox.Visible = true;
                    tfLabel.Visible = false;
                    tfTextBox.Visible = false;
                    twLabel.Visible = false;
                    twTextBox.Visible = false;
                    disLabel.Visible = false;
                    disTextBox.Visible = false;
                    sectionPictureBox.Image = Properties.Resources.RectangularSectionImg;
                }
                else if (currentSection is Circle)
                {
                    t3Label.Text = "D (" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
                    t3TextBox.Text = currentSection.T3.ToString();
                    t2Label.Visible = false;
                    t2TextBox.Visible = false;
                    tfLabel.Visible = false;
                    tfTextBox.Visible = false;
                    twLabel.Visible = false;
                    twTextBox.Visible = false;
                    disLabel.Visible = false;
                    disTextBox.Visible = false;
                    sectionPictureBox.Image = Properties.Resources.CircleSectionImg;
                }
                else if (currentSection is General)
                {
                    wizardControl.SelectTab(4);
                }
                else
                {
                    string unit = "(" + model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance) + ")";
                    t3Label.Text = "h " + unit;
                    t2Label.Text = "b " + unit;
                    tfLabel.Text = "Tf " + unit;
                    twLabel.Text = "Tw " + unit;
                    disLabel.Text = "Dis " + unit;
                    t2Label.Visible = true;
                    t2TextBox.Visible = true;
                    tfLabel.Visible = true;
                    tfTextBox.Visible = true;
                    twLabel.Visible = true;
                    twTextBox.Visible = true;
                    disLabel.Visible = false;
                    disTextBox.Visible = false;
                    columnRadioButton.Visible = false;
                    beamRadioButton.Visible = false;
                    columnRadioButton.Checked = false;
                    beamRadioButton.Checked = false;
                    if (currentSection is IWideFlange)
                        sectionPictureBox.Image = Properties.Resources.ISectionImg;
                    else if (currentSection is BoxTube)
                        sectionPictureBox.Image = Properties.Resources.TubeSectionImg;
                    else if (currentSection is Angle)
                        sectionPictureBox.Image = Properties.Resources.AngleSectionImg;
                    else if (currentSection is Channel)
                        sectionPictureBox.Image = Properties.Resources.ChannelSectionImg;
                    else if (currentSection is Tee)
                        sectionPictureBox.Image = Properties.Resources.TeeSectionImg;
                    else if (currentSection is Pipe)
                    {
                        t3Label.Text = "r " + unit;
                        t2Label.Text = "t " + unit;
                        t2TextBox.Text = currentSection.Tw.ToString();
                        t2Label.Visible = true;
                        t2TextBox.Visible = true;
                        tfLabel.Visible = false;
                        tfTextBox.Visible = false;
                        twLabel.Visible = false;
                        twTextBox.Visible = false;
                        sectionPictureBox.Image = Properties.Resources.PipeSectionImg;
                    }
                }
            }
        }

        private void UpdatePage4()
        {
            if (currentSection == null) // || !(currentSection is Rectangular || currentSection is Circle))
                wizardControl.SelectTab(0);
            else
            {
                ConcreteSectionProps concProps = currentSection.ConcreteProperties;
                if (concProps != null)
                {
                    if (concProps is ConcreteBeamSectionProps)
                    {
                        sqareColumnGroupBox.Visible = false;
                        beamGroupBox.Visible = true;

                        ConcreteBeamSectionProps props = (ConcreteBeamSectionProps)concProps;
                        coverTopTextBox.Text = string.Format("{0}", props.ConcreteCoverTop);
                        coverBottomTextBox.Text = string.Format("{0}", props.ConcreteCoverBottom);
                        coTopLeftTextBox.Text = string.Format("{0}", props.RoTopLeft);
                        coTopRightTextBox.Text = string.Format("{0}", props.RoTopRight);
                        coBottomLeftTextBox.Text = string.Format("{0}", props.RoBottomLeft);
                        coBottomRightTextBox.Text = string.Format("{0}", props.RoBottomRight);
                    }
                    else if (concProps is ConcreteColumnSectionProps)
                    {
                        columnRadioButton.Checked = true;
                        sqareColumnGroupBox.Visible = true;
                        beamGroupBox.Visible = false;
                        ConcreteColumnSectionProps props = (ConcreteColumnSectionProps)concProps;

                        sqColCoverToRebarTextBox.Text = string.Format("{0}", props.CoverToRebarCenter);
                        sqColRebars22TextBox.Text = string.Format("{0}", props.NumberOfBars2Dir);
                        sqColRebars33TextBox.Text = string.Format("{0}", props.NumberOfBars3Dir);
                        sqColReinforcementsTextBox.Text = string.Format("{0}", props.SpacingC);
                        sqColRebarSizeComboBox.SelectedItem = props.BarSize;
                        //sqColReinforcementsTextBox.Text = string.Format("{0:F}", props.);
                    }
                    else // Not concrete column nor beam
                        wizardControl.SelectTab(0);
                }
                else // Not concrete
                    wizardControl.SelectTab(0);
            }
        }

        private void UpdatePage5()
        {
            if (currentSection == null)
                wizardControl.SelectTab(0);
            else
            {
                areaTextBox.Text = currentSection.Area.ToString("G2");
                as22TextBox.Text = currentSection.As2.ToString("G2");
                as33TextBox.Text = currentSection.As3.ToString("G2");
                i22TextBox.Text = currentSection.I22.ToString("G2");
                i33TextBox.Text = currentSection.I33.ToString("G2");
                r22TextBox.Text = currentSection.R22.ToString("G2");
                r33TextBox.Text = currentSection.R33.ToString("G2");
                s22TextBox.Text = currentSection.S22.ToString("G2");
                s33TextBox.Text = currentSection.S33.ToString("G2");
                torsionalTextBox.Text = currentSection.TorsConst.ToString("G2");
                z22TextBox.Text = currentSection.Z22.ToString("G2");
                z33TextBox.Text = currentSection.Z33.ToString("G2");

                bool enable = currentSection is General;
                areaTextBox.Enabled = enable;
                as22TextBox.Enabled = enable;
                as33TextBox.Enabled = enable;
                i22TextBox.Enabled = enable;
                i33TextBox.Enabled = enable;
                r22TextBox.Enabled = enable;
                r33TextBox.Enabled = enable;
                s22TextBox.Enabled = enable;
                s33TextBox.Enabled = enable;
                torsionalTextBox.Enabled = enable;
                z22TextBox.Enabled = enable;
                z33TextBox.Enabled = enable;

                //propertyGrid.SelectedObject = currentSection;
                //propertyGrid.Width = enable;

                if (enable)
                {
                    applyButton.Visible = true;
                    startButton5.Text = Culture.Get("cancel");
                    AcceptButton = applyButton;
                }
                else
                {
                    applyButton.Visible = false;
                    startButton5.Text = Culture.Get("accept");
                    AcceptButton = startButton5;
                }
            }
        }

        private void UpdateModel()
        {
            UpdateModel(wizardControl.SelectedIndex);
        }

        private void UpdateModel(int page)
        {
            float value;

            switch (page)
            {
                case 1:
                    currentSection.Name = nameTextBox.Text;
                    if (currentMaterial != null)
                        currentSection.Material = currentMaterial;

                    if (!(currentSection.Material.DesignProperties is ConcreteDesignProps))
                        currentSection.ConcreteProperties = null;
                    break;
                case 2:
                    if (currentSection is Rectangular)
                    {
                        if (float.TryParse(t3TextBox.Text, out value))
                            ((Rectangular)currentSection).T3 = value;
                        if (float.TryParse(t2TextBox.Text, out value))
                            ((Rectangular)currentSection).T2 = value;
                    }
                    else if (currentSection is Circle)
                    {
                        if (float.TryParse(t3TextBox.Text, out value))
                            ((Circle)currentSection).D = value;
                    }
                    else if (currentSection is Pipe)
                    {
                        if (float.TryParse(t3TextBox.Text, out value))
                            currentSection.T3 = value;
                        if (float.TryParse(t2TextBox.Text, out value))
                            currentSection.Tw = value;
                    }
                    else
                    {
                        if (float.TryParse(t3TextBox.Text, out value))
                            currentSection.T3 = value;
                        if (float.TryParse(t2TextBox.Text, out value))
                            currentSection.T2 = value;
                        if (float.TryParse(tfTextBox.Text, out value))
                            currentSection.Tf = value;
                        if (float.TryParse(twTextBox.Text, out value))
                            currentSection.Tw = value;
                        if (float.TryParse(disTextBox.Text, out value))
                            currentSection.Dis = value;
                    }
                    break;
                case 3:
                    ConcreteSectionProps concProps = currentSection.ConcreteProperties;

                    if (concProps != null)
                    {
                        if (concProps is ConcreteBeamSectionProps)
                        {
                            ConcreteBeamSectionProps props = (ConcreteBeamSectionProps)concProps;
                            // TODO: Add an image to explain
                            if (float.TryParse(coverTopTextBox.Text, out value))
                            {
                                props.ConcreteCoverTop = value;
                                props.ConcreteCoverBottom = value;
                            }
                            //if (float.TryParse(coverBottomTextBox.Text, out value))
                            //    props.ConcreteCoverBottom = value;
                            if (float.TryParse(coTopLeftTextBox.Text, out value))
                                props.RoTopLeft = value;
                            if (float.TryParse(coTopRightTextBox.Text, out value))
                                props.RoTopRight = value;
                            if (float.TryParse(coBottomLeftTextBox.Text, out value))
                                props.RoBottomLeft = value;
                            if (float.TryParse(coBottomRightTextBox.Text, out value))
                                props.RoBottomRight = value;
                            currentSection.ConcreteProperties = props;
                        }
                        else if (concProps is ConcreteColumnSectionProps)
                        {
                            ConcreteColumnSectionProps props = (ConcreteColumnSectionProps)concProps;
                            int ival;
                            if (int.TryParse(sqColRebars22TextBox.Text, out ival))
                                props.NumberOfBars2Dir = ival;
                            if (int.TryParse(sqColRebars33TextBox.Text, out ival))
                                props.NumberOfBars3Dir = ival;
                            if (float.TryParse(sqColCoverToRebarTextBox.Text, out value))
                                props.CoverToRebarCenter = value;
                            if (float.TryParse(sqColReinforcementsTextBox.Text, out value))
                                props.SpacingC = value;
                            props.BarSize = sqColRebarSizeComboBox.SelectedItem.ToString();
                            currentSection.ConcreteProperties = props;
                        }
                    }
                    break;
                case 4:
                    if (currentSection is General)
                        UpdateGeneralSection();
                    break;
            }
        }

        private void UpdateGeneralSection()
        {
            float val;
            if (float.TryParse(areaTextBox.Text, out val)) currentSection.Area = val;
            if (float.TryParse(as22TextBox.Text, out val)) currentSection.As2 = val;
            if (float.TryParse(as33TextBox.Text, out val)) currentSection.As3 = val;
            if (float.TryParse(i22TextBox.Text, out val)) currentSection.I22 = val;
            if (float.TryParse(i33TextBox.Text, out val)) currentSection.I33 = val;
            if (float.TryParse(r22TextBox.Text, out val)) currentSection.R22 = val;
            if (float.TryParse(r33TextBox.Text, out val)) currentSection.R33 = val;
            if (float.TryParse(s22TextBox.Text, out val)) currentSection.S22 = val;
            if (float.TryParse(s33TextBox.Text, out val)) currentSection.S33 = val;
            if (float.TryParse(torsionalTextBox.Text, out val)) currentSection.TorsConst = val;
            if (float.TryParse(z22TextBox.Text, out val)) currentSection.Z22 = val;
            if (float.TryParse(z33TextBox.Text, out val)) currentSection.Z33 = val;
        }

        private void wizardControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (wizardControl.SelectedIndex)
            {
                case 0:
                    UpdatePage1();
                    AcceptButton = okButton;
                    break;
                case 1:
                    UpdatePage2();
                    AcceptButton = nextButton2;
                    break;
                case 2:
                    UpdatePage3();
                    AcceptButton = nextButton3;
                    break;
                case 3:
                    UpdatePage4();
                    AcceptButton = applyButton4;
                    break;
                case 4:
                    UpdatePage5();
                    AcceptButton = applyButton;
                    break;
            }
        }

        private void addMaterialLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MaterialsGUI gui = new MaterialsGUI(new Material());
            if (gui.ShowDialog(this) == DialogResult.OK)
            {
                currentMaterial = gui.Material;
                MaterialManager.Instance.Materials[currentMaterial.Name] = currentMaterial;
                UpdateMaterialsComboBox();
            }
        }

        private void materialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox.SelectedItem != null)
            {
                currentMaterial = MaterialManager.Instance.Materials[materialComboBox.SelectedItem.ToString()];
                if (currentSection != null)
                    currentSection.Material = currentMaterial;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            currentMaterial = null;
            currentSection = null;
            wizardControl.SelectTab(1);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            UpdatePage2();
            wizardControl.SelectTab(1);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (currentSection != null)
            {
                if (sectionInUse())
                {
                    MessageBox.Show(Culture.Get("sectionInUseError"), Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    deleteButton.Enabled = false;
                }
                else
                {
                    Canguro.Model.Model.Instance.Sections[currentSection.Name] = null;
                    UpdatePage1();
                }
            }
        }

        private void startButton2_Click(object sender, EventArgs e)
        {
            currentSection = null;

            if (oneSection)
                DialogResult = DialogResult.Cancel;
            else
            {
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void startButton3_Click(object sender, EventArgs e)
        {
            currentSection = null;

            if (oneSection)
                DialogResult = DialogResult.Cancel;
            else
            {
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void startButton4_Click(object sender, EventArgs e)
        {
            currentSection = null;

            if (oneSection)
                DialogResult = DialogResult.Cancel;
            else
            {
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void startButton5_Click(object sender, EventArgs e)
        {
            currentSection = null;

            if (oneSection)
                DialogResult = DialogResult.Cancel;
            else
            {
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void backButton3_Click(object sender, EventArgs e)
        {
            UpdateModel(2);
            wizardControl.SelectTab(1);
        }

        private void backButton4_Click(object sender, EventArgs e)
        {
            UpdateModel(3);
            wizardControl.SelectTab(2);
        }

        private void backButton5_Click(object sender, EventArgs e)
        {
            UpdateModel(4);
            wizardControl.SelectTab(0);
        }

        private void nextButton2_Click(object sender, EventArgs e)
        {
            if (currentSection == null)
            {
                currentMaterial = MaterialManager.Instance.Materials[materialComboBox.SelectedItem.ToString()];

                string secName = nameTextBox.Text;
                ConcreteSectionProps cProps = (currentMaterial.DesignProperties is ConcreteDesignProps) ? concreteSectionProps[0] : null;

                if (shapeNames["R"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new Rectangular(secName, currentMaterial, cProps, 0.2f, 0.4f);
                else if (shapeNames["RN"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new Circle(secName, currentMaterial, cProps, 0.4f);
                //else if (shapeNames["2L"].Equals(shapeComboBox.SelectedItem.ToString()))
                //    currentSection = new DoubleAngle(secName, "2L", currentMaterial, cProps, 0.2f, 0.2f, 0.02f, 0.02f);
                else if (shapeNames["C"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new Channel(secName, "C", currentMaterial, cProps, 0.2f, 0.2f, 0.02f, 0.02f);
                else if (shapeNames["I"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new IWideFlange(secName, "C", currentMaterial, cProps, 0.2f, 0.2f, 0.02f, 0.02f);
                else if (shapeNames["B"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new BoxTube(secName, "B", currentMaterial, cProps, 0.2f, 0.2f, 0.02f, 0.02f);
                else if (shapeNames["P"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new Pipe(secName, "P", currentMaterial, cProps, 0.2f, 0.02f);
                else if (shapeNames["L"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new Angle(secName, "L", currentMaterial, cProps, 0.2f, 0.2f, 0.02f, 0.02f);
                else if (shapeNames["T"].Equals(shapeComboBox.SelectedItem.ToString()))
                    currentSection = new Tee(secName, "T", currentMaterial, cProps, 0.2f, 0.2f, 0.02f, 0.02f);
                else
                    currentSection = new General(secName, "G", currentMaterial, cProps, 0.2f, 0.2f, 0, 0, 0, 0, 0, 0.0043f, 9.65e-8f, 0.0000657f, 0.0000033f, 0.002f, 0.002f, 0.00043f, 0.000052f, 0.00049f, 0.000081f, 0.1241f, 0.0278f);
            }

            UpdateModel(1);

            if (currentSection.GetType().Name.Equals("General"))
                wizardControl.SelectTab(4);
            else
                wizardControl.SelectTab(2);

            //if (currentSection is Rectangular || currentSection is Circle)
            //  wizardControl.SelectTab(2);
            //else
            //    wizardControl.SelectTab(0);
            DialogResult = DialogResult.None;
        }

        private void nextButton3_Click(object sender, EventArgs e)
        {
            UpdateModel(2);
            if (currentSection.ConcreteProperties is ConcreteBeamSectionProps || currentSection.ConcreteProperties is ConcreteColumnSectionProps)
                wizardControl.SelectTab(3);
            else
                applyButton_Click(sender, e);
            DialogResult = DialogResult.None;
        }

        private void nextButton4_Click(object sender, EventArgs e)
        {
            UpdateModel(3);
            wizardControl.SelectTab(4);
        }

        private void shapeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (currentMaterial != null && currentMaterial.DesignProperties is ConcreteDesignProps &&
            //    concreteSections.ContainsKey(shapeComboBox.SelectedItem.ToString()))
            //    currentSection = concreteSections[shapeComboBox.SelectedItem.ToString()];
            //else if (currentMaterial != null && currentMaterial.DesignProperties is SteelDesignProps &&
            //    steelSections.ContainsKey(shapeComboBox.SelectedItem.ToString()))
            //    currentSection = steelSections[shapeComboBox.SelectedItem.ToString()];

            //if (allSections.ContainsKey(shapeComboBox.SelectedItem.ToString()))
            //    currentSection = allSections[shapeComboBox.SelectedItem.ToString()];
            //if (currentMaterial != null && currentSection != null)
            //    currentSection.Material = currentMaterial;
            //else if (currentSection != null)
            //    currentMaterial = currentSection.Material;
        }

        private void sectionsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((currentSection = sectionsTreeView.SelectedNode.Tag as FrameSection) == null)
            {
                editButton.Enabled = false;
                deleteButton.Enabled = false;
                propertiesButton.Enabled = false;
            }
            else
            {
                editButton.Enabled = true;
                deleteButton.Enabled = true;
                propertiesButton.Enabled = true;
            }
            addButton.Enabled = true;
        }

        private bool sectionInUse()
        {
            foreach (LineElement line in Canguro.Model.Model.Instance.LineList)
                if (line != null && line.Properties is StraightFrameProps
                    && ((StraightFrameProps)line.Properties).Section == currentSection)
                    return true;
            return false;

        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            UpdateModel();
            if (oneSection)
                DialogResult = DialogResult.OK;
            else
            {
                Canguro.Model.Model.Instance.Sections[currentSection.Name] = currentSection;
                SectionManager.Instance.DefaultFrameSection = currentSection;
                wizardControl.SelectTab(0);
                DialogResult = DialogResult.None;
            }
        }

        private void columnRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (columnRadioButton.Checked)
                currentSection.ConcreteProperties = new ConcreteColumnSectionProps();
            else if (beamRadioButton.Checked)
                currentSection.ConcreteProperties = new ConcreteBeamSectionProps();
            else
                currentSection.ConcreteProperties = new ConcreteSectionProps();
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            wizardControl.SelectTab(4);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            nextButton2.Enabled = (nameTextBox.Text.Length > 0);
        }

        private void sectionsTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (sectionsTreeView.SelectedNode.Tag is FrameSection)
                editButton_Click(sender, e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
            string path = "";
            string currentPath = model.CurrentPath;
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = "Treu Structure Sections (*.xsec)|*.xsec";
            dlg.DefaultExt = "xsec";
            dlg.AddExtension = true;
            dlg.FileName = currentPath;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;
            try
            {
                if (path.Length > 0)
                {
                    System.Xml.XmlTextWriter xml = new System.Xml.XmlTextWriter(path, System.Text.ASCIIEncoding.ASCII);
                    xml.WriteStartDocument();

                    xml.WriteStartElement("Sections");

                    xml.WriteStartElement("Frame_Section_Properties_01_-_General");
                    foreach (Section sec in model.Sections)
                        if (sec is FrameSection)
                            Canguro.Model.Serializer.Serializer.writeFrameSection(xml, (FrameSection)sec);
                    xml.WriteEndElement();

                    xml.WriteStartElement("Frame_Section_Properties_03_-_Concrete_Beam");
                    foreach (Section sec in model.Sections)
                        if (sec is FrameSection && ((FrameSection)sec).ConcreteProperties is ConcreteBeamSectionProps)
                            Canguro.Model.Serializer.Serializer.writeConcreteBeamAssigments(xml, (FrameSection)sec);
                    xml.WriteEndElement();

                    xml.WriteStartElement("Frame_Section_Properties_02_-_Concrete_Column");
                    foreach (Section sec in model.Sections)
                        if (sec is FrameSection && ((FrameSection)sec).ConcreteProperties is ConcreteColumnSectionProps)
                            Canguro.Model.Serializer.Serializer.writeConcreteColumnSectionProps(xml, (FrameSection)sec);
                    xml.WriteEndElement();

                    xml.WriteEndElement();
                    xml.Close();
                }
            }
            finally
            {
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
            string path = "";
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Treu Structure Sections (*.xsec)|*.xsec";
            dlg.DefaultExt = "xsec";
            dlg.AddExtension = true;
            if (model.CurrentPath.Length > 0)
                dlg.FileName = model.CurrentPath;
            dlg.CheckPathExists = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;
            try
            {
                if (path.Length > 0)
                    SectionManager.Instance.LoadXmlSections(path, model.Sections);
            }
            catch
            {
                MessageBox.Show(Culture.Get("errorLoadingFile") + " " + path, Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
            }
            UpdatePage1();
        }
    }
}