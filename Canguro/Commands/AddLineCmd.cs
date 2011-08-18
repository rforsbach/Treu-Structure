using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Snap;
using Canguro.Controller.Tracking;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to add new Line Elements.
    /// </summary>
    public class AddLineCmd : Canguro.Commands.ModelCommand
    {
        private AddLineCmd() { }
        public static readonly AddLineCmd Instance = new AddLineCmd();

        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "addLine"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("addLine");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Adds a set of Line Elements. Opens a properties window and asks the user for two points or Joints for each one.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            LineElement line;
            Joint joint1, joint2;
            LineProps props = new StraightFrameProps();
            List<LineElement> newLines = new List<LineElement>();
            List<AreaElement> newAreas = new List<AreaElement>();

            services.GetProperties(Culture.Get("addLineProps"), props);


            try
            {
                while ((joint1 = services.GetJoint(newLines)) != null)
                {
                    services.TrackingService = LineTrackingService.Instance;
                    services.TrackingService.SetPoint(joint1.Position);

                    while ((joint2 = services.GetJoint(newLines)) == joint1) ;

                    if (joint2 == null)
                    {
                        services.Model.JointList.Remove(joint1);
                        break;
                    }
                    services.TrackingService = null;

                    services.Model.LineList.Add(line = new LineElement(props, joint1, joint2));
                    newLines.Add(line);

                    // Para que se refleje el cambio inmediatamente
                    services.Model.ChangeModel();
                }
            }
            catch (Canguro.Controller.CancelCommandException) { }
            JoinCmd.Join(services.Model, new List<Joint>(), newLines, newAreas);
        }
    }
}
