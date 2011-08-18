using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Resources;
using System.Reflection;
using Canguro.Model.Section;

namespace Canguro.Controller.PropertyGrid
{
    public class FrameSectionEditor : UITypeEditor
    {
        private FrameSectionFrm frmSE = null;

        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"></see> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>The new value of the object. If the value of the object has not changed, this should return the same object it was passed.</returns>
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService wfes = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            if (wfes != null)
            {
                if (frmSE == null)
                    frmSE = new FrameSectionFrm();

                frmSE.SetDropDownParams((Section)value, wfes);
                wfes.DropDownControl(frmSE);
                value = frmSE.Result;
            }
            return value;
        }

        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"></see> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"></see> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"></see>.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.</returns>
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        /// <param name="e">A <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see> that indicates what to paint and where to paint it.</param>
        public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
        {
            try
            {
                //Load SectionResources file
                string m = this.GetType().Module.Name;
                m = m.Substring(0, m.Length - 4);
                ResourceManager resourceManager =
                    new ResourceManager(m + ".Properties.SectionResources",
                    Assembly.GetExecutingAssembly());

                //Draw the corresponding image
                if (e.Value is FrameSection)
                {
                    string imageName = ((FrameSection)e.Value).Shape + "Section";
                    Bitmap newImage = (Bitmap)resourceManager.GetObject(imageName);
                    Rectangle destRect = e.Bounds;
                    newImage.MakeTransparent();
                    e.Graphics.DrawImage(newImage, destRect);
                }
            }
            catch (Exception) { }
        }
    }
}
