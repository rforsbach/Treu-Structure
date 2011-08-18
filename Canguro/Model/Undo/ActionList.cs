using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Undo
{
    /// <summary>
    /// Clase que representa una lista de acciones Undoable's
    /// En general, cada comando provoca que se cree una ActionList.
    /// El comando Undo, principalmente ejecuta Undo sobre un ActionList.
    /// </summary>
    public class ActionList : Undoable
    {
        private List<Undoable> actions = new List<Undoable>();
    
        /// <summary>
        /// Propiedad de solo lectura que regresa la lista de acciones.
        /// </summary>
        public List<Canguro.Model.Undo.Undoable> Actions
        {
            get
            {
                return actions;
            }
        }

        /// <summary>
        /// Ejecuta Undo sobre todas las acciones registradas en orden inverso.
        /// </summary>
        public void Undo()
        {
            for (int i = actions.Count - 1; i >= 0; i--)
                actions[i].Undo();
        }

        /// <summary>
        /// Ejecuta Undo sobre todas las acciones registradas, en orden.
        /// </summary>
        public void Redo()
        {
            for (int i = 0; i < actions.Count; i++)
                actions[i].Undo();
        }
    }
}
