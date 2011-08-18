using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;
using Canguro.Model.Load;
using Canguro.Model.Material;
using Canguro.Model.Section;
using Canguro.Model.Design;
using Canguro.Model;
using Canguro;

namespace Canguro.Commands.Model {
    /// <summary>
    /// Create a MDB file on the fly. MDB file will be send to SAP application 
    /// </summary> 
    /// <summary>
    /// Model Command to export the model into an MS Access database file
    /// </summary>

    public class ExportMDBCmd : Canguro.Commands.ModelCommand {

        /// <summary>
        /// Implemented method from abstract class ModelCommand
        /// </summary>
        /// <param name="services"></param>

        /// <summary>
        /// Executes the command. 
        /// Calls the Export static method with the current Model and the file name "tmp"
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services) {
            Canguro.Model.Model m = services.Model;
            Export(m, "tmp");
        }

        /// <summary>
        /// Create a temporary MDB file that will be send to SAP application 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="filePath"></param>
        internal void Export(Canguro.Model.Model m, string filePath)
        {
            OleDbConnection cn = null;
            Canguro.Model.UnitSystem.UnitSystem uSystem = m.UnitSystem;

            try
            {
                File.Copy(System.Windows.Forms.Application.StartupPath + "\\RuntimeData\\modelTransfer", filePath /*"tmp"*/, true);

                //Use a string variable to hold the ConnectionString.
                string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath;

                //Create an OleDbConnection object, 
                //and then pass in the ConnectionString to the constructor.
                cn = new OleDbConnection(connectString);
                cn.Open();

                m.UnitSystem = Canguro.Model.UnitSystem.InternationalSystem.Instance;
                // Create store procedures for joint items
                store(cn, m.JointList);
                // Create store procedures for line itemes
                store(cn, m.LineList);
                // Create store procedures for materials 
                foreach (Material mat in MaterialManager.Instance.Materials.AsReadOnly())
                    if (mat != null)
                        store(cn, mat);
                // Create store procedures for abstract case
                foreach (AbstractCase aCase in m.AbstractCases)
                    if (aCase != null)
                        store(cn, aCase, m);
                // Create store procedures for load cases
                foreach (LoadCase lCase in m.LoadCases.Values)
                    if (lCase != null)
                        store(cn, lCase);
                // Create store procedures for concrete material
                store(cn, m.ConcreteDesignOptions);
                // Create store procedures for steel materia
                store(cn, m.SteelDesignOptions);
                // Create store procedures for frame design
                storeFrameDesignProcedures(cn, m);
                // Create store procedures for spectrum analysis
                store(cn, m.ResponseSpectra, m.AbstractCases);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(Culture.Get("ErrorExporting"), Culture.Get("error"),
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                if (cn != null)
                    cn.Close();
                m.UnitSystem = uSystem;
            }
        }

        /// <summary>
        /// Create tables for Joint List
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="obj"></param>
        private void store(OleDbConnection cn, ItemList<Joint> obj) {
            foreach (Item i in obj) {
                if ((i is Joint) && (i != null))
                    store(cn, (Joint)i);
            }
        }
        
        /// <summary>
        /// Create tables for joint elements
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="obj"></param>
        private void store(OleDbConnection cn, Joint obj)
        {
            Microsoft.DirectX.Vector3 pos = obj.Position;
            string sql = "INSERT INTO [Joint Coordinates](Joint, CoordSys, CoordType, XorR, Y, Z, SpecialJt, GlobalX, GlobalY, GlobalZ) " +
                "VALUES (" + obj.Id + ", \"GLOBAL\", \"Cartesian\", " + pos.X + ", " + pos.Y + ", " + pos.Z + ", \"Yes\", " + pos.X + ", " + pos.Y + ", " + pos.Z + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();

            float[] masses = obj.Masses;

            if (masses != null && masses.Length >= 6)
            {
                if (masses[0] + masses[1] + masses[2] + masses[3] + masses[4] + masses[5] > float.Epsilon)
                {
                    sql = "INSERT INTO [Joint Added Mass Assignments](Joint, CoordSys, Mass1, Mass2, Mass3, MMI1, MMI2, MMI3) " +
                       "VALUES (" + obj.Id + ", \"GLOBAL\", " + masses[0] + ", " + masses[1] + ", " + masses[2] + ", " + masses[3] + ", " + masses[4] + ", " + masses[5] + ");";
                    new OleDbCommand(sql, cn).ExecuteNonQuery();
                }
            }

            JointDOF dof = obj.DoF;
            if (dof != null)
                store(cn, obj.Id, obj.DoF);
            AssignedLoads loads = obj.Loads;
            if (loads != null)
                store(cn, obj.Id, loads);
        }

        private void store(OleDbConnection cn, uint joint, JointDOF obj)
        {
            if (obj.IsRestrained)
            {
                string u1 = (obj.T1 == JointDOF.DofType.Restrained) ? "Yes" : "No";
                string u2 = (obj.T2 == JointDOF.DofType.Restrained) ? "Yes" : "No";
                string u3 = (obj.T3 == JointDOF.DofType.Restrained) ? "Yes" : "No";
                string r1 = (obj.R1 == JointDOF.DofType.Restrained) ? "Yes" : "No";
                string r2 = (obj.R2 == JointDOF.DofType.Restrained) ? "Yes" : "No";
                string r3 = (obj.R3 == JointDOF.DofType.Restrained) ? "Yes" : "No";
                string sql = "INSERT INTO [Joint Restraint Assignments](Joint, U1, U2, U3, R1, R2, R3) " +
                    "VALUES (" + joint + ", \"" + u1 + "\", \"" + u2 + "\", \"" + u3 + "\", \"" + r1 + "\", \"" + r2 + "\", \"" + r3 + "\");";
                new OleDbCommand(sql, cn).ExecuteNonQuery();
            }

            if (obj.IsSpring)
            {
                float[] sp = obj.SpringValues;
                string sql = "INSERT INTO [Joint Spring Assignments 1 - Uncoupled](Joint, CoordSys, U1, U2, U3, R1, R2, R3) " +
                    "VALUES (" + joint + ", \"GLOBAL\", " + sp[0] + ", " + sp[1] + ", " + sp[2] + ", " + sp[3] + ", " + sp[4] + ", " + sp[5] + ");";
                new OleDbCommand(sql, cn).ExecuteNonQuery();
            }
        }

        private void store(OleDbConnection cn, uint itemId, AssignedLoads obj)
        {
            Dictionary<string, LoadCase> cases = Canguro.Model.Model.Instance.LoadCases;
            foreach (LoadCase lCase in cases.Values)
            {
                ItemList<Canguro.Model.Load.Load> list = obj[lCase];
                if (list != null)
                {
                    foreach (Canguro.Model.Load.Load load in list)
                    {
                        if (load != null)
                            store(cn, itemId, lCase.Name, load);
                    }
                }
            }
        }

        private void store(OleDbConnection cn, uint itemId, string loadCase, Canguro.Model.Load.Load obj)
        {
            if (obj is JointLoad)
                store(cn, itemId, loadCase, (JointLoad)obj);
            else if (obj is LineLoad)
                store(cn, itemId, loadCase, (LineLoad)obj);
            else if (obj is AreaLoad)
                store(cn, itemId, loadCase, (AreaLoad)obj);
        }

        private void store(OleDbConnection cn, uint itemId, string loadCase, JointLoad obj)
        {
            string sql = "";
            if (obj is ForceLoad)
            {
                float[] force = ((ForceLoad)obj).Force;

                sql = "INSERT INTO [Joint Loads - Force] " +
                    "(Joint,LoadCase,CoordSys,F1,F2,F3,M1,M2,M3) VALUES " +
                    "(" + itemId + ",\"" + loadCase + "\",\"GLOBAL\"," +
                    force[0] + "," + force[1] + "," + force[2] + "," +
                    force[3] + "," + force[4] + "," + force[5] + ");";
            }
            else if (obj is GroundDisplacementLoad)
            {
                float[] disp = ((GroundDisplacementLoad)obj).Displacements;
                sql = "INSERT INTO [Joint Loads - Ground Displacement] " +
                    "(Joint,LoadCase,CoordSys,U1,U2,U3,R1,R2,R3) VALUES " +
                    "(" + itemId + ",\"" + loadCase + "\",\"GLOBAL\"," +
                    disp[0] + "," + disp[1] + "," + disp[2] + "," +
                    disp[3] + "," + disp[4] + "," + disp[5] + ");";
            }
            if (sql.Length > 0)
                new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        /// <summary>
        /// Create tables for line elements
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="obj"></param>
        private void store(OleDbConnection cn, ItemList<LineElement> obj)
        {
            Dictionary<FrameSection, FrameSection> frameSectionCache = new Dictionary<FrameSection, FrameSection>();

            foreach (LineElement i in obj)
            {
                if (i != null)
                {
                    store(cn, i);
                    LineProps props = i.Properties;
                    if (props != null && props is StraightFrameProps)
                    {
                        FrameSection sec = ((StraightFrameProps)props).Section;
                        if (!frameSectionCache.ContainsKey(sec))
                            frameSectionCache.Add(sec, sec);
                    }
                }
            }

            foreach (FrameSection fs in frameSectionCache.Keys)
                if (fs != null)
                    store(cn, fs);
        }

        private void store(OleDbConnection cn, LineElement obj) {
            Joint i = obj.I;
            Joint j = obj.J;
            string sql = "INSERT INTO [Connectivity - Frame] (Frame, JointI, JointJ, IsCurved, Length, CentroidX, CentroidY, CentroidZ) " +
                "VALUES (" + obj.Id + ", " + i.Id + ", " + j.Id + ",\"No\", " + obj.Length + ", " + (i.X + j.X) / 2.0F + ", " + (i.Y + j.Y) / 2.0F + ", " + (i.Z + j.Z) / 2.0F + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();

            LineProps props = obj.Properties;
            List<Section> exported = new List<Section>();
            if (props is StraightFrameProps)
            {
                FrameSection sec = ((StraightFrameProps)props).Section;
                sql = "INSERT INTO [Frame Section Assignments] (Frame, SectionType, AutoSelect, AnalSect, DesignSect, MatProp) " +
                    "VALUES (" + obj.Id + ", \"" + secShape(sec.Shape) + "\", \"N.A.\", \"" + sec.Name + "\", \"" + sec.Name + "\", \"Default\");";
                new OleDbCommand(sql, cn).ExecuteNonQuery();
                store(cn, sec.ConcreteProperties, sec, exported);
            }
            AssignedLoads loads = obj.Loads;
            if (loads != null)
                store(cn, obj.Id, loads);
        }

        private void store(OleDbConnection cn, ConcreteSectionProps obj, FrameSection section, List<Section> exported)
        {
            if (obj != null && !exported.Contains(section))
            {
                if (obj is ConcreteBeamSectionProps)
                    store(cn, (ConcreteBeamSectionProps)obj, section);
                else if (obj is ConcreteColumnSectionProps)
                    store(cn, (ConcreteColumnSectionProps)obj, section);
                exported.Add(section);
            }
        }

        private void store(OleDbConnection cn, ConcreteBeamSectionProps obj, FrameSection section)
        {
            string mat = MaterialManager.Instance.DefaultSteel.Name;
            string sec = section.Name;
            string sql = "INSERT INTO [Frame Section Properties 03 - Concrete Beam] (SectionName, RebarMatL, RebarMatC, TopCover, BotCover, TopLeftArea, TopRghtArea, BotLeftArea, BotRghtArea) " +
                "VALUES (\"" + sec + "\", \"" + mat + "\", \"" + mat + "\", " + obj.ConcreteCoverTop + ", " + obj.ConcreteCoverBottom + ", " + obj.RoTopLeft + ", " + obj.RoTopRight + ", " + obj.RoBottomLeft + ", " + obj.RoBottomRight + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, ConcreteColumnSectionProps obj, FrameSection section)
        {
            string mat = MaterialManager.Instance.DefaultSteel.Name;
            string sql, bars3, bars2, barsCirc;
            if (section is Rectangular)
            {
                bars2 = obj.NumberOfBars2Dir.ToString();
                bars3 = obj.NumberOfBars3Dir.ToString();
                barsCirc = "0";
            }
            else // Circular
            {
                bars2 = "0";
                bars3 = "0";
                barsCirc = obj.NumberOfBars.ToString();
            }

            string barsize = obj.BarSize.ToString();

            sql = "INSERT INTO [Frame Section Properties 02 - Concrete Column] (SectionName, RebarMatL, RebarMatC, ReinfConfig, LatReinf, Cover, NumBars3Dir, NumBars2Dir, NumBarsCirc, BarSize, SpacingC, NumCBars2, NumCBars3, ReinfType) " +
                "VALUES (\"" + section + "\", \"" + mat + "\", \"" + mat + "\", \"" + obj.RConfiguration + "\", \"" + obj.LateralR + "\", " + obj.CoverToRebarCenter + ", " + bars3 + ", " + bars2 + ", " + barsCirc + ", \"" + barsize + "\", " + obj.SpacingC + ", " + bars2 + ", " + bars3 + ", \"Design\");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, LoadCombination obj, Canguro.Model.Model model)
        {
            List<AbstractCaseFactor> list = obj.Cases;

            string sql = "";
            string steel = CodeYN(model.SteelDesignOptions.DesignCombinations.Contains(obj));
            string concrete = CodeYN(model.ConcreteDesignOptions.DesignCombinations.Contains(obj));
            string alum = CodeYN(model.AluminumDesignOptions.DesignCombinations.Contains(obj));
            string cold = CodeYN(model.ColdFormedDesignOptions.DesignCombinations.Contains(obj));
            string comboType = GetComboType(obj.Type);
            foreach (AbstractCaseFactor f in list)
            {
                AbstractCase aCase = f.Case as AbstractCase;
                sql = "";
                if (aCase != null) // && aCase.IsActive)
                {
                    if (aCase is LoadCombination)
                        sql = "INSERT INTO [Combination Definitions] " +
                            "(ComboName,ComboType,CaseType,CaseName,ScaleFactor,SteelDesign,ConcDesign,AlumDesign,ColdDesign) VALUES " +
                            "(\"" + obj.Name + "\",\"\",\"Combo\",\"" + aCase.Name + "\"," + f.Factor + "," + steel + "," + concrete + "," + alum + "," + cold + ");";
                    else if (aCase is AnalysisCase)
                    {
                        AnalysisCaseProps props = ((AnalysisCase)aCase).Properties;
                        if (props is StaticCaseProps)
                        {
                            // StaticCaseProps sprops = (StaticCaseProps)props;
                            sql = "INSERT INTO [Combination Definitions] " +
                                "(ComboName,ComboType,CaseType,CaseName,ScaleFactor,SteelDesign,ConcDesign,AlumDesign,ColdDesign) VALUES " +
                                "(\"" + obj.Name + "\",\"" + comboType + "\",\"Linear Static\",\"" + aCase.Name + "\"," + f.Factor + "," + steel + "," + concrete + "," + alum + "," + cold + ");";
                        }
                        else if (props is ResponseSpectrumCaseProps)
                        {
                            sql = "INSERT INTO [Combination Definitions] " +
                                "(ComboName,ComboType,CaseType,CaseName,ScaleFactor,SteelDesign,ConcDesign,AlumDesign,ColdDesign) VALUES " +
                                "(\"" + obj.Name + "\",\"" + comboType + "\",\"Response Spectrum\",\"" + aCase.Name + "\"," + f.Factor + "," + steel + "," + concrete + "," + alum + "," + cold + ");";
                        }
                        //steel = concrete = alum = cold = "\"\"";
                    }
                    if (sql.Length > 0)
                        new OleDbCommand(sql, cn).ExecuteNonQuery();
                }
            }

            // Insert record in RESULTS Named Set
            sql = " INSERT INTO [Named Sets - Database Tables 2 - Selections] " +
                "(DBNamedSet, SelectType, [Selection]) VALUES (\"RESULTS\", \"Combo\", \"" + obj.Name + "\");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, uint itemId, string loadCase, DirectionalLineLoad obj)
        {
            string sql = "";
            string dir = obj.Direction.ToString();
            string dirFrame = "GLOBAL";

            if (obj.Direction != LineLoad.LoadDirection.Gravity)
            {
                dirFrame = dir.Substring(0, dir.Length - 1).ToUpper();
                dir = dir.Substring(dir.Length - 1);
            }

            if (obj is ConcentratedSpanLoad)
            {
                ConcentratedSpanLoad point = (ConcentratedSpanLoad)obj;
                sql = "INSERT INTO [Frame Loads - Point] " +
                    "(Frame,LoadCase,CoordSys,Type,Dir,DistType,RelDist,AbsDist,Force) VALUES " +
                    "(" + itemId + ",\"" + loadCase + "\",\"" + dirFrame + "\",\"" + point.Type + "\"," +
                    "\"" + dir + "\",\"RelDist\"," + point.D + ",0," + point.LoadInt + ");";
            }
            else if (obj is DistributedSpanLoad)
            {
                DistributedSpanLoad dist = (DistributedSpanLoad)obj;
                sql = "INSERT INTO [Frame Loads - Distributed] " +
                    "(Frame,LoadCase,CoordSys,Type,Dir,DistType,RelDistA,RelDistB,AbsDistA,AbsDistB,FOverLA,FOverLB) VALUES " +
                    "(" + itemId + ",\"" + loadCase + "\",\"" + dirFrame + "\",\"" + dist.Type + "\"," +
                    "\"" + dir + "\",\"RelDist\"," + dist.Da + "," + dist.Db + ",0,0," + dist.LoadAInt + "," + dist.LoadBInt + ");";
            }
            if (sql.Length > 0)
                new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, uint itemId, string loadCase, AreaLoad obj)
        { }
                
        private void store(OleDbConnection cn, object obj)
        {
            throw new NotImplementedException("Type " + obj.GetType() + " not implemented");
        }

        private void store(OleDbConnection cn, Item obj)
        {
            if (obj != null)
            {
                if (obj is Joint)
                    store(cn, (Joint)obj);
                else if (obj is LineElement)
                    store(cn, (LineElement)obj);
                else if (obj is AreaElement)
                    store(cn, (AreaElement)obj);
            }
        }

        private void store(OleDbConnection cn, ItemList<AreaElement> obj)
        {
            throw new NotImplementedException("Not implemented until version 2.0");
        }

        private void store(OleDbConnection cn, ItemList<Layer> obj)
        {
            throw new NotImplementedException("Currently not relevant for analysis");
        }

        private void store(OleDbConnection cn, FrameSection obj)
        {
            ConcreteSectionProps cprops = obj.ConcreteProperties;
            string concC = (cprops != null && cprops is ConcreteColumnSectionProps) ? "Yes" : "No";
            string concB = (cprops != null && cprops is ConcreteBeamSectionProps) ? "Yes" : "No";
            string sql = "INSERT INTO [Frame Section Properties 01 - General] " +
                "(SectionName, Material, Shape, t3, t2, tf, tw, t2b, tfb, Area, TorsConst, I33, I22, AS2, AS3, S33, S22, Z33, Z22, R33, R22, ConcCol, ConcBeam, Color, TotalWt, TotalMass, FromFile, AMod, A2Mod, A3Mod, JMod, I2Mod, I3Mod, MMod, WMod) " +
                    "VALUES (\"" + obj.Name + "\", \"" + obj.Material.Name + "\", \"" + secShape(obj.Shape) + "\", " +
                    obj.T3 + ", " + obj.T2 + ", " + obj.Tf + ", " + obj.Tw + ", " + obj.T2b + ", " + obj.Tfb + ", " + obj.Area + ", " +
                    obj.TorsConst + ", " + obj.I33 + ", " + obj.I22 + ", " + obj.As2 + ", " + obj.As3 + ", " + obj.S33 + ", " + obj.S22 + ", " + obj.Z33 + ", " + obj.Z22 + ", " + obj.R33 + ", " + obj.R22 + ", \"" + concC + "\", \"" + concB + "\", \"Yellow\", 0, 0, \"No\", 1, 1, 1, 1, 1, 1, 1, 1);";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, AreaElement obj)
        {
            throw new NotImplementedException("Not implemented until version 2.0");
        }

        private void store(OleDbConnection cn, Material obj)
        {
            MaterialTypeProps tProps = obj.TypeProperties;
            MaterialDesignProps dProps = obj.DesignProperties;
            double e, u, a;
            string type = "";
            string design = (dProps is RebarDesignProps) ? "Rebar" :
                (dProps is ColdFormedDesignProps) ? "ColdFormed" :
                (dProps is SteelDesignProps) ? "Steel" :
                (dProps is ConcreteDesignProps) ? "Concrete" :
                (dProps is AluminumDesignProps) ? "Aluminum" : "None";
            if (tProps is IsotropicTypeProps)
            {
                IsotropicTypeProps iProps = tProps as IsotropicTypeProps;
                type = "Isotropic";
                e = iProps.E;
                u = iProps.Nu;
                a = iProps.Alpha;
            }
            else if (tProps is UniaxialTypeProps)
            {
                UniaxialTypeProps uProps = tProps as UniaxialTypeProps;
                type = "Uniaxial";
                e = uProps.E;
                u = 0.0;
                a = uProps.A;
            }
            else
            {
                e = u = a = 0.0;
                type = (tProps is OrthotropicTypeProps) ? "Orthotropic" : "Anisotropic";
            }

            string sql = "INSERT INTO [Material Properties 01 - General] " +
                "(Material,Type,DesignType,UnitMass,UnitWeight,E,U,A,MDampRatio,VDampMass,VDampStiff,HDampMass,HDampStiff,NumAdvance,Color) " +
                "VALUES (\"" + obj.Name + "\",\"" + type + "\",\"" + design + "\"," +
                obj.Density + "," + obj.UnitWeight + "," + e + "," + u + "," + a + ",0,0,0,0,0,0,\"Yellow\");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            store(cn, obj.Name, dProps);
        }

        private void store(OleDbConnection cn, string material, MaterialDesignProps props)
        {
            if (props is ConcreteDesignProps) store(cn, material, (ConcreteDesignProps)props);
            else if (props is NoDesignProps) store(cn, material, (NoDesignProps)props);
            else if (props is RebarDesignProps) store(cn, material, (RebarDesignProps)props);
            else if (props is AluminumDesignProps) store(cn, material, (AluminumDesignProps)props);
            else if (props is ColdFormedDesignProps) store(cn, material, (ColdFormedDesignProps)props);
            else if (props is SteelDesignProps) store(cn, material, (SteelDesignProps)props); // Se tiene que checar al final porque tiene herencia.
            else
                throw new NotSupportedException("Only specific DesignProps are supported, this method should not be called");
        }
        
        private void store(OleDbConnection cn, string material, ConcreteDesignProps props)
        {
            string isLW = (props.IsLightweightConcrete) ? "Yes" : "No";
            string sql = "INSERT INTO [Material Properties 04 - Design Concrete] " +
                "(Material, Fc, RebarFy, RebarFys, LtWtConc, LtWtFact) VALUES " +
                "(\"" + material + "\", " + props.Fc + ", " + props.RebarFy + ", " + props.RebarFys + ", \"" + isLW + "\", " + props.LightweightFactor + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, string material, NoDesignProps props)
        {
            // Ignore call: no design.
        }

        private void store(OleDbConnection cn, string material, RebarDesignProps props)
        {
            string sql = "INSERT INTO [Material Properties 11 - Design Rebar] " +
                "(Material, Fy, Fu) VALUES (\"" + material + "\", " + props.Fy + ", " + props.Fu + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, string material, AluminumDesignProps props)
        {
            string sql = "INSERT INTO [Material Properties 05 - Design Aluminum] " +
                "(Material, AlumType, Alloy, Ftu, Fty, Fcy, Fsu, Fsy) VALUES " +
                "(\"" + material + "\", " + props.Type + ", " + props.Alloy + ", " + props.Ftu + ", " + props.Fty + ", " + props.Fcy + ", " + props.Fsu + ", " + props.Fsy + ", " + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }
        
        private void store(OleDbConnection cn, string material, ColdFormedDesignProps props)
        {
            string sql = "INSERT INTO [Material Properties 06 - Design ColdFormed] " +
                "(Material, Fy, Fu) VALUES (\"" + material + "\", " + props.Fy + ", " + props.Fu + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }
        
        private void store(OleDbConnection cn, string material, SteelDesignProps props)
        {
            string sql = "INSERT INTO [Material Properties 03 - Design Steel] " +
                "(Material, Fy, Fu) VALUES (\"" + material + "\", " + props.Fy + ", " + props.Fu + ");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }
        
        private void store(OleDbConnection cn, AbstractCase obj, Canguro.Model.Model model)
        {
            if (obj is AnalysisCase && obj.IsActive)
                store(cn, (AnalysisCase)obj);
            else if (obj is LoadCombination && obj.IsActive)
                store(cn, (LoadCombination)obj, model);
        }

        private void store(OleDbConnection cn, AnalysisCase obj)
        {
            AnalysisCaseProps props = obj.Properties;
            string sql;
            if (props is ResponseSpectrumCaseProps)
            {
                ResponseSpectrumCaseProps rProps = props as ResponseSpectrumCaseProps;
                sql = "INSERT INTO [Analysis Case Definitions] " +
                    "([Case], Type, ModalCase, RunCase) VALUES " +
                    "(\"" + obj.Name + "\", \"LinRespSpec\", \"" + rProps.ModalAnalysisCase + "\", \"Yes\");";
            }
            else
            {
                string type = (props is ModalCaseProps) ? "LinModal" : "LinStatic";
                sql = "INSERT INTO [Analysis Case Definitions] " +
                    "([Case], Type, InitialCond, RunCase) VALUES " +
                    "(\"" + obj.Name + "\",\"" + type + "\",\"Zero\",\"Yes\");";
            }

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            // Insert record in RESULTS Named Set
            sql = " INSERT INTO [Named Sets - Database Tables 2 - Selections] " +
                "(DBNamedSet, SelectType, [Selection]) VALUES (\"RESULTS\", \"AnalysisCase\", \"" + obj.Name + "\");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();

            if (props is StaticCaseProps)
                store(cn, obj.Name, (StaticCaseProps)props);
            else if (props is ModalCaseProps)
                store(cn, obj.Name, (ModalCaseProps)props);
            else if (props is ResponseSpectrumCaseProps)
                store(cn, obj.Name, (ResponseSpectrumCaseProps)props);
            //else if (props is MultistepStaticCaseProps)
            //    store(cn, obj.Name, (MultistepStaticCaseProps)props);
            //else if (props is TimeHistoryCaseProps)
            //    store(cn, obj.Name, (TimeHistoryCaseProps)props);
            //else if (props is MovingLoadCaseProps)
            //    store(cn, obj.Name, (MovingLoadCaseProps)props);
            //else if (props is BucklingCaseProps)
            //    store(cn, obj.Name, (BucklingCaseProps)props);
            //else if (props is SteadyStateCaseProps)
            //    store(cn, obj.Name, (SteadyStateCaseProps)props);
            //else if (props is PowerSpectalDensityCaseProps)
            //    store(cn, obj.Name, (PowerSpectalDensityCaseProps)props);

        }

        private void store(OleDbConnection cn, string analysisCase, StaticCaseProps props)
        {
            foreach (StaticCaseFactor factor in props.Loads)
            {
                string lType, lName, sFact;
                AnalysisCaseAppliedLoad appLoad = factor.AppliedLoad;

                if (appLoad is AccelLoad)
                {
                    lType = "Accel load";
                    lName = ((AccelLoad)appLoad).Value.ToString();
                    sFact = "1.0";
                }
                else
                {
                    lType = "Load case";
                    lName = ((LoadCase)appLoad).Name;
                    sFact = factor.Factor.ToString();
                }
                string sql = "INSERT INTO [Case - Static 1 - Load Assignments] " +
                    "([Case],LoadType,LoadName, LoadSF) VALUES " +
                    "(\"" + analysisCase + "\",\"" + lType + "\",\"" + lName + "\"," + sFact + ");";
                new OleDbCommand(sql, cn).ExecuteNonQuery();
            }
        }

        private void store(OleDbConnection cn, string analysisCase, ModalCaseProps props)
        {
            string sql = "INSERT INTO [Case - Modal 1 - General] " +
                "([Case],ModeType,MaxNumModes,MinNumModes,EigenShift,EigenCutoff,EigenTol,AutoShift) VALUES " +
                "(\"" + analysisCase + "\",\"" + props.ModesType + "\"," + props.MaxModes + "," + props.MinModes + ",0,0,0.000000001,\"No\");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
            if (props.ModesType == ModalCaseProps.ModesMethod.RitzVectors)
            {
                List<ModalCaseFactor> list = props.Loads;
                foreach (ModalCaseFactor f in list)
                {
                    if (f == null) continue;
                    AnalysisCaseAppliedLoad load = f.AppliedLoad;
                    sql = "";
                    if (load is LoadCase)
                    {
                        sql = "INSERT INTO [Case - Modal 3 - Load Assignments - Ritz] " +
                            "([Case],LoadType,LoadName,MaxCycles,TargetPar) VALUES " +
                            "(\"" + analysisCase + "\",\"Load Case\",\"" + ((LoadCase)load).Name + "\"," +
                            f.Cycles + "," + f.Ratio + ");";
                    }
                    else if (load is AccelLoad)
                    {
                        sql = "INSERT INTO [Case - Modal 3 - Load Assignments - Ritz] " +
                            "([Case],LoadType,LoadName,MaxCycles,TargetPar) VALUES " +
                            "(\"" + analysisCase + "\",\"Accel\",\"Accel " + ((AccelLoad)load).Value + "\"," +
                            f.Cycles + "," + f.Ratio + ");";
                    }
                    if (sql.Length > 0)
                        new OleDbCommand(sql, cn).ExecuteNonQuery();
                }
            }
        }

        private void store(OleDbConnection cn, string analysisCase, ResponseSpectrumCaseProps props)
        {
            string sql = "INSERT INTO [Case - Response Spectrum 1 - General] " +
                "([Case],ModalCombo,DirCombo,DampingType,ConstDamp,EccenRatio,NumOverride) VALUES " +
                "(\"" + analysisCase + "\",\"" + props.ModalCombination + "\",\"" + props.DirectionalCombination + "\"," +
                "\"Constant\"," + props.ModalDamping + ",0,0);";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
            List<ResponseSpectrumCaseFactor> list = props.Loads;
            foreach (ResponseSpectrumCaseFactor f in list)
            {
                if (f == null) continue;
                AccelLoad load = f.Accel as AccelLoad;
                sql = "";
                if (load != null)
                {
                    sql = "INSERT INTO [Case - Response Spectrum 2 - Load Assignments] " +
                        "([Case],LoadType,LoadName,CoordSys,Function,Angle,TransAccSF) VALUES " +
                        "(\"" + analysisCase + "\",\"Acceleration\",\"" + encode(load.Value) + "\"," +
                        "\"GLOBAL\",\"" + props.ResponseSpectrumFunction + "\",0,1);";
                    new OleDbCommand(sql, cn).ExecuteNonQuery();
                }
            }
        }

        private void store(OleDbConnection cn, LoadCase obj)
        {
            string sql;
            if (string.IsNullOrEmpty(obj.AutoLoad))
                sql = "INSERT INTO [Load Case Definitions] " +
                    "(LoadCase, DesignType, SelfWtMult) VALUES " +
                    "(\"" + obj.Name + "\", \"" + obj.CaseType.ToString().ToUpper() + "\"," + obj.SelfWeight + ");";
            else
                sql = "INSERT INTO [Load Case Definitions] " +
                    "(LoadCase, DesignType, SelfWtMult, AutoLoad) VALUES " +
                    "(\"" + obj.Name + "\", \"" + obj.CaseType.ToString().ToUpper() + "\"," + obj.SelfWeight + ",\"" + obj.AutoLoad + "\");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            // Insert record in RESULTS Named Set
            sql = " INSERT INTO [Named Sets - Database Tables 2 - Selections] " +
                "(DBNamedSet, SelectType, [Selection]) VALUES (\"RESULTS\", \"LoadCase\", \"" + obj.Name + "\");";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, DesignOptions obj)
        {
            if (obj is NoDesign)
                return;
            else if (obj is LRFD99) //A
                store(cn, (LRFD99)obj);
            else if (obj is RCDF2001) //C
                store(cn, (RCDF2001)obj);
            else if (obj is ACI318_02) //C
                store(cn, (ACI318_02)obj);
            else if (obj is UBC97_ASD) //A
                store(cn, (UBC97_ASD)obj);
            else if (obj is UBC97_LRFD) //A
                store(cn, (UBC97_LRFD)obj);
            else if (obj is UBC97_Conc) //C
                store(cn, (UBC97_Conc)obj);
            else if (obj is ASD01) //A
                store(cn, (ASD01)obj);
        }

        private void store(OleDbConnection cn, LRFD99 obj) {
            string thDesign = obj.GetTHDesignName(obj.TimeHistoryDesign);
            string FrameType = "\"OMF\"";

            string sql = "INSERT INTO [Preferences - Steel Design - AISC-LRFD99] " +
                    "(THDesign, FrameType, PatLLF, SRatioLimit, MaxIter, PhiB, PhiC, PhiTY, PhiV, PhiTF, PhiVT, PhiCA, CheckDefl, DLRat, SDLAndLLRat, LLRat, TotalRat, NetRat, SeisCat, SeisCode, SeisLoad, PlugWeld) VALUES " +
                    "(\"" + thDesign + "\", " + FrameType + ", " + obj.PatLLF + ", " + obj.SRatioLimit + ", " + obj.MaxIter + ", " + obj.PhiB + ", " + obj.PhiC + ", " + obj.PhiTY + ", " + obj.PhiV + ", " + obj.PhiTF + ", " + obj.PhiVT + ", " + obj.PhiCA + ", " + CodeYN(obj.CheckDefl) + ", " + obj.DLRat + ", " + obj.SDLAndLLRat + ", " + obj.LLRat + ", " + obj.TotalRat + ", " + obj.NetRat + ", \"" + obj.SeisCat + "\", " + CodeYN(obj.SeisCode) + ", " + CodeYN(obj.SeisLoad) + ", " + CodeYN(obj.PlugWeld) + ");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set SteelCode=\"AISC-LRFD99\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, RCDF2001 obj)
        {
            string thDesign = obj.GetTHDesignName(obj.THDesign);
            string sql = "INSERT INTO [Preferences - Concrete Design - Mexican RCDF 2001] " +
                    "(THDesign, NumCurves, NumPoints, MinEccen, PatLLF, UFLimit, PhiB, PhiT, PhiCTied, PhiCSpiral, PhiV) VALUES " +
                    "(\"" + thDesign + "\", " + obj.NumCurves + ", " + obj.NumPoints + ", " + CodeYN(obj.MinEccen) + ", " + obj.PatLLF + ", " + obj.UFLimit + ", " + obj.PhiB + ", " + obj.PhiT + ", " + obj.PhiCTied + ", " + obj.PhiCSpiral + ", " + obj.PhiV + ");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set ConcCode=\"Mexican RCDF 2001\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, ACI318_02 obj)
        {
            string thDesign = obj.GetTHDesignName(obj.THDesign);
            //string FrameType = "\"OMF\"";
            string sql = "INSERT INTO [Preferences - Concrete Design - ACI 318-02] " +
                    "(THDesign, NumCurves, NumPoints, MinEccen, PatLLF, UFLimit, PhiT, PhiCTied, PhiCSpiral, PhiV, PhiVSeismic, PhiVJoint, SeisCat) VALUES " +
                    "(\"" + thDesign + "\", " + obj.NumCurves + ", " + obj.NumPoints + ", " + CodeYN(obj.MinEccen) + ", " + obj.PatLLF + ", " + obj.UFLimit + ", " + obj.PhiT + ", " + obj.PhiCTied + ", " + obj.PhiCSpiral + ", " + obj.PhiV + ", " + obj.PhiVSeismic + ", " + obj.PhiVJoint + ", \"" + obj.SeisCat + "\");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set ConcCode=\"ACI 318-02\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, UBC97_ASD obj)
        {
            string thDesign = obj.GetTHDesignName(obj.TimeHistoryDesign);
            //string FrameType = obj.GetFrameTypeName(obj.FrameType);
            string FrameType = "Ordinary MRF";
            string zone = obj.GetSeismicZoneName(obj.SeisZone);
            string sql = "INSERT INTO [Preferences - Steel Design - UBC97-ASD] " +
                    "(THDesign, FrameType, PatLLF, SRatioLimit, MaxIter, SeisZone, LatFactor, CheckDefl, DLRat, SDLAndLLRat, LLRat, TotalRat, NetRat) VALUES " +
                    "(\"" + thDesign + "\", \"" + FrameType + "\", " + obj.PatLLF + ", " + obj.SRatioLimit + ", " + obj.MaxIter + ", \"" + zone + "\", " + obj.LatFactor + ", " + CodeYN(obj.CheckDefl) + ", " + obj.DLRat + ", " + obj.SDLAndLLRat + ", " + obj.LLRat + ", " + obj.TotalRat + ", " + obj.NetRat + ");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set SteelCode=\"UBC97-ASD\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, UBC97_LRFD obj)
        {
            string thDesign = obj.GetTHDesignName(obj.TimeHistoryDesign);
            //string FrameType = obj.GetFrameTypeName(obj.FrameType);
            string FrameType = "Ordinary MRF";
            string zone = obj.GetSeismicZoneName(obj.SeisZone);
            string sql = "INSERT INTO [Preferences - Steel Design - UBC97-LRFD] " +
                    "(THDesign, FrameType, PatLLF, SRatioLimit, MaxIter, PhiB, PhiC, PhiT, PhiV, PhiCA, CheckDefl, DLRat, SDLAndLLRat, LLRat, TotalRat, NetRat, SeisZone, ImpFactor) VALUES " +
                    "(\"" + thDesign + "\", \"" + FrameType + "\", " + obj.PatLLF + ", " + obj.SRatioLimit + ", " + obj.MaxIter + ", " + obj.PhiB + ", " + obj.PhiC + ", " + obj.PhiT + ", " + obj.PhiV + ", " +
                    obj.PhiCA + ", " + CodeYN(obj.CheckDefl) + ", " + obj.DLRat + ", " + obj.SDLAndLLRat + ", " + obj.LLRat + ", " + obj.TotalRat + ", " + obj.NetRat + ", \"" + zone + "\", " + obj.ImpFactor + ");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set SteelCode=\"UBC97-LRFD\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, UBC97_Conc obj)
        {
            string thDesign = obj.GetTHDesignName(obj.THDesign);
            //string FrameType = "\"OMF\"";
            string sql = "INSERT INTO [Preferences - Concrete Design - UBC97] " +
                    "(THDesign, NumCurves, NumPoints, MinEccen, PatLLF, UFLimit, PhiB, PhiCTied, PhiCSpiral, PhiV) VALUES " +
                    "(\"" + thDesign + "\", " + obj.NumCurves + ", " + obj.NumPoints + ", " + CodeYN(obj.MinEccen) + ", " + obj.PatLLF + ", " + obj.UFLimit + ", " + obj.PhiB + ", " + obj.PhiCTied + ", " + obj.PhiCSpiral + ", " + obj.PhiV + ");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set ConcCode=\"UBC97\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void store(OleDbConnection cn, ASD01 obj)
        {
            string thDesign = (obj.TimeHistoryDesign == THDesignOptions.Envelopes) ? "\"Envelopes\"" : "\"Step-by-Step\"";
            string FrameType = "\"OMF\"";
            string sql = "INSERT INTO [Preferences - Steel Design - AISC-ASD01] " +
                    "(THDesign, FrameType, PatLLF, SRatioLimit, MaxIter, SeisCat, SeisCode, SeisLoad, PlugWeld, CheckDefl, DLRat, SDLAndLLRat, LLRat, TotalRat, NetRat) VALUES " +
                    "(" + thDesign + ", " + FrameType + ", " + obj.PatLLF + ", " + obj.SRatioLimit + ", " + obj.MaxIter + ", \"" + obj.SeisCat + "\", " + CodeYN(obj.SeisCode) + ", " + CodeYN(obj.SeisLoad) + ", " + CodeYN(obj.PlugWeld) + ", " + CodeYN(obj.CheckDefl) + ", " + obj.DLRat + ", " + obj.SDLAndLLRat + ", " + obj.LLRat + ", " + obj.TotalRat + ", " + obj.NetRat + ");";

            new OleDbCommand(sql, cn).ExecuteNonQuery();

            sql = "UPDATE [Program Control] set SteelCode=\"AISC-ASD01\" WHERE 1=1;";
            new OleDbCommand(sql, cn).ExecuteNonQuery();
        }

        private void storeFrameDesignProcedures(OleDbConnection cn, Canguro.Model.Model model)
        {
            if (model.SteelDesignOptions is NoDesign
                && model.ConcreteDesignOptions is NoDesign
                //&& model.AluminumDesignOptions is NoDesign
                //&& model.ColdFormedDesignOptions is NoDesign
                )
                return;

            foreach (LineElement e in model.LineList)
            {
                if (e != null)
                {
                    string sql = "INSERT INTO [Frame Design Procedures] (Frame, DesignProc) VALUES (\"" + e.Id + "\", \"From Material\");";
                    new OleDbCommand(sql, cn).ExecuteNonQuery();
                }
            }

        }

        private void store(OleDbConnection cn, IList<ResponseSpectrum> spectra, IList<AbstractCase> aCases)
        {
            bool[] used = new bool[spectra.Count];

            for (int i = 0; i < used.Length; i++)
                used[i] = false;

            foreach (AbstractCase ac in aCases)
                if (ac is AnalysisCase && ((AnalysisCase)ac).Properties is ResponseSpectrumCaseProps)
                    for (int i = 0; i < used.Length; i++)
                        if (((ResponseSpectrumCaseProps)((AnalysisCase)ac).Properties).ResponseSpectrumFunction.ToString().Equals(spectra[i].ToString()))
                            used[i] = true;

            for (int i = 0; i < used.Length; i++)
                if (used[i])
                    store(cn, spectra[i]);
        }

        private void store(OleDbConnection cn, ResponseSpectrum spectrum)
        {
            float[,] func = spectrum.Function;
            string sql;
            for (int i = 0; i < func.GetLength(0); i++)
            {
                if (i == 0)
                    sql = "INSERT INTO [Function - Response Spectrum - User] " +
                        "([Name], Period, Accel, FuncDamp) VALUES " +
                        "(\"" + spectrum.ToString() + "\"," + func[i, 0] + ", " + func[i, 1] + ", 0.05);";
                else
                    sql = "INSERT INTO [Function - Response Spectrum - User] " +
                        "([Name], Period, Accel) VALUES " +
                        "(\"" + spectrum.ToString() + "\"," + func[i, 0] + ", " + func[i, 1] + ");";

                new OleDbCommand(sql, cn).ExecuteNonQuery();
            }
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

        private string CodeYN(bool val)
        {
            return (val) ? "\"Yes\"" : "\"No\"";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string encode(AccelLoad.AccelLoadValues value)
        {
            switch (value)
            {
                case AccelLoad.AccelLoadValues.UX: return "Accel U1";
                case AccelLoad.AccelLoadValues.UY: return "Accel U2";
                case AccelLoad.AccelLoadValues.UZ: return "Accel U3";
                case AccelLoad.AccelLoadValues.RX: return "Accel R1";
                case AccelLoad.AccelLoadValues.RY: return "Accel R2";
                case AccelLoad.AccelLoadValues.RZ: return "Accel R3";
            }
            return "";
        }
    }
}