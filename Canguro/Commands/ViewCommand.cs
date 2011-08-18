using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands
{
    /// <summary>
    /// Generic View Command to group and provide services to concrete View Commands.
    /// </summary>
    public abstract class ViewCommand : Command
    {
        public virtual System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return System.Windows.Forms.Cursors.Default;
            }
        }

        /// <summary>
        /// Property to define if a concrete command is interactive and, therefore, should receive Mouse Events. Default is false. 
        /// Returns false.
        /// </summary>
        public virtual bool IsInteractive
        {
            get
            {
                return false;
            }
        }

        public virtual bool SavePrevious
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Ejecuta una acción no interactiva y termina
        /// </summary>
        public virtual void Run(Canguro.View.GraphicView activeView)
        {
#if DEBUG
            throw new System.NotImplementedException();
#endif
        }

        public virtual void ButtonDown(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public virtual void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public virtual void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public virtual void MouseWheel(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
        }
    }
}
