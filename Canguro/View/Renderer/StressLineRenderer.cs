using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Analysis;
using Canguro.Utility;
using Canguro.Model;

namespace Canguro.View.Renderer
{
    public class StressLineRenderer : DeformedLineShadedRenderer
    {
        Model.Results.ResultsCase currentCase = null;
        Model.LineElement currentLine = null;
        int[] colors = new int[10000];
        LineStressCalculator lsc = null;
        float deformationScale = 1f;
        List<LineElement> selectedItems = new List<LineElement>();
        List<LineElement> unselectedItems = new List<LineElement>();
        StressWireframeLineRenderer swlr;
        
        public StressLineRenderer(DeformedLineWireframeRenderer dwlr, StressWireframeLineRenderer swlr):base(dwlr)
        {
            vertexColoringEnabled = true;
            this.swlr = swlr;
        }

        protected void GetItemsInView(List<Item> itemsInView, List<LineElement> selectedItems, List<LineElement> unselectedItems)
        {
            Microsoft.DirectX.Direct3D.Viewport vp = Canguro.View.GraphicViewManager.Instance.ActiveView.Viewport;
            Canguro.Controller.Controller.Instance.SelectionCommand.SelectWindow(Canguro.View.GraphicViewManager.Instance.ActiveView, vp.X, vp.Y, vp.X + vp.Width, vp.Y + vp.Height, itemsInView, Canguro.Controller.SelectionFilter.Lines);

            foreach (Item i in itemsInView)
            {
                LineElement l = i as LineElement;

                if (l != null)
                {
                    if (l.IsSelected)
                        selectedItems.Add(l);
                    else
                        unselectedItems.Add(l);
                }
            }
        }

        public override void Render(Device device, Model.Model model, IEnumerable<Canguro.Model.LineElement> lines, RenderOptions options, List<Canguro.Model.Item> itemsInView)
        {          
            if (!model.HasResults || model.Results.ElementJointForces == null) return;

            deformationScale = options.DeformationScale;
            lastMaterialColor = 0;

            Analysis.StressHelper srh = model.Results.StressHelper;

            srh.Reset(model);
            if (lsc == null)
                lsc = new LineStressCalculator();

            if (lines != null && ((IList<LineElement>)lines).Count > 0)
            {
                base.Render(device, model, lines, options, itemsInView);
            }
            else if (itemsInView != null)
            {
                // Get list of items in view (Bounding Box)
                selectedItems.Clear();
                unselectedItems.Clear();
                GetItemsInView(itemsInView, selectedItems, unselectedItems);

                swlr.Render(device, model, unselectedItems, options, null);
                base.Render(device, model, selectedItems, options, null);
            }

            if (!GraphicViewManager.Instance.PrintingHiResImage)
            {
                // Draw stress scale bar
                device.RenderState.ShadeMode = ShadeMode.Gouraud;
                GraphicViewManager gvm = GraphicViewManager.Instance;
                Viewport vp = gvm.ActiveView.Viewport;
                CustomVertex.TransformedColored[] scaleBarVerts = new CustomVertex.TransformedColored[10];
                // Iverted order for having compression above, and expansion bellow device center's
                scaleBarVerts[9].Position = new Vector4(vp.X + vp.Width - 105, vp.Y + 25, 0, 1);
                scaleBarVerts[8].Position = new Vector4(vp.X + vp.Width - 125, vp.Y + 25, 0, 1);
                scaleBarVerts[7].Position = new Vector4(vp.X + vp.Width - 105, vp.Y + 25 + (vp.Height - 50) * .25f, 0, 1);
                scaleBarVerts[6].Position = new Vector4(vp.X + vp.Width - 125, vp.Y + 25 + (vp.Height - 50) * .25f, 0, 1);
                scaleBarVerts[5].Position = new Vector4(vp.X + vp.Width - 105, vp.Y + 25 + (vp.Height - 50) * .5f, 0, 1);
                scaleBarVerts[4].Position = new Vector4(vp.X + vp.Width - 125, vp.Y + 25 + (vp.Height - 50) * .5f, 0, 1);
                scaleBarVerts[3].Position = new Vector4(vp.X + vp.Width - 105, vp.Y + 25 + (vp.Height - 50) * .75f, 0, 1);
                scaleBarVerts[2].Position = new Vector4(vp.X + vp.Width - 125, vp.Y + 25 + (vp.Height - 50) * .75f, 0, 1);
                scaleBarVerts[1].Position = new Vector4(vp.X + vp.Width - 105, vp.Y + 25 + vp.Height - 50, 0, 1);
                scaleBarVerts[0].Position = new Vector4(vp.X + vp.Width - 125, vp.Y + 25 + vp.Height - 50, 0, 1);
                //scaleBarVerts[10].Position = new Vector4(vp.X - 100, vp.Y + (vp.Height - 50) * (srh.MinStress / (srh.MinStress-srh.MaxStress)), 0, 1);
                //scaleBarVerts[11].Position = new Vector4(vp.X - 80, vp.Y + (vp.Height - 50) * (srh.MinStress / (srh.MinStress-srh.MaxStress)), 0, 1);

                float minStress, maxStress;
                minStress = srh.getMinStress(model);
                maxStress = srh.getMaxStress(model);

                scaleBarVerts[0].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress);
                scaleBarVerts[1].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress);
                scaleBarVerts[2].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress + (maxStress - minStress) * .25f);
                scaleBarVerts[3].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress + (maxStress - minStress) * .25f);
                scaleBarVerts[4].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress + (maxStress - minStress) * .50f);
                scaleBarVerts[5].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress + (maxStress - minStress) * .50f);
                scaleBarVerts[6].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress + (maxStress - minStress) * .75f);
                scaleBarVerts[7].Color = ColorUtils.GetColorFromStress(srh.Largest, minStress + (maxStress - minStress) * .75f);
                scaleBarVerts[8].Color = ColorUtils.GetColorFromStress(srh.Largest, maxStress);
                scaleBarVerts[9].Color = ColorUtils.GetColorFromStress(srh.Largest, maxStress);
                //scaleBarVerts[10].Color = srh.GetColorFromStress(0);
                //scaleBarVerts[11].Color = srh.GetColorFromStress(0);

                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
                try
                {
                    minStress = srh.getMinStress(model);
                    maxStress = srh.getMaxStress(model);

                    device.VertexFormat = CustomVertex.TransformedColored.Format;
                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 8, scaleBarVerts);
                    device.RenderState.ShadeMode = ShadeMode.Flat;

                    // Draw Texts
                    int fontHeight = -6, y0 = (int)(fontHeight + vp.Y + 25 + (vp.Height - 50) * (minStress / (minStress - maxStress)));

                    // Every text is drawn following the same rule as stress ruler

                    string stressUnit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Stress);
                    string numberFormat = "G5";
                    int offsetX = 100;

                    // Zero 
                    gvm.ResourceManager.LabelFont.DrawText(null, "0.00" + " [" + stressUnit + "]",
                        new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vp.Y + 25 + (vp.Height - 50) * 0.50f)),
                        GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : System.Drawing.Color.White);

                    // Minimum stress
                    if (Math.Abs(y0 - (fontHeight + vp.Y + 25)) > 15)
                        gvm.ResourceManager.LabelFont.DrawText(null, minStress.ToString(numberFormat) + " [" + stressUnit + "]",
                            new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vp.Y + vp.Height - 25)),
                            GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : System.Drawing.Color.White);

                    // Maximum stress
                    if (Math.Abs(y0 - (fontHeight + vp.Y + vp.Height - 25)) > 15)
                        gvm.ResourceManager.LabelFont.DrawText(null, maxStress.ToString(numberFormat) + " [" + stressUnit + "]",
                            new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vp.Y + 25)),
                            GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : System.Drawing.Color.White);

                    // Quarter of total from min stress
                    if (Math.Abs(y0 - (fontHeight + vp.Y + 25 + (vp.Height - 50) * 0.25f)) > 15)
                        gvm.ResourceManager.LabelFont.DrawText(null, (minStress + (maxStress - minStress) * 0.25f).ToString(numberFormat) + " [" + stressUnit + "]",
                            new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vp.Y + 25 + (vp.Height - 50) * 0.75f)),
                            GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : System.Drawing.Color.White);

                    // Half of total from min stress
                    if (Math.Abs(y0 - (fontHeight + vp.Y + 25 + (vp.Height - 50) * 0.50f)) > 15)
                        gvm.ResourceManager.LabelFont.DrawText(null, (minStress + (maxStress - minStress) * 0.50f).ToString(numberFormat) + " [" + stressUnit + "]",
                            new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), y0),
                            GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : System.Drawing.Color.White);

                    // Three quarters of total from min stress
                    if (Math.Abs(y0 - (fontHeight + vp.Y + 25 + (vp.Height - 50) * 0.75f)) > 15)
                        gvm.ResourceManager.LabelFont.DrawText(null, (minStress + (maxStress - minStress) * 0.75f).ToString(numberFormat) + " [" + stressUnit + "]",
                            new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vp.Y + 25 + (vp.Height - 50) * 0.25f)),
                            GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : System.Drawing.Color.White);
                }
                finally
                {
                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                }
            }
        }

        protected override int GetColor(Canguro.Model.Results.ResultsCase rc, Canguro.Model.LineElement line, int index, LODLevels lineLOD, StressHelper srh)
        {
            if (rc == null)
            {
                currentLine = null;
                return 0;
            }

            LODLevels lineLODLevel;
            if (GraphicViewManager.Instance.PrintingHiResImage)
                lineLODLevel = new LODLevels(LODContour.HighStress, LODSegments.VeryNear);
            else
                lineLODLevel = lineLOD;

            if ((line != null && line != currentLine) || (rc != currentCase))
            {
                int numPoints = (int)lineLODLevel.LODSegments + 1;
                int numContourPoints = 0;
                Model.Section.FrameSection section;
                Canguro.Model.StraightFrameProps sfProps;
                Vector2[][] contour;
                float[,] s1, m22, m33;

                sfProps = line.Properties as Canguro.Model.StraightFrameProps;
                if (sfProps != null)
                {
                    s1 = lsc.GetForcesDiagram(rc.AbstractCase, line, Canguro.Model.Load.LineForceComponent.Axial, numPoints);
                    m22 = lsc.GetForcesDiagram(rc.AbstractCase, line, Canguro.Model.Load.LineForceComponent.Moment22, numPoints);
                    m33 = lsc.GetForcesDiagram(rc.AbstractCase, line, Canguro.Model.Load.LineForceComponent.Moment33, numPoints);

                    section = sfProps.Section;
                    contour = section.Contour;
                    //numContourPoints = contour[0].GetLength(0);
                    short[] activeContour = section.ContourIndices[(int)lineLODLevel.LODContour];
                    numContourPoints = activeContour.Length;
                    
                    if (numContourPoints * numPoints > colors.GetLength(0))
                        colors = new int[numPoints * numContourPoints];

                    for (int j = 0; j < numPoints; j++)
                        for (int i = 0; i < numContourPoints; i++)
                            colors[j * numContourPoints + i] = ColorUtils.GetColorFromStress(srh.Largest, lsc.GetStressAtPoint(section, s1, m22, m33, j,
                                                                contour[0][activeContour[i]].X, contour[0][activeContour[i]].Y) * deformationScale);
                }

                currentLine = line;
                currentCase = rc;
            }

            return colors[index];
        }
    }
}
