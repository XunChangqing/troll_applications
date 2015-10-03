using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    public partial class ImageButton : Label
    {
        public Image NormalBack { get; set; }
        public Image HoverBack { get; set; }
        public Image PressBack { get; set; }

        public ImageButton()
        {
            Width = 32;
            Height = 32;

            BackColor = Color.Transparent;
            BackgroundImageLayout = ImageLayout.Zoom;
            MouseEnter += LabelButtonOnMouseEnter;
            MouseLeave += LabelButtonOnMouseLeave;
            MouseDown += LabelButtonOnMouseDown;
            MouseUp += LabelButtonOnMouseUp;
        }

        void LabelButtonOnMouseUp(object sender, MouseEventArgs e)
        {
            Image = NormalBack;
        }

        void LabelButtonOnMouseDown(object sender, MouseEventArgs e)
        {
            Image = PressBack;
        }

        void LabelButtonOnMouseLeave(object sender, EventArgs e)
        {
            Image = NormalBack;
        }

        void LabelButtonOnMouseEnter(object sender, EventArgs e)
        {
            Image = HoverBack;
        }
    }
}
