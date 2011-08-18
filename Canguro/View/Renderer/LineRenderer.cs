using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.Model;
using Canguro.Model.Section;

using Canguro.View.Gadgets;
using Canguro.Analysis;

namespace Canguro.View.Renderer
{
    public abstract class LineRenderer : ItemRenderer
    {
        protected bool vertexColoringEnabled = false;
        protected int selectedColor = Properties.Settings.Default.SelectedDefaultColor.ToArgb();
        protected int selectedMaterialColor = Properties.Settings.Default.MaterialSelectedDefaultColor.ToArgb();
        protected Material pickMaterial = new Material();
        protected Material colorMaterial = new Material();
        protected int lastMaterialColor = 0;

        #region Private fields

        private Vector3 origin = Vector3.Empty;
        private Vector3 end = Vector3.Empty;
        private Vector3[] singleArrow = new Vector3[3];
        #endregion

        public abstract void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView);

        protected void drawReleaseIfNeeded(ResourceManager rc, LineElement l, RenderOptions options)
        {
            if ((options.OptionsShown & RenderOptions.ShowOptions.Releases) != 0)
                rc.GadgetManager.GadgetList.AddLast(new Gadget(l, GadgetType.Release));
        }

        public void DrawTexts(Device device, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            if (lines != null)
            {
                foreach (LineElement element in lines)
                {
                    if (element != null && element.IsSelected == true)
                        drawLineTexts(device, element, options);
                }
            }
            else if (itemsInView != null)
            {
                LineElement element;

                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    foreach (Item item in itemsInView)
                    {
                        if (item is LineElement)
                        {
                            element = (LineElement)item;

                            if (element != null && element.IsSelected == true)
                                drawLineTexts(device, element, options);
                        }
                    }
                }
            }
        }

        private void drawLineTexts(Device device, LineElement element, RenderOptions options)
        {
            string lineText = string.Empty;

            Vector3 position = element.I.Position + 0.5f * (element.J.Position - element.I.Position);

            if ((options.OptionsShown & RenderOptions.ShowOptions.LineIDs) != 0)
                lineText = lineText + Culture.Get("lineIDText") + element.Id.ToString() + "\n";

            if ((options.OptionsShown & RenderOptions.ShowOptions.LineLengths) != 0)
                lineText = lineText + Culture.Get("lineLengthText") + element.Length.ToString("f3") +
                           " [" + Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance) + "]" + "\n";

            if ((options.OptionsShown & RenderOptions.ShowOptions.LineSections) != 0)
            {
                FrameSection sec = ((StraightFrameProps)element.Properties).Section;
                lineText = lineText + Culture.Get("lineSecText") + sec.Description + "\n";
            }
            if ((options.OptionsShown & RenderOptions.ShowOptions.Strains) != 0)
            {
                LineDeformationCalculator calc = new LineDeformationCalculator();
                Canguro.Model.Results.ResultsCase resultsCase = Canguro.Model.Model.Instance.Results.ActiveCase;
                Model.Load.AbstractCase abstractCase = resultsCase.AbstractCase;

                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                string unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance);

                Vector3 vI = new Vector3(Canguro.Model.Model.Instance.UnitSystem.FromInternational(Canguro.Model.Model.Instance.Results.JointDisplacements[element.I.Id, 0], Canguro.Model.UnitSystem.Units.Distance),
                                         Canguro.Model.Model.Instance.UnitSystem.FromInternational(Canguro.Model.Model.Instance.Results.JointDisplacements[element.I.Id, 1], Canguro.Model.UnitSystem.Units.Distance),
                                         Canguro.Model.Model.Instance.UnitSystem.FromInternational(Canguro.Model.Model.Instance.Results.JointDisplacements[element.I.Id, 2], Canguro.Model.UnitSystem.Units.Distance));
                Vector3 vJ = new Vector3(Canguro.Model.Model.Instance.UnitSystem.FromInternational(Canguro.Model.Model.Instance.Results.JointDisplacements[element.J.Id, 0], Canguro.Model.UnitSystem.Units.Distance),
                                         Canguro.Model.Model.Instance.UnitSystem.FromInternational(Canguro.Model.Model.Instance.Results.JointDisplacements[element.J.Id, 1], Canguro.Model.UnitSystem.Units.Distance),
                                         Canguro.Model.Model.Instance.UnitSystem.FromInternational(Canguro.Model.Model.Instance.Results.JointDisplacements[element.J.Id, 2], Canguro.Model.UnitSystem.Units.Distance));

                //float[,] local2Values = calc.GetCurvedAxis(element, abstractCase, LineDeformationCalculator.DeformationAxis.Local2, 3);
                //float[,] local3Values = calc.GetCurvedAxis(element, abstractCase, LineDeformationCalculator.DeformationAxis.Local3, 3);

                lineText = lineText + "Strain at node I: " + "(" + vI.X.ToString("G4") + ", " + vI.Y.ToString("G4") + ", " + vI.Z.ToString("G4") + ") [" + unit + "]\n" +
                                      "Strain at node J: " + "(" + vJ.X.ToString("G4") + ", " + vJ.Y.ToString("G4") + ", " + vJ.Z.ToString("G4") + ") [" + unit + "]\n";

                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
            }

            if (lineText != string.Empty)
                drawSubstrings(device, lineText, position);
        }

        private void drawSubstrings(Device device, string text, Vector3 position)
        {
            DrawItemText(text, position, GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : Canguro.Properties.Settings.Default.TextColor);
        }

        private void beamLocalAxes(Device device, ResourceManager rc, LineElement element, float scale, ref PositionColoredPackage package, ref int lineVerts)
        {
            if (element != null && element.IsSelected == true)
            {
                Vector3 position = element.I.Position + scale * (element.J.Position - element.I.Position);
                Vector3 lineDir = element.J.Position - element.I.Position;
                Vector3[] local = element.LocalAxes;

                if (lineVerts + 6 >= package.NumVertices)
                {
                    rc.ReleaseBuffer(lineVerts, 0, ResourceStreamType.Lines);
                    package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);
                    lineVerts = 0;
                }

                lineVerts += 6;

                appendLine2VB(position, position + scale * local[0], System.Drawing.Color.Cyan.ToArgb(), ref package);
                DrawItemText("1", position + scale * local[0], Canguro.Properties.Settings.Default.TextColor);

                appendLine2VB(position, position + scale * local[1], System.Drawing.Color.Magenta.ToArgb(), ref package);
                DrawItemText("2", position + scale * local[1], Canguro.Properties.Settings.Default.TextColor);

                appendLine2VB(position, position + scale * local[2], System.Drawing.Color.Yellow.ToArgb(), ref package);
                DrawItemText("3", position + scale * local[2], Canguro.Properties.Settings.Default.TextColor);
            }
        }

        public void DrawLocalAxes(Device device, System.Collections.Generic.IEnumerable<LineElement> lines, List<Canguro.Model.Item> itemsInView)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            float scale = 0.25f;

            // Turn off lighting for color rendering
            device.RenderState.Lighting = false;

            rc.ActiveStream = ResourceStreamType.Lines;

            PositionColoredPackage package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);
            int lineVerts = 0;

            // Inicia código NO SEGURO
            if (itemsInView != null)
            {
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    LineElement element;

                    foreach (Item item in itemsInView)
                    {
                        element = item as LineElement;
                        beamLocalAxes(device, rc, element, scale, ref package, ref lineVerts);
                    }
                }
            }
            else
            {
                foreach (LineElement element in lines)
                    beamLocalAxes(device, rc, element, scale, ref package, ref lineVerts);
            }
            // Termina código NO SEGURO

            rc.ReleaseBuffer(lineVerts, 0, ResourceStreamType.Lines);
            rc.Flush(ResourceStreamType.Lines);

            //Turn on lighting
            device.RenderState.Lighting = true;
        }

        private void appendLine2VB(Vector3 start, Vector3 end, int color, ref PositionColoredPackage package)
        {
            unsafe
            {
                package.VBPointer->Position = start;
                package.VBPointer->Color = color;
                package.VBPointer++;

                package.VBPointer->Position = end;
                package.VBPointer->Color = color;
                package.VBPointer++;
            }
        }

        protected virtual int getLineColor(ResourceManager rc, LineElement l, bool pickingMode, RenderOptions.LineColorBy colorBy)
        {
            if (pickingMode)
                return rc.GetNextPickIndex(l);
            else
            {
                if (l.IsSelected == false && l.Properties is StraightFrameProps)
                {
                    if (colorBy == RenderOptions.LineColorBy.Material)
                    {
                        FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                        if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                            return System.Drawing.Color.FromArgb(255, 75, 105, 185).ToArgb();
                        else
                            return System.Drawing.Color.FromArgb(255, 135, 135, 129).ToArgb();
                    }
                    else if (colorBy == RenderOptions.LineColorBy.Layer)
                    {
                        return Utility.ColorUtils.GetColorForId((int)l.Layer.Id);
                    }
                    else if (colorBy == RenderOptions.LineColorBy.Section)
                    {
                        FrameSection sec = ((StraightFrameProps)l.Properties).Section;
                        return Utility.ColorUtils.GetColorForId(sec.Name.GetHashCode() + (int)(sec.Area * 10000));
                    }
                    else if (colorBy == RenderOptions.LineColorBy.Constraint)
                    {
                        if (l.J.Constraint == null || l.J.Constraint != l.I.Constraint) 
                            return 0x00606060;
                        return Utility.ColorUtils.GetColorForId(l.I.Constraint.GetHashCode() & 0xFF);
                    }
                    else if (colorBy == RenderOptions.LineColorBy.NonDefaultPropertyAssigned)
                    {
                        ItemList<Model.Load.Load> loads = l.Loads[Model.Model.Instance.ActiveLoadCase];
                        if (l.Angle != 0 || !l.DoFI.IsRestrained || !l.DoFJ.IsRestrained || (loads != null && loads.Count > 0))
                            return Utility.ColorUtils.GetColorForId(1);
                        else
                            return Utility.ColorUtils.GetColorForId(0);
                    }
                    else // RenderOptions.LineColorBy.Design
                    {
                        float ratio = 1f;
                        StraightFrameProps props = l.Properties as StraightFrameProps;

                        if (props != null && Canguro.Model.Model.Instance.HasResults)
                        {
                            FrameSection sec = props.Section;

                            if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                            {
                                // Steel Design
                                if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignSteelSummary[l.Id].ErrMsg))
                                    ratio = Canguro.Model.Model.Instance.Results.DesignSteelSummary[l.Id].Ratio;
                                else
                                    ratio = 1f;
                            }
                            else
                            {
                                // Concrete Design
                                if (sec.ConcreteProperties is ConcreteBeamSectionProps)
                                {
                                    if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignConcreteBeam[l.Id].ErrMsg))
                                        ratio = 0.5f;
                                    else
                                        ratio = 1f;
                                }
                                else if (sec.ConcreteProperties is ConcreteColumnSectionProps)
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
                                else
                                    ratio = 1f;
                            }
                        }

                        return Utility.ColorUtils.GetColorFromDesignRatio(ratio);
                    }
                }
                else
                    return selectedColor;
            }
        }

        protected virtual void setLineColor(Device device, ResourceManager rc, LineElement l, FrameSection sec, bool pickingMode, RenderOptions.LineColorBy colorBy)
        {
            // Select material
            if (pickingMode)
            {
                pickMaterial.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                device.Material = pickMaterial;
            }
            else
            {
                int color;
                                
                if (l.IsSelected)
                {
                    color = selectedMaterialColor;                    
                    colorMaterial.Emissive = System.Drawing.Color.FromArgb(color);
                }
                else
                {
                    colorMaterial.Emissive = System.Drawing.Color.Black;
                    color = getLineColor(rc, l, pickingMode, colorBy);

                    //if (colorBy == RenderOptions.LineColorBy.Material)
                    //{
                    //    if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                    //        color = System.Drawing.Color.FromArgb(255, 75, 105, 185).ToArgb();
                    //    else
                    //        color = System.Drawing.Color.FromArgb(255, 135, 135, 129).ToArgb();
                    //}
                    //else if (colorBy == RenderOptions.LineColorBy.Layer)
                    //    color = Utility.ColorUtils.GetColorForId((int)l.Layer.Id, l.IsSelected);
                    //else if (colorBy == RenderOptions.LineColorBy.Section)
                    //    color = Utility.ColorUtils.GetColorForId(sec.Name.GetHashCode() + (int)(sec.Area * 10000), l.IsSelected);
                    //else   // RenderOptions.LineColorBy.NonDefaultPropertyAssigned
                    //{
                    //    ItemList<Model.Load.Load> loads = l.Loads[Model.Model.Instance.ActiveLoadCase];
                    //    if (l.Angle != 0 || !l.DoFI.IsRestrained || !l.DoFJ.IsRestrained || (loads != null && loads.Count > 0))
                    //        color = Utility.ColorUtils.GetColorForId(1, l.IsSelected);
                    //    else
                    //        color = Utility.ColorUtils.GetColorForId(0, l.IsSelected);
                    //}
                }

                if (color != lastMaterialColor)
                {
                    colorMaterial.Diffuse = System.Drawing.Color.FromArgb(color);
                    colorMaterial.Ambient = System.Drawing.Color.FromArgb(color);

                    device.Material = colorMaterial;
                    lastMaterialColor = color;
                }
            }
        }
    }
}
