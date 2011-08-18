using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Microsoft.DirectX;
using System.Windows.Forms;

namespace Canguro.Commands.View
{
    /// <summary>
    /// Class for doing Zoom All over the active view
    /// </summary>
    public class ZoomAll : ViewCommand
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        private ZoomAll() { }
        /// <summary>
        /// Make a singleton for this command
        /// </summary>
        public static readonly ZoomAll Instance = new ZoomAll();

        /// <summary>
        /// Set interactionm behaviour
        /// </summary>
        public override bool IsInteractive
        {
            get { return false; }
        }

        public override bool SavePrevious
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Executuion method for this command
        /// </summary>
        /// <param name="activeView"> The view to zoom </param>
        public override void Run(Canguro.View.GraphicView activeView)
        {
            /// Get the joint list
            ItemList<Joint> jList = Canguro.Model.Model.Instance.JointList;
            
            // We need two vectors for having mininum and maximum BB corners
            Vector3 min = new Vector3(-1, -1, -1);
            Vector3 max = new Vector3(1, 1, 1);

            /// Get maximum and minimun from joint list
            foreach (Joint j in jList)
            {
                if (j != null && j.IsVisible )
                {
                    Vector3 pos = j.Position;
                    min.X = (min.X > pos.X) ? pos.X : min.X;
                    min.Y = (min.Y > pos.Y) ? pos.Y : min.Y;
                    min.Z = (min.Z > pos.Z) ? pos.Z : min.Z;
                    max.X = (max.X < pos.X) ? pos.X : max.X;
                    max.Y = (max.Y < pos.Y) ? pos.Y : max.Y;
                    max.Z = (max.Z < pos.Z) ? pos.Z : max.Z;
                }
            }

            /// Get line list
            ItemList<LineElement> lineList = Canguro.Model.Model.Instance.LineList;
            /// When all joints are invisible, we cannot compute a BB, so lets check against line joints
            foreach (LineElement l in lineList)
            {
                if (l != null && l.IsVisible)
                {
                    Vector3 pos = l.I.Position;
                    min.X = (min.X > pos.X) ? pos.X : min.X;
                    min.Y = (min.Y > pos.Y) ? pos.Y : min.Y;
                    min.Z = (min.Z > pos.Z) ? pos.Z : min.Z;
                    max.X = (max.X < pos.X) ? pos.X : max.X;
                    max.Y = (max.Y < pos.Y) ? pos.Y : max.Y;
                    max.Z = (max.Z < pos.Z) ? pos.Z : max.Z;

                    pos = l.J.Position;
                    min.X = (min.X > pos.X) ? pos.X : min.X;
                    min.Y = (min.Y > pos.Y) ? pos.Y : min.Y;
                    min.Z = (min.Z > pos.Z) ? pos.Z : min.Z;
                    max.X = (max.X < pos.X) ? pos.X : max.X;
                    max.Y = (max.Y < pos.Y) ? pos.Y : max.Y;
                    max.Z = (max.Z < pos.Z) ? pos.Z : max.Z;
                }
            }
            if (min.X < max.X)
            {
                // Diagonal de bounding box
                Vector3 diagonal = max - min;
                Vector3 center = min + Vector3.Multiply(diagonal, 0.5f);
                float modelSize = Vector3.Length(diagonal);
                float screenSize = (float)Math.Min(activeView.Viewport.Height-20, activeView.Viewport.Width-20);

                /************ Codigo genial **************
                 * Calculo de escala
                 * Calculo de traslacion
                 *****************************************/

                // Translation to bounding box center
                Vector3 centerProjected = center;
                activeView.Project(ref centerProjected);

                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, (int)centerProjected.X, (int)centerProjected.Y, 0);
                activeView.ArcBallCtrl.OnBeginPan(e);

                e = new MouseEventArgs(MouseButtons.Left, 1, activeView.Viewport.Width/2, activeView.Viewport.Height/2, 0);
                activeView.ArcBallCtrl.OnMovePan(e);

                activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;

                // View volume scaling

                Vector2[] maxDims = findMaxPoints(min, max, activeView);
                float sizeX = maxDims[1].X - maxDims[0].X;
                float sizeY = maxDims[1].Y - maxDims[0].Y;

                float newScale = 0.0f;

                newScale  = screenSize * activeView.ArcBallCtrl.ScalingFac / (float)Math.Max(sizeX, sizeY);

                activeView.ArcBallCtrl.ZoomAbsolute(newScale);
                activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            }
        }

        /// <summary>
        /// From min and max coords of the BB, project them according to the active view, for determining zooming scale
        /// </summary>
        /// <param name="min"> The min coord of BB </param>
        /// <param name="max"> The max coord of BB </param>
        /// <param name="activeView"> Active view to zoom </param>
        /// <returns> Returns an array of two vectos in screen space </returns>
        private Vector2[] findMaxPoints(Vector3 min, Vector3 max, Canguro.View.GraphicView activeView)
        {
            Vector3[] boundingVolume = new Vector3[8];

            Vector2[] screen = new Vector2[2];
            
            screen[0] = new Vector2(float.MaxValue, float.MaxValue);
            screen[1] = new Vector2(float.MinValue, float.MinValue);

            boundingVolume[0] = min;
            boundingVolume[1] = new Vector3(min.X, min.Y, max.Z);
            boundingVolume[2] = new Vector3(max.X, min.Y, max.Z);
            boundingVolume[3] = new Vector3(max.X, min.Y, min.Z);

            boundingVolume[4] = new Vector3(max.X, max.Y, min.Z);
            boundingVolume[5] = max;
            boundingVolume[6] = new Vector3(min.X, max.Y, max.Z);
            boundingVolume[7] = new Vector3(min.X, max.Y, min.Z);

            for (int i = 0; i < boundingVolume.GetLength(0); ++i)
            {
                activeView.Project(ref boundingVolume[i]);
                screen[0].X = (screen[0].X > boundingVolume[i].X) ? boundingVolume[i].X : screen[0].X;
                screen[0].Y = (screen[0].Y > boundingVolume[i].Y) ? boundingVolume[i].Y : screen[0].Y;

                screen[1].X = (screen[1].X < boundingVolume[i].X) ? boundingVolume[i].X : screen[1].X;
                screen[1].Y = (screen[1].Y < boundingVolume[i].Y) ? boundingVolume[i].Y : screen[1].Y;
            }

            return screen;
        }
    }
}

