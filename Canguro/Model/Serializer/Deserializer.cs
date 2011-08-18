using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Canguro.Model.Load;
using Canguro.Model.Material;
using Canguro.Model.Section;
using Canguro.Model.Design;
using Canguro.Model;
using Canguro;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.Serialization.Formatters.Binary;

namespace Canguro.Model.Serializer
{
    internal class Deserializer
    {
        protected Model model;

        public Deserializer(Model model)
        {
            this.model = model;
        }

        /// <summary>
        /// Create a temporary XML file that will be send to SAP application
        /// </summary>
        /// <param name="m"></param>
        /// <param name="filePath"></param>
        internal void Deserialize(string filePath)
        {
            Stream stream = new MemoryStream();
            System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursor.Current;
            try
            {
                byte[] buffer = new byte[8192];
                if (".xml".Equals(Path.GetExtension(filePath).ToLower()))
                {
                    FileStream fReader = File.OpenRead(filePath);
                    Deserialize(fReader);
                    fReader.Close();
                }
                else
                {
                    FileStream fs = File.OpenRead(filePath);
                    BinaryFormatter bformatter = new BinaryFormatter();
                    string version = (string)bformatter.Deserialize(fs); //"version=7.14"
                    if ("version=7.14".CompareTo(version) > 0)
                    {
                        fs.Close();
                        string translate = System.Windows.Forms.Application.StartupPath + "\\Translate.exe";
                        if (File.Exists(translate))
                        {
                            string msg = string.Format("El archivo {0} es para una versión anterior de Treu Structure. ¿Desea traducirlo a la versión actual? Se guardará una copia del original en {0}.bak", filePath);
                            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(msg,
                                "Actualización", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(translate);
                                start.Arguments = "\"" + filePath + "\"";
                                start.UseShellExecute = false;
                                start.RedirectStandardOutput = true;
                                System.Diagnostics.Process p = System.Diagnostics.Process.Start(start);
                                p.WaitForExit(60000);
                                string output = p.StandardOutput.ReadToEnd();
                                if (p.HasExited && p.ExitCode == 0)
                                    Deserialize(filePath);
                                else
                                {
                                    msg = "Ocurrió un error y no se pudo actualizar el archivo. Favor de contactar a Soporte Técnico o a contacto@treusoft.com";
                                    System.Windows.Forms.MessageBox.Show(msg, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                }
                            }
                            else
                                throw new Exception("File cannot be opened");
                        }
                        else
                            throw new Exception("File cannot be opened");
                    }
                    else
                    {
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                        using (ZipInputStream s = new ZipInputStream(fs))
                        {
                            try
                            {
                                s.Password = "mnloaw7ur4hlu.awdebnc7loy2hq3we89";
                                if (s.GetNextEntry() != null)
                                {
                                    StreamUtils.Copy(s, stream, buffer);
                                    stream.Position = 0;
                                    Deserialize(stream);
                                }
                            }
                            finally
                            {
                                s.Close();
                                s.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                stream.Close();
                System.Windows.Forms.Cursor.Current = cursor;
            }
        }

        internal void Deserialize(Stream file)
        {
            XmlDocument doc = new XmlDocument();
#if DEBUG
            byte[] buf = new byte[file.Length];
            file.Read(buf, 0, (int)file.Length);
            //File.WriteAllText("C:\\tmp.txt", System.Text.Encoding.ASCII.GetString(buf));
            file.Position = 0;
#endif
            model.Reset();
            doc.Load(file);
            Canguro.Model.UnitSystem.UnitSystem uSystem = model.UnitSystem;
            model.UnitSystem = Canguro.Model.UnitSystem.InternationalSystem.Instance;
            XmlNode root = doc.SelectSingleNode("//XmlExportedFile");
            try
            {
                MarkToDelete();
                readBasicElements(root);
                foreach (XmlNode child in root.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "Joint_Added_Mass_Assignments":
                            readJointMasses(child);
                            break;
                        case "Joint_Restraint_Assignments":
                            readJointRestraints(child);
                            break;
                        case "Joint_Spring_Assignments_1_-_Uncoupled":
                            readJointSprings(child);
                            break;
                        case "Joint_Loads_-_Force":
                            readJointForces(child);
                            break;
                        case "Joint_Loads_-_Ground_Displacement":
                            readJointGroundDisplacements(child);
                            break;
                        case "Frame_Section_Properties_03_-_Concrete_Beam":
                            readFrameConcreteBeams(child, model.Sections);
                            break;
                        case "Frame_Section_Properties_02_-_Concrete_Column":
                            readFrameConcreteColumns(child, model.Sections);
                            break;
                        case "Frame_Section_Assignments":
                            readFrameSectionAssignments(child);
                            break;
                        case "Frame_Release_Assignments_1_-_General":
                            readFrameReleases(child);
                            break;
                        case "Frame_Release_Assignments_2_-_Partial_Fixity":
                            readFramePartialFixities(child);
                            break;
                        case "Frame_Local_Axes_Assignments_1_-_Typical":
                            readFrameAngles(child);
                            break;
                        case "Frame_Loads_-_Point":
                            readFramePointLoads(child);
                            break;
                        case "Frame_Loads_-_Distributed":
                            readFrameDistributedLoads(child);
                            break;
                        case "Frame_Loads_-_Temperature":
                            readFrameTemperatureLoads(child);
                            break;
                        case "Case_-_Static_1_-_Load_Assignments":
                            readStaticCases(child);
                            break;
                        case "Case_-_Modal_1_-_General":
                            readModalCases(child);
                            break;
                        case "Case_-_Modal_3_-_Load_Assignments_-_Ritz":
                            readModalRitzCases(child);
                            break;
                        case "Case_-_Response_Spectrum_1_-_General":
                            readResponseSpectrumGeneralCases(child);
                            break;
                        case "Case_-_Response_Spectrum_2_-_Load_Assignments":
                            readResponseSpectrumAssignmentsCases(child);
                            break;
                        case "Case_-_Static_4_-_NonLinear_Parameters":
                            readNonLinearParameters(child);
                            break;
                        case "Case_-_Static_2_-_NonLinear_Load_Application":
                            readNonLinearLoadApplication(child);
                            break;
                        case "Frame_Insertion_Point_Assignments":
                            readFrameInsertionPointAssignments(child);
                            break;
                        case "Frame_Offset_Along_Length_Assignments":
                            readFrameOffsetAssignments(child);
                            break;
                        case "Joint_Constraint_Assignments":
                            readConstraintAssignments(child);
                            break;
                        case "T-Results":
                            new ResultsDeserializer(model).Deserialize(child);
                            break;
                        case "T-Treu_Options":
                            readCanguroVariables(child);
                            break;
                        //case "Frame_Design_Procedures":
                        //    readFrameDesignProcs(child);
                        //    break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(Culture.Get("errorLoadingFile"), Culture.Get("error"),
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                DeleteMarked();
                //                m.UnitSystem = uSystem;
            }
        }

        private void readBasicElements(XmlNode root)
        {
            XmlNode node;
            node = root.SelectSingleNode("Material_Properties_01_-_General");
            if (node != null)
                readMaterials(node);
            node = root.SelectSingleNode("Material_Properties_04_-_Design_Concrete");
            if (node != null)
                readConcreteMaterials(node);
            node = root.SelectSingleNode("Material_Properties_11_-_Design_Rebar");
            if (node != null)
                readRebarMaterials(node);
            node = root.SelectSingleNode("Material_Properties_05_-_Design_Aluminum");
            if (node != null)
                readAluminumMaterials(node);
            node = root.SelectSingleNode("Material_Properties_06_-_Design_ColdFormed");
            if (node != null)
                readColdFormedMaterials(node);
            node = root.SelectSingleNode("Material_Properties_03_-_Design_Steel");
            if (node != null)
                readSteelMaterials(node);
            node = root.SelectSingleNode("Frame_Section_Properties_01_-_General");
            if (node != null)
                readFrameSections(node, model.Sections);
            node = root.SelectSingleNode("Load_Case_Definitions");
            if (node != null)
                readLoadCases(node);
            node = root.SelectSingleNode("Analysis_Case_Definitions");
            if (node != null)
                readAnalysisCases(node);
            node = root.SelectSingleNode("Preferences_-_Concrete_Design_-_Mexican_RCDF_2001");
            if (node != null)
                readRCDF2001(node);
            node = root.SelectSingleNode("Preferences_-_Concrete_Design_-_ACI_318-02");
            if (node != null)
                readACI318_02(node);
            node = root.SelectSingleNode("Preferences_-_Concrete_Design_-_UBC97");
            if (node != null)
                readUBC97_Conc(node);
            node = root.SelectSingleNode("Preferences_-_Steel_Design_-_AISC-LRFD99");
            if (node != null)
                readLRFD99(node);
            node = root.SelectSingleNode("Preferences_-_Steel_Design_-_UBC97-ASD");
            if (node != null)
                readUBC97_ASD(node);
            node = root.SelectSingleNode("Preferences_-_Steel_Design_-_UBC97-LRFD");
            if (node != null)
                readUBC97_LRFD(node);
            node = root.SelectSingleNode("Preferences_-_Steel_Design_-_AISC-ASD01");
            if (node != null)
                readASD01(node);
            node = root.SelectSingleNode("Program_Control");
            if (node != null)
                readProgramControl(node);
            node = root.SelectSingleNode("Combination_Definitions");
            if (node != null)
                readCombinationsCases(node);
            node = root.SelectSingleNode("Function_-_Response_Spectrum_-_User");
            if (node != null)
                readResponseSpectrum(node);
            node = root.SelectSingleNode("T-Layers_1_-_Definitions");
            if (node != null)
                readLayers(node);
            node = root.SelectSingleNode("Joint_Coordinates");
            if (node != null)
                readJointCoordinates(node);
            node = root.SelectSingleNode("Connectivity_-_Frame");
            if (node != null)
                readFrames(node);
            node = root.SelectSingleNode("T-Layers_2_-_Assignments");
            if (node != null)
                readAllLayerAssignments(node);
            node = root.SelectSingleNode("Constraint_Definitions_-_Diaphragm");
            if (node != null)
                readDiaphragm(node);
        }

        private const string mark = "@###$$%||||FD#$E#EWQERQW#$#$@#$EDQW#";
        private void MarkToDelete()
        {
            int i = 0;
            List<string> names = new List<string>(model.LoadCases.Keys);
            foreach (string lc in names)
                if (model.LoadCases[lc] != null)
                    model.LoadCases[lc].Name = mark + (i++);

            List<AbstractCase> acs = new List<AbstractCase>(model.AbstractCases);
            foreach (AbstractCase ac in acs)
                if (ac != null)
                    ac.Name = mark + (i++);

            List<Layer> layers = new List<Layer>(model.Layers);
            foreach (Layer l in layers)
                if (l != null)
                    l.Name = mark + (i++);

            List<Material.Material> materials = new List<Canguro.Model.Material.Material>(MaterialManager.Instance.Materials);
            foreach (Material.Material mat in materials)
            {
                if (mat != null)
                {
                    mat.DesignProperties = new NoDesignProps();
                    MaterialManager.Instance.Materials[mat.Name].Name = mark + (i++);
                }
            }
        }

        private void DeleteMarked()
        {
            List<string> names = new List<string>(model.LoadCases.Keys);
            foreach (string lc in names)
                if (model.LoadCases[lc].Name.StartsWith(mark))
                    model.LoadCases.Remove(lc);
            if (!model.LoadCases.ContainsValue(model.ActiveLoadCase))
                foreach (LoadCase lc in model.LoadCases.Values)
                {
                    model.ActiveLoadCase = lc;
                    break;
                }

            List<AbstractCase> acs = new List<AbstractCase>(model.AbstractCases);
            foreach (AbstractCase ac in acs)
                if (ac.Name.StartsWith(mark))
                    model.AbstractCases.Remove(ac);

            List<Layer> layers = new List<Layer>(model.Layers);
            foreach (Layer l in layers)
                if (l != null)
                    if (l.Name.StartsWith(mark))
                        model.Layers.Remove(l);

            List<Material.Material> materials = new List<Canguro.Model.Material.Material>(MaterialManager.Instance.Materials);
            foreach (Material.Material mat in materials)
                if (MaterialManager.Instance.Materials[mat.Name].Name.StartsWith(mark))
                    MaterialManager.Instance.Materials[mat.Name] = null;
        }

        /// <summary>
        /// Create nodes in XML for joint elements
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="obj"></param>
        private void readJointCoordinates(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Joint".Equals(child.Name))
                    readJoint(child);
        }

        private void readJointMasses(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Joint".Equals(child.Name))
                    readMasses(child);
        }

        private void readJointRestraints(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Joint".Equals(child.Name))
                    readRestraints(child);
        }

        private void readJointSprings(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Joint".Equals(child.Name))
                    readSprings(child);
        }

        private void readJointForces(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Joint".Equals(child.Name))
                    readJointForce(child);
        }

        private void readJointGroundDisplacements(XmlNode node)
        {
            // xml.WriteStartElement("Joint_Loads_-_Ground_Displacement");
            foreach (XmlNode child in node.ChildNodes)
                if ("Joint".Equals(child.Name))
                    readLoadGroundDisplacements(child);

        }

        private void readJoint(XmlNode node)
        {
            string str;
            // xml.WriteStartElement("Joint");
            str = readAttribute(node, "Joint", "0");
            uint id = uint.Parse(str);
            while (model.JointList.Count <= id)
                model.JointList.Add(null);
            Joint j = new Joint();
            j.Id = id;
            model.JointList.Add(j);

            //readAttribute(node, "CoordSys", "GLOBAL");
            //readAttribute(node, "CoordType", "Cartesian");
            j.X = float.Parse(readAttribute(node, "XorR", j.X.ToString()));
            j.Y = float.Parse(readAttribute(node, "Y", j.Y.ToString()));
            j.Z = float.Parse(readAttribute(node, "Z", j.Z.ToString()));
            //readAttribute(node, "SpecialJt", "Yes");
            //readAttribute(node, "GlobalX", pos.X.ToString());
            //readAttribute(node, "GlobalY", pos.Y.ToString());
            //readAttribute(node, "GlobalZ", pos.Z.ToString());

        }

        private void readMasses(XmlNode node)
        {
            float[] masses = new float[6];
            // xml.WriteStartElement("Joint");
            uint jid = uint.Parse(readAttribute(node, "Joint", "0"));
            //readAttribute(node, "CoordSys", "GLOBAL");
            masses[0] = float.Parse(readAttribute(node, "Mass1", masses[0].ToString()));
            masses[1] = float.Parse(readAttribute(node, "Mass2", masses[1].ToString()));
            masses[2] = float.Parse(readAttribute(node, "Mass3", masses[2].ToString()));
            masses[3] = float.Parse(readAttribute(node, "MMI1", masses[3].ToString()));
            masses[4] = float.Parse(readAttribute(node, "MMI2", masses[4].ToString()));
            masses[5] = float.Parse(readAttribute(node, "MMI3", masses[5].ToString()));
            Joint j = model.JointList[jid];
            if (j != null)
                j.Masses = masses;
        }

        private void readRestraints(XmlNode node)
        {
            int jid = int.Parse(readAttribute(node, "Joint", "0"));
            Joint j = model.JointList[jid];
            if (j != null)
            {
                JointDOF dof = j.DoF;
                dof.T1 = ("Yes".Equals(readAttribute(node, "U1", "No"))) ? JointDOF.DofType.Restrained : dof.T1;
                dof.T2 = ("Yes".Equals(readAttribute(node, "U2", "No"))) ? JointDOF.DofType.Restrained : dof.T2;
                dof.T3 = ("Yes".Equals(readAttribute(node, "U3", "No"))) ? JointDOF.DofType.Restrained : dof.T3;
                dof.R1 = ("Yes".Equals(readAttribute(node, "R1", "No"))) ? JointDOF.DofType.Restrained : dof.R1;
                dof.R2 = ("Yes".Equals(readAttribute(node, "R2", "No"))) ? JointDOF.DofType.Restrained : dof.R2;
                dof.R3 = ("Yes".Equals(readAttribute(node, "R3", "No"))) ? JointDOF.DofType.Restrained : dof.R3;
                j.DoF = dof;
            }
        }

        private void readSprings(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Joint", "0"));
            Joint obj = model.JointList[id];
            if (obj != null)
            {
                float u1 = float.Parse(readAttribute(node, "U1", "0"));
                float u2 = float.Parse(readAttribute(node, "U2", "0"));
                float u3 = float.Parse(readAttribute(node, "U3", "0"));
                float r1 = float.Parse(readAttribute(node, "R1", "0"));
                float r2 = float.Parse(readAttribute(node, "R2", "0"));
                float r3 = float.Parse(readAttribute(node, "R3", "0"));
                JointDOF dof = obj.DoF;
                dof.SpringValues = new float[] { u1, u2, u3, r1, r2, r3 };
                obj.DoF = dof;
            }
        }

        private void readJointForce(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Joint", "0"));
            string lcName = readAttribute(node, "LoadCase", "").Trim();
            lcName = (lcName.Length > 0) ? lcName : Culture.Get("Case");
            LoadCase lCase = model.LoadCases[lcName];
            Joint obj = model.JointList[id];
            if (obj != null && lCase != null)
            {
                AssignedLoads loads = obj.Loads;
                //                readAttribute(node, "CoordSys", "GLOBAL");
                ForceLoad load = new ForceLoad();
                load.Fx = float.Parse(readAttribute(node, "F1", "0"));
                load.Fy = float.Parse(readAttribute(node, "F2", "0"));
                load.Fz = float.Parse(readAttribute(node, "F3", "0"));
                load.Mx = float.Parse(readAttribute(node, "M1", "0"));
                load.My = float.Parse(readAttribute(node, "M2", "0"));
                load.Mz = float.Parse(readAttribute(node, "M3", "0"));
                loads.Add(load, lCase);
            }
        }

        private void readLoadGroundDisplacements(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Joint", "0"));
            string lcName = readAttribute(node, "LoadCase", "").Trim();
            lcName = (lcName.Length > 0) ? lcName : Culture.Get("Case");
            LoadCase lCase = model.LoadCases[lcName];
            Joint obj = model.JointList[id];
            
            if (obj != null && lCase != null)
            {
                AssignedLoads loads = obj.Loads;
                //                readAttribute(node, "CoordSys", "GLOBAL");
                GroundDisplacementLoad load = new GroundDisplacementLoad();
                load.Tx = float.Parse(readAttribute(node, "U1", "0"));
                load.Ty = float.Parse(readAttribute(node, "U2", "0"));
                load.Tz = float.Parse(readAttribute(node, "U3", "0"));
                load.Rx = model.UnitSystem.Rad2Deg(float.Parse(readAttribute(node, "R1", "0")));
                load.Ry = model.UnitSystem.Rad2Deg(float.Parse(readAttribute(node, "R2", "0")));
                load.Rz = model.UnitSystem.Rad2Deg(float.Parse(readAttribute(node, "R3", "0")));
                loads.Add(load, lCase);
            }
        }

        /// <summary>
        /// Create nodes in XML for frame elements
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="obj"></param>
        private void readFrames(XmlNode node)
        {
            //Dictionary<FrameSection, FrameSection> frameSectionCache = new Dictionary<FrameSection, FrameSection>();
            //// xml.WriteStartElement("Connectivity_-_Frame");
            foreach (XmlNode child in node.ChildNodes)
            {
                if ("Frame".Equals(child.Name))
                    readLines(child);
            }
        }

        private void readFrameSectionAssignments(XmlNode node)
        {
            //// xml.WriteStartElement("Frame_Section_Assignments");
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readLineAssigments(child);
            //foreach (LineElement i in obj)
            //    if (i != null)
            //        writeLineAssigments(xml, i);

        }

        public void readFrameConcreteBeams(XmlNode node, Catalog<Section.Section> cat)
        {
            //// xml.WriteStartElement("Frame_Section_Properties_03_-_Concrete_Beam");
            foreach (XmlNode child in node.ChildNodes)
                if ("SectionName".Equals(child.Name))
                    readConcreteBeamSectionProps(child, cat);
        }

        public void readFrameConcreteColumns(XmlNode node, Catalog<Section.Section> cat)
        {
            //// xml.WriteStartElement("Frame_Section_Properties_02_-_Concrete_Column");
            foreach (XmlNode child in node.ChildNodes)
                if ("SectionName".Equals(child.Name))
                    readConcreteColumnSectionProps(child, cat);
        }

        private void readFramePartialFixities(XmlNode node)
        {
            //// xml.WriteStartElement("Frame_Release_Assignments_2_-_Partial_Fixity");
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readSpringAssignments(child);
        }

        private void readFrameReleases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readReleaseAssignments(child);

        }

        private void readFramePointLoads(XmlNode node)
        {
            //// xml.WriteStartElement("Frame_Loads_-_Point");
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readLinePointForces(child);

        }

        private void readFrameAngles(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readAngleAssignments(child);

        }

        private void readFrameDistributedLoads(XmlNode node)
        {
            //// xml.WriteStartElement("Frame_Loads_-_Distributed");
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readLineDistributedForces(child);
        }

        private void readFrameTemperatureLoads(XmlNode node)
        {
            //// xml.WriteStartElement("Frame_Loads_-_Temperature");
            foreach (XmlNode child in node.ChildNodes)
                if ("Frame".Equals(child.Name))
                    readLineTemperatureLoad(child);
        }

        public void readFrameSections(XmlNode node, Catalog<Section.Section> cat)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("SectionName".Equals(child.Name))
                    readFrameSection(child, cat);

        }

        private void readLines(XmlNode node)
        {
            LineElement line = new LineElement(new StraightFrameProps());

            //Joint i = obj.I;
            //Joint j = obj.J;
            //// xml.WriteStartElement("Frame");
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            int ji = int.Parse(readAttribute(node, "JointI", "0"));
            int jj = int.Parse(readAttribute(node, "JointJ", "0"));
            line.I = model.JointList[ji];
            line.J = model.JointList[jj];
            line.Id = id;
            model.LineList.Add(line);
            //readAttribute(node, "IsCurved", "No");
            //readAttribute(node, "Length", obj.Length.ToString());
            //readAttribute(node, "CentroidX", ((i.X + j.X) / 2.0F).ToString());
            //readAttribute(node, "CentroidY", ((i.Y + j.Y) / 2.0F).ToString());
            //readAttribute(node, "CentroidZ", ((i.Z + j.Z) / 2.0F).ToString());
        }

        private void readLineAssigments(XmlNode node)
        {
            int id = int.Parse(readAttribute(node, "Frame", "0"));
            if (id < model.LineList.Count)
            {
                LineElement e = model.LineList[id];
                string sName = readAttribute(node, "AnalSect", "");
                Section.FrameSection sec = model.Sections[sName] as Section.FrameSection;
                if (e != null && sec != null && e.Properties is StraightFrameProps)
                    ((StraightFrameProps)e.Properties).Section = sec;
                else if (e.Properties is StraightFrameProps)
                    model.Sections[((StraightFrameProps)e.Properties).Section.Name] = ((StraightFrameProps)e.Properties).Section;
            }
        }

        private void readConcreteBeamSectionProps(XmlNode node, Catalog<Section.Section> cat)
        {
            string sName = readAttribute(node, "SectionName", "");
            FrameSection sec = cat[sName] as FrameSection;
            if (sec != null)
            {
                ConcreteBeamSectionProps concreteBeam = sec.ConcreteProperties as ConcreteBeamSectionProps;
                if (concreteBeam == null)
                {
                    concreteBeam = new ConcreteBeamSectionProps();
                    sec.ConcreteProperties = concreteBeam as ConcreteBeamSectionProps;
                }
                concreteBeam.ConcreteCoverTop = float.Parse(readAttribute(node, "TopCover", concreteBeam.ConcreteCoverTop.ToString()));
                concreteBeam.ConcreteCoverTop = float.Parse(readAttribute(node, "BotCover", concreteBeam.ConcreteCoverBottom.ToString()));
                concreteBeam.ConcreteCoverTop = float.Parse(readAttribute(node, "TopLeftArea", concreteBeam.RoTopLeft.ToString()));
                concreteBeam.ConcreteCoverTop = float.Parse(readAttribute(node, "TopRghtArea", concreteBeam.RoTopRight.ToString()));
                concreteBeam.ConcreteCoverTop = float.Parse(readAttribute(node, "BotLeftArea", concreteBeam.RoBottomLeft.ToString()));
                concreteBeam.ConcreteCoverTop = float.Parse(readAttribute(node, "BotRghtArea", concreteBeam.RoBottomRight.ToString()));
            }
        }

        private void readConcreteColumnSectionProps(XmlNode node, Catalog<Section.Section> cat)
        {
            string sName = readAttribute(node, "SectionName", "");
            FrameSection sec = cat[sName] as FrameSection;
            if (sec != null)
            {
                ConcreteColumnSectionProps column = sec.ConcreteProperties as ConcreteColumnSectionProps;
                if (column == null)
                {
                    column = new ConcreteColumnSectionProps();
                    sec.ConcreteProperties = column as ConcreteColumnSectionProps;
                }

                string config = readAttribute(node, "ReinfConfig", column.RConfiguration.ToString());
                string lat = readAttribute(node, "LatReinf", column.LateralR.ToString());
                string cover = readAttribute(node, "Cover", column.CoverToRebarCenter.ToString());
                int n3 = int.Parse(readAttribute(node, "NumBars3Dir", "0"));
                int n2 = int.Parse(readAttribute(node, "NumBars2Dir", "0"));
                int circ = int.Parse(readAttribute(node, "NumBarsCirc", "0"));
                string size = readAttribute(node, "BarSize", column.BarSize.ToString());
                string space = readAttribute(node, "SpacingC", column.SpacingC.ToString());

                column.RConfiguration = (ConcreteColumnSectionProps.ReinforcementConfiguration)Enum.Parse(
                    typeof(ConcreteColumnSectionProps.ReinforcementConfiguration), config);
                column.LateralR = (ConcreteColumnSectionProps.LateralReinforcement)Enum.Parse(
                    typeof(ConcreteColumnSectionProps.LateralReinforcement), lat);
                column.CoverToRebarCenter = float.Parse(cover);
                column.NumberOfBars2Dir = n2;
                column.NumberOfBars3Dir = n3;
                if (column.RConfiguration == ConcreteColumnSectionProps.ReinforcementConfiguration.Circular)
                    column.NumberOfBars = circ;
                else
                {
                    column.NumberOfBars2Dir = n2;
                    column.NumberOfBars3Dir = n3;
                }
                column.BarSize = size;
                column.SpacingC = float.Parse(space);
            }
        }

        private void readReleaseAssignments(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            LineElement obj = model.LineList[id];

            if (obj != null)
            {
                JointDOF dofi = obj.DoFI;
                JointDOF dofj = obj.DoFJ;
                // xml.WriteStartElement("Frame");
                dofi.T1 = (readAttribute(node, "PI", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofi.T2 = (readAttribute(node, "V2I", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofi.T3 = (readAttribute(node, "V3I", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofi.R1 = (readAttribute(node, "TI", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofi.R2 = (readAttribute(node, "M2I", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofi.R3 = (readAttribute(node, "M3I", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofj.T1 = (readAttribute(node, "PJ", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofj.T2 = (readAttribute(node, "V2J", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofj.T3 = (readAttribute(node, "V3J", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofj.R1 = (readAttribute(node, "TJ", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofj.R2 = (readAttribute(node, "M2J", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                dofj.R3 = (readAttribute(node, "M3J", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                //readAttribute(node, "PartialFix", (dofi.IsSpring || dofj.IsSpring) ? "Yes" : "No");

                obj.DoFI = dofi;
                obj.DoFJ = dofj;
            }
        }

        private void readSpringAssignments(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            LineElement obj = model.LineList[id];
            if (obj != null)
            {
                JointDOF dofi = obj.DoFI;
                JointDOF dofj = obj.DoFJ;
                string[] str = new string[] { "PI", "V2I", "V3I", "TI", "M2I", "M3I", "PJ", "V2J", "V3J", "TJ", "M2J", "M3J" };

                // xml.WriteStartElement("Frame");
                float[] dofiValues = new float[6];
                float[] dofjValues = new float[6];

                string val;
                for (int i = 0; i < 6; i++)
                    if (!(val = readAttribute(node, str[i], "0")).Equals("0"))
                        dofiValues[i] = float.Parse(val);
                dofi.SpringValues = dofiValues;
                obj.DoFI = dofi;

                for (int i = 0; i < 6; i++)
                    if (!(val = readAttribute(node, str[i + 6], "0")).Equals("0"))
                        dofjValues[i] = float.Parse(val);
                dofj.SpringValues = dofjValues;
                obj.DoFJ = dofj;
            }
        }

        private void readAngleAssignments(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            LineElement obj = model.LineList[id];

            if (obj != null)
            {
                // xml.WriteStartElement("Frame");
                obj.Angle = float.Parse(readAttribute(node, "Angle", "0"));
                //obj.Angle = (readAttribute(node, "MirrorAbt2", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                //obj.Angle = (readAttribute(node, "MirrorAbt2", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;
                //obj.Angle = (readAttribute(node, "AdvanceAxes", "No").Equals("No")) ? JointDOF.DofType.Restrained : JointDOF.DofType.Free;

            }
        }

        private void readLinePointForces(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            string lcName = readAttribute(node, "LoadCase", "").Trim();
            lcName = (lcName.Length > 0) ? lcName : Culture.Get("Case");
            LoadCase lCase = model.LoadCases[lcName];
            LineElement obj = model.LineList[id];
            if (obj != null && lCase != null)
            {
                AssignedLoads loads = obj.Loads;
                ConcentratedSpanLoad load = new ConcentratedSpanLoad();
                load.Type = (LineLoad.LoadType)Enum.Parse(typeof(LineLoad.LoadType), readAttribute(node, "Type", load.Type.ToString()));
                string coordSys = readAttribute(node, "CoordSys", "GLOBAL");
                string dir = readAttribute(node, "Dir", load.Direction.ToString());
                load.Direction = GetLoadDirection(coordSys, dir);
                load.D = float.Parse(readAttribute(node, "RelDist", load.D.ToString()));
                load.L = float.Parse(readAttribute(node, "Force", load.LoadInt.ToString()));
                loads.Add(load, lCase);
            }
        }

        private void readLineDistributedForces(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            string lcName = readAttribute(node, "LoadCase", "").Trim();
            lcName = (lcName.Length > 0) ? lcName : Culture.Get("Case");
            LoadCase lCase = model.LoadCases[lcName];
            LineElement obj = model.LineList[id];
            if (obj != null && lCase != null)
            {
                AssignedLoads loads = obj.Loads;
                DistributedSpanLoad load = new DistributedSpanLoad();
                load.Type = (LineLoad.LoadType)Enum.Parse(typeof(LineLoad.LoadType), readAttribute(node, "Type", load.Type.ToString()));
                string coordSys = readAttribute(node, "CoordSys", "GLOBAL");
                string dir = readAttribute(node, "Dir", load.Direction.ToString());
                load.Direction = GetLoadDirection(coordSys, dir);
                load.Da = float.Parse(readAttribute(node, "RelDistA", load.Da.ToString()));
                load.Db = float.Parse(readAttribute(node, "RelDistB", load.Db.ToString()));
                load.La = float.Parse(readAttribute(node, "FOverLA", load.La.ToString()));
                load.Lb = float.Parse(readAttribute(node, "FOverLB", load.Lb.ToString()));
                loads.Add(load, lCase);
            }
        }

        private void readLineTemperatureLoad(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Frame", "0"));
            string lcName = readAttribute(node, "LoadCase", "").Trim();
            lcName = (lcName.Length > 0) ? lcName : Culture.Get("Case");
            LoadCase lCase = model.LoadCases[lcName];
            LineElement obj = model.LineList[id];
            if (obj != null && lCase != null)
            {
                AssignedLoads loads = obj.Loads;
                string type = readAttribute(node, "Type", "");
                TemperatureLineLoad load = null;
                switch (type)
                {
                    case "Temperature":
                        load = new TemperatureLineLoad();
                        load.Temperature = float.Parse(readAttribute(node, "Temp", "0"));
                        break;
                    case "Gradient2":
                        load = new TemperatureGradientLineLoad();
                        load.Temperature = float.Parse(readAttribute(node, "TempGrad2", "0"));
                        ((TemperatureGradientLineLoad)load).LoadType = TemperatureGradientLineLoad.GradientDirection.G22;
                        break;
                    case "Gradient3":
                        load = new TemperatureGradientLineLoad();
                        load.Temperature = float.Parse(readAttribute(node, "TempGrad3", "0"));
                        ((TemperatureGradientLineLoad)load).LoadType = TemperatureGradientLineLoad.GradientDirection.G33;
                        break;
                }
                if (load != null)
                    loads.Add(load, lCase);
            }
        }

        private LineLoad.LoadDirection GetLoadDirection(string coordSys, string dir)
        {
            if ("GRAVITY".Equals(dir.ToUpper()))
                return LineLoad.LoadDirection.Gravity;
            return (LineLoad.LoadDirection)Enum.Parse(typeof(LineLoad.LoadDirection), coordSys + dir, true);
        }

        private void readFrameSection(XmlNode node, Catalog<Section.Section> cat)
        {
            string name = readAttribute(node, "SectionName", "");
            string mat = readAttribute(node, "Material", "");
            string shape = readAttribute(node, "Shape", "General");
            float t3 = float.Parse(readAttribute(node, "t3", "0"));
            float t2 = float.Parse(readAttribute(node, "t2", "0"));
            float tf = float.Parse(readAttribute(node, "tf", "0"));
            float tw = float.Parse(readAttribute(node, "tw", "0"));
            float t2b = float.Parse(readAttribute(node, "t2b", "0"));
            float tfb = float.Parse(readAttribute(node, "tfb", "0"));
            float dis = float.Parse(readAttribute(node, "dis", "0"));
            float a = float.Parse(readAttribute(node, "Area", "0"));
            float j = float.Parse(readAttribute(node, "TorsConst", "0"));
            float I33 = float.Parse(readAttribute(node, "I33", "0"));
            float I22 = float.Parse(readAttribute(node, "I22", "0"));
            float AS2 = float.Parse(readAttribute(node, "AS2", "0"));
            float AS3 = float.Parse(readAttribute(node, "AS3", "0"));
            float S33 = float.Parse(readAttribute(node, "S33", "0"));
            float S22 = float.Parse(readAttribute(node, "S22", "0"));
            float Z33 = float.Parse(readAttribute(node, "Z33", "0"));
            float Z22 = float.Parse(readAttribute(node, "Z22", "0"));
            float R33 = float.Parse(readAttribute(node, "R33", "0"));
            float R22 = float.Parse(readAttribute(node, "R22", "0"));

            FrameSection sec = cat[name] as FrameSection;

            Material.Material material = MaterialManager.Instance.Materials[mat];
            if (material == null)
                material = MaterialManager.Instance.DefaultSteel;

            if (material != null)
            {
                if (sec != null)
                {
                    sec.Area = a;
                    sec.As2 = AS2;
                    sec.As3 = AS3;
                    sec.Dis = dis;
                    sec.I22 = I22;
                    sec.I33 = I33;
                    sec.Material = material;
                    sec.R22 = R22;
                    sec.R33 = R33;
                    sec.S22 = S22;
                    sec.S33 = S33;
                    sec.T2 = t2;
                    sec.T2b = t2b;
                    sec.T3 = t3;
                    sec.Tf = tf;
                    sec.Tfb = tfb;
                    sec.TorsConst = j;
                    sec.Tw = tw;
                    sec.Z22 = Z22;
                    sec.Z33 = Z33;
                }
                else
                {
                    model.Sections[name] = null;
                    switch (shape)
                    {
                        case "Double Angle":
                            model.Sections[name] = new Section.DoubleAngle(name, "2L", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Channel":
                            model.Sections[name] = new Section.Channel(name, "C", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "I/Wide Flange":
                            model.Sections[name] = new Section.IWideFlange(name, "I", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Box/Tube":
                            model.Sections[name] = new Section.BoxTube(name, "B", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Pipe":
                            model.Sections[name] = new Section.Pipe(name, "P", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Angle":
                            model.Sections[name] = new Section.Angle(name, "L", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Tee":
                            model.Sections[name] = new Section.Tee(name, "T", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Circle":
                            model.Sections[name] = new Section.Circle(name, "RN", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        case "Rectangular":
                            model.Sections[name] = new Section.Rectangular(name, "R", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                        default:
                            model.Sections[name] = new Section.General(name, "G", material, null, t3, t2, tf, tw, t2b, tfb, dis, a, j, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void readMaterials(XmlNode node)
        {
            MaterialManager.Instance.Initialize(node);
        }

        private void readConcreteMaterials(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Material".Equals(child.Name))
                    readConcreteDesignProps(child);
        }

        private void readRebarMaterials(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Material".Equals(child.Name))
                    readRebarDesignProps(child);
        }

        private void readAluminumMaterials(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Material".Equals(child.Name))
                    readAluminumDesignProps(child);
        }

        private void readColdFormedMaterials(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Material".Equals(child.Name))
                    readColdFormedDesignProps(child);
        }

        private void readSteelMaterials(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Material".Equals(child.Name))
                    readSteelDesignProps(child);
        }

        private void readConcreteDesignProps(XmlNode node)
        {
            string name = readAttribute(node, "Material", "");
            Material.Material mat = MaterialManager.Instance.Materials[name];
            if (mat != null)
            {
                float fc = float.Parse(readAttribute(node, "Fc", "0"));
                float fy = float.Parse(readAttribute(node, "RebarFy", "0"));
                float fys = float.Parse(readAttribute(node, "RebarFys", "0"));
                bool isLw = "Yes".Equals(readAttribute(node, "LtWtConc", "No"));
                float lwf = float.Parse(readAttribute(node, "LtWtFact", "0"));
                mat.DesignProperties = new ConcreteDesignProps(fc, fy, fys, isLw, lwf);
            }
        }

        private void readRebarDesignProps(XmlNode node)
        {
            Material.Material mat = MaterialManager.Instance.Materials[readAttribute(node, "Material", "")];
            if (mat != null)
            {
                float fy = float.Parse(readAttribute(node, "Fy", "0"));
                float fu = float.Parse(readAttribute(node, "Fu", "0"));
                SteelDesignProps props = mat.DesignProperties as SteelDesignProps;
                if (props == null)
                {
                    props = new SteelDesignProps(fy, fu);
                    mat.DesignProperties = props;
                }
                props.Fy = fy;
                props.Fu = fu;
                mat.DesignProperties = props;
            }
        }

        private void readAluminumDesignProps(XmlNode node)
        {
            //MaterialDesignProps dProps = mat.DesignProperties;
            //if (dProps is AluminumDesignProps)
            //{
            //    AluminumDesignProps props = (AluminumDesignProps)dProps;
            //    // xml.WriteStartElement("Material");
            //    readAttribute(node, "Material", mat.Name);
            //    readAttribute(node, "AlumType", props.Type.ToString());
            //    readAttribute(node, "Alloy", props.Alloy.ToString());
            //    readAttribute(node, "Ftu", props.Ftu.ToString());
            //    readAttribute(node, "Fty", props.Fty.ToString());
            //    readAttribute(node, "Fcy", props.Fcy.ToString());
            //    readAttribute(node, "Fsu", props.Fsu.ToString());
            //    readAttribute(node, "Fsy", props.Fsy.ToString());

            //}
        }

        private void readColdFormedDesignProps(XmlNode node)
        {
            //MaterialDesignProps dProps = mat.DesignProperties;
            //if (dProps is ColdFormedDesignProps)
            //{
            //    ColdFormedDesignProps props = (ColdFormedDesignProps)dProps;
            //    // xml.WriteStartElement("Material");
            //    readAttribute(node, "Material", mat.Name);
            //    readAttribute(node, "Fy", props.Fy.ToString());
            //    readAttribute(node, "Fu", props.Fu.ToString());

            //}
        }

        private void readSteelDesignProps(XmlNode node)
        {
            Material.Material mat = MaterialManager.Instance.Materials[readAttribute(node, "Material", "")];
            if (mat != null)
            {
                float fy = float.Parse(readAttribute(node, "Fy", "0"));
                float fu = float.Parse(readAttribute(node, "Fu", "0"));
                SteelDesignProps props = mat.DesignProperties as SteelDesignProps;
                if (props == null)
                {
                    props = new SteelDesignProps(fy, fu);
                    mat.DesignProperties = props;
                }
                props.Fy = fy;
                props.Fu = fu;
                mat.DesignProperties = props;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="m"></param>
        private void readAnalysisCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Case".Equals(child.Name))
                    readAnalysisCase(child);
            //        readAnalysisCase(node);
        }

        private void readStaticCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Case".Equals(child.Name))
                    readStaticCase(child);
        }

        private void readModalCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Case".Equals(child.Name))
                    readModalGeneralCase(child);
        }

        private void readModalRitzCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Case".Equals(child.Name))
                    readModalCase(child);
        }

        private void readResponseSpectrumGeneralCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Case".Equals(child.Name))
                    readResponseSpectrumGeneralCase(child);
        }

        private void readResponseSpectrumAssignmentsCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Case".Equals(child.Name))
                    readResponseSpectrumLoadCase(child);
        }

        private void readCombinationsCases(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Combo".Equals(child.Name))
                    readLoadCombinationCase(child);
        }

        private void readAnalysisCase(XmlNode node)
        {
            string name = readAttribute(node, "Case", "").Trim();
            name = (name.Length > 0) ? name : Culture.Get("Case");
            string type = readAttribute(node, "Type", "LinStatic");
            string modal = readAttribute(node, "ModalCase", " ");
            bool active = readAttribute(node, "RunCase", "Yes").Equals("Yes");
            AnalysisCaseProps props;

            switch (type)
            {
                case "LinRespSpec":
                    props = new ResponseSpectrumCaseProps();
                    AnalysisCase modalCase;
                    foreach (AbstractCase ac in model.AbstractCases)
                        if (ac.Name.Equals(modal))
                        {
                            ((ResponseSpectrumCaseProps)props).ModalAnalysisCase = (AnalysisCase)ac;
                            break;
                        }
                    break;
                case "LinModal":
                    props = new ModalCaseProps();
                    break;
                case "NonStatic":
                    props = new PDeltaCaseProps();
                    ((PDeltaCaseProps)props).Loads = new List<StaticCaseFactor>();
                    break;
                default:
                    props = new StaticCaseProps();
                    break;
            }
            AnalysisCase aCase = new AnalysisCase(name, props);
            aCase.IsActive = active;
            model.AbstractCases.Add(aCase);
        }

        private void readStaticCase(XmlNode node)
        {
            AnalysisCase aCase = null;
            string name = readAttribute(node, "Case", "").Trim();
            name = (name.Length > 0) ? name : Culture.Get("Case");
            foreach (AbstractCase ac in model.AbstractCases)
                if (ac.Name.Equals(name))
                {
                    aCase = ac as AnalysisCase;
                    break;
                }
            AnalysisCaseProps props = aCase.Properties;
            if (props is StaticCaseProps)
            {
                StaticCaseProps scprops = (StaticCaseProps)props;

                string lType = readAttribute(node, "LoadType", "Load Case");
                string lName = readAttribute(node, "LoadName", "").Trim();
                lName = (lName.Length > 0) ? lName : Culture.Get("Case");
                string sFact = readAttribute(node, "LoadSF", "0");

                AnalysisCaseAppliedLoad appLoad = null;
                if ("Load case".Equals(lType))
                    appLoad = model.LoadCases[lName];
                else
                    appLoad = new AccelLoad((AccelLoad.AccelLoadValues)Enum.Parse(typeof(AccelLoad.AccelLoadValues), lName));
                if (appLoad != null)
                {
                    StaticCaseFactor factor = new StaticCaseFactor(appLoad, float.Parse(sFact));
                    List<StaticCaseFactor> list = scprops.Loads;
                    list.Add(factor);
                    scprops.Loads = list;
                }
            }
        }

        private void readModalGeneralCase(XmlNode node)
        {
            string name = readAttribute(node, "Case", "").Trim();
            name = (name.Length > 0) ? name : Culture.Get("Case");
            AnalysisCase aCase = null;
            foreach (AnalysisCase ac in model.AbstractCases)
                if (name.Equals(ac.Name))
                {
                    aCase = ac;
                    break;
                }

            if (aCase != null)
            {
                ModalCaseProps mcp = aCase.Properties as ModalCaseProps;
                if (mcp == null)
                {
                    mcp = new ModalCaseProps();
                    aCase.Properties = mcp;
                }
                // xml.WriteStartElement("Case");
                mcp.ModesType = (ModalCaseProps.ModesMethod)Enum.Parse(typeof(ModalCaseProps.ModesMethod),
                    readAttribute(node, "ModeType", mcp.ModesType.ToString()));
                mcp.MaxModes = uint.Parse(readAttribute(node, "MaxNumModes", mcp.MaxModes.ToString()));
                mcp.MinModes = uint.Parse(readAttribute(node, "MinNumModes", mcp.MinModes.ToString()));
            }
        }

        private void readModalCase(XmlNode node)
        {
            string name = readAttribute(node, "Case", "").Trim();
            name = (name.Length > 0) ? name : Culture.Get("Case");

            AnalysisCase aCase = null;
            foreach (AnalysisCase ac in model.AbstractCases)
                if (name.Equals(ac.Name))
                {
                    aCase = ac;
                    break;
                }

            if (aCase != null)
            {
                ModalCaseProps mcp = aCase.Properties as ModalCaseProps;
                if (mcp == null)
                {
                    mcp = new ModalCaseProps();
                    aCase.Properties = mcp;
                }
                mcp.ModesType = ModalCaseProps.ModesMethod.RitzVectors;

                List<ModalCaseFactor> list = mcp.Loads;

                bool isAccel = readAttribute(node, "LoadType", "Accel").Equals("Accel"); // else "Load Case"
                AnalysisCaseAppliedLoad load = null;
                if (isAccel)
                {
                    string tmp = readAttribute(node, "LoadName", "Accel ").Substring(6);
                    load = new AccelLoad((AccelLoad.AccelLoadValues)Enum.Parse(typeof(AccelLoad.AccelLoadValues), tmp));
                }
                else
                {
                    load = model.LoadCases[readAttribute(node, "LoadName", ((LoadCase)load).Name)];
                }
                ModalCaseFactor f = Contains(list, load);
                if (f == null)
                {
                    f = new ModalCaseFactor(load);
                    list.Add(f);
                }
                f.Cycles = int.Parse(readAttribute(node, "MaxCycles", f.Cycles.ToString()));
                f.Ratio = float.Parse(readAttribute(node, "TargetPar", f.Ratio.ToString()));
                mcp.Loads = list;
            }
        }

        /// <summary>
        /// Ignore this. It's constant.
        /// </summary>
        /// <param name="node"></param>
        private void readNonLinearLoadApplication(XmlNode node)
        {
            string name = readAttribute(node, "Case", "");
            //readAttribute("LoadApp", "Full Load");
            //readAttribute("MonitorDOF", "U1");
        }

        private void readNonLinearParameters(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if ("Case".Equals(child.Name))
                {
                    string name = readAttribute(node, "Case", "").Trim();
                    name = (name.Length > 0) ? name : Culture.Get("Case");

                    AnalysisCase aCase = null;
                    foreach (AbstractCase ac in model.AbstractCases)
                        if (ac is AnalysisCase && name.Equals(ac.Name))
                        {
                            aCase = (AnalysisCase)ac;
                            break;
                        }

                    if (aCase != null)
                    {
                        if (!(aCase.Properties is PDeltaCaseProps))
                            aCase.Properties = new PDeltaCaseProps();
                        NonLinearParams nl = ((PDeltaCaseProps)aCase.Properties).NonLinearParams;
                        nl.Unloading = readAttribute(child, "Unloading", nl.Unloading);
                        nl.GeoNonLin = readAttribute(child, "GeoNonLin", nl.GeoNonLin);
                        nl.ResultsSave = readAttribute(child, "ResultsSave", nl.ResultsSave);
                        nl.MaxTotal = int.Parse(readAttribute(child, "MaxTotal", nl.MaxTotal.ToString()));
                        nl.MaxNull = int.Parse(readAttribute(child, "MaxNull", nl.MaxNull.ToString()));
                        nl.MaxIterCS = int.Parse(readAttribute(child, "MaxIterCS", nl.MaxIterCS.ToString()));
                        nl.MaxIterNR = int.Parse(readAttribute(child, "MaxIterNR", nl.MaxIterNR.ToString()));
                        nl.ItConvTol = float.Parse(readAttribute(child, "ItConvTol", nl.ItConvTol.ToString()));
                        nl.UseEvStep = readAttribute(child, "UseEvStep", CodeYN(nl.UseEvStep)).Equals("Yes");
                        nl.EvLumpTol = float.Parse(readAttribute(child, "EvLumpTol", nl.EvLumpTol.ToString()));
                        nl.LSPerIter = int.Parse(readAttribute(child, "LSPerIter", nl.LSPerIter.ToString()));
                        nl.LSTol = float.Parse(readAttribute(child, "LSTol", nl.LSTol.ToString()));
                        nl.LSStepFact = float.Parse(readAttribute(child, "LSStepFact", nl.LSStepFact.ToString()));
                        nl.FrameTC = readAttribute(child, "FrameTC", CodeYN(nl.FrameTC)).Equals("Yes");
                        nl.FrameHinge = readAttribute(child, "FrameHinge", CodeYN(nl.FrameHinge)).Equals("Yes");
                        nl.CableTC = readAttribute(child, "CableTC", CodeYN(nl.CableTC)).Equals("Yes");
                        nl.LinkTC = readAttribute(child, "LinkTC", CodeYN(nl.LinkTC)).Equals("Yes");
                        nl.LinkOther = readAttribute(child, "LinkOther", CodeYN(nl.LinkOther)).Equals("Yes");
                        nl.TFMaxIter = int.Parse(readAttribute(child, "TFMaxIter", nl.TFMaxIter.ToString()));
                        nl.TFTol = float.Parse(readAttribute(child, "TFTol", nl.TFTol.ToString()));
                        nl.TFAccelFact = float.Parse(readAttribute(child, "TFAccelFact", nl.TFAccelFact.ToString()));
                        nl.TFNoStop = readAttribute(child, "TFNoStop", CodeYN(nl.TFNoStop)).Equals("Yes");
                    }
                }
            }
        }

        private static ModalCaseFactor Contains(IList<ModalCaseFactor> list, AnalysisCaseAppliedLoad load)
        {
            foreach (ModalCaseFactor mcf in list)
                if (mcf.AppliedLoad.Equals(load))
                    return mcf;
            return null;
        }

        private void readResponseSpectrumGeneralCase(XmlNode node)
        {
            string name = readAttribute(node, "Case", "");
            AnalysisCase aCase = null;
            foreach (AnalysisCase ac in model.AbstractCases)
                if (name.Equals(ac.Name))
                {
                    aCase = ac;
                    break;
                }

            if (aCase != null)
            {
                ResponseSpectrumCaseProps rsp = aCase.Properties as ResponseSpectrumCaseProps;
                if (rsp == null)
                {
                    rsp = new ResponseSpectrumCaseProps();
                    aCase.Properties = rsp;
                }
                string modalCombo = readAttribute(node, "ModalCombo", rsp.ModalCombination.ToString());
                modalCombo = (modalCombo.Length > 0) ? modalCombo : Culture.Get("Case");
                string dirCombo = readAttribute(node, "DirCombo", rsp.DirectionalCombination.ToString());
                dirCombo = (dirCombo.Length > 0) ? dirCombo : Culture.Get("Case");

                rsp.ModalDamping = float.Parse(readAttribute(node, "ConstDamp", rsp.ModalDamping.ToString()));
                rsp.ModalCombination = (ResponseSpectrumCaseProps.ModalCombinationType)Enum.Parse(
                    typeof(ResponseSpectrumCaseProps.ModalCombinationType), modalCombo);
                rsp.DirectionalCombination = (ResponseSpectrumCaseProps.DirectionalCombinationType)Enum.Parse(
                    typeof(ResponseSpectrumCaseProps.DirectionalCombinationType), dirCombo);
            }
        }

        private void readResponseSpectrumLoadCase(XmlNode node)
        {
            string name = readAttribute(node, "Case", "").Trim();
            name = (name.Length > 0) ? name : Culture.Get("Case");
            AnalysisCase aCase = null;
            foreach (AnalysisCase ac in model.AbstractCases)
                if (name.Equals(ac.Name))
                {
                    aCase = ac;
                    break;
                }
            if (aCase != null)
            {
                ResponseSpectrumCaseProps rsp = aCase.Properties as ResponseSpectrumCaseProps;
                if (rsp == null)
                {
                    rsp = new ResponseSpectrumCaseProps();
                    aCase.Properties = rsp;
                }
                List<ResponseSpectrumCaseFactor> list = rsp.Loads;

                //readAttribute(node, "LoadType", "Acceleration");
                AccelLoad load = new AccelLoad(decodeAccel(readAttribute(node, "LoadName", "Accel U1")));
                bool contains = false;
                foreach (ResponseSpectrumCaseFactor rscf in list)
                    if (rscf.Accel.Equals(load))
                        contains = true;
                if (!contains)
                {
                    list.Add(new ResponseSpectrumCaseFactor(load));
                    rsp.Loads = list;
                }
                //readAttribute(node, "CoordSys", "GLOBAL");

                float factor = float.Parse(readAttribute(node, "TransAccSF", "1"));
                string spectrum = readAttribute(node, "Function", rsp.ResponseSpectrumFunction.ToString());
                foreach (ResponseSpectrum rs in model.ResponseSpectra)
                    if (spectrum.Equals(rs.ToString()))
                    {
                        rsp.ResponseSpectrumFunction = rs;
                        rsp.ScaleFactor = factor;
                        break;
                    }

                //readAttribute(node, "Angle", "0");

            }
        }

        private void readLoadCombinationCase(XmlNode node)
        {
            string name = readAttribute(node, "ComboName", "");
            name = (name.Length > 0) ? name : Culture.Get("Case");
            string type = readAttribute(node, "ComboType", "");
            string caseType = readAttribute(node, "CaseType", "Response Spectrum");
            string caseName = readAttribute(node, "CaseName", "");
            string factor = readAttribute(node, "ScaleFactor", "0");
            bool steel = readAttribute(node, "SteelDesign", "No").Equals("Yes");
            bool conc = readAttribute(node, "ConcDesign", "No").Equals("Yes");
            bool alum = readAttribute(node, "AlumDesign", "No").Equals("Yes");
            bool cold = readAttribute(node, "ColdDesign", "No").Equals("Yes");

            LoadCombination combo = null;
            AbstractCase aCase = null;
            foreach (AbstractCase ac in model.AbstractCases)
            {
                if (ac.Name.Equals(name) && ac is LoadCombination)
                    combo = (LoadCombination)ac;
                else if (ac.Name.Equals(caseName))
                    aCase = ac;
            }
            if (combo == null)
            {
                combo = new LoadCombination(name);
                model.AbstractCases.Add(combo);
            }
            if (aCase != null)
                combo.Cases.Add(new AbstractCaseFactor(aCase, float.Parse(factor)));

            if (steel && !model.SteelDesignOptions.DesignCombinations.Contains(combo))
                model.SteelDesignOptions.DesignCombinations.Add(combo);
            if (conc && !model.ConcreteDesignOptions.DesignCombinations.Contains(combo))
                model.ConcreteDesignOptions.DesignCombinations.Add(combo);
            if (alum && !model.AluminumDesignOptions.DesignCombinations.Contains(combo))
                model.AluminumDesignOptions.DesignCombinations.Add(combo);
            if (cold && !model.ColdFormedDesignOptions.DesignCombinations.Contains(combo))
                model.ColdFormedDesignOptions.DesignCombinations.Add(combo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void readLoadCases(XmlNode node)
        {
            //            xml.WriteStartElement("Load_Case_Definitions");

            foreach (XmlNode child in node.ChildNodes)
                if ("LoadCase".Equals(child.Name))
                    readLoadCase(child);
        }

        private void readLoadCase(XmlNode node)
        {
            string name = readAttribute(node, "LoadCase", "").Trim();
            name = (name.Length > 0) ? name : Culture.Get("Case");
            string desType = readAttribute(node, "DesignType", "");
            // Exception. Has a blank.
            desType = ("REDUCIBLE LIVE".Equals(desType)) ? LoadCase.LoadCaseType.ReduceLive.ToString() : desType;

            float sw = float.Parse(readAttribute(node, "SelfWtMult", "0"));
            string al = readAttribute(node, "AutoLoad", "");
            LoadCase lCase = new LoadCase(name, (LoadCase.LoadCaseType)Enum.Parse(typeof(LoadCase.LoadCaseType), desType, true));
            lCase.AutoLoad = al;
            lCase.SelfWeight = sw;
            model.LoadCases[name] = lCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="obj"></param>
        private void readConcreteDesign(XmlNode node)
        {
            switch (node.Name)
            {
                case "Preferences_-_Concrete_Design_-_Mexican_RCDF_2001":
                    readRCDF2001(node);
                    break;
                case "Preferences_-_Concrete_Design_-_ACI_318-02":
                    readACI318_02(node);
                    break;
                case "Preferences_-_Concrete_Design_-_UBC97":
                    readUBC97_Conc(node);
                    break;
            }

        }

        private void readRCDF2001(XmlNode node)
        {
            RCDF2001 obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is RCDF2001)
                    obj = (RCDF2001)opt;
            if (obj != null)
            {
                obj.THDesign = GetTHDesign(readAttribute(node, "Name", obj.GetTHDesignName(obj.THDesign)));
                obj.NumCurves = uint.Parse(readAttribute(node, "NumCurves", obj.NumCurves.ToString()));
                obj.NumPoints = uint.Parse(readAttribute(node, "NumPoints", obj.NumPoints.ToString()));
                obj.MinEccen = readAttribute(node, "MinEccen", "No").Equals("Yes");
                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.UFLimit = float.Parse(readAttribute(node, "UFLimit", obj.UFLimit.ToString()));
                obj.PhiB = float.Parse(readAttribute(node, "PhiB", obj.PhiB.ToString()));
                obj.PhiT = float.Parse(readAttribute(node, "PhiT", obj.PhiT.ToString()));
                obj.PhiCTied = float.Parse(readAttribute(node, "PhiCTied", obj.PhiCTied.ToString()));
                obj.PhiCSpiral = float.Parse(readAttribute(node, "PhiCSpiral", obj.PhiCSpiral.ToString()));
                obj.PhiV = float.Parse(readAttribute(node, "PhiV", obj.PhiV.ToString()));
                model.ConcreteDesignOptions = obj;
            }
        }

        private void readACI318_02(XmlNode node)
        {
            ACI318_02 obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is ACI318_02)
                    obj = (ACI318_02)opt;
            if (obj != null)
            {
                obj.THDesign = GetTHDesign(readAttribute(node, "Name", obj.GetTHDesignName(obj.THDesign)));
                obj.NumCurves = uint.Parse(readAttribute(node, "NumCurves", obj.NumCurves.ToString()));
                obj.NumPoints = uint.Parse(readAttribute(node, "NumPoints", obj.NumPoints.ToString()));
                obj.MinEccen = readAttribute(node, "MinEccen", CodeYN(obj.MinEccen)).Equals("Yes");
                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.UFLimit = float.Parse(readAttribute(node, "UFLimit", obj.UFLimit.ToString()));
                obj.SeisCat = char.Parse(readAttribute(node, "SeisCat", obj.SeisCat.ToString()));
                obj.PhiT = float.Parse(readAttribute(node, "PhiT", obj.PhiT.ToString()));
                obj.PhiCTied = float.Parse(readAttribute(node, "PhiCTied", obj.PhiCTied.ToString()));
                obj.PhiCSpiral = float.Parse(readAttribute(node, "PhiCSpiral", obj.PhiCSpiral.ToString()));
                obj.PhiV = float.Parse(readAttribute(node, "PhiV", obj.PhiV.ToString()));
                obj.PhiVSeismic = float.Parse(readAttribute(node, "PhiVSeismic", obj.PhiVSeismic.ToString()));
                obj.PhiVJoint = float.Parse(readAttribute(node, "PhiVJoint", obj.PhiVJoint.ToString()));
                model.ConcreteDesignOptions = obj;
            }

            //concreteCode = "ACI 318-02";
        }

        private void readUBC97_Conc(XmlNode node)
        {
            UBC97_Conc obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is UBC97_Conc)
                    obj = (UBC97_Conc)opt;
            if (obj != null)
            {
                obj.THDesign = GetTHDesign(readAttribute(node, "Name", obj.GetTHDesignName(obj.THDesign)));
                obj.NumCurves = uint.Parse(readAttribute(node, "NumCurves", obj.NumCurves.ToString()));
                obj.NumPoints = uint.Parse(readAttribute(node, "NumPoints", obj.NumPoints.ToString()));
                obj.MinEccen = readAttribute(node, "MinEccen", CodeYN(obj.MinEccen)).Equals("Yes");
                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.UFLimit = float.Parse(readAttribute(node, "UFLimit", obj.UFLimit.ToString()));
                obj.PhiB = float.Parse(readAttribute(node, "PhiB", obj.PhiB.ToString()));
                obj.PhiCTied = float.Parse(readAttribute(node, "PhiCTied", obj.PhiCTied.ToString()));
                obj.PhiCSpiral = float.Parse(readAttribute(node, "PhiCSpiral", obj.PhiCSpiral.ToString()));
                obj.PhiV = float.Parse(readAttribute(node, "PhiV", obj.PhiV.ToString()));
                model.ConcreteDesignOptions = obj;
            }
        }

        private void readLRFD99(XmlNode node)
        {
            LRFD99 obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is LRFD99)
                    obj = (LRFD99)opt;
            if (obj != null)
            {
                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.SRatioLimit = float.Parse(readAttribute(node, "SRatioLimit", obj.SRatioLimit.ToString()));
                obj.MaxIter = uint.Parse(readAttribute(node, "MaxIter", obj.MaxIter.ToString()));
                obj.PhiB = float.Parse(readAttribute(node, "PhiB", obj.PhiB.ToString()));
                obj.PhiC = float.Parse(readAttribute(node, "PhiC", obj.PhiC.ToString()));
                obj.PhiTY = float.Parse(readAttribute(node, "PhiTY", obj.PhiTY.ToString()));
                obj.PhiV = float.Parse(readAttribute(node, "PhiV", obj.PhiV.ToString()));
                obj.PhiTF = float.Parse(readAttribute(node, "PhiTF", obj.PhiTF.ToString()));
                obj.PhiVT = float.Parse(readAttribute(node, "PhiVT", obj.PhiVT.ToString()));
                obj.PhiCA = float.Parse(readAttribute(node, "PhiCA", obj.PhiCA.ToString()));
                obj.CheckDefl = readAttribute(node, "CheckDefl", CodeYN(obj.CheckDefl)).Equals("Yes");
                obj.DLRat = float.Parse(readAttribute(node, "DLRat", obj.DLRat.ToString()));
                obj.SDLAndLLRat = float.Parse(readAttribute(node, "SDLAndLLRat", obj.SDLAndLLRat.ToString()));
                obj.LLRat = float.Parse(readAttribute(node, "LLRat", obj.LLRat.ToString()));
                obj.TotalRat = float.Parse(readAttribute(node, "TotalRat", obj.TotalRat.ToString()));
                obj.NetRat = float.Parse(readAttribute(node, "NetRat", obj.NetRat.ToString()));
                obj.SeisCat = char.Parse(readAttribute(node, "SeisCat", obj.SeisCat.ToString()));
                obj.SeisCode = readAttribute(node, "SeisCode", CodeYN(obj.SeisCode)).Equals("Yes");
                obj.SeisLoad = readAttribute(node, "SeisLoad", CodeYN(obj.SeisLoad)).Equals("Yes");
                obj.PlugWeld = readAttribute(node, "PlugWeld", CodeYN(obj.PlugWeld)).Equals("Yes");
                model.SteelDesignOptions = obj;
            }
        }

        private void readUBC97_ASD(XmlNode node)
        {
            UBC97_ASD obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is UBC97_ASD)
                    obj = (UBC97_ASD)opt;
            if (obj != null)
            {
                string zone = obj.GetSeismicZoneName(obj.SeisZone);

                // obj.THDesign = GetTHDesign(readAttribute(node, "Name", obj.GetTHDesignName(obj.THDesign)));
                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.SRatioLimit = float.Parse(readAttribute(node, "SRatioLimit", obj.SRatioLimit.ToString()));
                obj.MaxIter = uint.Parse(readAttribute(node, "MaxIter", obj.MaxIter.ToString()));
                obj.SeisZone = GetSeismicZone(readAttribute(node, "SeisZone", zone));
                obj.LatFactor = float.Parse(readAttribute(node, "LatFactor", obj.LatFactor.ToString()));
                obj.CheckDefl = readAttribute(node, "CheckDefl", obj.CheckDefl.ToString()).Equals("Yes");
                obj.DLRat = float.Parse(readAttribute(node, "DLRat", obj.DLRat.ToString()));
                obj.SDLAndLLRat = float.Parse(readAttribute(node, "SDLAndLLRat", obj.SDLAndLLRat.ToString()));
                obj.LLRat = float.Parse(readAttribute(node, "LLRat", obj.LLRat.ToString()));
                obj.TotalRat = float.Parse(readAttribute(node, "TotalRat", obj.TotalRat.ToString()));
                obj.NetRat = float.Parse(readAttribute(node, "NetRat", obj.NetRat.ToString()));
                model.SteelDesignOptions = obj;
            }
        }

        private void readUBC97_LRFD(XmlNode node)
        {
            UBC97_LRFD obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is UBC97_LRFD)
                    obj = (UBC97_LRFD)opt;
            if (obj != null)
            {
                string zone = obj.GetSeismicZoneName(obj.SeisZone);

                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.SRatioLimit = float.Parse(readAttribute(node, "SRatioLimit", obj.SRatioLimit.ToString()));
                obj.MaxIter = uint.Parse(readAttribute(node, "MaxIter", obj.MaxIter.ToString()));
                obj.PhiB = float.Parse(readAttribute(node, "PhiB", obj.PhiB.ToString()));
                obj.PhiC = float.Parse(readAttribute(node, "PhiC", obj.PhiC.ToString()));
                obj.PhiT = float.Parse(readAttribute(node, "PhiT", obj.PhiT.ToString()));
                obj.PhiV = float.Parse(readAttribute(node, "PhiV", obj.PhiV.ToString()));
                obj.PhiCA = float.Parse(readAttribute(node, "PhiCA", obj.PhiCA.ToString()));
                obj.CheckDefl = readAttribute(node, "CheckDefl", CodeYN(obj.CheckDefl)).Equals("Yes");
                obj.DLRat = float.Parse(readAttribute(node, "DLRat", obj.DLRat.ToString()));
                obj.SDLAndLLRat = float.Parse(readAttribute(node, "SDLAndLLRat", obj.SDLAndLLRat.ToString()));
                obj.LLRat = float.Parse(readAttribute(node, "LLRat", obj.LLRat.ToString()));
                obj.TotalRat = float.Parse(readAttribute(node, "TotalRat", obj.TotalRat.ToString()));
                obj.NetRat = float.Parse(readAttribute(node, "NetRat", obj.NetRat.ToString()));
                obj.SeisZone = GetSeismicZone(readAttribute(node, "SeisZone", zone));
                obj.ImpFactor = float.Parse(readAttribute(node, "ImpFactor", obj.ImpFactor.ToString()));
                model.SteelDesignOptions = obj;
            }
        }

        private void readASD01(XmlNode node)
        {
            ASD01 obj = null;

            foreach (DesignOptions opt in model.DesignOptions)
                if (opt is ASD01)
                    obj = (ASD01)opt;
            if (obj != null)
            {
                obj.PatLLF = float.Parse(readAttribute(node, "PatLLF", obj.PatLLF.ToString()));
                obj.SRatioLimit = float.Parse(readAttribute(node, "SRatioLimit", obj.SRatioLimit.ToString()));
                obj.MaxIter = uint.Parse(readAttribute(node, "MaxIter", obj.MaxIter.ToString()));
                obj.SeisCat = char.Parse(readAttribute(node, "SeisCat", obj.SeisCat.ToString()));
                obj.SeisCode = readAttribute(node, "SeisCode", CodeYN(obj.SeisCode)).Equals("Yes");
                obj.SeisLoad = readAttribute(node, "SeisLoad", CodeYN(obj.SeisLoad)).Equals("Yes");
                obj.PlugWeld = readAttribute(node, "PlugWeld", CodeYN(obj.PlugWeld)).Equals("Yes");
                obj.CheckDefl = readAttribute(node, "CheckDefl", CodeYN(obj.CheckDefl)).Equals("Yes");
                obj.DLRat = float.Parse(readAttribute(node, "DLRat", obj.DLRat.ToString()));
                obj.SDLAndLLRat = float.Parse(readAttribute(node, "SDLAndLLRat", obj.SDLAndLLRat.ToString()));
                obj.LLRat = float.Parse(readAttribute(node, "LLRat", obj.LLRat.ToString()));
                obj.TotalRat = float.Parse(readAttribute(node, "TotalRat", obj.TotalRat.ToString()));
                obj.NetRat = float.Parse(readAttribute(node, "NetRat", obj.NetRat.ToString()));
                model.SteelDesignOptions = obj;
            }
        }

        private void readProgramControl(XmlNode node)
        {
            string steel = readAttribute(node, "SteelCode", "AISC-LRFD99");
            string conc = readAttribute(node, "ConcCode", "ACI 318-02");
            string alum = readAttribute(node, "AlumCode", "AA-ASD 2000");
            string cold = readAttribute(node, "ColdCode", "AISI-ASD96");
            //foreach (DesignOptions code in model.DesignOptions)
            //{
            //    if (steel.Equals(code.ToString()))
            //        model.SteelDesignOptions = code;
            //    if (conc.Equals(code.ToString()))
            //        model.ConcreteDesignOptions = code;
            //    if (alum.Equals(code.ToString()))
            //        model.AluminumDesignOptions = code;
            //    if (cold.Equals(code.ToString()))
            //        model.ColdFormedDesignOptions = code;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void readResponseSpecta(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("RS".Equals(child.Name))
                    readResponseSpectrum(child);
        }

        private void readResponseSpectrum(XmlNode node)
        {
            string name = readAttribute(node, "Name", "RS");
            float[,] func = null;
            foreach (ResponseSpectrum rs in model.ResponseSpectra)
                if (rs != null && rs.ToString().Equals(name))
                    func = rs.Function;

            // TODO: Guardar bien func
            //readAttribute(node, "Period", func[0, 0].ToString());
            //readAttribute(node, "Accel", func[0, 1].ToString());
            //readAttribute(node, "FuncDamp", "0.05");
        }

        private void readFrameInsertionPointAssignments(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                uint id = uint.Parse(readAttribute(child, "Frame", "0"));
                LineElement frame = model.LineList[id];
                if (frame != null)
                {
                    string pt = readAttribute(child, "CardinalPt", "10");
                    int num;
                    if (int.TryParse(pt.Substring(0, 2).Trim(), out num))
                        frame.CardinalPoint = (CardinalPoint)num;
                }
            }
        }

        private void readFrameOffsetAssignments(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                uint id = uint.Parse(readAttribute(child, "Frame", "0"));
                LineElement frame = model.LineList[id];
                if (frame != null)
                {
                    float offi = float.Parse(readAttribute(child, "LengthI", "0"));
                    float offj = float.Parse(readAttribute(child, "LengthJ", "0"));
                    float fact = float.Parse(readAttribute(child, "RigidFactor", "0"));
                    if (offi != 0 || offj != 0 || fact != 0)
                        frame.EndOffsets = new LineEndOffsets(offi, offj, fact);
                }
            }
        }

        private void readLayers(XmlNode node)
        {
            bool setActive = true;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.Equals("Layer"))
                {
                    uint lid = uint.Parse(readAttribute(child, "Layer"));
                    if (lid < model.Layers.Count && model.Layers[lid] != null)
                        model.Layers.RemoveAt((int)lid);

                    string name = readAttribute(child, "Name").Trim();
                    name = (name.Length > 0) ? name : Culture.Get("Layer");
                    Layer layer = new Layer(name);
                    layer.Id = lid;
                    layer.Layer = null;
                    model.Layers.Add(layer);
                    if (setActive)
                    {
                        model.ActiveLayer = layer;
                        setActive = false;
                    }
                }
            }
        }

        private void readAllLayerAssignments(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("Item".Equals(child.Name))
                    readLayerAssignments(child);
        }

        private void readLayerAssignments(XmlNode node)
        {
            uint id = uint.Parse(readAttribute(node, "Item"));
            uint lid = uint.Parse(readAttribute(node, "Layer"));
            bool visible = readAttribute(node, "Visible", "Yes").Equals("Yes");
            if (lid < model.Layers.Count)
            {
                Layer layer = model.Layers[lid];
                string type = readAttribute(node, "Type");
                if (layer != null)
                {
                    Item item = null;
                    switch (type)
                    {
                        case "Layer":
                            if (id < model.Layers.Count)
                                item = model.Layers[id];
                            break;
                        case "Joint":
                            if (id < model.JointList.Count)
                                item = model.JointList[id];
                            break;
                        case "Frame":
                            if (id < model.LineList.Count)
                                item = model.LineList[id];
                            break;
                    }
                    if (item != null)
                    {
                        item.Layer = layer;
                        item.IsVisible = visible;
                    }
                }
            }
            else
                Console.WriteLine("Layer {0} does not exist", lid);
        }


        private void readDiaphragm(XmlNode node)
        {
            readConstraints(node, Constraint.ConstType.Diaphragm);
        }

        private void readConstraints(XmlNode node, Constraint.ConstType type)
        {
            try
            {
                foreach (XmlNode child in node.ChildNodes)
                    if ("Constraint".Equals(child.Name))
                    {
                        string name = readAttribute(child, "Name");
                        string cs = readAttribute(child, "CoordSys");
                        Constraint.ConstraintAxis axis = (Constraint.ConstraintAxis)Enum.Parse(typeof(Constraint.ConstraintAxis), 
                            readAttribute(child, "Axis", "Z"));
                        //                    string ml = readAttribute(node, "MultiLevel", "No");
                        Constraint cons = new Constraint(name);
                        // cons.CoordinateSystem = cs; // Ignored
                        // cons.Axis = axis;
                        model.ConstraintList.Add(cons);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
            
        private void readConstraintAssignments(XmlNode node)
        {
            try
            {
                Dictionary<string, Constraint> map = new Dictionary<string, Constraint>();
                foreach (Constraint cons in model.ConstraintList)
                    if (cons != null)
                        map.Add(cons.Name, cons);

                foreach (XmlNode child in node.ChildNodes)
                    if ("Joint".Equals(child.Name))
                    {
                        int jid = int.Parse(readAttribute(child, "Joint", "0"));
                        string consName = readAttribute(child, "Constraint");

                        if (map.ContainsKey(consName) && model.JointList[jid] != null)
                            model.JointList[jid].Constraint = map[consName];

//                        readAttribute(child, "Type", j.Constraint.ConstraintType.ToString());
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void readCanguroVariables(XmlNode node)
        {
            int alid = int.Parse(readAttribute(node, "ActiveLayer", model.ActiveLayer.Id.ToString()));
            if (alid < model.Layers.Count && model.Layers[alid] != null)
                model.ActiveLayer = model.Layers[alid];

            string alc = readAttribute(node, "ActiveLoadCase", model.ActiveLoadCase.Name).Trim();
            alc = (alc.Length > 0) ? alc : Culture.Get("Case");
            if (model.LoadCases.ContainsKey(alc))
                model.ActiveLoadCase = model.LoadCases[alc];

            string usys = readAttribute(node, "UnitSystem", model.UnitSystem.GetType().ToString());
            foreach (UnitSystem.UnitSystem system in UnitSystem.UnitSystemsManager.Instance.UnitSystems)
                if (system.GetType().ToString().Equals(usys))
                {
                    model.UnitSystem = system;
                    break;
                }

            string layout = readAttribute(node, "Layout", "0");
            Canguro.View.GraphicViewManager.Instance.Layout = (Canguro.View.GraphicViewManager.ViewportsLayout)((byte)byte.Parse(layout));

            readViewOptions(node);
        }

        private void readViewOptions(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if ("View".Equals(child.Name))
                {
                    int id = int.Parse(readAttribute(child, "ID", "5"));
                    Canguro.View.GraphicView view = Canguro.View.GraphicViewManager.Instance.GetView(id);
                    if (view != null)
                    {
                        int options = int.Parse(readAttribute(child, "OptionsShown", "0"));
                        string rot = readAttribute(child, "Rotation");
                        string trans = readAttribute(child, "Translation");
                        string scale = readAttribute(child, "Scale");
                        string scaleFac = readAttribute(child, "ScaleFac", "1");
                        string[] vp = readAttribute(child, "Viewport").Split(",".ToCharArray());
                        float vpScale = 1;
                        if (vp.Length >= 6)
                        {
                            Microsoft.DirectX.Direct3D.Viewport vPort = new Microsoft.DirectX.Direct3D.Viewport();
                            vPort.X = int.Parse(vp[0]);
                            vPort.Width = int.Parse(vp[1]);
                            vPort.Y = int.Parse(vp[2]);
                            vPort.Height = int.Parse(vp[3]);
                            vPort.MinZ = float.Parse(vp[4]);
                            vPort.MaxZ = float.Parse(vp[5]);

                            Microsoft.DirectX.Direct3D.Viewport current = view.Viewport;
                            vpScale = (float)current.Width / (float)vPort.Width;
                            vpScale = (vpScale > (float)current.Height / (float)vPort.Height) ? (float)current.Height / (float)vPort.Height : vpScale;
                        }

                        view.ArcBallCtrl.RotationMatrix = GetAsMatrix(rot);
                        view.ArcBallCtrl.TranslationMatrix = GetAsMatrix(trans);
//                        view.ArcBallCtrl.Scaling = GetAsMatrix(scale);
                        view.ArcBallCtrl.ZoomAbsolute(Convert.ToSingle(scaleFac) * vpScale);
//                        view.ArcBallCtrl.ScalingFac = Convert.ToSingle(scaleFac) * vpScale;
                        view.ViewMatrix = view.ArcBallCtrl.ViewMatrix;
                        view.ModelRenderer.RenderOptions.OptionsShown = (Canguro.View.Renderer.RenderOptions.ShowOptions)options;
                    }
                }
            }
        }

        private Microsoft.DirectX.Matrix GetAsMatrix(string matrix)
        {
            string[] values = matrix.Split(",".ToCharArray());
            Microsoft.DirectX.Matrix m = new Microsoft.DirectX.Matrix();
            if (values.Length >= 16)
            {
                int i = 0;
                m.M11 = float.Parse(values[i++]);
                m.M12 = float.Parse(values[i++]);
                m.M13 = float.Parse(values[i++]);
                m.M14 = float.Parse(values[i++]);
                m.M21 = float.Parse(values[i++]);
                m.M22 = float.Parse(values[i++]);
                m.M23 = float.Parse(values[i++]);
                m.M24 = float.Parse(values[i++]);
                m.M31 = float.Parse(values[i++]);
                m.M32 = float.Parse(values[i++]);
                m.M33 = float.Parse(values[i++]);
                m.M34 = float.Parse(values[i++]);
                m.M41 = float.Parse(values[i++]);
                m.M42 = float.Parse(values[i++]);
                m.M43 = float.Parse(values[i++]);
                m.M44 = float.Parse(values[i++]);
            }
            return m;
        }

        public AccelLoad.AccelLoadValues decodeAccel(string value)
        {
            switch (value)
            {
                case "Accel U1": return AccelLoad.AccelLoadValues.UX;
                case "Accel U2": return AccelLoad.AccelLoadValues.UY;
                case "Accel U3": return AccelLoad.AccelLoadValues.UZ;
                case "Accel R1": return AccelLoad.AccelLoadValues.RX;
                case "Accel R2": return AccelLoad.AccelLoadValues.RY;
                case "Accel R3": return AccelLoad.AccelLoadValues.RZ;
            }
            return AccelLoad.AccelLoadValues.UX;
        }

        private string secShape(string shape)
        {
            switch (shape)
            {
                case "2L": return "Double Angle";
                case "C": return "Channel";
                case "I": return "I/Wide Flange";
                case "B": return "Box/Tube";
                case "P": return "Pipe";
                case "L": return "Angle";
                case "T": return "Tee";
                case "RN": return "Circle";
                case "R": return "Rectangular";
                default: return "General";
            }
        }

        private string CodeYN(bool val)
        {
            return (val) ? "Yes" : "No";
        }

        private string GetComboType(LoadCombination.CombinationType comboType)
        {
            switch (comboType)
            {
                case LoadCombination.CombinationType.AbsoluteAdd: return "Abs Add";
                case LoadCombination.CombinationType.Envelope: return "Envelope";
                case LoadCombination.CombinationType.SRSS: return "SRSS";
                default: return "Linear Add";
            }
        }

        private THDesignOptions GetTHDesign(string str)
        {
            return ("Envelopes".Equals(str)) ? THDesignOptions.Envelopes : THDesignOptions.Step_by_Step;
        }

        internal static string readAttribute(XmlNode node, string attName)
        {
            return readAttribute(node, attName, "");
        }

        internal static string readAttribute(XmlNode node, string attName, string defaultValue)
        {
            if (node.Attributes != null && node.Attributes[attName] != null)
                return node.Attributes[attName].Value;
            return defaultValue;
        }

        public SeismicZone GetSeismicZone(string zone)
        {
            switch (zone)
            {
                case "Zone 0": return SeismicZone.Zone0;
                case "Zone 1": return SeismicZone.Zone1;
                case "Zone 2": return SeismicZone.Zone2;
                case "Zone 3": return SeismicZone.Zone3;
                default: return SeismicZone.Zone4;
            }
        }
    }
}
