using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Controller.Grid;
using Canguro.Properties;

namespace Canguro
{
    public partial class MainFrm : Form
    {
        private Canguro.Commands.Forms.CommandToolbox commandToolbox = null;
        private Canguro.View.ScenePanel scenePanel;

        /// <summary>
        /// Public constructor. Called by Program.Main()
        /// </summary>
        public MainFrm()
        {
            InitializeComponent();

            this.scenePanel = new Canguro.View.ScenePanel();
            this.scenePanel.SuspendLayout();
            this.gridSplit.Panel2.Controls.Add(this.scenePanel);
            // 
            // scenePanel
            // 
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));

            this.scenePanel.Controls.Add(this.smallPanel);
            resources.ApplyResources(this.scenePanel, "scenePanel");
            this.scenePanel.Name = "scenePanel";
            this.scenePanel.ResumeLayout(false);
            scenePanel.Dock = DockStyle.Fill;
            jointGridView.SelectionChanged += new EventHandler(GridView_SelectionChanged);
            lineGridView.SelectionChanged += new EventHandler(GridView_SelectionChanged);
            //areaGridView.SelectionChanged += new EventHandler(GridView_SelectionChanged);
        }

        /// <summary>
        /// Method called when an item is selected / deselected. Updates the grid.
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        void GridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateGridToolStrips();
        }

        /// <summary>
        /// Gets the DirectX Panel where all drawing will occur
        /// </summary>
        public View.ScenePanel ScenePanel
        {
            get
            {
                return scenePanel;
            }
        }

        /// <summary>
        /// Gets the main command prompt panel, which is used as the main
        /// typed user input entrance for the commands.
        /// </summary>
        public View.SmallPanel SmallPanel
        {
            get
            {
                return smallPanel;
            }
        }

        /// <summary>
        /// Gets the data grids.
        /// </summary>
        /// <returns>An array with the joints and lines grids</returns>
        public DataGridView[] GetGridViews()
        {
            DataGridView[] gvs = new DataGridView[2];

            gvs[0] = jointGridView;
            gvs[1] = lineGridView;
            //gvs[2] = areaGridView;

            return gvs;
        }

        public void SetActiveGridTab(int index)
        {
            gridTabs.SelectTab(index);
        }

        #region MainFrm events

        /// <summary>
        /// Method called when a key is pressed. Ignores all keys.
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void MainFrm_KeyPress(object sender, KeyPressEventArgs e)
        {
#if DEBUG
            if (e.KeyChar == (char)27)
                e.Handled = true;
#endif
        }

        /// <summary>
        /// Called when the window is closed. Cancels the current command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Controller.Controller.Instance.Execute("cancel");
            }
            catch (Exception)
            {
                if (Controller.Controller.Instance.ModelCommand != null)
                {
                    Controller.Controller.Instance.ModelCommand.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Initialization code
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void MainFrm_Load(object sender, EventArgs e) {
            int width = Screen.AllScreens[0].WorkingArea.Width;
            int height = Screen.AllScreens[0].WorkingArea.Height;
            this.SetDesktopLocation((width / 2) - (this.Size.Width / 2), (height / 2) - (this.Size.Height / 2));

            if (Settings.Default.WindowMaximazed.Equals("Yes"))
                this.WindowState = FormWindowState.Maximized;
            else
            this.Size = Settings.Default.WindowSize;           
            Controller.Controller.Instance.StartViewCommand += new EventHandler(Controller_CommandChanged);
            Controller.Controller.Instance.EndViewCommand += new Canguro.Controller.Controller.EndViewCommandEventHandler(Controller_EndViewCommand);
            Controller.Controller.Instance.StartModelCommand += new EventHandler(Controller_CommandChanged);
            Controller.Controller.Instance.EndModelCommand += new EventHandler(Controller_CommandChanged);
            UpdateArea(Canguro.Controller.UpdateAreaEvent.ModelReset, Canguro.Model.Model.Instance, null);
        }

        /// <summary>
        /// Updates the window according to the type of UpdateAreaEvent parameters.
        /// </summary>
        /// <param name="areaEvent">Defines what kind of change ocurred</param>
        /// <param name="model">The current Model object</param>
        /// <param name="view">The current view</param>
        public void UpdateArea(Controller.UpdateAreaEvent areaEvent, Model.Model model, View.GraphicView view)
        {
            if ((areaEvent & Controller.UpdateAreaEvent.ResultsArrived) > 0)
            {
                if (model.Results.Finished)
                {
                    resultsCasesCombo.Items.Clear();
                    List<Model.Results.ResultsCase> rcList = model.Results.ResultsCases;

                    foreach (Model.Results.ResultsCase rc in rcList)
                        if (rc != null)
                            resultsCasesCombo.Items.Add(rc);

                    if (!resultsToolStrip.Visible)
                        resultsToolStrip.Visible = true;
                }
            }

            if ((areaEvent & Canguro.Controller.UpdateAreaEvent.ModelChanged) > 0)
            {
                if (model.HasResults)
                {
                    if (model.Results.ActiveCase != null)
                        resultsCasesCombo.SelectedItem = model.Results.ActiveCase;
                }

                // ComboBoxs
                UpdateLayersCombo();
                UpdateLoadCasesCombo();
                UpdateUnitsCombo();
                UpdateToolBar(model);
            }

            if ((areaEvent & Canguro.Controller.UpdateAreaEvent.ModelReset) > 0)
            {
                jointGridView.DataSource = model.JointList;
                lineGridView.DataSource = model.LineList;
                //areaGridView.DataSource = model.AreaList;
            }

            if ((areaEvent & Canguro.Controller.UpdateAreaEvent.SelectionChanged) > 0)
                SetStatusLabel(null);
        }

        /// <summary>
        /// Callback executed when the Controller.EndViewCommand event is triggered.
        /// Updates the toolbar
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        void Controller_EndViewCommand(object sender, Canguro.Controller.Controller.EndViewCommandArgs e)
        {
            Controller_CommandChanged(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Callback executed when the Controller.CommandChanged event is triggered.
        /// Updates the toolbar
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        void Controller_CommandChanged(object sender, EventArgs e)
        {
            UpdateToolBar(Model.Model.Instance);
        }

        /// <summary>
        /// Updates all menues and toolbar to reflect the state of the Model
        /// </summary>
        /// <param name="model">Current Model object</param>
        public void UpdateToolBar(Model.Model model)
        {
            if (!IsDisposed)
            {
                UpdateFileMenu(model);
                UpdateEditMenu(model);
                UpdateViewMenu(model);
                UpdateModelMenu(model);
                UpdateLayersMenu(model);
                UpdateSelectMenu(model);
                UpdateAnalysisMenu(model);
                UpdateHelpMenu(model);
                UpdateCombosStatus(model);
                UpdateGridToolStrips();

                string title = (string.IsNullOrEmpty(model.CurrentPath)) ? Culture.Get("defaultModelName") :
                    System.IO.Path.GetFileNameWithoutExtension(model.CurrentPath);
                title = (model.Modified) ? title + "*" : title;
                Text = title + " - Treu Structure";
            }
        }

        /// <summary>
        /// Updates the jointGrid, lineGrid and areaGrid toolbars to reflect the state of the Model
        /// </summary>
        private void UpdateGridToolStrips()
        {
            jointsToolStrip.Visible = true;
            linesToolStrip.Visible = true;
            //areasToolStrip.Visible = true;

            int nJoints = jointGridView.SelectedObjects.Count;
            int nLines = lineGridView.SelectedObjects.Count;
            //int nAreas = areaGridView.SelectedObjects.Count;
            bool jointsSelected = (nJoints > 0);
            bool linesSelected = (nLines > 0);
            //bool areasSelected = (nAreas > 0);
            bool oneJoint = (nJoints == 1);
            bool oneLine = (nLines == 1);
            //bool oneArea = (nAreas == 1);

            jointFillDownButton.Enabled = oneJoint;
            lineFillDownButton.Enabled = oneLine;
            //areaFillDownButton.Enabled = oneArea;
            restraintAllButton.Enabled = jointsSelected;
            restraintFreeButton.Enabled = jointsSelected;
            restraintTransButton.Enabled = jointsSelected;
            restraintZButton.Enabled = jointsSelected;
            selectSimilarJointButton.Enabled = oneJoint;
            selectSimilarLineButton.Enabled = oneLine;
            //selectSimilarAreaButton.Enabled = oneArea;
            noReleaseButton.Enabled = linesSelected;
            axialReleaseButton.Enabled = linesSelected;
            momentReleaseButton.Enabled = linesSelected;
        }

        /// <summary>
        /// Updates the File menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateFileMenu(Model.Model model)
        {
            bool animating = View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated;
            fileEditToolStrip.Enabled = !animating;

            newModelToolStripMenuItem.Enabled = newButton.Enabled;
            openModelToolStripMenuItem.Enabled = openButton.Enabled;
            saveToolStripMenuItem.Enabled = saveButton.Enabled;
            saveAsToolStripMenuItem.Enabled = true;
            importDXFToolStripMenuItem.Enabled = true;
            exportDXFToolStripMenuItem.Enabled = true;
            exitToolStripMenuItem.Enabled = true;
        }


        /// <summary>
        /// Updates the Edit menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateEditMenu(Model.Model model)
        {
            bool notEmptyModel = (model.Summary.NumJoints > 0);
            bool notExecuting = (Controller.Controller.Instance.ModelCommand == null);
            bool hasCopied = (Clipboard.GetData("Canguro") is object[] || Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue));

            undoButton.Enabled = model.Undo.CanUndo && notExecuting;
            undoToolStripMenuItem.Enabled = undoButton.Enabled && notExecuting;
            redoButton.Enabled = model.Undo.CanRedo && notExecuting;
            redoToolStripMenuItem.Enabled = redoButton.Enabled && notExecuting;

            cutButton.Enabled = notEmptyModel && notExecuting;
            cutToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            copyButton.Enabled = notEmptyModel && notExecuting;
            copyToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            copyPasteButton.Enabled = notEmptyModel && notExecuting;
            copyPasteToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            pasteButton.Enabled = hasCopied && notExecuting;
            pasteToolStripMenuItem.Enabled = hasCopied && notExecuting;
            deleteButton.Enabled = notEmptyModel && notExecuting;
            deleteToolStripMenuItem.Enabled = notEmptyModel && notExecuting;

            moveButton.Enabled = notEmptyModel && notExecuting;
            moveToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            editButton.Enabled = notEmptyModel && notExecuting;
            editToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            mirrorButton.Enabled = notEmptyModel && notExecuting;
            mirrorToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            scaleButton.Enabled = notEmptyModel && notExecuting;
            scaleToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            arrayButton.Enabled = notEmptyModel && notExecuting;
            arrayToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            polararrayButton.Enabled = notEmptyModel && notExecuting;
            polarArrayToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            modelrotateButton.Enabled = notEmptyModel && notExecuting;
            rotateModelToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            splitButton.Enabled = notEmptyModel && notExecuting;
            splitToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            joinButton.Enabled = notEmptyModel && notExecuting;
            joinToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            intersectButton.Enabled = notEmptyModel && notExecuting;
            intersectToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            flipLineButton.Enabled = notEmptyModel && notExecuting;
            flipLineToolStripMenuItem.Enabled = notEmptyModel && notExecuting;
            constraintsButton.Enabled = notExecuting;
            constraintsToolStripMenuItem.Enabled = notExecuting;
            diaphragmsButton.Enabled = notEmptyModel && notExecuting;
            diaphragmsToolStripMenuItem.Enabled = notEmptyModel && notExecuting;

            preferencesToolStripMenuItem.Enabled = true;
        }


        /// <summary>
        /// Updates the View menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateViewMenu(Model.Model model)
        {
            bool modelHasLines = (model.Summary.NumLines > 0);
            Controller.Controller ctrl = Controller.Controller.Instance;

            selectButton.Checked = ctrl.ViewCommand is Commands.View.Selection;
            interactiveZoomToolStripMenuItem.Checked = ctrl.ViewCommand is Commands.View.ZoomInteractive;
            zoomButton.Checked = interactiveZoomToolStripMenuItem.Checked;
            panButton.Checked = ctrl.ViewCommand is Commands.View.Pan;
            panToolStripMenuItem.Checked = panButton.Checked;
            rotateButton.Checked = ctrl.ViewCommand is Commands.View.Trackball3D;
            rotateToolStripMenuItem.Checked = rotateButton.Checked;

            zoomPreviousButton.Enabled = View.GraphicViewManager.Instance.ActiveView.ArcBallsInStack > 1;

            View.Renderer.ModelRenderer renderer = View.GraphicViewManager.Instance.ActiveView.ModelRenderer;
            View.Renderer.RenderOptions options = renderer.RenderOptions;

            viewShadedButton.Checked = options.ShowShaded;
            viewShadedToolStripMenuItem.Checked = options.ShowShaded;
            viewLoadsToolStripMenuItem.Checked = !model.HasResults && (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.Loads) > 0;
            showLoadsButton.Checked = viewLoadsToolStripMenuItem.Checked;
            viewLoadsToolStripMenuItem.Enabled = !model.HasResults;
            showLoadsButton.Enabled = viewLoadsToolStripMenuItem.Enabled;

            showJointIDToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.JointIDs) > 0;
            showJointIDToolStripMenuItem2.Checked = showJointIDToolStripMenuItem.Checked;
            showDOFToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.JointDOFs) > 0;
            showDOFToolStripMenuItem2.Checked = showDOFToolStripMenuItem.Checked;
            showFloorToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.GridFloor) > 0;
            showFloorToolStripMenuItem2.Checked = showDOFToolStripMenuItem.Checked;
            showLineIDToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.LineIDs) > 0;
            showLineIDToolStripMenuItem2.Checked = showLineIDToolStripMenuItem.Checked;
            showLineLenghtToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.LineLengths) > 0;
            showLineLengthToolStripMenuItem2.Checked = showLineLenghtToolStripMenuItem.Checked;
            showAxesToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.GlobalAxes) > 0;
            showAxesToolStripMenuItem2.Checked = showAxesToolStripMenuItem.Checked;
            showFloorToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.GridFloor) > 0;
            showFloorToolStripMenuItem2.Checked = showFloorToolStripMenuItem.Checked;
            showLocalAxesToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.LineLocalAxes) > 0; ;
            showLocalAxesToolStripMenuItem2.Checked = showLocalAxesToolStripMenuItem.Checked;
            showSectionsToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.LineSections) > 0; ;
            showSectionsToolStripMenuItem2.Checked = showSectionsToolStripMenuItem.Checked;
            loadSizesToolStripMenuItem.Checked = !model.HasResults && (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.LoadMagnitudes) > 0; ;
            loadSizeToolStripMenuItem2.Checked = loadSizesToolStripMenuItem.Checked;
            loadSizesToolStripMenuItem.Enabled = !model.HasResults;
            loadSizeToolStripMenuItem2.Enabled = loadSizesToolStripMenuItem.Enabled;
            releasesToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.Releases) > 0; ;
            releasesToolStripMenuItem2.Checked = releasesToolStripMenuItem.Checked;
            jointCoordinatesToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.JointCoordinates) > 0; ;
            showJointCoordinatesToolStripMenuItem1.Checked = jointCoordinatesToolStripMenuItem.Checked;
            hideJointsToolStripMenuItem.Checked = (options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.ShowJoints) > 0; ;
            hideJointsToolStripMenuItem1.Checked = hideJointsToolStripMenuItem.Checked;

            colorsSplitButton.Enabled = modelHasLines;
            colorsToolStripMenuItem.Enabled = modelHasLines;
            colorMaterialsToolStripMenuItem.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Material);
            colorMaterialsToolStripMenuItem2.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Material);
            colorSectionsToolStripMenuItem.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Section);
            colorSectionsToolStripMenuItem2.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Section);
            colorLayersToolStripMenuItem.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Layer);
            colorLayersToolStripMenuItem2.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Layer);
            colorConstraintsToolStripMenuItem.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Constraint);
            colorConstraintsToolStripMenuItem2.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.Constraint);
            colorLoadsToolStripMenuItem.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.NonDefaultPropertyAssigned);
            colorLoadsToolStripMenuItem2.Checked = (options.LineColoredBy == Canguro.View.Renderer.RenderOptions.LineColorBy.NonDefaultPropertyAssigned);
        }


        /// <summary>
        /// Updates the Model menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateModelMenu(Model.Model model)
        {
            bool notExecuting = (Controller.Controller.Instance.ModelCommand == null);
            bool notEmptyModel = (model.Summary.NumJoints > 0);
            bool resultsArrived = (model.Results != null && model.Results.AnalysisID != 0);
            bool animating = View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated;
            bool manyCases = (model.LoadCases.Count > 1);

            modelToolStrip.Enabled = !animating;

            //loadToolStrip.Visible = !resultsArrived;
            loadCasesComboBox.Visible = !resultsArrived;
            loadCaseButton.Visible = !resultsArrived;
            loadCaseToolStripMenuItem.Visible = !resultsArrived;
            editLoadCaseButton.Visible = !resultsArrived;
            deleteLoadCaseButton.Visible = !resultsArrived;

            jointButton.Enabled = notExecuting;
            jointToolStripMenuItem.Enabled = notExecuting;
            lineButton.Enabled = notExecuting;
            linesToolStripMenuItem.Enabled = notExecuting;
            lineStripButton.Enabled = notExecuting;
            lineStripToolStripMenuItem.Enabled = notExecuting;
            arcButton.Enabled = notExecuting;
            arcToolStripMenuItem.Enabled = notExecuting;
            materialsToolStripMenuItem.Enabled = notExecuting;
            sectionsToolStripMenuItem.Enabled = notExecuting;
            forceLoadButton.Enabled = notExecuting && notEmptyModel;
            forceLoadToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            groundDisplacementButton.Enabled = notExecuting && notEmptyModel;
            groundDisplacementToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            concentratedLoadButton.Enabled = notExecuting && notEmptyModel;
            concentratedLoadToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            distributedLoadButton.Enabled = notExecuting && notEmptyModel;
            distributedLoadToolStripMenuItem.Enabled =  notExecuting && notEmptyModel;
            uniformLineLoadButton.Enabled = notExecuting && notEmptyModel;
            uniformLineLoadToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            triangleLineLoadButton.Enabled = notExecuting && notEmptyModel;
            triangularLineLoadToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            temperatureLineLoadButton.Enabled = notExecuting && notEmptyModel;
            temperatureLineLoadToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            temperatureGradientLineLoadButton.Enabled = notExecuting && notEmptyModel;
            temperatureGradientLineLoadToolStripMenuItem.Enabled = notExecuting && notEmptyModel;
            loadCaseButton.Enabled = notExecuting;
            loadCaseToolStripMenuItem.Enabled = notExecuting && !resultsArrived;
            deleteLoadCaseButton.Enabled = notExecuting && manyCases;            
            analysisCaseButton.Enabled = notExecuting;
            analysisCaseToolStripMenuItem.Enabled = notExecuting && !resultsArrived;
            loadComboButton.Enabled = notExecuting;
            loadComboToolStripMenuItem.Enabled = notExecuting && !resultsArrived;
            gridButton.Enabled = notExecuting;
            gridToolStripMenuItem.Enabled = notExecuting;
            domeButton.Enabled = notExecuting;
            domeToolStripMenuItem.Enabled = notExecuting;
            cylinderButton.Enabled = notExecuting;
            cylinderToolStripMenuItem.Enabled = notExecuting;
        }

        /// <summary>
        /// Updates the Layer menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateLayersMenu(Model.Model model)
        {
            bool notExecuting = (Controller.Controller.Instance.ModelCommand == null);
            bool animating = View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated;
            bool manyLayers = (model.Layers.CountNotNull() > 1);
            layersToolStrip.Enabled = !animating;

            layerButton.Enabled = notExecuting;
            addLayerToolStripMenuItem.Enabled = notExecuting;
            editlayerButton.Enabled = notExecuting;
            editLayerToolStripMenuItem.Enabled = notExecuting;
            deletelayerButton.Enabled = notExecuting && manyLayers;
            deleteLayerToolStripMenuItem.Enabled = notExecuting && manyLayers;
            selectLayerButton.Enabled = notExecuting;
            selectLayerToolStripMenuItem.Enabled = notExecuting;
            activateLayerButton.Enabled = notExecuting && manyLayers;
            activateLayerToolStripMenuItem.Enabled = notExecuting && manyLayers;
            hideLayerButton.Enabled = notExecuting;
            hideLayerToolStripMenuItem.Enabled = notExecuting;
            showLayerButton.Enabled = notExecuting;
            showLayerToolStripMenuItem.Enabled = notExecuting;
            moveToLayerButton.Enabled = notExecuting && manyLayers;
            moveToLayerToolStripMenuItem.Enabled = notExecuting && manyLayers;
        }

        /// <summary>
        /// Updates the Select menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateSelectMenu(Model.Model model)
        {
            bool animating = View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated;
            bool notExecuting = (Controller.Controller.Instance.ModelCommand == null);
            selectionToolStrip.Enabled = !animating && notExecuting;
            selectToolStripMenuItem.Enabled = !animating && notExecuting;
        }

        /// <summary>
        /// Updates the Analysis menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateAnalysisMenu(Model.Model model)
        {
            bool notExecuting = (Controller.Controller.Instance.ModelCommand == null);
            bool resultsArrived = (model.Results != null && model.Results.Finished); // model.Results.AnalysisID != 0);
            bool designAvailable = (resultsArrived &&
                ((model.SteelDesignOptions is Model.Design.SteelDesignOptions && model.Results.DesignSteelSummary != null) ||
                (model.ConcreteDesignOptions is Model.Design.ConcreteDesignOptions && model.Results.DesignConcreteBeam != null) ||
                (model.ConcreteDesignOptions is Model.Design.ConcreteDesignOptions && model.Results.DesignConcreteColumn != null)));

            View.Renderer.ModelRenderer renderer = View.GraphicViewManager.Instance.ActiveView.ModelRenderer;
            View.Renderer.RenderOptions options = renderer.RenderOptions;

            analyzeToolStripMenuItem.Enabled = notExecuting;
            showDeformedButton.Visible = resultsArrived;
            showDeformedToolStripMenuItem.Enabled = resultsArrived;
            animateButton.Visible = resultsArrived;
            animatedToolStripMenuItem.Enabled = resultsArrived;
            showStressesButton.Visible = resultsArrived;
            showStressesToolStripMenuItem.Enabled = resultsArrived;
            showDesignButton.Visible = designAvailable;
            designToolStripMenuItem.Enabled = designAvailable;
            reportButton.Enabled = !Canguro.View.Reports.ReportsWindow.IsOpen;
            reportsToolStripMenuItem.Enabled = !Canguro.View.Reports.ReportsWindow.IsOpen;
            showJointReactionsButton.Visible = resultsArrived;
            jointReactionsToolStripMenuItem.Enabled = resultsArrived;
            showJointReactionsTextsButton.Visible = resultsArrived;
            jointReactionTextsToolStripMenuItem.Enabled = resultsArrived;

            diagramsDropDownButton.Visible = resultsArrived;
            showDiagramToolStripMenuItem.Enabled = resultsArrived;

            showDeformedButton.Checked = options.ShowDeformed;
            showDeformedToolStripMenuItem.Checked = options.ShowDeformed;
            animateButton.Checked = options.ShowAnimated;
            animatedToolStripMenuItem.Checked = options.ShowAnimated;
            showStressesButton.Checked = options.ShowStressed;
            showStressesToolStripMenuItem.Checked = options.ShowStressed;
            showDesignButton.Checked = options.ShowDesigned;
            designToolStripMenuItem.Checked = options.ShowDesigned;

            noDiagramsToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.None);
            noDiagramsToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.None);
            axialDiagramToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Sx);
            axialDiagramToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Sx);
            s2DiagramToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Sy);
            s2DiagramToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Sy);
            s3DiagramToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Sz);
            s3DiagramToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Sz);
            torsionDiagramToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Mx);
            torsionDiagramToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Mx);
            m2DiagramToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.My);
            m2DiagramToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.My);
            m3DiagramToolStripMenuItem.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Mz);
            m3DiagramToolStripMenuItem2.Checked = (options.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Mz);
            showJointReactionsButton.Checked = ((options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.Reactions) > 0);
            showJointReactionsTextsButton.Checked = ((options.OptionsShown & Canguro.View.Renderer.RenderOptions.ShowOptions.ReactionLoads) > 0);
        }

        /// <summary>
        /// Updates the Help menu and related toolbar buttons
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateHelpMenu(Model.Model model)
        {
        }

        /// <summary>
        /// Updates the results, layers, units and load cases combo boxes
        /// </summary>
        /// <param name="model">Current Model object</param>
        private void UpdateCombosStatus(Model.Model model)
        {
            bool notExecuting = (Controller.Controller.Instance.ModelCommand == null);
            bool resultsArrived = (model.Results != null && model.Results.Finished); // model.Results.AnalysisID != 0);

            if (!IsDisposed)
            {
                resultsCasesCombo.Visible = resultsArrived;
                layersComboBox.Enabled = notExecuting;
                unitsComboBox.Enabled = notExecuting;
                loadCasesComboBox.Enabled = notExecuting;
            }
        }

        /// <summary>
        /// Updates the LayersComboBox to reflect the Models state
        /// </summary>
        private void UpdateLayersCombo()
        {
            layersComboBox.Items.Clear();
            int i = 0;
            foreach (Model.Layer layer in Model.Model.Instance.Layers)
            {
                if (layer != null)
                {
                    layersComboBox.Items.Add(layer);
                    if (Model.Model.Instance.ActiveLayer.Id == layer.Id)
                        layersComboBox.SelectedIndex = i;
                    i++;
                }
            }
        }

        /// <summary>
        /// Updates the UnitsComboBox to reflect the Models state
        /// </summary>
        private void UpdateUnitsCombo()
        {
            if (unitsComboBox.Items.Count == 0)
                foreach (Model.UnitSystem.UnitSystem us in Model.UnitSystem.UnitSystemsManager.Instance.UnitSystems)
                    if (us != null)
                        unitsComboBox.Items.Add(us);

            unitsComboBox.SelectedItem = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
        }

        /// <summary>
        /// Updates the LoadCasesComboBox to reflect the Models state
        /// </summary>
        private void UpdateLoadCasesCombo()
        {
            loadCasesComboBox.Items.Clear();
            int i = 0;
            foreach (Model.Load.LoadCase lCase in Model.Model.Instance.LoadCases.Values)
            {
                if (lCase != null)
                {
                    loadCasesComboBox.Items.Add(lCase);
                    if (Model.Model.Instance.ActiveLoadCase.Name.Equals(lCase.Name))
                        loadCasesComboBox.SelectedIndex = i;
                    i++;
                }
            }
        }

        /// <summary>
        /// Method dalled when the window is closing.
        /// Opens the file save dialog when needed.
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e) {
            if (WindowState == FormWindowState.Minimized)
                Settings.Default.WindowSize = new System.Drawing.Size(800, 600);
            else 
                Settings.Default.WindowSize = new System.Drawing.Size(Size.Width, Size.Height);
            Settings.Default.Save();
            e.Cancel = !SaveChanges();
        }
        #endregion

        /// <summary>
        /// If needed, asks the User to save changes
        /// </summary>
        /// <returns>true if the User accepts to save, false otherwise.</returns>
        private bool SaveChanges()
        {
            if (Model.Model.Instance.Modified)
            {
                DialogResult dr = MessageBox.Show(Culture.Get("askSaveChangesAndExit"), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                {
                    return false;
                }
                else if (dr == DialogResult.Yes)
                {
                    Controller.Controller.Instance.Execute("cancel");
                    Controller.Controller.Instance.Execute("save");
                }
            }
            return true;
        }

        #region CommandToolbox
        /// <summary>
        /// Method to display a Command Toolbox, a property grid to get input from the user.
        /// It can be shown modal or modeless.
        /// </summary>
        /// <param name="title">Title of the dialog</param>
        /// <param name="obj">Object to get/set the properties within the property grid</param>
        /// <param name="runAsync">True for modeless, false for modal dialog</param>
        /// <param name="listItems">Optional, list of items to be displayed in a combobox
        /// above the property grid. It can change the object the grid is editing.</param>
        public void ShowCommandToolbox(string title, object[] obj, bool runAsync, string[] listItems)
        {
            if (obj != null && obj.Length > 0)
            {
                if (commandToolbox == null || commandToolbox.Disposing || commandToolbox.IsDisposed)
                    commandToolbox = new Canguro.Commands.Forms.CommandToolbox(this);

                if (obj[0] != null)
                {
                    commandToolbox.Title = title;
                    commandToolbox.SetComboItems(listItems);
                    commandToolbox.ShowOKCancel = !runAsync;
                    commandToolbox.Properties.SelectedObject = obj[0];
                    if (runAsync)
                        commandToolbox.Show(this);
                    else
                    {
                        try
                        {
                            DialogResult dr = commandToolbox.ShowDialog(findToolboxOwner(this, commandToolbox));
                            if (dr == DialogResult.Cancel)
                                Controller.Controller.Instance.Execute("cancel");
                            else
                                HideCommandToolbox();
                        }
                        catch (ArgumentException) { }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the owner of a toolbox
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Form findToolboxOwner(Form start, Form target)
        {
            Form toolboxOwner = start;
            foreach (Form form in start.OwnedForms)
                if (form.TopLevel && form != target)
                {
                    toolboxOwner = findToolboxOwner(form, target);
                }

            return toolboxOwner;
        }

        /// <summary>
        /// Method to hide and end the Command Toolbox interaction.
        /// </summary>
        public void HideCommandToolbox()
        {
            if (commandToolbox != null && !commandToolbox.IsDisposed && !commandToolbox.Disposing)
            {
                commandToolbox.Properties.SelectedObject = null;
                commandToolbox.SetComboItems(null);
                commandToolbox.Hide();
            }
        }
        #endregion

        #region ReportProgress + SetStatusLabel
        long lastReportProgressTicks = 0;
        string lastReportProgressLock = "";
        /// <summary>
        /// Delegate to allow commands to display their progress. If a command takes some 
        /// time to complete without user intervention, it should call this method. 
        /// </summary>
        /// <param name="mainText">Description of the whole command process</param>
        /// <param name="mainProgress">Percentage completed of the whole command's execution</param>
        /// <param name="subtaskText">Description of the command's current task</param>
        /// <param name="subtaskProgress">Percentage completed of the current command task being performed</param>
        /// <param name="timeStamp">Time Stamp to determine the order of the messages when multiple threads are involved</param>
        delegate void reportProgressDelegate(string mainText, uint mainProgress, string subtaskText, uint subtaskProgress, DateTime timeStamp);

        /// <summary>
        /// Method to allow commands to display their progress. If a command takes some 
        /// time to complete without user intervention, it should call this method. 
        /// </summary>
        /// <param name="mainText">Description of the whole command process</param>
        /// <param name="mainProgress">Percentage completed of the whole command's execution</param>
        /// <param name="subtaskText">Description of the command's current task</param>
        /// <param name="subtaskProgress">Percentage completed of the current command task being performed</param>
        /// <param name="timeStamp">Time Stamp to determine the order of the messages when multiple threads are involved</param>
        public void ReportProgress(string mainText, uint mainProgress, string subtaskText, uint subtaskProgress, DateTime timeStamp)
        {
            lock (lastReportProgressLock)
            {
                if (timeStamp.Ticks < lastReportProgressTicks)
                    return;

                lastReportProgressTicks = timeStamp.Ticks;
                //System.Diagnostics.Debug.Print("Report {0}, {1}, {2}, {3}", subtaskProgress, timeStamp.ToLongTimeString(), timeStamp.Millisecond, timeStamp.Ticks);
            }

            if (!InvokeRequired && !Disposing && !IsDisposed)
            {
                if (string.IsNullOrEmpty(mainText))   // Hide progress bars
                {
                    SetStatusLabel(null);
                    progress1.Visible = progress2.Visible = report2Label.Visible = false;
                }
                else
                {
                    if (mainProgress > 100) mainProgress = 100;
                    statusLabel.Text = mainText;
                    progress1.Visible = true;
                    progress1.Value = (int)mainProgress;

                    if (string.IsNullOrEmpty(subtaskText)) // Hide subTask progress bar
                        progress2.Visible = report2Label.Visible = false;
                    else
                    {
                        if (subtaskProgress > 100) subtaskProgress = 100;
                        progress2.Visible = report2Label.Visible = true;
                        report2Label.Text = subtaskText;
                        progress2.Value = (int)subtaskProgress;
                    }
                }
            }
            else
            {
                try
                {
                    BeginInvoke(new reportProgressDelegate(ReportProgress),
                        new object[] { mainText, mainProgress, subtaskText, subtaskProgress, timeStamp });
                }
                catch (InvalidOperationException) { } // Ignore error
            }
        }

        /// <summary>
        /// Method to change the Status Label Text displayed on the StatusBar
        /// </summary>
        /// <param name="message">The text message to display</param>
        public void SetStatusLabel(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Model.ModelSummary sum = Model.Model.Instance.Summary;
                if (sum.SelectedJoints > 0 || sum.SelectedLines > 0)
                    statusLabel.Text = Culture.Get("Selection") + ": " + sum.SelectedJoints + " " + Culture.Get("Joints") + ", " + sum.SelectedLines + " " + Culture.Get("Lines");
                else
                    statusLabel.Text = Culture.Get("ready");
            }
            else
                statusLabel.Text = message;
        }
        #endregion

        protected override void OnActivated(EventArgs e)
        {
            //Console.Write("Activated");
            UpdateEditMenu(Canguro.Model.Model.Instance);
            base.OnActivated(e);
        }

        /// <summary>
        /// Updates the active layer
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void layersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Model.Model.Instance.ActiveLayer != layersComboBox.SelectedItem) {
                    Model.Model.Instance.ActiveLayer = (Model.Layer)layersComboBox.SelectedItem;
                    Model.Model.Instance.Undo.Commit();
                }
            }
            catch { }
        }

        /// <summary>
        /// Updates the name of the active layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layersComboBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Model.Model.Instance.ActiveLayer.Name = layersComboBox.Text;
                UpdateLayersCombo();
            }
            catch { }
        }

        /// <summary>
        /// Updates the active load case
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void loadCasesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Model.Model.Instance.ActiveLoadCase = (Model.Load.LoadCase)loadCasesComboBox.SelectedItem;
            }
            catch { }
        }

        /// <summary>
        /// Updates the active Unit system
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void unitsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem =
                    (Model.UnitSystem.UnitSystem)unitsComboBox.SelectedItem;
                Model.Model.Instance.ChangeModel();
            }
            catch { }
        }

        /// <summary>
        /// Updates the Results.ActiveCase
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void resultsCasesCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Model.Model.Instance.HasResults)
            {
                Model.Results.ResultsCase rc = Model.Model.Instance.Results.ActiveCase;
                try
                {
                    if (resultsCasesCombo.SelectedItem.ToString()[0] == '-')
                    {
                        resultsCasesCombo.SelectedIndex++;
                        return;
                    }

                    Model.Model.Instance.Results.ActiveCase = (Model.Results.ResultsCase)resultsCasesCombo.SelectedItem;
                    Model.Model.Instance.ChangeModel();
                }
                catch (Exception)
                {
                    MessageBox.Show("resultsActiveCaseChangeProblemStr", "resultsActiveCaseChangeTitle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Model.Model.Instance.Results.ActiveCase = rc;
                }
            }
        }

        /// <summary>
        /// Executes "selection"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void resultsCasesCombo_Click(object sender, EventArgs e)
        {

        }

        #region Commands

        /// <summary>
        /// Executes "selection"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void selectButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("selection");
        }

        /// <summary>
        /// Executes "zoom"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void zoomButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("zoom");
        }

        /// <summary>
        /// Executes "zoomall"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void viewAllButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("zoomall");
        }

        /// <summary>
        /// Executes "pan"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void panButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("pan");
        }

        /// <summary>
        /// Executes "rotate"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void rotateButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("rotate");
        }

        /// <summary>
        /// Executes "predefinedxy"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void predefinedxy_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("predefinedxy");
        }

        /// <summary>
        /// Executes "predefinedxz"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void predefinedxz_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("predefinedxz");
        }

        /// <summary>
        /// Executes "predefinedyz"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void predefinedyz_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("predefinedyz");
        }

        /// <summary>
        /// Executes "newwzd"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void newButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("newwzd");
        }

        /// <summary>
        /// Executes "open"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void openButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("open");
        }

        /// <summary>
        /// Executes "save"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("save");
        }

        /// <summary>
        /// Executes "saveas"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("saveas");
        }

        /// <summary>
        /// Executes "print"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void screenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("savescreenshot");
        }

        /// <summary>
        /// Executes "print"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void printButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("print");
        }

        /// <summary>
        /// Executes "printpreview"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void printPreviewButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("printpreview");
        }

        /// <summary>
        /// Executes "cut"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void cutButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("cut");
        }

        /// <summary>
        /// Executes "copy"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void copyButton_Click(object sender, EventArgs e)
        {
            bool gridHasFocus = false;
            DataGridView[] gvs = GetGridViews();
            foreach (DataGridView gv in gvs)
                if (gv.Focused)
                    gridHasFocus = true;

            if (gridHasFocus)
            {
                if (gridTabs.SelectedTab == tabJoints)
                    jointGridView.Copy();
                else if (gridTabs.SelectedTab == tabFrames)
                    lineGridView.Copy();
                //else if (gridTabs.SelectedTab == tabShells)
                //    areaGridView.Copy();

                MessageBox.Show(Culture.Get("strGridDataCopiedToClipboard"));
            }
            else
                Controller.Controller.Instance.ExecuteAsync("copy");
        }

        /// <summary>
        /// Executes "paste"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void pasteButton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue))     // Paste into active Grid
            {
                // Get the data
                string clipCSV = Clipboard.GetText();
                string[] csv = clipCSV.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string[][] values = new string[csv.Length][];
                for (int i = 0; i < csv.Length; i++)
                    values[i] = csv[i].Split(",\t".ToCharArray());

                // Find active Grid and active cell
                if (gridTabs.SelectedTab == tabJoints)
                    jointGridView.Paste(values);
                else if (gridTabs.SelectedTab == tabFrames)
                    lineGridView.Paste(values);
                //else if (gridTabs.SelectedTab == tabShells)
                //    areaGridView.Paste(values);
                else
                {
                    MessageBox.Show(Culture.Get("strCannotPasteInCurrentGridTab"));
                    return;
                }
            }
            else    // Execute Paste command
                Controller.Controller.Instance.ExecuteAsync("paste");
        }

        /// <summary>
        /// Executes "undo"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void undoButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("undo");
        }

        /// <summary>
        /// Executes "redo"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void redoButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("redo");
        }

        /// <summary>
        /// Executes "joint"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void jointButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("joint");
        }

        /// <summary>
        /// Executes "line"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void lineButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("line");
        }

        /// <summary>
        /// Executes "loadcase"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void loadCaseButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("loadcase");
        }

        /// <summary>
        /// Executes "analysiscase"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void analysisCaseButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("analysiscase");
        }

        /// <summary>
        /// Executes "loadcombination"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void loadComboButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("loadcombination");
        }

        /// <summary>
        /// Executes "mirror"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void mirrorButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("mirror");
        }

        /// <summary>
        /// Executes "scale"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void scaleButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("scale");
        }

        /// <summary>
        /// Executes "array"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void arrayButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("array");
        }

        /// <summary>
        /// Executes "polararray"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void polararrayButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("polararray");
        }

        /// <summary>
        /// Executes "modelrotate"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void modelrotateButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("modelrotate");
        }

        /// <summary>
        /// Executes "zoomin"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void zoomInButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("zoomin");
        }

        /// <summary>
        /// Executes "zoomout"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("zoomout");
        }

        /// <summary>
        /// Executes "zoomprevious"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void zoomPreviousButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("zoomprevious");
        }

        /// <summary>
        /// Executes "hide"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void hideButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("hide");
        }

        /// <summary>
        /// Executes "showall"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showAllButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("showall");
        }

        #region RenderOptions
        /// <summary>
        /// Toggle the ShowShaded Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void viewShadedButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowShaded = !View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowShaded;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the JointIDs Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void jointIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.JointIDs;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the LineIDs Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void lineIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.LineIDs;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the LineLengths Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void lineLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.LineLengths;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the LineLocalAxes Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showLocalAxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.LineLocalAxes;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the LineSections Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showSectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.LineSections;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Sets the Nothing Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown = Canguro.View.Renderer.RenderOptions.ShowOptions.Nothing;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the Loads Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showLoadsButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.Loads;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the GridFloor Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void viewFloorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.GridFloor;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the GlobalAxes Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void viewAxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.GlobalAxes;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the LoadMagnitudes Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void loadSizesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.LoadMagnitudes;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the JointDOFs Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showDOFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.JointDOFs;
            UpdateViewMenu(Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the ShowDeformed Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showDeformedButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowDeformed = !View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowDeformed;
            UpdateAnalysisMenu(Canguro.Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the ShowAnimated Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void animateButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated = !View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated;
            UpdateAnalysisMenu(Canguro.Model.Model.Instance);
            UpdateToolBar(Canguro.Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the ShowStressed Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showStressesButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowStressed = !View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowStressed;
            Canguro.Model.Model model = Canguro.Model.Model.Instance;
            UpdateAnalysisMenu(model);
            UpdateViewMenu(model);
        }

        /// <summary>
        /// Toggle the ShowDesigned Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showDesignButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowDesigned = !View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowDesigned;
            View.GraphicViewManager.Instance.UpdateView();
            Canguro.Model.Model model = Canguro.Model.Model.Instance;
            UpdateAnalysisMenu(model);
            UpdateViewMenu(model);
        }

        /// <summary>
        /// Toggle the ShowDesigned Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void jointCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.JointCoordinates;
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the ShowDesigned Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showJointReactionsButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.Reactions;
            UpdateAnalysisMenu(Canguro.Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the ShowDesigned Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showJointReactionsTextsButton_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.ReactionLoads;
            UpdateAnalysisMenu(Canguro.Model.Model.Instance);
        }

        /// <summary>
        /// Toggle the ShowOptions.Releases Render Option
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void releasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.Releases;
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        #region Colors
        private void colorMaterialsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.LineColoredBy = Canguro.View.Renderer.RenderOptions.LineColorBy.Material;
            View.GraphicViewManager.Instance.UpdateView();
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        private void colorSectionsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.LineColoredBy = Canguro.View.Renderer.RenderOptions.LineColorBy.Section;
            View.GraphicViewManager.Instance.UpdateView();
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        private void colorLayersToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.LineColoredBy = Canguro.View.Renderer.RenderOptions.LineColorBy.Layer;
            View.GraphicViewManager.Instance.UpdateView();
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        private void colorConstraintsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.LineColoredBy = Canguro.View.Renderer.RenderOptions.LineColorBy.Constraint;
            View.GraphicViewManager.Instance.UpdateView();
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        private void colorLoadsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.LineColoredBy = Canguro.View.Renderer.RenderOptions.LineColorBy.NonDefaultPropertyAssigned;
            View.GraphicViewManager.Instance.UpdateView();
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }

        private void hideJointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown ^= Canguro.View.Renderer.RenderOptions.ShowOptions.ShowJoints;
            UpdateViewMenu(Canguro.Model.Model.Instance);
        }
        #endregion

        #endregion

        /// <summary>
        /// Executes "predefinedxyz"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void predefinedxyzButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("predefinedxyz");
        }


        /// <summary>
        /// Executes "linestrip"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void lineStripButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("linestrip");
        }

        /// <summary>
        /// Executes "split"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void splitButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("split");
        }

        /// <summary>
        /// Executes "grid"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void gridButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("grid");
        }

        /// <summary>
        /// Executes "dome"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void domeButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("dome");
        }

        /// <summary>
        /// Executes "cylinder"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void cylinderButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("cylinder");
        }

        /// <summary>
        /// Executes "delete"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("delete");
        }

        /// <summary>
        /// Executes "copypaste"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void copyPasteButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("copypaste");
        }

        /// <summary>
        /// Executes "edit"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void editButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("edit");
        }

        /// <summary>
        /// Executes "move"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void moveButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("move");
        }

        /// <summary>
        /// Executes "selectlayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void selectLayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("selectlayer");
        }

        /// <summary>
        /// Executes "hidelayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void hideLayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("hidelayer");
        }

        /// <summary>
        /// Executes "showlayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void showLayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("showlayer");
        }

        /// <summary>
        /// Executes "movetolayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void moveToLayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("movetolayer");
        }

        /// <summary>
        /// Executes "activatelayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void activateLayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("activatelayer");
        }

        /// <summary>
        /// Executes "layer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void layerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("layer");
        }

        /// <summary>
        /// Executes "editlayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void editlayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("editlayer");
        }

        /// <summary>
        /// Executes "deletelayer"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void deletelayerButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("deletelayer");
        }

        /// <summary>
        /// Executes "join"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void joinButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("join");
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executes "selectall"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void selectAllButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("selectall");
        }

        /// <summary>
        /// Executes "selectconnected"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void selectConnectedButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("selectconnected");
        }

        /// <summary>
        /// Executes "invertselection"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void invertSelectionButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("invertselection");
        }

        /// <summary>
        /// Executes "unselect"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void unselectButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("unselect");
        }

        /// <summary>
        /// Executes "selectline"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void selectLineButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("selectline");
        }

        /// <summary>
        /// Executes "hidediagrams"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void noneToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("hidediagrams");
        }

        /// <summary>
        /// Executes "renders1"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void axialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("renders1");
        }

        /// <summary>
        /// Executes "renders2"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void s2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("renders2");
        }

        /// <summary>
        /// Executes "renders3"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void s3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("renders3");
        }

        /// <summary>
        /// Executes "renderm1"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void torsionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("renderm1");
        }

        /// <summary>
        /// Executes "renderm2"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void m2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("renderm2");
        }

        /// <summary>
        /// Executes "renderm3"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void m3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.Execute("renderm3");
        }

        /// <summary>
        /// Executes "forceload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void forceLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("forceload");
        }

        /// <summary>
        /// Executes "grounddisplacementload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void groundDisplacementButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("grounddisplacementload");
        }

        /// <summary>
        /// Executes "concentratedload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void concentratedLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("concentratedload");
        }

        /// <summary>
        /// Executes "distributedspanload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void distributedLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("distributedspanload");
        }

        /// <summary>
        /// Executes "uniformlineload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void uniformLineLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("uniformlineload");
        }

        /// <summary>
        /// Executes "trianglelineload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void triangleLineLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("trianglelineload");
        }

        /// <summary>
        /// Executes "temperaturelineload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void temperatureLineLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("temperaturelineload");
        }

        /// <summary>
        /// Executes "temperaturegradientlineload"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void temperatureGradientLineLoadButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("temperaturegradientlineload");
        }

        /// <summary>
        /// Executes "editloadcase"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void editLoadCaseButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("editloadcase");
        }

        /// <summary>
        /// Executes "deleteloadcase"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void deleteLoadCaseButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("deleteloadcase");
        }

        /// <summary>
        /// Executes "report"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void reportButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("report");
        }

        /// <summary>
        /// Executes "importdxf"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void importDXFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("importdxf");
        }

        /// <summary>
        /// Executes "exportdxf"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void exportDXFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("exportdxf");
        }

        /// <summary>
        /// Executes "exports2k"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void exportS2kToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("exports2k");
        }

        /// <summary>
        /// Executes "analyze"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void analizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("analyze");
        }

        /// <summary>
        /// Executes "materials"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void materialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("materials");
        }

        /// <summary>
        /// Executes "sections"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void sectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("sections");
        }

        /// <summary>
        /// Executes "intersect"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void intersectButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("intersect");
        }

        /// <summary>
        /// Executes "arc"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void arcButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("arc");
        }

        /// <summary>
        /// Executes "flipline"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void flipLineButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("flipline");
        }

        /// <summary>
        /// Executes "distance"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void distanceButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("distance");
        }

        /// <summary>
        /// Executes "constraints"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void constraintsButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("constraints");
            colorConstraintsToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Executes "diaphragms"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void diaphragmsButton_Click(object sender, EventArgs e)
        {
            Controller.Controller.Instance.ExecuteAsync("diaphragms");
            colorConstraintsToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Opens the AboutBox dialog
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        /// <summary>
        /// Open the PreferencesDialog window
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PreferencesDialog().ShowDialog();
        }

        #region Grid Toolbars
        /// <summary>
        /// Executes jointGridView.FillDown()
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void jointFillDownButton_Click(object sender, EventArgs e)
        {
            jointGridView.FillDown();
        }

        /// <summary>
        /// Executes lineGridView.FillDown()
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void lineFillDownButton_Click(object sender, EventArgs e)
        {
            lineGridView.FillDown();
        }

        /// <summary>
        /// Executes lineGridView.FillDown()
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void areaFillDownButton_Click(object sender, EventArgs e)
        {
            //areaGridView.FillDown();
        }

        /// <summary>
        /// Sets all selected joints' DoF in the grid to RRRRRR
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void restraintAllButton_Click(object sender, EventArgs e)
        {
            JointDOF dof = new JointDOF(true);
            foreach (Joint j in jointGridView.SelectedObjects)
                j.DoF = dof;

            if (jointGridView.CurrentCell != null)
                jointGridView.CurrentCell = jointGridView.CurrentRow.Cells[3];

            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
        }

        /// <summary>
        /// Sets all selected joints' DoF in the grid to RRRFFF
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void restraintTransButton_Click(object sender, EventArgs e)
        {
            JointDOF dof = new JointDOF(false);
            dof.T1 = JointDOF.DofType.Restrained;
            dof.T2 = JointDOF.DofType.Restrained;
            dof.T3 = JointDOF.DofType.Restrained;
            foreach (Joint j in jointGridView.SelectedObjects)
                j.DoF = dof;

            if (jointGridView.CurrentCell != null)
                jointGridView.CurrentCell = jointGridView.CurrentRow.Cells[3];

            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
        }

        /// <summary>
        /// Sets all selected joints' DoF in the grid to FFRFFF
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void restraintZButton_Click(object sender, EventArgs e)
        {
            JointDOF dof = new JointDOF(false);
            dof.T3 = JointDOF.DofType.Restrained;
            foreach (Joint j in jointGridView.SelectedObjects)
                j.DoF = dof;

            if (jointGridView.CurrentCell != null)
                jointGridView.CurrentCell = jointGridView.CurrentRow.Cells[3];

            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
        }

        /// <summary>
        /// Sets all selected joints' DoF in the grid to FFFFFF
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void restraintFreeButton_Click(object sender, EventArgs e)
        {
            JointDOF dof = new JointDOF(false);
            foreach (Joint j in jointGridView.SelectedObjects)
                j.DoF = dof;

            if (jointGridView.CurrentCell != null)
                jointGridView.CurrentCell = jointGridView.CurrentRow.Cells[3];

            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
        }

        /// <summary>
        /// Sets Releases to RRRRRR RRRRRR
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void noReleaseButton_Click(object sender, EventArgs e)
        {
            JointDOF dofi = new JointDOF(true);
            JointDOF dofj = new JointDOF(true);
            foreach (LineElement line in lineGridView.SelectedObjects)
            {
                line.DoFI = dofi;
                line.DoFJ = dofj;
            }

            if (lineGridView.CurrentCell != null)
                lineGridView.CurrentCell = lineGridView.CurrentRow.Cells[4];

            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
            //areaGridView.Refresh();
        }

        /// <summary>
        /// Sets Releases to RRRRFF RRRRFF
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void momentReleaseButton_Click(object sender, EventArgs e)
        {
            JointDOF dofi = new JointDOF(true);
            dofi.R2 = JointDOF.DofType.Free;
            dofi.R3 = JointDOF.DofType.Free;
            JointDOF dofj = new JointDOF(true);
            dofj.R2 = JointDOF.DofType.Free;
            dofj.R3 = JointDOF.DofType.Free;
            foreach (LineElement line in lineGridView.SelectedObjects)
            {
                line.DoFI = dofi;
                line.DoFJ = dofj;
            }

            if (lineGridView.CurrentCell != null)
                lineGridView.CurrentCell = lineGridView.CurrentRow.Cells[4];

            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
            //areaGridView.Refresh();
        }

        /// <summary>
        /// Sets Releases to RRRRRR FRRRRR
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void axialReleaseButton_Click(object sender, EventArgs e)
        {
            JointDOF dofi = new JointDOF(true);
            JointDOF dofj = new JointDOF(true);
            dofj.T1 = JointDOF.DofType.Free;
            foreach (LineElement line in lineGridView.SelectedObjects)
            {
                line.DoFI = dofi;
                line.DoFJ = dofj;
            }
            if (lineGridView.CurrentCell != null)
                lineGridView.CurrentCell = lineGridView.CurrentRow.Cells[4];
            Model.Model.Instance.ChangeModel();
            Model.Model.Instance.Undo.Commit();
            jointGridView.Refresh();
            //areaGridView.Refresh();
        }

        private void selectSimilarAreaButton_Click(object sender, EventArgs e)
        {
            //SelectSimilarCmd.SelectAreas(areaGridView);
        }

        private void selectSimilarLineButton_Click(object sender, EventArgs e)
        {
            SelectSimilarCmd.SelectLines(lineGridView);
        }

        private void selectSimilarJointButton_Click(object sender, EventArgs e)
        {
            SelectSimilarCmd.SelectJoints(jointGridView);
        }

        #endregion
        /// <summary>
        /// Opens the Manuals Directory
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void userManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Manuals\\";
                System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(path);
                start.UseShellExecute = true;
                System.Diagnostics.Process.Start(start);
            }
            catch
            {
                MessageBox.Show("Sorry, file not found");
            }
        }

        /// <summary>
        /// Opens "\\Tutorials\\index.html"
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        private void tutorialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Tutorials\\index.html";
                System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(path);
                start.UseShellExecute = true;
                System.Diagnostics.Process.Start(start);
            }
            catch
            {
                MessageBox.Show("Sorry, file not found");
            }
        }
        #endregion

        private void MainFrm_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileNames != null && fileNames.Length != 0)
            {
                string file = null;
                List<string> acceptedExtensions = new List<string>(new string[] { ".tsm", ".dxf", ".xml" });
                foreach (string fName in fileNames)
                    if (acceptedExtensions.Contains(System.IO.Path.GetExtension(fName).ToLower()))
                    {
                        file = fName;
                        break;
                    }
                if (SaveChanges())
                    Controller.Controller.Instance.LoadModel(file);
            }
        }

        private void MainFrm_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == View.Presenter.CM_PAINT || m.Msg == View.Presenter.CM_TRACKINGPAINT)
            {
                Controller.Controller.Instance.ProcessCustomMessages(ref m);
            }
        
            base.WndProc(ref m);
        }

        private void MainFrm_SizeChanged(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Maximized)
                Settings.Default.WindowMaximazed = "Yes";
            else
                Settings.Default.WindowMaximazed = "No";
            Settings.Default.Save();
        }

        private void MainFrm_MaximizedBoundsChanged(object sender, EventArgs e) {}

        private void MainFrm_MaximumSizeChanged(object sender, EventArgs e) {}

        private void MainFrm_ResizeBegin(object sender, EventArgs e) {}

        private void MainFrm_ResizeEnd(object sender, EventArgs e) {}

        private void MainFrm_Resize(object sender, EventArgs e) {}

        private void MainFrm_AutoSizeChanged(object sender, EventArgs e) {}
    }
}