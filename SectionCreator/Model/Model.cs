using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator
{
    public class Model : Canguro.Model.IModel
    {
        private ManagedList<Contour> contours;
        private bool modified;
        private Canguro.Model.Undo.UndoManager undoManager;
        private string currentPath = "";
        private LenghtUnits units = LenghtUnits.mm;

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static readonly Model Instance = new Model();
        private Model() 
        {
            Reset();
        }

        public void Reset()
        {
            contours = new ManagedList<Contour>();
            undoManager = new Canguro.Model.Undo.UndoManager(this);
            currentPath = "";
            ChangeModel();
            modified = false;
        }

        public Canguro.Model.Undo.UndoManager Undo
        {
            get { return undoManager; }
        }

        public ManagedList<Contour> Contours
        {
            get
            {
                return contours;
            }
        }

        // Gets fired when the model changes (except selection changes)
        public event EventHandler ModelChanged;

        // Gets fired when the model changes (except selection changes)
        public event EventHandler SelectionChanged;

        public void ChangeModel()
        {
            if (ModelChanged != null)
                this.ModelChanged(this, EventArgs.Empty);
            modified = true;
        }

        public void ChangeSelection()
        {
            if (SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);
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
                    throw new System.InvalidOperationException("Modified can only be set to true");
                modified = true;
            }
        }

        public void ClearSelection()
        {
            foreach (Contour con in contours)
            {
                foreach (Point p in con.Points)
                    p.IsSelected = false;
                con.IsSelected = false;
            }
            ChangeSelection();
        }

        /// <summary>
        /// Gets the current path, where the file is saved.
        /// </summary>
        public string CurrentPath
        {
            get { return currentPath; }
        }

        public void Save(string path)
        {
            currentPath = path;
            new Serializer(this).Serialize(path);
        }

        public void Load(string path)
        {
            Reset();
            undoManager.Enabled = false;
            new Deserializer(this).Deserialize(path);
            currentPath = path;
            modified = false;
            undoManager.Enabled = true;
        }

        public bool IsTemplate
        {
            get { return false; }
        }

        public LenghtUnits Unit
        {
            get { return units; }
            set 
            {
                float zoom = (float)value / (float)units;
                units = value;
                View.ViewState view = Controller.Instance.View;
                view.Zoom *= zoom;
                view.Pan = new System.Drawing.PointF(view.Pan.X / zoom, view.Pan.Y / zoom);
                ChangeModel();
            }
        }

        public List<ISelectable> GetSelection()
        {
            List<ISelectable> list = new List<ISelectable>();
            foreach (Contour con in contours)
            {
                if (con.IsSelected)
                    list.Add(con);
                foreach (Point p in con.Points)
                    if (p.IsSelected)
                        list.Add(p);
            }
            return list;
        }
    }
}
