using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Canguro.Model.Section
{
    /// <summary>
    /// Clase Singleton que administra los catálogos de secciones.
    /// </summary>
    sealed public class SectionManager
    {
        /// <summary>
        /// Constructora privada para hacer singleton.
        /// </summary>
        private SectionManager()
        {
            catalogs = new Dictionary<string, Catalog<Section>>();
            DefaultShape = "R";
        }
        public static readonly SectionManager Instance = new SectionManager();

        private Dictionary<string, Catalog<Section>> catalogs;

        private FrameSection defaultFrameSection;
        public FrameSection DefaultFrameSection
        {
            get
            {
                if (defaultFrameSection == null)
                    defaultFrameSection = new IWideFlange("W10X100", "I/Wide Flange", Material.MaterialManager.Instance.DefaultSteel, null, 0.281940009689331f, 0.261620004844666f, 2.84480001211166E-02f, 0.017272000181675f, 0.261620004844666f, 2.84480001211166E-02f, 0f, 0.018967703753891f, 4.53692238026031E-06f, 2.593121781488E-04f, 8.61599050992E-05f, 4.86966758893967E-03f, 1.24042767424774E-02f, 1.83948477858489E-03f, 6.58664501977642E-04f, 0.00213031832f, 0.000999610904f, 0.116924111270662f, 6.73977234049011E-02f);
                //                    defaultSection = new Rectangular("R1x1", "R", Material.MaterialManager.Instance.DefaultSteel, null, 0.1F, 0.1F, .00001408F, .00000833F, .00000833F, .00833F, .00833F, .0001667F, .0001667F, .00025F, .00025F, .02887F, .02887F);
                return defaultFrameSection;
            }
            set
            {
                defaultFrameSection = value;
            }
        }

        private AreaSection defaultAreaSection;
        public AreaSection DefaultAreaSection
        {
            get
            {
                if (defaultAreaSection == null)
                    defaultAreaSection = new ShellSection("AS1", "Area", Material.MaterialManager.Instance.DefaultSteel, .1f, .1f, ShellType.ShellThin, 0f, null);
                return defaultAreaSection;
            }
            set
            {
                defaultAreaSection = value;
            }
        }

        public readonly string DefaultShape;

        /// <summary>
        /// Inicializa leyendo todos los archivos *.sec en el directorio \section
        /// Se tiene que llamar después de crear Model
        /// </summary>
        public void Initialize(ref Catalog<Canguro.Model.Section.Section> modelSections)
        {
            catalogs.Clear();

            if (this["modelCatalog"] == null)
                this["modelCatalog"] = new Catalog<Canguro.Model.Section.Section>();
            modelSections = this["modelCatalog"];
            modelSections[DefaultFrameSection.Name] = DefaultFrameSection;

            // Create a reference to the current directory.
            DirectoryInfo di = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + @"\RuntimeData\section");

            if (!di.Exists)
                di = new DirectoryInfo(@"section");

            // Create an array representing the files in the current directory.
            FileInfo[] fi = di.GetFiles();

            //Console.WriteLine("The following files exist in the current directory:");
            // Print out the names of the files in the current directory.
            foreach (FileInfo fiTemp in fi)
            {
                //                Console.WriteLine(fiTemp.Name);
                try
                {
                    if (fiTemp.Extension.Equals(".sec"))
                    {
                        Catalog<Section> tmp = new Catalog<Section>();
                        tmp.Load(fiTemp.FullName);
                        if (!catalogs.ContainsKey(tmp.Name))
                        {
                            catalogs.Add(tmp.Name, tmp);
                            tmp.CatalogChanged += new EventHandler(oneCatalogChanged);
                        }
                    }
                    else if (fiTemp.Extension.Equals(".txt"))
                    {
                        string name = fiTemp.Name;
                        name = name.Substring(0, name.Length - 4);
                        Catalog<Section> tmp = new Catalog<Section>(name, false);
                        LoadTxtCatalog(tmp, fiTemp.FullName);
                        if (!catalogs.ContainsKey(tmp.Name))
                        {
                            catalogs.Add(tmp.Name, tmp);
                            tmp.CatalogChanged += new EventHandler(oneCatalogChanged);
                        }
                        tmp.IsReadOnly = true;
                        //tmp.Save(fiTemp.Directory + "\\" + tmp.Name + ".sec");
                    }
                    else if (fiTemp.Extension.Equals(".xsec"))
                    {
                        if (!catalogs.ContainsKey(Culture.Get("userSectionsCatalog")))
                            catalogs.Add(Culture.Get("userSectionsCatalog"), new Catalog<Section>(Culture.Get("userSectionsCatalog"), false));
                        LoadXmlSections(fiTemp.FullName, catalogs[Culture.Get("userSectionsCatalog")]);
                    }
                }
                catch (System.IO.FileNotFoundException e)
                {
                    // Reconstruir catálogo
                    throw e;
                }
            }
        }

        public event EventHandler CatalogChanged;
        void oneCatalogChanged(object sender, EventArgs e)
        {
            if (CatalogChanged != null) CatalogChanged(sender, e);
        }

        /// <summary>
        /// Indexador de catálogos. Regresa (asigna) el catálogo dado su nombre.
        /// </summary>
        /// <param name="key">Nombre del catálogo</param>
        /// <returns>Catálogo</returns>
        public Catalog<Section> this[string key]
        {
            get
            {
                return (catalogs.ContainsKey(key)) ? catalogs[key] : null;
            }
            set
            {
                if (catalogs.ContainsKey(key))
                    throw new InvalidIndexException();

                catalogs[key] = value;
                value.CatalogChanged += new EventHandler(oneCatalogChanged);
            }
        }

        private void LoadTxtCatalog(Catalog<Section> cat, string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            Material.Material mat = Material.MaterialManager.Instance.DefaultSteel;
            ConcreteSectionProps csp = null;
            try
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] arr = line.Split("\t".ToCharArray());
                    if (arr.Length == 23)
                    {
                        Section sec = NewSection(mat, csp, arr);
                        cat[sec.Name] = sec;
                    }
                }
            }
            finally
            {
                reader.Close();
                stream.Close();
            }

        }

        public static Section NewSection(Material.Material mat, ConcreteSectionProps csp, string[] arr)
        {
            if (arr.Length == 23)
            {
                switch (arr[1])
                {
                    case "Double Angle":
                        return new DoubleAngle(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    case "Channel":
                        return new Channel(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    case "I/Wide Flange":
                        return new IWideFlange(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    case "Box/Tube":
                        return new BoxTube(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    case "Pipe":
                        return new Pipe(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    case "Angle":
                        return new Angle(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    case "Tee":
                        return new Tee(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                    default:
                        return new General(arr[0], arr[1], mat, csp, Convert.ToSingle(arr[4]), Convert.ToSingle(arr[5]), Convert.ToSingle(arr[6]), Convert.ToSingle(arr[7]), Convert.ToSingle(arr[8]), Convert.ToSingle(arr[9]), Convert.ToSingle(arr[10]), Convert.ToSingle(arr[11]), Convert.ToSingle(arr[12]), Convert.ToSingle(arr[13]), Convert.ToSingle(arr[14]), Convert.ToSingle(arr[15]), Convert.ToSingle(arr[16]), Convert.ToSingle(arr[17]), Convert.ToSingle(arr[18]), Convert.ToSingle(arr[19]), Convert.ToSingle(arr[20]), Convert.ToSingle(arr[21]), Convert.ToSingle(arr[22]));
                }
            }
            return null;
        }

        public void LoadXmlSections(string filePath, Catalog<Section> cat)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode sections = doc.SelectSingleNode("//Frame_Section_Properties_01_-_General");
                if (sections != null)
                {
                    XmlNodeList instances = sections.SelectNodes("SectionName");

                    List<string> knownTemplates = new List<string>(new string[] { "Double Angle", "Channel", "I/Wide Flange", "Box/Tube", "Pipe", "Angle", "Tee" });
                    if (instances != null && instances.Count > 0)
                    {
                        foreach (XmlNode node in instances)
                        {
                            string temp = CustomSection.readAttribute(node, "Shape", "");
                            Section sec = null;
                            if (knownTemplates.Contains(temp))
                                sec = NewSection(Material.MaterialManager.Instance.DefaultSteel, null, LoadXmlVariables(node));
                            else
                                sec = new CustomSection(node);

                            if (sec != null)
                                cat[sec.Name] = sec;
                        }
                    }
                }

                Canguro.Model.Serializer.Deserializer deserializer = new Canguro.Model.Serializer.Deserializer(Model.Instance);
                XmlNode beams = doc.SelectSingleNode("//Frame_Section_Properties_03_-_Concrete_Beam");
                if (beams != null)
                    deserializer.readFrameConcreteBeams(beams, cat);
                XmlNode columns = doc.SelectSingleNode("//Frame_Section_Properties_02_-_Concrete_Column");
                if (columns != null)
                    deserializer.readFrameConcreteColumns(columns, cat);
            }
            catch
            {
                MessageBox.Show(Culture.Get("WrongXsecFile") + ": " + filePath, Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string[] LoadXmlVariables(XmlNode xml)
        {
            string[] ret = new string[23];
            ret[0] = CustomSection.readAttribute(xml, "SectionName", "sec");
            ret[1] = CustomSection.readAttribute(xml, "Shape", "G");
            ret[4] = CustomSection.readAttribute(xml, "t3", "0");
            ret[5] = CustomSection.readAttribute(xml, "t2", "0");
            ret[6] = CustomSection.readAttribute(xml, "tf", "0");
            ret[7] = CustomSection.readAttribute(xml, "tw", "0");
            ret[8] = CustomSection.readAttribute(xml, "t2b", "0");
            ret[9] = CustomSection.readAttribute(xml, "tfb", "0");
            ret[10] = CustomSection.readAttribute(xml, "dis", "0");
            ret[11] = CustomSection.readAttribute(xml, "Area", "0");
            ret[12] = CustomSection.readAttribute(xml, "TorsConst", "0");
            ret[13] = CustomSection.readAttribute(xml, "I33", "0");
            ret[14] = CustomSection.readAttribute(xml, "I22", "0");
            ret[15] = CustomSection.readAttribute(xml, "AS2", "0");
            ret[16] = CustomSection.readAttribute(xml, "AS3", "0");
            ret[17] = CustomSection.readAttribute(xml, "S33", "0");
            ret[18] = CustomSection.readAttribute(xml, "S22", "0");
            ret[19] = CustomSection.readAttribute(xml, "Z33", "0");
            ret[20] = CustomSection.readAttribute(xml, "Z22", "0");
            ret[21] = CustomSection.readAttribute(xml, "R33", "0");
            ret[22] = CustomSection.readAttribute(xml, "R22", "0");
            return ret;
        }


        // Variables para acelerar la obtención del árbol.
        private TreeNode rootNodeCache;

        /// <summary>
        /// Propiedad de sólo lectura que genera un árbol para desplegar todos los catálogos.
        /// </summary>
        public TreeNode Tree
        {
            get
            {
                rootNodeCache = new TreeNode(Culture.Get("rootSectionCatalog"));
                foreach (string key in catalogs.Keys)
                {
                    if (key.Length > 0)
                        rootNodeCache.Nodes.Add(catalogs[key].Tree);
                }

                return rootNodeCache;
            }
        }
    }
}
