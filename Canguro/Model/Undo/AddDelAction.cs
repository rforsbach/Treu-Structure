using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Undo
{
    /// <summary>
    /// Clase que representa una acción de agregar o quitar un elemento dado de una lista dada.
    /// Permite deshacer la agregación / eliminación
    /// </summary>
    public class AddDelAction : Undoable
    {
        private object obj;
        private bool isAdd;
        private System.Collections.IList itemList;

        /// <summary>
        /// Constructora que almacena el estado inicial de la acción.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="itemList"></param>
        /// <param name="isAdd"></param>
        public AddDelAction(object obj, System.Collections.IList itemList, bool isAdd)
        {
            this.obj = obj;
            this.itemList = itemList;
            this.isAdd = isAdd;
        }

        /// <summary>
        /// Método que deshace la acción. Invierte su estado para que la siguiente llamada
        /// tenga el comportamiento contrario (Agregar / quitar).
        /// </summary>
        public void Undo()
        {
            if (isAdd)
            {
                if (obj is Item)
                    itemList[(int)((Item)obj).Id] = null;
                else
                    itemList.Remove(obj);
            }
            else
            {
                if (obj is Item)
                    itemList[(int)((Item)obj).Id] = obj;
                else
                itemList.Add(obj);
            }
            isAdd = !isAdd;
        }
    }
}
