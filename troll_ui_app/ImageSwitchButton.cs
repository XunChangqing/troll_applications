using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    class ImageSwitchButton : Label
    {
        public event EventHandler<bool> SwitchChanged;
        public bool State { get; set; }
        public Image OnBack { get; set; }
        public Image OffBack { get; set; }
        public ImageSwitchButton(bool state, Image onBack, Image offBack)
        {
            BackColor = Color.Transparent;
            State = state;
            OnBack = onBack;
            OffBack = offBack;
            Size = OnBack.Size;
            if (State)
                Image = OnBack;
            else
                Image = offBack;
            Click += ImageSwitchButtonOnClick;
        }
        void ImageSwitchButtonOnClick(object sender, EventArgs e)
        {
            State = !State;
            if (State)
                Image = OnBack;
            else
                Image = OffBack;
            if (SwitchChanged != null)
                SwitchChanged(this, State);
        }
    }
}
