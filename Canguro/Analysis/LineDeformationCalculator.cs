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
    class LineDeformationCalculator : ModelCalculator
    {
        private DistributedSpanLoad selfWeight;

        public LineDeformationCalculator()
        {
            selfWeight = new DistributedSpanLoad();
            selfWeight.Direction = LineLoad.LoadDirection.Gravity;
            selfWeight.Da = 0f;
            selfWeight.Db = 1f;
        }

        public enum DeformationAxis { Local2, Local3};

        public Vector3[] GetCurve(LineElement l, AbstractCase ac, int numPoints, float deformationScale, float paintScaleFactorTranslation, out float[] xPos)
        {
            if (l == null)
            {
                xPos = null;
                return null;
            }

            Vector3 iPos, jPos;
            iPos = new Vector3(model.Results.JointDisplacements[l.I.Id, 0],
                             model.Results.JointDisplacements[l.I.Id, 1],
                             model.Results.JointDisplacements[l.I.Id, 2]);
            iPos = deformationScale * paintScaleFactorTranslation * iPos + l.I.Position;

            jPos = new Vector3(model.Results.JointDisplacements[l.J.Id, 0],
                             model.Results.JointDisplacements[l.J.Id, 1],
                             model.Results.JointDisplacements[l.J.Id, 2]);
            jPos = deformationScale * paintScaleFactorTranslation * jPos + l.J.Position;

            float[,] local2Values = GetCurvedAxis(l, model.Results.ActiveCase.AbstractCase, Analysis.LineDeformationCalculator.DeformationAxis.Local2, numPoints);
            float[,] local3Values = GetCurvedAxis(l, model.Results.ActiveCase.AbstractCase, Analysis.LineDeformationCalculator.DeformationAxis.Local3, numPoints);

            int nVertices = local2Values.GetLength(0);
            Vector3[] curve = new Vector3[nVertices];
            xPos = new float[nVertices];
            for (int i = 0; i < nVertices; i++)
            {
                xPos[i] = local2Values[i, 0];
                curve[i] = iPos + local2Values[i, 0] * (jPos - iPos) +
                    local2Values[i, 1] * deformationScale * paintScaleFactorTranslation * l.LocalAxes[1] +
                    local3Values[i, 1] * deformationScale * paintScaleFactorTranslation * l.LocalAxes[2];
            }

            return curve;
        }

        //public Vector3 GetPoint(LineElement l, AbstractCase ac, float xPos, float deformationScale, float paintScaleFactorTranslation)
        //{
        //    if (l == null)
        //    {
        //        return Vector3.Empty;
        //    }

        //    Vector3 iPos, jPos;
        //    iPos = new Vector3(model.Results.JointDisplacements[l.I.Id, 0],
        //                     model.Results.JointDisplacements[l.I.Id, 1],
        //                     model.Results.JointDisplacements[l.I.Id, 2]);
        //    iPos = deformationScale * paintScaleFactorTranslation * iPos + l.I.Position;

        //    jPos = new Vector3(model.Results.JointDisplacements[l.J.Id, 0],
        //                     model.Results.JointDisplacements[l.J.Id, 1],
        //                     model.Results.JointDisplacements[l.J.Id, 2]);
        //    jPos = deformationScale * paintScaleFactorTranslation * jPos + l.J.Position;

        //    float[,] local2Values = GetCurvedPoint(l, model.Results.ActiveCase.AbstractCase, Analysis.LineDeformationCalculator.DeformationAxis.Local2, xPos);
        //    float[,] local3Values = GetCurvedPoint(l, model.Results.ActiveCase.AbstractCase, Analysis.LineDeformationCalculator.DeformationAxis.Local3, xPos);

        //    Vector3 point = iPos + local2Values[0, 0] * (jPos - iPos) +
        //        local2Values[0, 1] * deformationScale * paintScaleFactorTranslation * l.LocalAxes[1] +
        //        local3Values[0, 1] * deformationScale * paintScaleFactorTranslation * l.LocalAxes[2];

        //    return point;
        //}

        private void getCurvedAxis(LineElement l, AbstractCase ac, DeformationAxis component, float[,] controlPoints)
        {
            Vector3 vI, vJ, mI, mJ, dI, dJ;

            vI = new Vector3(model.Results.ElementJointForces[l.Id, 0, 0],
                             model.Results.ElementJointForces[l.Id, 0, 1],
                             model.Results.ElementJointForces[l.Id, 0, 2]);

            vJ = new Vector3(model.Results.ElementJointForces[l.Id, 1, 0],
                             model.Results.ElementJointForces[l.Id, 1, 1],
                             model.Results.ElementJointForces[l.Id, 1, 2]);

            mI = new Vector3(model.Results.ElementJointForces[l.Id, 0, 3],
                             model.Results.ElementJointForces[l.Id, 0, 4],
                             model.Results.ElementJointForces[l.Id, 0, 5]);

            mJ = new Vector3(model.Results.ElementJointForces[l.Id, 1, 3],
                             model.Results.ElementJointForces[l.Id, 1, 4],
                             model.Results.ElementJointForces[l.Id, 1, 5]);

            dI = l.I.Position + new Vector3(model.Results.JointDisplacements[l.I.Id, 0],
                                            model.Results.JointDisplacements[l.I.Id, 1],
                                            model.Results.JointDisplacements[l.I.Id, 2]);

            dJ = l.J.Position + new Vector3(model.Results.JointDisplacements[l.J.Id, 0],
                                            model.Results.JointDisplacements[l.J.Id, 1],
                                            model.Results.JointDisplacements[l.J.Id, 2]);

            // Transform to the line's local coordinate system
            // From the global system 
            // (WARNING: This is supposed to come in the joint's local coordinate system)
            vI = toLocal(l, vI);
            vJ = toLocal(l, vJ);

            mI = toLocal(l, mI);
            mJ = toLocal(l, mJ);

            dI = toLocal(l, dI);
            dJ = toLocal(l, dJ);

            float fI = 0f, fJ = 0f, fmI = 0f, fmJ = 0f, fdI = 0f, fdJ = 0f;
            float EI = ((StraightFrameProps)l.Properties).Section.Material.TypeProperties.E;
            float lineLength = l.Length;

            switch (component)
            {
                case DeformationAxis.Local2:
                    fI = vI.Y; fmI = -mI.Z; fdI = dI.Y;
                    fJ = vJ.Y; fmJ = -mJ.Z; fdJ = dJ.Y;

                    EI *= ((StraightFrameProps)l.Properties).Section.I33;
                    break;
                case DeformationAxis.Local3:
                    fI = vI.Z; fmI = mI.Y; fdI = dI.Z;
                    fJ = vJ.Z; fmJ = mJ.Y; fdJ = dJ.Z;

                    EI *= ((StraightFrameProps)l.Properties).Section.I22;
                    break;
            }

            addLoadDeflection(ac, l, lineLength, controlPoints, component, 1.0f, EI);
            addMomentDeflection(fmI, lineLength, controlPoints, EI, -1);
            addMomentDeflection(fmJ, lineLength, controlPoints, EI, 1);
        }

        public float[,] GetCurvedAxis(LineElement l, AbstractCase ac, DeformationAxis component, int numPoints)
        {
            if (!model.HasResults) return null;

            /// controlPoints[i, 0]: x position on Beam, controlPoints[i, 1]: 'deflection' for x position, controlPoints[i,2]: deformation angle
            float[,] controlPoints = new float[numPoints, 3];            

            float[] controlPointsX = new float[1]; // requestXCtrlPts(load);
            for (int i = 0, bufi = 0; i < numPoints; i++)
            {
                controlPoints[i, 0] = i / (numPoints - 1f);

                // Adjust control points
                if ((bufi < controlPointsX.Length) && (controlPointsX[bufi] <= controlPoints[i, 0]))
                    controlPoints[i, 0] = controlPointsX[bufi++];

                controlPoints[i, 1] = 0f; // controlPoints[i, 0] * (fdJ - fdI);

                controlPoints[i, 2] = 0.0f;
            }

            getCurvedAxis(l, ac, component, controlPoints);

            return controlPoints;
        }

        public float[] GetCurvedPoint(LineElement l, AbstractCase ac, DeformationAxis component, float xPos)
        {
            if (!model.HasResults) return null;

            /// controlPoints[i, 0]: x position on Beam, controlPoints[i, 1]: 'deflection' for x position, controlPoints[i,2]: deformation angle
            float[,] controlPoints = new float[1, 3];
            controlPoints[0, 0] = xPos;
            controlPoints[0, 1] = 0f;
            controlPoints[0, 2] = 0.0f;

            getCurvedAxis(l, ac, component, controlPoints);

            // Copy to 1-dimensional array
            float[] ret = new float[3];
            for (int i = 0; i < 3; i++)
                ret[i] = controlPoints[0, i];
            
            return ret;
        }

        #region Add Load Deflection
        private void addLoadDeflection(AbstractCase ac, LineElement line, float lineLength, float[,] controlPoints, DeformationAxis component, float scale, float EI)
        {
            if (ac == null) return;

            if (ac is LoadCombination)
            {
                foreach (AbstractCaseFactor acf in ((LoadCombination)ac).Cases)
                    addLoadDeflection(acf.Case, line, lineLength, controlPoints, component, acf.Factor, EI);
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
                                {
                                    //addUniformLoadDeflection(line, lineLength, load as LineLoad, controlPoints, component, scale, EI);
                                    //addTriangularLoadDeflection(line, lineLength, load as LineLoad, controlPoints, component, scale, EI);
                                    if (load is DirectionalLineLoad)
                                        addLoadDeflection(line, lineLength, load as DirectionalLineLoad, controlPoints, component, scale, EI);
                                }
                            }
                            if (lc.SelfWeight > 0f)
                            {
                                if (line.Properties is StraightFrameProps)
                                {
                                    StraightFrameProps frameProps = line.Properties as StraightFrameProps;
                                    selfWeight.La = selfWeight.Lb = frameProps.Section.Area * frameProps.Section.Material.UnitWeight;
                                    //addUniformLoadDeflection(line, lineLength, selfWeight, controlPoints, component, lc.SelfWeight * scale, EI);
                                    addLoadDeflection(line, lineLength, selfWeight, controlPoints, component, lc.SelfWeight * scale, EI);
                                }
                            }              
                        }
                    }
                }
            }
        }

        private void addLoadDeflection(LineElement line, float lineLength, DirectionalLineLoad load, float[,] controlPoints, DeformationAxis component, float scale, float EI)
        {
            if (load == null) return;

            Vector3 dir = getLocalDir(line, load.Direction);
            float dirComponent = 0.0f;

            switch (component)
            {
                case DeformationAxis.Local2:
                    dirComponent = dir.Y;
                    break;
                case DeformationAxis.Local3:
                    dirComponent = dir.Z;
                    break;
            }

            if (load is ConcentratedSpanLoad)
            {
                if (load.Type == LineLoad.LoadType.Force)
                    addConcentratedForceDeflection(line, lineLength, (ConcentratedSpanLoad)load, controlPoints, dirComponent, scale, EI);
                else
                    addConcentratedMomentDeflection(line, lineLength, (ConcentratedSpanLoad)load, controlPoints, dirComponent, scale, EI);
            }
            else if (load is DistributedSpanLoad)
            {
                DistributedSpanLoad dsl = (DistributedSpanLoad)load;

                float a, b, c;
                a = dsl.Da * lineLength;
                b = (dsl.Db - dsl.Da) * lineLength;
                c = (1.0f - dsl.Db) * lineLength;

                if (load.Type == LineLoad.LoadType.Force)
                {
                    addUniformForceDeflection(line, lineLength, dsl, a, b, c, controlPoints, dirComponent, scale, EI);
                    addTriangularForceDeflection(line, lineLength, dsl, a, b, c, controlPoints, dirComponent, scale, EI);
                }
                else
                {
                    // TODO: Partial Uniform and Triangular Moments Deflection calculator
                }
            }
        }
        #endregion

        #region Uniform Load
        private void addUniformForceDeflection(LineElement line, float lineLength, DistributedSpanLoad load, float a, float b, float c, float[,] controlPoints, float dirComponent, float scale, float EI)
        {
            float W, w;
            float RA;
            float c1, c3, c5, c6;
            float x = 0.0f, deflection = 0.0f, angle = 0.0f;

            w = -1f * Math.Sign(load.La) * Math.Min(Math.Abs(load.La), Math.Abs(load.Lb));
            W = w * b;

            RA = W * (1.0f - (2.0f * a + b) / (2.0f * lineLength));

            // Calculo de las constantes
            c6 = w * (float)Math.Pow(b, 3) * (b + 2.0f * a) / 48.0f;
            c5 = (w * b * (float)Math.Pow(lineLength - a - b / 2.0f, 3) / (6.0f * lineLength)) - RA * (float)Math.Pow(lineLength, 2) / 6.0f - c6 / lineLength;
            c1 = c3 = w * (float)Math.Pow(b, 3) / 24.0f + c5;

            // Flechas, angulos
            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                x = controlPoints[i, 0] * lineLength;
                deflection = addUniformForceDeflection(load, x, lineLength, ref angle, a, b, c, W, w, RA, c1, c3, c5, c6) * scale / EI;
                controlPoints[i, 1] += (deflection * dirComponent);
                controlPoints[i, 2] += angle * dirComponent / EI;
            }
        }

        private float addUniformForceDeflection(LineLoad load, float x, float lineLength, ref float angle, float a, float b, float c, float W, float w, float RA, float c1, float c3, float c5, float c6)
        {
            float flecha = 0.0f;

            if (x < a)
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6.0f + c1 * x;
                angle = RA * x * x / 2f + c1;
            }
            else if (x >= a && x <= a + b)
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6.0f - w * (float)Math.Pow(x - a, 4) / 24.0f + c3 * x;
                angle = RA * x * x / 2f - w * (float)Math.Pow(x - a, 3) / 6.0f + c3;
            }
            else if (x > a + b)
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6.0f - W * (float)Math.Pow(x - a - b / 2.0f, 3) / 6.0f + c5 * x + c6;
                angle = RA * x * x / 2f - W * (float)Math.Pow(x - a - b / 2.0f, 2) / 2f + c5;
            }

            return flecha;
        }
        #endregion

        #region Triangular Load
        private void addTriangularForceDeflection(LineElement line, float lineLength, DistributedSpanLoad load, float a, float b, float c, float[,] controlPoints, float dirComponent, float scale, float EI)
        {
            float q;
            float RA;
            float c1, c2, d1, d2, e1, e2;
            float x = 0.0f, deflection = 0.0f, angle = 0.0f;

            if ((load.La - load.Lb) == 0) return;

            // Define load orientation +/-, left/right
            bool isLeft = false;
            if (Math.Abs(load.La) > Math.Abs(load.Lb))
                isLeft = true;

            if (isLeft)
            {                
                q = load.Lb - load.La;

                // Swap a and c
                float swapAC = a;
                a = c;
                c = swapAC;
            }
            else
                q = load.La - load.Lb;

            RA = (q * b / 2f) * (1f - (a + 2f * b / 3f) / lineLength);

            // Calculo de las constantes
            c2 = d2 = 0f;
            e1 = (q * b * (float)Math.Pow(lineLength - a - 2f * b / 3f, 3) / 12f - RA * (float)Math.Pow(lineLength, 3) / 6f - 7f * q * (float)Math.Pow(b, 4) / 810f - q * a * (float)Math.Pow(b, 3) / 72f) / lineLength;
            e2 = q * a * (float)Math.Pow(b, 3) / 72 + 7f * q * (float)Math.Pow(b, 4) / 810f;
            c1 = d1 = e1 + q * (float)Math.Pow(b, 3) / 72;

            // Flechas, angulos
            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                if (isLeft)
                    x = (1f - controlPoints[i, 0]) * lineLength;
                else
                    x = controlPoints[i, 0] * lineLength;
                deflection = addTriangularForceDeflection(load, lineLength, x, ref angle, RA, a, b, q, c1, c2, d1, d2, e1, e2) * scale / EI;
                controlPoints[i, 1] += (deflection * dirComponent);
                controlPoints[i, 2] += angle * dirComponent / EI;
            }
        }

        private float addTriangularForceDeflection(LineLoad load, float lineLength, float x, ref float angle, float RA, float a, float b, float q, float c1, float c2, float d1, float d2, float e1, float e2)
        {
            float flecha = 0.0f;

            if (x < a)
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6.0f + c1 * x + c2;
                angle = RA * x * x / 2f + c1;
            }
            else if (x >= a && x <= a + b)
            {
                flecha = b > float.Epsilon ? RA * (float)Math.Pow(x, 3) / 6.0f - q * (float)Math.Pow(x - a, 5) / (120.0f * b) + d1 * x + d2 : 0.0f;
                angle = b > float.Epsilon ? RA * x * x / 2f - q * (float)Math.Pow(x - a, 4) / (24f * b) + d1 : 0.0f;
            }
            else if (x > a + b)
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6.0f - q * b * (float)Math.Pow(x - (a + 2.0f * b / 3.0f), 3) / 12.0f + e1 * x + e2;
                angle = RA * x * x / 2f - q * b * (float)Math.Pow(x - (a + 2f * b / 3f), 2) / 4f + e1;
            }

            return flecha;
        }
        #endregion

        #region Concentrated Load

        #region Concentrated Forces
        private void addConcentratedForceDeflection(LineElement line, float lineLength, ConcentratedSpanLoad load, float[,] controlPoints, float dirComponent, float scale, float EI)
        {
            float a, b;
            float RA, P;
            float c1, c3;
            float x = 0.0f, deflection = 0.0f, angle = 0.0f;

            ConcentratedSpanLoad csl = (ConcentratedSpanLoad)load;
            a = csl.D * lineLength;
            b = lineLength - a;
            P = -csl.L;
            RA = P * (1f - a / lineLength);
            c1 = c3 = P * a * (lineLength - a) * (a - 2f * lineLength) / (6f * lineLength);

            // Flechas, angulos
            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                x = controlPoints[i, 0] * lineLength;
                deflection = addConcentratedForceDeflection(load, x, lineLength, ref angle, a, b, RA, P, c1, c3) * scale / EI;
                controlPoints[i, 1] += (deflection * dirComponent);
                controlPoints[i, 2] += angle * dirComponent / EI;
            }
        }

        private float addConcentratedForceDeflection(ConcentratedSpanLoad load, float x, float lineLength, ref float angle, float a, float b, float RA, float P, float c1, float c3)
        {
            float flecha = 0.0f;
            if (x < a)
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6f + c1 * x;
                angle = RA * (float)Math.Pow(x, 2) / 2f + c1;
            }
            else
            {
                flecha = RA * (float)Math.Pow(x, 3) / 6f - P * (float)Math.Pow(x - a, 3) / 6f + c3 * x;
                angle = RA * (float)Math.Pow(x, 2) / 2f - P * (float)Math.Pow(x - a, 2) / 2f + c3;
            }

            return flecha;
        }
        #endregion

        #region Concentrated Moments
        private void addConcentratedMomentDeflection(LineElement line, float lineLength, ConcentratedSpanLoad load, float[,] controlPoints, float dirComponent, float scale, float EI)
        {
            float a, b;
            float RA, M;
            float c1, c3, c4;
            float x = 0.0f, deflection = 0.0f, angle = 0.0f;

            ConcentratedSpanLoad csl = (ConcentratedSpanLoad)load;
            a = csl.D * lineLength;
            b = lineLength - a;
            M = csl.L;
            RA = M / lineLength;
            c3 = M * (-lineLength * lineLength / 3f - a * a / 2f) / lineLength;
            c1 = a * M + c3;
            c4 = M * a * a / 2f;

            // Flechas, angulos
            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                x = controlPoints[i, 0] * lineLength;
                deflection = addConcentratedMomentDeflection(load, x, lineLength, ref angle, a, b, RA, M, c1, c3, c4) * scale / EI;
                controlPoints[i, 1] += (deflection * dirComponent);
                controlPoints[i, 2] += angle * dirComponent / EI;
            }
        }

        private float addConcentratedMomentDeflection(ConcentratedSpanLoad load, float x, float lineLength, ref float angle, float a, float b, float RA, float M, float c1, float c3, float c4)
        {
            float flecha = 0.0f;
            if (x < a)
            {
                flecha = -RA * (float)Math.Pow(x, 3) / 6f + c1 * x;
                angle = -RA * (float)Math.Pow(x, 2) / 2f + c1;
            }
            else
            {
                flecha = -RA * (float)Math.Pow(x, 3) / 6f + M * (float)Math.Pow(x, 2) / 2f + c3 * x + c4;
                angle = -RA * (float)Math.Pow(x, 2) / 2f + M * x + c3;
            }

            return flecha;
        }
        #endregion

        #endregion

        #region Moments at the ends calculation
        private void addMomentDeflection(float moment, float lineLength, float[,] controlPoints, float EI, float scale)
        {
            float x, deflection, angle = 0.0f;
            float c1, c3;
            c3 = -5f * moment * lineLength / 6f;
            c1 = moment * lineLength + c3;

            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                x = controlPoints[i, 0] * lineLength;
                deflection = addMomentDeflection(moment, x, lineLength, ref angle, c1, c3, EI, scale);
                controlPoints[i, 1] += deflection;
                controlPoints[i, 2] += angle;
            }
        }

        private float addMomentDeflection(float moment, float x, float lineLength, ref float angle, float c1, float c3, float EI, float scale)
        {
            if (scale < 0)
            {
                c3 = -moment * lineLength / 3f;
                angle = -scale * (-moment * x * x / (2f * lineLength) + moment * x + c3) / EI;
                return scale * moment * lineLength * (lineLength - x) * (1f - (lineLength - x) * (lineLength - x) / (lineLength * lineLength)) / (6.0f * EI);
            }
            else
            {
                angle = scale * (-moment * x * x / (2f * lineLength) + c1) / EI;
                return scale * moment * lineLength * x * (1f - x * x / (lineLength * lineLength)) / (6.0f * EI);
            }
        }
        #endregion
    }
}
