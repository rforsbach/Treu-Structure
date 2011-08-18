using System;
using System.Drawing;

using Microsoft.DirectX;
using Canguro.Model;

namespace Canguro.Analysis
{
    public class StressHelper
    {
        private float maxStress = 0f;
        private float minStress = 0f;
        private float largest = 0f;
        private bool isDirty = true;

        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        public float Largest
        {
            get { return largest; }
            set { largest = value; }
        }

        public float getMaxStress(Model.Model model)
        {
            return model.UnitSystem.FromInternational(maxStress, Canguro.Model.UnitSystem.Units.Stress);
        }

        public float getMinStress(Model.Model model)
        {
            return model.UnitSystem.FromInternational(minStress, Canguro.Model.UnitSystem.Units.Stress);
        }

        public void Reset(Canguro.Model.Model model)
        {
            if (isDirty)
            {
                maxStress = 0f;
                minStress = 0f;
                largest = 0f;

                if (recalculateMinMaxStressesOfSelection(model))
                    largest = Math.Max(Math.Abs(maxStress), Math.Abs(minStress));

                isDirty = false;
            }
        }

        private bool recalculateMinMaxStressesOfSelection(Canguro.Model.Model model)
        {
            if (!model.HasResults || model.Results.ActiveCase == null) return false;

            Model.Load.AbstractCase ac = model.Results.ActiveCase.AbstractCase;
            int numPoints = Properties.Settings.Default.ElementForcesSegments;

            LineStressCalculator lsc = new LineStressCalculator();
            Model.Section.FrameSection section;
            StraightFrameProps sfProps;
            Vector2[][] contour;
            float stress;
            float[,] s1, m22, m33;

            foreach (LineElement line in model.LineList)
                if (line != null && line.IsVisible)
                {
                    sfProps = line.Properties as StraightFrameProps;
                    if (sfProps != null)
                    {
                        s1  = lsc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Axial, numPoints);
                        m22 = lsc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Moment22, numPoints);
                        m33 = lsc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Moment33, numPoints);

                        section = sfProps.Section;
                        contour = section.Contour;
                        for (int j = 0; j < numPoints; j++)
                            for (int i = 0; i < contour[0].Length; i++)
                            {
                                stress = lsc.GetStressAtPoint(section, s1, m22, m33, j, contour[0][i].X, contour[0][i].Y);
                                if (stress > maxStress) maxStress = stress;
                                if (stress < minStress) minStress = stress;
                            }
                    }
                }
            return true;
        }
    }
}
