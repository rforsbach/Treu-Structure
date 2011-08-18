using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.DirectX;

namespace Canguro.Model.Section
{
    /// <summary>
    /// This class represents an instance of a section defined by a SectionTemplate and variables values.
    /// </summary>
    [Serializable]
    public class CustomSection : FrameSection
    {
        [NonSerialized]
        XmlNode rootNode;

        short[][] contourIndices = null;

        /// <summary>
        /// Constructor that gets all the data from a node and a template.
        /// </summary>
        /// <param name="template">The SectionTemplate with parameters and functions to build sections.</param>
        /// <param name="root">The Xml "section" node with variables and properties</param>
        public CustomSection(XmlNode root)
        {
            this.rootNode = root;

            UpdateData();
        }

        private void initContourIndices()
        {
            contourIndices = new short[4][];
            int numContourVertices = contour[0].Length;
            contourIndices[0] = new short[numContourVertices + 1];
            contourIndices[1] = new short[numContourVertices + 1];
            contourIndices[2] = new short[numContourVertices + 1];
            contourIndices[3] = new short[numContourVertices + 1];

            for (short i = 0; i < numContourVertices; i++)
            {
                contourIndices[0][i] = i;
                contourIndices[1][i] = i; 
                contourIndices[2][i] = i;
                contourIndices[3][i] = i;
            }

            contourIndices[0][numContourVertices] = 0;
            contourIndices[1][numContourVertices] = 0;
            contourIndices[2][numContourVertices] = 0;
            contourIndices[3][numContourVertices] = 0;

            calculateBB();
            calcLODSize();
        }

        /// <summary>
        /// Reads the Xml node and sets the shape, name, faceNormals and section properties from it.
        /// </summary>
        protected void UpdateData()
        {
            if (rootNode == null)
                return;
            initContour();
            //shape = template.Shape;
            //faceNormals = template.FaceNormals;
            this.Name = readAttribute(rootNode, "name", "sec");
            t3 = float.Parse(readAttribute(rootNode, "t3", "0"));
            t2 = float.Parse(readAttribute(rootNode, "t2", "0"));
            tf = float.Parse(readAttribute(rootNode, "tf", "0"));
            tw = float.Parse(readAttribute(rootNode, "tw", "0"));
            t2b = float.Parse(readAttribute(rootNode, "t2b", "0"));
            tfb = float.Parse(readAttribute(rootNode, "tfb", "0"));
            dis = float.Parse(readAttribute(rootNode, "dis", "0"));
            area = float.Parse(readAttribute(rootNode, "area", "0"));
            torsConst = float.Parse(readAttribute(rootNode, "torsConst", "0"));
            i33 = float.Parse(readAttribute(rootNode, "i33", "0"));
            i22 = float.Parse(readAttribute(rootNode, "i22", "0"));
            as2 = float.Parse(readAttribute(rootNode, "as2", "0"));
            as3 = float.Parse(readAttribute(rootNode, "as3", "0"));
            s33 = float.Parse(readAttribute(rootNode, "s33", "0"));
            s22 = float.Parse(readAttribute(rootNode, "s22", "0"));
            z33 = float.Parse(readAttribute(rootNode, "z33", "0"));
            z22 = float.Parse(readAttribute(rootNode, "z22", "0"));
            r33 = float.Parse(readAttribute(rootNode, "r33", "0"));
            r22 = float.Parse(readAttribute(rootNode, "r22", "0"));
        }

        /// <summary>
        /// Uses the SectionTemplate given in the constructor and the variable values to get a contour for the section.
        /// </summary>
        protected override void initContour()
        {
            if (rootNode == null)
                return;

            XmlNode pointsXml = rootNode.SelectSingleNode("//points");
            List<Vector2> points = new List<Vector2>();
            foreach (XmlNode pointXml in pointsXml.ChildNodes)
            {
                float x = float.Parse(readAttribute(pointXml, "x", "0"));
                float y = float.Parse(readAttribute(pointXml, "y", "0"));
                points.Add(new Microsoft.DirectX.Vector2(x, y));
            }

            Vector2[][] cont = new Vector2[2][];
            int numPoints = points.Count;
            cont[0] = points.ToArray();
            cont[1] = new Vector2[numPoints];


            //Dictionary<string, double> vars = new Dictionary<string, double>();
            //XmlNode varsXml = rootNode.SelectSingleNode("vars");
            //foreach (XmlNode node in varsXml.ChildNodes)
            //{
            //    if ("var".Equals(node.Name))
            //    {
            //        string id = readAttribute(node, "id", "0");
            //        string val = readAttribute(node, "value", "0");
            //        vars.Add(id.ToLower(), double.Parse(val));
            //    }
            //}
            //contour = template.GetContour(vars);

            initContourIndices();
            buildHighStressCover();
        }

        /// <summary>
        /// Should triangulate to fill the section
        /// </summary>
        protected override void  buildHighStressCover()
        {
            coverHighStress = new short[6 * 3];
        }

        /// <summary>
        /// Reads an attribute from an Xml node and returns a usable value
        /// </summary>
        /// <param name="node">The Xml node with the attribute</param>
        /// <param name="attName">The name of the attribute</param>
        /// <param name="defaultValue">A default value to return if the attribute is not found</param>
        /// <returns></returns>
        public static string readAttribute(XmlNode node, string attName, string defaultValue)
        {
            XmlAttribute ret = node.Attributes[attName];
            return (ret == null) ? defaultValue : ret.Value;
        }

        /// <summary>
        /// The sections shape, defined in the template
        /// </summary>
        protected string shape;
        public override string Shape
        {
            get { return shape; }
        }

        /// <summary>
        /// True if the defined normals are for faces (square), False if they're for vertices (round).
        /// </summary>
        protected bool faceNormals;
        public override bool FaceNormals
        {
            get { return faceNormals; }
        }

        public override short[][] ContourIndices
        {
            get { return contourIndices; }
        }
    }
}
