using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class DeleteCommand : RunnableCommand
    {
        protected override void Run()
        {
            ManagedList<Contour> contours = Model.Instance.Contours;
            for (int c = contours.Count - 1; c >= 0; c--)
            {
                Contour con = contours[c];
                if (con.IsSelected)
                    contours.RemoveAt(c);
                else
                    for (int p = con.Points.Count - 1; p >= 0; p--)
                        if (con.Points[p].IsSelected)
                            con.Points.RemoveAt(p);
            }
        }
    }
}
