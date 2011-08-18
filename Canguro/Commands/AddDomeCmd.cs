using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Section;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Command to create a semielitical frame based dome with round base.
    /// </summary>
    public class AddDomeCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Gets the parameters and calls createDome to add a Dome to the model
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (section == null)
                section = Canguro.Model.Section.SectionManager.Instance.DefaultFrameSection as Canguro.Model.Section.FrameSection;
            services.GetProperties(Culture.Get("domeCmdTitle"), this);

            Controller.Snap.Magnet m = services.GetPoint(Culture.Get("selectDomeCenter"));
            if (m == null) return;
            Microsoft.DirectX.Vector3 o = m.SnapPosition;

            StraightFrameProps props = new StraightFrameProps();
            props.Section = section;
            createDome(services.Model, o, r, c, h, s + 1, props);
        }

        static protected float r = 5, h = 5;
        static protected int c = 6, s = 4;
        static Canguro.Model.Section.FrameSection section;

        /// <summary>
        /// Frame section to use in all the elements.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Editor(typeof(Canguro.Controller.PropertyGrid.FrameSectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Canguro.Model.Section.FrameSection Section
        {
            get { return section; }
            set { section = value; }
        }

        /// <summary>
        /// Gets or sets the Number of levels or rings in the dome. [0 - 100]
        /// </summary>
        public int Stories
        {
            get { return s; }
            set { s = (value < 1) ? 1 : (value > 100)? 100 : value; }
        }

        /// <summary>
        /// Gets or sets the Integer number of columns in the dome. Also, the number of beams in each level. [1 - 100]
        /// </summary>
        public int Columns
        {
            get { return c; }
            set { c = (value < 1) ? 1 : (value > 100) ? 100 : value; }
        }

        /// <summary>
        /// Gets or sets the Distance from the center of the base to the top most node. [1 - 100m]
        /// </summary>
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Height
        {
            get { return h; }
            set { h = (value <= 0) ? 0.001F : value; }
        }

        /// <summary>
        /// Gets or sets the Radius of the Dome in the base. [0 - 100m]
        /// </summary>
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Radius
        {
            get { return r; }
            set { r = (value <= 0) ? 0.001F : value; }
        }


        /// <summary>
        /// Creates a dome and adds it to the model.
        /// </summary>
        /// <param name="model">The Model object</param>
        /// <param name="C">The Center of the base</param>
        /// <param name="radius">The radius</param>
        /// <param name="cols">Number of columns</param>
        /// <param name="height">Height of each story</param>
        /// <param name="stories">Number of stories</param>
        /// <param name="props">Frame properties to use in all elements</param>
        protected void createDome(Canguro.Model.Model model, Vector3 C, float radius, int cols, float height, int stories, StraightFrameProps props)
        {
            float[,] columns = new float[cols, 3];
            int i, f, c;
            Queue<Joint> jQueue = new Queue<Joint>();
            Joint joint, first, prev;
            joint = prev = first = null;
            LineElement line;

            double angle, delta = 2 * Math.PI / (double)cols;

            for (i = 0, angle = 0; i < cols; angle += delta, i++)
            {
                columns[i, 0] = (float)(C.X + Math.Cos(angle) * radius);
                columns[i, 1] = (float)(C.Y + Math.Sin(angle) * radius);
                columns[i, 2] = (float)C.Z;
            }

            JointDOF baseDoF = new JointDOF();
            baseDoF.T1 = baseDoF.T2 = baseDoF.T3 = JointDOF.DofType.Restrained;
    		double angle2 = 0.0, delta2 = Math.PI / (2.0 * stories);
            for (angle2 = 0.0, f = 0; f < stories; angle2 += delta2, f++)
            {
                for (angle = 0.0, c = 0; c < cols; angle += delta, c++)
                {
                    columns[c, 0] = (float)(C.X + Math.Cos(angle2) * Math.Cos(angle) * r);
                    columns[c, 1] = (float)(C.Y + Math.Cos(angle2) * Math.Sin(angle) * r);
                    columns[c, 2] = (float)(C.Z + Math.Sin(angle2) * height);
                    joint = new Joint(columns[c, 0], columns[c, 1], columns[c, 2]);
                    if (c == 0) first = joint;
                    if (f == 0) joint.DoF = baseDoF;
                    model.JointList.Add(joint);
                    jQueue.Enqueue(joint);
                    if (f > 0)
                    {
                        model.LineList.Add(line = new LineElement(props));
                        line.I = jQueue.Dequeue();
                        line.J = joint;
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
                } // End of for(cols)
            } // End of for(stories)
            model.JointList.Add(joint = new Joint(C.X, C.Y, C.Z + height));
		    for (c=0; c<cols; c++)
            {
                model.LineList.Add(line = new LineElement(props));
                line.I = jQueue.Dequeue();
                line.J = joint;
		    }
        } // End of createCylinder
    } // End of class
} // End of namespace

