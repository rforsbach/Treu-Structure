using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Canguro.SectionCreator.View
{
    class SectionPanel : Panel
    {
        protected ViewState view = new ViewState();
        protected SectionPainter painter = new SectionPainter();

        protected override void OnSizeChanged(EventArgs e)
        {
            view.viewport = new System.Drawing.Point(Width, Height);
        }

        public SectionPanel()
        {
            this.DoubleBuffered = true;
        }

        public ViewState View
        {
            get { return view; }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            painter.Paint(e.Graphics, view);
        }
    }
}
