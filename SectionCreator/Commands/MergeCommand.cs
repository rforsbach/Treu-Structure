using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Analysis.Sections;
using Canguro.Analysis.Sections.Meshing;
using System.Drawing;

namespace Canguro.SectionCreator.Commands
{
    class MergeCommand : RunnableCommand
    {
        protected override void Run()
        {
            List<Contour> contours = Model.Instance.Contours;
            IList<IList<PointF>> pConts = new List<IList<PointF>>();
            IList<Material> materials = new List<Material>();
            foreach (Contour con in contours)
            {
                IList<PointF> list = new List<PointF>();
                foreach (Point p in con.Points)
                    list.Add(p.Position);
                pConts.Add(list);
                materials.Add(con.Material);
            }

            for (int i = model.Contours.Count - 1; i >= 0; i--)
                model.Contours.RemoveAt(i);

            ContourMerger merger = new ContourMerger(pConts, materials);
            List<Mesh> meshes = merger.Merge2();

            List<List<System.Drawing.PointF>> newContours;
            foreach (Mesh mesh in meshes)
            {
                newContours = GetContours(mesh);
                Material material = mesh.Material;
                foreach (List<System.Drawing.PointF> con in newContours)
                {
                    Contour cont = new Contour();
                    cont.Material = material;
                    foreach (System.Drawing.PointF pt in con)
                        cont.Points.Add(new Point(pt));
                    model.Contours.Add(cont);
                }
            }
        }


        public static List<List<System.Drawing.PointF>> GetContours(Mesh mesh)
        {
            List<List<System.Drawing.PointF>> contours = new List<List<System.Drawing.PointF>>();
            List<Edge> allEdges = new List<Edge>();

            int[] parent = new int[mesh.Vertices.Count];
            for (int i = 0; i < parent.Length; i++)
                parent[i] = -1;

            Vertex next = mesh.Vertices[0];
            parent[0] = 0;

            bool finish = false;
            bool innerContour = false;
            while (!finish)
            {
                innerContour = (next != null && allEdges.Count > 0 && 
                    Triangulator.PointInsidePolygon(new PointF((float)next.X, (float)next.Y), allEdges, mesh.BoundingBox));

                List<System.Drawing.PointF> list = new List<System.Drawing.PointF>();
                while (next != null)
                {
                    Vertex u = next;
                    next = null;
                    list.Add(new System.Drawing.PointF((float)u.X, (float)u.Y));
                    if (u.Edges.Count > 2)
                        throw new Exception("Vertex " + u + " excedes 2 edges");
                    foreach (Edge e in u.Edges)
                    {
                        if (e.V1 == u && parent[e.V2.Id] == -1)
                        {
                            parent[e.V2.Id] = u.Id;
                            next = e.V2;
                            allEdges.Add(e);
                            break;
                        }
                        else if (e.V2 == u && parent[e.V1.Id] == -1)
                        {
                            parent[e.V1.Id] = u.Id;
                            next = e.V1;
                            allEdges.Add(e);
                            break;
                        }
                    }
                }
                if (!innerContour)
                    contours.Add(list);

                finish = true;
                for (int i = 0; i < parent.Length; i++)
                    if (parent[i] == -1)
                    {
                        next = mesh.Vertices[i];
                        parent[i] = i;
                        finish = false;
                        break;
                    }
            } // while !finish
            return contours;
        } // GetContours

    }
}
