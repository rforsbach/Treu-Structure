using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Canguro.Commands
{
    /// <summary>
    /// Generic Model Command to group and provide services to all the commands that act upon the Model.
    /// </summary>
    public abstract class ModelCommand : Canguro.Utility.GlobalizedObject, Command
    {
        private bool cancel = false;

        /// <summary>
        /// Returns String.Empty
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public virtual string Title
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets if the command should be cancelled.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public bool Cancel
        {
            get
            {
                return cancel;
            }
            set
            {
                cancel = value;
            }
        }

        /// <summary>
        /// Throws a NotImplementedException. Subclasses should always override this method.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public virtual void Run(Canguro.Controller.CommandServices services)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns true if a command allows cancelling. Can be overrided to disable Cancel.
        /// </summary>
        /// <returns>true</returns>
        public virtual bool AllowCancel()
        {
            return true;
        }
    }
}
