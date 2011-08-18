using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Load;
using Canguro.Utility;

namespace Canguro.Analysis
{
    class LineStressCalculator : ModelCalculator
    {
        private DistributedSpanLoad selfWeight;
        public const int NumControlPoints = 11;

        public LineStressCalculator()
        {
            selfWeight = new DistributedSpanLoad();
            selfWeight.Direction = LineLoad.LoadDirection.Gravity;
            selfWeight.Da = 0f;
            selfWeight.Db = 1f;
        }

        public void getForcesDiagram(AbstractCase ac, LineElement line, LineForceComponent component, float[,] controlPoints, out float fI, out float fJ)
        {
            Vector3 vI, vJ;
            switch (component)
            {
                case LineForceComponent.Axial:
                case LineForceComponent.Shear22:
                case LineForceComponent.Shear33:
                    vI = new Vector3(model.Results.ElementJointForces[line.Id, 0, 0],
                                     model.Results.ElementJointForces[line.Id, 0, 1],
                                     model.Results.ElementJointForces[line.Id, 0, 2]);
                    vJ = new Vector3(model.Results.ElementJointForces[line.Id, 1, 0],
                                     model.Results.ElementJointForces[line.Id, 1, 1],
                                     model.Results.ElementJointForces[line.Id, 1, 2]);
                    break;
                default:
                    vI = new Vector3(model.Results.ElementJointForces[line.Id, 0, 3],
                                     model.Results.ElementJointForces[line.Id, 0, 4],
                                     model.Results.ElementJointForces[line.Id, 0, 5]);
                    vJ = new Vector3(model.Results.ElementJointForces[line.Id, 1, 3],
                                     model.Results.ElementJointForces[line.Id, 1, 4],
                                     model.Results.ElementJointForces[line.Id, 1, 5]);
                    break;
            }

            // Transform to the line's local coordinate system
            // From the global system 
            // (WARNING: This is supposed to come in the joint's local coordinate system)
            vI = toLocal(line, vI);
            vJ = toLocal(line, vJ);

            switch (component)
            {
                case LineForceComponent.Axial:
                case LineForceComponent.Torsion:
                    fI = vI.X;
                    fJ = vJ.X;
                    break;
                case LineForceComponent.Shear22:
                case LineForceComponent.Moment22:
                    fI = vI.Y;
                    fJ = vJ.Y;
                    break;
                default:
                    fI = vI.Z;
                    fJ = vJ.Z;
                    break;
            }

            //float fI = model.Results.ElementJointForces[line.Id, 0, (int)component];
            //float fJ = model.Results.ElementJointForces[line.Id, 1, (int)component];


            // Add Isostatic curves if there's a load on the LineElement
            addToShearMomentDiagram(ac, line, controlPoints, component, 1f);
        }

        public float[] GetForceAtPoint(AbstractCase ac, LineElement line, LineForceComponent component, float xPos)
        {
            if (!model.HasResults) return null;

            float[,] controlPoints = new float[1, 2];
            controlPoints[0, 0] = xPos;
            controlPoints[0, 1] = 0f;

            float fI, fJ;
            getForcesDiagram(ac, line, component, controlPoints, out fI, out fJ);

            // Los valores en los nodos se encuentran volteados. Del lado izquierdo positivo
            // es arriba, mientras del lado derecho positivo es abajo, por lo que si se tienen
            // dos valores positivos se debería pintar una recta con pendiente negativa y si
            // se tienen dos negativos al revés. DeltaY = Y1 + Y2 (por el cambio de signo).
            controlPoints[0, 1] += controlPoints[0, 0] * (fI + fJ) - fI;

            // Copy to 1-dimensional array
            float[] ret = new float[2];
            for (int i = 0; i < 2; i++)
                ret[i] = controlPoints[0, i];

            return ret;
        }

        public float[,] GetForcesDiagram(AbstractCase ac, LineElement line, LineForceComponent component, int numPoints)
        {
            if (!model.HasResults) return null;

            float[,] controlPoints = new float[numPoints, 2];

            float[] controlPointsX = new float[1]; // requestXCtrlPts(load);
            for (int i = 0, bufi = 0; i < numPoints; i++)
            {
                controlPoints[i,0] = i / (numPoints - 1f);

                // Adjust control points
                if ((bufi < controlPointsX.Length) && (controlPointsX[bufi] <= controlPoints[i, 0]))
                    controlPoints[i, 0] = controlPointsX[bufi++];

                controlPoints[i, 1] = 0f; // controlPoints[i, 0] * (fdJ - fdI);
            }

            float fI, fJ;
            getForcesDiagram(ac, line, component, controlPoints, out fI, out fJ);

            for (int i = 0, bufi = 0; i < numPoints; i++)
            {
                // Los valores en los nodos se encuentran volteados. Del lado izquierdo positivo
                // es arriba, mientras del lado derecho positivo es abajo, por lo que si se tienen
                // dos valores positivos se debería pintar una recta con pendiente negativa y si
                // se tienen dos negativos al revés. DeltaY = Y1 + Y2 (por el cambio de signo).
                controlPoints[i, 1] += controlPoints[i, 0] * (fI + fJ) - fI;
            }

            return controlPoints;
        }

        /// <summary>
        /// Gets the stress at the point specified by (pt22, pt33) in the Line's contour local axes, given the internal
        /// forces diagrams for Axial and Moments in 2 and 3.
        /// </summary>
        /// <param name="line">The line to calculate the stress to</param>
        /// <param name="s1">The axial internal forces diagram for the Line</param>
        /// <param name="m22">The Moment in 2 internal forces diagram for the Line</param>
        /// <param name="m33">The Moment in 3 internal forces diagram for the Line</param>
        /// <param name="pointIndex">The index of the diagram to use at the moment, 
        /// corresponds to the position in the Local-1 Axis</param>
        /// <param name="pt22">The point in the Local-2 Axis, corresponds to contour [.][1]</param>
        /// <param name="pt33">The point in the Local-2 Axis, corresponds to contour [.][0]</param>
        /// <returns>The stress at the specified point.</returns>
        public float GetStressAtPoint(Model.Section.FrameSection section, float[,] s1, float[,] m22, float[,] m33, int pointIndex, float pt22, float pt33)
        {
            return (s1[pointIndex, 1] / section.Area) + (m22[pointIndex, 1] * pt22 / section.I22) - (m33[pointIndex, 1] * pt33 / section.I33);
        }

        private float[] requestXCtrlPts(LineLoad load)
        {
            float[] controlPoints = new float[2];

            if (load is ConcentratedSpanLoad)
            {
                controlPoints[0] = ((ConcentratedSpanLoad)load).D - 0.00001f;
                controlPoints[1] = controlPoints[0] + 0.00002f;
            }
            else if (load is DistributedSpanLoad)
            {
                controlPoints[0] = ((DistributedSpanLoad)load).Da;
                controlPoints[1] = ((DistributedSpanLoad)load).Db;
            }

            return controlPoints;
        }

        #region Shear and Moments calculation
        private void addToShearMomentDiagram(AbstractCase ac, LineElement line, float[,] controlPoints, LineForceComponent component, float scale)
        {
            if (ac == null) return;

            if (ac is LoadCombination)
            {
                foreach (AbstractCaseFactor acf in ((LoadCombination)ac).Cases)
                    addToShearMomentDiagram(acf.Case, line, controlPoints, component, acf.Factor);
            }
            else if (ac is AnalysisCase)
            {
                if (((AnalysisCase)ac).Properties is StaticCaseProps)
                {
                    foreach (StaticCaseFactor staticCase in ((StaticCaseProps)((AnalysisCase)ac).Properties).Loads)
                    {
                        if (staticCase.AppliedLoad is LoadCase)
                        {
                            LoadCase lc = staticCase.AppliedLoad as LoadCase;
                            if (line.Loads[lc] != null)
                            {
                                foreach (Load load in line.Loads[lc])
                                    if (load is DirectionalLineLoad)
                                        addToShearMomentDiagram(line, load as DirectionalLineLoad, controlPoints, component, scale);
                            }

                            if (lc.SelfWeight > 0f)
                            {
                                if (line.Properties is StraightFrameProps)
                                {
                                    StraightFrameProps frameProps = line.Properties as StraightFrameProps;
                                    selfWeight.La = selfWeight.Lb = frameProps.Section.Area * frameProps.Section.Material.UnitWeight;
                                    addToShearMomentDiagram(line, selfWeight, controlPoints, component, lc.SelfWeight * scale);
                                }
                            }                            
                        }
                    }
                }
            }
        }

        private void addToShearMomentDiagram(LineElement line, DirectionalLineLoad load, float[,] controlPoints, LineForceComponent component, float scale)
        {
            if (load == null) return;

            float dirComponent = 0f, position;

            // Get Load direction in Local Coordinate frame
            Vector3 dir = getLocalDir(line, load.Direction);

            switch (component)
            {
                case LineForceComponent.Axial:
                case LineForceComponent.Torsion:
                    dirComponent = dir.X;
                    break;
                case LineForceComponent.Shear22:
                case LineForceComponent.Moment33:
                    dirComponent = dir.Y;
                    break;
                case LineForceComponent.Shear33:
                case LineForceComponent.Moment22:
                    dirComponent = dir.Z;
                    break;
            }

            int i;
            switch (component)
            {
                case LineForceComponent.Shear22:
                case LineForceComponent.Shear33:
                    for (i = 0; i < controlPoints.GetLength(0); i++)
                    {
                        position = controlPoints[i,0] * line.Length;
                        controlPoints[i, 1] += addToShearDiagram(load, dirComponent, position, line.Length) * scale;
                    }
                    break;
                case LineForceComponent.Moment33:
                    dirComponent = -dirComponent;
                    for (i = 0; i < controlPoints.GetLength(0); i++)
                    {
                        position = controlPoints[i,0] * line.Length;
                        controlPoints[i, 1] += addToMomentDiagram(load, dirComponent, position, line.Length) * scale;
                    }
                    break;
                case LineForceComponent.Moment22:
                    for (i = 0; i < controlPoints.GetLength(0); i++)
                    {
                        position = controlPoints[i,0] * line.Length;
                        controlPoints[i, 1] += addToMomentDiagram(load, dirComponent, position, line.Length) * scale;
                    }
                    break;
                default:
                    break;
            }
        }

        private float addToMomentDiagram(LineLoad load, float dirComponent, float position, float lineLength)
        {
            if (load is ConcentratedSpanLoad)
            {
                float P = ((ConcentratedSpanLoad)load).L;
                float pDist = ((ConcentratedSpanLoad)load).D;

                if (position < (pDist * lineLength))
                    return P * dirComponent * (1f - pDist) * position;
                else
                    return P * dirComponent * pDist * (lineLength - position);
            }
            else if (load is DistributedSpanLoad)
            {
                DistributedSpanLoad dLoad = load as DistributedSpanLoad;

                //if (position >= 0)
                //    return -(dLoad.La * dirComponent * position / 2f) * (lineLength - position);

                float a, b, c, Ra, W, xt;
                a = dLoad.Da * lineLength;
                b = dLoad.Db * lineLength - a;
                c = lineLength - a - b;

                W = (dLoad.La + dLoad.Lb) * b / 2f;
                xt = Math.Abs(dLoad.La + dLoad.Lb) > float.Epsilon ? b * (dLoad.La + 2f * dLoad.Lb) / (3 * (dLoad.La + dLoad.Lb)) : 0.0f;
                Ra = W / lineLength * (lineLength - (a + xt));

                if (b == 0f)
                    return 0f;
                else if (position < a)
                {
                    return Ra * position * dirComponent;
                }
                else if (position <= (a + b))
                {
                    float Wx, y, xt2, y1;
                    y1 = (dLoad.Lb - dLoad.La) * (position - a) / b;
                    y = dLoad.La + y1;
                    xt2 = Math.Abs(dLoad.La + y) > float.Epsilon ? (position - a) / 3f * (dLoad.La + 2 * y) / (dLoad.La + y) : 0.0f;
                    Wx = (dLoad.La + y) * (position - a) / 2f;

                    return (Ra * position - Wx * (position - a - xt2)) * dirComponent;
                }
                else
                {
                    return (Ra * position - W * (position - a - xt)) * dirComponent;
                }
            }

            return 0f;
        }

        private float addToShearDiagram(LineLoad load, float dirComponent, float position, float lineLength)
        {
            if (load is ConcentratedSpanLoad)
            {
                float P = ((ConcentratedSpanLoad)load).L;
                float pDist = ((ConcentratedSpanLoad)load).D;

                if (position < (pDist * lineLength))
                    return (P * dirComponent) / lineLength * position;
                else
                    return -(P * dirComponent) / lineLength * (lineLength - position);
            }
            else if (load is DistributedSpanLoad)
            {
                DistributedSpanLoad dLoad = load as DistributedSpanLoad;

                //return 0f;

                float a, b, c, Ra, W, xt;
                a = dLoad.Da * lineLength;
                b = dLoad.Db * lineLength - a;
                c = lineLength - a - b;

                if (b == 0)
                    return 0f;
                else if (position >= a && position <= (a + b))
                {
                    W = (dLoad.La + dLoad.Lb) * b / 2f;
                    xt = Math.Abs(dLoad.La + dLoad.Lb) > float.Epsilon ? b * (dLoad.La + 2f * dLoad.Lb) / (3 * (dLoad.La + dLoad.Lb)) : 0.0f;
                    Ra = W / lineLength * (lineLength - (a + xt));

                    /////////////////////////////////////////
                    // Shear in a
                    float shear_a = Ra;
                    /////////////////////////////////////////

                    /////////////////////////////////////////
                    // Shear in a + b
                    float Wx_b, shear_b;
                    Wx_b = (dLoad.La + dLoad.La + dLoad.Lb - dLoad.La) * b / 2f;
                    shear_b = Ra - Wx_b;
                    /////////////////////////////////////////

                    /////////////////////////////////////////
                    // Line between a --- b
                    float originalLine = (position - a) / b * ((shear_b - shear_a)) + shear_a;
                    /////////////////////////////////////////
                    
                    float Wx, y, xt2, y1;
                    y1 = (dLoad.Lb - dLoad.La) * (position - a) / b;
                    y = dLoad.La + y1;
                    xt2 = Math.Abs(dLoad.La + y) > float.Epsilon ? (position - a) / 3f * (dLoad.La + 2 * y) / (dLoad.La + y) : 0.0f;
                    Wx = (dLoad.La + y) * (position - a) / 2f;

                    return (Ra - Wx - originalLine) * dirComponent;
                    //return dirComponent * dLoad.La * (position - a) + dLoad.La * (dLoad.Db - dLoad.Da) * position;
                }
            }

            return 0f;
        }
        #endregion

        #region Legacy
        //private Vector4 assembleF(LineLoad load, float lineLength)
        //{
        //    Vector4 loadJointForces = new Vector4();

        //    if (load is ConcentratedSpanLoad)
        //    {
        //        float P = ((ConcentratedSpanLoad)load).L;
        //        float a = lineLength * ((ConcentratedSpanLoad)load).D;
        //        float b = lineLength - a;
        //        loadJointForces.X = (P * b * b * (3 * a + b)) / (lineLength * lineLength * lineLength); // Translation
        //        loadJointForces.Y = (P * a * a * (3 * b + a)) / (lineLength * lineLength * lineLength); // Translation
        //        loadJointForces.Z = P * a * b * b / (lineLength * lineLength);		// Rotation
        //        loadJointForces.W = -(P * a * a * b / (lineLength * lineLength));	// Rotation
        //    }
        //    else if (load is DistributedSpanLoad)
        //    {
        //    }

        //    return loadJointForces;
        //}

        //private void getNodalF(AbstractCase ac, LineElement line, float[] jointForces, float scale)
        //{
        //    //void AssignedLoads::getNodalF(int conditionID, Beam *beam, double f[])
        //    if (ac == null) return;

        //    if (ac is LoadCombination)
        //    {
        //        foreach (AbstractCaseFactor acf in ((LoadCombination)ac).Cases)
        //            getNodalF(acf.Case, line, jointForces, acf.Factor);
        //    }
        //    else if (ac is AnalysisCase)
        //    {
        //        if (((AnalysisCase)ac).Properties is StaticCaseProps)
        //        {
        //            foreach (StaticCaseFactor staticCase in ((StaticCaseProps)((AnalysisCase)ac).Properties).Loads)
        //            {
        //                if (staticCase.AppliedLoad is LoadCase)
        //                {
        //                    LoadCase lc = staticCase.AppliedLoad as LoadCase;
        //                    foreach (Load load in line.Loads[lc])
        //                        addNodalF(line, load, jointForces, scale);

        //                    if (lc.SelfWeight > 0f)
        //                    {
        //                        if (line.Properties is StraightFrameProps)
        //                        {
        //                            StraightFrameProps frameProps = line.Properties as StraightFrameProps;
        //                            selfWeight.La = selfWeight.Lb = frameProps.Section.Area * frameProps.Section.Material.UnitWeight;
        //                            addNodalF(line, selfWeight, jointForces, lc.SelfWeight * scale);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private void addNodalF(LineElement line, Load load, float[] jointForces, float scale)
        //{
        //    LineLoad lload;
        //    if ((lload = (load as LineLoad)) == null) return;
        //    float lineLength = line.Length;

        //    // Get 4 joint load Vectors, 1 Shear + 1 Moment per joint
        //    Vector4 loadJointForces = assembleF(lload, lineLength);

        //    // Get Load direction in Local Coordinate frame
        //    Vector3 dir = getLocalDir(line, lload.Direction);

        //    // Calculate
        //    // Checar esta parte (es correcto proyectar los momentos según sus ejes?, z negativo?)

        //    // X Component
        //    float jointAxialForce = dir.X * (loadJointForces.X + loadJointForces.Y) / 2f * scale;
        //    jointForces[0] += jointAxialForce;
        //    jointForces[6] += jointAxialForce;
        //    //jointForces[3]  +=  0f;
        //    //jointForces[9]  +=  0f;

        //    // Y Component
        //    jointForces[1] += dir.Y * loadJointForces.X * scale;		// ShearY
        //    jointForces[7] += dir.Y * loadJointForces.Y * scale;		// ShearY
        //    jointForces[4] += -dir.Z * loadJointForces.Z * scale;	    // MomentY
        //    jointForces[10] += -dir.Z * loadJointForces.W * scale;	    // MomentY

        //    // Z Component
        //    jointForces[2] += dir.Z * loadJointForces.X * scale;		// ShearZ
        //    jointForces[8] += dir.Z * loadJointForces.Y * scale;		// ShearZ
        //    jointForces[5] += dir.Y * loadJointForces.Z * scale;		// MomentZ
        //    jointForces[11] += dir.Y * loadJointForces.W * scale;		// MomentZ
        //}
        #endregion

    }
}
