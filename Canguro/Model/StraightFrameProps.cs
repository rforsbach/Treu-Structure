using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// Clase que define las propiedades de una barra de sección única.
    /// El único tipo de LineElement que se implementa en la versión 1.
    /// </summary>
    [Serializable]
    public class StraightFrameProps : LineProps
    {
        private Section.FrameSection section = null;

        /// <summary>
        /// La única propiedad de esta clase LineProps es la sección de la barra.
        /// Si la sección no está en el catálogo del modelo, se agrega.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Editor(typeof(Canguro.Controller.PropertyGrid.FrameSectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Section.FrameSection Section
        {
            get
            {
                if (section == null)
                    section = Canguro.Model.Section.SectionManager.Instance.DefaultFrameSection as Section.FrameSection;
                return section;
            }
            set
            {
                if (value != null)
                {
                    if (section != null)
                        Model.Instance.Undo.Change(this, section, GetType().GetProperty("Section"));
                    // Si no está con el mismo nombre, se agrega
                    if (Model.Instance.Sections[value.Name] == null)
                        Model.Instance.Sections[value.Name] = value;
                    // Si hay otra sección con el mismo nombre, se cambia el nombre y se agrega
                    if (Model.Instance.Sections[value.Name] != value)
                    {
                        int i = 1;
                        string name = value.Name;
                        value.Name = name + "(" + i++ + ")";
                        while (Model.Instance.Sections[value.Name] != null)
                            value.Name = name + "(" + i++ + ")";
                        Model.Instance.Sections[value.Name] = value;
                    }
                    // Se asigna
                    section = value;
                    Canguro.Model.Section.SectionManager.Instance.DefaultFrameSection = section;
                }
            }
        }

        public override object Clone()
        {
            StraightFrameProps clone = (StraightFrameProps) base.Clone();
            if (clone.section != null)
            {
                if (Model.Instance.Sections[clone.section.Name] == null)
                    Model.Instance.Sections[clone.section.Name] = clone.section;
                else if (Model.Instance.Sections[clone.section.Name] is Canguro.Model.Section.FrameSection)
                    clone.section = (Canguro.Model.Section.FrameSection)Model.Instance.Sections[clone.section.Name];
            }
            return clone;
        }
    }
}
