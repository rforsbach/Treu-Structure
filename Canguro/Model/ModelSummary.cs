using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;

namespace Canguro.Model
{
    /// <summary>
    /// Cache class for accessing important model variables
    /// </summary>
    public class ModelSummary
    {
        private bool isUpdated = false;
        private int numJoints = 0;
        private int numLines = 0;
        private Vector3[] boundingBox = new Vector3[2];
        private Vector3 centroid;
        private Model model;
        private string modelTitle;
        private string modelAuthor;
        private int selectedJoints;
        private int selectedLines;
        private bool selectionValid = false;

        /// <summary>
        /// Constructor that saves a reference to the current Model object.
        /// </summary>
        /// <param name="model">The current Model object</param>
        public ModelSummary(Model model)
        {
            this.model = model;
            model.ModelChanged += new EventHandler(model_ModelChanged);
            model.ModelReset += new EventHandler(model_ModelChanged);
            model.SelectionChanged += new Model.SelectionChangedEventHandler(model_SelectionChanged);
        }

        void model_SelectionChanged(object sender, Model.SelectionChangedEventArgs e)
        {
            selectionValid = false;
        }

        void model_ModelChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Private method to update the summary if necessary. Called every time a value is accessed.
        /// </summary>
        private void Update()
        {
            if (!isUpdated)
            {
                numJoints = 0;
                boundingBox[0].Scale(0);
                boundingBox[1].Scale(0);
                centroid.Scale(0);
                foreach (Joint j in model.JointList)
                    if (j != null)
                    {
                        numJoints++;
                        Vector3 pos = j.Position;
                        boundingBox[0] = Vector3.Minimize(pos, boundingBox[0]);
                        boundingBox[1] = Vector3.Maximize(pos, boundingBox[1]);
                        centroid += pos;
                    }
                centroid.Multiply(1f / numJoints);

                numLines = 0;
                foreach (LineElement l in model.LineList)
                    if (l != null)
                        numLines++;
                isUpdated = true;
            }
        }

        /// <summary>
        /// Invalidates the current values. Forces an update next time a value is accessed.
        /// </summary>
        public void Invalidate()
        {
            isUpdated = false;
            selectionValid = false;
        }

        /// <summary>
        /// Gets the number of Joints in the Model
        /// </summary>
        public int NumJoints
        {
            get 
            {
                Update();
                return numJoints; 
            }
        }

        /// <summary>
        /// Gets the number of Line Elements in the Model
        /// </summary>
        public int NumLines
        {
            get
            {
                Update(); 
                return numLines;
            }
        }

        /// <summary>
        /// Gets a copy of the bounding box corners in a 2 element array
        /// </summary>
        public Vector3[] BoundingBox
        {
            get
            {
                Update();
                return new Vector3[] { boundingBox[0], boundingBox[1] };
            }
        }

        /// <summary>
        /// Gets the centroid of the joints in the model.
        /// </summary>
        public Vector3 Centroid
        {
            get
            {
                Update();
                return centroid;
            }
        }

        /// <summary>
        /// Gets or sets the author's name
        /// </summary>
        public string Author
        {
            get
            {
                return modelAuthor;
            }
            set
            {
                modelAuthor = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the model (appears in the report)
        /// </summary>
        public string Title
        {
            get
            {
                return modelTitle;
            }
            set
            {
                modelTitle = value;
            }
        }

        private void updateSelection()
        {
            selectedJoints = 0;
            foreach (Joint j in model.JointList)
                if (j != null && j.IsSelected)
                    ++selectedJoints;

            selectedLines = 0;
            foreach (LineElement l in model.LineList)
                if (l != null && l.IsSelected)
                    ++selectedLines;

            selectionValid = true;
        }

        /// <summary>
        /// Gets the number of currently selected joints
        /// </summary>        
        public int SelectedJoints
        {
            get 
            {
                if (!selectionValid)
                    updateSelection();

                return selectedJoints; 
            }
        }

        /// <summary>
        /// Gets the number of currently selected lines
        /// </summary>
        public int SelectedLines
        {
            get 
            {
                if (!selectionValid)
                    updateSelection();

                return selectedLines; 
            }
        }
    }
}
