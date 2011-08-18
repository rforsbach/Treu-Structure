using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Canguro.Model.Undo;
using Canguro.Model.Load;
using Canguro.Model.Material;
using Canguro.Model.Results;
using Canguro.Model.Section;
using Canguro.Model.UnitSystem;
using Canguro.Model.Design;

namespace Canguro.Model
{
    sealed public class Model : IModel
    {
        /// <summary>
        /// Destruye todo el modelo y lo deja limpio
        /// </summary>
        /// <TODO>Modified, Save, Load</TODO>
        public void Reset()
        {
            try
            {
                this.undoManager = new UndoManager(this);
                undoManager.Enabled = false;
                this.abstractCases = new ManagedList<Canguro.Model.Load.AbstractCase>();
                abstractCases.ElementRemoved += new ManagedList<AbstractCase>.ListChangedEventHandler(abstractCases_ElementRemoved);
                this.activeLoadCase = null;
                this.areaList = new ItemList<AreaElement>();
                this.constraintList = new ManagedList<Constraint>();
                this.isLocked = false;
                this.jointList = new ItemList<Joint>();
                this.layers = new ItemList<Layer>();
                layers.ElementRemoved += new ManagedList<Layer>.ListChangedEventHandler(layers_ElementRemoved);
                this.lineList = new ItemList<LineElement>();
                this.loadCases = new ManagedDictionary<string, LoadCase>();
                loadCases.ElementRemoved += new ManagedDictionary<string, LoadCase>.ListChangedEventHandler(loadCases_ElementRemoved);
                this.summary = new ModelSummary(this);

                this.designOptions = new List<DesignOptions>();
                designOptions.Add(NoDesign.Instance);
                designOptions.Add(new LRFD99());
                designOptions.Add(new ACI318_02());
                designOptions.Add(new ASD01());
                designOptions.Add(new RCDF2001());
                designOptions.Add(new UBC97_ASD());
                designOptions.Add(new UBC97_LRFD());
                designOptions.Add(new UBC97_Conc());
                steelDesignOptions = NoDesign.Instance;
                concreteDesignOptions = NoDesign.Instance;
                coldFormedDesignOptions = NoDesign.Instance;
                aluminumDesignOptions = NoDesign.Instance;

                this.results = new Canguro.Model.Results.Results(0);

                // Layer es un Item y todos los Items asignan su propiedad layer
                // de acuerdo a ActiveLayer, por lo que hay que asignarla en null
                // antes de crear el primer Layer, root de todos los demás
                activeLayer = null;
                Layer rootLayer = new Layer(Culture.Get("defaultLayerName"));
                ActiveLayer = rootLayer;

                activeLoadCase = new LoadCase(Culture.Get("defaultLoadCase"), LoadCase.LoadCaseType.Dead);
                activeLoadCase.SelfWeight = 1.0f;
                loadCases.Add(activeLoadCase.Name, activeLoadCase);

                AnalysisCase anc = new Canguro.Model.Load.AnalysisCase(Culture.Get("defaultLoadCase"));
                AbstractCases.Add(anc);
                if (anc != null)
                {
                    StaticCaseProps props = anc.Properties as StaticCaseProps;
                    if (props != null)
                    {
                        List<StaticCaseFactor> list = props.Loads;
                        list.Add(new StaticCaseFactor(ActiveLoadCase));
                        props.Loads = list;
                    }
                }

                MaterialManager.Instance.Initialize();
                SectionManager.Instance.Initialize(ref sections);
                sections.ElementRemoved += new Catalog<Canguro.Model.Section.Section>.ListChangedEventHandler(sections_ElementRemoved);
                this.currentPath = "";
                foreach (Canguro.Model.UnitSystem.UnitSystem us in UnitSystemsManager.Instance.UnitSystems)
                    if (Properties.Settings.Default.UnitSystem.Equals(us.GetType().Name))
                        UnitSystemsManager.Instance.CurrentSystem = us;

                viewManager = Canguro.View.GraphicViewManager.Instance;
                modified = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw e;
            }
            finally
            {
                if (ModelReset != null)
                    ModelReset(this, EventArgs.Empty);
                undoManager.Enabled = true;
            }
        }

        void sections_ElementRemoved(object sender, ListChangedEventArgs<Canguro.Model.Section.Section> args)
        {
            foreach (LineElement line in lineList)
                if (line != null && line.Properties is StraightFrameProps &&
                    ((StraightFrameProps)line.Properties).Section == args.ChangedObject)
                {
                    args.Cancel = true;
                    break;
                }

        }

        void loadCases_ElementRemoved(object sender, ListChangedEventArgs<string> args)
        {
            if (LoadCases.Count <= 1)
                args.Cancel = true;
            else
            {
                string name = args.ChangedObject;
                LoadCase lCase = LoadCases[name];
                foreach (Element element in JointList)
                    if (element != null && element.Loads != null)
                        element.Loads.Remove(lCase);
                foreach (Element element in LineList)
                    if (element != null && element.Loads != null)
                        element.Loads.Remove(lCase);

                if (ActiveLoadCase == lCase)
                {
                    foreach (Canguro.Model.Load.LoadCase lc in LoadCases.Values)
                    {
                        ActiveLoadCase = lc;
                        break;
                    }
                }
            }
        }

        void layers_ElementRemoved(object sender, ListChangedEventArgs<Layer> args)
        {
            args.Cancel = (layers.CountNotNull() <= 1);
        }

        #region AbstractCases Consistency

        /// <summary>
        /// Callback function called before an abstract case is deleted. 
        /// Cancels the removing if the case has dependencies.
        /// </summary>
        /// <param name="sender">The AbstractCases Managed List</param>
        /// <param name="args">Object to check the Abstract case being deleted and to cancel if needed</param>
        void abstractCases_ElementRemoved(object sender, ListChangedEventArgs<AbstractCase> args)
        {
            Dictionary<AbstractCase, LinkedList<AbstractCase>> adjacency = BuildAnalysisCaseAdjacency();
            bool cancel = false;
            // Cancel if there are dependencies.
            if (adjacency.ContainsKey(args.ChangedObject) && adjacency[args.ChangedObject].Count > 0)
                cancel = true;
            else
            {
                // Deactivate before removing
                args.ChangedObject.IsActive = false;
                // Cancel remove if can't deactivate
                cancel = args.ChangedObject.IsActive;
            }
            args.Cancel = cancel;
        }

        /// <summary>
        /// Repairs the IsActive state of the abstract cases so that no active case depends upon an unactive case
        /// </summary>
        /// <param name="changedAc">The last abstract case that has changed</param>
        public void RepairAbstractCases(AbstractCase changedAc)
        {
            if (!changedAc.IsActive)
            {
                // If IsActive == false then deactivate all dependant cases
                Dictionary<AbstractCase, LinkedList<AbstractCase>> adjacency = BuildAnalysisCaseAdjacency();
                repairAbstractCases(changedAc, adjacency);
            }
        }

        private void repairAbstractCases(AbstractCase changedAc, Dictionary<AbstractCase, LinkedList<AbstractCase>> adjacency)
        {
            if (adjacency.ContainsKey(changedAc))
            {
                foreach (AbstractCase ac in adjacency[changedAc])
                {
                    if (ac.IsActive)
                    {
                        ac.IsActive = false;
                        repairAbstractCases(ac, adjacency);
                    }
                }
            }
        }

        /// <summary>
        /// Builds an adjacencyList of dependant abstract cases, so that each entry has its dependant cases in its list
        /// </summary>
        /// <returns>The adjacency list</returns>
        public Dictionary<AbstractCase, LinkedList<AbstractCase>> BuildAnalysisCaseAdjacency()
        {
            Dictionary<AbstractCase, LinkedList<AbstractCase>> adjancencyList = new Dictionary<AbstractCase, LinkedList<AbstractCase>>();

            AnalysisCase aCase = null;
            LoadCombination combo = null;

            foreach (AbstractCase ac in abstractCases)
            {
                if ((aCase = ac as AnalysisCase) != null)
                {
                    if (aCase.Properties.DependsOn == null)
                    {
                        if (!adjancencyList.ContainsKey(aCase))
                            adjancencyList.Add(aCase, new LinkedList<AbstractCase>());
                    }
                    else if (adjancencyList.ContainsKey(aCase.Properties.DependsOn))
                        adjancencyList[aCase.Properties.DependsOn].AddLast(aCase);
                    else
                    {
                        adjancencyList.Add(aCase.Properties.DependsOn, new LinkedList<AbstractCase>());
                        adjancencyList[aCase.Properties.DependsOn].AddLast(aCase);
                    }
                }
                else if ((combo = ac as LoadCombination) != null)
                {
                    foreach (AbstractCaseFactor acf in combo.Cases)
                    {
                        if (adjancencyList.ContainsKey(acf.Case))
                            adjancencyList[acf.Case].AddLast(combo);
                        else
                        {
                            adjancencyList.Add(acf.Case, new LinkedList<AbstractCase>());
                            adjancencyList[acf.Case].AddLast(combo);
                        }
                    }
                }
            }

            return adjancencyList;
        }
        #endregion

        /// <summary>
        /// Por este medio se le avisa a Model que están occurriendo cambios.
        /// Lanza el evento ModelChanged
        /// </summary>
        public void ChangeModel()
        {
            if (ModelChanged != null)
                this.ModelChanged(this, EventArgs.Empty);
        }

        internal void ChangeModel(bool gadgetsChanged)
        {
            if (gadgetsChanged)
                View.ResourceManager.GadgetManager.Reset();
            ChangeModel();
        }

        /// <summary>
        /// Method to inform the model that new results have arrived
        /// Throws the event ResultsArrived
        /// </summary>
        public void NewResults()
        {
            if (ResultsArrived != null && HasResults)
                this.ResultsArrived(this, EventArgs.Empty);
        }

        /// <summary>
        /// Por este medio se le avisa a Model que la selección cambió.
        /// Lanza el evento SelectionChanged
        /// </summary>
        public void ChangeSelection(Item picked)
        {
            if (SelectionChanged != null)
                this.SelectionChanged(this, new SelectionChangedEventArgs(picked));
        }

        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
        public class SelectionChangedEventArgs : EventArgs
        {
            public readonly Item Picked;

            public SelectionChangedEventArgs(Item picked)
            {
                Picked = picked;
            }
        }

        // Gets fired when the model changes (except selection changes)
        public event EventHandler ModelChanged;

        /// <summary>
        /// Event thrown when new results have arrived 
        /// (i.e. a new Results Case was just downloaded and parsed into Results).
        /// The sender sent is Model.Results
        /// </summary>
        public event EventHandler ResultsArrived;

        /// <summary>
        /// Gets fired when the selection changes
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets fired after the model resets itself
        /// </summary>
        public event EventHandler ModelReset;

        private Model()
        {
            ResponseSpectra = ResponseSpectrum.ReadDirectory();
            if (ResponseSpectra.Count > 0)
                DefaultResponseSpectrum = ResponseSpectra[0];
            foreach (ResponseSpectrum rs in ResponseSpectra)
                if ("Unit".Equals(rs.ToString()))
                    DefaultResponseSpectrum = rs;
        }

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        public static readonly Model Instance = new Model();
        private ManagedList<Load.AbstractCase> abstractCases;
        private ItemList<AreaElement> areaList;
        private ManagedList<Constraint> constraintList;
        private ItemList<Joint> jointList;
        private ItemList<LineElement> lineList;
        private Results.Results results;
        private Catalog<Section.Section> sections;
        private UndoManager undoManager;
        private Canguro.View.GraphicViewManager viewManager;
        private ManagedDictionary<string, LoadCase> loadCases;
        private LoadCase activeLoadCase;
        private bool isLocked;
        private bool modified;
        private ItemList<Layer> layers;
        private Layer activeLayer;
        private List<DesignOptions> designOptions;
        private DesignOptions steelDesignOptions;
        private DesignOptions concreteDesignOptions;
        private DesignOptions coldFormedDesignOptions;
        private DesignOptions aluminumDesignOptions;
        private ModelSummary summary;
        public readonly IList<ResponseSpectrum> ResponseSpectra;
        public readonly ResponseSpectrum DefaultResponseSpectrum;

        /// <summary>
        /// Gets a self consistent list of joints.
        /// </summary>
        public Canguro.Model.ItemList<Joint> JointList
        {
            get
            {
                return jointList;
            }
        }

        /// <summary>
        /// Gets a self consistent list of linear elements.
        /// </summary>
        public Canguro.Model.ItemList<LineElement> LineList
        {
            get
            {
                return lineList;
            }
        }

        /// <summary>
        /// Gets a self consistent list of 2D elements.
        /// </summary>
        public Canguro.Model.ItemList<AreaElement> AreaList
        {
            get
            {
                return areaList;
            }
        }

        /// <summary>
        /// Gets the user catalog of sections
        /// </summary>
        public Catalog<Section.Section> Sections
        {
            get
            {
                return sections;
            }
        }

        /// <summary>
        /// Gets a list of constraints.
        /// </summary>
        public ManagedList<Constraint> ConstraintList
        {
            get
            {
                return constraintList;
            }
        }

        /// <summary>
        /// Returns true when the model is analyzed
        /// </summary>
        public bool HasResults
        {
            get { return (results != null && results.AnalysisID != 0); }
        }

        /// <summary>
        /// Gets a Results object with the analysis results.
        /// </summary>
        public Results.Results Results
        {
            get
            {
                return results;
            }
            internal set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (value.AnalysisID == 0)
                    throw new ArgumentException();
               
                //if (!(Controller.Controller.Instance.ModelCommand is Commands.Model.AnalysisCmd))
                //    throw new InvalidCallException("Cannot set Results from outside of AnalysisCmd");

                results = value;
            }
        }

        /// <summary>
        /// Gets the UndoManager, which has a list of all the most resent actions.
        /// </summary>
        public UndoManager Undo
        {
            get
            {
                return undoManager;
            }
        }

        /// <summary>
        /// Gets the GraphicViewManager object to handle the view state.
        /// </summary>
        public Canguro.View.GraphicViewManager View
        {
            get
            {
                return viewManager;
            }
        }

        /// <summary>
        /// Gets a list of AbstractCases (Analysis Cases + Combinations)
        /// </summary>
        public ManagedList<Canguro.Model.Load.AbstractCase> AbstractCases
        {
            get
            {
                return abstractCases;
            }
        }

        /// <summary>
        /// Gets or sets the current Unit System
        /// </summary>
        public UnitSystem.UnitSystem UnitSystem
        {
            get
            {
                return Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
            }
            set
            {
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem = value;
            }
        }

        /// <summary>
        /// Gets a list with all the Load Cases
        /// </summary>
        public ManagedDictionary<string, LoadCase> LoadCases
        {
            get
            {
                return loadCases;
            }
        }

        /// <summary>
        /// Gets or sets the Active Load Case. All loads added will be included in the this.
        /// </summary>
        public Load.LoadCase ActiveLoadCase
        {
            get
            {
                return activeLoadCase;
            }
            set
            {
                if ((value != null) && !(loadCases.ContainsValue(value)))
                    loadCases.Add(value.Name, value);
                if (value != activeLoadCase)
                {
                    activeLoadCase = value;
                    ChangeModel();
                }
            }
        }

        /// <summary>
        /// Gets or sets the locked status. If results are available and IsLocked is set to false, the results are deleted.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return isLocked;
            }
            set
            {
                if (!value && HasResults)
                {
                    Canguro.View.GraphicViewManager.Instance.Reset(false);
                    results = new Canguro.Model.Results.Results(0);
                }
                isLocked = value;
                ChangeModel();
            }
        }

        /// <summary>
        /// Gets or sets the Modified value. 
        /// Calue is true when the Model has been modified since the last save. 
        /// It can only be set to true. Throws an InvalidCallException otherwise.
        /// </summary>
        public bool Modified
        {
            get
            {
                return modified;
            }
            set
            {
                if (!value)
                    throw new InvalidCallException("Modified can only be set to true");
                if (Model.Instance.IsLocked)
                {
                    if (!ConfirmUnlockModel())
                        throw new ModelIsLockedException();
                }
                modified = true;
            }
        }

        /// <summary>
        /// Opens a dialog to confirm unlocking the model.
        /// </summary>
        /// <returns></returns>
        public bool ConfirmUnlockModel()
        {
            if (!IsLocked)
                return true;

            System.Windows.Forms.DialogResult res = System.Windows.Forms.MessageBox.Show(Culture.Get("confirmUnlockModel"), Culture.Get("confirm"), 
                System.Windows.Forms.MessageBoxButtons.OKCancel);
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                IsLocked = false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets a self consistent list of Layer objects.
        /// </summary>
        public ItemList<Layer> Layers
        {
            get
            {
                return layers;
            }
        }

        /// <summary>
        /// Gets the current path, where the file is saved.
        /// </summary>
        public string CurrentPath
        {
            get { return currentPath; }
        }

        /// <summary>
        /// Gets or sets all the available Design Options.
        /// </summary>
        public List<DesignOptions> DesignOptions
        {
            get { return designOptions; }
            set { designOptions = value; }
        }

        /// <summary>
        /// Gets or sets the Design options for steel design
        /// </summary>
        public DesignOptions SteelDesignOptions
        {
            get { return steelDesignOptions; }
            set 
            {
                Undo.Change(this, steelDesignOptions, GetType().GetProperty("SteelDesignOptions"));
                steelDesignOptions = (value is SteelDesignOptions || value is NoDesign) ? value : steelDesignOptions; 
            }
        }

        /// <summary>
        /// Gets or sets the Design options for concrete design
        /// </summary>
        public DesignOptions ConcreteDesignOptions
        {
            get { return concreteDesignOptions; }
            set
            {
                Undo.Change(this, steelDesignOptions, GetType().GetProperty("ConcreteDesignOptions")); 
                concreteDesignOptions = (value is ConcreteDesignOptions || value is NoDesign) ? value : concreteDesignOptions;
            }
        }

        /// <summary>
        /// Gets or sets the Design options for cold formed steel design
        /// </summary>
        public DesignOptions ColdFormedDesignOptions
        {
            get { return coldFormedDesignOptions; }
            set
            {
                Undo.Change(this, steelDesignOptions, GetType().GetProperty("ColdFormedDesignOptions")); 
                coldFormedDesignOptions = (value is ColdFormedDesignOptions || value is NoDesign) ? value : coldFormedDesignOptions;
            }
        }

        /// <summary>
        /// Gets or sets the Design options for aluminum design
        /// </summary>
        public DesignOptions AluminumDesignOptions
        {
            get { return aluminumDesignOptions; }
            set
            {
                Undo.Change(this, steelDesignOptions, GetType().GetProperty("AluminumDesignOptions"));
                aluminumDesignOptions = (value is AluminumDesignOptions || value is NoDesign) ? value : aluminumDesignOptions;
            }
        }

        /// <summary>
        /// Gets the ModelSummary object
        /// </summary>
        public ModelSummary Summary
        {
            get { return summary; }
        }

        /// <summary>
        /// Gets or sets the Active layer. All items are added to the active layer.
        /// </summary>
        public Layer ActiveLayer
        {
            get
            {
                return activeLayer;
            }
            set
            {
                if (value != null && value != activeLayer)
                {
                    if (!(layers.Contains(value)))
                        layers.Add(value);

                    //Undo.Change(this, activeLayer, GetType().GetProperty("ActiveLayer"));
                    activeLayer = value;
                }
            }
        }

        /// <summary>
        /// Deselects all joints, line and area elements.
        /// </summary>
        public void UnSelectAll()
        {
            if (jointList != null)
                foreach (Item i in jointList)
                    if (i != null) i.IsSelected = false;
            if (lineList != null)
                foreach (Item i in lineList)
                    if (i != null) i.IsSelected = false;
            if (areaList != null)
                foreach (Item i in areaList)
                    if (i != null) i.IsSelected = false;
            ChangeSelection(null);
        }

        private string currentPath = "";

        //[Obsolete("Use SaveModelCmd")]
        //public void Save()
        //{
        //    string path = "";
        //    if (currentPath.Length == 0)
        //    {
        //        System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
        //        dlg.Filter = "Treu Structure Model (*.tsm)|*.tsm";
        //        dlg.DefaultExt = "tsm";
        //        dlg.AddExtension = true;
        //        dlg.FileName = currentPath;
        //        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            path = dlg.FileName;
        //    }
        //    else
        //        path = currentPath;
        //    if (path.Length > 0)
        //        Save(path);
        //}

        //[Obsolete("Use LoadModelCmd")]
        //public void Load()
        //{
        //    string path = "";
        //    System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
        //    dlg.Filter = "Treu Structure Model (*.tsm)|*.tsm";
        //    dlg.DefaultExt = "tsm";
        //    dlg.AddExtension = true;
        //    if (currentPath.Length > 0)
        //        dlg.FileName = Path.GetDirectoryName(currentPath);
        //    dlg.CheckPathExists = true;
        //    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        path = dlg.FileName;
        //    if (path.Length > 0)
        //        Load(path);
        //}

        /// <summary>
        /// Serializes and saves the Model to a file.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public void Save(string path)
        {
//            Stream stream = null;
            try
            {
                new Serializer.Serializer(this).Serialize(path);
                //stream = File.Open(path, FileMode.Create);
                //BinaryFormatter bformatter = new BinaryFormatter();
                //bformatter.Serialize(stream, "version=7.11");
                //bformatter.Serialize(stream, this.activeLayer.Id);
                //bformatter.Serialize(stream, this.layers);

                //this.sections.Save(stream, bformatter);

                //bformatter.Serialize(stream, this.loadCases);
                //bformatter.Serialize(stream, this.abstractCases);

                //bformatter.Serialize(stream, this.jointList);
                //bformatter.Serialize(stream, this.constraintList);
                //bformatter.Serialize(stream, this.lineList);
                //bformatter.Serialize(stream, this.areaList);

                //bformatter.Serialize(stream, this.isLocked);
                //bformatter.Serialize(stream, this.results);

                //bformatter.Serialize(stream, this.designOptions);
                //bformatter.Serialize(stream, this.steelDesignOptions.ToString());
                //bformatter.Serialize(stream, this.concreteDesignOptions.ToString());
                //bformatter.Serialize(stream, this.aluminumDesignOptions.ToString());
                //bformatter.Serialize(stream, this.coldFormedDesignOptions.ToString());

                currentPath = path;
                this.modified = false;
            }
            catch (Exception)
            {
                currentPath = "";
                System.Windows.Forms.MessageBox.Show(Culture.Get("cantSaveFile") + " " + path);
            }
            //finally
            //{
            //    if (stream != null)
            //        stream.Close();
            //}
        }
        
        /// <summary>
        /// Reads a file with a serialized model.
        /// </summary>
        /// <param name="path">Path to the .tsm file</param>
        public void Load(string path)
        {
            Stream stream = null;
            try
            {
                if (ModelReset != null)
                    ModelReset(this, EventArgs.Empty);
                new Serializer.Deserializer(this).Deserialize(path);
            //    stream = File.Open(path, FileMode.Open);
            //    BinaryFormatter bformatter = new BinaryFormatter();
            //    undoManager.Enabled = false;
            //    string version = (string)bformatter.Deserialize(stream);
            //    uint activeLayerID = (uint)bformatter.Deserialize(stream);
            //    this.layers = (ItemList<Layer>)bformatter.Deserialize(stream);
            //    activeLayer = layers[activeLayerID];

            //    this.sections = SectionManager.Instance["modelCatalog"];
            //    this.sections.Load(stream, bformatter);
            //    if (sections.Count == 0)
            //        sections[SectionManager.Instance.DefaultSection.Name] = SectionManager.Instance.DefaultSection;

            //    Dictionary<string, LoadCase> lcTemp = (Dictionary<string, LoadCase>)bformatter.Deserialize(stream);
            //    this.loadCases = new ManagedDictionary<string, LoadCase>(lcTemp);

            //    List<AbstractCase> acList = (List<AbstractCase>)bformatter.Deserialize(stream);
            //    this.abstractCases = new ManagedList<AbstractCase>(acList);
            //    abstractCases.ElementRemoved += new ManagedList<AbstractCase>.ListChangedEventHandler(abstractCases_ElementRemoved);

            //    this.jointList = (ItemList<Joint>)bformatter.Deserialize(stream);
            //    List<Constraint> conList = (List<Constraint>)bformatter.Deserialize(stream);
            //    this.constraintList = new ManagedList<Constraint>(conList);

            //    this.lineList = (ItemList<LineElement>)bformatter.Deserialize(stream);
            //    this.areaList = (ItemList<AreaElement>)bformatter.Deserialize(stream);

            //    this.isLocked = (bool)bformatter.Deserialize(stream);
            //    this.results = (Canguro.Model.Results.Results)bformatter.Deserialize(stream);

            //    if (stream.Position < stream.Length)
            //    {
            //        designOptions = (List<DesignOptions>)bformatter.Deserialize(stream);

            //        string steel = (string)bformatter.Deserialize(stream);
            //        string conc = (string)bformatter.Deserialize(stream);
            //        string alum = (string)bformatter.Deserialize(stream);
            //        string cold = (string)bformatter.Deserialize(stream);
            //        foreach (DesignOptions opt in designOptions)
            //        {
            //            if (opt.ToString().Equals(steel))
            //                steelDesignOptions = opt;
            //            else if (opt.ToString().Equals(conc))
            //                concreteDesignOptions = opt;
            //            else if (opt.ToString().Equals(alum))
            //                aluminumDesignOptions = opt;
            //            else if (opt.ToString().Equals(cold))
            //                coldFormedDesignOptions = opt;
            //        }
            //    }

            //    foreach (LoadCase current in loadCases.Values)
            //    {
            //        activeLoadCase = current;
            //        break;
            //    }

                new Canguro.Model.Serializer.ModelRepairer().Repair(this);

                currentPath = path;
                ChangeModel();
                undoManager = new UndoManager(this);
                undoManager.Enabled = true;

                // If Results exists and there's no ModelCommand currently in execution, run GetResultsCmd
                // If a ModelCommand called Load, then it's the responsibility of that command to call GetResultsCommand
                if (results.AnalysisID != 0 && !results.Finished && Controller.Controller.Instance.ModelCommand == null)
                    Controller.Controller.Instance.Execute("getresults");

                this.modified = false;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(Culture.Get("fileNotFound") + ": '" + path + "'");
                throw e;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Creates and returns a connectivity graph in the form of an adjacency list.
        /// Each joint is a vertex in the garph and each element is one or more (areas) edges.
        /// </summary>
        /// <returns></returns>
        public List<LinkedList<int>> GetConnectivityGraph()
        {
            List<LinkedList<int>> list = new List<LinkedList<int>>(jointList.Count);
            for (int i = 0; i < jointList.Count; i++)
                list.Add(null);

            foreach (LineElement element in lineList)
            {
                if (element != null && element.I != null && element.J != null)
                {
                    int i = (int)element.I.Id;
                    int j = (int)element.J.Id;
                    if (list[i] == null)
                        list[i] = new LinkedList<int>();
                    if (list[j] == null)
                        list[j] = new LinkedList<int>();
                    list[i].AddLast(j);
                    list[j].AddLast(i);
                }
            }

            return list;
        }
    }
}
