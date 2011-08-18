using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Undo
{
    class ChangeCatalogAction<Tvalue> : Undoable
    {
        private Tvalue old;
        private Catalog<Tvalue> catalog;
        private string key;

        /// <summary>
        /// Constructora que almacena el estado inicial de la acción.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="itemList"></param>
        /// <param name="isAdd"></param>
        public ChangeCatalogAction(Tvalue oldValue, string key, Catalog<Tvalue> catalog)
        {
            this.old = oldValue;
            this.key = key;
            this.catalog = catalog;
        }

        /// <summary>
        /// Método que deshace la acción. Invierte su estado para que la siguiente llamada
        /// tenga el comportamiento contrario (Agregar / quitar).
        /// </summary>
        public void Undo()
        {
            Tvalue tmp = catalog[key];
            catalog[key] = old;
            old = tmp;
        }
    }
}
