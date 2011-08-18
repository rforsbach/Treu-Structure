using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Canguro.SectionCreator.View
{
    class ViewState : ICloneable
    {
        public System.Drawing.PointF Pan = new System.Drawing.Point(0, 0);
        public float Zoom = 2f;
        public System.Drawing.Point viewport = new System.Drawing.Point();

        public object Clone()
        {
            ViewState ret = new ViewState();
            ret.Pan = Pan;
            ret.Zoom = Zoom;
            return ret;
        }

        /// <summary>
        /// Gets a point in screen coordinates and converts it to model coordinates.
        /// </summary>
        /// <param name="point">Point in screen coordinate system</param>
        /// <returns>Point in model coordinate system</returns>
        public PointF GetModelPosition(System.Drawing.Point point)
        {
            System.Drawing.PointF ret = new System.Drawing.PointF((point.X - viewport.X / 2f) / Zoom - Pan.X,
                (-point.Y + viewport.Y / 2f) / Zoom - Pan.Y);

            double log = Math.Log10(Zoom / 5f);
            double flog = Math.Floor(log);
            float fact = (float)Math.Pow(10.0, flog);
            fact = (log - flog > 0.69) ? fact * 2 : fact;

            ret.X = (float)Math.Round(ret.X * fact) / fact;
            ret.Y = (float)Math.Round(ret.Y * fact) / fact;

            return ret;
        }

        /// <summary>
        /// Gets a point in model coordinates and converts it to screen coordinates.
        /// </summary>
        /// <param name="point">Point in model coordinate system</param>
        /// <returns>Point in screen coordinate system</returns>
        public System.Drawing.Point GetScreenPosition(System.Drawing.PointF point)
        {
            return new System.Drawing.Point((int)((point.X + Pan.X) * Zoom + viewport.X / 2f), (int)((-point.Y - Pan.Y) * Zoom + viewport.Y / 2f));
        }
    }
}
