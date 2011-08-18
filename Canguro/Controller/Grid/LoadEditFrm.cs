using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Load;

namespace Canguro.Controller.Grid
{
    public partial class LoadEditFrm : Form
    {
        static LoadEditFrm form = new LoadEditFrm();
        LoadEditFrm()
        {
            InitializeComponent();            
        }

        public static DialogResult EditLoad(object l)
        {
            return EditLoad(l, null);
        }

        public static DialogResult EditLoad(object l, IWin32Window wnd)
        {
            if (l == null) return DialogResult.Ignore;
            Canguro.Model.Model.Instance.Undo.Commit();

            initialize(l);

            form.properties.Enabled = true;
            try
            {
                Canguro.Model.Model.Instance.Modified = true;
            }
            catch (Model.ModelIsLockedException)
            {
                form.properties.Enabled = false;
            }
            
            DialogResult dr = form.ShowDialog(wnd);

            if (dr == DialogResult.OK)
                Canguro.Model.Model.Instance.Undo.Commit();
            else
                Canguro.Model.Model.Instance.Undo.Rollback();

            return dr;
        }

        static void initialize(object l)
        {
            form.loadPicture.Image = form.loadImageList.Images[l.GetType().Name];
            form.loadText.Text = Culture.Get("loadType" + l.GetType().Name);
            form.loadDescription.Text = Culture.Get("loadDescription" + l.GetType().Name);

            form.properties.SelectedObject = l;
        }

        private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            properties.Refresh();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            Canguro.View.GraphicViewManager.Instance.ActiveView.ModelRenderer.RenderOptions.OptionsShown |= Canguro.View.Renderer.RenderOptions.ShowOptions.Loads | Canguro.View.Renderer.RenderOptions.ShowOptions.LoadMagnitudes;
        }
    }
}