using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Clase Singleton que administra los materiales a través de un catálogo único.
    /// </summary>
    sealed public class MaterialManager
    {
        /// <summary>
        /// Constructora privada
        /// </summary>
        private MaterialManager() {
            materials = new Catalog<Material>();
            materials.CatalogChanged += new EventHandler(materials_CatalogChanged);
        }

        /// <summary>
        /// Definición del material Default (Acero)
        /// </summary>
        private Material defaultSteel = null;
        private Material defaultConcrete = null;
        private Material defaultRebar = null;

        public Material DefaultSteel
        {
            get
            {
                if (defaultSteel == null) // Always null first time
                {
                    defaultSteel = materials[Properties.Settings.Default.DefaultSteel];
                }
                if (defaultSteel == null) // Still
                {
                    foreach (Material mat in materials)
                        if (mat.DesignProperties.GetType().Name.Equals("SteelDesignProps"))
                        {
                            defaultSteel = mat;
                            break;
                        }
                }
                if (defaultSteel == null) // Still
                {
                    if (materials[Properties.Settings.Default.DefaultSteel] == null)
                        defaultSteel = new Material(Properties.Settings.Default.DefaultSteel, true, new SteelDesignProps(), new IsotropicTypeProps(1.999E+11F, 0.3F, 0.0000117F), 7849F);
                    else
                    {
                        defaultSteel = materials[Culture.Get("defaultSteelName")];
                        //materials[defaultSteel.Name] = defaultSteel;
                    }
                }
                return defaultSteel;
            }
        }

        public Material DefaultRebar
        {
            get
            {
                if (defaultRebar == null) // Always null
                {
                    foreach (Material mat in materials)
                        if (mat.DesignProperties is RebarDesignProps)
                        {
                            defaultRebar = mat;
                            break;
                        }
                }
                if (defaultRebar == null) // Always null
                {
                    if (materials[Culture.Get("rebarName")] == null)
                        defaultRebar = new Material(Culture.Get("rebarName"), true, new RebarDesignProps(), new UniaxialTypeProps(1.99947978795958E+11F, 0.0000117F), 7849F);
                    else
                    {
                        defaultRebar = materials[Culture.Get("rebarName")];
                        //materials[defaultRebar.Name] = defaultRebar;
                    }
                }
                return defaultRebar;
            }
        }

        public Material DefaultConcrete
        {
            get
            {
                if (defaultConcrete == null) // Always null first time
                {
                    defaultConcrete = materials[Properties.Settings.Default.DefaultConcrete];
                }
                if (defaultConcrete == null) // Still null?
                {
                    foreach (Material mat in materials)
                        if (mat.DesignProperties is ConcreteDesignProps)
                        {
                            defaultConcrete = mat;
                            break;
                        }
                }
                if (defaultConcrete == null) // Still null?
                {
                    if (materials[Culture.Get("concreteName")] == null)
                        defaultConcrete = new Material(Culture.Get("concreteName"), true, new ConcreteDesignProps(27579031.56F, 344737894, 448159263, false, 1), new IsotropicTypeProps(24860000000, 0.2F, 0.0000099F), 2402.8F);
                    else
                    {
                        defaultConcrete = materials[Culture.Get("concreteName")];
                        //materials[defaultConcrete.Name] = defaultConcrete;
                    }
                }
                return defaultConcrete;
            }
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static readonly MaterialManager Instance = new MaterialManager();

        /// <summary>
        /// Catálogo de materiales
        /// </summary>
        private Catalog<Material> materials;

        /// <summary>
        /// Propiedad de sólo lectura que regresa el catálogo de materiales.
        /// </summary>
        public Catalog<Material> Materials
        {
            get
            {
                return materials;
            }
        }

        /// <summary>
        /// Inicializa el catálogo cargando el archivo default.mat
        /// Si no encuentra el archivo, carga los datos 'hardcoded'
        /// </summary>
        public void Initialize()
        {
            try
            {
                materials.Load(System.Windows.Forms.Application.StartupPath + "\\RuntimeData\\default.mat");
                if (materials[DefaultConcrete.Name] == null)
                    materials[DefaultConcrete.Name] = DefaultConcrete;
                if (materials[DefaultSteel.Name] == null)
                    materials[DefaultSteel.Name] = DefaultSteel;
                if (materials[DefaultRebar.Name] == null)
                    materials[DefaultRebar.Name] = DefaultRebar;
            }
            catch (Exception)
            {
                // Reconstruir catálogo de materiales
                materials = new Catalog<Material>();
                //                defaultMaterial = new Material(Culture.Get("aluminumName"), false, new AluminumDesignProps(), new IsotropicTypeProps(69637054684.101F, 0.33F, 0.00002358F), 2.714F);
//                materials[DefaultMaterial.Name] = DefaultMaterial;
                //                defaultMaterial = new Material(Culture.Get("coldFormedName"), false, new ColdFormedDesignProps(), new IsotropicTypeProps(2.03395357740716E+11F, 0.3F, 0.0000117F), 7.849F);
//                materials[DefaultMaterial.Name] = DefaultMaterial;
/************ CONCRETE *************/
                defaultConcrete = new Material("4000Psi", false, new ConcreteDesignProps(27579031.558F, 413685473.37F, 275790315.58F, false, 1.0F), new IsotropicTypeProps(24821128402.26F, 0.2F, 0.0000099F), 2403F);
                materials[defaultConcrete.Name] = DefaultConcrete;
                materials["3000Psi"] = new Material("3000Psi", false, new ConcreteDesignProps(20684273.67f, 413685473.37F, 275790315.58F, false, 1f), new IsotropicTypeProps(21530000000f, 0.2f, 0.0000099f), 2402.8f);
                //                materials["4000Psi"] = new Material("4000Psi", false, new ConcreteDesignProps(27579031.56f, 413685473.37F, 275790315.58F, false, 1f), new IsotropicTypeProps(24860000000f, 0.2f, 0.0000099f), 2402.8f);
                materials["5000Psi"] = new Material("5000Psi", false, new ConcreteDesignProps(34473789.45f, 413685473.37F, 275790315.58F, false, 1f), new IsotropicTypeProps(27790000000f, 0.2f, 0.0000099f), 2402.8f);
                materials["6000Psi"] = new Material("6000Psi", false, new ConcreteDesignProps(41368547.34f, 413685473.37F, 275790315.58F, false, 1f), new IsotropicTypeProps(30440000000f, 0.2f, 0.0000099f), 2402.8f);

                //defaultSteel = new Material(Culture.Get("noMaterialName"), true, new NoDesignProps(), new IsotropicTypeProps(24821128402.26F, 0.2F, 0.0000099F), 2401F);
                //materials[DefaultSteel.Name] = DefaultSteel;
/************ REBAR *************/
                defaultRebar = new Material(Culture.Get("rebarName"), false, new RebarDesignProps(), new UniaxialTypeProps(1.99947978795958E+11F, 0.0000117F), 7849F);
                materials[defaultRebar.Name] = defaultRebar;
                
/************ STEEL *************/
                materials["A36"] = new Material("A36", false, new SteelDesignProps(248211284f, 399895958f), new IsotropicTypeProps(1.99947978795958E+11F, 0.3F, 0.0000117F), 7849F);
                defaultSteel = materials["A36"];
                materials["A50"] = new Material("A50", false, new SteelDesignProps(379211684f, 427474989f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);
                materials["A500GrB42"] = new Material("A500GrB42", false, new SteelDesignProps(289579831.4f, 399895958f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);
                materials["A500GrB46"] = new Material("A500GrB46", false, new SteelDesignProps(317158863f, 399895958f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);
                materials["A53GrB"] = new Material("A53GrB", false, new SteelDesignProps(241316526.1f, 413685473f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);
                materials["A572Gr50"] = new Material("A572Gr50", false, new SteelDesignProps(344737894, 448159263f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);
                materials["A913Gr50"] = new Material("A913Gr50", false, new SteelDesignProps(344737894, 413685473f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);
                materials["A992Fy50"] = new Material("A992Fy50", false, new SteelDesignProps(344737894, 448159263f), new IsotropicTypeProps(1.999E+11f, 0.3f, 0.0000117f), 7849);

                materials.Save(System.Windows.Forms.Application.StartupPath + "\\RuntimeData\\default.mat");
                //throw e;
            }
        }


        public void Initialize(System.Xml.XmlNode xml)
        {
            try
            {
                materials = new Catalog<Material>();
                foreach (System.Xml.XmlNode child in xml.ChildNodes)
                    if ("Material".Equals(child.Name))
                        readMaterial(child);
            }
            catch
            {
                Initialize();
            }
        }

        private void readMaterial(System.Xml.XmlNode node)
        {
            if (!"Material".Equals(node.Name))
                return;

            string att;
            string name = Canguro.Model.Serializer.Deserializer.readAttribute(node, "Material", "MAT");
            MaterialTypeProps tProps;

            float e = float.Parse(Canguro.Model.Serializer.Deserializer.readAttribute(node, "E", "0"));
            float u = float.Parse(Canguro.Model.Serializer.Deserializer.readAttribute(node, "U", "0"));
            float a = float.Parse(Canguro.Model.Serializer.Deserializer.readAttribute(node, "A", "0"));
            att = Canguro.Model.Serializer.Deserializer.readAttribute(node, "Type", "Isotropic");
            switch (att)
            {
                case "Uniaxial":
                    tProps = new UniaxialTypeProps(e, a);
                    break;
                case "Isotropic":
                default:
                    tProps = new IsotropicTypeProps(e, u, a);
                    break;
            }

            string design = Canguro.Model.Serializer.Deserializer.readAttribute(node, "DesignType", "None");
            MaterialDesignProps dProps;
            switch (design)
            {
                case "Rebar":
                    dProps = new RebarDesignProps();
                    break;
                case "ColdFormed":
                    dProps = new ColdFormedDesignProps();
                    break;
                case "Steel":
                    dProps = new SteelDesignProps(); // Changes when readSteelDesignProps() is called.
                    break;
                case "Concrete":
                    dProps = new NoDesignProps(); // Changes when readConcreteDesignProps() is called.
                    break;
                case "Aluminum":
                    dProps = new AluminumDesignProps();
                    break;
                default:
                    dProps = new NoDesignProps();
                    break;
            }

            float d = float.Parse(Canguro.Model.Serializer.Deserializer.readAttribute(node, "UnitMass", "d"));

            Material mat = new Material(name, false, dProps, tProps, d);
            Materials[name] = mat;
        }

        public event EventHandler CatalogChanged;
        void materials_CatalogChanged(object sender, EventArgs e)
        {
            if (CatalogChanged != null) CatalogChanged(sender, e);
        }
    }
}
