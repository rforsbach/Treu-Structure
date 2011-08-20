namespace Canguro
{
    /// <summary>
    /// Main window. Contains all menues, toolboxes, views and related events.
    /// </summary>
    partial class MainFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progress1 = new System.Windows.Forms.ToolStripProgressBar();
            this.report2Label = new System.Windows.Forms.ToolStripStatusLabel();
            this.progress2 = new System.Windows.Forms.ToolStripProgressBar();
            this.gridSplit = new System.Windows.Forms.SplitContainer();
            this.gridTabs = new System.Windows.Forms.TabControl();
            this.tabJoints = new System.Windows.Forms.TabPage();
            this.jointsToolStrip = new System.Windows.Forms.ToolStrip();
            this.jointFillDownButton = new System.Windows.Forms.ToolStripButton();
            this.selectSimilarJointButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.restraintAllButton = new System.Windows.Forms.ToolStripButton();
            this.restraintTransButton = new System.Windows.Forms.ToolStripButton();
            this.restraintZButton = new System.Windows.Forms.ToolStripButton();
            this.restraintFreeButton = new System.Windows.Forms.ToolStripButton();
            this.tabFrames = new System.Windows.Forms.TabPage();
            this.linesToolStrip = new System.Windows.Forms.ToolStrip();
            this.lineFillDownButton = new System.Windows.Forms.ToolStripButton();
            this.selectSimilarLineButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.axialReleaseButton = new System.Windows.Forms.ToolStripButton();
            this.momentReleaseButton = new System.Windows.Forms.ToolStripButton();
            this.noReleaseButton = new System.Windows.Forms.ToolStripButton();
            this.modelToolStrip = new System.Windows.Forms.ToolStrip();
            this.jointButton = new System.Windows.Forms.ToolStripButton();
            this.lineStripButton = new System.Windows.Forms.ToolStripButton();
            this.lineButton = new System.Windows.Forms.ToolStripButton();
            this.arcButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.sectionsButton = new System.Windows.Forms.ToolStripButton();
            this.materialsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.moveButton = new System.Windows.Forms.ToolStripButton();
            this.mirrorButton = new System.Windows.Forms.ToolStripButton();
            this.scaleButton = new System.Windows.Forms.ToolStripButton();
            this.arrayButton = new System.Windows.Forms.ToolStripButton();
            this.polararrayButton = new System.Windows.Forms.ToolStripButton();
            this.modelrotateButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.splitButton = new System.Windows.Forms.ToolStripButton();
            this.intersectButton = new System.Windows.Forms.ToolStripButton();
            this.joinButton = new System.Windows.Forms.ToolStripButton();
            this.flipLineButton = new System.Windows.Forms.ToolStripButton();
            this.constraintsButton = new System.Windows.Forms.ToolStripButton();
            this.diaphragmsButton = new System.Windows.Forms.ToolStripButton();
            this.loadToolStrip = new System.Windows.Forms.ToolStrip();
            this.analysisCaseButton = new System.Windows.Forms.ToolStripButton();
            this.loadCaseButton = new System.Windows.Forms.ToolStripButton();
            this.editLoadCaseButton = new System.Windows.Forms.ToolStripButton();
            this.deleteLoadCaseButton = new System.Windows.Forms.ToolStripButton();
            this.loadCasesComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.loadComboButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.forceLoadButton = new System.Windows.Forms.ToolStripButton();
            this.groundDisplacementButton = new System.Windows.Forms.ToolStripButton();
            this.concentratedLoadButton = new System.Windows.Forms.ToolStripButton();
            this.distributedLoadButton = new System.Windows.Forms.ToolStripButton();
            this.uniformLineLoadButton = new System.Windows.Forms.ToolStripButton();
            this.triangleLineLoadButton = new System.Windows.Forms.ToolStripButton();
            this.temperatureLineLoadButton = new System.Windows.Forms.ToolStripButton();
            this.temperatureGradientLineLoadButton = new System.Windows.Forms.ToolStripButton();
            this.templatesToolStrip = new System.Windows.Forms.ToolStrip();
            this.gridButton = new System.Windows.Forms.ToolStripButton();
            this.domeButton = new System.Windows.Forms.ToolStripButton();
            this.cylinderButton = new System.Windows.Forms.ToolStripButton();
            this.viewToolStrip = new System.Windows.Forms.ToolStrip();
            this.selectButton = new System.Windows.Forms.ToolStripButton();
            this.zoomButton = new System.Windows.Forms.ToolStripButton();
            this.panButton = new System.Windows.Forms.ToolStripButton();
            this.rotateButton = new System.Windows.Forms.ToolStripButton();
            this.predefinedxy = new System.Windows.Forms.ToolStripButton();
            this.predefinedxz = new System.Windows.Forms.ToolStripButton();
            this.predefinedyz = new System.Windows.Forms.ToolStripButton();
            this.predefinedxyzButton = new System.Windows.Forms.ToolStripButton();
            this.zoomInButton = new System.Windows.Forms.ToolStripButton();
            this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.zoomPreviousButton = new System.Windows.Forms.ToolStripButton();
            this.viewAllButton = new System.Windows.Forms.ToolStripButton();
            this.viewShadedButton = new System.Windows.Forms.ToolStripButton();
            this.colorsSplitButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.colorMaterialsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.colorSectionsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.colorLayersToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.colorLoadsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.colorConstraintsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showTextDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showJointIDToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.hideJointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointCoordinatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDOFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLineIDToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showLineLengthToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showLocalAxesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showSectionsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.releasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAxesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showFloorToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSizeToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showLoadsButton = new System.Windows.Forms.ToolStripButton();
            this.resultsToolStrip = new System.Windows.Forms.ToolStrip();
            this.analyzeButton = new System.Windows.Forms.ToolStripButton();
            this.resultsCasesCombo = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showDeformedButton = new System.Windows.Forms.ToolStripButton();
            this.animateButton = new System.Windows.Forms.ToolStripButton();
            this.showStressesButton = new System.Windows.Forms.ToolStripButton();
            this.showDesignButton = new System.Windows.Forms.ToolStripButton();
            this.diagramsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.noDiagramsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.axialDiagramToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.s2DiagramToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.s3DiagramToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.torsionDiagramToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.m2DiagramToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.m3DiagramToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.reportButton = new System.Windows.Forms.ToolStripButton();
            this.showJointReactionsButton = new System.Windows.Forms.ToolStripButton();
            this.showJointReactionsTextsButton = new System.Windows.Forms.ToolStripButton();
            this.layersToolStrip = new System.Windows.Forms.ToolStrip();
            this.layerButton = new System.Windows.Forms.ToolStripButton();
            this.editlayerButton = new System.Windows.Forms.ToolStripButton();
            this.deletelayerButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.layersComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.hideLayerButton = new System.Windows.Forms.ToolStripButton();
            this.showLayerButton = new System.Windows.Forms.ToolStripButton();
            this.moveToLayerButton = new System.Windows.Forms.ToolStripButton();
            this.activateLayerButton = new System.Windows.Forms.ToolStripButton();
            this.selectLayerButton = new System.Windows.Forms.ToolStripButton();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.importDXFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDXFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportS2kToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polarArrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.intersectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interactiveZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.predefinedXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.predefinedXZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.predefinedYZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.predefinedXYZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewShadedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.showJointIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideJointsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showJointCoordinatesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showDOFToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showLineIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLineLenghtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLocalAxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releasesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showAxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFloorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSizesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLoadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorMaterialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorSectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorLayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorLoadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorConstraintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.materialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.forceLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groundDisplacementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.concentratedLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uniformLineLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangularLineLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distributedLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.temperatureLineLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temperatureGradientLineLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.loadCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadComboToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.constraintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diaphragmsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.domeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cylinderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.hideLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activateLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unselectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectConnectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.showDeformedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noDiagramsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axialDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.s2DiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.s3DiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.torsionDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m2DiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m3DiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStressesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.designToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointReactionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointReactionTextsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ayudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectionToolStrip = new System.Windows.Forms.ToolStrip();
            this.selectAllButton = new System.Windows.Forms.ToolStripButton();
            this.invertSelectionButton = new System.Windows.Forms.ToolStripButton();
            this.unselectButton = new System.Windows.Forms.ToolStripButton();
            this.selectLineButton = new System.Windows.Forms.ToolStripButton();
            this.selectConnectedButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.hideButton = new System.Windows.Forms.ToolStripButton();
            this.showAllButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.distanceButton = new System.Windows.Forms.ToolStripButton();
            this.fileEditToolStrip = new System.Windows.Forms.ToolStrip();
            this.newButton = new System.Windows.Forms.ToolStripButton();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.printButton = new System.Windows.Forms.ToolStripButton();
            this.printPreviewButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cutButton = new System.Windows.Forms.ToolStripButton();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.copyPasteButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.editButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.undoButton = new System.Windows.Forms.ToolStripButton();
            this.redoButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.unitsComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.jointGridView = new Canguro.JointGridView();
            this.lineGridView = new Canguro.LineElementGridView();
            this.smallPanel = new Canguro.View.SmallPanel();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.RightToolStripPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSplit)).BeginInit();
            this.gridSplit.Panel1.SuspendLayout();
            this.gridSplit.SuspendLayout();
            this.gridTabs.SuspendLayout();
            this.tabJoints.SuspendLayout();
            this.jointsToolStrip.SuspendLayout();
            this.tabFrames.SuspendLayout();
            this.linesToolStrip.SuspendLayout();
            this.modelToolStrip.SuspendLayout();
            this.loadToolStrip.SuspendLayout();
            this.templatesToolStrip.SuspendLayout();
            this.viewToolStrip.SuspendLayout();
            this.resultsToolStrip.SuspendLayout();
            this.layersToolStrip.SuspendLayout();
            this.menu.SuspendLayout();
            this.selectionToolStrip.SuspendLayout();
            this.fileEditToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jointGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.gridSplit);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.Controls.Add(this.modelToolStrip);
            this.toolStripContainer1.TabStop = false;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.selectionToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menu);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.fileEditToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.resultsToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.templatesToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.loadToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.viewToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.layersToolStrip);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.progress1,
            this.report2Label,
            this.progress2});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            resources.ApplyResources(this.statusLabel, "statusLabel");
            // 
            // progress1
            // 
            this.progress1.Name = "progress1";
            resources.ApplyResources(this.progress1, "progress1");
            // 
            // report2Label
            // 
            this.report2Label.Name = "report2Label";
            resources.ApplyResources(this.report2Label, "report2Label");
            // 
            // progress2
            // 
            this.progress2.Name = "progress2";
            this.progress2.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            resources.ApplyResources(this.progress2, "progress2");
            // 
            // gridSplit
            // 
            this.gridSplit.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            resources.ApplyResources(this.gridSplit, "gridSplit");
            this.gridSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.gridSplit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridSplit.Name = "gridSplit";
            // 
            // gridSplit.Panel1
            // 
            this.gridSplit.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.gridSplit.Panel1.Controls.Add(this.gridTabs);
            this.gridSplit.Panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            // 
            // gridSplit.Panel2
            // 
            this.gridSplit.Panel2.CausesValidation = false;
            this.gridSplit.TabStop = false;
            // 
            // gridTabs
            // 
            this.gridTabs.Controls.Add(this.tabJoints);
            this.gridTabs.Controls.Add(this.tabFrames);
            resources.ApplyResources(this.gridTabs, "gridTabs");
            this.gridTabs.HotTrack = true;
            this.gridTabs.Name = "gridTabs";
            this.gridTabs.SelectedIndex = 0;
            // 
            // tabJoints
            // 
            this.tabJoints.Controls.Add(this.jointGridView);
            this.tabJoints.Controls.Add(this.jointsToolStrip);
            resources.ApplyResources(this.tabJoints, "tabJoints");
            this.tabJoints.Name = "tabJoints";
            this.tabJoints.UseVisualStyleBackColor = true;
            // 
            // jointsToolStrip
            // 
            this.jointsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.jointsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jointFillDownButton,
            this.selectSimilarJointButton,
            this.toolStripSeparator21,
            this.restraintAllButton,
            this.restraintTransButton,
            this.restraintZButton,
            this.restraintFreeButton});
            resources.ApplyResources(this.jointsToolStrip, "jointsToolStrip");
            this.jointsToolStrip.Name = "jointsToolStrip";
            // 
            // jointFillDownButton
            // 
            this.jointFillDownButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.jointFillDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.jointFillDownButton.Image = global::Canguro.Properties.Resources.FillDownHS;
            resources.ApplyResources(this.jointFillDownButton, "jointFillDownButton");
            this.jointFillDownButton.Name = "jointFillDownButton";
            this.jointFillDownButton.Click += new System.EventHandler(this.jointFillDownButton_Click);
            // 
            // selectSimilarJointButton
            // 
            this.selectSimilarJointButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectSimilarJointButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectSimilarJointButton.Image = global::Canguro.Properties.Resources.selectsimilar;
            resources.ApplyResources(this.selectSimilarJointButton, "selectSimilarJointButton");
            this.selectSimilarJointButton.Name = "selectSimilarJointButton";
            this.selectSimilarJointButton.Click += new System.EventHandler(this.selectSimilarJointButton_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            // 
            // restraintAllButton
            // 
            this.restraintAllButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restraintAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restraintAllButton.Image = global::Canguro.Properties.Resources.RestraintAll;
            resources.ApplyResources(this.restraintAllButton, "restraintAllButton");
            this.restraintAllButton.Name = "restraintAllButton";
            this.restraintAllButton.Click += new System.EventHandler(this.restraintAllButton_Click);
            // 
            // restraintTransButton
            // 
            this.restraintTransButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restraintTransButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restraintTransButton.Image = global::Canguro.Properties.Resources.RestraintTrans;
            resources.ApplyResources(this.restraintTransButton, "restraintTransButton");
            this.restraintTransButton.Name = "restraintTransButton";
            this.restraintTransButton.Click += new System.EventHandler(this.restraintTransButton_Click);
            // 
            // restraintZButton
            // 
            this.restraintZButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restraintZButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restraintZButton.Image = global::Canguro.Properties.Resources.RestraintTransZ;
            resources.ApplyResources(this.restraintZButton, "restraintZButton");
            this.restraintZButton.Name = "restraintZButton";
            this.restraintZButton.Click += new System.EventHandler(this.restraintZButton_Click);
            // 
            // restraintFreeButton
            // 
            this.restraintFreeButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restraintFreeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restraintFreeButton.Image = global::Canguro.Properties.Resources.RestraintFree;
            resources.ApplyResources(this.restraintFreeButton, "restraintFreeButton");
            this.restraintFreeButton.Name = "restraintFreeButton";
            this.restraintFreeButton.Click += new System.EventHandler(this.restraintFreeButton_Click);
            // 
            // tabFrames
            // 
            this.tabFrames.Controls.Add(this.lineGridView);
            this.tabFrames.Controls.Add(this.linesToolStrip);
            resources.ApplyResources(this.tabFrames, "tabFrames");
            this.tabFrames.Name = "tabFrames";
            this.tabFrames.UseVisualStyleBackColor = true;
            // 
            // linesToolStrip
            // 
            this.linesToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.linesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineFillDownButton,
            this.selectSimilarLineButton,
            this.toolStripSeparator22,
            this.axialReleaseButton,
            this.momentReleaseButton,
            this.noReleaseButton});
            resources.ApplyResources(this.linesToolStrip, "linesToolStrip");
            this.linesToolStrip.Name = "linesToolStrip";
            // 
            // lineFillDownButton
            // 
            this.lineFillDownButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lineFillDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lineFillDownButton.Image = global::Canguro.Properties.Resources.FillDownHS;
            resources.ApplyResources(this.lineFillDownButton, "lineFillDownButton");
            this.lineFillDownButton.Name = "lineFillDownButton";
            this.lineFillDownButton.Click += new System.EventHandler(this.lineFillDownButton_Click);
            // 
            // selectSimilarLineButton
            // 
            this.selectSimilarLineButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectSimilarLineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectSimilarLineButton.Image = global::Canguro.Properties.Resources.selectsimilar;
            resources.ApplyResources(this.selectSimilarLineButton, "selectSimilarLineButton");
            this.selectSimilarLineButton.Name = "selectSimilarLineButton";
            this.selectSimilarLineButton.Click += new System.EventHandler(this.selectSimilarLineButton_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            // 
            // axialReleaseButton
            // 
            this.axialReleaseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.axialReleaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.axialReleaseButton.Image = global::Canguro.Properties.Resources.axialrelease;
            resources.ApplyResources(this.axialReleaseButton, "axialReleaseButton");
            this.axialReleaseButton.Name = "axialReleaseButton";
            this.axialReleaseButton.Click += new System.EventHandler(this.axialReleaseButton_Click);
            // 
            // momentReleaseButton
            // 
            this.momentReleaseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.momentReleaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.momentReleaseButton.Image = global::Canguro.Properties.Resources.momentrelease;
            resources.ApplyResources(this.momentReleaseButton, "momentReleaseButton");
            this.momentReleaseButton.Name = "momentReleaseButton";
            this.momentReleaseButton.Click += new System.EventHandler(this.momentReleaseButton_Click);
            // 
            // noReleaseButton
            // 
            this.noReleaseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.noReleaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.noReleaseButton.Image = global::Canguro.Properties.Resources.norelease;
            resources.ApplyResources(this.noReleaseButton, "noReleaseButton");
            this.noReleaseButton.Name = "noReleaseButton";
            this.noReleaseButton.Click += new System.EventHandler(this.noReleaseButton_Click);
            // 
            // modelToolStrip
            // 
            resources.ApplyResources(this.modelToolStrip, "modelToolStrip");
            this.modelToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jointButton,
            this.lineStripButton,
            this.lineButton,
            this.arcButton,
            this.toolStripSeparator14,
            this.sectionsButton,
            this.materialsButton,
            this.toolStripSeparator23,
            this.moveButton,
            this.mirrorButton,
            this.scaleButton,
            this.arrayButton,
            this.polararrayButton,
            this.modelrotateButton,
            this.toolStripSeparator25,
            this.splitButton,
            this.intersectButton,
            this.joinButton,
            this.flipLineButton,
            this.constraintsButton,
            this.diaphragmsButton});
            this.modelToolStrip.Name = "modelToolStrip";
            // 
            // jointButton
            // 
            this.jointButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.jointButton.Image = global::Canguro.Properties.Resources.adjoint;
            this.jointButton.Name = "jointButton";
            resources.ApplyResources(this.jointButton, "jointButton");
            this.jointButton.Click += new System.EventHandler(this.jointButton_Click);
            // 
            // lineStripButton
            // 
            this.lineStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lineStripButton.Image = global::Canguro.Properties.Resources.addlinestript;
            resources.ApplyResources(this.lineStripButton, "lineStripButton");
            this.lineStripButton.Name = "lineStripButton";
            this.lineStripButton.Click += new System.EventHandler(this.lineStripButton_Click);
            // 
            // lineButton
            // 
            this.lineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lineButton.Image = global::Canguro.Properties.Resources.addline1;
            resources.ApplyResources(this.lineButton, "lineButton");
            this.lineButton.Name = "lineButton";
            this.lineButton.Click += new System.EventHandler(this.lineButton_Click);
            // 
            // arcButton
            // 
            this.arcButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.arcButton.Image = global::Canguro.Properties.Resources.arc;
            resources.ApplyResources(this.arcButton, "arcButton");
            this.arcButton.Name = "arcButton";
            this.arcButton.Click += new System.EventHandler(this.arcButton_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // sectionsButton
            // 
            this.sectionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sectionsButton.Image = global::Canguro.Properties.Resources.sections;
            resources.ApplyResources(this.sectionsButton, "sectionsButton");
            this.sectionsButton.Name = "sectionsButton";
            this.sectionsButton.Click += new System.EventHandler(this.sectionsToolStripMenuItem_Click);
            // 
            // materialsButton
            // 
            this.materialsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.materialsButton.Image = global::Canguro.Properties.Resources.Materials;
            resources.ApplyResources(this.materialsButton, "materialsButton");
            this.materialsButton.Name = "materialsButton";
            this.materialsButton.Click += new System.EventHandler(this.materialsToolStripMenuItem_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            // 
            // moveButton
            // 
            this.moveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveButton.Image = global::Canguro.Properties.Resources.move;
            resources.ApplyResources(this.moveButton, "moveButton");
            this.moveButton.Name = "moveButton";
            this.moveButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // mirrorButton
            // 
            this.mirrorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mirrorButton.Image = global::Canguro.Properties.Resources.mirror;
            resources.ApplyResources(this.mirrorButton, "mirrorButton");
            this.mirrorButton.Name = "mirrorButton";
            this.mirrorButton.Click += new System.EventHandler(this.mirrorButton_Click);
            // 
            // scaleButton
            // 
            this.scaleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.scaleButton.Image = global::Canguro.Properties.Resources.scale;
            resources.ApplyResources(this.scaleButton, "scaleButton");
            this.scaleButton.Name = "scaleButton";
            this.scaleButton.Click += new System.EventHandler(this.scaleButton_Click);
            // 
            // arrayButton
            // 
            this.arrayButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.arrayButton.Image = global::Canguro.Properties.Resources.array;
            resources.ApplyResources(this.arrayButton, "arrayButton");
            this.arrayButton.Name = "arrayButton";
            this.arrayButton.Click += new System.EventHandler(this.arrayButton_Click);
            // 
            // polararrayButton
            // 
            this.polararrayButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.polararrayButton.Image = global::Canguro.Properties.Resources.polarray;
            resources.ApplyResources(this.polararrayButton, "polararrayButton");
            this.polararrayButton.Name = "polararrayButton";
            this.polararrayButton.Click += new System.EventHandler(this.polararrayButton_Click);
            // 
            // modelrotateButton
            // 
            this.modelrotateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.modelrotateButton.Image = global::Canguro.Properties.Resources.modelrotate;
            resources.ApplyResources(this.modelrotateButton, "modelrotateButton");
            this.modelrotateButton.Name = "modelrotateButton";
            this.modelrotateButton.Click += new System.EventHandler(this.modelrotateButton_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // splitButton
            // 
            this.splitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.splitButton.Image = global::Canguro.Properties.Resources.split;
            resources.ApplyResources(this.splitButton, "splitButton");
            this.splitButton.Name = "splitButton";
            this.splitButton.Click += new System.EventHandler(this.splitButton_Click);
            // 
            // intersectButton
            // 
            this.intersectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.intersectButton.Image = global::Canguro.Properties.Resources.Intersect;
            resources.ApplyResources(this.intersectButton, "intersectButton");
            this.intersectButton.Name = "intersectButton";
            this.intersectButton.Click += new System.EventHandler(this.intersectButton_Click);
            // 
            // joinButton
            // 
            this.joinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.joinButton.Image = global::Canguro.Properties.Resources.join;
            resources.ApplyResources(this.joinButton, "joinButton");
            this.joinButton.Name = "joinButton";
            this.joinButton.Click += new System.EventHandler(this.joinButton_Click);
            // 
            // flipLineButton
            // 
            this.flipLineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipLineButton.Image = global::Canguro.Properties.Resources.flipline;
            resources.ApplyResources(this.flipLineButton, "flipLineButton");
            this.flipLineButton.Name = "flipLineButton";
            this.flipLineButton.Click += new System.EventHandler(this.flipLineButton_Click);
            // 
            // constraintsButton
            // 
            this.constraintsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.constraintsButton.Image = global::Canguro.Properties.Resources.constraints;
            resources.ApplyResources(this.constraintsButton, "constraintsButton");
            this.constraintsButton.Name = "constraintsButton";
            this.constraintsButton.Click += new System.EventHandler(this.constraintsButton_Click);
            // 
            // diaphragmsButton
            // 
            this.diaphragmsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.diaphragmsButton.Image = global::Canguro.Properties.Resources.diaphragms;
            resources.ApplyResources(this.diaphragmsButton, "diaphragmsButton");
            this.diaphragmsButton.Name = "diaphragmsButton";
            this.diaphragmsButton.Click += new System.EventHandler(this.diaphragmsButton_Click);
            // 
            // loadToolStrip
            // 
            resources.ApplyResources(this.loadToolStrip, "loadToolStrip");
            this.loadToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analysisCaseButton,
            this.loadCaseButton,
            this.editLoadCaseButton,
            this.deleteLoadCaseButton,
            this.loadCasesComboBox,
            this.loadComboButton,
            this.toolStripSeparator15,
            this.forceLoadButton,
            this.groundDisplacementButton,
            this.concentratedLoadButton,
            this.distributedLoadButton,
            this.uniformLineLoadButton,
            this.triangleLineLoadButton,
            this.temperatureLineLoadButton,
            this.temperatureGradientLineLoadButton});
            this.loadToolStrip.Name = "loadToolStrip";
            // 
            // analysisCaseButton
            // 
            this.analysisCaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.analysisCaseButton.Image = global::Canguro.Properties.Resources.analisiscase;
            this.analysisCaseButton.Name = "analysisCaseButton";
            resources.ApplyResources(this.analysisCaseButton, "analysisCaseButton");
            this.analysisCaseButton.Click += new System.EventHandler(this.analysisCaseButton_Click);
            // 
            // loadCaseButton
            // 
            this.loadCaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadCaseButton.Image = global::Canguro.Properties.Resources.loadcase;
            this.loadCaseButton.Name = "loadCaseButton";
            resources.ApplyResources(this.loadCaseButton, "loadCaseButton");
            this.loadCaseButton.Click += new System.EventHandler(this.loadCaseButton_Click);
            // 
            // editLoadCaseButton
            // 
            this.editLoadCaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editLoadCaseButton.Image = global::Canguro.Properties.Resources.editloadcase;
            resources.ApplyResources(this.editLoadCaseButton, "editLoadCaseButton");
            this.editLoadCaseButton.Name = "editLoadCaseButton";
            this.editLoadCaseButton.Click += new System.EventHandler(this.editLoadCaseButton_Click);
            // 
            // deleteLoadCaseButton
            // 
            this.deleteLoadCaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteLoadCaseButton.Image = global::Canguro.Properties.Resources.deleteloadcase;
            resources.ApplyResources(this.deleteLoadCaseButton, "deleteLoadCaseButton");
            this.deleteLoadCaseButton.Name = "deleteLoadCaseButton";
            this.deleteLoadCaseButton.Click += new System.EventHandler(this.deleteLoadCaseButton_Click);
            // 
            // loadCasesComboBox
            // 
            this.loadCasesComboBox.AutoToolTip = true;
            this.loadCasesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.loadCasesComboBox.DropDownWidth = 200;
            this.loadCasesComboBox.Name = "loadCasesComboBox";
            resources.ApplyResources(this.loadCasesComboBox, "loadCasesComboBox");
            this.loadCasesComboBox.SelectedIndexChanged += new System.EventHandler(this.loadCasesComboBox_SelectedIndexChanged);
            // 
            // loadComboButton
            // 
            this.loadComboButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadComboButton.Image = global::Canguro.Properties.Resources.loadcombination;
            this.loadComboButton.Name = "loadComboButton";
            resources.ApplyResources(this.loadComboButton, "loadComboButton");
            this.loadComboButton.Click += new System.EventHandler(this.loadComboButton_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // forceLoadButton
            // 
            this.forceLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forceLoadButton.Image = global::Canguro.Properties.Resources.forceload;
            resources.ApplyResources(this.forceLoadButton, "forceLoadButton");
            this.forceLoadButton.Name = "forceLoadButton";
            this.forceLoadButton.Click += new System.EventHandler(this.forceLoadButton_Click);
            // 
            // groundDisplacementButton
            // 
            this.groundDisplacementButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.groundDisplacementButton.Image = global::Canguro.Properties.Resources.grounddisplacementload;
            resources.ApplyResources(this.groundDisplacementButton, "groundDisplacementButton");
            this.groundDisplacementButton.Name = "groundDisplacementButton";
            this.groundDisplacementButton.Click += new System.EventHandler(this.groundDisplacementButton_Click);
            // 
            // concentratedLoadButton
            // 
            this.concentratedLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.concentratedLoadButton, "concentratedLoadButton");
            this.concentratedLoadButton.Name = "concentratedLoadButton";
            this.concentratedLoadButton.Click += new System.EventHandler(this.concentratedLoadButton_Click);
            // 
            // distributedLoadButton
            // 
            this.distributedLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.distributedLoadButton.Image = global::Canguro.Properties.Resources.distributedexpandload;
            resources.ApplyResources(this.distributedLoadButton, "distributedLoadButton");
            this.distributedLoadButton.Name = "distributedLoadButton";
            this.distributedLoadButton.Click += new System.EventHandler(this.distributedLoadButton_Click);
            // 
            // uniformLineLoadButton
            // 
            this.uniformLineLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uniformLineLoadButton.Image = global::Canguro.Properties.Resources.uniformLineLoad;
            resources.ApplyResources(this.uniformLineLoadButton, "uniformLineLoadButton");
            this.uniformLineLoadButton.Name = "uniformLineLoadButton";
            this.uniformLineLoadButton.Click += new System.EventHandler(this.uniformLineLoadButton_Click);
            // 
            // triangleLineLoadButton
            // 
            this.triangleLineLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.triangleLineLoadButton.Image = global::Canguro.Properties.Resources.triangleLineLoad;
            resources.ApplyResources(this.triangleLineLoadButton, "triangleLineLoadButton");
            this.triangleLineLoadButton.Name = "triangleLineLoadButton";
            this.triangleLineLoadButton.Click += new System.EventHandler(this.triangleLineLoadButton_Click);
            // 
            // temperatureLineLoadButton
            // 
            this.temperatureLineLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.temperatureLineLoadButton.Image = global::Canguro.Properties.Resources.temperatureLineLoad;
            resources.ApplyResources(this.temperatureLineLoadButton, "temperatureLineLoadButton");
            this.temperatureLineLoadButton.Name = "temperatureLineLoadButton";
            this.temperatureLineLoadButton.Click += new System.EventHandler(this.temperatureLineLoadButton_Click);
            // 
            // temperatureGradientLineLoadButton
            // 
            this.temperatureGradientLineLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.temperatureGradientLineLoadButton.Image = global::Canguro.Properties.Resources.temperatureGradientLineLoad;
            resources.ApplyResources(this.temperatureGradientLineLoadButton, "temperatureGradientLineLoadButton");
            this.temperatureGradientLineLoadButton.Name = "temperatureGradientLineLoadButton";
            this.temperatureGradientLineLoadButton.Click += new System.EventHandler(this.temperatureGradientLineLoadButton_Click);
            // 
            // templatesToolStrip
            // 
            resources.ApplyResources(this.templatesToolStrip, "templatesToolStrip");
            this.templatesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridButton,
            this.domeButton,
            this.cylinderButton});
            this.templatesToolStrip.Name = "templatesToolStrip";
            // 
            // gridButton
            // 
            this.gridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.gridButton.Image = global::Canguro.Properties.Resources.grid;
            resources.ApplyResources(this.gridButton, "gridButton");
            this.gridButton.Name = "gridButton";
            this.gridButton.Click += new System.EventHandler(this.gridButton_Click);
            // 
            // domeButton
            // 
            this.domeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.domeButton.Image = global::Canguro.Properties.Resources.dome;
            resources.ApplyResources(this.domeButton, "domeButton");
            this.domeButton.Name = "domeButton";
            this.domeButton.Click += new System.EventHandler(this.domeButton_Click);
            // 
            // cylinderButton
            // 
            this.cylinderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cylinderButton.Image = global::Canguro.Properties.Resources.cylinder;
            resources.ApplyResources(this.cylinderButton, "cylinderButton");
            this.cylinderButton.Name = "cylinderButton";
            this.cylinderButton.Click += new System.EventHandler(this.cylinderButton_Click);
            // 
            // viewToolStrip
            // 
            resources.ApplyResources(this.viewToolStrip, "viewToolStrip");
            this.viewToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectButton,
            this.zoomButton,
            this.panButton,
            this.rotateButton,
            this.predefinedxy,
            this.predefinedxz,
            this.predefinedyz,
            this.predefinedxyzButton,
            this.zoomInButton,
            this.zoomOutButton,
            this.zoomPreviousButton,
            this.viewAllButton,
            this.viewShadedButton,
            this.colorsSplitButton,
            this.showTextDropDownButton,
            this.showLoadsButton});
            this.viewToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.viewToolStrip.Name = "viewToolStrip";
            // 
            // selectButton
            // 
            this.selectButton.Checked = true;
            this.selectButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.selectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.selectButton, "selectButton");
            this.selectButton.Name = "selectButton";
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // zoomButton
            // 
            this.zoomButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomButton.Image = global::Canguro.Properties.Resources.zoom;
            resources.ApplyResources(this.zoomButton, "zoomButton");
            this.zoomButton.Name = "zoomButton";
            this.zoomButton.Click += new System.EventHandler(this.zoomButton_Click);
            // 
            // panButton
            // 
            this.panButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.panButton, "panButton");
            this.panButton.Name = "panButton";
            this.panButton.Click += new System.EventHandler(this.panButton_Click);
            // 
            // rotateButton
            // 
            this.rotateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateButton.Image = global::Canguro.Properties.Resources.rotate;
            resources.ApplyResources(this.rotateButton, "rotateButton");
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Click += new System.EventHandler(this.rotateButton_Click);
            // 
            // predefinedxy
            // 
            this.predefinedxy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.predefinedxy.Image = global::Canguro.Properties.Resources.predefinedxy;
            resources.ApplyResources(this.predefinedxy, "predefinedxy");
            this.predefinedxy.Name = "predefinedxy";
            this.predefinedxy.Click += new System.EventHandler(this.predefinedxy_Click);
            // 
            // predefinedxz
            // 
            this.predefinedxz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.predefinedxz.Image = global::Canguro.Properties.Resources.predefinedxz;
            resources.ApplyResources(this.predefinedxz, "predefinedxz");
            this.predefinedxz.Name = "predefinedxz";
            this.predefinedxz.Click += new System.EventHandler(this.predefinedxz_Click);
            // 
            // predefinedyz
            // 
            this.predefinedyz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.predefinedyz.Image = global::Canguro.Properties.Resources.predefinedyz;
            resources.ApplyResources(this.predefinedyz, "predefinedyz");
            this.predefinedyz.Name = "predefinedyz";
            this.predefinedyz.Click += new System.EventHandler(this.predefinedyz_Click);
            // 
            // predefinedxyzButton
            // 
            this.predefinedxyzButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.predefinedxyzButton.Image = global::Canguro.Properties.Resources.predefinedXyz;
            resources.ApplyResources(this.predefinedxyzButton, "predefinedxyzButton");
            this.predefinedxyzButton.Name = "predefinedxyzButton";
            this.predefinedxyzButton.Click += new System.EventHandler(this.predefinedxyzButton_Click);
            // 
            // zoomInButton
            // 
            this.zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInButton.Image = global::Canguro.Properties.Resources.zoommin;
            resources.ApplyResources(this.zoomInButton, "zoomInButton");
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutButton.Image = global::Canguro.Properties.Resources.zoomout;
            resources.ApplyResources(this.zoomOutButton, "zoomOutButton");
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
            // 
            // zoomPreviousButton
            // 
            this.zoomPreviousButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomPreviousButton.Image = global::Canguro.Properties.Resources.zoompreviews;
            resources.ApplyResources(this.zoomPreviousButton, "zoomPreviousButton");
            this.zoomPreviousButton.Name = "zoomPreviousButton";
            this.zoomPreviousButton.Click += new System.EventHandler(this.zoomPreviousButton_Click);
            // 
            // viewAllButton
            // 
            this.viewAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewAllButton.Image = global::Canguro.Properties.Resources.zoomall;
            resources.ApplyResources(this.viewAllButton, "viewAllButton");
            this.viewAllButton.Name = "viewAllButton";
            this.viewAllButton.Click += new System.EventHandler(this.viewAllButton_Click);
            // 
            // viewShadedButton
            // 
            this.viewShadedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.viewShadedButton, "viewShadedButton");
            this.viewShadedButton.Name = "viewShadedButton";
            this.viewShadedButton.Click += new System.EventHandler(this.viewShadedButton_Click);
            // 
            // colorsSplitButton
            // 
            this.colorsSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.colorsSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorMaterialsToolStripMenuItem2,
            this.colorSectionsToolStripMenuItem2,
            this.colorLayersToolStripMenuItem2,
            this.colorLoadsToolStripMenuItem2,
            this.colorConstraintsToolStripMenuItem2});
            this.colorsSplitButton.Image = global::Canguro.Properties.Resources.colors;
            resources.ApplyResources(this.colorsSplitButton, "colorsSplitButton");
            this.colorsSplitButton.Name = "colorsSplitButton";
            // 
            // colorMaterialsToolStripMenuItem2
            // 
            this.colorMaterialsToolStripMenuItem2.Name = "colorMaterialsToolStripMenuItem2";
            resources.ApplyResources(this.colorMaterialsToolStripMenuItem2, "colorMaterialsToolStripMenuItem2");
            this.colorMaterialsToolStripMenuItem2.Click += new System.EventHandler(this.colorMaterialsToolStripMenuItem2_Click);
            // 
            // colorSectionsToolStripMenuItem2
            // 
            this.colorSectionsToolStripMenuItem2.Name = "colorSectionsToolStripMenuItem2";
            resources.ApplyResources(this.colorSectionsToolStripMenuItem2, "colorSectionsToolStripMenuItem2");
            this.colorSectionsToolStripMenuItem2.Click += new System.EventHandler(this.colorSectionsToolStripMenuItem2_Click);
            // 
            // colorLayersToolStripMenuItem2
            // 
            this.colorLayersToolStripMenuItem2.Name = "colorLayersToolStripMenuItem2";
            resources.ApplyResources(this.colorLayersToolStripMenuItem2, "colorLayersToolStripMenuItem2");
            this.colorLayersToolStripMenuItem2.Click += new System.EventHandler(this.colorLayersToolStripMenuItem2_Click);
            // 
            // colorLoadsToolStripMenuItem2
            // 
            this.colorLoadsToolStripMenuItem2.Name = "colorLoadsToolStripMenuItem2";
            resources.ApplyResources(this.colorLoadsToolStripMenuItem2, "colorLoadsToolStripMenuItem2");
            this.colorLoadsToolStripMenuItem2.Click += new System.EventHandler(this.colorLoadsToolStripMenuItem2_Click);
            // 
            // colorConstraintsToolStripMenuItem2
            // 
            this.colorConstraintsToolStripMenuItem2.Name = "colorConstraintsToolStripMenuItem2";
            resources.ApplyResources(this.colorConstraintsToolStripMenuItem2, "colorConstraintsToolStripMenuItem2");
            this.colorConstraintsToolStripMenuItem2.Click += new System.EventHandler(this.colorConstraintsToolStripMenuItem_Click);
            // 
            // showTextDropDownButton
            // 
            this.showTextDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showTextDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.showJointIDToolStripMenuItem2,
            this.hideJointsToolStripMenuItem,
            this.jointCoordinatesToolStripMenuItem,
            this.showDOFToolStripMenuItem,
            this.showLineIDToolStripMenuItem2,
            this.showLineLengthToolStripMenuItem2,
            this.showLocalAxesToolStripMenuItem2,
            this.showSectionsToolStripMenuItem2,
            this.releasesToolStripMenuItem,
            this.showAxesToolStripMenuItem2,
            this.showFloorToolStripMenuItem2,
            this.loadSizeToolStripMenuItem2});
            this.showTextDropDownButton.Image = global::Canguro.Properties.Resources.Showtexts;
            resources.ApplyResources(this.showTextDropDownButton, "showTextDropDownButton");
            this.showTextDropDownButton.Name = "showTextDropDownButton";
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            resources.ApplyResources(this.noneToolStripMenuItem, "noneToolStripMenuItem");
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // showJointIDToolStripMenuItem2
            // 
            this.showJointIDToolStripMenuItem2.Name = "showJointIDToolStripMenuItem2";
            resources.ApplyResources(this.showJointIDToolStripMenuItem2, "showJointIDToolStripMenuItem2");
            this.showJointIDToolStripMenuItem2.Click += new System.EventHandler(this.jointIDToolStripMenuItem_Click);
            // 
            // hideJointsToolStripMenuItem
            // 
            this.hideJointsToolStripMenuItem.Name = "hideJointsToolStripMenuItem";
            resources.ApplyResources(this.hideJointsToolStripMenuItem, "hideJointsToolStripMenuItem");
            this.hideJointsToolStripMenuItem.Click += new System.EventHandler(this.hideJointsToolStripMenuItem_Click);
            // 
            // jointCoordinatesToolStripMenuItem
            // 
            this.jointCoordinatesToolStripMenuItem.Name = "jointCoordinatesToolStripMenuItem";
            resources.ApplyResources(this.jointCoordinatesToolStripMenuItem, "jointCoordinatesToolStripMenuItem");
            this.jointCoordinatesToolStripMenuItem.Click += new System.EventHandler(this.jointCoordinatesToolStripMenuItem_Click);
            // 
            // showDOFToolStripMenuItem
            // 
            this.showDOFToolStripMenuItem.Name = "showDOFToolStripMenuItem";
            resources.ApplyResources(this.showDOFToolStripMenuItem, "showDOFToolStripMenuItem");
            this.showDOFToolStripMenuItem.Click += new System.EventHandler(this.showDOFToolStripMenuItem_Click);
            // 
            // showLineIDToolStripMenuItem2
            // 
            this.showLineIDToolStripMenuItem2.Name = "showLineIDToolStripMenuItem2";
            resources.ApplyResources(this.showLineIDToolStripMenuItem2, "showLineIDToolStripMenuItem2");
            this.showLineIDToolStripMenuItem2.Click += new System.EventHandler(this.lineIDToolStripMenuItem_Click);
            // 
            // showLineLengthToolStripMenuItem2
            // 
            this.showLineLengthToolStripMenuItem2.Name = "showLineLengthToolStripMenuItem2";
            resources.ApplyResources(this.showLineLengthToolStripMenuItem2, "showLineLengthToolStripMenuItem2");
            this.showLineLengthToolStripMenuItem2.Click += new System.EventHandler(this.lineLengthToolStripMenuItem_Click);
            // 
            // showLocalAxesToolStripMenuItem2
            // 
            this.showLocalAxesToolStripMenuItem2.Name = "showLocalAxesToolStripMenuItem2";
            resources.ApplyResources(this.showLocalAxesToolStripMenuItem2, "showLocalAxesToolStripMenuItem2");
            this.showLocalAxesToolStripMenuItem2.Click += new System.EventHandler(this.showLocalAxesToolStripMenuItem_Click);
            // 
            // showSectionsToolStripMenuItem2
            // 
            this.showSectionsToolStripMenuItem2.Name = "showSectionsToolStripMenuItem2";
            resources.ApplyResources(this.showSectionsToolStripMenuItem2, "showSectionsToolStripMenuItem2");
            this.showSectionsToolStripMenuItem2.Click += new System.EventHandler(this.showSectionsToolStripMenuItem_Click);
            // 
            // releasesToolStripMenuItem
            // 
            this.releasesToolStripMenuItem.Name = "releasesToolStripMenuItem";
            resources.ApplyResources(this.releasesToolStripMenuItem, "releasesToolStripMenuItem");
            this.releasesToolStripMenuItem.Click += new System.EventHandler(this.releasesToolStripMenuItem_Click);
            // 
            // showAxesToolStripMenuItem2
            // 
            this.showAxesToolStripMenuItem2.Name = "showAxesToolStripMenuItem2";
            resources.ApplyResources(this.showAxesToolStripMenuItem2, "showAxesToolStripMenuItem2");
            this.showAxesToolStripMenuItem2.Click += new System.EventHandler(this.viewAxesToolStripMenuItem_Click);
            // 
            // showFloorToolStripMenuItem2
            // 
            this.showFloorToolStripMenuItem2.Name = "showFloorToolStripMenuItem2";
            resources.ApplyResources(this.showFloorToolStripMenuItem2, "showFloorToolStripMenuItem2");
            this.showFloorToolStripMenuItem2.Click += new System.EventHandler(this.viewFloorToolStripMenuItem_Click);
            // 
            // loadSizeToolStripMenuItem2
            // 
            this.loadSizeToolStripMenuItem2.Name = "loadSizeToolStripMenuItem2";
            resources.ApplyResources(this.loadSizeToolStripMenuItem2, "loadSizeToolStripMenuItem2");
            this.loadSizeToolStripMenuItem2.Click += new System.EventHandler(this.loadSizesToolStripMenuItem_Click);
            // 
            // showLoadsButton
            // 
            this.showLoadsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showLoadsButton.Image = global::Canguro.Properties.Resources.show_loads;
            resources.ApplyResources(this.showLoadsButton, "showLoadsButton");
            this.showLoadsButton.Name = "showLoadsButton";
            this.showLoadsButton.Click += new System.EventHandler(this.showLoadsButton_Click);
            // 
            // resultsToolStrip
            // 
            resources.ApplyResources(this.resultsToolStrip, "resultsToolStrip");
            this.resultsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzeButton,
            this.resultsCasesCombo,
            this.toolStripSeparator1,
            this.showDeformedButton,
            this.animateButton,
            this.showStressesButton,
            this.showDesignButton,
            this.diagramsDropDownButton,
            this.reportButton,
            this.showJointReactionsButton,
            this.showJointReactionsTextsButton});
            this.resultsToolStrip.Name = "resultsToolStrip";
            // 
            // analyzeButton
            // 
            this.analyzeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.analyzeButton.Image = global::Canguro.Properties.Resources.analize;
            resources.ApplyResources(this.analyzeButton, "analyzeButton");
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Click += new System.EventHandler(this.analizarToolStripMenuItem_Click);
            // 
            // resultsCasesCombo
            // 
            this.resultsCasesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resultsCasesCombo.DropDownWidth = 200;
            this.resultsCasesCombo.Name = "resultsCasesCombo";
            resources.ApplyResources(this.resultsCasesCombo, "resultsCasesCombo");
            this.resultsCasesCombo.SelectedIndexChanged += new System.EventHandler(this.resultsCasesCombo_SelectedIndexChanged);
            this.resultsCasesCombo.Click += new System.EventHandler(this.resultsCasesCombo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // showDeformedButton
            // 
            this.showDeformedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showDeformedButton.Image = global::Canguro.Properties.Resources.deformed;
            resources.ApplyResources(this.showDeformedButton, "showDeformedButton");
            this.showDeformedButton.Name = "showDeformedButton";
            this.showDeformedButton.Click += new System.EventHandler(this.showDeformedButton_Click);
            // 
            // animateButton
            // 
            this.animateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.animateButton.Image = global::Canguro.Properties.Resources.animation;
            resources.ApplyResources(this.animateButton, "animateButton");
            this.animateButton.Name = "animateButton";
            this.animateButton.Click += new System.EventHandler(this.animateButton_Click);
            // 
            // showStressesButton
            // 
            this.showStressesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showStressesButton.Image = global::Canguro.Properties.Resources.showstresses;
            resources.ApplyResources(this.showStressesButton, "showStressesButton");
            this.showStressesButton.Name = "showStressesButton";
            this.showStressesButton.Click += new System.EventHandler(this.showStressesButton_Click);
            // 
            // showDesignButton
            // 
            this.showDesignButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showDesignButton.Image = global::Canguro.Properties.Resources.Design;
            resources.ApplyResources(this.showDesignButton, "showDesignButton");
            this.showDesignButton.Name = "showDesignButton";
            this.showDesignButton.Click += new System.EventHandler(this.showDesignButton_Click);
            // 
            // diagramsDropDownButton
            // 
            this.diagramsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.diagramsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noDiagramsToolStripMenuItem2,
            this.axialDiagramToolStripMenuItem2,
            this.s2DiagramToolStripMenuItem2,
            this.s3DiagramToolStripMenuItem2,
            this.torsionDiagramToolStripMenuItem2,
            this.m2DiagramToolStripMenuItem2,
            this.m3DiagramToolStripMenuItem2});
            this.diagramsDropDownButton.Image = global::Canguro.Properties.Resources.alldiagrams;
            resources.ApplyResources(this.diagramsDropDownButton, "diagramsDropDownButton");
            this.diagramsDropDownButton.Name = "diagramsDropDownButton";
            // 
            // noDiagramsToolStripMenuItem2
            // 
            this.noDiagramsToolStripMenuItem2.Image = global::Canguro.Properties.Resources.hidediagrams;
            this.noDiagramsToolStripMenuItem2.Name = "noDiagramsToolStripMenuItem2";
            resources.ApplyResources(this.noDiagramsToolStripMenuItem2, "noDiagramsToolStripMenuItem2");
            this.noDiagramsToolStripMenuItem2.Click += new System.EventHandler(this.noneToolStripMenuItem1_Click);
            // 
            // axialDiagramToolStripMenuItem2
            // 
            this.axialDiagramToolStripMenuItem2.Image = global::Canguro.Properties.Resources.showaxial;
            this.axialDiagramToolStripMenuItem2.Name = "axialDiagramToolStripMenuItem2";
            resources.ApplyResources(this.axialDiagramToolStripMenuItem2, "axialDiagramToolStripMenuItem2");
            this.axialDiagramToolStripMenuItem2.Click += new System.EventHandler(this.axialToolStripMenuItem_Click);
            // 
            // s2DiagramToolStripMenuItem2
            // 
            this.s2DiagramToolStripMenuItem2.Image = global::Canguro.Properties.Resources.show_S1;
            this.s2DiagramToolStripMenuItem2.Name = "s2DiagramToolStripMenuItem2";
            resources.ApplyResources(this.s2DiagramToolStripMenuItem2, "s2DiagramToolStripMenuItem2");
            this.s2DiagramToolStripMenuItem2.Click += new System.EventHandler(this.s2ToolStripMenuItem_Click);
            // 
            // s3DiagramToolStripMenuItem2
            // 
            this.s3DiagramToolStripMenuItem2.Image = global::Canguro.Properties.Resources.show_S2;
            this.s3DiagramToolStripMenuItem2.Name = "s3DiagramToolStripMenuItem2";
            resources.ApplyResources(this.s3DiagramToolStripMenuItem2, "s3DiagramToolStripMenuItem2");
            this.s3DiagramToolStripMenuItem2.Click += new System.EventHandler(this.s3ToolStripMenuItem_Click);
            // 
            // torsionDiagramToolStripMenuItem2
            // 
            this.torsionDiagramToolStripMenuItem2.Image = global::Canguro.Properties.Resources.show_torcion;
            this.torsionDiagramToolStripMenuItem2.Name = "torsionDiagramToolStripMenuItem2";
            resources.ApplyResources(this.torsionDiagramToolStripMenuItem2, "torsionDiagramToolStripMenuItem2");
            this.torsionDiagramToolStripMenuItem2.Click += new System.EventHandler(this.torsionToolStripMenuItem_Click);
            // 
            // m2DiagramToolStripMenuItem2
            // 
            this.m2DiagramToolStripMenuItem2.Image = global::Canguro.Properties.Resources.show_M1;
            this.m2DiagramToolStripMenuItem2.Name = "m2DiagramToolStripMenuItem2";
            resources.ApplyResources(this.m2DiagramToolStripMenuItem2, "m2DiagramToolStripMenuItem2");
            this.m2DiagramToolStripMenuItem2.Click += new System.EventHandler(this.m2ToolStripMenuItem_Click);
            // 
            // m3DiagramToolStripMenuItem2
            // 
            this.m3DiagramToolStripMenuItem2.Image = global::Canguro.Properties.Resources.show_M2;
            this.m3DiagramToolStripMenuItem2.Name = "m3DiagramToolStripMenuItem2";
            resources.ApplyResources(this.m3DiagramToolStripMenuItem2, "m3DiagramToolStripMenuItem2");
            this.m3DiagramToolStripMenuItem2.Click += new System.EventHandler(this.m3ToolStripMenuItem_Click);
            // 
            // reportButton
            // 
            this.reportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reportButton.Image = global::Canguro.Properties.Resources.NewReportHS;
            resources.ApplyResources(this.reportButton, "reportButton");
            this.reportButton.Name = "reportButton";
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // showJointReactionsButton
            // 
            this.showJointReactionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showJointReactionsButton.Image = global::Canguro.Properties.Resources.jointreactions;
            resources.ApplyResources(this.showJointReactionsButton, "showJointReactionsButton");
            this.showJointReactionsButton.Name = "showJointReactionsButton";
            this.showJointReactionsButton.Click += new System.EventHandler(this.showJointReactionsButton_Click);
            // 
            // showJointReactionsTextsButton
            // 
            this.showJointReactionsTextsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showJointReactionsTextsButton.Image = global::Canguro.Properties.Resources.jointReactionsTexts;
            resources.ApplyResources(this.showJointReactionsTextsButton, "showJointReactionsTextsButton");
            this.showJointReactionsTextsButton.Name = "showJointReactionsTextsButton";
            this.showJointReactionsTextsButton.Click += new System.EventHandler(this.showJointReactionsTextsButton_Click);
            // 
            // layersToolStrip
            // 
            resources.ApplyResources(this.layersToolStrip, "layersToolStrip");
            this.layersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layerButton,
            this.editlayerButton,
            this.deletelayerButton,
            this.toolStripSeparator24,
            this.layersComboBox,
            this.hideLayerButton,
            this.showLayerButton,
            this.moveToLayerButton,
            this.activateLayerButton,
            this.selectLayerButton});
            this.layersToolStrip.Name = "layersToolStrip";
            // 
            // layerButton
            // 
            this.layerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.layerButton.Image = global::Canguro.Properties.Resources.addlayer;
            resources.ApplyResources(this.layerButton, "layerButton");
            this.layerButton.Name = "layerButton";
            this.layerButton.Click += new System.EventHandler(this.layerButton_Click);
            // 
            // editlayerButton
            // 
            this.editlayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editlayerButton.Image = global::Canguro.Properties.Resources.editlayer;
            resources.ApplyResources(this.editlayerButton, "editlayerButton");
            this.editlayerButton.Name = "editlayerButton";
            this.editlayerButton.Click += new System.EventHandler(this.editlayerButton_Click);
            // 
            // deletelayerButton
            // 
            this.deletelayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deletelayerButton.Image = global::Canguro.Properties.Resources.deletelayer;
            resources.ApplyResources(this.deletelayerButton, "deletelayerButton");
            this.deletelayerButton.Name = "deletelayerButton";
            this.deletelayerButton.Click += new System.EventHandler(this.deletelayerButton_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            // 
            // layersComboBox
            // 
            this.layersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.layersComboBox.DropDownWidth = 200;
            this.layersComboBox.Name = "layersComboBox";
            resources.ApplyResources(this.layersComboBox, "layersComboBox");
            this.layersComboBox.SelectedIndexChanged += new System.EventHandler(this.layersComboBox_SelectedIndexChanged);
            // 
            // hideLayerButton
            // 
            this.hideLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideLayerButton.Image = global::Canguro.Properties.Resources.hidelayer;
            resources.ApplyResources(this.hideLayerButton, "hideLayerButton");
            this.hideLayerButton.Name = "hideLayerButton";
            this.hideLayerButton.Click += new System.EventHandler(this.hideLayerButton_Click);
            // 
            // showLayerButton
            // 
            this.showLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showLayerButton.Image = global::Canguro.Properties.Resources.showlayer;
            resources.ApplyResources(this.showLayerButton, "showLayerButton");
            this.showLayerButton.Name = "showLayerButton";
            this.showLayerButton.Click += new System.EventHandler(this.showLayerButton_Click);
            // 
            // moveToLayerButton
            // 
            this.moveToLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveToLayerButton.Image = global::Canguro.Properties.Resources.move_to_corrent;
            resources.ApplyResources(this.moveToLayerButton, "moveToLayerButton");
            this.moveToLayerButton.Name = "moveToLayerButton";
            this.moveToLayerButton.Click += new System.EventHandler(this.moveToLayerButton_Click);
            // 
            // activateLayerButton
            // 
            this.activateLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.activateLayerButton.Image = global::Canguro.Properties.Resources.activate_layer;
            resources.ApplyResources(this.activateLayerButton, "activateLayerButton");
            this.activateLayerButton.Name = "activateLayerButton";
            this.activateLayerButton.Click += new System.EventHandler(this.activateLayerButton_Click);
            // 
            // selectLayerButton
            // 
            this.selectLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectLayerButton.Image = global::Canguro.Properties.Resources.select;
            resources.ApplyResources(this.selectLayerButton, "selectLayerButton");
            this.selectLayerButton.Name = "selectLayerButton";
            this.selectLayerButton.Click += new System.EventHandler(this.selectLayerButton_Click);
            // 
            // menu
            // 
            resources.ApplyResources(this.menu, "menu");
            this.menu.GripMargin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editMenuToolStripMenuItem,
            this.verToolStripMenuItem,
            this.insertarToolStripMenuItem,
            this.capasToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.analysisToolStripMenuItem,
            this.ayudaToolStripMenuItem});
            this.menu.Name = "menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newModelToolStripMenuItem,
            this.openModelToolStripMenuItem,
            this.toolStripSeparator12,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.screenshotToolStripMenuItem,
            this.toolStripSeparator13,
            this.importDXFToolStripMenuItem,
            this.exportDXFToolStripMenuItem,
            this.exportS2kToolStripMenuItem,
            this.toolStripSeparator16,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.toolStripSeparator29,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newModelToolStripMenuItem
            // 
            this.newModelToolStripMenuItem.Image = global::Canguro.Properties.Resources.DocumentHS;
            this.newModelToolStripMenuItem.Name = "newModelToolStripMenuItem";
            resources.ApplyResources(this.newModelToolStripMenuItem, "newModelToolStripMenuItem");
            this.newModelToolStripMenuItem.Click += new System.EventHandler(this.newButton_Click);
            // 
            // openModelToolStripMenuItem
            // 
            this.openModelToolStripMenuItem.Image = global::Canguro.Properties.Resources.openHS;
            this.openModelToolStripMenuItem.Name = "openModelToolStripMenuItem";
            resources.ApplyResources(this.openModelToolStripMenuItem, "openModelToolStripMenuItem");
            this.openModelToolStripMenuItem.Click += new System.EventHandler(this.openButton_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::Canguro.Properties.Resources.saveHS;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // screenshotToolStripMenuItem
            // 
            this.screenshotToolStripMenuItem.Image = global::Canguro.Properties.Resources.saveScreenshot;
            this.screenshotToolStripMenuItem.Name = "screenshotToolStripMenuItem";
            resources.ApplyResources(this.screenshotToolStripMenuItem, "screenshotToolStripMenuItem");
            this.screenshotToolStripMenuItem.Click += new System.EventHandler(this.screenshotToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // importDXFToolStripMenuItem
            // 
            this.importDXFToolStripMenuItem.Image = global::Canguro.Properties.Resources.import_dfx;
            this.importDXFToolStripMenuItem.Name = "importDXFToolStripMenuItem";
            resources.ApplyResources(this.importDXFToolStripMenuItem, "importDXFToolStripMenuItem");
            this.importDXFToolStripMenuItem.Click += new System.EventHandler(this.importDXFToolStripMenuItem_Click);
            // 
            // exportDXFToolStripMenuItem
            // 
            this.exportDXFToolStripMenuItem.Image = global::Canguro.Properties.Resources.export_dfx;
            this.exportDXFToolStripMenuItem.Name = "exportDXFToolStripMenuItem";
            resources.ApplyResources(this.exportDXFToolStripMenuItem, "exportDXFToolStripMenuItem");
            this.exportDXFToolStripMenuItem.Click += new System.EventHandler(this.exportDXFToolStripMenuItem_Click);
            // 
            // exportS2kToolStripMenuItem
            // 
            this.exportS2kToolStripMenuItem.Image = global::Canguro.Properties.Resources.export_s2k;
            this.exportS2kToolStripMenuItem.Name = "exportS2kToolStripMenuItem";
            resources.ApplyResources(this.exportS2kToolStripMenuItem, "exportS2kToolStripMenuItem");
            this.exportS2kToolStripMenuItem.Click += new System.EventHandler(this.exportS2kToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Image = global::Canguro.Properties.Resources.PrintHS;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            resources.ApplyResources(this.printToolStripMenuItem, "printToolStripMenuItem");
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printButton_Click);
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Image = global::Canguro.Properties.Resources.PrintPreviewHS;
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            resources.ApplyResources(this.printPreviewToolStripMenuItem, "printPreviewToolStripMenuItem");
            this.printPreviewToolStripMenuItem.Click += new System.EventHandler(this.printPreviewButton_Click);
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            resources.ApplyResources(this.toolStripSeparator29, "toolStripSeparator29");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editMenuToolStripMenuItem
            // 
            this.editMenuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator7,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.copyPasteToolStripMenuItem,
            this.toolStripSeparator8,
            this.moveToolStripMenuItem,
            this.editToolStripMenuItem,
            this.mirrorToolStripMenuItem,
            this.scaleToolStripMenuItem,
            this.arrayToolStripMenuItem,
            this.polarArrayToolStripMenuItem,
            this.rotateModelToolStripMenuItem,
            this.flipLineToolStripMenuItem,
            this.splitToolStripMenuItem,
            this.joinToolStripMenuItem,
            this.intersectToolStripMenuItem,
            this.toolStripSeparator19,
            this.preferencesToolStripMenuItem});
            this.editMenuToolStripMenuItem.Name = "editMenuToolStripMenuItem";
            resources.ApplyResources(this.editMenuToolStripMenuItem, "editMenuToolStripMenuItem");
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = global::Canguro.Properties.Resources.Edit_UndoHS;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Image = global::Canguro.Properties.Resources.Edit_RedoHS;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoButton_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::Canguro.Properties.Resources.CutHS;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::Canguro.Properties.Resources.CopyHS;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::Canguro.Properties.Resources.PasteHS;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::Canguro.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // copyPasteToolStripMenuItem
            // 
            this.copyPasteToolStripMenuItem.Image = global::Canguro.Properties.Resources.copypaste;
            this.copyPasteToolStripMenuItem.Name = "copyPasteToolStripMenuItem";
            resources.ApplyResources(this.copyPasteToolStripMenuItem, "copyPasteToolStripMenuItem");
            this.copyPasteToolStripMenuItem.Click += new System.EventHandler(this.copyPasteButton_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Image = global::Canguro.Properties.Resources.move;
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            resources.ApplyResources(this.moveToolStripMenuItem, "moveToolStripMenuItem");
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Image = global::Canguro.Properties.Resources.edit;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editButton_Click);
            // 
            // mirrorToolStripMenuItem
            // 
            this.mirrorToolStripMenuItem.Image = global::Canguro.Properties.Resources.mirror;
            this.mirrorToolStripMenuItem.Name = "mirrorToolStripMenuItem";
            resources.ApplyResources(this.mirrorToolStripMenuItem, "mirrorToolStripMenuItem");
            this.mirrorToolStripMenuItem.Click += new System.EventHandler(this.mirrorButton_Click);
            // 
            // scaleToolStripMenuItem
            // 
            this.scaleToolStripMenuItem.Image = global::Canguro.Properties.Resources.scale;
            this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            resources.ApplyResources(this.scaleToolStripMenuItem, "scaleToolStripMenuItem");
            this.scaleToolStripMenuItem.Click += new System.EventHandler(this.scaleButton_Click);
            // 
            // arrayToolStripMenuItem
            // 
            this.arrayToolStripMenuItem.Image = global::Canguro.Properties.Resources.array;
            this.arrayToolStripMenuItem.Name = "arrayToolStripMenuItem";
            resources.ApplyResources(this.arrayToolStripMenuItem, "arrayToolStripMenuItem");
            this.arrayToolStripMenuItem.Click += new System.EventHandler(this.arrayButton_Click);
            // 
            // polarArrayToolStripMenuItem
            // 
            this.polarArrayToolStripMenuItem.Image = global::Canguro.Properties.Resources.polarray;
            this.polarArrayToolStripMenuItem.Name = "polarArrayToolStripMenuItem";
            resources.ApplyResources(this.polarArrayToolStripMenuItem, "polarArrayToolStripMenuItem");
            this.polarArrayToolStripMenuItem.Click += new System.EventHandler(this.polararrayButton_Click);
            // 
            // rotateModelToolStripMenuItem
            // 
            this.rotateModelToolStripMenuItem.Image = global::Canguro.Properties.Resources.modelrotate;
            this.rotateModelToolStripMenuItem.Name = "rotateModelToolStripMenuItem";
            resources.ApplyResources(this.rotateModelToolStripMenuItem, "rotateModelToolStripMenuItem");
            this.rotateModelToolStripMenuItem.Click += new System.EventHandler(this.modelrotateButton_Click);
            // 
            // flipLineToolStripMenuItem
            // 
            this.flipLineToolStripMenuItem.Image = global::Canguro.Properties.Resources.flipline;
            this.flipLineToolStripMenuItem.Name = "flipLineToolStripMenuItem";
            resources.ApplyResources(this.flipLineToolStripMenuItem, "flipLineToolStripMenuItem");
            this.flipLineToolStripMenuItem.Click += new System.EventHandler(this.flipLineButton_Click);
            // 
            // splitToolStripMenuItem
            // 
            this.splitToolStripMenuItem.Image = global::Canguro.Properties.Resources.split;
            this.splitToolStripMenuItem.Name = "splitToolStripMenuItem";
            resources.ApplyResources(this.splitToolStripMenuItem, "splitToolStripMenuItem");
            this.splitToolStripMenuItem.Click += new System.EventHandler(this.splitButton_Click);
            // 
            // joinToolStripMenuItem
            // 
            this.joinToolStripMenuItem.Image = global::Canguro.Properties.Resources.join;
            this.joinToolStripMenuItem.Name = "joinToolStripMenuItem";
            resources.ApplyResources(this.joinToolStripMenuItem, "joinToolStripMenuItem");
            this.joinToolStripMenuItem.Click += new System.EventHandler(this.joinButton_Click);
            // 
            // intersectToolStripMenuItem
            // 
            this.intersectToolStripMenuItem.Image = global::Canguro.Properties.Resources.Intersect;
            this.intersectToolStripMenuItem.Name = "intersectToolStripMenuItem";
            resources.ApplyResources(this.intersectToolStripMenuItem, "intersectToolStripMenuItem");
            this.intersectToolStripMenuItem.Click += new System.EventHandler(this.intersectButton_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Image = global::Canguro.Properties.Resources.LegendHS;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            resources.ApplyResources(this.preferencesToolStripMenuItem, "preferencesToolStripMenuItem");
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // verToolStripMenuItem
            // 
            this.verToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToolStripMenuItem,
            this.panToolStripMenuItem,
            this.rotateToolStripMenuItem,
            this.zoomPreviousToolStripMenuItem,
            this.toolStripSeparator2,
            this.predefinedXYToolStripMenuItem,
            this.predefinedXZToolStripMenuItem,
            this.predefinedYZToolStripMenuItem,
            this.predefinedXYZToolStripMenuItem,
            this.toolStripSeparator10,
            this.hideToolStripMenuItem,
            this.showAllToolStripMenuItem,
            this.viewShadedToolStripMenuItem,
            this.viewOptionsToolStripMenuItem,
            this.viewLoadsToolStripMenuItem,
            this.colorsToolStripMenuItem});
            this.verToolStripMenuItem.Name = "verToolStripMenuItem";
            resources.ApplyResources(this.verToolStripMenuItem, "verToolStripMenuItem");
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.interactiveZoomToolStripMenuItem,
            this.zoomAllToolStripMenuItem});
            this.zoomToolStripMenuItem.Image = global::Canguro.Properties.Resources.zoom;
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            resources.ApplyResources(this.zoomToolStripMenuItem, "zoomToolStripMenuItem");
            this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomButton_Click);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Image = global::Canguro.Properties.Resources.zoommin;
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            resources.ApplyResources(this.zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInButton_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Image = global::Canguro.Properties.Resources.zoomout;
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            resources.ApplyResources(this.zoomOutToolStripMenuItem, "zoomOutToolStripMenuItem");
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutButton_Click);
            // 
            // interactiveZoomToolStripMenuItem
            // 
            this.interactiveZoomToolStripMenuItem.Image = global::Canguro.Properties.Resources.zoom;
            this.interactiveZoomToolStripMenuItem.Name = "interactiveZoomToolStripMenuItem";
            resources.ApplyResources(this.interactiveZoomToolStripMenuItem, "interactiveZoomToolStripMenuItem");
            this.interactiveZoomToolStripMenuItem.Click += new System.EventHandler(this.zoomButton_Click);
            // 
            // zoomAllToolStripMenuItem
            // 
            this.zoomAllToolStripMenuItem.Image = global::Canguro.Properties.Resources.zoomall;
            this.zoomAllToolStripMenuItem.Name = "zoomAllToolStripMenuItem";
            resources.ApplyResources(this.zoomAllToolStripMenuItem, "zoomAllToolStripMenuItem");
            this.zoomAllToolStripMenuItem.Click += new System.EventHandler(this.viewAllButton_Click);
            // 
            // panToolStripMenuItem
            // 
            this.panToolStripMenuItem.Image = global::Canguro.Properties.Resources.pan;
            this.panToolStripMenuItem.Name = "panToolStripMenuItem";
            resources.ApplyResources(this.panToolStripMenuItem, "panToolStripMenuItem");
            this.panToolStripMenuItem.Click += new System.EventHandler(this.panButton_Click);
            // 
            // rotateToolStripMenuItem
            // 
            this.rotateToolStripMenuItem.Image = global::Canguro.Properties.Resources.rotate;
            this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
            resources.ApplyResources(this.rotateToolStripMenuItem, "rotateToolStripMenuItem");
            this.rotateToolStripMenuItem.Click += new System.EventHandler(this.rotateButton_Click);
            // 
            // zoomPreviousToolStripMenuItem
            // 
            this.zoomPreviousToolStripMenuItem.Image = global::Canguro.Properties.Resources.zoompreviews;
            this.zoomPreviousToolStripMenuItem.Name = "zoomPreviousToolStripMenuItem";
            resources.ApplyResources(this.zoomPreviousToolStripMenuItem, "zoomPreviousToolStripMenuItem");
            this.zoomPreviousToolStripMenuItem.Click += new System.EventHandler(this.zoomPreviousButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // predefinedXYToolStripMenuItem
            // 
            this.predefinedXYToolStripMenuItem.Image = global::Canguro.Properties.Resources.predefinedxy;
            this.predefinedXYToolStripMenuItem.Name = "predefinedXYToolStripMenuItem";
            resources.ApplyResources(this.predefinedXYToolStripMenuItem, "predefinedXYToolStripMenuItem");
            this.predefinedXYToolStripMenuItem.Click += new System.EventHandler(this.predefinedxy_Click);
            // 
            // predefinedXZToolStripMenuItem
            // 
            this.predefinedXZToolStripMenuItem.Image = global::Canguro.Properties.Resources.predefinedxz;
            this.predefinedXZToolStripMenuItem.Name = "predefinedXZToolStripMenuItem";
            resources.ApplyResources(this.predefinedXZToolStripMenuItem, "predefinedXZToolStripMenuItem");
            this.predefinedXZToolStripMenuItem.Click += new System.EventHandler(this.predefinedxz_Click);
            // 
            // predefinedYZToolStripMenuItem
            // 
            this.predefinedYZToolStripMenuItem.Image = global::Canguro.Properties.Resources.predefinedyz;
            this.predefinedYZToolStripMenuItem.Name = "predefinedYZToolStripMenuItem";
            resources.ApplyResources(this.predefinedYZToolStripMenuItem, "predefinedYZToolStripMenuItem");
            this.predefinedYZToolStripMenuItem.Click += new System.EventHandler(this.predefinedyz_Click);
            // 
            // predefinedXYZToolStripMenuItem
            // 
            this.predefinedXYZToolStripMenuItem.Image = global::Canguro.Properties.Resources.predefinedXyz;
            this.predefinedXYZToolStripMenuItem.Name = "predefinedXYZToolStripMenuItem";
            resources.ApplyResources(this.predefinedXYZToolStripMenuItem, "predefinedXYZToolStripMenuItem");
            this.predefinedXYZToolStripMenuItem.Click += new System.EventHandler(this.predefinedxyzButton_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Image = global::Canguro.Properties.Resources.hide;
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            resources.ApplyResources(this.hideToolStripMenuItem, "hideToolStripMenuItem");
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideButton_Click);
            // 
            // showAllToolStripMenuItem
            // 
            this.showAllToolStripMenuItem.Image = global::Canguro.Properties.Resources.showall;
            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
            resources.ApplyResources(this.showAllToolStripMenuItem, "showAllToolStripMenuItem");
            this.showAllToolStripMenuItem.Click += new System.EventHandler(this.showAllButton_Click);
            // 
            // viewShadedToolStripMenuItem
            // 
            this.viewShadedToolStripMenuItem.Image = global::Canguro.Properties.Resources.view_shaded;
            this.viewShadedToolStripMenuItem.Name = "viewShadedToolStripMenuItem";
            resources.ApplyResources(this.viewShadedToolStripMenuItem, "viewShadedToolStripMenuItem");
            this.viewShadedToolStripMenuItem.Click += new System.EventHandler(this.viewShadedButton_Click);
            // 
            // viewOptionsToolStripMenuItem
            // 
            this.viewOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem3,
            this.showJointIDToolStripMenuItem,
            this.hideJointsToolStripMenuItem1,
            this.showJointCoordinatesToolStripMenuItem1,
            this.showDOFToolStripMenuItem2,
            this.showLineIDToolStripMenuItem,
            this.showLineLenghtToolStripMenuItem,
            this.showLocalAxesToolStripMenuItem,
            this.showSectionsToolStripMenuItem,
            this.releasesToolStripMenuItem2,
            this.showAxesToolStripMenuItem,
            this.showFloorToolStripMenuItem,
            this.loadSizesToolStripMenuItem});
            this.viewOptionsToolStripMenuItem.Image = global::Canguro.Properties.Resources.Showtexts;
            this.viewOptionsToolStripMenuItem.Name = "viewOptionsToolStripMenuItem";
            resources.ApplyResources(this.viewOptionsToolStripMenuItem, "viewOptionsToolStripMenuItem");
            // 
            // noneToolStripMenuItem3
            // 
            this.noneToolStripMenuItem3.Name = "noneToolStripMenuItem3";
            resources.ApplyResources(this.noneToolStripMenuItem3, "noneToolStripMenuItem3");
            this.noneToolStripMenuItem3.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // showJointIDToolStripMenuItem
            // 
            this.showJointIDToolStripMenuItem.Name = "showJointIDToolStripMenuItem";
            resources.ApplyResources(this.showJointIDToolStripMenuItem, "showJointIDToolStripMenuItem");
            this.showJointIDToolStripMenuItem.Click += new System.EventHandler(this.jointIDToolStripMenuItem_Click);
            // 
            // hideJointsToolStripMenuItem1
            // 
            this.hideJointsToolStripMenuItem1.Name = "hideJointsToolStripMenuItem1";
            resources.ApplyResources(this.hideJointsToolStripMenuItem1, "hideJointsToolStripMenuItem1");
            this.hideJointsToolStripMenuItem1.Click += new System.EventHandler(this.hideJointsToolStripMenuItem_Click);
            // 
            // showJointCoordinatesToolStripMenuItem1
            // 
            this.showJointCoordinatesToolStripMenuItem1.Name = "showJointCoordinatesToolStripMenuItem1";
            resources.ApplyResources(this.showJointCoordinatesToolStripMenuItem1, "showJointCoordinatesToolStripMenuItem1");
            this.showJointCoordinatesToolStripMenuItem1.Click += new System.EventHandler(this.jointCoordinatesToolStripMenuItem_Click);
            // 
            // showDOFToolStripMenuItem2
            // 
            this.showDOFToolStripMenuItem2.Name = "showDOFToolStripMenuItem2";
            resources.ApplyResources(this.showDOFToolStripMenuItem2, "showDOFToolStripMenuItem2");
            this.showDOFToolStripMenuItem2.Click += new System.EventHandler(this.showDOFToolStripMenuItem_Click);
            // 
            // showLineIDToolStripMenuItem
            // 
            this.showLineIDToolStripMenuItem.Name = "showLineIDToolStripMenuItem";
            resources.ApplyResources(this.showLineIDToolStripMenuItem, "showLineIDToolStripMenuItem");
            this.showLineIDToolStripMenuItem.Click += new System.EventHandler(this.lineIDToolStripMenuItem_Click);
            // 
            // showLineLenghtToolStripMenuItem
            // 
            this.showLineLenghtToolStripMenuItem.Name = "showLineLenghtToolStripMenuItem";
            resources.ApplyResources(this.showLineLenghtToolStripMenuItem, "showLineLenghtToolStripMenuItem");
            this.showLineLenghtToolStripMenuItem.Click += new System.EventHandler(this.lineLengthToolStripMenuItem_Click);
            // 
            // showLocalAxesToolStripMenuItem
            // 
            this.showLocalAxesToolStripMenuItem.Name = "showLocalAxesToolStripMenuItem";
            resources.ApplyResources(this.showLocalAxesToolStripMenuItem, "showLocalAxesToolStripMenuItem");
            this.showLocalAxesToolStripMenuItem.Click += new System.EventHandler(this.showLocalAxesToolStripMenuItem_Click);
            // 
            // showSectionsToolStripMenuItem
            // 
            this.showSectionsToolStripMenuItem.Name = "showSectionsToolStripMenuItem";
            resources.ApplyResources(this.showSectionsToolStripMenuItem, "showSectionsToolStripMenuItem");
            this.showSectionsToolStripMenuItem.Click += new System.EventHandler(this.showSectionsToolStripMenuItem_Click);
            // 
            // releasesToolStripMenuItem2
            // 
            this.releasesToolStripMenuItem2.Name = "releasesToolStripMenuItem2";
            resources.ApplyResources(this.releasesToolStripMenuItem2, "releasesToolStripMenuItem2");
            this.releasesToolStripMenuItem2.Click += new System.EventHandler(this.releasesToolStripMenuItem_Click);
            // 
            // showAxesToolStripMenuItem
            // 
            this.showAxesToolStripMenuItem.Name = "showAxesToolStripMenuItem";
            resources.ApplyResources(this.showAxesToolStripMenuItem, "showAxesToolStripMenuItem");
            this.showAxesToolStripMenuItem.Click += new System.EventHandler(this.viewAxesToolStripMenuItem_Click);
            // 
            // showFloorToolStripMenuItem
            // 
            this.showFloorToolStripMenuItem.Name = "showFloorToolStripMenuItem";
            resources.ApplyResources(this.showFloorToolStripMenuItem, "showFloorToolStripMenuItem");
            this.showFloorToolStripMenuItem.Click += new System.EventHandler(this.viewFloorToolStripMenuItem_Click);
            // 
            // loadSizesToolStripMenuItem
            // 
            this.loadSizesToolStripMenuItem.Name = "loadSizesToolStripMenuItem";
            resources.ApplyResources(this.loadSizesToolStripMenuItem, "loadSizesToolStripMenuItem");
            this.loadSizesToolStripMenuItem.Click += new System.EventHandler(this.loadSizesToolStripMenuItem_Click);
            // 
            // viewLoadsToolStripMenuItem
            // 
            this.viewLoadsToolStripMenuItem.Image = global::Canguro.Properties.Resources.show_loads;
            this.viewLoadsToolStripMenuItem.Name = "viewLoadsToolStripMenuItem";
            resources.ApplyResources(this.viewLoadsToolStripMenuItem, "viewLoadsToolStripMenuItem");
            this.viewLoadsToolStripMenuItem.Click += new System.EventHandler(this.showLoadsButton_Click);
            // 
            // colorsToolStripMenuItem
            // 
            this.colorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorMaterialsToolStripMenuItem,
            this.colorSectionsToolStripMenuItem,
            this.colorLayersToolStripMenuItem,
            this.colorLoadsToolStripMenuItem,
            this.colorConstraintsToolStripMenuItem});
            this.colorsToolStripMenuItem.Image = global::Canguro.Properties.Resources.colors;
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            resources.ApplyResources(this.colorsToolStripMenuItem, "colorsToolStripMenuItem");
            // 
            // colorMaterialsToolStripMenuItem
            // 
            this.colorMaterialsToolStripMenuItem.Name = "colorMaterialsToolStripMenuItem";
            resources.ApplyResources(this.colorMaterialsToolStripMenuItem, "colorMaterialsToolStripMenuItem");
            this.colorMaterialsToolStripMenuItem.Click += new System.EventHandler(this.colorMaterialsToolStripMenuItem2_Click);
            // 
            // colorSectionsToolStripMenuItem
            // 
            this.colorSectionsToolStripMenuItem.Name = "colorSectionsToolStripMenuItem";
            resources.ApplyResources(this.colorSectionsToolStripMenuItem, "colorSectionsToolStripMenuItem");
            this.colorSectionsToolStripMenuItem.Click += new System.EventHandler(this.colorSectionsToolStripMenuItem2_Click);
            // 
            // colorLayersToolStripMenuItem
            // 
            this.colorLayersToolStripMenuItem.Name = "colorLayersToolStripMenuItem";
            resources.ApplyResources(this.colorLayersToolStripMenuItem, "colorLayersToolStripMenuItem");
            this.colorLayersToolStripMenuItem.Click += new System.EventHandler(this.colorLayersToolStripMenuItem2_Click);
            // 
            // colorLoadsToolStripMenuItem
            // 
            this.colorLoadsToolStripMenuItem.Name = "colorLoadsToolStripMenuItem";
            resources.ApplyResources(this.colorLoadsToolStripMenuItem, "colorLoadsToolStripMenuItem");
            this.colorLoadsToolStripMenuItem.Click += new System.EventHandler(this.colorLoadsToolStripMenuItem2_Click);
            // 
            // colorConstraintsToolStripMenuItem
            // 
            this.colorConstraintsToolStripMenuItem.Name = "colorConstraintsToolStripMenuItem";
            resources.ApplyResources(this.colorConstraintsToolStripMenuItem, "colorConstraintsToolStripMenuItem");
            this.colorConstraintsToolStripMenuItem.Click += new System.EventHandler(this.colorConstraintsToolStripMenuItem_Click);
            // 
            // insertarToolStripMenuItem
            // 
            this.insertarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jointToolStripMenuItem,
            this.linesToolStripMenuItem,
            this.lineStripToolStripMenuItem,
            this.arcToolStripMenuItem,
            this.toolStripSeparator5,
            this.materialsToolStripMenuItem,
            this.sectionsToolStripMenuItem,
            this.toolStripSeparator17,
            this.forceLoadToolStripMenuItem,
            this.groundDisplacementToolStripMenuItem,
            this.lineLoadToolStripMenuItem,
            this.toolStripSeparator6,
            this.loadCaseToolStripMenuItem,
            this.analysisCaseToolStripMenuItem,
            this.loadComboToolStripMenuItem,
            this.toolStripSeparator9,
            this.constraintsToolStripMenuItem,
            this.diaphragmsToolStripMenuItem,
            this.toolStripSeparator32,
            this.gridToolStripMenuItem,
            this.domeToolStripMenuItem,
            this.cylinderToolStripMenuItem});
            this.insertarToolStripMenuItem.Name = "insertarToolStripMenuItem";
            resources.ApplyResources(this.insertarToolStripMenuItem, "insertarToolStripMenuItem");
            // 
            // jointToolStripMenuItem
            // 
            this.jointToolStripMenuItem.Image = global::Canguro.Properties.Resources.adjoint;
            this.jointToolStripMenuItem.Name = "jointToolStripMenuItem";
            resources.ApplyResources(this.jointToolStripMenuItem, "jointToolStripMenuItem");
            this.jointToolStripMenuItem.Click += new System.EventHandler(this.jointButton_Click);
            // 
            // linesToolStripMenuItem
            // 
            this.linesToolStripMenuItem.Image = global::Canguro.Properties.Resources.addline1;
            this.linesToolStripMenuItem.Name = "linesToolStripMenuItem";
            resources.ApplyResources(this.linesToolStripMenuItem, "linesToolStripMenuItem");
            this.linesToolStripMenuItem.Click += new System.EventHandler(this.lineButton_Click);
            // 
            // lineStripToolStripMenuItem
            // 
            this.lineStripToolStripMenuItem.Image = global::Canguro.Properties.Resources.addlinestript;
            this.lineStripToolStripMenuItem.Name = "lineStripToolStripMenuItem";
            resources.ApplyResources(this.lineStripToolStripMenuItem, "lineStripToolStripMenuItem");
            this.lineStripToolStripMenuItem.Click += new System.EventHandler(this.lineStripButton_Click);
            // 
            // arcToolStripMenuItem
            // 
            this.arcToolStripMenuItem.Image = global::Canguro.Properties.Resources.arc;
            this.arcToolStripMenuItem.Name = "arcToolStripMenuItem";
            resources.ApplyResources(this.arcToolStripMenuItem, "arcToolStripMenuItem");
            this.arcToolStripMenuItem.Click += new System.EventHandler(this.arcButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // materialsToolStripMenuItem
            // 
            this.materialsToolStripMenuItem.Image = global::Canguro.Properties.Resources.Materials;
            this.materialsToolStripMenuItem.Name = "materialsToolStripMenuItem";
            resources.ApplyResources(this.materialsToolStripMenuItem, "materialsToolStripMenuItem");
            this.materialsToolStripMenuItem.Click += new System.EventHandler(this.materialsToolStripMenuItem_Click);
            // 
            // sectionsToolStripMenuItem
            // 
            this.sectionsToolStripMenuItem.Image = global::Canguro.Properties.Resources.sections;
            this.sectionsToolStripMenuItem.Name = "sectionsToolStripMenuItem";
            resources.ApplyResources(this.sectionsToolStripMenuItem, "sectionsToolStripMenuItem");
            this.sectionsToolStripMenuItem.Click += new System.EventHandler(this.sectionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // forceLoadToolStripMenuItem
            // 
            this.forceLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.forceload;
            this.forceLoadToolStripMenuItem.Name = "forceLoadToolStripMenuItem";
            resources.ApplyResources(this.forceLoadToolStripMenuItem, "forceLoadToolStripMenuItem");
            // 
            // groundDisplacementToolStripMenuItem
            // 
            this.groundDisplacementToolStripMenuItem.Image = global::Canguro.Properties.Resources.grounddisplacementload;
            this.groundDisplacementToolStripMenuItem.Name = "groundDisplacementToolStripMenuItem";
            resources.ApplyResources(this.groundDisplacementToolStripMenuItem, "groundDisplacementToolStripMenuItem");
            // 
            // lineLoadToolStripMenuItem
            // 
            this.lineLoadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.concentratedLoadToolStripMenuItem,
            this.uniformLineLoadToolStripMenuItem,
            this.triangularLineLoadToolStripMenuItem,
            this.distributedLoadToolStripMenuItem,
            this.toolStripSeparator30,
            this.temperatureLineLoadToolStripMenuItem,
            this.temperatureGradientLineLoadToolStripMenuItem});
            this.lineLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.distributedexpandload;
            this.lineLoadToolStripMenuItem.Name = "lineLoadToolStripMenuItem";
            resources.ApplyResources(this.lineLoadToolStripMenuItem, "lineLoadToolStripMenuItem");
            // 
            // concentratedLoadToolStripMenuItem
            // 
            this.concentratedLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.concentrated_load;
            this.concentratedLoadToolStripMenuItem.Name = "concentratedLoadToolStripMenuItem";
            resources.ApplyResources(this.concentratedLoadToolStripMenuItem, "concentratedLoadToolStripMenuItem");
            this.concentratedLoadToolStripMenuItem.Click += new System.EventHandler(this.concentratedLoadButton_Click);
            // 
            // uniformLineLoadToolStripMenuItem
            // 
            this.uniformLineLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.uniformLineLoad;
            this.uniformLineLoadToolStripMenuItem.Name = "uniformLineLoadToolStripMenuItem";
            resources.ApplyResources(this.uniformLineLoadToolStripMenuItem, "uniformLineLoadToolStripMenuItem");
            this.uniformLineLoadToolStripMenuItem.Click += new System.EventHandler(this.uniformLineLoadButton_Click);
            // 
            // triangularLineLoadToolStripMenuItem
            // 
            this.triangularLineLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.triangleLineLoad;
            this.triangularLineLoadToolStripMenuItem.Name = "triangularLineLoadToolStripMenuItem";
            resources.ApplyResources(this.triangularLineLoadToolStripMenuItem, "triangularLineLoadToolStripMenuItem");
            this.triangularLineLoadToolStripMenuItem.Click += new System.EventHandler(this.triangleLineLoadButton_Click);
            // 
            // distributedLoadToolStripMenuItem
            // 
            this.distributedLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.distributedexpandload;
            this.distributedLoadToolStripMenuItem.Name = "distributedLoadToolStripMenuItem";
            resources.ApplyResources(this.distributedLoadToolStripMenuItem, "distributedLoadToolStripMenuItem");
            this.distributedLoadToolStripMenuItem.Click += new System.EventHandler(this.distributedLoadButton_Click);
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            resources.ApplyResources(this.toolStripSeparator30, "toolStripSeparator30");
            // 
            // temperatureLineLoadToolStripMenuItem
            // 
            this.temperatureLineLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.temperatureLineLoad;
            this.temperatureLineLoadToolStripMenuItem.Name = "temperatureLineLoadToolStripMenuItem";
            resources.ApplyResources(this.temperatureLineLoadToolStripMenuItem, "temperatureLineLoadToolStripMenuItem");
            this.temperatureLineLoadToolStripMenuItem.Click += new System.EventHandler(this.temperatureLineLoadButton_Click);
            // 
            // temperatureGradientLineLoadToolStripMenuItem
            // 
            this.temperatureGradientLineLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.temperatureGradientLineLoad;
            this.temperatureGradientLineLoadToolStripMenuItem.Name = "temperatureGradientLineLoadToolStripMenuItem";
            resources.ApplyResources(this.temperatureGradientLineLoadToolStripMenuItem, "temperatureGradientLineLoadToolStripMenuItem");
            this.temperatureGradientLineLoadToolStripMenuItem.Click += new System.EventHandler(this.temperatureGradientLineLoadButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // loadCaseToolStripMenuItem
            // 
            this.loadCaseToolStripMenuItem.Image = global::Canguro.Properties.Resources.loadcase;
            this.loadCaseToolStripMenuItem.Name = "loadCaseToolStripMenuItem";
            resources.ApplyResources(this.loadCaseToolStripMenuItem, "loadCaseToolStripMenuItem");
            this.loadCaseToolStripMenuItem.Click += new System.EventHandler(this.loadCaseButton_Click);
            // 
            // analysisCaseToolStripMenuItem
            // 
            this.analysisCaseToolStripMenuItem.Image = global::Canguro.Properties.Resources.analisis_case;
            this.analysisCaseToolStripMenuItem.Name = "analysisCaseToolStripMenuItem";
            resources.ApplyResources(this.analysisCaseToolStripMenuItem, "analysisCaseToolStripMenuItem");
            this.analysisCaseToolStripMenuItem.Click += new System.EventHandler(this.analysisCaseButton_Click);
            // 
            // loadComboToolStripMenuItem
            // 
            this.loadComboToolStripMenuItem.Image = global::Canguro.Properties.Resources.loadcombination;
            this.loadComboToolStripMenuItem.Name = "loadComboToolStripMenuItem";
            resources.ApplyResources(this.loadComboToolStripMenuItem, "loadComboToolStripMenuItem");
            this.loadComboToolStripMenuItem.Click += new System.EventHandler(this.loadComboButton_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // constraintsToolStripMenuItem
            // 
            this.constraintsToolStripMenuItem.Image = global::Canguro.Properties.Resources.constraints;
            this.constraintsToolStripMenuItem.Name = "constraintsToolStripMenuItem";
            resources.ApplyResources(this.constraintsToolStripMenuItem, "constraintsToolStripMenuItem");
            this.constraintsToolStripMenuItem.Click += new System.EventHandler(this.constraintsButton_Click);
            // 
            // diaphragmsToolStripMenuItem
            // 
            this.diaphragmsToolStripMenuItem.Image = global::Canguro.Properties.Resources.diaphragms;
            this.diaphragmsToolStripMenuItem.Name = "diaphragmsToolStripMenuItem";
            resources.ApplyResources(this.diaphragmsToolStripMenuItem, "diaphragmsToolStripMenuItem");
            this.diaphragmsToolStripMenuItem.Click += new System.EventHandler(this.diaphragmsButton_Click);
            // 
            // toolStripSeparator32
            // 
            this.toolStripSeparator32.Name = "toolStripSeparator32";
            resources.ApplyResources(this.toolStripSeparator32, "toolStripSeparator32");
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.Image = global::Canguro.Properties.Resources.grid;
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            resources.ApplyResources(this.gridToolStripMenuItem, "gridToolStripMenuItem");
            this.gridToolStripMenuItem.Click += new System.EventHandler(this.gridButton_Click);
            // 
            // domeToolStripMenuItem
            // 
            this.domeToolStripMenuItem.Image = global::Canguro.Properties.Resources.dome;
            this.domeToolStripMenuItem.Name = "domeToolStripMenuItem";
            resources.ApplyResources(this.domeToolStripMenuItem, "domeToolStripMenuItem");
            this.domeToolStripMenuItem.Click += new System.EventHandler(this.domeButton_Click);
            // 
            // cylinderToolStripMenuItem
            // 
            this.cylinderToolStripMenuItem.Image = global::Canguro.Properties.Resources.cylinder;
            this.cylinderToolStripMenuItem.Name = "cylinderToolStripMenuItem";
            resources.ApplyResources(this.cylinderToolStripMenuItem, "cylinderToolStripMenuItem");
            this.cylinderToolStripMenuItem.Click += new System.EventHandler(this.cylinderButton_Click);
            // 
            // capasToolStripMenuItem
            // 
            this.capasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem,
            this.editLayerToolStripMenuItem,
            this.deleteLayerToolStripMenuItem,
            this.selectLayerToolStripMenuItem,
            this.toolStripSeparator11,
            this.hideLayerToolStripMenuItem,
            this.showLayerToolStripMenuItem,
            this.moveToLayerToolStripMenuItem,
            this.activateLayerToolStripMenuItem});
            this.capasToolStripMenuItem.Name = "capasToolStripMenuItem";
            resources.ApplyResources(this.capasToolStripMenuItem, "capasToolStripMenuItem");
            // 
            // addLayerToolStripMenuItem
            // 
            this.addLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.addlayer;
            this.addLayerToolStripMenuItem.Name = "addLayerToolStripMenuItem";
            resources.ApplyResources(this.addLayerToolStripMenuItem, "addLayerToolStripMenuItem");
            this.addLayerToolStripMenuItem.Click += new System.EventHandler(this.layerButton_Click);
            // 
            // editLayerToolStripMenuItem
            // 
            this.editLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.editlayer;
            this.editLayerToolStripMenuItem.Name = "editLayerToolStripMenuItem";
            resources.ApplyResources(this.editLayerToolStripMenuItem, "editLayerToolStripMenuItem");
            this.editLayerToolStripMenuItem.Click += new System.EventHandler(this.editlayerButton_Click);
            // 
            // deleteLayerToolStripMenuItem
            // 
            this.deleteLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.deletelayer;
            this.deleteLayerToolStripMenuItem.Name = "deleteLayerToolStripMenuItem";
            resources.ApplyResources(this.deleteLayerToolStripMenuItem, "deleteLayerToolStripMenuItem");
            this.deleteLayerToolStripMenuItem.Click += new System.EventHandler(this.deletelayerButton_Click);
            // 
            // selectLayerToolStripMenuItem
            // 
            this.selectLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.select;
            this.selectLayerToolStripMenuItem.Name = "selectLayerToolStripMenuItem";
            resources.ApplyResources(this.selectLayerToolStripMenuItem, "selectLayerToolStripMenuItem");
            this.selectLayerToolStripMenuItem.Click += new System.EventHandler(this.selectLayerButton_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // hideLayerToolStripMenuItem
            // 
            this.hideLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.hidelayer;
            this.hideLayerToolStripMenuItem.Name = "hideLayerToolStripMenuItem";
            resources.ApplyResources(this.hideLayerToolStripMenuItem, "hideLayerToolStripMenuItem");
            this.hideLayerToolStripMenuItem.Click += new System.EventHandler(this.hideLayerButton_Click);
            // 
            // showLayerToolStripMenuItem
            // 
            this.showLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.showlayer;
            this.showLayerToolStripMenuItem.Name = "showLayerToolStripMenuItem";
            resources.ApplyResources(this.showLayerToolStripMenuItem, "showLayerToolStripMenuItem");
            this.showLayerToolStripMenuItem.Click += new System.EventHandler(this.showLayerButton_Click);
            // 
            // moveToLayerToolStripMenuItem
            // 
            this.moveToLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.move_to_corrent;
            this.moveToLayerToolStripMenuItem.Name = "moveToLayerToolStripMenuItem";
            resources.ApplyResources(this.moveToLayerToolStripMenuItem, "moveToLayerToolStripMenuItem");
            this.moveToLayerToolStripMenuItem.Click += new System.EventHandler(this.moveToLayerButton_Click);
            // 
            // activateLayerToolStripMenuItem
            // 
            this.activateLayerToolStripMenuItem.Image = global::Canguro.Properties.Resources.activate_layer;
            this.activateLayerToolStripMenuItem.Name = "activateLayerToolStripMenuItem";
            resources.ApplyResources(this.activateLayerToolStripMenuItem, "activateLayerToolStripMenuItem");
            this.activateLayerToolStripMenuItem.Click += new System.EventHandler(this.activateLayerButton_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.invertSelectionToolStripMenuItem,
            this.unselectToolStripMenuItem,
            this.selectLineToolStripMenuItem,
            this.selectConnectedToolStripMenuItem,
            this.toolStripSeparator28,
            this.distanceToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            resources.ApplyResources(this.selectToolStripMenuItem, "selectToolStripMenuItem");
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Image = global::Canguro.Properties.Resources.select_all;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Image = global::Canguro.Properties.Resources.invert_selection;
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            resources.ApplyResources(this.invertSelectionToolStripMenuItem, "invertSelectionToolStripMenuItem");
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionButton_Click);
            // 
            // unselectToolStripMenuItem
            // 
            this.unselectToolStripMenuItem.Image = global::Canguro.Properties.Resources.unselect;
            this.unselectToolStripMenuItem.Name = "unselectToolStripMenuItem";
            resources.ApplyResources(this.unselectToolStripMenuItem, "unselectToolStripMenuItem");
            this.unselectToolStripMenuItem.Click += new System.EventHandler(this.unselectButton_Click);
            // 
            // selectLineToolStripMenuItem
            // 
            this.selectLineToolStripMenuItem.Image = global::Canguro.Properties.Resources.selectline;
            this.selectLineToolStripMenuItem.Name = "selectLineToolStripMenuItem";
            resources.ApplyResources(this.selectLineToolStripMenuItem, "selectLineToolStripMenuItem");
            this.selectLineToolStripMenuItem.Click += new System.EventHandler(this.selectLineButton_Click);
            // 
            // selectConnectedToolStripMenuItem
            // 
            this.selectConnectedToolStripMenuItem.Image = global::Canguro.Properties.Resources.selectconnected;
            this.selectConnectedToolStripMenuItem.Name = "selectConnectedToolStripMenuItem";
            resources.ApplyResources(this.selectConnectedToolStripMenuItem, "selectConnectedToolStripMenuItem");
            this.selectConnectedToolStripMenuItem.Click += new System.EventHandler(this.selectConnectedButton_Click);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            resources.ApplyResources(this.toolStripSeparator28, "toolStripSeparator28");
            // 
            // distanceToolStripMenuItem
            // 
            this.distanceToolStripMenuItem.Image = global::Canguro.Properties.Resources.distanceButton;
            this.distanceToolStripMenuItem.Name = "distanceToolStripMenuItem";
            resources.ApplyResources(this.distanceToolStripMenuItem, "distanceToolStripMenuItem");
            this.distanceToolStripMenuItem.Click += new System.EventHandler(this.distanceButton_Click);
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzeToolStripMenuItem,
            this.toolStripSeparator20,
            this.showDeformedToolStripMenuItem,
            this.animatedToolStripMenuItem,
            this.showDiagramToolStripMenuItem,
            this.showStressesToolStripMenuItem,
            this.designToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.jointReactionsToolStripMenuItem,
            this.jointReactionTextsToolStripMenuItem});
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            resources.ApplyResources(this.analysisToolStripMenuItem, "analysisToolStripMenuItem");
            // 
            // analyzeToolStripMenuItem
            // 
            this.analyzeToolStripMenuItem.Image = global::Canguro.Properties.Resources.analize;
            this.analyzeToolStripMenuItem.Name = "analyzeToolStripMenuItem";
            resources.ApplyResources(this.analyzeToolStripMenuItem, "analyzeToolStripMenuItem");
            this.analyzeToolStripMenuItem.Click += new System.EventHandler(this.analizarToolStripMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            // 
            // showDeformedToolStripMenuItem
            // 
            this.showDeformedToolStripMenuItem.Image = global::Canguro.Properties.Resources.deformed;
            this.showDeformedToolStripMenuItem.Name = "showDeformedToolStripMenuItem";
            resources.ApplyResources(this.showDeformedToolStripMenuItem, "showDeformedToolStripMenuItem");
            this.showDeformedToolStripMenuItem.Click += new System.EventHandler(this.showDeformedButton_Click);
            // 
            // animatedToolStripMenuItem
            // 
            this.animatedToolStripMenuItem.Image = global::Canguro.Properties.Resources.animation;
            this.animatedToolStripMenuItem.Name = "animatedToolStripMenuItem";
            resources.ApplyResources(this.animatedToolStripMenuItem, "animatedToolStripMenuItem");
            this.animatedToolStripMenuItem.Click += new System.EventHandler(this.animateButton_Click);
            // 
            // showDiagramToolStripMenuItem
            // 
            this.showDiagramToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noDiagramsToolStripMenuItem,
            this.axialDiagramToolStripMenuItem,
            this.s2DiagramToolStripMenuItem,
            this.s3DiagramToolStripMenuItem,
            this.torsionDiagramToolStripMenuItem,
            this.m2DiagramToolStripMenuItem,
            this.m3DiagramToolStripMenuItem});
            this.showDiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.alldiagrams;
            this.showDiagramToolStripMenuItem.Name = "showDiagramToolStripMenuItem";
            resources.ApplyResources(this.showDiagramToolStripMenuItem, "showDiagramToolStripMenuItem");
            // 
            // noDiagramsToolStripMenuItem
            // 
            this.noDiagramsToolStripMenuItem.Image = global::Canguro.Properties.Resources.hidediagrams;
            this.noDiagramsToolStripMenuItem.Name = "noDiagramsToolStripMenuItem";
            resources.ApplyResources(this.noDiagramsToolStripMenuItem, "noDiagramsToolStripMenuItem");
            this.noDiagramsToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem1_Click);
            // 
            // axialDiagramToolStripMenuItem
            // 
            this.axialDiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.showaxial;
            this.axialDiagramToolStripMenuItem.Name = "axialDiagramToolStripMenuItem";
            resources.ApplyResources(this.axialDiagramToolStripMenuItem, "axialDiagramToolStripMenuItem");
            this.axialDiagramToolStripMenuItem.Click += new System.EventHandler(this.axialToolStripMenuItem_Click);
            // 
            // s2DiagramToolStripMenuItem
            // 
            this.s2DiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.show_S1;
            this.s2DiagramToolStripMenuItem.Name = "s2DiagramToolStripMenuItem";
            resources.ApplyResources(this.s2DiagramToolStripMenuItem, "s2DiagramToolStripMenuItem");
            this.s2DiagramToolStripMenuItem.Click += new System.EventHandler(this.s2ToolStripMenuItem_Click);
            // 
            // s3DiagramToolStripMenuItem
            // 
            this.s3DiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.show_S2;
            this.s3DiagramToolStripMenuItem.Name = "s3DiagramToolStripMenuItem";
            resources.ApplyResources(this.s3DiagramToolStripMenuItem, "s3DiagramToolStripMenuItem");
            this.s3DiagramToolStripMenuItem.Click += new System.EventHandler(this.s3ToolStripMenuItem_Click);
            // 
            // torsionDiagramToolStripMenuItem
            // 
            this.torsionDiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.show_torcion;
            this.torsionDiagramToolStripMenuItem.Name = "torsionDiagramToolStripMenuItem";
            resources.ApplyResources(this.torsionDiagramToolStripMenuItem, "torsionDiagramToolStripMenuItem");
            this.torsionDiagramToolStripMenuItem.Click += new System.EventHandler(this.torsionToolStripMenuItem_Click);
            // 
            // m2DiagramToolStripMenuItem
            // 
            this.m2DiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.show_M1;
            this.m2DiagramToolStripMenuItem.Name = "m2DiagramToolStripMenuItem";
            resources.ApplyResources(this.m2DiagramToolStripMenuItem, "m2DiagramToolStripMenuItem");
            this.m2DiagramToolStripMenuItem.Click += new System.EventHandler(this.m2ToolStripMenuItem_Click);
            // 
            // m3DiagramToolStripMenuItem
            // 
            this.m3DiagramToolStripMenuItem.Image = global::Canguro.Properties.Resources.show_M2;
            this.m3DiagramToolStripMenuItem.Name = "m3DiagramToolStripMenuItem";
            resources.ApplyResources(this.m3DiagramToolStripMenuItem, "m3DiagramToolStripMenuItem");
            this.m3DiagramToolStripMenuItem.Click += new System.EventHandler(this.m3ToolStripMenuItem_Click);
            // 
            // showStressesToolStripMenuItem
            // 
            this.showStressesToolStripMenuItem.Image = global::Canguro.Properties.Resources.showstresses;
            this.showStressesToolStripMenuItem.Name = "showStressesToolStripMenuItem";
            resources.ApplyResources(this.showStressesToolStripMenuItem, "showStressesToolStripMenuItem");
            this.showStressesToolStripMenuItem.Click += new System.EventHandler(this.showStressesButton_Click);
            // 
            // designToolStripMenuItem
            // 
            this.designToolStripMenuItem.Image = global::Canguro.Properties.Resources.Design;
            this.designToolStripMenuItem.Name = "designToolStripMenuItem";
            resources.ApplyResources(this.designToolStripMenuItem, "designToolStripMenuItem");
            this.designToolStripMenuItem.Click += new System.EventHandler(this.showDesignButton_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.Image = global::Canguro.Properties.Resources.NewReportHS;
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            resources.ApplyResources(this.reportsToolStripMenuItem, "reportsToolStripMenuItem");
            this.reportsToolStripMenuItem.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // jointReactionsToolStripMenuItem
            // 
            this.jointReactionsToolStripMenuItem.Image = global::Canguro.Properties.Resources.jointreactions;
            this.jointReactionsToolStripMenuItem.Name = "jointReactionsToolStripMenuItem";
            resources.ApplyResources(this.jointReactionsToolStripMenuItem, "jointReactionsToolStripMenuItem");
            this.jointReactionsToolStripMenuItem.Click += new System.EventHandler(this.showJointReactionsButton_Click);
            // 
            // jointReactionTextsToolStripMenuItem
            // 
            this.jointReactionTextsToolStripMenuItem.Image = global::Canguro.Properties.Resources.jointReactionsTexts;
            this.jointReactionTextsToolStripMenuItem.Name = "jointReactionTextsToolStripMenuItem";
            resources.ApplyResources(this.jointReactionTextsToolStripMenuItem, "jointReactionTextsToolStripMenuItem");
            this.jointReactionTextsToolStripMenuItem.Click += new System.EventHandler(this.showJointReactionsTextsButton_Click);
            // 
            // ayudaToolStripMenuItem
            // 
            this.ayudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userManualToolStripMenuItem,
            this.tutorialsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            resources.ApplyResources(this.ayudaToolStripMenuItem, "ayudaToolStripMenuItem");
            // 
            // userManualToolStripMenuItem
            // 
            this.userManualToolStripMenuItem.Image = global::Canguro.Properties.Resources.help;
            this.userManualToolStripMenuItem.Name = "userManualToolStripMenuItem";
            resources.ApplyResources(this.userManualToolStripMenuItem, "userManualToolStripMenuItem");
            this.userManualToolStripMenuItem.Click += new System.EventHandler(this.userManualToolStripMenuItem_Click);
            // 
            // tutorialsToolStripMenuItem
            // 
            this.tutorialsToolStripMenuItem.Image = global::Canguro.Properties.Resources.tutorial;
            this.tutorialsToolStripMenuItem.Name = "tutorialsToolStripMenuItem";
            resources.ApplyResources(this.tutorialsToolStripMenuItem, "tutorialsToolStripMenuItem");
            this.tutorialsToolStripMenuItem.Click += new System.EventHandler(this.tutorialsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::Canguro.Properties.Resources.Logo16;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // selectionToolStrip
            // 
            resources.ApplyResources(this.selectionToolStrip, "selectionToolStrip");
            this.selectionToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllButton,
            this.invertSelectionButton,
            this.unselectButton,
            this.selectLineButton,
            this.selectConnectedButton,
            this.toolStripSeparator26,
            this.hideButton,
            this.showAllButton,
            this.toolStripSeparator27,
            this.distanceButton});
            this.selectionToolStrip.Name = "selectionToolStrip";
            // 
            // selectAllButton
            // 
            this.selectAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.selectAllButton, "selectAllButton");
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // invertSelectionButton
            // 
            this.invertSelectionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.invertSelectionButton, "invertSelectionButton");
            this.invertSelectionButton.Name = "invertSelectionButton";
            this.invertSelectionButton.Click += new System.EventHandler(this.invertSelectionButton_Click);
            // 
            // unselectButton
            // 
            this.unselectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.unselectButton.Image = global::Canguro.Properties.Resources.unselect;
            resources.ApplyResources(this.unselectButton, "unselectButton");
            this.unselectButton.Name = "unselectButton";
            this.unselectButton.Click += new System.EventHandler(this.unselectButton_Click);
            // 
            // selectLineButton
            // 
            this.selectLineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectLineButton.Image = global::Canguro.Properties.Resources.selectline;
            resources.ApplyResources(this.selectLineButton, "selectLineButton");
            this.selectLineButton.Name = "selectLineButton";
            this.selectLineButton.Click += new System.EventHandler(this.selectLineButton_Click);
            // 
            // selectConnectedButton
            // 
            this.selectConnectedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectConnectedButton.Image = global::Canguro.Properties.Resources.selectconnected;
            resources.ApplyResources(this.selectConnectedButton, "selectConnectedButton");
            this.selectConnectedButton.Name = "selectConnectedButton";
            this.selectConnectedButton.Click += new System.EventHandler(this.selectConnectedButton_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            resources.ApplyResources(this.toolStripSeparator26, "toolStripSeparator26");
            // 
            // hideButton
            // 
            this.hideButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideButton.Image = global::Canguro.Properties.Resources.hide;
            resources.ApplyResources(this.hideButton, "hideButton");
            this.hideButton.Name = "hideButton";
            this.hideButton.Click += new System.EventHandler(this.hideButton_Click);
            // 
            // showAllButton
            // 
            this.showAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.showAllButton, "showAllButton");
            this.showAllButton.Name = "showAllButton";
            this.showAllButton.Click += new System.EventHandler(this.showAllButton_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            resources.ApplyResources(this.toolStripSeparator27, "toolStripSeparator27");
            // 
            // distanceButton
            // 
            this.distanceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.distanceButton.Image = global::Canguro.Properties.Resources.distanceButton;
            resources.ApplyResources(this.distanceButton, "distanceButton");
            this.distanceButton.Name = "distanceButton";
            this.distanceButton.Click += new System.EventHandler(this.distanceButton_Click);
            // 
            // fileEditToolStrip
            // 
            resources.ApplyResources(this.fileEditToolStrip, "fileEditToolStrip");
            this.fileEditToolStrip.GripMargin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.fileEditToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newButton,
            this.openButton,
            this.saveButton,
            this.printButton,
            this.printPreviewButton,
            this.toolStripSeparator3,
            this.cutButton,
            this.copyButton,
            this.pasteButton,
            this.copyPasteButton,
            this.deleteButton,
            this.editButton,
            this.toolStripSeparator4,
            this.undoButton,
            this.redoButton,
            this.toolStripSeparator18,
            this.unitsComboBox});
            this.fileEditToolStrip.Name = "fileEditToolStrip";
            // 
            // newButton
            // 
            this.newButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.newButton, "newButton");
            this.newButton.Name = "newButton";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.openButton, "openButton");
            this.openButton.Name = "openButton";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // printButton
            // 
            this.printButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printButton.Image = global::Canguro.Properties.Resources.PrintHS;
            resources.ApplyResources(this.printButton, "printButton");
            this.printButton.Name = "printButton";
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // printPreviewButton
            // 
            this.printPreviewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printPreviewButton.Image = global::Canguro.Properties.Resources.PrintPreviewHS;
            resources.ApplyResources(this.printPreviewButton, "printPreviewButton");
            this.printPreviewButton.Name = "printPreviewButton";
            this.printPreviewButton.Click += new System.EventHandler(this.printPreviewButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // cutButton
            // 
            this.cutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.cutButton, "cutButton");
            this.cutButton.Name = "cutButton";
            this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.copyButton, "copyButton");
            this.copyButton.Name = "copyButton";
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.pasteButton, "pasteButton");
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyPasteButton
            // 
            this.copyPasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyPasteButton.Image = global::Canguro.Properties.Resources.copypaste;
            resources.ApplyResources(this.copyPasteButton, "copyPasteButton");
            this.copyPasteButton.Name = "copyPasteButton";
            this.copyPasteButton.Click += new System.EventHandler(this.copyPasteButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::Canguro.Properties.Resources.delete;
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // editButton
            // 
            this.editButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editButton.Image = global::Canguro.Properties.Resources.edit;
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Name = "editButton";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // undoButton
            // 
            this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.undoButton, "undoButton");
            this.undoButton.Name = "undoButton";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // redoButton
            // 
            this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.redoButton, "redoButton");
            this.redoButton.Name = "redoButton";
            this.redoButton.Click += new System.EventHandler(this.redoButton_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // unitsComboBox
            // 
            this.unitsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.unitsComboBox.Name = "unitsComboBox";
            resources.ApplyResources(this.unitsComboBox, "unitsComboBox");
            this.unitsComboBox.SelectedIndexChanged += new System.EventHandler(this.unitsComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            resources.ApplyResources(this.toolStripSeparator31, "toolStripSeparator31");
            // 
            // jointGridView
            // 
            resources.ApplyResources(this.jointGridView, "jointGridView");
            this.jointGridView.Name = "jointGridView";
            this.jointGridView.VirtualMode = true;
            // 
            // lineGridView
            // 
            resources.ApplyResources(this.lineGridView, "lineGridView");
            this.lineGridView.Name = "lineGridView";
            this.lineGridView.VirtualMode = true;
            // 
            // smallPanel
            // 
            resources.ApplyResources(this.smallPanel, "smallPanel");
            this.smallPanel.Name = "smallPanel";
            this.smallPanel.TrackingText = global::Canguro.Properties.Resources.CanguroPreferencesSmallPanelForeColorDescription;
            // 
            // MainFrm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menu;
            this.Name = "MainFrm";
            this.AutoSizeChanged += new System.EventHandler(this.MainFrm_AutoSizeChanged);
            this.MaximizedBoundsChanged += new System.EventHandler(this.MainFrm_MaximizedBoundsChanged);
            this.MaximumSizeChanged += new System.EventHandler(this.MainFrm_MaximumSizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFrm_FormClosed);
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.ResizeBegin += new System.EventHandler(this.MainFrm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainFrm_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.MainFrm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainFrm_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.MainFrm_DragOver);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainFrm_KeyPress);
            this.Resize += new System.EventHandler(this.MainFrm_Resize);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.RightToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.RightToolStripPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gridSplit.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSplit)).EndInit();
            this.gridSplit.ResumeLayout(false);
            this.gridTabs.ResumeLayout(false);
            this.tabJoints.ResumeLayout(false);
            this.tabJoints.PerformLayout();
            this.jointsToolStrip.ResumeLayout(false);
            this.jointsToolStrip.PerformLayout();
            this.tabFrames.ResumeLayout(false);
            this.tabFrames.PerformLayout();
            this.linesToolStrip.ResumeLayout(false);
            this.linesToolStrip.PerformLayout();
            this.modelToolStrip.ResumeLayout(false);
            this.modelToolStrip.PerformLayout();
            this.loadToolStrip.ResumeLayout(false);
            this.loadToolStrip.PerformLayout();
            this.templatesToolStrip.ResumeLayout(false);
            this.templatesToolStrip.PerformLayout();
            this.viewToolStrip.ResumeLayout(false);
            this.viewToolStrip.PerformLayout();
            this.resultsToolStrip.ResumeLayout(false);
            this.resultsToolStrip.PerformLayout();
            this.layersToolStrip.ResumeLayout(false);
            this.layersToolStrip.PerformLayout();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.selectionToolStrip.ResumeLayout(false);
            this.selectionToolStrip.PerformLayout();
            this.fileEditToolStrip.ResumeLayout(false);
            this.fileEditToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jointGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar progress1;
        private System.Windows.Forms.ToolStrip fileEditToolStrip;
        private System.Windows.Forms.ToolStripButton newButton;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton printButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton cutButton;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton pasteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton undoButton;
        private System.Windows.Forms.ToolStripButton redoButton;
        private System.Windows.Forms.ToolStrip viewToolStrip;
        private System.Windows.Forms.ToolStripButton selectButton;
        private System.Windows.Forms.ToolStripButton zoomButton;
        private System.Windows.Forms.ToolStripButton viewAllButton;
        private System.Windows.Forms.ToolStripButton panButton;
        private System.Windows.Forms.ToolStripButton rotateButton;
        private System.Windows.Forms.ToolStrip modelToolStrip;
        private System.Windows.Forms.ToolStripButton jointButton;
        private System.Windows.Forms.ToolStripButton lineButton;
        private System.Windows.Forms.SplitContainer gridSplit;
        private Canguro.View.SmallPanel smallPanel;
        private System.Windows.Forms.TabControl gridTabs;
        private System.Windows.Forms.TabPage tabJoints;
        private System.Windows.Forms.TabPage tabFrames;
        private System.Windows.Forms.ToolStripStatusLabel report2Label;
        private System.Windows.Forms.ToolStripProgressBar progress2;
        private System.Windows.Forms.ToolStripButton predefinedxy;
        private System.Windows.Forms.ToolStripButton predefinedxz;
        private System.Windows.Forms.ToolStripButton predefinedyz;
        private System.Windows.Forms.ToolStrip layersToolStrip;
        private System.Windows.Forms.ToolStripButton layerButton;
        private System.Windows.Forms.ToolStripButton editlayerButton;
        private System.Windows.Forms.ToolStripButton deletelayerButton;
        private System.Windows.Forms.ToolStrip resultsToolStrip;
        private System.Windows.Forms.ToolStripComboBox resultsCasesCombo;
        private System.Windows.Forms.ToolStripButton showDeformedButton;
        private System.Windows.Forms.ToolStripButton animateButton;
        private System.Windows.Forms.ToolStripButton showStressesButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton zoomInButton;
        private System.Windows.Forms.ToolStripButton zoomOutButton;
        private System.Windows.Forms.ToolStripButton zoomPreviousButton;
        private System.Windows.Forms.ToolStripButton viewShadedButton;
        private System.Windows.Forms.ToolStripDropDownButton showTextDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem showJointIDToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showLineIDToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showLineLengthToolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton showLoadsButton;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton predefinedxyzButton;
        private System.Windows.Forms.ToolStripButton lineStripButton;
        private System.Windows.Forms.ToolStripButton splitButton;
        private System.Windows.Forms.ToolStrip templatesToolStrip;
        private System.Windows.Forms.ToolStripButton gridButton;
        private System.Windows.Forms.ToolStripButton domeButton;
        private System.Windows.Forms.ToolStripButton cylinderButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton copyPasteButton;
        private System.Windows.Forms.ToolStripButton editButton;
        private System.Windows.Forms.ToolStripButton selectLayerButton;
        private System.Windows.Forms.ToolStripButton hideLayerButton;
        private System.Windows.Forms.ToolStripButton showLayerButton;
        private System.Windows.Forms.ToolStripButton moveToLayerButton;
        private System.Windows.Forms.ToolStripButton activateLayerButton;
        private System.Windows.Forms.ToolStripComboBox layersComboBox;
        private System.Windows.Forms.ToolStrip selectionToolStrip;
        private System.Windows.Forms.ToolStripButton selectAllButton;
        private System.Windows.Forms.ToolStripButton invertSelectionButton;
        private System.Windows.Forms.ToolStripButton unselectButton;
        private System.Windows.Forms.ToolStripDropDownButton diagramsDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem noDiagramsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem axialDiagramToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem s2DiagramToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem s3DiagramToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem torsionDiagramToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem m2DiagramToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem m3DiagramToolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStrip loadToolStrip;
        private System.Windows.Forms.ToolStripButton analysisCaseButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton loadCaseButton;
        private System.Windows.Forms.ToolStripButton forceLoadButton;
        private System.Windows.Forms.ToolStripButton groundDisplacementButton;
        private System.Windows.Forms.ToolStripButton concentratedLoadButton;
        private System.Windows.Forms.ToolStripButton distributedLoadButton;
        private System.Windows.Forms.ToolStripComboBox loadCasesComboBox;
        private System.Windows.Forms.ToolStripButton editLoadCaseButton;
        private System.Windows.Forms.ToolStripButton deleteLoadCaseButton;
        private System.Windows.Forms.ToolStripButton reportButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripComboBox unitsComboBox;
        private System.Windows.Forms.ToolStripButton loadComboButton;
        private System.Windows.Forms.ToolStripButton moveButton;
        private System.Windows.Forms.ToolStripButton mirrorButton;
        private System.Windows.Forms.ToolStripButton scaleButton;
        private System.Windows.Forms.ToolStripButton arrayButton;
        private System.Windows.Forms.ToolStripButton polararrayButton;
        private System.Windows.Forms.ToolStripButton modelrotateButton;
        private System.Windows.Forms.ToolStripButton joinButton;
        private System.Windows.Forms.ToolStrip jointsToolStrip;
        private System.Windows.Forms.ToolStripButton jointFillDownButton;
        private JointGridView jointGridView;
        private LineElementGridView lineGridView;
        private System.Windows.Forms.ToolStripMenuItem showFloorToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showAxesToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showLocalAxesToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showSectionsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem loadSizeToolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton analyzeButton;
        private System.Windows.Forms.ToolStripButton restraintAllButton;
        private System.Windows.Forms.ToolStripButton restraintTransButton;
        private System.Windows.Forms.ToolStripButton restraintZButton;
        private System.Windows.Forms.ToolStripButton restraintFreeButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripButton showDesignButton;
        private System.Windows.Forms.ToolStripButton sectionsButton;
        private System.Windows.Forms.ToolStripButton materialsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripMenuItem showDOFToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton intersectButton;
        private System.Windows.Forms.ToolStripButton arcButton;
        private System.Windows.Forms.ToolStripMenuItem releasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton selectLineButton;
        private System.Windows.Forms.ToolStripButton selectSimilarJointButton;
        private System.Windows.Forms.ToolStrip linesToolStrip;
        private System.Windows.Forms.ToolStripButton lineFillDownButton;
        private System.Windows.Forms.ToolStripButton selectSimilarLineButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripButton noReleaseButton;
        private System.Windows.Forms.ToolStripButton axialReleaseButton;
        private System.Windows.Forms.ToolStripButton momentReleaseButton;
        private System.Windows.Forms.ToolStripMenuItem jointCoordinatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton showJointReactionsButton;
        private System.Windows.Forms.ToolStripButton showJointReactionsTextsButton;
        private System.Windows.Forms.ToolStripButton flipLineButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripDropDownButton colorsSplitButton;
        private System.Windows.Forms.ToolStripMenuItem colorMaterialsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem colorSectionsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem colorLayersToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem colorLoadsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem hideJointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripButton hideButton;
        private System.Windows.Forms.ToolStripButton showAllButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripButton distanceButton;
        private System.Windows.Forms.ToolStripButton printPreviewButton;
        private System.Windows.Forms.ToolStripButton uniformLineLoadButton;
        private System.Windows.Forms.ToolStripButton triangleLineLoadButton;
        private System.Windows.Forms.ToolStripButton temperatureLineLoadButton;
        private System.Windows.Forms.ToolStripButton temperatureGradientLineLoadButton;
        //private System.Windows.Forms.TabPage tabShells;
        //private System.Windows.Forms.ToolStrip areasToolStrip;
        //private System.Windows.Forms.ToolStripButton areaFillDownButton;
        //private System.Windows.Forms.ToolStripButton selectSimilarAreaButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem importDXFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDXFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportS2kToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mirrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem polarArrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem intersectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interactiveZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem panToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem predefinedXYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem predefinedXZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem predefinedYZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem predefinedXYZToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewShadedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem showJointIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideJointsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showJointCoordinatesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showDOFToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showLineIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLineLenghtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLocalAxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem releasesToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showAxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFloorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSizesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewLoadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorMaterialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorSectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorLayersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorLoadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem materialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem forceLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groundDisplacementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem concentratedLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uniformLineLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangularLineLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distributedLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator30;
        private System.Windows.Forms.ToolStripMenuItem temperatureLineLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temperatureGradientLineLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem loadCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadComboToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem domeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cylinderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem capasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem hideLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activateLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unselectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.ToolStripMenuItem distanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyzeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem showDeformedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noDiagramsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem axialDiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem s2DiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem s3DiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem torsionDiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m2DiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m3DiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStressesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem designToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jointReactionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jointReactionTextsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tutorialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        //private AreaElementGridView areaGridView;
        private System.Windows.Forms.ToolStripMenuItem colorConstraintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorConstraintsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton constraintsButton;
        private System.Windows.Forms.ToolStripButton diaphragmsButton;
        private System.Windows.Forms.ToolStripMenuItem constraintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diaphragmsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator32;
        private System.Windows.Forms.ToolStripButton selectConnectedButton;
        private System.Windows.Forms.ToolStripMenuItem selectConnectedToolStripMenuItem;
    }
}