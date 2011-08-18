using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator
{
    [Serializable]
    public class Point : ISelectable
    {
        protected System.Drawing.PointF position;
        [NonSerialized]
        bool isSelected = false;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y) : this ((float)x, (float)y)
        {
        }

        /// <summary>
        /// Receives the position in model (not user) coordinate system.
        /// </summary>
        /// <param name="pos">Position in model (not used, untransformed) coordinate system</param>
        internal Point(System.Drawing.PointF pos)
        {
            Position = pos;
        }

        public float X
        {
            get
            {
                return position.X / (int)Model.Instance.Unit;
            }
            set
            {
                if (value != X)
                {
                    Model.Instance.Undo.Change(this, X, GetType().GetProperty("X"));
                    position.X = value * (int)Model.Instance.Unit;
                }
            }
        }

        public float Y
        {
            get
            {
                return position.Y / (int)Model.Instance.Unit;
            }
            set
            {
                if (value != Y)
                {
                    Model.Instance.Undo.Change(this, Y, GetType().GetProperty("Y"));
                    position.Y = value * (int)Model.Instance.Unit;
                }
            }
        }

        /// <summary>
        /// Non undoable, non browsable, non unit system affected property
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal System.Drawing.PointF Position
        {
            get
            {
                return new System.Drawing.PointF(position.X / (float)Model.Instance.Unit, position.Y / (float)Model.Instance.Unit);
            }
            set
            {
                position = new System.Drawing.PointF(value.X * (float)Model.Instance.Unit, value.Y * (float)Model.Instance.Unit);
            }
        }

        public override string ToString()
        {
            return string.Format("({0:G3}, {1:G3})", X, Y);
        }

        public override bool Equals(object obj)
        {
            System.Drawing.PointF pt = ((Point)obj).position;
            return ((pt.X - position.X) * (pt.X - position.X) + (pt.Y - position.Y) * (pt.Y - position.Y)) < 1;
        }

        #region ISelectable Members

        [System.ComponentModel.Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }

        #endregion
    }
}
