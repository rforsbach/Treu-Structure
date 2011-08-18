using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Section;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Command to create a frame based cylinder
    /// </summary>
    public class AddCylinderCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Gets the parameters and calls createCylinder to add a Cylinder to the model
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (section == null)
                section = Canguro.Model.Section.SectionManager.Instance.DefaultFrameSection as Canguro.Model.Section.FrameSection;
            services.GetProperties(Culture.Get("cylinderCmdTitle"), this);

            Controller.Snap.Magnet m = services.GetPoint(Culture.Get("selectCylinderCenter"));
            if (m == null) return;
            Microsoft.DirectX.Vector3 o = m.SnapPosition;

            StraightFrameProps props = new StraightFrameProps();
            props.Section = section;
            createCylinder(services.Model, o, r, c, h, s + 1, props);
        }

        static protected float r = 5, h = 3;
        static protected int c = 6, s = 4;
        static Canguro.Model.Section.FrameSection section;

        /// <summary>
        /// Frame section to use in all the elements.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Editor(typeof(Canguro.Controller.PropertyGrid.FrameSectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [System.ComponentModel.Description("Frame section to use for all elements in the cylinder")]
        public Canguro.Model.Section.FrameSection Section
        {
            get { return section; }
            set { section = value; }
        }

        /// <summary>
        /// Gets or sets the Number of levels or rings in the cylinder. [0 - 100]
        /// </summary>
        [System.ComponentModel.Description("Number of levels or rings in the cylinder. [0 - 100]")]
        public int Stories
        {
            get { return s; }
            set { s = (value <= 0) ? 1 : (value > 100) ? 100 : value; }
        }

        /// <summary>
        /// Gets or sets the Number of columns in the cylinder. Same as number of segments in each ring. [0 - 100].
        /// </summary>
        [System.ComponentModel.Description("Number of columns in the cylinder. Same as number of segments in each ring. [0 - 100]")]
        public int Columns
        {
            get { return c; }
            set { c = (value <= 0) ? 1 : (value > 100) ? 100 : value; }
        }

        /// <summary>
        /// Gets or sets the Height of each level in the cylinder (distance between rings) [0 - 100m]
        /// </summary>
        [System.ComponentModel.Description("Height of each level in the cylinder (distance between rings) [0 - 100m]")]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Height
        {
            get { return h; }
            set { h = (value <= 0) ? 0.001F : value; }
        }

        /// <summary>
        /// Gets or sets the Radius of all the rings in the cylinder. [0 - 100m]
        /// </summary>
        [System.ComponentModel.Description("Radius of all the rings in the cylinder. [0 - 100m]")]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Radius
        {
            get { return r; }
            set { r = (value <=0) ? 0.001F : value; }
        }

        /// <summary>
        /// Creates a cylinder and adds it to the model.
        /// </summary>
        /// <param name="model">The Model object</param>
        /// <param name="C">The Center of the base</param>
        /// <param name="radius">The radius</param>
        /// <param name="cols">Number of columns</param>
        /// <param name="height">Height of each story</param>
        /// <param name="stories">Number of stories</param>
        /// <param name="props">Frame properties to use in all elements</param>
        protected void createCylinder(Canguro.Model.Model model, Vector3 C, float radius, int cols, float height, int stories, StraightFrameProps props)
        {
            float[,] columns = new float[cols, 3];
            int i, f, c;
            Queue<Joint> jQueue = new Queue<Joint>();
            Joint joint, first, prev;
            joint = prev = first = null;
            LineElement line;

            double angle, delta = 2 * Math.PI / (double)cols;
            float[] angles = new float[cols];

            for (i = 0, angle = 0; i < cols; angle += delta, i++)
            {
                columns[i, 0] = (float)(C.X + Math.Cos(angle) * radius);
                columns[i, 1] = (float)(C.Y + Math.Sin(angle) * radius);
                columns[i, 2] = (float)C.Z;
                angles[i] = (float)(angle * 180.0 / Math.PI);
            }

            JointDOF baseDoF = new JointDOF();
            baseDoF.T1 = baseDoF.T2 = baseDoF.T3 = JointDOF.DofType.Restrained;
            for (f = 0; f < stories; f++)
            {
                for (c = 0; c < cols; c++)
                {
                    joint = new Joint(columns[c, 0], columns[c, 1], columns[c, 2] + height * f);
                    if (c == 0) first = joint;
                    if (f == 0) joint.DoF = baseDoF;
                    model.JointList.Add(joint);
                    jQueue.Enqueue(joint);
                    if (f > 0)
                    {
                        model.LineList.Add(line = new LineElement(props));
                        line.I = jQueue.Dequeue();
                        line.J = joint;
                        line.Angle = angles[c];
                        if (c > 0)
                        {
                            model.LineList.Add(line = new LineElement(props));
                            line.I = prev;
                            line.J = joint;
                            if (c == cols - 1)
                            {
                                model.LineList.Add(line = new LineElement(props));
                                line.I = joint;
                                line.J = first;
                            }
                        }
                        prev = joint;
                    }
                }
            }
        } // End of createCylinder
    } // End of class
} // End of namespace
