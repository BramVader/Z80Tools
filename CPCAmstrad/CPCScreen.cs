using System.Drawing;
using System.Windows.Forms;

namespace CPCAmstrad
{
    public partial class CPCScreen : Form
    {
        private readonly CPC464Model hardwareModel;
        private bool firstFrame = true;

        public CPCScreen(CPC464Model hardwareModel)
        {
            this.hardwareModel = hardwareModel;
            InitializeComponent();
        }

        public void InvokeRender(Bitmap bitmap)
        {
            if (InvokeRequired)
                Invoke(() => Map(bitmap));
            else
                Map(bitmap);
        }

        public void Map(Bitmap bitmap)
        {
            if (firstFrame)
            {
                ClientSize = new Size(bitmap.Width, bitmap.Height * 2);
                firstFrame = false;
            }
            using var g = Graphics.FromHwnd(this.Handle);
            g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height * 2);
        }

        private void CPCScreen_KeyDown(object sender, KeyEventArgs e)
        {
            hardwareModel.Keyboard.OnKeyDown(e.KeyData);
        }

        private void CPCScreen_KeyUp(object sender, KeyEventArgs e)
        {
            hardwareModel.Keyboard.OnKeyUp(e.KeyData);
        }

        private void CPCScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
    }
}

