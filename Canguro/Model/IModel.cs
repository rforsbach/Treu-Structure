using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    public interface IModel
    {
        /// <summary>
        /// gets or sets the Modified status.
        /// true if the model has been modified since last Save, false otherwise.
        /// </summary>
        bool Modified { get; set; }

        /// <summary>
        /// Reises the ModelChanged event
        /// </summary>
        void ChangeModel();
    }
}
