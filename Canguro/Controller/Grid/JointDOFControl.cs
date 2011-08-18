using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public partial class JointDOFControl : UserControl, IPopupGridControl
    {
        TextBox[] springConstants = new TextBox[6];
        Canguro.Model.JointDOF value = new Canguro.Model.JointDOF();
        PopupCellEditingControl editingControl = null;

        public JointDOFControl()
        {
            InitializeComponent();
        }

        #region IPopupGridControl Members

        private Canguro.Model.JointDOF.DofType getComboValue(ref float sConstant, ComboBox c, TextBox s)
        {
            try
            {
                switch (((string)c.SelectedItem)[0])
                {
                    case 'R':
                        sConstant = 0;
                        return Canguro.Model.JointDOF.DofType.Restrained;
                    case 'S':
                        sConstant = float.Parse(s.Text);
                        return Canguro.Model.JointDOF.DofType.Spring;
                    default:
                        sConstant = 0;
                        return Canguro.Model.JointDOF.DofType.Free;
                }
            }
            catch (Exception)
            {
                sConstant = 0;
                return Canguro.Model.JointDOF.DofType.Free;
            }
        }
        
        private void setComboValue(Canguro.Model.JointDOF.DofType dof, float sConstant, ComboBox c, TextBox s)
        {
            switch (dof)
            {
                case Canguro.Model.JointDOF.DofType.Restrained:
                    c.SelectedIndex = 1;
                    s.Text = "0.0";
                    break;
                case Canguro.Model.JointDOF.DofType.Spring:
                    c.SelectedIndex = 2;
                    s.Text = sConstant.ToString();
                    break;
                default:
                    c.SelectedIndex = 0;
                    s.Text = "0.0";
                    break;
            }
        }

        bool settingValue = false;
        public object Value
        {
            get
            {
                if (value != null)
                {
                    bool undoEnabledState = Canguro.Model.Model.Instance.Undo.Enabled;
                    try
                    {
                        Canguro.Model.Model.Instance.Undo.Enabled = false;
                        float[] valueSpringConstants = new float[6];
                        value.T1 = getComboValue(ref valueSpringConstants[0], comboT1, spring1);
                        value.T2 = getComboValue(ref valueSpringConstants[1], comboT2, spring2);
                        value.T3 = getComboValue(ref valueSpringConstants[2], comboT3, spring3);
                        value.R1 = getComboValue(ref valueSpringConstants[3], comboR1, spring4);
                        value.R2 = getComboValue(ref valueSpringConstants[4], comboR2, spring5);
                        value.R3 = getComboValue(ref valueSpringConstants[5], comboR3, spring6);
                        value.SpringValues = valueSpringConstants;
                    }
                    finally
                    {
                        Canguro.Model.Model.Instance.Undo.Enabled = undoEnabledState;
                    }
                }

                return value;
            }
            set
            {
                settingValue = true;
                this.value = value as Canguro.Model.JointDOF;
                if (this.value != null)
                {
                    this.value = this.value.Clone();
                    setComboValue(this.value.T1, this.value.SpringValues[0], comboT1, spring1);
                    setComboValue(this.value.T2, this.value.SpringValues[1], comboT2, spring2);
                    setComboValue(this.value.T3, this.value.SpringValues[2], comboT3, spring3);
                    setComboValue(this.value.R1, this.value.SpringValues[3], comboR1, spring4);
                    setComboValue(this.value.R2, this.value.SpringValues[4], comboR2, spring5);
                    setComboValue(this.value.R3, this.value.SpringValues[5], comboR3, spring6);
                    settingValue = false;
                }
            }
        }

        public PopupCellEditingControl EditingControl
        {
            get
            {
                return editingControl;
            }
            set
            {
                editingControl = value;
            }
        }

        public bool HoldFocus
        {
            get { return false; }
        }
        #endregion


        private void nextFocus(object sender)
        {
            bool startLooking = false;
            Control next = null;
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (startLooking && c.Visible)
                {
                    next = c;
                    break;
                }

                if (c == sender)
                    startLooking = true;
            }

            if (next != null)
                next.Focus();
            else
                flowLayoutPanel1.Controls[0].Focus();
        }

        private void previousFocus(object sender)
        {
            Control previous = null;
            
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c == sender)
                    break;

                if (c.Visible)
                    previous = c;
            }

            if (previous != null)
                previous.Focus();
            else
                if (flowLayoutPanel1.Controls["spring6"].Visible)
                    flowLayoutPanel1.Controls["spring6"].Focus();
                else
                    flowLayoutPanel1.Controls["comboR3"].Focus();
        }

        private void JointDOFControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab)
            {
                nextFocus(sender);                
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                previousFocus(sender);                
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
                e.Handled = true;
            }
            else if (sender is TextBox)
            {
                Keys k = e.KeyCode;
                bool beep = false;

                if (k != Keys.Back)
                    beep = !e.Alt && !e.Control && (k >= Keys.A && k <= Keys.Z);

                if (k == Keys.OemPeriod)
                    if (((TextBox)sender).Text.Contains("."))
                        beep = true;

                if (beep)
                {
                    Canguro.Utility.NativeMethods.MessageBeep(Canguro.Utility.NativeMethods.MB_ICONEXCLAMATION);
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            if (cmb.SelectedIndex == 2) // Spring selected
            {
                if (sender == comboT1)
                    spring1.Visible = true;
                else if (sender == comboT2)
                    spring2.Visible = true;
                else if (sender == comboT3)
                    spring3.Visible = true;
                else if (sender == comboR1)
                    spring4.Visible = true;
                else if (sender == comboR2)
                    spring5.Visible = true;
                else if (sender == comboR3)
                    spring6.Visible = true;
                nextFocus(sender);
            }
            else // Anything else selected (not Spring)
            {
                if (sender == comboT1)
                    spring1.Visible = false;
                else if (sender == comboT2)
                    spring2.Visible = false;
                else if (sender == comboT3)
                    spring3.Visible = false;
                else if (sender == comboR1)
                    spring4.Visible = false;
                else if (sender == comboR2)
                    spring5.Visible = false;
                else if (sender == comboR3)
                    spring6.Visible = false;
            }

            notifyCellDirty();
        }

        private void spring_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectAll();
            setStatusText(sender);
        }

        private void combo_Enter(object sender, EventArgs e)
        {
            setStatusText(sender);
        }

        private void setStatusText(object sender)
        {
            char[] separators = { ' ' };
            string[] ss = ((string)((Control)sender).Tag).Split(separators, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder(30);
            foreach (string s in ss)
            {
                sb.Append(Culture.Get(s));
                sb.Append(' ');
            }

            statusText.Text = sb.ToString();
        }

        private void notifyCellDirty()
        {
            if (!settingValue && editingControl != null)
            {
                editingControl.EditingControlValueChanged = true;
                editingControl.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }
        }

        private void spring_TextChanged(object sender, EventArgs e)
        {
            notifyCellDirty();
        }
    }
}
