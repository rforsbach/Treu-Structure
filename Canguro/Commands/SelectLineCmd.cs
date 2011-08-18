using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    public class SelectLineCmd : ModelCommand
    {
        private const float minAngle = 0.79f;

        public override void Run(Canguro.Controller.CommandServices services)
        {
            services.StoreSelection();
            LineElement line = services.GetLine();
            List<LinkedList<LineElement>> graph = GetLineGraph(services.Model);
            ItemList<Joint> joints = services.Model.JointList;
            int numJoints = joints.Count;
            bool[] colors = new bool[numJoints];

            Stack<LineElement> stack = new Stack<LineElement>();
            stack.Push(line);
            while (stack.Count > 0)
            {
                line = stack.Pop();
                line.IsSelected = true;
                if (!colors[line.I.Id])
                {
                    line.I.IsSelected = true;
                    visit(graph, (int)line.I.Id, stack, line);
                    colors[line.I.Id] = true;
                }
                if (!colors[line.J.Id])
                {
                    line.J.IsSelected = true;
                    visit(graph, (int)line.J.Id, stack, line);
                    colors[line.J.Id] = true;
                }
            }

            services.RestoreSelection();

            services.Model.ChangeSelection(null);
        }

        private static void visit(List<LinkedList<LineElement>> graph, int jid, Stack<LineElement> stack, LineElement line)
        {
            LineElement minLine = null;
            float min = (float)Math.Cos(minAngle);
            if (graph[jid] != null)
            {
                foreach (LineElement adj in graph[jid])
                {
                    if (adj != null && adj != line)
                    {
                        float ang = cosAngle(line, adj);
                        if (ang > min)
                        {
                            min = ang;
                            minLine = adj;
                        }
                    }
                }
                if (minLine != null)
                    stack.Push(minLine);
            }
        }

        private static float cosAngle(LineElement l1, LineElement l2)
        {
            bool negative = (l1.I.Id == l2.I.Id || l1.J.Id == l2.J.Id);
            Vector3 v1 = l1.I.Position - l1.J.Position;
            Vector3 v2 = l2.I.Position - l2.J.Position;
            v1.Normalize(); 
            v2.Normalize();
                
            return (negative) ? -Vector3.Dot(v1, v2) : Vector3.Dot(v1, v2);    
        }

        public static List<LinkedList<LineElement>> GetLineGraph(Canguro.Model.Model model)
        {
            List<LinkedList<LineElement>> list = new List<LinkedList<LineElement>>(model.JointList.Count);
            for (int i = 0; i < model.JointList.Count; i++)
                list.Add(null);

            foreach (LineElement element in model.LineList)
            {
                if (element != null && element.I != null && element.J != null)
                {
                    int i = (int)element.I.Id;
                    int j = (int)element.J.Id;
                    if (list[i] == null)
                        list[i] = new LinkedList<LineElement>();
                    if (list[j] == null)
                        list[j] = new LinkedList<LineElement>();
                    list[i].AddLast(element);
                    list[j].AddLast(element);
                }
            }

            return list;
        }
    }
}
