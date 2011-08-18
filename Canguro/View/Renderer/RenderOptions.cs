using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Renderer
{
    public class RenderOptions
    {
        [Flags]
        public enum ShowOptions : int
        {
            Nothing = 0,
            Loads = 1,
            JointIDs = 2,
            LineIDs = 4,
            LineLengths = 8,
            LineSections = 0x10,
            LineLocalAxes = 0x20,
            LoadMagnitudes = 0x40,
            GlobalAxes = 0x80,
            GridFloor = 0x100,
            JointDOFs = 0x200,
            Strains = 0x400,
            Releases = 0x800,
            Reactions = 0x1000,
            ReactionLoads = 0x2000,
            JointCoordinates = 0x4000,
            ShowJoints = 0x8000,
            ShellTransparency = 0x10000,
            ShellDiagonals = 0x20000,
        }

        public enum InternalForces
        {
            None,
            Sx, Sy, Sz,
            Mx, My, Mz
        }

        public enum RenderMode
        {
            Undeformed,
            Deformed,
            Animated
        }

        public enum RenderStyle
        {
            Wireframe,
            Shaded,
            Stressed
        }

        public enum LineColorBy
        {
            Material,
            Section,
            Layer,
            Constraint,
            NonDefaultPropertyAssigned,
            Design
        }

        #region Fields
        private RenderMode renderMode;
        private RenderStyle renderStyle;
        private ShowOptions showOptions;
        private InternalForces internalForces;
        private float deformationScale;
        private float deformationProgress;
        private ModelRenderer modelRenderer;
        private LODClassifier lodClassifier;
        private LineColorBy lineColoredBy;
        private LineColorBy lastColorBy;
        #endregion

        public LODClassifier LOD
        {
            get { return lodClassifier; }
            set { lodClassifier = value; }
        }

        public RenderOptions(ModelRenderer modelRenderer)
        {
            renderMode = RenderMode.Undeformed;
            renderStyle = RenderStyle.Wireframe;
            showOptions = ShowOptions.Loads | ShowOptions.GlobalAxes | ShowOptions.GridFloor | ShowOptions.JointDOFs;
            internalForces = InternalForces.None;
            deformationScale = 1f;
            this.modelRenderer = modelRenderer;
            lodClassifier = new LODClassifier(this);
            lineColoredBy = LineColorBy.Material;
            lastColorBy = lineColoredBy;
        }

        /// <summary>
        /// Gets or sets the animation progress as a value between [0,  1).
        /// This recalculates the deformation scale automaticcally as the sin(progress * 2 * PI)
        /// </summary>
        public float AnimationProgress
        {
            get { return deformationProgress; }
            set
            {
                deformationProgress = value - ((int)value);
                deformationScale = (float)Math.Sin(deformationProgress * 2.0 * Math.PI);
            }
        }
        
        public float DeformationScale
        {
            get { return deformationScale; }
            set { deformationScale = Math.Max(0, Math.Min(1, value)); }
        }
        
        public LineColorBy LineColoredBy
        {
            get { return lineColoredBy; }
            set { lineColoredBy = value; }
        }

        public bool ShowDeformed
        {
            get { return !ShowDesigned && (renderMode == RenderMode.Deformed || renderMode == RenderMode.Animated) && modelRenderer.HasResults; }
            set 
            {
                deformationScale = 1f;
                deformationProgress = 0f;
                internalForces = InternalForces.None;

                if (value)
                {
                    ShowDesigned = false;
                    renderMode = RenderMode.Deformed;
                }
                else
                {
                    renderMode = RenderMode.Undeformed;
                    if (renderStyle == RenderStyle.Stressed)
                        renderStyle = RenderStyle.Shaded;
                }
                modelRenderer.ReconfigureRenderers();
            }
        }

        public bool ShowAnimated
        {
            get
            { return !ShowDesigned && (renderMode == RenderMode.Animated) && modelRenderer.HasResults; }
            set 
            {
                ShowDesigned = false;
                deformationScale = 1f;
                deformationProgress = 0f;
                internalForces = InternalForces.None;
                if (value)
                    renderMode = RenderMode.Animated;
                else
                    renderMode = RenderMode.Deformed;
                modelRenderer.ReconfigureRenderers();
            }
        }

        public bool ShowShaded
        {
            get { return (renderStyle == RenderStyle.Shaded); }
            set 
            {
                if (value)
                    renderStyle = RenderStyle.Shaded;
                else
                    renderStyle = RenderStyle.Wireframe;
                modelRenderer.ReconfigureRenderers();
            }
        }

        public bool ShowStressed
        {
            get { return !ShowDesigned && (renderStyle == RenderStyle.Stressed) && modelRenderer.HasResults; }
            set 
            {
                ShowDesigned = false;
                if (value)
                {
                    if (!Controller.Controller.Instance.Execute("selectall"))
                        return;

                    renderStyle = RenderStyle.Stressed;
                    renderMode = RenderMode.Deformed;
                }
                else
                    renderStyle = RenderStyle.Shaded;
                modelRenderer.ReconfigureRenderers();
            }
        }

        public bool ShowDesigned
        {
            get { return (lineColoredBy == LineColorBy.Design) && modelRenderer.HasResults; }
            set 
            {
                if (value && modelRenderer.HasResults)
                {
                    ShowDeformed = false;
                    lastColorBy = lineColoredBy;
                    lineColoredBy = LineColorBy.Design;
                }
                else if(lineColoredBy == LineColorBy.Design)
                    lineColoredBy = lastColorBy;
            }
        }

        public ShowOptions OptionsShown
        {
            get { return showOptions; }
            set { showOptions = value; modelRenderer.ReconfigureRenderers(); }
        }

        public InternalForces InternalForcesShown
        {
            get 
            {
                if (!modelRenderer.HasResults)
                    internalForces = InternalForces.None;

                return internalForces; 
            }
            set 
            { 
                internalForces = value;
                if (internalForces != InternalForces.None)
                {
                    ShowDesigned = false;
                    deformationScale = 1f;
                    deformationProgress = 0f;
                    renderMode = RenderMode.Undeformed;
                    renderStyle = RenderStyle.Wireframe;
                }

                modelRenderer.ReconfigureRenderers(); 
            }
        }
    }
}
