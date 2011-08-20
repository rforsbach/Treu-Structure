using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Canguro.Commands;
using Canguro.Model;
using Canguro.View;

namespace Canguro.Controller
{
    sealed public class Controller
    {
        /// <summary>
        /// <todo>
        /// Falta implementar Event Handlers del Panel de DirectX
        /// MouseWheel, H/V Scroll
        /// </todo> 
        /// </summary>

        private Queue<string> executeQueue;
        private MainFrm mainFrm;
        private CommandServices services;
        private Commands.ViewCommand lastViewCmd;
        private Commands.ViewCommand viewCmd;
        private Commands.ModelCommand modelCmd;
        internal readonly Commands.View.Selection SelectionCommand;
        internal readonly TrackingController TrackingController;
        private Credentials userCredentials;
        private ItemTextBuilder itemTextBuilder;

        private Controller() 
        {
            itemTextBuilder = new ItemTextBuilder();
            TrackingController = new TrackingController();
            SelectionCommand = Canguro.Commands.View.Selection.Instance;
            commands.Add("select", SelectionCommand);
            loadCommands();

            services = null;
            modelCmd = null;
            lastViewCmd = null;
            viewCmd = SelectionCommand;
            userCredentials = new Credentials();
            executeQueue = new Queue<string>();
            Idle += new EventHandler(flushExecuteQueue);
        }
        public static readonly Controller Instance = new Controller();

        private Dictionary<string, Command> commands = new Dictionary<string,Command>();

        /// <summary>
        /// Loads commands (view and model) as plug-ins using the Activator (when applicable)
        /// </summary>
        void loadCommands()
        {
            loadViewCommands();
            loadStdModelCommands();
        }

        /// <summary>
        /// Loads standard view commands
        /// </summary>
        void loadViewCommands()
        {
            commands.Add("selection", SelectionCommand);
            commands.Add("zoom", Commands.View.ZoomInteractive.Instance);
            commands.Add("zoomin", Commands.View.ZoomIn.Instance);
            commands.Add("zoomout", Commands.View.ZoomOut.Instance);
            commands.Add("zoomall", Commands.View.ZoomAll.Instance);
            commands.Add("zoomprevious", Commands.View.ZoomPrevious.Instance);
            commands.Add("pan", Commands.View.Pan.Instance);
            commands.Add("rotate", Commands.View.Trackball3D.Instance);
            commands.Add("zoomstep", Commands.View.ZoomStep.Instance);
            commands.Add("predefinedxy", Commands.View.PredefinedXY.Instance);
            commands.Add("predefinedxz", Commands.View.PredefinedXZ.Instance);
            commands.Add("predefinedyz", Commands.View.PredefinedYZ.Instance);
            commands.Add("predefinedxyz", Commands.View.PredefinedXYZ.Instance);

            commands.Add("hidediagrams", Commands.View.HideDiagrams.Instance);
            commands.Add("renderm1", Commands.View.RenderM1.Instance);
            commands.Add("renderm2", Commands.View.RenderM2.Instance);
            commands.Add("renderm3", Commands.View.RenderM3.Instance);
            commands.Add("renders1", Commands.View.RenderS1.Instance);
            commands.Add("renders2", Commands.View.RenderS2.Instance);
            commands.Add("renders3", Commands.View.RenderS3.Instance);

            commands.Add("colorlinesbymaterial", Commands.View.ColorLinesByMaterial.Instance);
            commands.Add("colorlinesbylayer", Commands.View.ColorLinesByLayer.Instance);
            commands.Add("colorlinesbysection", Commands.View.ColorLinesBySection.Instance);
            commands.Add("colorlinesbyassignment", Commands.View.ColorLinesByAssignment.Instance);
        }

        /// <summary>
        /// Loads standard model commands
        /// </summary>
        void loadStdModelCommands()
        {
            commands.Add("addjoint", Commands.Model.AddJointCmd.Instance);
            commands.Add("addline", Commands.Model.AddLineCmd.Instance);
            //commands.Add("addarea", Commands.Model.AddAreaCmd.Instance);

            commands.Add("hide", new Commands.Model.HideSelectionCmd());
            commands.Add("showall", new Commands.Model.ShowAllCmd());
            commands.Add("forceload", new Commands.Load.AddForceLoadCmd());
            commands.Add("grounddisplacementload", new Commands.Load.AddRestraintDisplacementLoadCmd()); // (~)
            commands.Add("concentratedload", new Commands.Load.AddConcentratedSpanLoadCmd());
            commands.Add("distributedspanload", new Commands.Load.AddDistributedSpanLoadCmd());
            commands.Add("uniformlineload", new Commands.Load.UniformLineLoadCmd());
            commands.Add("trianglelineload", new Commands.Load.TriangleLineLoadCmd());
            commands.Add("temperaturelineload", new Commands.Load.AddTemperatureLineLoadCmd());
            commands.Add("temperaturegradientlineload", new Commands.Load.TemperatureGradientLineLoadCmd());
            commands.Add("loadcase", new Commands.Load.AddLoadCaseCmd());
            commands.Add("deleteloadcase", new Commands.Load.DeleteLoadCaseCmd());
            commands.Add("editloadcase", new Commands.Load.EditLoadCaseCmd()); 
            commands.Add("analysiscase", new Commands.Load.AddAnalysisCaseCmd()); // (N)
            commands.Add("loadcombination", new Commands.Load.AddLoadCombinationCmd()); // (N)
            commands.Add("copy", new Commands.Model.CopyCmd());
            commands.Add("cut", new Commands.Model.CutCmd());
            commands.Add("paste", new Commands.Model.PasteCmd());
            commands.Add("join", new Commands.Model.JoinCmd());
            commands.Add("intersect", new Commands.Model.IntersectCmd());
            commands.Add("copypaste", new Commands.Model.CopyPasteCmd());
            commands.Add("undo", new Commands.Model.UndoCmd());
            commands.Add("redo", new Commands.Model.RedoCmd());
            commands.Add("mirror", new Commands.Model.MirrorCmd());
            commands.Add("scale", new Commands.Model.ScaleCmd());
            commands.Add("array", new Commands.Model.ArrayCmd());
            commands.Add("polararray", new Commands.Model.PolarArrayCmd());
            commands.Add("modelrotate", new Commands.Model.RotateCmd());
            commands.Add("joint", Commands.Model.AddJointCmd.Instance);
            commands.Add("line", Commands.Model.AddLineCmd.Instance);
            commands.Add("linestrip", Commands.Model.LineStripCmd.Instance);
            commands.Add("flipline", new Commands.Model.FlipLineJointsCmd());
            commands.Add("arc", new Commands.Model.ArcCircularCmd());
            commands.Add("split", Commands.Model.SplitCmd.Instance);
//            commands.Add("area", Commands.Model.AddAreaCmd.Instance);
            commands.Add("edit", new Commands.Model.EditCmd());
            commands.Add("delete", new Commands.Model.DeleteCmd());
            commands.Add("flipareajoints", Commands.Model.FlipAreaJoints.Instance);
            commands.Add("constraints", new Commands.Model.ConstraintsCmd());
            commands.Add("diaphragms", new Commands.Model.DiaphragmsCmd());

            commands.Add("layer", new Commands.Model.AddLayerCmd());
            commands.Add("editlayer", new Commands.Model.EditLayerCmd());
            commands.Add("deletelayer", new Commands.Model.DeleteLayer());
            commands.Add("selectlayer", new Commands.Model.SelectLayerCmd());
            commands.Add("hidelayer", new Commands.Model.HideLayerCmd());
            commands.Add("showlayer", new Commands.Model.ShowLayerCmd());
            commands.Add("movetolayer", new Commands.Model.MoveToLayerCmd());
            commands.Add("activatelayer", new Commands.Model.ActivateLayerCmd());

            commands.Add("selectall", new Commands.Model.SelectAllCmd());
            commands.Add("invertselection", new Commands.Model.InvertSelectionCmd());
            commands.Add("unselect", new Commands.Model.UnselectCmd());
            commands.Add("selectline", new Commands.Model.SelectLineCmd());
            commands.Add("selectconnected", new Commands.Model.SelectConnectedCmd());

            commands.Add("move", new Commands.Model.MoveCmd());
            commands.Add("grid", new Commands.Model.AddGridCmd());
            commands.Add("dome", new Commands.Model.AddDomeCmd());
            commands.Add("cylinder", new Commands.Model.AddCylinderCmd());
            commands.Add("3dtruss", new Commands.Model.Add3DTrussCmd());
            commands.Add("newwzd", new Commands.Model.NewModelWizard());
            commands.Add("open", new Commands.Model.OpenModelCmd());
            commands.Add("save", new Commands.Model.SaveModelCmd());
            commands.Add("saveas", new Commands.Model.SaveAsCmd());
            commands.Add("print", new Commands.Model.PrintCmd());
            commands.Add("printpreview", new Commands.Model.PrintPreviewCmd());
            commands.Add("savescreenshot", new Commands.Model.SaveScreenshot());
            commands.Add("analyze", new Commands.Model.AnalysisCmd());
            commands.Add("importdxf", new Commands.Model.ImportDXFCmd());
            commands.Add("importifc", new Commands.Model.ImportIFCCmd());
            commands.Add("exportdxf", new Commands.Model.ExportDXFCmd());
            commands.Add("exportifc", new Commands.Model.ExportIFCCmd());
            commands.Add("exportmdb", new Commands.Model.ExportMDBCmd());
            commands.Add("exportxml", new Commands.Model.ExportXMLCmd());
            commands.Add("exports2k", new Commands.Model.ExportS2kCmd());

            // New commands for debug
            commands.Add("createxml", new Commands.Model.CreateXMLCmd());


            commands.Add("testcmd", Commands.Model.TestCmd.Instance); // (N)

            commands.Add("report", new Commands.Model.MakeReportCmd());
            commands.Add("materials", new Commands.Model.MaterialsCmd());
            commands.Add("sections", new Commands.Model.SectionsCmd());
            commands.Add("distance", Commands.Model.InquiryDistance.Instance);
        }

        /// <summary>
        /// Gets the current view command, that is, the one executing at the moment
        /// </summary>
        public Commands.ViewCommand ViewCommand
        {
            get
            {
                return viewCmd;
            }
        }

        /// <summary>
        /// Gets the current model command, that is, the one executing at the moment
        /// </summary>
        public Commands.ModelCommand ModelCommand
        {
            get
            {
                return modelCmd;
            }
        }

        internal Credentials UserCredentials
        {
            get { return userCredentials; }
        }

        #region Idle Handling + ExecuteAsync and Animation controlling
        private IdleReason idleReason = IdleReason.NoIdle;
        
        internal void IdleStop(IdleReason reason)
        {
            idleReason &= (~reason);

            if (idleReason == IdleReason.NoIdle)
                System.Windows.Forms.Application.Idle -= new EventHandler(application_Idle);
        }

        internal void IdleStart(IdleReason reason)
        {
            if (idleReason == IdleReason.NoIdle)
                System.Windows.Forms.Application.Idle += new EventHandler(application_Idle);

            idleReason |= reason;
        }

        public void IdleReset()
        {
            idleReason = IdleReason.NoIdle;
            System.Windows.Forms.Application.Idle -= new EventHandler(application_Idle);
        }

        private void application_Idle(object sender, EventArgs e)
        {
            
            if (Idle != null)
                Idle(this, EventArgs.Empty);
        }

        public event EventHandler Idle;

        [Flags]
        internal enum IdleReason
        {
            NoIdle = 0,
            ExecuteAsync = 1,
            Animating = 2
        }

        private void flushExecuteQueue(object sender, EventArgs e)
        {
            IdleStop(IdleReason.ExecuteAsync);

            while (executeQueue.Count > 0)
            {
                string command = executeQueue.Dequeue();
                if (!string.IsNullOrEmpty(command))
                    Execute(command);
            }
        }

        /// <summary>
        /// Executes a command asynchronously (on next Application.Idle). See Execute for more details.
        /// </summary>
        /// <param name="command">The command to be asynchronously executed</param>
        public void ExecuteAsync(string command)
        {
            executeQueue.Enqueue(command);
            IdleStart(IdleReason.ExecuteAsync);
        }

        #endregion

        /// <summary>
        /// Este método es llamado como resultado de una acción del usuario, no de algún otro comando.
        /// Por esto sus mensajes de error se mandan al usuario como avisos y se resuelven aquí mismo
        /// en lugar de aventar excepciones.
        /// </summary>
        /// <param name="command">El nombre del comando a ejecutar</param>
        /// <returns>True if the command executed successfully. False otherwise.</returns>
        public bool Execute(string command)
        {
            // Clean command
            command = command.Trim().ToLowerInvariant();

            // Check cancel first
            if (command.Equals("cancel"))
            {
                // Cancel View Command, Model Command and Selection if applicable
                // (in that order, only one action per cancel)
                if ((viewCmd != SelectionCommand || Canguro.View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated) && EndViewCommand != null)
                    EndViewCommand(this, new EndViewCommandArgs(viewCmd));
                else
                {
                    if (modelCmd != null)
                    {
                        if (!modelCmd.AllowCancel())
                            return false;
                                                
                        endModelCommand();
                    }
                    else
                    {
                        // If there was no ModelCommand, clear and reset selection
                        Canguro.Model.Model.Instance.UnSelectAll();
                        SelectionCommand.Reset();
                    }
                }

                IdleReset();

                viewCmd = SelectionCommand;
                if (StartViewCommand != null)
                    StartViewCommand(this, EventArgs.Empty);
                mainFrm.ScenePanel.Cursor = SelectionCommand.DefaultCursor;
                
                return true;
            }
            
            // Check if command exists
            if (!commands.ContainsKey(command))
            {
                System.Windows.Forms.MessageBox.Show(Culture.Get("CommandNotFound") + ": '" + command + "'", Culture.Get("ActionNotPossibleTitle"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            // Execute Command
            // Prioritize View Commands
            Command cmd = commands[command];
            if (cmd is ViewCommand)
            {
                // If a series of viewCommands has ended (entered SelectionCommand)
                // then send the EndViewCommand signal
                // If Selection was doing anything and the viewCmd changes to zoom, pan, rotate, etc.
                // (exits selection mode), reset SelectionCommand
                if ((cmd == SelectionCommand) && (viewCmd != SelectionCommand) && (EndViewCommand != null))
                    EndViewCommand(this, new EndViewCommandArgs(viewCmd));
                else if ((cmd != SelectionCommand) && (viewCmd == SelectionCommand))
                    // Tal vez en lugar de un Reset sea mejor un Cancel, para no perder el tracking
                    SelectionCommand.Reset();

                // Instantiate and run new ViewCommand
                ViewCommand lastViewCmd = viewCmd;
                viewCmd = (ViewCommand)cmd;
                if (StartViewCommand != null)
                    StartViewCommand(this, EventArgs.Empty);

                if (viewCmd.SavePrevious)
                {
                    GraphicViewManager.Instance.SavePreviousActiveView();
                    //System.Console.WriteLine("SavePrevious");
                }

                if (viewCmd.IsInteractive)
                    mainFrm.ScenePanel.Cursor = viewCmd.Cursor;
                else
                {
                    viewCmd.Run(GraphicViewManager.Instance.ActiveView);
                    GraphicViewManager.Instance.updateView(false);
                    if (EndViewCommand != null)
                        EndViewCommand(this, new EndViewCommandArgs(viewCmd));

                    if (lastViewCmd == SelectionCommand)
                        viewCmd = SelectionCommand;
                    else
                    {
                        //if (lastViewCmd.SavePrevious && viewCmd != Commands.View.ZoomPrevious.Instance)
                        //{
                        //    GraphicViewManager.Instance.SavePreviousActiveView();
                        //    System.Console.WriteLine("SavePrevious");
                        //}

                        //viewCmd = lastViewCmd;

                        Execute("select");
                    }

                    if (StartViewCommand != null)
                        StartViewCommand(this, EventArgs.Empty);
                }
            }
            else if (cmd is ModelCommand)
            {
                if (modelCmd == null)
                {
                    // Por el momento funcionan en el mismo hilo, por lo que al terminar la
                    // llamada a Run se puede asignar modelCmd = null y services = null
                    modelCmd = (ModelCommand)cmd;
                    modelCmd.Cancel = false;
                    try
                    {
                        if (StartModelCommand != null)
                            StartModelCommand(this, EventArgs.Empty);
                        modelCmd.Run(services = new CommandServices(this));
                    }
                    catch (CancelCommandException) { Model.Model.Instance.Undo.Rollback(); }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Controller.Execute: {0}", ex.Message); }
                    finally
                    {
                        endModelCommand();
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(Culture.Get("ModelCmdAlreadyExecuting"), Culture.Get("ActionNotPossibleTitle"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Method called when a model command ends or is cancelled
        /// </summary>
        private void endModelCommand()
        {
            if (modelCmd != null)
                modelCmd.Cancel = true;

            mainFrm.ReportProgress("", 0, "", 0, DateTime.Now.AddMilliseconds(1));

            if (!mainFrm.SmallPanel.IsDisposed && !mainFrm.SmallPanel.Disposing)
                mainFrm.SmallPanel.Stop();

            mainFrm.HideCommandToolbox();

            Model.Model.Instance.Undo.Commit();

            modelCmd = null;
            if (services != null)
            {
                services.Dispose();
                services = null;
            }
            TrackingController.TrackingService = null;
            TrackingController.ResetStatus(Canguro.View.GraphicViewManager.Instance.ActiveView);
            TrackingController.SnapController.IsActive = false;
            TrackingController.SnapController.PrimaryPoint = Snap.PointMagnet.ZeroMagnet;
            
            // Reset selection
            SelectionCommand.Start(null);
            viewCmd = SelectionCommand;
            mainFrm.ScenePanel.Cursor = viewCmd.Cursor;

            if (EndModelCommand != null)
                EndModelCommand(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets or Sets (only one time allowed) the MainFrm. When the MainFrm is
        /// set, all behaviour-dependent events are linked here to the Controller.
        /// </summary>
        public MainFrm MainFrm
        {
            get
            {
                return mainFrm;
            }
            set
            {
                if (mainFrm != null)
                    throw new Exception("MainFrame is already defined");

                mainFrm = value;
                // Attach mouse events to Controller
                mainFrm.ScenePanel.MouseDown += new MouseEventHandler(scenePanel_MouseDown);
                mainFrm.ScenePanel.MouseMove += new MouseEventHandler(scenePanel_MouseMove);
                mainFrm.ScenePanel.MouseUp += new MouseEventHandler(scenePanel_MouseUp);
                mainFrm.ScenePanel.MouseWheel += new MouseEventHandler(scenePanel_MouseWheel);

                Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);

                // Set default Cursor
                mainFrm.ScenePanel.Cursor = SelectionCommand.Cursor;

                // Attach mainFrm KeyDown event as it catches all KeyDown events within the form
                // It will act as the main processing area for HotKeys inside the App
                mainFrm.KeyDown += new KeyEventHandler(processHotKeys);
                mainFrm.KeyPreview = true;                

                // Catch Grid click events (used to cancel current selection commands)
                DataGridView[] gvs = mainFrm.GetGridViews();
                foreach (DataGridView gv in gvs)
                    gv.Click += new EventHandler(spinteraction_looseFocusAfterClick);

                // Catch ActiveViewChange events 
                // (due to: new ActiveView, resize of current View, Layout change, etc.)
                GraphicViewManager.Instance.ActiveViewChange += new GraphicViewManager.ActiveViewChangedEventHandler(spinteraction_ActiveViewChange);

                // Connect to Model Events
                Model.Model.Instance.ModelChanged += new EventHandler(model_ModelChanged);
                Model.Model.Instance.SelectionChanged += new Canguro.Model.Model.SelectionChangedEventHandler(model_SelectionChanged);
                Model.Model.Instance.ResultsArrived += new EventHandler(model_ResultsArrived);
                Model.Model.Instance.ModelReset += new EventHandler(model_ModelReset);

                mainFrm.UpdateArea(UpdateAreaEvent.ModelChanged | UpdateAreaEvent.SelectionChanged, Model.Model.Instance, null);
            }
        }

        internal void ProcessCustomMessages(ref Message msg)
        {
            if (msg.Msg == Presenter.CM_PAINT)
            {
                if (msg.LParam == (IntPtr)(-1))
                    GraphicViewManager.Instance.Presenter.PresentImmediately(-1);
                else
                    GraphicViewManager.Instance.Presenter.PresentImmediately((int)msg.LParam);
            }
            else if (msg.Msg == Presenter.CM_TRACKINGPAINT)
                GraphicViewManager.Instance.Presenter.TrackingPaintImmediately((int)msg.LParam);            
        }

        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            //if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            //{
            //    if (Canguro.Model.Model.Instance.Modified)
            //    {

            //        DialogResult r = System.Windows.Forms.MessageBox.Show(Culture.Get(Canguro.Properties.Resources.SuspendMessage), Culture.Get(Canguro.Properties.Resources.SuspendMessageBoxCaption), MessageBoxButtons.YesNo);
            //        if (r == DialogResult.Yes)
            //            Execute("save");
            //    }

            //    //if (Canguro.Model.Model.Instance.HasResults)
            //    mainFrm.Close();
            //}
        }

        void model_ModelReset(object sender, EventArgs e)
        {
            MainFrm.UpdateArea(UpdateAreaEvent.ModelReset, (Canguro.Model.Model)sender, null);
        }

        void model_ResultsArrived(object sender, EventArgs e)
        {
            MainFrm.UpdateArea(UpdateAreaEvent.ResultsArrived, (Canguro.Model.Model)sender, null);
        }

        void model_SelectionChanged(object sender, Canguro.Model.Model.SelectionChangedEventArgs e)
        {
            // Activate the grid that corresponds to the picked item (if any)
            if (e.Picked is Joint)
                MainFrm.SetActiveGridTab(0);
            else if (e.Picked is LineElement)
                MainFrm.SetActiveGridTab(1);

            MainFrm.UpdateArea(UpdateAreaEvent.SelectionChanged, (Canguro.Model.Model)sender, null);
        }

        void model_ModelChanged(object sender, EventArgs e)
        {
            MainFrm.UpdateArea(UpdateAreaEvent.ModelChanged, (Canguro.Model.Model)sender, null);
        }

        /// <summary>
        /// Method to dispatch active view change events. This occurs when another view
        /// is selected, the view gets resized, the layout changes, etc.
        /// It resets the Selection Command.
        /// </summary>
        /// <param name="sender">The GraphicViewManager</param>
        /// <param name="e">Empty argument</param>
        void spinteraction_ActiveViewChange(object sender, GraphicViewManager.ActiveViewChangedEventArgs e)
        {
            if (viewCmd == SelectionCommand)
            {
                SelectionCommand.Reset();
                GraphicViewManager.Instance.UpdateView();
                TrackingController.Reset(e.NewView);
                mainFrm.ScenePanel.Cursor = viewCmd.Cursor;                
            }
        }

        /// <summary>
        /// Method to act as a Scenepanel LooseFocus event (i.e. a gridView Click event).
        /// It resets the Selection Command.
        /// </summary>
        /// <param name="sender">The control which sent the event</param>
        /// <param name="e">The argument of the click event</param>
        void spinteraction_looseFocusAfterClick(object sender, EventArgs e)
        {
            if (viewCmd == SelectionCommand)
            {
                SelectionCommand.Reset();
                GraphicViewManager.Instance.UpdateView();
                mainFrm.ScenePanel.Cursor = viewCmd.Cursor;
            }
        }

        /// <summary>
        /// Method to retrieve the control that currently has the Focus.
        /// </summary>
        /// <returns>The focused control or null if no .net control has the focus</returns>
        Control getFocusControl()
        {
            Control focusControl = null;
            IntPtr focusHandle = Canguro.Utility.NativeMethods.GetFocus();
            if (focusHandle != IntPtr.Zero)
                // returns null if handle is not to a .NET control
                focusControl = Control.FromHandle(focusHandle);
            return focusControl;
        }

        /// <summary>
        /// Method to dispatch HotKeys and act accordingly. It catches any KeyDown
        /// events coming from any control inside MainFrm and from MainFrm itself.
        /// </summary>
        /// <param name="sender">The MainFrm</param>
        /// <param name="e">The KeyDown event argument</param>
        void processHotKeys(object sender, KeyEventArgs e)
        {
#if DEBUG
            if (e.KeyCode == Keys.Space)
            {
                Control c = getFocusControl();
                if (c is TabControl || c is Form || c is Panel || c is ToolBar)
                    Canguro.View.GraphicViewManager.Instance.Layout = (Canguro.View.GraphicViewManager.ViewportsLayout)(((int)Canguro.View.GraphicViewManager.Instance.Layout + 1) % 8);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (mainFrm.SmallPanel.Height == 0)
                {
                    string str = InputBox.ShowInputBox();
                    Execute(str);
                }
            }
#endif
            if (e.KeyCode == Keys.Escape)
                Execute("cancel");
            else if (e.KeyCode == Keys.Delete && !gridHasFocus(sender))
                Execute("delete");
        }

        private bool gridHasFocus(object sender)
        {
            MainFrm mainFrm = sender as MainFrm;
            bool gridFocus = false;
            if (mainFrm != null)
            {

                DataGridView[] gvs = mainFrm.GetGridViews();
                foreach (DataGridView gv in gvs)
                    if (gv.ContainsFocus)
                    {
                        gridFocus = true;
                        break;
                    }
            }

            return gridFocus;
        }

        /// <summary>
        /// Method to dispatch the MouseWheel event from the ScenePanel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void scenePanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Diagnostics.Debug.Assert(mainFrm != null);

            // Store previous view command
            ViewCommand vcmd = viewCmd;

            // Execute zoom step
            viewCmd = (ViewCommand)commands["zoomstep"];
            GraphicViewManager.Instance.SavePreviousActiveView();
            viewCmd.MouseWheel(GraphicViewManager.Instance.ActiveView, e);
            GraphicViewManager.Instance.updateView(false);

            // Restore previous view command
            viewCmd = vcmd;

            if (viewCmd == SelectionCommand && EndViewCommand != null)
                EndViewCommand(this, new EndViewCommandArgs((ViewCommand)commands["zoomstep"]));
        }

        /// <summary>
        /// Method to dispatch the MouseUp event from the ScenePanel and forward it to
        /// the current ViewCommand. This method is responsible for releasing mouse 
        /// capture, ending view commands and updating views accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">MouseUp params</param>
        void scenePanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Diagnostics.Debug.Assert(mainFrm != null);
            if (viewCmd != null)
            {
                GraphicView gv = GraphicViewManager.Instance.ActiveView;
                bool windowSelectionOn = SelectionCommand.WindowSelectionOn;

                viewCmd.ButtonUp(gv, e);
                mainFrm.ScenePanel.Capture = false;

                // Check if its time to end current ViewCommand
                if (e.Button == System.Windows.Forms.MouseButtons.Right || e.Button == MouseButtons.Middle)
                {
                    if ((viewCmd != SelectionCommand || gv.ModelRenderer.RenderOptions.ShowAnimated) && EndViewCommand != null)
                        EndViewCommand(this, new EndViewCommandArgs(viewCmd));
                    else if (!SelectionCommand.IsWorkingAsCommandService)
                    {
                        if (!windowSelectionOn)
                            Canguro.Model.Model.Instance.UnSelectAll();
                    }

                    viewCmd = SelectionCommand;

                    if (StartViewCommand != null)
                        StartViewCommand(this, EventArgs.Empty);
                    mainFrm.ScenePanel.Cursor = SelectionCommand.Cursor;
                }

                if (e.Button == MouseButtons.Middle && lastViewCmd != null)
                {
                    if (lastViewCmd == Commands.View.Trackball3D.Instance)
                        Execute("rotate");
                    else if (lastViewCmd == Commands.View.Pan.Instance)
                        Execute("pan");
                    else if (lastViewCmd == Commands.View.ZoomInteractive.Instance)
                        Execute("zoom");

                    lastViewCmd = null;
                }
                
                if (viewCmd == SelectionCommand)
                    GraphicViewManager.Instance.UpdateView(gv);
            }
        }

        /// <summary>
        /// Method to dispatch the MouseMove event from the ScenePanel and forward it to
        /// the current ViewCommand. It is responsible of updating the current view if
        /// necessary and of setting the new tracking point for the active TrackingService.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">MouseMove params</param>
        void scenePanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Diagnostics.Debug.Assert(mainFrm != null);
            // Si el botón del mouse está apretado o es el SelectionCommand mandar evento
            // (La selección lo necesita aunque no esté apretado para poder manejar el Tracking)
            if ((viewCmd == SelectionCommand) || ((mainFrm.ScenePanel.Capture == true) && (viewCmd != null)))
            {
                GraphicViewManager gvm = GraphicViewManager.Instance;
                GraphicView gv = gvm.ActiveView;
                viewCmd.MouseMove(gv, e);

                if (viewCmd != SelectionCommand)
                    gvm.UpdateView(gv);
                else
                {
                    // Notify and Paint trackers
                    if (TrackingController.MouseMove(this, e, gv))
                        gvm.Presenter.TrackingPaint(this, e, gv, TrackingController);
                }
            }
        }

        /// <summary>
        /// Method to dispatch the MouseDown event from the ScenePanel and forward it to
        /// the current ViewCommand. It is responsible for capturing the mouse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void scenePanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Diagnostics.Debug.Assert(mainFrm != null);
            mainFrm.ScenePanel.Focus();
            GraphicViewManager.Instance.SetActiveViewFromPoint(e.X, e.Y);

            if (e.Button == MouseButtons.Middle)
            {
                lastViewCmd = viewCmd;

                if (Control.ModifierKeys == Keys.Control)
                    Execute("rotate");
                else
                    Execute("pan");
            }
            
            if (viewCmd != null)
            {
                if (viewCmd != SelectionCommand)
                    mainFrm.ScenePanel.Capture = true;
                
                // If animating, don't allow Selection
                if (viewCmd != SelectionCommand || !GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.ShowAnimated)
                {
                    viewCmd.ButtonDown(GraphicViewManager.Instance.ActiveView, e);
                    mainFrm.ScenePanel.Cursor = viewCmd.Cursor;
                }
            }
        }

        public void LoadModel(string path)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(path).ToLower();
                if (System.IO.File.Exists(path))
                {
                    switch (extension)
                    {
                        case ".dxf":
                            Commands.Model.ImportDXFCmd.Import(path, Model.Model.Instance);
                            break;
                        case ".xml":
                            new Canguro.Model.Serializer.Deserializer(Model.Model.Instance).Deserialize(path);
                            break;
                        default:
                            Model.Model.Instance.Load(path);
                            break;
                    }
                    Model.Model.Instance.Undo.Commit();
                }
                else
                    MessageBox.Show(Culture.Get("fileNotFound"), Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(Culture.Get("errorLoadingFile") + " " + path, Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Model.Model.Instance.Undo.Rollback();
            }
        }

        public class EndViewCommandArgs : EventArgs
        {
            private ViewCommand command;

            public EndViewCommandArgs(ViewCommand command)
            {
                this.command = command;
            }

            public ViewCommand Command
            {
                get { return command; }
            }
        }
        public delegate void EndViewCommandEventHandler(object sender, EndViewCommandArgs e);
        public event EndViewCommandEventHandler EndViewCommand;
        public event EventHandler EndModelCommand;
        public event EventHandler StartViewCommand;
        public event EventHandler StartModelCommand;

        #region InputBox
        /// <summary>
        /// Esta clase simula la función InputBox de Visual Basic 6.
        /// No es necesaria en el proyecto, pero es muy útil para hacer pruebas
        /// </summary>
        private class InputBox : System.Windows.Forms.Form
        {
            private System.Windows.Forms.TextBox textBox1;
            private System.ComponentModel.Container components = null;

            private InputBox()
            {
                InitializeComponent();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.textBox1 = new System.Windows.Forms.TextBox();
                this.SuspendLayout();
                //
                // textBox1
                //
                this.textBox1.Location = new System.Drawing.Point(16, 16);
                this.textBox1.Name = "textBox1";
                this.textBox1.Size = new System.Drawing.Size(256, 20);
                this.textBox1.TabIndex = 0;
                this.textBox1.Text = "";
                this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
                //
                // InputBox
                //
                this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
                this.ClientSize = new System.Drawing.Size(292, 53);
                this.ControlBox = false;
                this.Controls.AddRange(new System.Windows.Forms.Control[] { this.textBox1 });
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.Name = "InputBox";
                this.Text = "InputBox";
                this.ResumeLayout(false);
            }

            private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                    this.Close();
            }

            public static string ShowInputBox()
            {
                InputBox box = new InputBox();
                box.ShowDialog();
                return box.textBox1.Text;
            }
        }
        #endregion

        public ItemTextBuilder ItemTextBuilder
        {
            get
            {
                return itemTextBuilder;
            }
        }
    }
}
