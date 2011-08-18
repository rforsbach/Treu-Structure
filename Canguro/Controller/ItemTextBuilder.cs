using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;

using Canguro.Model;
using Canguro.View;
using Canguro.View.Renderer;

namespace Canguro.Controller
{
    public class ItemTextBuilder
    {
        public ItemTextBuilder()
        {
        }

        private GraphicViewManager graphicViewManager
        {
            get { return GraphicViewManager.Instance; }
        }

        private Model.Model model
        {
            get { return Model.Model.Instance; }
        }

        private Model.UnitSystem.UnitSystem units
        {
            get { return Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;}
        }

        #region Joint Texts
        private string getJointID(Joint j)
        {
            return Culture.Get("enterDataJoint") + ":" + j.Id.ToString();
        }

        private string getJointLayer(Joint j)
        {
            return j.Layer.Name;
        }

        private string getJointCoord(Joint j)
        {
            string distance = units.UnitName(Canguro.Model.UnitSystem.Units.Distance);
            return "\n(" + j.X + distance + ", " + j.Y + distance + ", " + j.Z + distance + ")";
        }

        private string getJointRestraints(Joint j)
        {
            if (j.DoF.IsRestrained)
                return "\n" + Culture.Get("jointDoFProp") + ": " + j.DoF.ToString();

            return string.Empty;
        }

        private string getJointConstraints(Joint j)
        {
            if (j.Constraint != null)
                return "\n" + Culture.Get("jointConstraintProp") + ": " + j.Constraint;

            return string.Empty;
        }

        private string getJointMasses(Joint j)
        {
            bool hasMass = false;
            string ret = string.Empty;

            for (int i=0; i<6; i++)
            {
                if (j.Masses[i] != 0)
                    hasMass = true;

                ret += j.Masses[i].ToString("F") + ", ";
            }

            if (hasMass)
                return ret.Substring(0, ret.Length - 2);
            else
                return string.Empty;
        }

        private string getJointDisplacements(Joint j)
        {
            string angleRad = "Rad";
            string distance = units.UnitName(Canguro.Model.UnitSystem.Units.Distance);

            float[,] deformations = model.Results.JointDisplacements;
            if (deformations != null)
                return "\nU1: " + units.FromInternational(deformations[j.Id, 0], Canguro.Model.UnitSystem.Units.Distance).ToString("G3") + distance + ", U2: " + units.FromInternational(deformations[j.Id, 1], Canguro.Model.UnitSystem.Units.Distance).ToString("G3") + distance + ", U3: " + units.FromInternational(deformations[j.Id, 2], Canguro.Model.UnitSystem.Units.Distance).ToString("G3") + distance +
                        "\nR1: " + deformations[j.Id, 3] + angleRad + ", R2: " + deformations[j.Id, 4] + angleRad + ", R3: " + deformations[j.Id, 5] + angleRad;
            else
                return "";
        }
        #endregion

        #region Line texts
        private string getLineID(LineElement l, float xPos)
        {
            return xPos.ToString("F") + " x " +  Culture.Get("enterDataLine") + ":" + l.Id.ToString() + " (I: " + l.I.Id + ", J: " + l.J.Id + ")";
        }

        private string getLineLayer(LineElement l)
        {
            return l.Layer.Name;
        }

        private string getLineLength(LineElement l)
        {
            string distance = units.UnitName(Canguro.Model.UnitSystem.Units.Distance);
            string angle = units.UnitName(Canguro.Model.UnitSystem.Units.Angle);

            string ret = "\n" + Culture.Get("lineLengthText") + l.Length.ToString("G3") + distance;
            if (l.Angle != 0)
                ret += ", " + Culture.Get("Angle") + ": " + l.Angle.ToString("G3") + angle;
            return ret;
        }

        private string getLineSection(LineElement l)
        {
            if (l.Properties is StraightFrameProps)
            {
                Model.Section.FrameSection sec = ((StraightFrameProps)l.Properties).Section;
                return "\n" + sec.Description;
            }

            return string.Empty;
        }

        private string getLineDeformedInfo(LineElement l, float xPos)
        {
            string angleRad = "Rad";
            string distance = units.UnitName(Canguro.Model.UnitSystem.Units.Distance);

            float[] deflection2;
            float[] deflection3;
            bool lastUndoEnabled = model.Undo.Enabled;
            bool lastUnitSystemEnabled = Model.UnitSystem.UnitSystemsManager.Instance.Enabled;

            try
            {
                if (model.IsLocked)
                    model.Undo.Enabled = false;

                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

                Analysis.LineDeformationCalculator calc = new Canguro.Analysis.LineDeformationCalculator();

                deflection2 = calc.GetCurvedPoint(l, model.Results.ActiveCase.AbstractCase, Canguro.Analysis.LineDeformationCalculator.DeformationAxis.Local2, xPos);
                deflection3 = calc.GetCurvedPoint(l, model.Results.ActiveCase.AbstractCase, Canguro.Analysis.LineDeformationCalculator.DeformationAxis.Local3, xPos);
            }
            finally
            {
                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = lastUnitSystemEnabled;
                model.Undo.Enabled = lastUndoEnabled;
            }

            return "\n U2: " + units.FromInternational(deflection2[1], Canguro.Model.UnitSystem.Units.Distance).ToString("G3") + distance + ", U3: " + units.FromInternational(deflection3[1], Canguro.Model.UnitSystem.Units.Distance).ToString("G3") + distance +
                   "\n R3: " + units.FromInternational(deflection2[2], Canguro.Model.UnitSystem.Units.Angle).ToString("G3") + angleRad + ", R2: " + units.FromInternational(deflection3[2], Canguro.Model.UnitSystem.Units.Angle).ToString("G3") + angleRad;
        }

        private string getLineForcesInfo(LineElement l, float xPos, RenderOptions.InternalForces iForces)
        {
            string force = units.UnitName(Canguro.Model.UnitSystem.Units.Force);
            string moment = units.UnitName(Canguro.Model.UnitSystem.Units.Moment);

            string unitStr = force;
            float[] diagram;
            string iForceName = string.Empty;
            int numPoints = Properties.Settings.Default.ElementForcesSegments;
            Model.Load.AbstractCase ac = model.Results.ActiveCase.AbstractCase;
            Model.Load.LineForceComponent component = Canguro.Model.Load.LineForceComponent.Axial;

            bool lastUndoEnabled = model.Undo.Enabled;
            bool lastUnitSystemEnabled = Model.UnitSystem.UnitSystemsManager.Instance.Enabled;

            try
            {
                if (model.IsLocked)
                    model.Undo.Enabled = false;

                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

                Analysis.LineStressCalculator calc = new Analysis.LineStressCalculator();

                // Shear forces
                switch (iForces)
                {
                    case RenderOptions.InternalForces.Sx:
                        component = Canguro.Model.Load.LineForceComponent.Axial;
                        iForceName = Culture.Get("Axial");
                        break;
                    case RenderOptions.InternalForces.Sy:
                        component = Canguro.Model.Load.LineForceComponent.Shear22;
                        iForceName = Culture.Get("Shear") + " 22";
                        break;
                    case RenderOptions.InternalForces.Sz:
                        component = Canguro.Model.Load.LineForceComponent.Shear33;
                        iForceName = Culture.Get("Shear") + " 33";
                        break;

                    // Moments
                    case RenderOptions.InternalForces.Mx:
                        component = Canguro.Model.Load.LineForceComponent.Torsion;
                        iForceName = Culture.Get("Torsion");
                        unitStr = moment;
                        break;
                    case RenderOptions.InternalForces.My:
                        component = Canguro.Model.Load.LineForceComponent.Moment22;
                        iForceName = Culture.Get("Moment") + " 22";
                        unitStr = moment;
                        break;
                    case RenderOptions.InternalForces.Mz:
                        component = Canguro.Model.Load.LineForceComponent.Moment33;
                        iForceName = Culture.Get("Moment") + " 33";
                        unitStr = moment;
                        break;
                }

                // Get Diagram
                diagram = calc.GetForceAtPoint(ac, l, component, xPos);
            }
            finally
            {
                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = lastUnitSystemEnabled;
                model.Undo.Enabled = lastUndoEnabled;
            }

            return "\n" + iForceName + ": " + units.FromInternational(diagram[1], Canguro.Model.UnitSystem.Units.Force).ToString("G3") + unitStr;
        }

        private string getLineDesignInfo(LineElement l, float xPos)
        {
            string smallDistance = units.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance);
            string area = units.UnitName(Canguro.Model.UnitSystem.Units.Area);
            string distance = units.UnitName(Canguro.Model.UnitSystem.Units.Distance);

            string ret;
            Model.Section.FrameSection sec = ((StraightFrameProps)l.Properties).Section;

            if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
            {
                ret = Culture.Get("CanguroViewReportsSteelDesignWrapper") + "\n\n";

                Model.Results.SteelDesignSummary designSum = model.Results.DesignSteelSummary[l.Id];
                Model.Results.SteelDesignPMMDetails designPMM = model.Results.DesignSteelPMMDetails[l.Id];
                Model.Results.SteelDesignShearDetails designShear = model.Results.DesignSteelShearDetails[l.Id];

                // Steel Design Summary
                ret = "\n";
                if (!string.IsNullOrEmpty(designSum.Status) && !designSum.Status.Equals("No Messages"))
                    ret += designSum.Status + "\n";
                if (!string.IsNullOrEmpty(designSum.ErrMsg))
                    ret += designSum.ErrMsg + "\n";
                if (!string.IsNullOrEmpty(designSum.WarnMsg))
                    ret += designSum.WarnMsg + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignWrapperRatio") + ": " + designSum.Ratio.ToString("F5");

                // Steel Design PMM Details
                ret += "\n\n" + Culture.Get("CanguroViewReportsSteelDesignPMMWrapper") + "\n\n";
                if (!string.IsNullOrEmpty(designPMM.Status) && !designPMM.Status.Equals("No Messages"))
                    ret += designPMM.Status + "\n";
                if (!string.IsNullOrEmpty(designPMM.ErrMsg))
                    ret += designPMM.ErrMsg + "\n";
                if (!string.IsNullOrEmpty(designPMM.WarnMsg))
                    ret += designPMM.WarnMsg + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignPMMWrapperTotalRatio") + ": " + designPMM.TotalRatio.ToString("F5") + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignPMMWrapperPRatio") + ": " + designPMM.PRatio.ToString("F5") + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignPMMWrapperMMajRatio") + ": " + designPMM.MMajRatio.ToString("F5") + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignPMMWrapperMMinRatio") + ": " + designPMM.MMinRatio.ToString("F5") + "\n";

                // Steel Design Shear Details
                ret += "\n\n" + Culture.Get("CanguroViewReportsSteelDesignShearWrapper") + "\n\n";
                if (!string.IsNullOrEmpty(designShear.Status) && !designShear.Status.Equals("No Messages"))
                    ret += designShear.Status + "\n";
                if (!string.IsNullOrEmpty(designShear.ErrMsg))
                    ret += designShear.ErrMsg + "\n";
                if (!string.IsNullOrEmpty(designShear.WarnMsg))
                    ret += designShear.WarnMsg + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignShearWrapperVMajorRatio") + ": " + designShear.VMajorRatio.ToString("F5") + "\n";
                ret += Culture.Get("CanguroViewReportsSteelDesignShearWrapperVMinorRatio") + ": " + designShear.VMinorRatio.ToString("F5");
            }
            else
            {
                ret = Culture.Get("CanguroViewReportsConcreteBeamDesignWrapper") + "\n\n";

                // Concrete Design
                if (sec.ConcreteProperties is Model.Section.ConcreteBeamSectionProps)
                {
                    Model.Results.ConcreteBeamDesign design = model.Results.DesignConcreteBeam[l.Id];

                    ret = "\n";
                    if (!string.IsNullOrEmpty(design.Status) && !design.Status.Equals("No Messages"))
                        ret += design.Status + "\n";
                    if (!string.IsNullOrEmpty(design.ErrMsg))
                        ret += design.ErrMsg + "\n";
                    if (!string.IsNullOrEmpty(design.WarnMsg))
                        ret += design.WarnMsg + "\n";
                    ret += Culture.Get("CanguroViewReportsConcreteBeamDesignWrapperBottomRebarArea") + ": " + units.FromInternational(design.FBotArea, Canguro.Model.UnitSystem.Units.Area).ToString("G3") + area + "\n";
                    ret += Culture.Get("CanguroViewReportsConcreteBeamDesignWrapperTopRebarArea") + ": " + units.FromInternational(design.FTopArea, Canguro.Model.UnitSystem.Units.Area).ToString("G3") + area + "\n";
                    ret += Culture.Get("CanguroViewReportsConcreteBeamDesignWrapperVRebar") + ": " + units.FromInternational(units.Convert(design.VRebar, Canguro.Model.UnitSystem.Units.Distance, Canguro.Model.UnitSystem.Units.SmallDistance), Canguro.Model.UnitSystem.Units.SmallDistance).ToString("G3") + smallDistance + "2/" + distance;

                }
                else if (sec.ConcreteProperties is Model.Section.ConcreteColumnSectionProps)
                {
                    Model.Results.ConcreteColumnDesign design = model.Results.DesignConcreteColumn[l.Id];

                    ret = "\n";
                    if (!string.IsNullOrEmpty(design.Status) && !design.Status.Equals("No Messages"))
                        ret += design.Status + "\n";
                    if (!string.IsNullOrEmpty(design.ErrMsg))
                        ret += design.ErrMsg + "\n";
                    if (!string.IsNullOrEmpty(design.WarnMsg))
                        ret += design.WarnMsg + "\n";

                    Model.Section.ConcreteColumnSectionProps cProps = (Model.Section.ConcreteColumnSectionProps)sec.ConcreteProperties;
                    float rebarArea = cProps.NumberOfBars * Model.Section.BarSizes.Instance.GetArea(cProps.BarSize);

                    ret += Culture.Get("CanguroViewReportsConcreteColumnDesignWrapperPMMArea") + ": " + units.FromInternational(design.PMMArea, Canguro.Model.UnitSystem.Units.Area).ToString("G3") + area + " (" + (rebarArea / design.PMMArea).ToString("F") + "%)\n";
                    ret += Culture.Get("CanguroViewReportsConcreteColumnDesignWrapperPMMRatio") + ": " + design.PMMRatio + "\n";
                    ret += Culture.Get("CanguroViewReportsConcreteColumnDesignWrapperVMajRebar") + ": " + units.FromInternational(units.Convert(design.VMajRebar, Canguro.Model.UnitSystem.Units.Distance, Canguro.Model.UnitSystem.Units.SmallDistance), Canguro.Model.UnitSystem.Units.SmallDistance).ToString("G3") + smallDistance + "^2/" + distance + "\n";
                    ret += Culture.Get("CanguroViewReportsConcreteColumnDesignWrapperVMinRebar") + ": " + units.FromInternational(units.Convert(design.VMinRebar, Canguro.Model.UnitSystem.Units.Distance, Canguro.Model.UnitSystem.Units.SmallDistance), Canguro.Model.UnitSystem.Units.SmallDistance).ToString("G3") + smallDistance + "^2/" + distance + "\n";
                }
            }

            return ret;
        }
        #endregion

        public string GetItemText(Item item, Vector3 position, float xPos)
        {
            StringBuilder sb = new StringBuilder();
            RenderOptions options = graphicViewManager.ActiveView.ModelRenderer.RenderOptions;

            if (model.HasResults && (model.Results.ActiveCase == null || model.Results.ActiveCase.AbstractCase == null))
                return string.Empty;

            if (item != null && !options.ShowAnimated)
            {
                if (!options.ShowDeformed && options.InternalForcesShown == RenderOptions.InternalForces.None && !options.ShowDesigned)
                {
                    #region Show just model info (No Results here)
                    if (item is Joint)
                    {
                        Joint j = item as Joint;
                        sb.Append(getJointID(j));
                        sb.Append(" " + Culture.Get("Layer") + ": " + getJointLayer(j));
                        sb.Append(getJointCoord(j));
                        sb.Append(getJointRestraints(j));
                        sb.Append(getJointConstraints(j));
                        sb.Append(getJointMasses(j));
                    }
                    else if (item is LineElement)
                    {
                        LineElement l = item as LineElement;
                        sb.Append(getLineID(l, xPos));
                        sb.Append(getLineLength(l));
                        sb.Append(" " + Culture.Get("Layer") + ": " + getLineLayer(l));
                        sb.Append(getLineSection(l));
                    }
                    #endregion
                }
                else if (options.ShowStressed) { }  // Don't do anything (yet)
                else if (options.ShowDesigned)
                {
                    #region Design
                    if (item is Joint)
                    {
                        Joint j = item as Joint;
                        sb.Append(getJointID(j));
                        sb.Append(getJointCoord(j));
                        // TODO: Show design results (at point)
                    }
                    else if (item is LineElement)
                    {
                        LineElement l = item as LineElement;
                        sb.Append(getLineID(l, xPos));
                        sb.Append(getLineDesignInfo(l, xPos));
                    }
                    #endregion
                }
                else if (options.ShowDeformed)
                {
                    #region Deformed
                    if (item is Joint)
                    {
                        Joint j = item as Joint;
                        sb.Append(getJointID(j));
                        sb.Append(getJointCoord(j));
                        sb.Append(getJointDisplacements(j));
                    }
                    else if (item is LineElement)
                    {
                        LineElement l = item as LineElement;
                        sb.Append(getLineID(l, xPos));
                        sb.Append(getLineDeformedInfo(l, xPos));
                    }
                    #endregion
                }
                else if (options.InternalForcesShown != RenderOptions.InternalForces.None)
                {
                    #region Internal Forces
                    if (item is Joint)
                    {
                        Joint j = item as Joint;
                        sb.Append(getJointID(j));
                        sb.Append(getJointCoord(j));
                    }
                    else if (item is LineElement)
                    {
                        LineElement l = item as LineElement;
                        sb.Append(getLineID(l, xPos));
                        sb.Append(getLineForcesInfo(l, xPos, options.InternalForcesShown));
                        // TODO: Show internal force value at point
                    }
                    #endregion
                }
            }

            return sb.ToString();
        }
    }
}
