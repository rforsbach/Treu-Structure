using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.View.Gadgets
{
    public class LineGadgetService : GadgetService
    {
        GadgetManager gadgetManager;
        /// <summary> Sections dictionary, for use when using ShadedLineRenderer </summary>
        Dictionary<FrameSection, GadgetLODLocator> sections = new Dictionary<FrameSection, GadgetLODLocator>();
        /// <summary> Releases dictionary, for use when rendergin releases on lines </summary>
        Dictionary<byte, GadgetLocator> releases = new Dictionary<byte, GadgetLocator>();

        List<LineElement> freeReleases;
        List<LineElement> springReleases;

        #region Releases constuction vars
        private int transSymColor = System.Drawing.Color.FromArgb(150, System.Drawing.Color.Gold).ToArgb();
        private int rotSymColor = System.Drawing.Color.FromArgb(100, System.Drawing.Color.GhostWhite).ToArgb();
        private int rotSpringColor = System.Drawing.Color.FromArgb(150, System.Drawing.Color.PaleGoldenrod).ToArgb();
        private int transSpringColor = System.Drawing.Color.FromArgb(150, System.Drawing.Color.SkyBlue).ToArgb();

        private float quadSize = 0.3f;
        private int circleSegments = 10;
        private float circleRadius = 0.12f;
        #endregion

        public LineGadgetService(GadgetManager gm)
        {
            gadgetManager = gm;
            freeReleases = new List<LineElement>();
            springReleases = new List<LineElement>();
        }

        #region Releases Drawing

        public void DrawReleases(Device device)
        {
            if ((freeReleases != null && freeReleases.Count > 0) || (springReleases != null && springReleases.Count > 0))
            {
                JointDOF activeDOF;
                Vector3[] localAxes;
                Vector3 jointPos;
                bool invertSpring;

                byte[] transRestraint = new byte[2];
                byte[] rotRestraint = new byte[2];

                byte[] transSpring = new byte[2];
                byte[] rotSpring = new byte[2];

                #region Get active renderstates, for restoring them after having enabled alpha blending
                Cull cull = device.RenderState.CullMode;
                bool alphaEnable = device.RenderState.AlphaBlendEnable;

                // For having the grid plane we need:
                device.RenderState.CullMode = Cull.None;                        // Disable Face Culling
                device.RenderState.Lighting = false;                            // Disable lighting
                device.RenderState.AlphaBlendEnable = true;                     // Enable alpha blending
                device.RenderState.SourceBlend = Blend.BothInvSourceAlpha;
                device.RenderState.DestinationBlend = Blend.DestinationColor;
                #endregion

                gadgetManager.SetActiveStream(ResourceStreamType.TriangleListPositionColored);

                foreach (LineElement l in freeReleases)
                {
                    localAxes = l.LocalAxes;
                    #region Free release drawing
                    // Quad shape
                    transRestraint[0] = (byte)((l.DoFI.T1 == JointDOF.DofType.Free || l.DoFI.T2 == JointDOF.DofType.Free || l.DoFI.T3 == JointDOF.DofType.Free) ? 1 : 0);
                    transRestraint[1] = (byte)((l.DoFJ.T1 == JointDOF.DofType.Free || l.DoFJ.T2 == JointDOF.DofType.Free || l.DoFJ.T3 == JointDOF.DofType.Free) ? 1 : 0);

                    // Round shape
                    rotRestraint[0] = (byte)((l.DoFI.R1 == JointDOF.DofType.Free || l.DoFI.R2 == JointDOF.DofType.Free || l.DoFI.R3 == JointDOF.DofType.Free) ? 2 : 0);
                    rotRestraint[1] = (byte)((l.DoFJ.R1 == JointDOF.DofType.Free || l.DoFJ.R2 == JointDOF.DofType.Free || l.DoFJ.R3 == JointDOF.DofType.Free) ? 2 : 0);

                    activeDOF = l.DoFI;
                    jointPos = l.I.Position + quadSize / 2 * Vector3.Normalize(localAxes[0]);

                    for (int i = 0; i < 2; ++i)
                    {
                        if (transRestraint[i] != 0)
                        {
                            if (!releases.ContainsKey(transRestraint[i]))
                                genRestraints(transRestraint[i]);

                            drawFreeSymbol4DoF(device, activeDOF, jointPos, l, localAxes, transRestraint[i]);
                        }

                        if (rotRestraint[i] != 0)
                        {
                            if (!releases.ContainsKey(rotRestraint[i]))
                                genRestraints(rotRestraint[i]);

                            drawFreeSymbol4DoF(device, activeDOF, jointPos, l, localAxes, rotRestraint[i]);
                        }

                        activeDOF = l.DoFJ;
                        jointPos = l.J.Position - quadSize / 2 * Vector3.Normalize(localAxes[0]);
                    }
                    #endregion
                }

                #region Restore active render states
                // For having contour and gird lines drawn:
                device.RenderState.AlphaBlendEnable = alphaEnable;              // Disable alpha blending
                device.RenderState.CullMode = cull;                             // Disable face culling
                #endregion

                gadgetManager.SetActiveStream(ResourceStreamType.Lines);

                foreach (LineElement l in springReleases)
                {
                    localAxes = l.LocalAxes;
                    #region Spring release drawing
                    // Translation spring
                    transSpring[0] = (byte)((l.DoFI.T1 == JointDOF.DofType.Spring || l.DoFI.T2 == JointDOF.DofType.Spring || l.DoFI.T3 == JointDOF.DofType.Spring) ? 4 : 0);
                    transSpring[1] = (byte)((l.DoFJ.T1 == JointDOF.DofType.Spring || l.DoFJ.T2 == JointDOF.DofType.Spring || l.DoFJ.T3 == JointDOF.DofType.Spring) ? 4 : 0);

                    // Rotation spring
                    rotSpring[0] = (byte)((l.DoFI.R1 == JointDOF.DofType.Spring || l.DoFI.R2 == JointDOF.DofType.Spring || l.DoFI.R3 == JointDOF.DofType.Spring) ? 8 : 0);
                    rotSpring[1] = (byte)((l.DoFJ.R1 == JointDOF.DofType.Spring || l.DoFJ.R2 == JointDOF.DofType.Spring || l.DoFJ.R3 == JointDOF.DofType.Spring) ? 8 : 0);

                    activeDOF = l.DoFI;
                    jointPos = l.I.Position + quadSize / 2 * Vector3.Normalize(localAxes[0]);
                    invertSpring = false;

                    for (int i = 0; i < 2; ++i)
                    {
                        if (transSpring[i] != 0)
                        {
                            if (!releases.ContainsKey(transSpring[i]))
                                genSprings(transSpring[i]);

                            drawSpringSymbol4DoF(device, activeDOF, jointPos, l, localAxes, transSpring[i], invertSpring);
                        }

                        if (rotSpring[i] != 0)
                        {
                            if (!releases.ContainsKey(rotSpring[i]))
                                genSprings(rotSpring[i]);

                            drawSpringSymbol4DoF(device, activeDOF, jointPos, l, localAxes, rotSpring[i], invertSpring);
                        }

                        activeDOF = l.DoFJ;
                        jointPos = l.J.Position - quadSize / 2 * Vector3.Normalize(localAxes[0]);
                        invertSpring = true;
                    }
                    #endregion
                }

                freeReleases.Clear();
                springReleases.Clear();
            }
        }

        public void PrepareReleases(Device device, LineElement l)
        {
            if ((l.DoFI.T1 == JointDOF.DofType.Free || l.DoFI.T2 == JointDOF.DofType.Free || l.DoFI.T3 == JointDOF.DofType.Free) ||
                (l.DoFJ.T1 == JointDOF.DofType.Free || l.DoFJ.T2 == JointDOF.DofType.Free || l.DoFJ.T3 == JointDOF.DofType.Free) ||
                (l.DoFI.R1 == JointDOF.DofType.Free || l.DoFI.R2 == JointDOF.DofType.Free || l.DoFI.R3 == JointDOF.DofType.Free) ||
                (l.DoFJ.R1 == JointDOF.DofType.Free || l.DoFJ.R2 == JointDOF.DofType.Free || l.DoFJ.R3 == JointDOF.DofType.Free))
                freeReleases.Add(l);

            if ((l.DoFI.T1 == JointDOF.DofType.Spring || l.DoFI.T2 == JointDOF.DofType.Spring || l.DoFI.T3 == JointDOF.DofType.Spring) ||
                (l.DoFJ.T1 == JointDOF.DofType.Spring || l.DoFJ.T2 == JointDOF.DofType.Spring || l.DoFJ.T3 == JointDOF.DofType.Spring) ||
                (l.DoFI.R1 == JointDOF.DofType.Spring || l.DoFI.R2 == JointDOF.DofType.Spring || l.DoFI.R3 == JointDOF.DofType.Spring) ||
                (l.DoFJ.R1 == JointDOF.DofType.Spring || l.DoFJ.R2 == JointDOF.DofType.Spring || l.DoFJ.R3 == JointDOF.DofType.Spring))
                springReleases.Add(l);
        }

        private void setTransformationMatrix(Device device, Matrix lineOrientationMatrix, Vector3 rotationAxis, float angle, Vector3 translationPos)
        {
            Matrix transM = Matrix.Identity;
            // Get transformation matrix for drawing this release in the correct position and orientation
            transM = lineOrientationMatrix * Matrix.RotationAxis(rotationAxis, angle) * Matrix.Translation(translationPos);
            device.Transform.World = transM;
        }

        private void drawFreeSymbol4DoF(Device device, JointDOF dof, Vector3 jointPos, LineElement l, Vector3[] localAxes, byte key)
        {
            float ninetyDegrees = (float)(Math.PI / 2.0);
            // Get orientation according to the real orientation of the line
            Matrix orientationMatrix;
            l.RotationMatrix(out orientationMatrix);

            if (key == 1)
            {
                if (dof.T1 == JointDOF.DofType.Free)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[1], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
                if (dof.T2 == JointDOF.DofType.Free)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[0], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
                if (dof.T3 == JointDOF.DofType.Free)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[2], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
            }
            else if (key == 2)
            {
                if (dof.R1 == JointDOF.DofType.Free)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[1], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
                if (dof.R2 == JointDOF.DofType.Free)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[0], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
                if (dof.R3 == JointDOF.DofType.Free)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[2], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
            }
        }

        private void drawSpringSymbol4DoF(Device device, JointDOF dof, Vector3 jointPos, LineElement l, Vector3[] localAxes, byte key,bool invertSpring)
        {
            float ninetyDegrees = (float)(Math.PI / 2);
            // Get orientation according to the real orientation of the line
            Matrix orientationMatrix;
            
            l.RotationMatrix(out orientationMatrix);

            if (key == 4)
            {
                if (dof.T1 == JointDOF.DofType.Spring)
                {
                    if (invertSpring)
                        setTransformationMatrix(device, orientationMatrix * Matrix.RotationAxis(localAxes[1], (float)Math.PI), localAxes[0], ninetyDegrees, jointPos);
                    else
                        setTransformationMatrix(device, orientationMatrix, localAxes[0], ninetyDegrees, jointPos);

                    drawSymbolByKey(device, key);
                }
                if (dof.T2 == JointDOF.DofType.Spring)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[2], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
                if (dof.T3 == JointDOF.DofType.Spring)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[1], -ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
            }
            else if (key == 8)
            {
                if (dof.R1 == JointDOF.DofType.Spring)
                {
                    if (invertSpring)
                        setTransformationMatrix(device, orientationMatrix * Matrix.RotationAxis(localAxes[1], (float)Math.PI), localAxes[0], ninetyDegrees, jointPos);
                    else
                        setTransformationMatrix(device, orientationMatrix, localAxes[0], ninetyDegrees, jointPos);

                    drawSymbolByKey(device, key);
                }
                if (dof.R2 == JointDOF.DofType.Spring)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[2], ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
                if (dof.R3 == JointDOF.DofType.Spring)
                {
                    setTransformationMatrix(device, orientationMatrix, localAxes[1], -ninetyDegrees, jointPos);
                    drawSymbolByKey(device, key);
                }
            }
        }

        private void drawSymbolByKey(Device device, byte key)
        {
            if (key == 1 || key == 2)
                device.DrawPrimitives(PrimitiveType.TriangleList, releases[key].Offset, releases[key].Size / 3);
            else if (key == 4 || key == 8)
                device.DrawPrimitives(PrimitiveType.LineList, releases[key].Offset, releases[key].Size / 2);
        }

        private void genSprings(byte spring)
        {
            CustomVertex.PositionColored[] symVertices;
            float lastX = 0.0f, lastY = 0.0f;
            float x = 0.0f, y = 0.0f, z = 0.0f;
            int totalVertices;

            if (spring == 4)
            {
                float translationSpringSegmentSize = 0.2f;
                int translationSpringSegments = 8;

                totalVertices = translationSpringSegments * 2;

                symVertices = new CustomVertex.PositionColored[totalVertices];

                PositionColoredPackage package = (PositionColoredPackage)gadgetManager.CaptureBuffer(ResourceStreamType.Lines, 0, totalVertices);

                #region Put spring vertices in VB
                unsafe
                {
                    int dir = 1, step = 1;

                    for (int i = 0; i < totalVertices; i += 2)
                    {
                        symVertices[i] = new CustomVertex.PositionColored(lastX, lastY, 0.0f, transSymColor);

                        if (step % 2 == 0)
                            dir *= -1;

                        x = lastX + translationSpringSegmentSize * (float)Math.Cos(dir * 3.0 * Math.PI / 8.0f);
                        y = lastY + translationSpringSegmentSize * (float)Math.Sin(dir * 3.0 * Math.PI / 8.0f);

                        symVertices[i + 1] = new CustomVertex.PositionColored(x, y, 0.0f, transSymColor);

                        lastX = x; lastY = y;

                        step += dir;
                    }

                    // Orient segments... Rotation about draw axis
                    for (int index = 0; index < totalVertices; ++index)
                    {
                        //x = -symVertices[index].X;
                        //y = -symVertices[index].Y;
                        //z = symVertices[index].Z;

                        //symVertices[index].X = x;
                        //symVertices[index].Y = y;
                        //symVertices[index].Z = z;

                        //x = symVertices[index].X;
                        //y = symVertices[index].Y * (float)Math.Cos(-Math.PI / 4.0f) - symVertices[index].Z * (float)Math.Sin(-Math.PI / 4.0f);
                        //z = symVertices[index].Y * (float)Math.Sin(-Math.PI / 4.0f) + symVertices[index].Z * (float)Math.Cos(-Math.PI / 4.0f);

                        //symVertices[index].X = x;
                        //symVertices[index].Y = y;
                        //symVertices[index].Z = z;

                        package.VBPointer->Position = symVertices[index].Position;
                        package.VBPointer->Color = transSymColor;
                        package.VBPointer++;
                    }
                }
                #endregion

                gadgetManager.ReleaseBuffer(totalVertices, 0, ResourceStreamType.Lines);

                GadgetLocator locator = new GadgetLocator();
                locator.Offset = package.Offset;
                locator.Size = totalVertices;
                releases.Add(spring, locator);
            }
            else if (spring == 8)
            {
                float angle, angleStep;
                float rotationSpringRadius = 0.1f;
                float rotationSpringHeight = 0.01f;
                int rotationSpringSegments = 20;
                totalVertices = rotationSpringSegments * 2;

                symVertices = new CustomVertex.PositionColored[totalVertices];

                angleStep = 3.0f * (float)Math.PI / rotationSpringSegments;
                angle = 0.0f;

                lastX = 0.0f; lastY = 0.0f;

                x = y = z = 0.0f;

                PositionColoredPackage package = (PositionColoredPackage)gadgetManager.CaptureBuffer(ResourceStreamType.Lines, 0, totalVertices);

                #region Put rotation spring vertices in VB
                unsafe
                {

                    for (int i = 0; i < totalVertices; i+=2)
                    {
                        symVertices[i] = new CustomVertex.PositionColored(x, y, z, rotSpringColor);

                        y = rotationSpringRadius * (float)Math.Cos(angle);
                        z = rotationSpringRadius * (float)Math.Sin(angle);
                        x = rotationSpringHeight * (float)Math.PI * angle * 2.0f;

                        symVertices[i + 1] = new CustomVertex.PositionColored(x, y, z, rotSpringColor);

                        angle += angleStep;
                    }

                    // Orient segments... Rotation about draw axis
                    for (int index = 0; index < totalVertices; ++index)
                    {
                        //x = -symVertices[index].X;
                        //y = -symVertices[index].Y;
                        //z = symVertices[index].Z;

                        //symVertices[index].X = x;
                        //symVertices[index].Y = y;
                        //symVertices[index].Z = z;

                        package.VBPointer->Position = symVertices[index].Position;
                        package.VBPointer->Color = transSpringColor;
                        package.VBPointer++;
                    }
                }
                #endregion

                gadgetManager.ReleaseBuffer(totalVertices, 0, ResourceStreamType.Lines);

                GadgetLocator locator = new GadgetLocator();
                locator.Offset = package.Offset;
                locator.Size = totalVertices;
                releases.Add(spring, locator);
            }
        }

        private void genRestraints(byte restraints)
        {
            if (restraints == 1)
            {
                #region Produce Quad for representing a release on translations
                PositionColoredPackage package = (PositionColoredPackage)gadgetManager.CaptureBuffer(ResourceStreamType.TriangleListPositionColored, 0, 6);

                unsafe
                {
                    package.VBPointer->Position = new Vector3(-quadSize/2, -quadSize/2, 0.0f);
                    package.VBPointer->Color = transSymColor;
                    package.VBPointer++;

                    package.VBPointer->Position = new Vector3(quadSize/2, -quadSize/2, 0.0f);
                    package.VBPointer->Color = transSymColor;
                    package.VBPointer++;

                    package.VBPointer->Position = new Vector3(quadSize/2, quadSize/2, 0.0f);
                    package.VBPointer->Color = transSymColor;
                    package.VBPointer++;

                    package.VBPointer->Position = new Vector3(-quadSize/2, -quadSize/2, 0.0f);
                    package.VBPointer->Color = transSymColor;
                    package.VBPointer++;

                    package.VBPointer->Position = new Vector3(quadSize/2, quadSize/2, 0.0f);
                    package.VBPointer->Color = transSymColor;
                    package.VBPointer++;

                    package.VBPointer->Position = new Vector3(-quadSize/2, quadSize/2, 0.0f);
                    package.VBPointer->Color = transSymColor;
                    package.VBPointer++;
                }

                gadgetManager.ReleaseBuffer(6, 0, ResourceStreamType.TriangleListPositionColored);

                GadgetLocator locator = new GadgetLocator();
                locator.Offset = package.Offset;
                locator.Size = 6;
                releases.Add(restraints, locator);
                #endregion
            }
            else if (restraints == 2)
            {
                #region Prodice Circle for representing a release on rotations
                float angleStep = 2.0f * (float)Math.PI / circleSegments;

                int totalVertices = circleSegments * 3;

                PositionColoredPackage package = (PositionColoredPackage)gadgetManager.CaptureBuffer(ResourceStreamType.TriangleListPositionColored, 0, totalVertices);

                unsafe
                {
                    for (int i = 0; i < circleSegments; ++i)
                    {
                        package.VBPointer->Position = Vector3.Empty;
                        package.VBPointer->Color = transSymColor;
                        package.VBPointer++;

                        package.VBPointer->Position = new Vector3(circleRadius * (float)Math.Cos(angleStep * (i)),
                                                                  circleRadius * (float)Math.Sin(angleStep * (i)), 0.0f);
                        package.VBPointer->Color = rotSymColor;
                        package.VBPointer++;

                        package.VBPointer->Position = new Vector3(circleRadius * (float)Math.Cos(angleStep * (i + 1)),
                                                                  circleRadius * (float)Math.Sin(angleStep * (i + 1)), 0.0f);
                        package.VBPointer->Color = rotSymColor;
                        package.VBPointer++;
                    }
                }

                gadgetManager.ReleaseBuffer(totalVertices, 0, ResourceStreamType.TriangleListPositionColored);

                GadgetLocator locator = new GadgetLocator();
                locator.Offset = package.Offset;
                locator.Size = totalVertices;
                releases.Add(restraints, locator);
                #endregion
            }
        }
        #endregion

        #region Sections Drawing
        public void DrawCanonicalSection(Device device, FrameSection sec, View.LODContour lod)
        {
            int i, j, k;

            LODContour lineLOD = GraphicViewManager.Instance.PrintingHiResImage ? LODContour.High : lod;

            if (lineLOD == Canguro.View.LODContour.Wireframe)
                return;
            
            gadgetManager.SetActiveStream(ResourceStreamType.TriangleListPositionNormalColored);

            #region Add new section
            if (!sections.ContainsKey(sec))
            {
                int white = System.Drawing.Color.White.ToArgb();
                                
                // Get triangulated section indices
                short[][] cover = new short[][] { sec.CoverHigh, sec.CoverHighstress };
                int[] nIndices = new int[] { cover[0].Length, cover[1].Length };
                int contourLen = sec.Contour[0].Length;
                int totalVertices = contourLen << 2; //contourLen * 4
                
                // Get contour indices
                int totalIndices = 0;
                int numLODs = sec.ContourIndices.GetLength(0);
                for (i = 0; i < numLODs; i++)   // Beam triangles indices
                    totalIndices += (sec.ContourIndices[i].Length - 1) * ((needCover(i)) ? 6 : 2);
                for (i = 0; i < nIndices.Length; i++)   // Top/Bottom Cover triangles indices
                    totalIndices += 2 * nIndices[i];

                PositionNormalColoredPackage package = (PositionNormalColoredPackage)gadgetManager.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, totalIndices, totalVertices);

                #region Vertices
                unsafe
                {                                        
                    #region Draw beam vertices
                    for (i = 0; i < contourLen; i++)
                    {
                        package.VBPointer->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
                        package.VBPointer->Color = white;
                        package.VBPointer++;

                        package.VBPointer->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
                        package.VBPointer->Color = white;
                        package.VBPointer++;
                    }
                    #endregion                   

                    #region Draw covers vertices
                    for (i = 0; i < contourLen; ++i)
                    {
                        package.VBPointer->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                        package.VBPointer->Normal = -Canguro.Utility.CommonAxes.GlobalAxes[0];
                        package.VBPointer->Color = white;
                        package.VBPointer++;
                    }

                    for (i = 0; i < contourLen; ++i)
                    {
                        package.VBPointer->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                        package.VBPointer->Normal = Canguro.Utility.CommonAxes.GlobalAxes[0];
                        package.VBPointer->Color = white;
                        package.VBPointer++;
                    }
                    #endregion                   
                }
                #endregion

                #region Locator
                // Create locator
                GadgetLODLocator locator;

                // Assign vertiex buffer info
                locator.verticesLocator.Offset = package.Offset;
                locator.verticesLocator.Size = totalVertices;
                locator.indexOffsets = new int[numLODs];

                // Add indices for the different LODs
                int indexOffset = 0;
                unsafe
                {
                    for (i = 0; i < numLODs; ++i)
                    {
                        for (j = 0; j < sec.ContourIndices[i].Length-1; ++j)
                        {
                            *package.IBPointer = (short)(2 * sec.ContourIndices[i][j]);
                            ++package.IBPointer;

                            *package.IBPointer = (short)(2 * sec.ContourIndices[i][j] + 1);
                            ++package.IBPointer;

                            #region When using triangle lists (needed when drawing covers)
                            //if (needCover(i))
                            //{
                                *package.IBPointer = (short)(2 * sec.ContourIndices[i][j + 1] + 1);
                                ++package.IBPointer;

                                *package.IBPointer = (short)(2 * sec.ContourIndices[i][j]);
                                ++package.IBPointer;

                                *package.IBPointer = (short)(2 * sec.ContourIndices[i][j + 1] + 1);
                                ++package.IBPointer;

                                *package.IBPointer = (short)(2 * sec.ContourIndices[i][j + 1]);
                                ++package.IBPointer;
                            //}
                            #endregion
                        }

                        j *= 6; // (needCover(i)) ? 6 : 2;

                        #region Draw the covers
                        // If the contour is a High Quality one, then draw the cover
                        int lodIndex;
                        if (needCover(i))
                        {
                            lodIndex = i - (int)LODContour.High;
                            for (k = 0; k < nIndices[lodIndex]; k++)
                            {
                                *package.IBPointer = (short)(2 * contourLen + sec.ContourIndices[i][cover[lodIndex][k]]);
                                ++package.IBPointer;
                            }

                            j += k;

                            for (k = 0; k < nIndices[lodIndex]; k+=3)
                            {
                                *package.IBPointer = (short)(3 * contourLen + sec.ContourIndices[i][cover[lodIndex][k+2]]);
                                ++package.IBPointer;
                                *package.IBPointer = (short)(3 * contourLen + sec.ContourIndices[i][cover[lodIndex][k+1]]);
                                ++package.IBPointer;
                                *package.IBPointer = (short)(3 * contourLen + sec.ContourIndices[i][cover[lodIndex][k]]);
                                ++package.IBPointer;
                            }

                            j += k;
                        }
                        #endregion

                        locator.indexOffsets[i] = package.IBOffset + indexOffset;
                        indexOffset += j;
                    }
                }
                locator.totalIndices = indexOffset;
                #endregion

                gadgetManager.ReleaseBuffer(totalVertices, indexOffset, ResourceStreamType.TriangleListPositionNormalColored);

                // Add section to catalog
                if (sections.ContainsKey(sec))
                    sections.Remove(sec);
                sections.Add(sec, locator);
            }
            #endregion


            int lodIndices = ((int)lineLOD < sections[sec].indexOffsets.Length - 1) ? sections[sec].indexOffsets[(int)lineLOD + 1] - sections[sec].indexOffsets[(int)lineLOD] : sections[sec].totalIndices - sections[sec].indexOffsets[(int)lineLOD];
            //if (needCover((int)lineLOD))
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, sections[sec].verticesLocator.Offset, 0, sections[sec].verticesLocator.Size, sections[sec].indexOffsets[(int)lineLOD], lodIndices / 3);
            //else
            //    device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, sections[sec].verticesLocator.Offset, 0, sections[sec].verticesLocator.Size, sections[sec].indexOffsets[(int)lineLOD], lodIndices - 2);
        }

        private bool needCover(int intLOD)
        {
            return (intLOD >= (int)LODContour.High);
        }
        #endregion

        #region GadgetService Members

        public void ClearLocators()
        {
            sections.Clear();
            releases.Clear();
        }

        #endregion

        #region All Legacy
        #region Old DrawCanonicalSection
        //public void DrawCanonicalSection(Device device, FrameSection sec, View.Renderer.LODLevels lod)
        //{
        //    gadgetManager.SetActiveStream(ResourceStreamType.TriangleListPositionNormalColored);

        //    if (!sections.ContainsKey(sec))
        //    {
        //        //// Lock the VertexBuffer and obtain the unsafe pointer from the new lock
        //        //vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
        //        //vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
        //        int white = System.Drawing.Color.White.ToArgb();
                                
        //        // Get triangulated section indices
        //        int[] sectionIndices = sec.Cover;
        //        int nIndices = sectionIndices.Length;
        //        int contourLen = sec.Contour[0].Length;
        //        int totalVertices = nIndices * 2 + contourLen * 6;

        //        PositionNormalColoredPackage package = (PositionNormalColoredPackage)gadgetManager.CaptureVertexBuffer(ResourceStreamType.TriangleListPositionNormalColored, false, totalVertices);

        //        unsafe
        //        {
        //            #region Draw initial triangulated section
        //            for (int i = 0; i < nIndices; ++i)
        //            {
        //                package.VBPointer->Position = new Vector3(0, sec.Contour[0][sectionIndices[i]].Y, sec.Contour[0][sectionIndices[i]].X);
        //                package.VBPointer->Normal = -Canguro.Utility.CommonAxes.GlobalAxes[0];
        //                package.VBPointer->Color = white;
        //                package.VBPointer++;
        //            }
        //            #endregion

        //            #region Draw beam
        //            for (int i = 0; i < contourLen; i++)
        //            {
        //                // First triangle
        //                package.VBPointer->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
        //                package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                package.VBPointer->Color = white;
        //                package.VBPointer++;

        //                package.VBPointer->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
        //                package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                package.VBPointer->Color = white;
        //                package.VBPointer++;

        //                if (i + 1 < contourLen)
        //                {
        //                    package.VBPointer->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
        //                    if (sec.FaceNormals)
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                    else
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
        //                }
        //                else
        //                {
        //                    package.VBPointer->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
        //                    if (sec.FaceNormals)
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
        //                    else
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
        //                }
        //                package.VBPointer->Color = white;
        //                package.VBPointer++;

        //                // Second triangle
        //                package.VBPointer->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
        //                package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                package.VBPointer->Color = white;
        //                package.VBPointer++;

        //                if (i + 1 < contourLen)
        //                {
        //                    package.VBPointer->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
        //                    if (sec.FaceNormals)
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                    else
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
        //                    package.VBPointer->Color = white;
        //                    package.VBPointer++;

        //                    package.VBPointer->Position = new Vector3(0, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
        //                    if (sec.FaceNormals)
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                    else
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
        //                    package.VBPointer->Color = white;
        //                    package.VBPointer++;
        //                }
        //                else
        //                {
        //                    package.VBPointer->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
        //                    if (sec.FaceNormals)
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
        //                    else
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
        //                    package.VBPointer->Color = white;
        //                    package.VBPointer++;

        //                    package.VBPointer->Position = new Vector3(0, sec.Contour[0][0].Y, sec.Contour[0][0].X);
        //                    if (sec.FaceNormals)
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
        //                    else
        //                        package.VBPointer->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
        //                    package.VBPointer->Color = white;
        //                    package.VBPointer++;
        //                }
        //            }
        //            #endregion

        //            #region Draw end triangulated section
        //            for (int i = nIndices - 1; i >= 0; --i)
        //            {
        //                package.VBPointer->Position = new Vector3(1, sec.Contour[0][sectionIndices[i]].Y, sec.Contour[0][sectionIndices[i]].X);
        //                package.VBPointer->Normal = new Vector3(1, 0, 0);
        //                package.VBPointer->Color = white;
        //                package.VBPointer++;
        //            }
        //            #endregion
        //        }

        //        //theVB.Unlock();
        //        //vbBase += vertsSize;
        //        gadgetManager.ReleaseVertexBuffer(totalVertices, ResourceStreamType.TriangleListPositionNormalColored, false);

        //        GadgetLocator locator;
        //        locator.Offset = package.Offset;
        //        locator.Size = totalVertices;
        //        sections.Add(sec, locator);
        //    }

        //    device.DrawPrimitives(PrimitiveType.TriangleList, sections[sec].Offset, sections[sec].Size / 3);
        //    device.DrawIndexedPrimitives 
        //}
        #endregion

        #region Legacy
        // to do 
        //void oldRenderer()
        //{
        //    foreach (LineElement l in lines)
        //    {
        //        if (l != null && l.IsVisible)
        //        {
        //            if (l.Properties is StraightFrameProps)
        //            {
        //                FrameSection sec = ((StraightFrameProps)l.Properties).Section;

        #region GadgetRenderer Code (Moved Code) (Orientation + Material)
        //                l.RotationMatrix(out rotMatrix);

        //                // rotMatrix * Matrix.Translation(l.I.Position);
        //                rotMatrix.M41 = l.I.Position.X;
        //                rotMatrix.M42 = l.I.Position.Y;
        //                rotMatrix.M43 = l.I.Position.Z;

        //                // Matrix.Scaling(len, 1, 1) * rotMatrix;
        //                float len = l.LengthInt;
        //                rotMatrix.M11 *= len;
        //                rotMatrix.M12 *= len;
        //                rotMatrix.M13 *= len;

        //                device.Transform.World = rotMatrix;

        //                // Select material
        //                if (pickingMode)
        //                {
        //                    pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
        //                    pickMtrl.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
        //                    device.Material = pickMtrl;
        //                }
        //                else
        //                {
        //                    if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
        //                    {
        //                        if (l.IsSelected)
        //                        {
        //                            if (lastMaterial != 1) device.Material = sectionMaterials[1];
        //                            lastMaterial = 1;
        //                        }
        //                        else
        //                        {
        //                            if (lastMaterial != 0) device.Material = sectionMaterials[0];
        //                            lastMaterial = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (l.IsSelected)
        //                        {
        //                            if (lastMaterial != 3) device.Material = sectionMaterials[3];
        //                            lastMaterial = 3;
        //                        }
        //                        else
        //                        {
        //                            if (lastMaterial != 2) device.Material = sectionMaterials[2];
        //                            lastMaterial = 2;
        //                        }
        //                    }
        //                }
        #endregion


        //                if (!sectionDictionary.ContainsKey(sec))
        //                {
        //                    // Lock the VertexBuffer and obtain the unsafe pointer from the new lock
        //                    vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
        //                    vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
        //                    vertsSize = 0;

        //                    // Get triangulated section indices
        //                    int[] sectionIndices = sec.TriangSectionIndices;
        //                    int nIndices = sectionIndices.Length;

        //                    #region Draw initial triangulated section
        //                    for (int i = 0; i < nIndices; ++i)
        //                    {
        //                        vbArray->Position = new Vector3(0, sec.Contour[0][sectionIndices[i]].Y, sec.Contour[0][sectionIndices[i]].X);
        //                        vbArray->Normal = new Vector3(-1, 0, 0);
        //                        vbArray++;
        //                        vertsSize++;
        //                    }
        //                    #endregion

        //                    #region Draw beam
        //                    int contourLen = sec.Contour[0].Length;
        //                    for (int i = 0; i < contourLen; i++)
        //                    {
        //                        // First triangle
        //                        vbArray->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
        //                        vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                        vbArray++;
        //                        vertsSize++;

        //                        vbArray->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
        //                        vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                        vbArray++;
        //                        vertsSize++;

        //                        if (i + 1 < contourLen)
        //                        {
        //                            vbArray->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
        //                            if (sec.FaceNormals)
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                            else
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
        //                        }
        //                        else
        //                        {
        //                            vbArray->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
        //                            if (sec.FaceNormals)
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
        //                            else
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
        //                        }
        //                        vbArray++;
        //                        vertsSize++;

        //                        // Second triangle
        //                        vbArray->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
        //                        vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                        vbArray++;
        //                        vertsSize++;

        //                        if (i + 1 < contourLen)
        //                        {
        //                            vbArray->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
        //                            if (sec.FaceNormals)
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                            else
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
        //                            vbArray++;
        //                            vertsSize++;

        //                            vbArray->Position = new Vector3(0, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
        //                            if (sec.FaceNormals)
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
        //                            else
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
        //                            vbArray++;
        //                            vertsSize++;
        //                        }
        //                        else
        //                        {
        //                            vbArray->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
        //                            if (sec.FaceNormals)
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
        //                            else
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
        //                            vbArray++;
        //                            vertsSize++;

        //                            vbArray->Position = new Vector3(0, sec.Contour[0][0].Y, sec.Contour[0][0].X);
        //                            if (sec.FaceNormals)
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
        //                            else
        //                                vbArray->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
        //                            vbArray++;
        //                            vertsSize++;
        //                        }
        //                    }
        //                    #endregion

        //                    #region Draw end triangulated section
        //                    for (int i = nIndices - 1; i >= 0; --i)
        //                    {
        //                        vbArray->Position = new Vector3(1, sec.Contour[0][sectionIndices[i]].Y, sec.Contour[0][sectionIndices[i]].X);
        //                        vbArray->Normal = new Vector3(1, 0, 0);
        //                        vbArray++;
        //                        vertsSize++;
        //                    }
        //                    #endregion

        //                    theVB.Unlock();

        //                    SectionLocator locator;
        //                    locator.offset = vbBase;
        //                    locator.size = vertsSize;
        //                    sectionDictionary.Add(sec, locator);

        //                    vbBase += vertsSize;
        //                }



        //                device.DrawPrimitives(PrimitiveType.TriangleList, sectionDictionary[sec].offset, sectionDictionary[sec].size / 3);
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Sections Legacy
        ///// <summary>
        ///// Retrieves the main vertex buffer used as a generic holder for vertices 
        ///// by the Item Renderers (JointRenderer, LineRenderer, AreaRenderer, etc)
        ///// </summary>
        //public VertexBuffer SectionsVB
        //{
        //    get
        //    {
        //        if (sectionsVbIndex == -1)
        //            updateSectionsVB();

        //        return this[sectionsVbIndex];
        //    }
        //}

        //private void updateSectionsVB()
        //{
        //    sectionsVbBase = 0;

        //    if ((sectionsVbIndex >= 0) && (sectionsVbIndex < Count) && (this[sectionsVbIndex] != null))
        //        DelVB(sectionsVbIndex);

        //    // En caso de que el device se haya reseteado, reconstruir buffer
        //    try
        //    {
        //        sectionsVbIndex = AddVB(typeof(CustomVertex.PositionNormal), sectionsVbSize + sectionsVbFlush,
        //                                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormal.Format, Pool.Default);
        //    }
        //    catch (OutOfMemoryException e)
        //    {
        //        // Si no hubo memoria suficiente tratar con la mitad recursivamente
        //        if (sectionsVbSize > 10000)
        //        {
        //            sectionsVbSize = sectionsVbSize / 2;
        //            sectionsVbFlush = sectionsVbFlush / 2;
        //            updateSectionsVB();
        //        }
        //        else
        //            throw e;    // Si es muy poca la memoria el programa no puede funcionar
        //    }
        //}

        //public int SectionsVbBase
        //{
        //    get
        //    {
        //        return sectionsVbBase;
        //    }
        //    set
        //    {
        //        if ((value < 0) || (value > sectionsVbSize))
        //            sectionsVbBase = 0;
        //        else
        //            sectionsVbBase = value;
        //    }
        //}

        //public int SectionsVbFlush
        //{
        //    get
        //    {
        //        return sectionsVbFlush;
        //    }
        //    set
        //    {
        //        if ((value < 1) || (value > sectionsVbSize))
        //            sectionsVbFlush = 50;
        //        else
        //            sectionsVbFlush = value;
        //    }
        //}

        //public int SectionsVbSize
        //{
        //    get
        //    {
        //        return sectionsVbSize;
        //    }
        //    set
        //    {
        //        if (value < 1)
        //            sectionsVbSize = 40000;
        //        else
        //            sectionsVbSize = value;
        //    }
        //}

        //public IndexBuffer SectionsIB
        //{
        //    get
        //    {
        //        if (sectionsIbIndex == -1)
        //            updateSectionsIB();

        //        return indexBuffers[sectionsIbIndex];
        //    }
        //}

        //private void updateSectionsIB()
        //{
        //    sectionsIbBase = 0;

        //    if ((sectionsIbIndex >= 0) && (sectionsIbIndex < Count) && (this[sectionsIbIndex] != null))
        //        DelIB(sectionsIbIndex);

        //    // En caso de que el device se haya reseteado, reconstruir buffer
        //    try
        //    {
        //        sectionsIbIndex = AddIB(typeof(int), sectionsIbSize + sectionsIbFlush, Usage.Dynamic | Usage.WriteOnly, Pool.Default);
        //    }
        //    catch (OutOfMemoryException e)
        //    {
        //        // Si no hubo memoria suficiente tratar con la mitad recursivamente
        //        if (sectionsIbSize > 10000)
        //        {
        //            sectionsIbSize = sectionsIbSize / 2;
        //            sectionsIbFlush = sectionsIbFlush / 2;
        //            updateSectionsIB();
        //        }
        //        else
        //            throw e;    // Si es muy poca la memoria el programa no puede funcionar
        //    }
        //}

        //public int SectionsIbBase
        //{
        //    get
        //    {
        //        return sectionsIbBase;
        //    }
        //    set
        //    {
        //        if ((value < 0) || (value > sectionsIbSize))
        //            sectionsIbBase = 0;
        //        else
        //            sectionsIbBase = value;
        //    }
        //}

        //public int SectionsIbFlush
        //{
        //    get
        //    {
        //        return sectionsIbFlush;
        //    }
        //    set
        //    {
        //        if ((value < 1) || (value > sectionsIbSize))
        //            sectionsIbFlush = 50;
        //        else
        //            sectionsIbFlush = value;
        //    }
        //}

        //public int SectionsIbSize
        //{
        //    get
        //    {
        //        return sectionsIbSize;
        //    }
        //    set
        //    {
        //        if (value < 1)
        //            sectionsIbSize = 40000;
        //        else
        //            sectionsIbSize = value;
        //    }
        //}

        #endregion
        #endregion


    }
}
