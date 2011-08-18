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
    public class InquiryDistance : Canguro.Commands.ModelCommand
    {
        private InquiryDistance() { }
        public static readonly InquiryDistance Instance = new InquiryDistance();

        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "addLine"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("inquiryDistance");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Adds a set of Line Elements. Opens a properties window and asks the user for two points or Joints for each one.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            try
            {
                Magnet p1 = services.GetPoint();
                services.TrackingService = VectorTrackingService.Instance;
                services.TrackingService.SetPoint(p1.SnapPositionInt);

                Magnet p2 = services.GetPoint();

                services.TrackingService = null;

                float distance = (p2.SnapPosition - p1.SnapPosition).Length();
                string message = string.Format("{0}: {1:G6} {2}", Culture.Get("Distance"), distance, services.Model.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance));
                System.Windows.Forms.MessageBox.Show(message, Title);
            }
            catch (Canguro.Controller.CancelCommandException) { }
        }
    }
}
