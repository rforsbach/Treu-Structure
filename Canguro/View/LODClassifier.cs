using System;
using System.Collections.Generic;

using Canguro.Model;
using Canguro.Model.Section;
using Canguro.View.Renderer;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    #region LODContour, LODSegments and LODLevels Data Structures
    public enum LODContour : byte
    {
        Wireframe = 255,
        Low = 0,
        Medium = 1,
        High = 2,
        HighStress = 3
    }

    public enum LODSegments : byte
    {
        FarFarAway = 1,
        FarAway = 2,
        Medium = 4,
        Near = 8,
        VeryNear = 16,
        Printing = 32
    }

    public struct LODLevels
    {
        public LODContour LODContour;
        public LODSegments LODSegments;

        public LODLevels(LODContour lodContour, LODSegments lodSegments)
        {
            LODContour = lodContour;
            LODSegments = lodSegments;
        }
    }
    #endregion

    public class LODClassifier
    {
        RenderOptions renderOptions;
        public LODClassifier(RenderOptions ro)
        {
            renderOptions = ro;
        }

        public float ZoomScale
        {
            get
            {
                return GraphicViewManager.Instance.ActiveView.ArcBallCtrl.ScaleOnZoom;
            }
        }

        public LODLevels GetLOD(Model.LineElement line)
        {
            LODLevels lodLevels;
            
            // LOD Segments
            float lengthSq = (line.J.Position - line.I.Position).LengthSq() * ZoomScale;
            if (lengthSq < 1f)
                lodLevels.LODSegments = LODSegments.FarFarAway;
            else if (lengthSq < 5f)
                lodLevels.LODSegments = LODSegments.FarAway;
            else if (lengthSq < 10f)
                lodLevels.LODSegments = LODSegments.Medium;
            else if (lengthSq < 20f)
                lodLevels.LODSegments = LODSegments.Near;
            else
                lodLevels.LODSegments = LODSegments.VeryNear;

            // LOD Contour
            StraightFrameProps sfp = line.Properties as StraightFrameProps;
            if (sfp != null)
            {
                FrameSection sec = sfp.Section;
                float lod = sec.LODSize * ZoomScale;

                if (lod < 0.1f && !renderOptions.ShowStressed)
                    lodLevels.LODContour = LODContour.Wireframe;
                else if (lod < 0.25f)
                    lodLevels.LODContour = LODContour.Low;
                else if (lod < 0.5f)
                    lodLevels.LODContour = LODContour.Medium;
                else if (!renderOptions.ShowStressed)
                    lodLevels.LODContour = LODContour.High;
                else
                    lodLevels.LODContour = LODContour.HighStress;
            }
            else
                lodLevels.LODContour = LODContour.Wireframe;

            return lodLevels;
        }
    }
}
