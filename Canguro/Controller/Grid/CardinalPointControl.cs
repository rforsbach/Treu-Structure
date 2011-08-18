using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.Controller.Grid
{
    public partial class CardinalPointControl : UserControl, IPopupGridControl
    {
        long clickTimer;
        const long minClickTime = 5000000;
        PopupCellEditingControl editingControl = null;
        CardinalPoint value = CardinalPoint.Centroid;

        public CardinalPointControl()
        {
            InitializeComponent();
        }

        #region IPopupGridControl Members

        bool settingValue = false;
        bool askedUnlockModel = false;
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = (value is CardinalPoint) ? (CardinalPoint)value : this.value;
                Invalidate();
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

        private void notifyCellDirty()
        {
            if (!settingValue && editingControl != null)
            {
                editingControl.EditingControlValueChanged = true;
                editingControl.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }
        }

        private static int[] nextUp = new int[] { 10, 4, 5, 6, 7, 8, 9, 1, 2, 3, 11, 10 };
        private static int[] nextDown = new int[] { 10, 7, 8, 9, 1, 2, 3, 4, 5, 6, 11, 10 };
        private static int[] nextRight = new int[] { 10, 2, 3, 1, 5, 6, 10, 8, 9, 10, 4, 7 };
        private static int[] nextLeft = new int[] { 10, 3, 1, 2, 11, 4, 5, 10, 7, 8, 9, 6 };

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Right || keyData == Keys.Tab)
            {
                value = (CardinalPoint)nextRight[(int)value];
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Left)
            {
                value = (CardinalPoint)nextLeft[(int)value];
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Up)
            {
                value = (CardinalPoint)nextUp[(int)value];
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Down)
            {
                value = (CardinalPoint)nextDown[(int)value];
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Enter)
            {
                EndEdit();
                editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        private void CardinalPointControl_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter)
            {
                EndEdit();
                editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab)
            {
                value = (CardinalPoint)nextRight[(int)value];
                Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                value = (CardinalPoint)nextLeft[(int)value];
                Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                value = (CardinalPoint)nextUp[(int)value];
                Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                value = (CardinalPoint)nextDown[(int)value];
                Invalidate();
                e.Handled = true;
            }
        }

        bool cancelClick = false;
        private void st_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            cancelClick = false;
            if ((DateTime.Now.Ticks - clickTimer) < minClickTime && !settingValue)
            {
                cancelClick = true;
                return;
            }

            if ((e.Node.FirstNode) == null)
                editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
        }

        private void st_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((e.Node.FirstNode) == null && !settingValue)
                notifyCellDirty();
        }

        private void st_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelClick)
                e.Cancel = true;
        }

        private void st_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelClick)
                e.Cancel = true;
        }

        private void st_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelClick)
                e.Cancel = true;
        }

        private void button_Clicked(object sender, EventArgs e)
        {
            EndEdit();
        }

        private void EndEdit()
        {
            notifyCellDirty();
            editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);   
        }

        private void CardinalPointControl_Load(object sender, EventArgs e)
        {

        }

        private void CardinalPointControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            CardinalPointControl_KeyDown(sender, new KeyEventArgs(e.KeyData));
        }

        private static readonly CardinalPoint[][] Points = new CardinalPoint[][] {
            new CardinalPoint[]{ CardinalPoint.TopLeft, CardinalPoint.TopCenter, CardinalPoint.TopRight },
            new CardinalPoint[]{ CardinalPoint.MiddleLeft, CardinalPoint.MiddleCenter, CardinalPoint.MiddleRight },
            new CardinalPoint[]{ CardinalPoint.BottomLeft, CardinalPoint.BottomCenter, CardinalPoint.BottomRight } };

        private static readonly Point[] Positions = new Point[] { Point.Empty, new Point(3,70), new Point(37,70), new Point(72,70),
            new Point(3,37), new Point(37,37), new Point(72,37), new Point(3,3), new Point(37,3), new Point(72,3), new Point(37,37), new Point(37,37) };

        private void button11_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Button)
                ((Button)sender).Focus();

            CardinalPoint tmp = value;
            if (sender == button10)
                tmp = CardinalPoint.Centroid;
            else if (sender == button11)
                tmp = CardinalPoint.ShearCenter;
            else if (e.X < 78 && e.Y < 78)
                tmp = Points[e.Y / 26][e.X / 26];
            if (tmp != value)
            {
                value = tmp;
                Console.WriteLine(value);
                Invalidate();            
            }
        }

        Pen pen = new Pen(Brushes.Red);
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if ((int)value < Positions.Length)
                e.Graphics.FillEllipse(Brushes.Red, Positions[(int)value].X, Positions[(int)value].Y, 10, 10);
            if (value == CardinalPoint.Centroid)
                e.Graphics.DrawRectangle(pen, button10.Bounds);
            else if (value == CardinalPoint.ShearCenter)
                e.Graphics.DrawRectangle(pen, button11.Bounds);

        }
    }
}
