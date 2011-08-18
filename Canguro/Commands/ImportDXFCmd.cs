using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to import a Drawing Exchange Format file into the Model.
    /// Only Lines and PolyLines are supported.
    /// </summary>
    public class ImportDXFCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the Open File Dialog and imports the selected Drawing Wxchange Format file into the current Model.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            string path = "";
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Drawing Exchange Format (*.dxf)|*.dxf";
            dlg.DefaultExt = "dxf";
            dlg.AddExtension = true;
            dlg.Title = Culture.Get("ImportDXFTitle");
            if (services.Model.CurrentPath.Length > 0)
                dlg.FileName = Path.GetDirectoryName(services.Model.CurrentPath);
            dlg.CheckPathExists = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;
            Import(path, services.Model);
        }

        /// <summary>
        /// Reads a DXF file and adds all the lines defined in it to the given Model
        /// </summary>
        /// <param name="path">The path to the DXF file</param>
        /// <param name="model">The current Model object</param>
        public static void Import(string path, Canguro.Model.Model model)
        {
            if (path.Length > 0)
            {
                string[] file = File.ReadAllLines(path);
                List<string> search = new List<string>(new string[] { "LINE", "10", "20", "30", "11", "21", "31", "LWPOLYLINE" });
                int state = 0;
                Joint ji = null;
                Joint jj = null;
                StraightFrameProps props = new StraightFrameProps();
                List<LineElement> newLines = new List<LineElement>();
                List<AreaElement> newAreas = new List<AreaElement>();
                List<Joint> newJoints = new List<Joint>();

                bool addLine = false;
                bool polyline = false;
                for (int i = 0; i < file.Length; i++)
                {
                    string line = file[i].Trim().ToUpper();
                    while (!search.Contains(line) && i < file.Length - 1)
                        line = file[++i].Trim().ToUpper();
                    state = search.IndexOf(line);
                    if (state == 0 || state > 6)
                    {
                        addLine = true;
                        jj = null;
                        ji = null;
                    }
                    if (state == 7)
                        polyline = true;

                    if (!addLine)
                        continue;
                    if (i == file.Length - 1)
                        break;
                    line = file[++i].Trim();
                    switch (state)
                    {
                        case 1:
                            jj = (polyline) ? ji : jj;
                            ji = new Joint(Convert.ToSingle(line), 0, 0);
                            if (polyline && jj != null)
                                AddLine(model, ji, jj, props, newJoints, newLines);
                            break;
                        case 2:
                            if (ji != null)
                                ji.Y = Convert.ToSingle(line);
                            break;
                        case 3:
                            if (ji != null)
                                ji.Z = Convert.ToSingle(line);
                            break;
                        case 4:
                            jj = new Joint(Convert.ToSingle(line), 0, 0);
                            break;
                        case 5:
                            if (addLine && jj != null)
                                jj.Y = Convert.ToSingle(line);
                            AddLine(model, ji, jj, props, newJoints, newLines);
                            polyline = false;
                            break;
                        case 6:
                            if (jj != null)
                                jj.Z = Convert.ToSingle(line);
                            break;
                    }
                }
                JoinCmd.Join(model, newJoints, newLines, newAreas);
            }
        }

        /// <summary>
        /// Adds a Line Element to the given Model
        /// </summary>
        /// <param name="model">The Model object</param>
        /// <param name="ji">The initial Joint</param>
        /// <param name="jj">The final Joint</param>
        /// <param name="props">Frame properties for the Line Element</param>
        /// <param name="newJoints">List of new Joints to use in Join</param>
        /// <param name="newLines">List of new Line Elements to use in Join</param>
        private static void AddLine(Canguro.Model.Model model, Joint ji, Joint jj, StraightFrameProps props, List<Joint> newJoints, List<LineElement> newLines)
        {
            if (ji != null && jj != null)
            {
                model.JointList.Add(ji);
                model.JointList.Add(jj);
                LineElement elem = new LineElement(props, ji, jj);
                newJoints.Add(ji);
                newJoints.Add(jj);
                newLines.Add(elem);
                model.LineList.Add(elem);
            }
        }
    }
}
