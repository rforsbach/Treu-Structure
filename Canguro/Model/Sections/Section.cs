using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    /// <summary>
    /// Interfaz común que usan todas las secciones.
    /// La primera versión sólo incluye FrameSection's, pero esta interfaz será necesaria en versiones posteriores.
    /// </summary>
    public interface Section : INamed
    {
        /// <summary>
        /// El nombre de la sección. Cada sección debe tener un nombre único en el catálogo en el que se encuentra.
        /// SectionManager debe asegurarse de esto.
        /// </summary>
        new string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Specific to each section, includes name, material and other basic properties.
        /// </summary>
        string Description
        {
            get;
        }
        /// <summary>
        /// La forma de la sección. Se usa para distinguir los diferentes tipos de secciones.
        /// </summary>
        string Shape
        {
            get;
        }

        /// <summary>
        /// Cada sección va ligada a un material (ie. Una sección no puede aparecer con dos materiales diferentes).
        /// </summary>
        Material.Material Material
        {
            get;
            set;
        }
    }
}
