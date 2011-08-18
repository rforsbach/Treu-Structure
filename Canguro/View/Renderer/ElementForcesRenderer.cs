using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Analysis;

namespace Canguro.View.Renderer
{
    public class ElementForcesRenderer : LoadRenderer
    {
        float[] scale = null;

        public override void Render(Microsoft.DirectX.Direct3D.Device device, Model.Model model, RenderOptions options, List<Canguro.Model.Item> itemsInView)
        {
            if (!model.HasResults || model.Results.ElementJointForces == null || itemsInView == null ||
                options.InternalForcesShown == RenderOptions.InternalForces.None) return;

            if (itemsInView.Count <= 0)
                GetItemsInView(itemsInView);

            if (itemsInView.Count > 0)
            {

                int numPoints = Properties.Settings.Default.ElementForcesSegments;
                LineStressCalculator calc = new LineStressCalculator();

                // Calculate scale factor
                //if (scale == null)
                //    scale = getScale(calc, model.Results.ActiveCase.AbstractCase, model, options);
                scale = model.Results.PaintScaleFactorElementForces;

                ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

                int numTriangVerticesDrawn = 0;
                int numLineVerticesDrawn = 0;

                // Turn off lighting for color rendering
                device.RenderState.Lighting = false;
                device.RenderState.CullMode = Cull.None;

                // Vertex Buffer capture depends on current "renderer" (joints, lines, areas) and must be updated by them according to the feeded vertices
                // For simplicity, any "renderer" takes as parameter the last captured VertexBuffer and the number of vertices drawn
                PositionColoredPackage linePack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);
                PositionColoredPackage triangPack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionColored, false, true);

                Canguro.Model.LineElement line;

                foreach (Model.Item item in itemsInView)
                {
                    line = item as Model.LineElement;

                    if (line != null && line.IsVisible && line.IsSelected)
                    {
                        // Shear forces
                        switch (options.InternalForcesShown)
                        {
                            case RenderOptions.InternalForces.Sx:
                                paintDiagram(calc, model.Results.ActiveCase.AbstractCase, line,
                                    Canguro.Model.Load.LineForceComponent.Axial, numPoints, line.LocalAxes[2], Color.Magenta.ToArgb(), scale[0],
                                    ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                                break;
                            case RenderOptions.InternalForces.Sy:
                                paintDiagram(calc, model.Results.ActiveCase.AbstractCase, line,
                                    Canguro.Model.Load.LineForceComponent.Shear22, numPoints, Vector3.Scale(line.LocalAxes[1], -1), Color.Cyan.ToArgb(), scale[0],
                                    ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                                break;
                            case RenderOptions.InternalForces.Sz:
                                paintDiagram(calc, model.Results.ActiveCase.AbstractCase, line,
                                    Canguro.Model.Load.LineForceComponent.Shear33, numPoints, Vector3.Scale(line.LocalAxes[2], -1), Color.Cyan.ToArgb(), scale[0],
                                    ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                                break;

                            // Moments
                            case RenderOptions.InternalForces.Mx:
                                paintDiagram(calc, model.Results.ActiveCase.AbstractCase, line,
                                    Canguro.Model.Load.LineForceComponent.Torsion, numPoints, line.LocalAxes[2], Color.Magenta.ToArgb(), scale[1],
                                    ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                                break;
                            case RenderOptions.InternalForces.My:
                                paintDiagram(calc, model.Results.ActiveCase.AbstractCase, line,
                                    Canguro.Model.Load.LineForceComponent.Moment22, numPoints, Vector3.Scale(line.LocalAxes[2], -1), Color.Yellow.ToArgb(), scale[1],
                                    ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                                break;
                            case RenderOptions.InternalForces.Mz:
                                paintDiagram(calc, model.Results.ActiveCase.AbstractCase, line,
                                    Canguro.Model.Load.LineForceComponent.Moment33, numPoints, line.LocalAxes[1], Color.Yellow.ToArgb(), scale[1],
                                    ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                                break;
                        }
                    }
                }

                // Flush remaining vertices
                rc.ReleaseBuffer(numLineVerticesDrawn, 0, ResourceStreamType.Lines);
                rc.Flush(ResourceStreamType.Lines);
                rc.ReleaseBuffer(numTriangVerticesDrawn, 0, ResourceStreamType.TriangleListPositionColored);
                rc.Flush(ResourceStreamType.TriangleListPositionColored);

                //Turn on lighting
                device.RenderState.Lighting = true;
            }
        }

        private float[] getScale(LineStressCalculator calc, Model.Load.AbstractCase ac, Model.Model model, RenderOptions options)
        {
            float[,] diagram;
            int numPoints = 3;
            float[] s = new float[2];
            s[0] = s[1] = 0f;

            foreach (Model.LineElement line in model.LineList)
            {
                if (line != null)
                {
                    //Shears
                    diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Axial, numPoints);
                    s[0] = Math.Max(s[0], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                    diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Shear22, numPoints);
                    s[0] = Math.Max(s[0], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                    diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Shear33, numPoints);
                    s[0] = Math.Max(s[0], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));

                    //Moments
                    diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Torsion, numPoints);
                    s[1] = Math.Max(s[1], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                    diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Moment22, numPoints);
                    s[1] = Math.Max(s[1], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                    diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Moment33, numPoints);
                    s[1] = Math.Max(s[1], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                }
            }

            for (int i = 0; i < s.Length; i++)
                s[i] = 1f / s[i];

            return s;
        }

        private void paintDiagram(LineStressCalculator calc, Model.Load.AbstractCase ac, Model.LineElement line, Model.Load.LineForceComponent component, int numPoints, Vector3 direction, int color, float scaleFactor, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            // Get Diagram
            float[,] diagram = calc.GetForcesDiagram(ac, line, component, numPoints);

            // Scale for painting
            for (int i = 0; i < numPoints; i++)
                diagram[i, 1] *= scaleFactor;

            // Paint diagram
            // The direction is reversed because drawPolygonalLoad is optimized for Loads, 
            // which have to be drawn backwards (i.e. If the Load is in the direction of -Z,
            // then the load is drawn as if it comes from +Z and towards the line)
            //this.drawPolygonalLoad(diagram, line, direction, color, 0);
            this.drawPolygonalLoad(diagram, line, direction, color, 0, ref triangPack, ref linePack, ref numTriangVerticesInVB, ref numLineVerticesInVB);
        }
    }
}
