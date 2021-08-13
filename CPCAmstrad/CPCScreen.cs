using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CPCAmstrad
{
    public partial class CPCScreen : Form
    {
        private readonly CPC464Model hardwareModel;

        private Bitmap screenBitmap;
        private Rectangle screenDataRect;
        
        public CPCScreen(CPC464Model hardwareModel)
        {
            this.hardwareModel = hardwareModel;
            InitializeComponent();
        }

        public void Map()
        {
            var crtc = hardwareModel.CRTC6845;
            var gateArray = hardwareModel.GateArray;

            var borderColor = gateArray.ColorToRgb(gateArray.BorderColor);

            int width = (crtc.HorizontalTotal + 1) * 16;
            int height = (crtc.VerticalTotal + 1) * 16;
            if (width != screenDataRect.Width ||
                height != screenDataRect.Height)
            {
                screenDataRect = new Rectangle(0, 0, width, height);
                if (screenBitmap != null)
                    screenBitmap.Dispose();
                screenBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            }

            bool[] ramEnabled = new[] { false, false, true };
            unsafe
            {
                var data = screenBitmap.LockBits(screenDataRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                // x & y are 40x25 character coordinates (in all modes)
                for (int y = 0; y <= crtc.VerticalTotal; y++)
                {
                    int desty = y; // (y + crtc.VerticalSyncPosition) % crtc.VerticalTotal;
                    if (y < crtc.VerticalDisplayed)
                    {
                        for (int x = 0; x <= crtc.HorizontalTotal; x++)
                        {
                            int destX = x; // (x + crtc.HorizontalSyncPosition) % crtc.HorizontalTotal;
                            int destAdr1 = destX * 16 * 4 + (desty * 16) * data.Stride;
                            if (x < crtc.HorizontalDisplayed)
                            {
                                // Each character has 8 scanlines
                                for (int z = 0; z < 8; z++)
                                {
                                    int destAdr2 = destAdr1 + z * data.Stride * 2;
                                    int srcAdr = (((x + y * crtc.HorizontalDisplayed) * 2 + z * 0x800 - crtc.VideoOffset) & (crtc.VideoSize - 1)) + crtc.VideoPage;
                                    for (int w = 0; w < 2; w++)
                                    {
                                        int value = hardwareModel.MemoryModel.Read(srcAdr + w, ramEnabled);
                                        switch (gateArray.Mode)
                                        {
                                            case 0:
                                                // One byte = 2 pixels in Mode 0 => 8 pixels on result
                                                for (int n = 0; n < 2; n++)
                                                {
                                                    int c = 0;
                                                    switch (n)
                                                    {
                                                        case 0: c = ((value & 0x80) >> 7) | ((value & 0x08) >> 2) | ((value & 0x20) >> 3) | ((value & 0x02) << 2); break;
                                                        case 1: c = ((value & 0x40) >> 6) | ((value & 0x04) >> 1) | ((value & 0x10) >> 2) | ((value & 0x01) << 3); break;                                                 
                                                    }
                                                    Color v = gateArray.ColorToRgb(gateArray.Colors[c]);
                                                    for (int m = 0; m < 4; m++)
                                                    {
                                                        ((byte*)data.Scan0)[destAdr2] = v.B;
                                                        ((byte*)data.Scan0)[destAdr2 + 1] = v.G;
                                                        ((byte*)data.Scan0)[destAdr2 + 2] = v.R;
                                                        ((byte*)data.Scan0)[destAdr2 + data.Stride] = v.B;
                                                        ((byte*)data.Scan0)[destAdr2 + data.Stride + 1] = v.G;
                                                        ((byte*)data.Scan0)[destAdr2 + data.Stride + 2] = v.R; 
                                                        destAdr2 += 4;
                                                    }
                                                }
                                                break;
                                            case 1:
                                                // One byte = 4 pixels in Mode 1 => 8 pixels on result
                                                for (int n = 0; n < 4; n++)
                                                {
                                                    int c = 0;
                                                    switch (n)
                                                    {
                                                        case 0: c = ((value & 0x80) >> 7) | ((value & 0x08) >> 2); break;
                                                        case 1: c = ((value & 0x40) >> 6) | ((value & 0x04) >> 1); break;
                                                        case 2: c = ((value & 0x20) >> 5) | ((value & 0x02) >> 0); break;
                                                        case 3: c = ((value & 0x10) >> 4) | ((value & 0x01) << 1); break;
                                                    }
                                                    Color v = gateArray.ColorToRgb(gateArray.Colors[c]);
                                                    for (int m = 0; m < 2; m++)
                                                    {
                                                        ((byte*)data.Scan0)[destAdr2] = v.B;
                                                        ((byte*)data.Scan0)[destAdr2 + 1] = v.G;
                                                        ((byte*)data.Scan0)[destAdr2 + 2] = v.R;
                                                        ((byte*)data.Scan0)[destAdr2 + data.Stride] = v.B;
                                                        ((byte*)data.Scan0)[destAdr2 + data.Stride + 1] = v.G;
                                                        ((byte*)data.Scan0)[destAdr2 + data.Stride + 2] = v.R;
                                                        destAdr2 += 4;
                                                    }
                                                }
                                                break;
                                            case 2:
                                                // One byte = 8 pixels in Mode 2 => 8 pixels on result
                                                for (int n = 0; n < 8; n++)
                                                {
                                                    int c = 0;
                                                    switch (n)
                                                    {
                                                        case 0: c = (value & 0x80) >> 7; break;
                                                        case 1: c = (value & 0x40) >> 6; break;
                                                        case 2: c = (value & 0x20) >> 5; break;
                                                        case 3: c = (value & 0x10) >> 4; break;
                                                        case 4: c = (value & 0x08) >> 3; break;
                                                        case 5: c = (value & 0x04) >> 2; break;
                                                        case 6: c = (value & 0x02) >> 1; break;
                                                        case 7: c = (value & 0x01) >> 0; break;
                                                    }
                                                    Color v = gateArray.ColorToRgb(gateArray.Colors[value]);
                                                    ((byte*)data.Scan0)[destAdr2] = v.B;
                                                    ((byte*)data.Scan0)[destAdr2 + 1] = v.G;
                                                    ((byte*)data.Scan0)[destAdr2 + 2] = v.R;
                                                    ((byte*)data.Scan0)[destAdr2 + data.Stride] = v.B;
                                                    ((byte*)data.Scan0)[destAdr2 + data.Stride + 1] = v.G;
                                                    ((byte*)data.Scan0)[destAdr2 + data.Stride + 2] = v.R;
                                                    destAdr2 += 4;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Each character has 8 scanlines
                                Color v = gateArray.ColorToRgb(gateArray.BorderColor);
                                for (int z = 0; z < 8; z++)
                                {
                                    int destAdr2 = destAdr1 + z * data.Stride * 2;
                                    for (int w = 0; w < 16; w++)
                                    {
                                        ((byte*)data.Scan0)[destAdr2] = v.B;
                                        ((byte*)data.Scan0)[destAdr2 + 1] = v.G;
                                        ((byte*)data.Scan0)[destAdr2 + 2] = v.R;
                                        ((byte*)data.Scan0)[destAdr2 + data.Stride] = v.B;
                                        ((byte*)data.Scan0)[destAdr2 + data.Stride + 1] = v.G;
                                        ((byte*)data.Scan0)[destAdr2 + data.Stride + 2] = v.R;
                                        destAdr2 += 4;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int x = 0; x <= crtc.HorizontalTotal; x++)
                        {
                            int destX = x; // (x + crtc.HorizontalSyncPosition) % crtc.HorizontalTotal;
                            int destAdr1 = destX * 16 * 4 + (desty * 8) * data.Stride * 2;
                            Color v = gateArray.ColorToRgb(gateArray.BorderColor);

                            // Each character has 8 scanlines
                            for (int z = 0; z < 8; z++)
                            {
                                int destAdr2 = destAdr1 + z * data.Stride * 2;
                                for (int w = 0; w < 16; w++)
                                {
                                    ((byte*)data.Scan0)[destAdr2] = v.B;
                                    ((byte*)data.Scan0)[destAdr2 + 1] = v.G;
                                    ((byte*)data.Scan0)[destAdr2 + 2] = v.R;
                                    ((byte*)data.Scan0)[destAdr2 + data.Stride] = v.B;
                                    ((byte*)data.Scan0)[destAdr2 + data.Stride + 1] = v.G;
                                    ((byte*)data.Scan0)[destAdr2 + data.Stride + 2] = v.R;
                                    destAdr2 += 4;
                                }
                            }
                        }
                    }
                }
                screenBitmap.UnlockBits(data);
            }
            using var g = Graphics.FromHwnd(this.Handle);
            g.DrawImageUnscaled(screenBitmap, 0, 0);
        }

        private void CPCScreen_KeyDown(object sender, KeyEventArgs e)
        {
            hardwareModel.Keyboard.KeyDown(e.KeyCode);
        }

        private void CPCScreen_KeyUp(object sender, KeyEventArgs e)
        {
            hardwareModel.Keyboard.KeyUp(e.KeyCode);
        }

        private void CPCScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void CPCScreen_Paint(object sender, PaintEventArgs e)
        {
            if (screenBitmap != null)
                 e.Graphics.DrawImageUnscaled(screenBitmap, 0, 0);
        }
    }
}
