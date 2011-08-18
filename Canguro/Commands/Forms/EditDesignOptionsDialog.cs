using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Design;

namespace Canguro.Commands.Forms
{
    public partial class EditDesignOptionsDialog : Form
    {
        private DesignOptions options;
        public EditDesignOptionsDialog(DesignOptions options)
        {
            this.options = options;
            InitializeComponent();
            editPropertyGrid.SelectedObject = options;
            editPanel.Visible = true;
        }

        private void defaultsButton_Click(object sender, EventArgs e)
        {
            options.SetDefaults();
            editPropertyGrid.SelectedObject = options;
            editPropertyGrid.Update();
        }
    }
}