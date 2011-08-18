using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Snap;
using Canguro.Controller.Tracking;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to add a Joint to the Model
    /// </summary>
    public class AddJointCmd : Canguro.Commands.ModelCommand
    {
        private AddJointCmd() { }
        public static readonly AddJointCmd Instance = new AddJointCmd();

        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "addJoint"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("addJoint");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Adds Joints in selected points until canceled.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Joint joint;
            Microsoft.DirectX.Vector3 pt;
            Controller.Snap.Magnet m;
            bool showJoints = false;

            while ((joint = services.GetJoint((IList<LineElement>)null)) != null)
            {
                if (!showJoints)
                    Canguro.View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown |= Canguro.View.Renderer.RenderOptions.ShowOptions.ShowJoints;

                showJoints = true;

                //pt = m.SnapPosition;
                //services.Model.JointList.Add(joint = new Joint(pt.X, pt.Y, pt.Z));
                //services.SnapPrimaryPoint = new Canguro.Controller.Snap.PointMagnet(joint);
                //// Para que se refleje el cambio inmediatamente
                services.Model.ChangeModel();
            }
        }
    }
}
