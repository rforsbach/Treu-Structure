using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.View;
using Canguro.Utility;

namespace Canguro.Controller.Tracking
{
    public class HoverController : IDisposable
    {
        public const int TooltipDelay = 250;

        // General fields
        Item hoverItem;
        Vector3 hoverPos;
        bool showingTooltip;
        string tooltipText;
        bool enabled;

        // LineElement fields
        float hoverXPos;
        Vector3[] curve;

        // Delay to show/hide tooltip
        System.Threading.Timer timer;
        System.Threading.TimerCallback timerDelegate;

        // tooltip rectangle
        Rectangle tooltipRectangle;
        Size deltaRect = new Size(15, -15);

        // Helpers
        RectangleHelper rectHelper;
        HoverPainter hoverPainter;

        public HoverController()
        {
            enabled = true;
            hoverItem = null;
            showingTooltip = false;
            rectHelper = new RectangleHelper(Color.FromArgb(120, 192, 192, 192), Color.White);
            hoverPainter = new HoverPainter();

            timerDelegate = new System.Threading.TimerCallback(timerCallback);
            timer = new System.Threading.Timer(timerDelegate, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        private ItemTextBuilder itemTextBuilder
        {
            get { return Controller.Instance.ItemTextBuilder; }
        }

        void timerCallback(object stateInfo)
        {
            showingTooltip = !showingTooltip;

            Controller controller = Controller.Instance;
            GraphicViewManager gvm = GraphicViewManager.Instance;
            //gvm.Presenter.TrackingPaintImmediately(this, null, gvm.ActiveView, controller.TrackingController);
            gvm.Presenter.TrackingPaint(this, null, gvm.ActiveView, controller.TrackingController);
        }

        public bool IsActive
        {
            get { return enabled; }
        }

        public bool ShowingTooltip
        {
            get { return showingTooltip; }
        }

        internal void Reset(GraphicView graphicView)
        {
        }

        private Item pickItem(System.Windows.Forms.MouseEventArgs e)
        {
            //////////////////////////////////////////////////
            List<Item> pickedItems = Canguro.View.GraphicViewManager.Instance.PickItem(e.X, e.Y);
            if (pickedItems == null || pickedItems.Count < 1)
                return null;
            else
                return pickedItems[0];
        }

        internal bool MouseMove(GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            bool needPaint = false;
            Item newHover = pickItem(e);
            if (hoverItem != newHover)
            {
                if (!showingTooltip)
                {
                    if (newHover != null)
                        timer.Change(TooltipDelay, System.Threading.Timeout.Infinite);
                    else
                        timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
                else
                {
                    if (newHover == null)
                        timer.Change(TooltipDelay, System.Threading.Timeout.Infinite);
                    else
                        timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            }

            if (hoverItem != null || hoverItem != newHover)
                needPaint = true;

            hoverItem = newHover;

            Model.Model model = Model.Model.Instance;
            View.Renderer.RenderOptions options = activeView.ModelRenderer.RenderOptions;

            // Get position in the item's local coordinate system
            if (hoverItem is Joint)
                hoverPos = getHoverPos((Joint)hoverItem, model, options);
            else if (hoverItem is LineElement)
            {
                LineElement line = hoverItem as LineElement;

                if (options.ShowDeformed)
                    hoverPos = getHoverPos(line, activeView, e.Location, model, options);
                else
                {
                    Snap.LineMagnet lm = new Canguro.Controller.Snap.LineMagnet(line);
                    lm.Snap(activeView, e.Location);
                    hoverPos = lm.SnapPositionInt;
                    hoverXPos = (hoverPos - line.I.Position).Length() / line.LengthInt;
                }
            }

            // Get text to draw
            tooltipText = itemTextBuilder.GetItemText(hoverItem, hoverPos, hoverXPos);

            // Get Size of text
            if (!string.IsNullOrEmpty(tooltipText))
            {
                tooltipRectangle = hoverPainter.MeasureText(tooltipText);
#if DEBUG
                if (tooltipRectangle.Width == 0 || tooltipRectangle.Height == 0)
                    throw new Exception("Measure String returned 0");
#endif
                tooltipRectangle.Location += deltaRect;
                tooltipRectangle.X += e.X;
                tooltipRectangle.Y += e.Y;
                tooltipRectangle.Width += 8;
                tooltipRectangle.Height += 4;

                Viewport vp = activeView.Viewport;
                if (tooltipRectangle.Right > (vp.X + vp.Width))
                    tooltipRectangle.X -= (tooltipRectangle.Width + 2 * deltaRect.Width);

                rectHelper.MouseMove(activeView, tooltipRectangle.Location, tooltipRectangle.Location + tooltipRectangle.Size);
            }
            else if (!showingTooltip)
                timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            
            return showingTooltip || needPaint;
        }

        private Vector3 getHoverPos(LineElement line, GraphicView activeView, Point location, Model.Model model, View.Renderer.RenderOptions options)
        {
            Vector3 pos = Vector3.Empty;
            bool lastUndoEnabled = model.Undo.Enabled;
            bool lastUnitSystemEnabled = Model.UnitSystem.UnitSystemsManager.Instance.Enabled;

            if (!model.HasResults || model.Results.ActiveCase == null)
                return pos;

            try
            {
                if (model.IsLocked)
                    model.Undo.Enabled = false;

                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

                int numPoints = (int)options.LOD.GetLOD(line).LODSegments + 1;

                float[] xPos;
                Analysis.LineDeformationCalculator calc = new Analysis.LineDeformationCalculator();                
                curve = calc.GetCurve(line, model.Results.ActiveCase.AbstractCase, numPoints, options.DeformationScale, model.Results.PaintScaleFactorTranslation, out xPos);

                int iCurve = 0, numLines = curve.Length - 1;
                Snap.LineMagnet minLine = null, lm;
                float minLineDist = float.MaxValue, lineDist;

                for (int i = 0; i < numLines; i++)
                {
                    lm = new Canguro.Controller.Snap.LineMagnet(curve[i], curve[i + 1] - curve[i], Canguro.Controller.Snap.LineMagnetType.FollowProjection);
                    if ((lineDist = lm.Snap(activeView, location)) < minLineDist)
                    {
                        if ((lm.SnapPositionInt - curve[i]).LengthSq() <= (curve[i + 1] - curve[i]).LengthSq())
                        {
                            minLineDist = lineDist;
                            minLine = lm;
                            iCurve = i;
                        }
                    }
                }

                if (minLine != null)
                {
                    pos = minLine.SnapPositionInt;
                    lm = new Canguro.Controller.Snap.LineMagnet(curve[iCurve], curve[iCurve + 1] - curve[iCurve], Canguro.Controller.Snap.LineMagnetType.FollowProjection);

                    Vector3 tmp = pos - curve[0];
                    Vector3 xPosUnit = curve[numLines] - curve[0];
                    xPosUnit = Vector3.Scale(xPosUnit, 1.0f / xPosUnit.LengthSq());
                    hoverXPos = Vector3.Dot(tmp, xPosUnit);
                }
                else
                    hoverItem = null;
            }
            finally
            {
                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = lastUnitSystemEnabled;
                model.Undo.Enabled = lastUndoEnabled;
            }

            return pos;
        }
        
        private Vector3 getHoverPos(Joint j, Model.Model model, View.Renderer.RenderOptions options)
        {
            if (options.ShowDeformed && model.HasResults && model.Results.ActiveCase != null)
            {
                float[,] deformations = model.Results.JointDisplacements;
                if (deformations != null)
                {
                    // Get joint defomations
                    Vector3 vI = new Vector3(deformations[j.Id, 0],
                                     deformations[j.Id, 1],
                                     deformations[j.Id, 2]);

                    vI = options.DeformationScale * model.Results.PaintScaleFactorTranslation * vI + j.Position;

                    return vI;
                }
            }
            return j.Position;
        }

        internal void Paint(Device device)
        {
            if (hoverItem != null)
            {
                // Glow item
                if (hoverItem is LineElement)
                {
                    LineElement line = hoverItem as LineElement;

                    Model.Model model = Model.Model.Instance;
                    GraphicView activeView = GraphicViewManager.Instance.ActiveView;
                    View.Renderer.RenderOptions options = activeView.ModelRenderer.RenderOptions;

                    if (options.ShowDeformed && model.HasResults && curve != null)
                        for (int i = 0; i < curve.Length - 1; i++)
                            hoverPainter.PaintLine(device, curve[i], curve[i + 1]);
                    else
                        hoverPainter.PaintLine(device, line);
                }

                // Show hoverPos
                Vector3 hoverPos2D = hoverPos;
                GraphicViewManager.Instance.ActiveView.Project(ref hoverPos2D);
                hoverPainter.PaintPoint(device, hoverPos2D);

                if (showingTooltip && !string.IsNullOrEmpty(tooltipText))
                {
                    // Draw tooltip rectangle
                    rectHelper.Paint(device);

                    // Draw text
                    Rectangle rect = tooltipRectangle;
                    rect.X += 4;
                    rect.Y += 2;
                    hoverPainter.DrawText(tooltipText, rect, GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : Color.White);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            timer.Dispose();
        }

        #endregion
    }
}
