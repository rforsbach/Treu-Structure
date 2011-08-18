using System;
using System.Collections;
using System.Text;

namespace Canguro.Model.Undo
{
    /// <summary>
    /// Clase Undoable que representa la acción de agregar o quitar una lista de un diccionario.
    /// </summary>
    public class AddDelListAction : Undoable
    {
        private object key;
        private object obj;
        private bool isAdd;
        private System.Collections.ICollection collection;

        /// <summary>
        /// Constructora que inicializa los valores de la acción con valores dados.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dictionary"></param>
        /// <param name="isAdd"></param>
        public AddDelListAction(object key, object obj, System.Collections.ICollection dictionary, bool isAdd)
        {
            this.key = key;
            this.obj = obj;
            this.collection = dictionary;
            this.isAdd = isAdd;
        }

    /// <summary>
    /// Método que deshace la acción e invierte el estado.
    /// </summary>
        public void Undo()
        {

            if (isAdd)
            {
                if (collection is IDictionary)
                {
                    obj = ((IDictionary)collection)[key];
                    ((IDictionary)collection).Remove(key);
                }
                else if (collection is IList)
                {
                    obj = ((IList)collection)[(int)key];
                    ((IList)collection).RemoveAt((int)key);
                }
            }
            else
            {
                if (collection is IDictionary)
                {
                    ((IDictionary)collection).Add(key, obj);
                    obj = null;
                }
                else if (collection is IList)
                {
                    ((IList)collection).Insert((int)key, obj);
                    obj = null;
                }
            }
            isAdd = !isAdd;
        }
    }
}
