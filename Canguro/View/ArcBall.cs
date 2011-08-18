using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    // Cortesía de Vanaithilion

    public class ArcBall:ICloneable
    {
        #region Instance Data
        protected Matrix translation;       // Matrix for arc ball's position
        protected Matrix translationDelta;  // Matrix for arc ball's position
        protected Matrix translationOld;    // Last translation

        protected Vector3 rotationCenter;
        protected Vector3 lastRotCenter;
        protected Matrix lastRotationMatrix;
        protected Vector3 zoomingCenter;

        protected Matrix scalingOld;        // Last scaling matrix
        protected Matrix scaling;           // Actual scaling matrix

        protected int width;                // arc ball's window width
        protected int height;               // arc ball's window height
        protected Vector2 center;           // center of arc ball 
        protected float radius;             // arc ball's radius in screen coords
        protected float radiusTranslation;  // arc ball's radius for translating the target

        protected Quaternion downQuat;      // Quaternion before button down
        protected Quaternion nowQuat;       // Composite quaternion for current drag

        protected System.Drawing.Point lastMousePosition; // position of last mouse point
        protected Vector3 downPt;           // starting point of rotation arc
        protected Vector3 currentPt;        // current point of rotation arc

        private int fx, fy;
        private Vector3 basisU, basisV;
        private GraphicView graphicView;

        private float scalingFac;
        private bool onZoom = false;
        private float scaleOnZoom = 0.0f;
        
        #endregion

        #region Simple Properties
        /// <summary>Gets the rotation matrix</summary>
        public Matrix RotationMatrix
        {
            get
            {
                return lastRotationMatrix * 
                       Matrix.Translation(-rotationCenter) * 
                       Matrix.RotationQuaternion(nowQuat) * 
                       Matrix.Translation(rotationCenter); 
            }
            set { lastRotationMatrix = value; }
        }
        public Matrix Scaling { get { return scaling; } internal set { scaling = value; } }
        public Matrix ViewMatrix { get { return RotationMatrix * TranslationMatrix * Scaling; } }
        /// <summary>Gets the translation matrix</summary>
        public Matrix TranslationMatrix 
        { 
            get { return translation; }
            internal set { translation = value; }
        }
        /// <summary>Gets or sets the current quaternion</summary>
        public Quaternion CurrentQuaternion { get { return nowQuat; } set { nowQuat = value; } }
        public float ScalingFac { get { return scalingFac; } internal set { scalingFac = value; } }
        public float ScaleOnZoom 
        { 
            get
            {
                if (onZoom)
                    return scaleOnZoom;
                else
                    return scalingFac; 
            } 
        }
        
        #endregion

        #region Class creation
        // Class methods

        /// <summary>
        /// Create new instance of the arcball class
        /// </summary>
        public ArcBall(GraphicView gv)
        {
            Reset();
            this.graphicView = gv;
            downPt = Vector3.Empty;
            currentPt = Vector3.Empty;

            zoomingCenter = new Vector3(0.0f, 0.0f, 0.5f);

            SetWindow(gv.Viewport.Width, gv.Viewport.Height);
        }

        /// <summary>
        /// Resets the arcball
        /// </summary>
        public void Reset()
        {
            downQuat = Quaternion.Identity;
            nowQuat = Quaternion.Identity;
            translation = Matrix.LookAtRH(new Vector3(5, 5, 0), new Vector3(5, 5, -1), new Vector3(0, 1, 0));
            translationDelta = Matrix.Identity;
            radius = 1.0f;
            radiusTranslation = 1.0f;

            scaling = Matrix.Identity;
            scalingOld = Matrix.Identity;

            lastRotationMatrix = Matrix.Identity;
            rotationCenter = Vector3.Empty;
            lastRotCenter = Vector3.Empty;

            scalingFac = 1.0f;
        }

        #endregion

        #region Quaternion related
        /// <summary>
        /// Convert a screen point to a vector
        /// </summary>
        public Vector3 ScreenToVector(float screenPointX, float screenPointY)
        {
            float x = -(screenPointX - width / 2.0f) / (radius * width / 2.0f);
            float y = (screenPointY - height / 2.0f) / (radius * height / 2.0f);
            float z = 0.0f;
            float mag = (x * x) + (y * y);

            if (mag > 1.0f)
            {
                float scale = 1.0f / (float)Math.Sqrt(mag);
                x *= scale;
                y *= scale;
            }
            else
                z = (float)Math.Sqrt(1.0f - mag);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Set window paramters
        /// </summary>
        public void SetWindow(int w, int h, float r)
        {
            width = w; height = h; radius = r;
            center = new Vector2(w / 2.0f, h / 2.0f);
        }
        public void SetWindow(int w, int h)
        {
            SetWindow(w, h, 0.9f); // default radius
        }

        /// <summary>
        /// Computes a quaternion from ball points
        /// </summary>
        public static Quaternion QuaternionFromBallPoints(Vector3 from, Vector3 to)
        {
            float dot = Vector3.Dot(from, -to);
            Vector3 part = Vector3.Cross(from, to);
            return new Quaternion(part.X, part.Y, part.Z, dot);
        }
        #endregion

        #region Rotating actions
        /// <summary>
        /// Rotate2 Command
        /// </summary>
        public void OnBeginRotate(System.Windows.Forms.MouseEventArgs e, Vector3 rotCenter)
        {
            /// Set rotating mode...
            downQuat = nowQuat;

            rotationCenter = rotCenter;
            rotationCenter.TransformCoordinate(lastRotationMatrix);

            lastRotCenter = rotCenter;

            SetWindow(graphicView.Viewport.Width, graphicView.Viewport.Height);

            downPt = ScreenToVector((float)e.X, (float)e.Y);
        }

        /// <summary>
        /// Rotation in progress
        /// </summary>
        public void OnMoveRot(System.Windows.Forms.MouseEventArgs e)
        {
            currentPt = ScreenToVector((float)e.X, (float)e.Y);
            nowQuat = downQuat * QuaternionFromBallPoints(downPt, currentPt);
        }

        public void OnEndRot()
        {
            lastRotationMatrix = lastRotationMatrix * 
                                 Matrix.Translation(-rotationCenter) * 
                                 Matrix.RotationQuaternion(nowQuat) * 
                                 Matrix.Translation(rotationCenter);

            nowQuat = Quaternion.Identity;
        }
        #endregion

        #region Panning actions
        /// <summary>
        /// Pan Command
        /// </summary>
        public void OnBeginPan(System.Windows.Forms.MouseEventArgs e)
        {
            SetWindow(graphicView.Viewport.Width, graphicView.Viewport.Height);

            // Store fx and fy as first interaction point
            fx = e.X; fy = e.Y;

            // Store current translation matrix
            translationOld = translation;

            // Calculate unitary (in terms of pixels) basis for displacement
            Vector3 zero = new Vector3(0, 0, 0.5f);
            basisU = new Vector3(1, 0, 0.5f);
            basisV = new Vector3(0, 1, 0.5f);

            Matrix realView = graphicView.ViewMatrix;
            graphicView.ViewMatrix = Matrix.Identity;

            graphicView.Unproject(ref zero);
            graphicView.Unproject(ref basisU);
            graphicView.Unproject(ref basisV);

            graphicView.ViewMatrix = realView;
            
            basisU -= zero;
            basisV -= zero;
        }

        /// <summary>
        /// Panning scene...
        /// </summary>
        public void OnMovePan(System.Windows.Forms.MouseEventArgs e)
        {
            float x = (float)e.X, y = (float)e.Y;

            Vector3 delta = basisU * (x - fx) + basisV * (y - fy);
            //delta = Vector3.TransformCoordinate(delta, ViewMatrix);
            translationDelta = Matrix.Translation(delta*(1/scalingFac));
            translation = translationDelta * translationOld;
        }
        #endregion

        #region Zooming actions
        /// <summary>
        /// Zoom Command
        /// </summary>
        public void OnBeginZoom(System.Windows.Forms.MouseEventArgs e)
        {
            lastMousePosition = new System.Drawing.Point(e.X, e.Y);
            scalingOld = scaling;

            onZoom = true;
        }

        /// <summary>
        /// The arcball is 'moving'
        /// </summary>
        public void OnMoveZoom(System.Windows.Forms.MouseEventArgs e)
        {
            float deltaY = (lastMousePosition.Y - e.Y);

            float s = (float)Math.Pow(2, deltaY * 0.02);

            scaleOnZoom = scalingFac * s;

            scaling = Matrix.Scaling(scalingFac*s, scalingFac*s, scalingFac*s);
        }

        /// <summary>
        /// The arcball is 'moving'
        /// </summary>
        public void OnEndZoom(System.Windows.Forms.MouseEventArgs e)
        {
            float deltaY = (lastMousePosition.Y - e.Y);
            float newscale;

            float s = (float)Math.Pow(2, deltaY * 0.02);

            newscale = scalingFac * s;

            if (newscale > 0.01f)
            {
                scalingFac = newscale;
                scaling = Matrix.Scaling(scalingFac, scalingFac, scalingFac);
            }

            onZoom = false;
        }

        public void ZoomStep(float step)
        {
            float newscale;

            newscale = scalingFac * (float)Math.Pow(2, step);

            if (newscale > 0.01f)
            {
                scalingFac = newscale;
                scaling = Matrix.Scaling(scalingFac, scalingFac, scalingFac);
            }

            onZoom = false;
        }

        public void ZoomAbsolute(float newscale)
        {
            if (newscale > 0.01f)
            {
                scaling = Matrix.Identity;
                scalingFac = newscale;
                scaling = Matrix.Scaling(scalingFac, scalingFac, scalingFac);
            }

            onZoom = false;
        }

        #endregion

        public void ResetRotation()
        {
            lastRotationMatrix = Matrix.Identity;
            downQuat = Quaternion.Identity;
            nowQuat = Quaternion.Identity;
            this.downQuat = Quaternion.Identity;
        }

        #region ICloneable Members

        public object Clone()
        {
            ArcBall value = new ArcBall(graphicView);

            value.fx = fx;

            value.translation = translation;       // Matrix for arc ball's position
            value.translationDelta = translationDelta;  // Matrix for arc ball's position
            value.translationOld = translationOld;    // Last translation

            value.rotationCenter = rotationCenter;
            value.lastRotCenter = lastRotCenter;
            value.lastRotationMatrix = lastRotationMatrix;
            value.zoomingCenter = zoomingCenter;

            value.scalingOld = scalingOld;        // Last scaling matrix
            value.scaling = scaling;           // Actual scaling matrix

            value.width = width;                // arc ball's window width
            value.height = height;               // arc ball's window height
            value.center = center;           // center of arc ball 
            value.radius = radius;             // arc ball's radius in screen coords
            value.radiusTranslation = radiusTranslation;  // arc ball's radius for translating the target

            value.downQuat = downQuat;      // Quaternion before button down
            value.nowQuat = nowQuat;       // Composite quaternion for current drag

            value.lastMousePosition = lastMousePosition; // position of last mouse point
            value.downPt = downPt;           // starting point of rotation arc
            value.currentPt = currentPt;        // current point of rotation arc

            value.fx = fx; 
            value.fy = fy;
            value.basisU = basisU; 
            value.basisV = basisV;
            value.graphicView = graphicView;

            value.scalingFac = scalingFac;
            value.onZoom = onZoom;
            value.scaleOnZoom = scaleOnZoom;

            return value;
        }

        #endregion
    }
}
