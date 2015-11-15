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
        bool _state;
        public bool State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                StateChanged(_state);
            }
        }
        public Image OnBack { get; set; }
        public Image OffBack { get; set; }
        public ImageSwitchButton(bool state, Image onBack, Image offBack)
        {
            BackColor = Color.Transparent;
            _state = state;
            OnBack = onBack;
            OffBack = offBack;
            Size = OnBack.Size;
            if (_state)
                Image = OnBack;
            else
                Image = offBack;
            Click += ImageSwitchButtonOnClick;
        }
        void ImageSwitchButtonOnClick(object sender, EventArgs e)
        {
            _state = !_state;
            StateChanged(_state);
        }

        private void StateChanged(bool nstate)
        {
            if (nstate)
                Image = OnBack;
            else
                Image = OffBack;
            if (SwitchChanged != null)
                SwitchChanged(this, nstate);
        }
    }
}
