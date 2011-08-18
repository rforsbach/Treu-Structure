using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que generaliza el concepto de carga.
    /// Heredan clases para cada tipo de elemento (o nodo)
    /// </summary>
    [Serializable]
    public class Load : Item, ICloneable
    {
        [System.ComponentModel.Browsable(false)]
        public override uint Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != 0)
                    Model.Instance.Undo.Change(this, id, GetType().GetProperty("Id"));
                id = value;
            }
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
