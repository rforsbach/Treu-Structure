using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Section;
using Canguro.Model.Load;

using Canguro.Analysis;

namespace Canguro.View.Renderer
{
    public class DeformedLineWireframeRenderer : LineRenderer
    {
        LineDeformationCalculator calc = null;
        // DeformedLineCalculator Resources
        Vector3 vI, vJ, newDir;

        private Canguro.Model.Model model
        {
            get { return Canguro.Model.Model.Instance; }
        }

        public void SetVertexColoring(bool val)
        {
            vertexColoringEnabled = val;
        }

        private void drawDeformedLine(ResourceManager rc, LineElement l, bool pickingMode, RenderOptions options, Canguro.Model.Model model, ref PositionColoredPackage package, ref int numLineVerticesInVB)
        {
            if (l != null && l.IsVisible)
            {
                drawReleaseIfNeeded(rc, l, options);

                int numPoints = (int)options.LOD.GetLOD(l).LODSegments + 1;
                // Get joint defomations
                vI = new Vector3(model.Results.JointDisplacements[l.I.Id, 0],
                                 model.Results.JointDisplacements[l.I.Id, 1],
                                 model.Results.JointDisplacements[l.I.Id, 2]);
                vJ = new Vector3(model.Results.JointDisplacements[l.J.Id, 0],
                                 model.Results.JointDisplacements[l.J.Id, 1],
                                 model.Results.JointDisplacements[l.J.Id, 2]);

                Vector3 lineDir = l.J.Position - l.I.Position;
                lineDir.Normalize();

                vI = options.DeformationScale * model.Results.PaintScaleFactorTranslation * vI + l.I.Position + l.EndOffsets.EndIInternational * lineDir;
                vJ = options.DeformationScale * model.Results.PaintScaleFactorTranslation * vJ + l.J.Position - l.EndOffsets.EndJInternational * lineDir;

                if (numPoints > 2)
                {
                    // Get deformed line 'direction'
                    newDir = vJ - vI;

                    float[,] local2Values = calc.GetCurvedAxis(l, model.Results.ActiveCase.AbstractCase, LineDeformationCalculator.DeformationAxis.Local2, numPoints);
                    float[,] local3Values = calc.GetCurvedAxis(l, model.Results.ActiveCase.AbstractCase, LineDeformationCalculator.DeformationAxis.Local3, numPoints);

                    int nVertices = local2Values.GetLength(0) - 1;

                    // Check if there are enough space for adding vertices
                    int requiredLineVertices = nVertices * 2;

                    if (numLineVerticesInVB + requiredLineVertices >= package.NumVertices)
                    {
                        rc.ReleaseBuffer(numLineVerticesInVB, 0, ResourceStreamType.Lines);
                        package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, 0, requiredLineVertices, true);
                        numLineVerticesInVB = 0;
                    }
                    numLineVerticesInVB += requiredLineVertices;

                    unsafe
                    {
                        for (int i = 0; i < nVertices; ++i)
                        {
                            package.VBPointer->Position = vI + local2Values[i, 0] * newDir +
                                                        local2Values[i, 1] * options.DeformationScale * model.Results.PaintScaleFactorTranslation * l.LocalAxes[1] +
                                                        local3Values[i, 1] * options.DeformationScale * model.Results.PaintScaleFactorTranslation * l.LocalAxes[2];
                            package.VBPointer->Color = getLineColor(rc, l, pickingMode, options.LineColoredBy);
                            package.VBPointer++;

                            package.VBPointer->Position = vI + local2Values[i + 1, 0] * newDir +
                                                        local2Values[i + 1, 1] * options.DeformationScale * model.Results.PaintScaleFactorTranslation * l.LocalAxes[1] +
                                                        local3Values[i + 1, 1] * options.DeformationScale * model.Results.PaintScaleFactorTranslation * l.LocalAxes[2];
                            package.VBPointer->Color = getLineColor(rc, l, pickingMode, options.LineColoredBy);
                            package.VBPointer++;
                        }
                    }
                }
                else
                {
                    if (numLineVerticesInVB + 2 >= package.NumVertices)
                    {
                        rc.ReleaseBuffer(numLineVerticesInVB, 0, ResourceStreamType.Lines);
                        package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, 0, 2, true);
                        numLineVerticesInVB = 0;
                    }
                    numLineVerticesInVB += 2;

                    unsafe
                    {
                        package.VBPointer->Position = vI;
                        package.VBPointer->Color = getLineColor(rc, l, pickingMode, options.LineColoredBy);
                        package.VBPointer++;

                        package.VBPointer->Position = vJ;
                        package.VBPointer->Color = getLineColor(rc, l, pickingMode, options.LineColoredBy);
                        package.VBPointer++;
                    }
                }
            }
        }

        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            if (!model.HasResults || model.Results.JointDisplacements == null ) return;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            selectedColor = Properties.Settings.Default.SelectedDefaultColor.ToArgb();

            int numLineVerticesInVB = 0;

            if(device.RenderState.Lighting == true)
                device.RenderState.Lighting = false;

            if(device.RenderState.CullMode != Cull.None)
                device.RenderState.CullMode = Cull.None;

            // Renderer and device resources
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            calc = new LineDeformationCalculator();

            rc.ActiveStream = ResourceStreamType.Lines;

            PositionColoredPackage package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);

            if (lines != null && ((IList<LineElement>)lines).Count > 0)
            {
                foreach (LineElement l in lines)
                    drawDeformedLine(rc, l, pickingMode, options, model, ref package, ref numLineVerticesInVB);
            }
            else if (itemsInView != null)
            {
                // Get list of items in view (Bounding Box)
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    LineElement l;
                    foreach (Item item in itemsInView)
                    {
                        l = item as LineElement;
                        drawDeformedLine(rc, l, pickingMode, options, model, ref package, ref numLineVerticesInVB);
                    }
                }
            }

            // Flush remaining vertices
            rc.ReleaseBuffer(numLineVerticesInVB, 0, ResourceStreamType.Lines);
            rc.Flush(ResourceStreamType.Lines);
        }

    }
}
