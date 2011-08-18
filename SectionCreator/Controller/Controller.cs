using System;
using System.Collections.Generic;
using System.Text;
using Canguro.SectionCreator.Commands;
using Canguro.SectionCreator.View;

namespace Canguro.SectionCreator 
{
    class Controller
    {
        public static readonly Controller Instance = new Controller();

        private readonly SelectionCommand selectionCommand = new SelectionCommand();

        private Controller()
        {
        }

        private MainFrm mainFrame;
        private ViewCommand viewCommand;
        private ViewCommand lastCommand;
        private ViewState view;

        private Dictionary<string, ICommand> commands;

        public void Initialize()
        {
            LoadCommands();
            CurrentCommand = selectionCommand;
            lastCommand = null;
        }

        public MainFrm MainFrame
        {
            set 
            {
                mainFrame = value;
                mainFrame.SectionPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(MouseMove);
                mainFrame.SectionPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(MouseWheel);
                mainFrame.SectionPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(MouseClick);
                mainFrame.SectionPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(MouseDown);
                mainFrame.SectionPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUp);
                mainFrame.SectionPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
                mainFrame.KeyDown +=new System.Windows.Forms.KeyEventHandler(KeyDown);
                view = mainFrame.SectionPanel.View;
                Initialize();
            }
        }

        void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (viewCommand != null)
                viewCommand.MouseUp(e);
        }

        void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == System.Windows.Forms.Keys.Delete)
                Execute("Delete");
            if (e.KeyData == System.Windows.Forms.Keys.Escape)
                CancelCommand();
        }

        public void Execute(string cmd)
        {
            try
            {
                if (commands.ContainsKey(cmd))
                {
                    ICommand command = commands[cmd];
                    if (command is ViewCommand)
                    {
                        EndCommand();
                        CurrentCommand = (ViewCommand)command;
                        CurrentCommand.Init();
                    }
                    if (command is RunnableCommand)
                    {
                        ((RunnableCommand)command).Run(this, Model.Instance);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error ejecutando " + cmd);
#endif
            }
        }

        private void LoadCommands()
        {
            commands = new Dictionary<string, ICommand>();
            commands.Add("Selection", selectionCommand);
            commands.Add("Pan", new PanCommand());
            commands.Add("ZoomIn", new ZoomInCommand());
            commands.Add("ZoomOut", new ZoomOutCommand());
            commands.Add("ZoomAll", new ZoomAllCommand());
            commands.Add("Polygon", new AddContourCommand());
            commands.Add("Delete", new DeleteCommand());
            commands.Add("New", new NewCommand());
            commands.Add("Open", new OpenCommand());
            commands.Add("Save", new SaveCommand());
            commands.Add("SaveAs", new SaveAsCommand());
            commands.Add("Copy", new CopyCommand());
            commands.Add("Cut", new CutCommand());
            commands.Add("Paste", new PasteCommand());
            commands.Add("Edit", new EditCommand());
            commands.Add("Move", new MoveCommand());
            commands.Add("Undo", new UndoCommand());
            commands.Add("Redo", new RedoCommand());
            commands.Add("Merge", new MergeCommand());
        }

        void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (viewCommand != null)
            {
                if (viewCommand is SelectionCommand && e.Button == System.Windows.Forms.MouseButtons.Middle)
                    Execute("Pan");

                viewCommand.MouseDown(e);
                mainFrame.SectionPanel.Invalidate();
            }
        }

        void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (viewCommand != null)
            {
                viewCommand.MouseClick(e);
                mainFrame.SectionPanel.Invalidate();
            }
        }

        void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float delta = 1f + e.Delta / 1000f;
            delta = (delta > 10f) ? 10f : (delta < 0.1f) ? 0.1f : delta;
            View.Zoom *= delta;
            if (View.Zoom < 0.000001f)
                View.Zoom = 0.000001f;
            else if (View.Zoom > 1000000f)
                View.Zoom = 1000000f;

            mainFrame.SectionPanel.Invalidate();
        }

        public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lastPosition = e.Location;
            mainFrame.UpdateStatusBar();
            if (viewCommand != null)
            {
                viewCommand.MouseMove(e);
                mainFrame.SectionPanel.Invalidate();
            }
        }

        System.Drawing.Point lastPosition;
        public System.Drawing.Point MousePosition
        {
            get { return lastPosition; }
        }

        public ViewState View
        {
            get { return view; }
        }

        public void EndCommand()
        {
            Model.Instance.Undo.Commit();
            CurrentCommand = selectionCommand;
            mainFrame.SectionPanel.Invalidate();
        }

        public void CancelCommand()
        {
            Model.Instance.Undo.Rollback();
            CurrentCommand = selectionCommand;
            mainFrame.SectionPanel.Invalidate();
        }

        public void Paint(SectionPainter painter)
        {
            if (viewCommand != null && viewCommand.AllowSelection)
            {
                object obj = selectionCommand.GetObjectAt(lastPosition);
                if (obj is Contour)
                    painter.PaintContour((Contour)obj, System.Drawing.Brushes.Transparent, new System.Drawing.Pen(System.Drawing.Color.Red, 3), View);
                else if (obj is Point)
                    painter.PaintPoint((Point)obj, System.Drawing.Brushes.Red, View);
            }
        }

        public object GetHoverObject()
        {
            return selectionCommand.GetObjectAt(lastPosition);
        }

        /// <summary>
        /// Gets the object (if any) on which the cursor is.
        /// If the cursos is on a contour, returns the previos Point in the contour position.
        /// </summary>
        /// <param name="afterPoint"></param>
        /// <returns></returns>
        public object GetHoverObject(out Point afterPoint)
        {
            return selectionCommand.GetObjectAt(lastPosition, out afterPoint);
        }

        public ViewCommand CurrentCommand
        {
            get
            {
                return viewCommand;
            }
            set
            {
                mainFrame.SectionPanel.Cursor = value.Cursor;
                viewCommand = value;
                if (CommandChanged != null)
                    CommandChanged(this, EventArgs.Empty);
            }
        }

        public bool IsExecuting
        {
            get { return viewCommand != selectionCommand; }
        }

        public event EventHandler CommandChanged;
    }
}
