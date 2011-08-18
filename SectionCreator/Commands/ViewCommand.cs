using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class ViewCommand : ICommand
    {
        private Controller ctrl;

        public ViewCommand(Controller controller)
        {
            this.ctrl = controller;
        }

        public ViewCommand() : this(Controller.Instance) { }

        public virtual void Init()
        {
            // Do nothing
        }

        protected Controller controller
        {
            get
            {
                if (ctrl == null)
                    ctrl = Controller.Instance;
                return ctrl;
            }
        }

        public virtual void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
        }

        public virtual void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
        }

        public virtual void MouseClick(System.Windows.Forms.MouseEventArgs e)
        {
        }

        public virtual void MouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
        }

        public virtual void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
        }

        public virtual System.Windows.Forms.Cursor Cursor
        {
            get { return System.Windows.Forms.Cursors.Default; }
        }

        public virtual bool AllowSelection
        {
            get { return false; }
        }
    }
}
