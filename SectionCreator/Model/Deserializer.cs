using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Canguro.SectionCreator
{
    class Deserializer
    {
        protected Model model;

        public Deserializer(Model model)
        {
            this.model = model;
        }

        internal void Deserialize(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode root = doc.SelectSingleNode("//sections");

            XmlNode node;

            node = root.SelectSingleNode("units");
            if (node != null)
                readUnits(node);

            node = root.SelectSingleNode("contours");
            if (node != null)
                readAllContours(node);
        }

        protected void readUnits(XmlNode node)
        {
            model.Unit = (LenghtUnits)Enum.Parse(typeof(LenghtUnits), readAttribute(node, "name"));
        }

        private void readAllContours(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("contour".Equals(child.Name))
                    readContour(child);
        }

        private void readContour(XmlNode node)
        {
            Contour con = new Contour();
            con.Name = readAttribute(node, "name");
            con.Material = (Canguro.Analysis.Sections.Material)Enum.Parse(typeof(Canguro.Analysis.Sections.Material), readAttribute(node, "material"));
            con.Color = System.Drawing.Color.FromArgb(int.Parse(readAttribute(node, "color", "0xCCCCCC")));
            node = node.SelectSingleNode("points");
            if (node != null)
            {
                readPoints(node, con.Points);
                model.Contours.Add(con);
            }
        }

        private void readPoints(XmlNode node, IList<Point> points)
        {
            foreach (XmlNode child in node.ChildNodes)
                if ("point".Equals(child.Name))
                {
                    double x = double.Parse(readAttribute(child, "x"));
                    double y = double.Parse(readAttribute(child, "y"));
                    Point p = new Point(x, y);
                    points.Add(p);
                }
        }

        internal static string readAttribute(XmlNode node, string attName)
        {
            return readAttribute(node, attName, "");
        }

        internal static string readAttribute(XmlNode node, string attName, string defaultValue)
        {
            if (node.Attributes != null && node.Attributes[attName] != null)
                return node.Attributes[attName].Value;
            return defaultValue;
        }
    }
}
