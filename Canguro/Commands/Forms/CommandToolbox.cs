using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Commands.Forms
{
    public partial class CommandToolbox : Form
    {
        //public CommandToolbox()
        //{
        //    InitializeComponent();
        //}

        private MainFrm mainFrm;
        private bool showOKCancel = true;
        private bool showComboList = true;

        public CommandToolbox(MainFrm mainFrm)
        {
            InitializeComponent();
            this.mainFrm = mainFrm;
        }

        public PropertyGrid Properties
        {
            get
            {
                return properties;
            }
        }

        public bool ShowOKCancel
        {
            get
            {
                return panelOkCancel.Visible;
            }
            set
            {
                if (showOKCancel && !value)
                {
                    this.Height -= panelOkCancel.Height;
                    showOKCancel = false;
                }
                else if (!showOKCancel && value)
                {
                    this.Height += panelOkCancel.Height;
                    showOKCancel = true;
                }

                panelOkCancel.Visible = showOKCancel;
            }
        }

        public void SetComboItems(string[] items)
        {
            if (items == null)
            {
                if (showComboList)
                {
                    this.Height -= comboList.Height;
                    showComboList = false;
                }

                comboList.Items.Clear();
            }
            else
            {
                if (!showComboList)
                {
                    this.Height += comboList.Height;
                    showComboList = true;
                }

                comboList.Items.Clear();
                comboList.Items.AddRange(items);
            }
            comboList.Visible = showComboList;
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        private void CommandToolbox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                Controller.Controller.Instance.Execute("cancel");
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }
    }
}