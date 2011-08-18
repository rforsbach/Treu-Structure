using System;
using System.Collections.Generic;
using System.Text;

using Canguro.Model;
using Canguro.Model.Section;

using Canguro.Utility;

namespace Canguro.View.Renderer
{
    class DesignWireframeLineRenderer : WireframeLineRenderer
    {
        protected override int getLineColor(ResourceManager rc, Canguro.Model.LineElement l, bool pickingMode, RenderOptions.LineColorBy colorBy)
        {
            int lineColor = System.Drawing.Color.White.ToArgb(); 

            // Select material
            if (pickingMode)
                lineColor = rc.GetNextPickIndex(l);
            else
            {
                FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                {
                    // Steel Design
                    float ratio;
                    if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignSteelSummary[l.Id].ErrMsg))
                        ratio = Canguro.Model.Model.Instance.Results.DesignSteelSummary[l.Id].Ratio;
                    else
                        ratio = 1f;

                    lineColor = ColorUtils.GetColorFromDesignRatio(ratio);
                }
                else
                {
                    // Concrete Design
                    float ratio = 1f;
                    StraightFrameProps props = l.Properties as StraightFrameProps;
                    if (props != null)
                    {
                        if (props.Section.ConcreteProperties is ConcreteBeamSectionProps)
                        {
                            if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignConcreteBeam[l.Id].ErrMsg))
                                ratio = 0.5f;
                            else
                                ratio = 1f;
                        }
                        else if (props.Section.ConcreteProperties is ConcreteColumnSectionProps)
                        {
                            if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignConcreteColumn[l.Id].ErrMsg))
                            {
                                float rebarArea = 0f;
                                ConcreteColumnSectionProps cProps = (ConcreteColumnSectionProps)props.Section.ConcreteProperties;
                                rebarArea = cProps.NumberOfBars * BarSizes.Instance.GetArea(cProps.BarSize);
                                ratio = 1.5f - rebarArea / Canguro.Model.Model.Instance.Results.DesignConcreteColumn[l.Id].PMMArea;
                                if (ratio > 1f) ratio = 0.99f;
                            }
                            else
                                ratio = 1f;
                        }
                    }
                    lineColor = ColorUtils.GetColorFromDesignRatio(ratio);
                }
            }
            return lineColor;
        }
    }
}
