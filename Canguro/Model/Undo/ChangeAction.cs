using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Undo
{
    /// <summary>
    /// Clase Undoable que representa un cambio a alguna propiedad de un objeto.
    /// </summary>
    public class ChangeAction : Undoable
    {
        private object obj;
        private object oldValue;
        private static Dictionary<System.Reflection.PropertyInfo, int> properties = new Dictionary<System.Reflection.PropertyInfo,int>();
        private static List<System.Reflection.PropertyInfo> propList = new List<System.Reflection.PropertyInfo>(50);
        private int propertyID;


        /// <summary>
        /// Constructora que almacena el estado inicial de la acción.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="oldValue"></param>
        /// <param name="property"></param>
        public ChangeAction(object obj, object oldValue, System.Reflection.PropertyInfo property)
        {
            this.obj = obj;
            this.oldValue = oldValue;
            if (properties.ContainsKey(property))
                this.propertyID = properties[property];
            else
            {
                int lastID = propList.Count;
                properties.Add(property, lastID);
                propertyID = lastID;
                propList.Add(property);
            }
        }

        /// <summary>
        /// Método que deshace la acción y guarda el estado actual.
        /// </summary>
        public void Undo()
        {
            object tmp = propList[propertyID].GetValue(obj, null);
            propList[propertyID].SetValue(obj, oldValue, null);
            oldValue = tmp;
        }
    }
}
