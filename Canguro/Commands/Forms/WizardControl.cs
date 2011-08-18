using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Canguro.Commands.Forms
{
    [ToolboxBitmap(typeof(System.Windows.Forms.TabControl))]
    public class WizardControl : System.Windows.Forms.TabControl
    {
        private bool m_HideTabs = false;
        
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool HideTabs
        {
            get{return m_HideTabs;}
            set
            {
                if (m_HideTabs == value) return;
                m_HideTabs = value;
                if (value == true) this.Multiline = true;
                this.UpdateStyles();
            }
        }
        
        [RefreshProperties(RefreshProperties.All)]
        public new bool Multiline 
        {
            get
            {
                if (this.HideTabs) return true;
                return base.Multiline;
            }
            set
            {
               if (this.HideTabs)
                    base.Multiline = true;
                else
                    base.Multiline = value;
            }
        }
        
        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                if (this.HideTabs)
                    return new Rectangle(0, 0, Width, Height);
                else
                {
                    int tabStripHeight, itemHeight;

                    if (this.Alignment <= TabAlignment.Bottom)
                        itemHeight = this.ItemSize.Height;
                    else
                        itemHeight = this.ItemSize.Width;

                    if (this.Appearance == TabAppearance.Normal)
                        tabStripHeight = 5 + (itemHeight * this.RowCount);
                    else
                        tabStripHeight = (3 + itemHeight) * this.RowCount;

                    switch (this.Alignment)
                    {
                        case TabAlignment.Bottom:
                            return new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
                        case TabAlignment.Left:
                            return new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
                        case TabAlignment.Right:
                            return new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
                        default:
                            return new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                    }

                }
                
            }
            
        }
        
    }
}
