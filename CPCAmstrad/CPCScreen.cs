using System;
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
            int height = (crtc.VerticalTotal + 1) * (crtc.MaximumRasterAddress + 1);
            ClientSize = new Size(width, height * 2);
            if (width != screenDataRect.Width ||
                height != screenDataRect.Height)
            {
                screenDataRect = new Rectangle(0, 0, width, height);
                if (screenBitmap != null)
                    screenBitmap.Dispose();
                screenBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            }

            int xOffset = (crtc.HorizontalTotal + 1) - crtc.HorizontalSyncPosition - (crtc.HorizontalAndVerticalSyncWidths & 0xF); // Horizontal offset in characters.
            int yOffset = 0; // (crtc.VerticalTotal + 1) - crtc.VerticalSyncPosition + crtc.VerticalTotalAdjust + 10;

            bool[] ramEnabled = new[] { false, false, true };
            unsafe
            {
                var data = screenBitmap.LockBits(screenDataRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

                int destAddr = 0;
                for (int y = 0; y < height; y++)        // 0..311
                {
                    int destAddr2 = destAddr;
                    int y1 = y; // y - yOffset;
                    for (int x = 0; x <= crtc.HorizontalTotal; x++)      // 0..63  (1µs per char)
                    {
                        int x1 = x - xOffset;
                        bool drawContent =
                            y1 >= 0 &&
                            y1 < (crtc.VerticalDisplayed & 0x7F) * (crtc.MaximumRasterAddress + 1) &&
                            x1 >= 0 &&
                            x1 < crtc.HorizontalDisplayed;

                        // Display content
                        int srcAdr = crtc.VideoPage + (
                            (
                                x1 +    // char: 0..39
                                (
                                    (y1 >> 3) * crtc.HorizontalDisplayed +      // 0..25
                                    (y1 & 7) * 0x400
                                )  +
                                crtc.VideoOffset
                            ) * 2       // 2 bytes per char

                        ) % crtc.VideoSize;

                        // Two bytes per char
                        for (int w = 0; w < 2; w++)
                        {
                            int value = drawContent ? hardwareModel.MemoryModel.Read(srcAdr + w, ramEnabled) : -1;
                            switch (gateArray.Mode)
                            {
                                case 0:
                                    // One byte = 2 pixels in Mode 0 => 8 pixels on result
                                    for (int n = 0; n < 2; n++)
                                    {
                                        int c = n switch
                                        {
                                            0 => ((value & 0x80) >> 7) | ((value & 0x08) >> 2) | ((value & 0x20) >> 3) | ((value & 0x02) << 2),
                                            _ => ((value & 0x40) >> 6) | ((value & 0x04) >> 1) | ((value & 0x10) >> 2) | ((value & 0x01) << 3),
                                        };
                                        Color v = value < 0
                                            ? gateArray.ColorToRgb(gateArray.BorderColor)
                                            : gateArray.ColorToRgb(gateArray.Colors[c]);
                                        for (int m = 0; m < 4; m++)
                                        {
                                            ((byte*)data.Scan0)[destAddr2] = v.B;
                                            ((byte*)data.Scan0)[destAddr2 + 1] = v.G;
                                            ((byte*)data.Scan0)[destAddr2 + 2] = v.R;
                                            destAddr2 += 4;
                                        }
                                    }
                                    break;
                                case 1:
                                    // One byte = 4 pixels in Mode 1 => 8 pixels on result
                                    for (int n = 0; n < 4; n++)
                                    {
                                        int c = n switch
                                        {
                                            0 => ((value & 0x80) >> 7) | ((value & 0x08) >> 2),
                                            1 => ((value & 0x40) >> 6) | ((value & 0x04) >> 1),
                                            2 => ((value & 0x20) >> 5) | ((value & 0x02) >> 0),
                                            _ => ((value & 0x10) >> 4) | ((value & 0x01) << 1),
                                        };
                                        Color v = value < 0
                                            ? gateArray.ColorToRgb(gateArray.BorderColor)
                                            : gateArray.ColorToRgb(gateArray.Colors[c]);
                                        for (int m = 0; m < 2; m++)
                                        {
                                            ((byte*)data.Scan0)[destAddr2] = v.B;
                                            ((byte*)data.Scan0)[destAddr2 + 1] = v.G;
                                            ((byte*)data.Scan0)[destAddr2 + 2] = v.R;
                                            destAddr2 += 4;
                                        }
                                    }
                                    break;
                                case 2:
                                    // One byte = 8 pixels in Mode 2 => 8 pixels on result
                                    for (int n = 0; n < 8; n++)
                                    {
                                        int c = n switch
                                        {
                                            0 => (value & 0x80) >> 7,
                                            1 => (value & 0x40) >> 6,
                                            2 => (value & 0x20) >> 5,
                                            3 => (value & 0x10) >> 4,
                                            4 => (value & 0x08) >> 3,
                                            5 => (value & 0x04) >> 2,
                                            6 => (value & 0x02) >> 1,
                                            _ => (value & 0x01) >> 0,
                                        };
                                        Color v = value < 0
                                            ? gateArray.ColorToRgb(gateArray.BorderColor)
                                            : gateArray.ColorToRgb(gateArray.Colors[c]);
                                        ((byte*)data.Scan0)[destAddr2] = v.B;
                                        ((byte*)data.Scan0)[destAddr2 + 1] = v.G;
                                        ((byte*)data.Scan0)[destAddr2 + 2] = v.R;
                                        destAddr2 += 4;
                                    }
                                    break;
                            }  // .. switch
                        }  // .. for w
                    } // for (x..

                    destAddr += data.Stride;
                } // for (y..
                screenBitmap.UnlockBits(data);
            } // unsafe
            using var g = Graphics.FromHwnd(this.Handle);
            g.DrawImage(screenBitmap, 0, 0, width, height * 2);
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

        private void CPCScreen_Paint(object sender, PaintEventArgs e)
        {
            if (screenBitmap != null)
                e.Graphics.DrawImageUnscaled(screenBitmap, 0, 0);
        }
    }
}

