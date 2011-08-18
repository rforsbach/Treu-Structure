using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Snap;
using Canguro.Controller.Tracking;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Add contiguous lines to the Model.
    /// </summary>
    public class LineStripCmd : Canguro.Commands.ModelCommand
    {
        private LineStripCmd() { }
        public static readonly LineStripCmd Instance = new LineStripCmd();

        /// <summary>
        /// Gets the Locale dependent title of the Command under the key "addLine"
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
        /// Creates a series of connected Line Elements given at least 2 points. Each subsequent point given adds a new Line Element.
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

            joint1 = services.GetJoint(newLines);
            services.TrackingService = LineTrackingService.Instance;
            services.TrackingService.SetPoint(joint1.Position);

            try
            {
                while ((joint2 = services.GetJoint(newLines)) != null)
                {
                    if (joint2 != joint1)
                    {
                        services.Model.LineList.Add(line = new LineElement(props, joint1, joint2));
                        newLines.Add(line);
                        joint1 = joint2;
                        services.TrackingService.SetPoint(joint1.Position);
                        // Para que se refleje el cambio inmediatamente
                        services.Model.ChangeModel();
                    }
                }
            }
            catch (Canguro.Controller.CancelCommandException) { }
            if (newLines.Count == 0)
                services.Model.Undo.Rollback();
            else
                JoinCmd.Join(services.Model, new List<Joint>(), newLines, newAreas);
        }
    }
}
