using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Snap;
using Canguro.Controller.Tracking;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Adds an AreaElement (Not implemented)
    /// </summary>
    public class AddAreaCmd : Canguro.Commands.ModelCommand
    {
        private AddAreaCmd() { }
        public static readonly AddAreaCmd Instance = new AddAreaCmd();

        /// <summary>
        /// Gets the Locale dependent title of the Command under the key "addAreaStr"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("addAreaStr");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Creates one AreaElement.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            AreaElement area;
            List<Joint> joints = new List<Joint>();
            Joint joint1, joint2, joint3, joint4;
            AreaProps props = new AreaProps();
            List<LineElement> newLines = new List<LineElement>();

            try
            {
            services.GetProperties(Culture.Get("addAreaProps"), props);

            joint1 = services.GetJoint(newLines);
            services.TrackingService = PolygonTrackingService.Instance;
            services.TrackingService.SetPoint(joint1.Position);
            services.Model.ChangeModel();

            joint2 = services.GetJoint(newLines);
            services.TrackingService.SetPoint(joint2.Position);
            services.Model.ChangeModel();

            joint3 = services.GetJoint(newLines);
            services.TrackingService.SetPoint(joint3.Position);
            services.Model.ChangeModel();

            joint4 = services.GetJoint(newLines);
            if (joint4 != null)
                services.TrackingService.SetPoint(joint4.Position);

            services.Model.AreaList.Add(area = new AreaElement(props, joint1, joint2, joint3, joint4));
            services.Model.ChangeModel();

            }
            catch (Canguro.Controller.CancelCommandException) { }
            //if (newLines.Count == 0)
            //    services.Model.Undo.Rollback();
            //else
            //    JoinCmd.Join(services.Model, new List<Joint>(), newLines);
        }
    }
}
