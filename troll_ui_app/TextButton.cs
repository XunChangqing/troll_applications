using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    public class TextButton : Label
    {
        //public System.Drawing.Font HoverFont;
        public Color HoverColor;
        private Color orgColor;
        public TextButton()
        {
            //AutoSize = false;
            AutoSize = true;
            MouseEnter += TextButtonOnMouseEnter;
            MouseLeave += TextButtonOnMouseLeave;
        }
        void TextButtonOnMouseLeave(object sender, EventArgs e)
        {
            Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Regular);
            Cursor = Cursors.Default;
            ForeColor = orgColor;
        }
        void TextButtonOnMouseEnter(object sender, EventArgs e)
        {
            Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Underline);
            orgColor = ForeColor;
            ForeColor = HoverColor;
            Cursor = Cursors.Hand;
        }
    }
}
