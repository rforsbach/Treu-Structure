using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Canguro.Analysis.Sections.Meshing
{
    public class ContourMerger
    {
        IList<IList<PointF>> contours;
        IList<Material> materials;

        public ContourMerger(IList<IList<PointF>> contours, IList<Material> materials)
        {
            this.contours = contours;
            this.materials = materials;
        }

        public List<Mesh> Merge2()
        {
            List<Mesh> meshes = new List<Mesh>();
            List<Mesh> list = new List<Mesh>();
            
            // Create Meshes (one per contour)
            for (int i = 0; i < contours.Count; i++)
                list.Add(GetMesh(contours[i], materials[i]));

            // Add vertices at points of intersection between contours
            for (int i = 0; i < list.Count; i++)
                for (int j = i; j < list.Count; j++)
                    AddIntersections(list[i], list[j]);

            #region Get global bounding box
            PointF[] gBoundingBox = new PointF[3];
            gBoundingBox[0].X = float.MaxValue;
            gBoundingBox[0].Y = float.MaxValue;
            gBoundingBox[1].X = float.MinValue;
            gBoundingBox[1].Y = float.MinValue;
            foreach (Mesh m in list)
            {
                PointF[] bb = m.BoundingBox;
                gBoundingBox[0].X = (bb[0].X < gBoundingBox[0].X) ? bb[0].X : gBoundingBox[0].X;
                gBoundingBox[0].Y = (bb[0].Y < gBoundingBox[0].Y) ? bb[0].Y : gBoundingBox[0].Y;

                gBoundingBox[1].X = (bb[1].X > gBoundingBox[1].X) ? bb[1].X : gBoundingBox[1].X;
                gBoundingBox[1].Y = (bb[1].Y > gBoundingBox[1].Y) ? bb[1].Y : gBoundingBox[1].Y;

                gBoundingBox[0].X = (bb[1].X < gBoundingBox[0].X) ? bb[1].X : gBoundingBox[0].X;
                gBoundingBox[0].Y = (bb[1].Y < gBoundingBox[0].Y) ? bb[1].Y : gBoundingBox[0].Y;

                gBoundingBox[1].X = (bb[0].X > gBoundingBox[1].X) ? bb[0].X : gBoundingBox[1].X;
                gBoundingBox[1].Y = (bb[0].Y > gBoundingBox[1].Y) ? bb[0].Y : gBoundingBox[1].Y;
            }

            gBoundingBox[2].X = gBoundingBox[1].X - gBoundingBox[0].X;
            gBoundingBox[2].Y = gBoundingBox[1].Y - gBoundingBox[0].Y;
            #endregion
            
            // Join Meshes
            double joinTolerance = 1e-4 * Math.Max(gBoundingBox[2].X, gBoundingBox[2].Y);
            foreach (Mesh m in list)
                m.Join(joinTolerance);

            #region Get min distance between any 2 vertices and between any vertices and edges
            double minDist = double.MaxValue;
            // min distance between vertices
            for (int i = 0; i < list.Count; i++)
                for (int j = i; j < list.Count; j++)
                {
                    foreach (Vertex v1 in list[i].Vertices)
                        foreach (Vertex v2 in list[j].Vertices)
                        {
                            if (!v1.Equals(v2))
                            {
                                double distSqr = (v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y);
                                minDist = (distSqr < minDist) ? distSqr : minDist;
                            }
                        }
                }

            // min distance between edges and vertices
            for (int i = 0; i < list.Count; i++)
                for (int j = 0; j < list.Count; j++)
                {
                    foreach (Vertex v in list[i].Vertices)
                        foreach (Edge e in list[j].Edges)
                        {
                            if (!v.Equals(e.V1) && !v.Equals(e.V2))
                            {
                                double t, s;
                                double dx = e.V2.Y - e.V1.Y, dy = e.V1.X - e.V2.X;
                                Triangulator.IntersectionResult res = Triangulator.LineSegmentsIntersection(e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, v.X - dx, v.Y - dy, v.X + dx, v.Y + dy, joinTolerance, out t, out s);

                                if (res == Triangulator.IntersectionResult.Intersect && (s < 0.5 - joinTolerance || s > 0.5 + joinTolerance))
                                {
                                    double ex = e.V1.X + t * (e.V2.X - e.V1.X), ey = e.V1.Y + t * (e.V2.Y - e.V1.Y);
                                    double distSqr = (ex - v.X) * (ex - v.X) + (ey - v.Y) * (ey - v.Y);
                                    minDist = (distSqr > joinTolerance && distSqr < minDist) ? distSqr : minDist;
                                }
                            }
                        }
                }

            if (minDist == double.MaxValue || minDist < Triangulator.IntersectionEpsilon)
                return list;
            minDist = Math.Sqrt(minDist);
            #endregion            

            #region Build bitmap
            float minDistPixels = 7;
            int imgWidth = (int)(gBoundingBox[2].X * minDistPixels / minDist) + 2;
            int imgHeight = (int)(gBoundingBox[2].Y * minDistPixels / minDist) + 2;
            Bitmap bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);            
            Image img = Image.FromHbitmap(bmp.GetHbitmap(Color.FromArgb(0)));
            bmp.Dispose();
            bmp = null;
            Graphics graphic = Graphics.FromImage(img);
            graphic.FillRectangle(Brushes.Black, 0, 0, imgWidth, imgHeight);
            Color backGround = Color.FromArgb(0);
            Color backGround2 = Color.FromArgb(255, 0, 0, 1);
            SolidBrush brush = new SolidBrush(backGround2);
            int verticesStartColorArgb = 2 + list.Count;
            int alphaMask = 0xFF << 24;
            SolidBrush nodeBrush = new SolidBrush(Color.FromArgb(verticesStartColorArgb));
            List<Vertex> globalVertices = new List<Vertex>();
            #endregion

            #region Draw each mesh in the graphic
            int meshIndex = 2;
            foreach (Mesh m in list)
            {
                // Get contour (Euler cycle)
                LinkedList<Vertex> con = m.GetContourFromVertex(0);

                brush.Color = Color.FromArgb(255, Color.FromArgb(meshIndex++));
                PaintContour(con, brush, gBoundingBox, graphic);
                foreach (Vertex v in con)
                {
                    globalVertices.Add(v);
                    nodeBrush.Color = Color.FromArgb(255, Color.FromArgb(globalVertices.Count - 1 + verticesStartColorArgb));
                    PaintPoint(v, nodeBrush, gBoundingBox, graphic);
                }
            }
            img.Save(@"c:\contours2.png", System.Drawing.Imaging.ImageFormat.Png);
            #endregion

            #region Get regions using Flood Fill
            bmp = new Bitmap(img);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Fill regions and obtain new contour vertices
            List<List<int>> regions = new List<List<int>>();
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                {
                    int pixelArgb = getPixel(bmpData.Scan0, bmpData.Stride / 4, i, j); // bmp.GetPixel(i, j).ToArgb();
                    if (pixelArgb > alphaMask + 1 && pixelArgb <= brush.Color.ToArgb())
                    {
                        regions.Add(getRegionContour(bmpData, i, j, pixelArgb, backGround2.ToArgb(), alphaMask | verticesStartColorArgb));
                        setBGColor(bmpData, backGround2.ToArgb(), backGround.ToArgb());
                    }
                }

            bmp.UnlockBits(bmpData);
            //bmp.Save(@"c:\contours3.png", System.Drawing.Imaging.ImageFormat.Png);
            #endregion

            #region Build new meshes
            foreach (List<int> contour in regions)
            {
                List<PointF> points = new List<PointF>();
                foreach (int p in contour)
                    points.Add(new PointF((float)globalVertices[p].X, (float)globalVertices[p].Y));

                meshes.Add(GetMesh(points, Material.Steel));
            }
            #endregion

            return meshes;
        }

        private unsafe int getPixel(IntPtr scan0, int stride, int x, int y)
        {
            return *(((int*)scan0) + (y * stride) + x);
        }

        private unsafe void setPixel(IntPtr scan0, int stride, int x, int y, int newColor)
        {
            *(((int*)scan0) + (y * stride) + x) = newColor;
        }

        private void setBGColor(System.Drawing.Imaging.BitmapData bmpData, int backGroundOld, int backGroundNew)
        {
            for (int i = 0; i < bmpData.Width; i++)
                for (int j = 0; j < bmpData.Height; j++)
                    if (getPixel(bmpData.Scan0, bmpData.Stride / 4, i, j) /*bmp.GetPixel(i, j).ToArgb()*/ == backGroundOld)
                        setPixel(bmpData.Scan0, bmpData.Stride / 4, i, j, backGroundNew); // bmp.SetPixel(i, j, backGroundNew);
        }

        private enum WalkPosition
        {
            Center = 0,
            RightCenter, LeftCenter, 
            MiddleTop, MiddleBottom, 
            LeftTop, RightBottom,
            LeftBottom, RightTop            
        }

        private List<int> getRegionContour(System.Drawing.Imaging.BitmapData bmpData, int x, int y, int oldArgb, int newArgb, int verticesStartColor)
        {
            #region Variables
            int w = bmpData.Width;
            int h = bmpData.Height;
            int[,] border = new int[w, h];
            int pixelArgb;
            IntPtr bmpScan0 = bmpData.Scan0;
            int bmpStride = bmpData.Stride / 4;

            if (oldArgb == newArgb) return null;
            
            List<int> vertices = new List<int>();
            Stack<Point> points = new Stack<Point>();

            Point[] deltas = new Point[9];
            deltas[(int)WalkPosition.Center].X = 0; deltas[(int)WalkPosition.Center].Y = 0;
            deltas[(int)WalkPosition.RightCenter].X = 1; deltas[(int)WalkPosition.RightCenter].Y = 0;
            deltas[(int)WalkPosition.LeftCenter].X = -1; deltas[(int)WalkPosition.LeftCenter].Y = 0;
            deltas[(int)WalkPosition.MiddleTop].X = 0; deltas[(int)WalkPosition.MiddleTop].Y = -1;
            deltas[(int)WalkPosition.MiddleBottom].X = 0; deltas[(int)WalkPosition.MiddleBottom].Y = 1;
            deltas[(int)WalkPosition.LeftTop].X = -1; deltas[(int)WalkPosition.LeftTop].Y = -1;
            deltas[(int)WalkPosition.RightBottom].X = 1; deltas[(int)WalkPosition.RightBottom].Y = 1;
            deltas[(int)WalkPosition.LeftBottom].X = -1; deltas[(int)WalkPosition.LeftBottom].Y = 1;
            deltas[(int)WalkPosition.RightTop].X = 1; deltas[(int)WalkPosition.RightTop].Y = -1;
            #endregion

            #region Find region border and vertices (Flood fill)
            points.Push(new Point(x, y));
            while (points.Count > 0)
            {
                Point p = points.Pop();
                x = p.X; y = p.Y;

                //bmp.SetPixel(x, y, newColor);
                setPixel(bmpScan0, bmpStride, x, y, newArgb);

                //pixelArgb = (x + 1 < w) ? bmp.GetPixel(x + 1, y).ToArgb() : 0;
                pixelArgb = (x + 1 < w) ?  getPixel(bmpScan0, bmpStride, x + 1, y) : 0;
                if (x + 1 < w && pixelArgb == oldArgb)
                    points.Push(new Point(x + 1, y));
                else if (x + 1 < w && pixelArgb != 0 && pixelArgb != newArgb && pixelArgb >= verticesStartColor)
                    border[x + 1, y] = pixelArgb - verticesStartColor + 1;
                else if (x + 1 == w || pixelArgb != newArgb) // Edge
                    border[x, y] = -1;

                //pixelArgb = (x - 1 >= 0) ? bmp.GetPixel(x - 1, y).ToArgb() : 0;
                pixelArgb = (x - 1 >= 0) ? getPixel(bmpScan0, bmpStride, x - 1, y) : 0;
                if (x - 1 >= 0 && pixelArgb == oldArgb)
                    points.Push(new Point(x - 1, y));
                else if (x - 1 >= 0 && pixelArgb != 0 && pixelArgb != newArgb && pixelArgb >= verticesStartColor)
                    border[x - 1, y] = pixelArgb - verticesStartColor + 1;
                else if (x - 1 == -1 || pixelArgb != newArgb) // Edge
                    border[x, y] = -1;

                //pixelArgb = (y + 1 < h) ? bmp.GetPixel(x, y + 1).ToArgb() : 0;
                pixelArgb = (y + 1 < h) ? getPixel(bmpScan0, bmpStride, x, y + 1) : 0;
                if (y + 1 < h && pixelArgb == oldArgb)
                    points.Push(new Point(x, y + 1));
                else if (y + 1 < h && pixelArgb != 0 && pixelArgb != newArgb && pixelArgb >= verticesStartColor)
                    border[x, y + 1] = pixelArgb - verticesStartColor + 1;
                else if (y + 1 == h || pixelArgb != newArgb) // Edge
                    border[x, y] = -1;

                //pixelArgb = (y - 1 >= 0) ? bmp.GetPixel(x, y - 1).ToArgb() : 0;
                pixelArgb = (y - 1 >= 0) ? getPixel(bmpScan0, bmpStride, x, y - 1) : 0;
                if (y - 1 >= 0 && pixelArgb == oldArgb)
                    points.Push(new Point(x, y - 1));
                else if (y - 1 >= 0 && pixelArgb != 0 && pixelArgb != newArgb && pixelArgb >= verticesStartColor)
                    border[x, y - 1] = pixelArgb - verticesStartColor + 1;
                else if (y - 1 == -1 || pixelArgb != newArgb) // Edge
                    border[x, y] = -1;
            }
            #endregion

            #region Find first vertex pixel in border array
            x = y = -1;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++) 
                    if (border[i, j] > 0)
                    {
                        x = i; y = j;
                        i = w; j = h; // Exit the loops
                    }
            #endregion

            #region Walk on the contour finding vertices
            int oldx = x + 1, oldy = y;
            WalkPosition lastWalkPos = WalkPosition.Center, walkPos = WalkPosition.Center;
            float priority = float.MaxValue, lastPriority;
            PointF dir = new PointF(), unitDir = new PointF();
            List<Point> vertexAdj = new List<Point>();
            bool eatingVertex = false;

            // Save border to bitmap for debugging
            #if DEBUG
            using (Bitmap bmpTmp = new Bitmap(border.GetLength(0), border.GetLength(1)))
            {
                for (int i = 0; i < border.GetLength(0); i++)
                    for (int j = 0; j < border.GetLength(1); j++)
                        bmpTmp.SetPixel(i, j, Color.FromArgb(255, Color.FromArgb(border[i, j])));
                bmpTmp.Save(@"c:\contours4.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            int ppp = 0;
            #endif

            while (x != oldx || y != oldy)
            {
                if (border[x, y] > 0)
                {
                    if (vertices.Count == 0 || vertices[vertices.Count - 1] != border[x, y] - 1)
                    {
                        vertices.Add(border[x, y] - 1);
                        vertexAdj.Clear();
                        eatingVertex = true;
                    }

                    dir.X = 0; dir.Y = 0;
                }
                else if (eatingVertex)
                {
                    // Find best adjacency and move there
                    int maxPriority = border[x, y];
                    foreach (Point p in vertexAdj)
                        if (border[p.X, p.Y] > maxPriority)
                        {                            
                            x = p.X; y = p.Y;
                        }

                    // Stop eating vertex pixels
                    eatingVertex = false;
                }

                lastPriority = priority;
                priority = int.MinValue;

                if (border[x, y] > 0)
                    border[x, y] = -10;
                else
                    border[x, y]--;

                oldx = x; oldy = y;
                
                lastWalkPos = walkPos;
                walkPos = WalkPosition.Center;

                // Get unitary dir
                float len = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
                unitDir.X = dir.X;
                unitDir.Y = dir.Y;
                if (len > 0)
                {
                    unitDir.X /= len;
                    unitDir.Y /= len;
                }
                
                // Get one step walk direction (if any)
                testWalk(deltas, x, y, w, h, border, WalkPosition.RightCenter, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.LeftCenter, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.MiddleBottom, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.MiddleTop, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.RightBottom, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.RightTop, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.LeftBottom, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);
                testWalk(deltas, x, y, w, h, border, WalkPosition.LeftTop, unitDir, ref walkPos, ref priority, vertexAdj, eatingVertex);

                // If moving backwards is not helping, stop loop
                if (priority < 0 && lastPriority < 0 && Math.Floor(priority) < Math.Floor(lastPriority))
                    break;
                
                // Walk
                switch (walkPos)
                {
                    case WalkPosition.RightCenter:
                        x++; break;
                    case WalkPosition.LeftCenter:
                        x--; break;
                    case WalkPosition.MiddleBottom:
                        y++; break;
                    case WalkPosition.MiddleTop:
                        y--; break;
                    case WalkPosition.RightBottom:
                        x++; y++; break;
                    case WalkPosition.RightTop:
                        x++; y--; break;
                    case WalkPosition.LeftBottom:
                        x--; y++; break;
                    case WalkPosition.LeftTop:
                        x--; y--; break;
                    default:
                        break;
                }

                // Update dir
                dir.X += deltas[(int)walkPos].X; dir.Y += deltas[(int)walkPos].Y;
            }
            #endregion

            return vertices;
        }

        private void testWalk(Point[] deltas, int x, int y, int w, int h, int[,] border, WalkPosition testWp, PointF unitaryDir, ref WalkPosition walkPos, ref float priority, List<Point> vertexAdj, bool eatingVertex)
        {
            float dx = deltas[(int)testWp].X, dy = deltas[(int)testWp].Y;
            if (dx != 0 && dy != 0)
            {
                dx *= 0.5f; dy *= 0.5f;
            }
            int newx = x + deltas[(int)testWp].X, newy = y + deltas[(int)testWp].Y;
            if (newx < w && newx >= 0 && newy < h && newy >= 0 && border[newx, newy] != 0)
            {
                float deltaPriority = (border[newx, newy] < 0) ? Math.Max(0f, (unitaryDir.X * dx + unitaryDir.Y * dy) / 10f) : 0;
                if (border[newx, newy] + deltaPriority > priority)
                {
                    walkPos = testWp;
                    priority = border[newx, newy] + deltaPriority;
                }

                if (eatingVertex && border[newx, newy] < 0)
                    vertexAdj.Add(new Point(newx, newy));
            }
        }

        public void PaintContour(LinkedList<Vertex> con, Brush brush, PointF[] gBoundingBox, Graphics g)
        {           
            PointF[] points = new PointF[con.Count];
            RectangleF rect = g.VisibleClipBounds;
            int i = 0;
            foreach (Vertex v in con)
                points[i++] = new PointF((((float)v.X) - gBoundingBox[0].X) * (rect.Width-2) / gBoundingBox[2].X, (((float)v.Y) - gBoundingBox[0].Y) * (rect.Height-2) / gBoundingBox[2].Y);

            if (points.Length > 2)
            {
                g.FillPolygon(brush, points);
                g.DrawPolygon(new Pen(brush), points);
            }
        }

        public void PaintPoint(Vertex v, Brush brush, PointF[] gBoundingBox, Graphics g)
        {
            RectangleF rect = g.VisibleClipBounds;
            System.Drawing.PointF pos = new PointF((((float)v.X) - gBoundingBox[0].X) * (rect.Width-2) / gBoundingBox[2].X, (((float)v.Y) - gBoundingBox[0].Y) * (rect.Height -2)/ gBoundingBox[2].Y);
            g.FillRectangle(brush, pos.X - 1, pos.Y - 1, 3, 3);
            
        }

        public List<Mesh> Merge()
        {
            List<Mesh> meshes = new List<Mesh>();
            List<Mesh> list = new List<Mesh>();

            // Create Meshes (one per contour)
            for (int i = 0; i < contours.Count; i++)
                list.Add(GetMesh(contours[i], materials[i]));

            // Add vertices at points of intersection between contours
            for (int i = 0; i < list.Count; i++)
                for (int j = i; j < list.Count; j++)
                    AddIntersections(list[i], list[j]);

            // Join Meshes
            foreach (Mesh m in list)
                m.Join(Triangulator.IntersectionEpsilon);

            // Split if Meshes self intersect            
            for (int i = 0; i < list.Count; i++)
            {
                List<Mesh> newMeshes = new List<Mesh>();
                splitSelfIntersectigMesh(list[i], newMeshes);

                foreach (Mesh m in newMeshes)
                    meshes.Add(m);
            }

            for (int i = 0; i < meshes.Count; i++)
            {
                Mesh m1 = meshes[i];

                bool skipLoop = false;
                List<Edge> bEdges1 = m1.GetEdgeList();
                for (int j = meshes.Count - 1; j > i; j--)
                {
                    Mesh m2 = meshes[j];
                    List<Vertex> interesections = new List<Vertex>();
                    Dictionary<Vertex, Vertex> oldNew = new Dictionary<Vertex, Vertex>();
                    List<Edge> bEdges2 = m2.GetEdgeList();

                    #region Get Intersections
                    // Check if vertices are equal (if so, mark them as intersections)
                    foreach (Vertex u in m1.Vertices)
                        foreach (Vertex v in m2.Vertices)
                            if (u.Equals(v))
                                interesections.Add(v);
                    #endregion

                    #region Contention search (Adds holes to meshes if other meshes are inside)
                    // If there are no intersections, it means one contour is either completely inside
                    // or completely outside the other. So check for contention
                    if (interesections.Count == 0)
                    {
                        // if inside, delete, else leave.
                        Vertex v1 = m1.Vertices[0];
                        Vertex v2 = m2.Vertices[0];
                        bool m1InsideM2, m2InsideM1;
                        if ((m2InsideM1 = Triangulator.PointInsidePolygon(new PointF((float)v2.X, (float)v2.Y), bEdges1, m1.BoundingBox)) ||
                            (m1InsideM2 = Triangulator.PointInsidePolygon(new PointF((float)v1.X, (float)v1.Y), bEdges2, m2.BoundingBox)))
                        {
                            // If its inside and has the same material, delete inside contour
                            if (m1.Material == m2.Material)
                            {
                                if (m2InsideM1)
                                {
                                    meshes.RemoveAt(j);
                                    continue;
                                }
                                else
                                {
                                    meshes.RemoveAt(i--);
                                    skipLoop = true;
                                    break;
                                }
                            }
                            else // Add all
                            {
                                foreach (Vertex v in m2.Vertices)
                                {
                                    // Add hole vertices
                                    Vertex nv = m1.AddVertex(v.X, v.Y, v.IsBoundary, true);
                                    oldNew.Add(v, nv);
                                }

                                // Add hole edges
                                foreach (Edge edge in bEdges2)
                                    if (oldNew.ContainsKey(edge.V1) && oldNew.ContainsKey(edge.V2))
                                        m1.AddEdge(oldNew[edge.V1], oldNew[edge.V2], true); // Agrega boundary edges
                            }
                        }
                        continue;
                    } // if no intersections
                    #endregion

                    if (skipLoop) continue;
                    oldNew.Clear();

                    #region Join/Intersect Meshes (Contours).
                    foreach (Vertex v in m2.Vertices)
                    {
                        if (m1.Material == m2.Material)
                        {
                            // Unir 2 contornos del mismo material (agrega vertices)
                            if (interesections.Contains(v) ||
                                !Triangulator.PointInsidePolygon(new PointF((float)v.X, (float)v.Y), bEdges1, m1.BoundingBox))
                            {
                                Vertex nv = m1.AddVertex(v.X, v.Y, v.IsBoundary, true);
                                oldNew.Add(v, nv);
                            }
                        }

                        else
                        {
                            // Obtiene la interseccion de 2 contornos de distinto material (agrega vertices)
                            if (interesections.Contains(v) ||
                                Triangulator.PointInsidePolygon(new PointF((float)v.X, (float)v.Y), bEdges1, m1.BoundingBox))
                            {
                                Vertex nv = m1.AddVertex(v.X, v.Y, v.IsBoundary, true);
                                oldNew.Add(v, nv);
                            }
                        }
                    } // foreach v in m2

                    // Agrega edges de m2 en m1 resultados de la union o interseccion
                    foreach (Edge edge in bEdges2)
                        if (oldNew.ContainsKey(edge.V1) && oldNew.ContainsKey(edge.V2))
                            m1.AddEdge(oldNew[edge.V1], oldNew[edge.V2], true);

                    // Elimina edges internos que quedan de unir contornos
                    foreach (Edge edge in bEdges1)
                        if (EdgeInsidePolygon(edge, bEdges2, m2.BoundingBox))
                            m1.RemoveEdge(edge);

                    // Elimina vertices internos resultados de una union/interseccion
                    if (interesections.Count > 0)
                    {
                        foreach (Vertex v in m1.Vertices.ToArray())
                        {
                            if (!interesections.Contains(v) &&
                                Triangulator.PointInsidePolygon(new PointF((float)v.X, (float)v.Y), bEdges2, m2.BoundingBox))
                            {
                                m1.RemoveVertex(v);
                            }
                        }
                        if (m1.Material == m2.Material)
                            list.RemoveAt(j);
                    } // if mat equals
                    #endregion

                } // for m2

                foreach (Vertex v in m1.Vertices.ToArray())
                    if (v.Edges.Count > 2)
                        RemoveExtraEdges(m1, v);

            } // for m1
            return meshes;
        }

        private void splitSelfIntersectigMesh(Mesh mesh, List<Mesh> splittedMeshes)
        {
            if (mesh.Shapes.Count > 0)
                throw new InvalidOperationException("No Shapes allowed when joining Mesh");

            Vertex splitVertex = null;
            List<List<Edge>> paths = new List<List<Edge>>();

            // Find first vertex with a parity > 2
            foreach (Vertex v in mesh.Vertices)
                if (v.Edges.Count > 2)
                {
                    splitVertex = v;
                    List<Edge> vEdges = new List<Edge>(v.Edges);
                    while (vEdges.Count > 1)
                    {
                        int numEdges = vEdges.Count;
                        Edge last = vEdges[numEdges - 1];
                        List<Edge> path = getMeshPath(mesh, v, v, last);
                        paths.Add(path);                                                
                        vEdges.RemoveAt(numEdges - 1);
                        vEdges.Remove(path[path.Count - 1]);
                    }
                    break;
                }

            if (splitVertex != null)
            {
                // Build sub meshes
                Mesh[] ms = new Mesh[paths.Count];
                for (int i = 0; i < paths.Count; i++)
                {
                    ms[i] = new Mesh();
                    ms[i].Material = mesh.Material;
                    Dictionary<int, int> mapIDs = new Dictionary<int, int>();

                    foreach (Edge e in paths[i])
                    {
                        if (!mapIDs.ContainsKey(e.V1.Id))
                        {
                            Vertex v = ms[i].AddVertex(e.V1.X, e.V1.Y, e.V1.IsBoundary);
                            mapIDs.Add(e.V1.Id, v.Id);
                        }
                        if (!mapIDs.ContainsKey(e.V2.Id))
                        {
                            Vertex v = ms[i].AddVertex(e.V2.X, e.V2.Y, e.V2.IsBoundary);
                            mapIDs.Add(e.V2.Id, v.Id);
                        }
                        
                        mesh.AddEdge(ms[i].Vertices[mapIDs[e.V1.Id]], ms[i].Vertices[mapIDs[e.V2.Id]], e.IsBoundary);
                    }
                }

                // Split sub meshes
                foreach (Mesh m in ms)
                    splitSelfIntersectigMesh(m, splittedMeshes);
            }
            else
                splittedMeshes.Add(mesh);
        }

        private List<Edge> getMeshPath(Mesh mesh, Vertex start, Vertex end, Edge startEdge)
        {
            Dictionary<Edge, bool> visitedEdges = new Dictionary<Edge,bool>();
            List<Edge> path = new List<Edge>();
            path.Add(startEdge);
            visitedEdges.Add(startEdge, true);
            bool foundNewEdge = true;

            Vertex next = (startEdge.V1 == start) ? startEdge.V2 : startEdge.V1;
            while (next != end &&  foundNewEdge)
            {
                foundNewEdge = false;
                foreach (Edge e in next.Edges)
                    if (!visitedEdges.ContainsKey(e))
                    {
                        next = (e.V1 == next) ? e.V2 : e.V1;
                        path.Add(e);
                        visitedEdges.Add(e, true);
                        foundNewEdge = true;
                        break;
                    }
            }

            return path;
        }

        protected void AddIntersections(Mesh m1, Mesh m2)
        {
            List<Edge> edges1, edges2;
            edges1 = m1.GetEdgeList();
            edges2 = m2.GetEdgeList();

            double s, t;
            List<double>[] ts = new List<double>[edges1.Count];
            List<double>[] ss = new List<double>[edges2.Count];

            // Calculate intersections and store the parameters (ts, ss) of each one that apprears
            for (int i1 = edges1.Count - 1; i1 >= 0; i1--)
            {
                Edge e1 = edges1[i1];
                if (ts[i1] == null) ts[i1] = new List<double>();

                for (int i2 = edges2.Count - 1; i2 >= 0; i2--)
                {
                    Edge e2 = edges2[i2];
                    if (ss[i2] == null) ss[i2] = new List<double>();
                    
                    Triangulator.IntersectionResult result = Triangulator.LineSegmentsIntersection
                        (e1.V1.X, e1.V1.Y, e1.V2.X, e1.V2.Y, e2.V1.X, e2.V1.Y, e2.V2.X,e2.V2.Y, Triangulator.IntersectionEpsilon, out t, out s);

                    if ((result == Triangulator.IntersectionResult.Intersect || result == Triangulator.IntersectionResult.TooCloseToDecide)
                        && t >= 0-Triangulator.IntersectionEpsilon && t <= 1 + Triangulator.IntersectionEpsilon)
                    {
                        if (t > 0 + Triangulator.IntersectionEpsilon && t < 1 - Triangulator.IntersectionEpsilon)
                            ts[i1].Add(t);

                        if (s > 0 + Triangulator.IntersectionEpsilon && s < 1 - Triangulator.IntersectionEpsilon)
                            ss[i2].Add(s);
                    }
                }
            }

            // Split Edges according to ts and ss
            for (int i = 0; i < edges1.Count; i++)
                if (ts[i].Count > 0)
                {
                    ts[i].Sort();
                    m1.SplitEdge(edges1[i], ts[i]);
                }
            for (int i = 0; i < edges2.Count; i++)
                if (ss[i].Count > 0)
                {
                    ss[i].Sort();
                    m2.SplitEdge(edges2[i], ss[i]);
                }
        }

        protected Mesh GetMesh(IList<PointF> con, Material mat)
        {
            Mesh mesh = new Mesh();
            mesh.Material = mat;
            List<Vertex> vertices = mesh.Vertices;
            Vertex last = null, first = null;
            foreach (PointF p in con)
            {
                if (!vertices.Exists(delegate(Vertex ve) { return (p.X == ve.X && p.Y == ve.Y); }))
                {
                    Vertex v = mesh.AddVertex(p.X, p.Y, true);

                    if (last != null)
                        mesh.AddEdge(last, v, true);
                    else
                        first = v;

                    last = v;
                }
            }

            mesh.AddEdge(last, first, true);
            return mesh;
        }

        protected bool EdgeInsidePolygon(Edge e, List<Edge> polygon, PointF[] bb)
        {
            PointF pt = new PointF((float)(e.V1.X + e.V2.X) / 2f, (float)(e.V1.Y + e.V2.Y) / 2f);
            return Triangulator.PointInsidePolygon(pt, polygon, bb);
        }

        /// <summary>
        /// Look for erroneous edges and remove them. Happens when two edges that
        /// must be removed are one on top of the other thus difficulting the check
        /// in previous steps
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="v"></param>
        protected static void RemoveExtraEdges(Mesh mesh, Vertex v)
        {
            PointF[] bb = mesh.BoundingBox;
            List<Edge> pol = mesh.GetEdgeList();
            foreach (Edge e in v.Edges.ToArray())
            {
                float eps = 1.234543e-6f * (float)(e.V1.X - e.V2.X);
                float eps2 = 1.724542e-6f * (float)(e.V2.Y - e.V1.Y);
                // Get two points besides the edge.
                PointF pt1 = new PointF((float)(e.V1.X + e.V2.X) / 2f + eps2, (float)(e.V1.Y + e.V2.Y) / 2f + eps);
                PointF pt2 = new PointF((float)(e.V1.X + e.V2.X) / 2f - eps2, (float)(e.V1.Y + e.V2.Y) / 2f - eps);

                // If both points are inside the polygon, remove edge.
                if (Triangulator.PointInsidePolygon(pt1, pol, bb) && Triangulator.PointInsidePolygon(pt2, pol, bb))
                    mesh.RemoveEdge(e);
            }

            bool removed = true;
            while (removed)
            {
                removed = false;
                foreach (Vertex u in mesh.Vertices.ToArray())
                    if (u.Edges.Count < 2)
                    {
                        mesh.RemoveVertex(u);
                        removed = true;
                    }
            }
        }
    }
}
