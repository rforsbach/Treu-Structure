using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class CopyCommand : RunnableCommand
    {
        protected override void Run()
        {
            List<Contour> selection = new List<Contour>();
            foreach (Contour con in model.Contours)
                if (con.IsSelected)
                    selection.Add(con);

            StringBuilder str = new StringBuilder();
            foreach (Contour con in selection)
            {
                foreach (Point p in con.Points)
                    str.AppendLine(p.X.ToString() + "," + p.Y.ToString());
                if (con.Points.Count > 0)
                {
                    Point p = con.Points[0];
                    str.AppendLine(p.X.ToString() + "," + p.Y.ToString());
                }
            }

            DataObject data = new DataObject();
            data.SetData("SectionCreator", selection);
            data.SetData(DataFormats.Text, str.ToString());
            Clipboard.SetDataObject(data);
        }
    }
}
