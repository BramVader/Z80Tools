using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CPCAmstrad
{
    public partial class CPCKeyboard : Form
    {
        private enum KeyShape
        {
            Normal,
            Esc,
            Del,
            Tab,
            Enter,
            CapsLock,
            LShift,
            RShift,
            Space,
            NormalGreen,
            NormalBlue
        }
        private enum KeyPos
        {
            Esc,
            NextKey,
            NextLine,
            Space,
            ArrowUp,
            ArrowLeft,
            ArrowDown,
            Numpad7
        }

        private Dictionary<KeyShape, (Bitmap, Bitmap)> keyBitmaps = new();

        // Inverse logic, up = bit set in byte
        private byte[] keyup;

        private class KeyInfo
        {
            public KeyShape Shape = KeyShape.Normal;
            public Keys Key = Keys.None;
            public Keys ShiftKey = Keys.None;
            public KeyPos Pos = KeyPos.NextKey;
            public bool Down = false;
            public string Glyph;
            public string SecondaryGlyph;
            public (int, int) MatrixPos;
            public Point Position { get; internal set; }
            public Size Size { get; internal set; }

            public KeyInfo((int, int) pos, Keys key, Keys shiftKey = Keys.None)
            {
                this.MatrixPos = pos;
                this.Key = key;
                this.ShiftKey = shiftKey == Keys.None ? key | Keys.Shift : shiftKey;
                this.Glyph = key.ToString().ToUpper();
                this.SecondaryGlyph = shiftKey != Keys.None ? key.ToString().ToUpper() : null;
            }

            public KeyInfo((int, int) pos, (Keys key, string glyph) key)
            {
                this.MatrixPos = pos;
                this.Key = key.key;
                this.ShiftKey = key.key | Keys.Shift;
                this.Glyph = key.glyph;
                this.SecondaryGlyph = null;
            }

            public KeyInfo((int, int) pos, (Keys key, string glyph) key, (Keys key, string glyph) shiftKey)
            {
                this.MatrixPos = pos;
                this.Key = key.key;
                this.ShiftKey = shiftKey.key;
                this.Glyph = key.glyph;
                this.SecondaryGlyph = shiftKey.glyph;
            }


        }

        private Queue<Action>[] keyActions = Enumerable.Range(0, 10).Select(it => new Queue<Action>()).ToArray();
        private Queue<Action>[] delayedKeyActions = Enumerable.Range(0, 10).Select(it => new Queue<Action>()).ToArray();

        private static List<KeyInfo> keys = [
            // First Row 
            new ((8,2), (Keys.Escape, "ESC")) { Shape = KeyShape.Esc, Pos = KeyPos.Esc },
            new ((8,0), (Keys.D1, "1"), (Keys.D1 | Keys.Shift, "!")),
            new ((8,1), (Keys.D2, "2"), (Keys.Oem7 | Keys.Shift, "\"")),
            new ((7,1), (Keys.D3, "3"), (Keys.D3 | Keys.Shift, "#")),
            new ((7,0), (Keys.D4, "4"), (Keys.D4 | Keys.Shift, "$")),
            new ((6,1), (Keys.D5, "5"), (Keys.D5 | Keys.Shift, "%")),
            new ((6,0), (Keys.D6, "6"), (Keys.D7 | Keys.Shift, "&")),
            new ((5,1), (Keys.D7, "7"), (Keys.Oem7, "'")),
            new ((5,0), (Keys.D8, "8"), (Keys.D9 | Keys.Shift, "(")),
            new ((4,1), (Keys.D9, "9"), (Keys.D0 | Keys.Shift, ")")),
            new ((4,0), (Keys.D0, "0"), (Keys.OemMinus | Keys.Shift, "_")),
            new ((3,1), (Keys.OemMinus, "-"), (Keys.Oemplus, "=")),
            new ((3,0), (Keys.D6 | Keys.Shift, "↑"), (Keys.None, "£")),
            new ((2,0), (Keys.Delete, "CLR")),
            new ((9,7), (Keys.Back, "DEL")) { Shape = KeyShape.Del },

            // Second row
            new ((8,4), (Keys.Tab, "TAB")) { Shape = KeyShape.Tab, Pos = KeyPos.NextLine },
            new ((8,3), Keys.Q), new ((7,3), Keys.W), new ((7,2), Keys.E), new ((6,2), Keys.R), new ((6,3), Keys.T), new ((5,3), Keys.Y), new ((5,2), Keys.U), new ((4,3), Keys.I), new ((4,2), Keys.O), new ((3,3), Keys.P),
            new ((3,2), (Keys.D2 | Keys.Shift, "@"), (Keys.OemPipe | Keys.Shift, "|")),
            new ((2,1), (Keys.Oem4, "[")),
            new ((2,2), (Keys.Enter, "ENTER")) { Shape = KeyShape.Enter },

            // Third row
            new ((8,6), (Keys.CapsLock, "CAPS\nLOCK")) { Shape = KeyShape.CapsLock, Pos = KeyPos.NextLine },
            new ((8,5), Keys.A), new ((7,4), Keys.S), new ((7,5), Keys.D), new ((6,5), Keys.F),new ((6,4), Keys.G), new ((5,4), Keys.H),new ((5,5), Keys.J), new ((4,5), Keys.K),new ((4,4), Keys.L),
            new ((3,5), (Keys.OemSemicolon | Keys.Shift, ":"), (Keys.D8 | Keys.Shift, "*")),
            new ((3,4), (Keys.OemSemicolon, ";"), (Keys.Oemplus | Keys.Shift, "+")),
            new ((2,3), (Keys.Oem6, "]")),

            // Fourth row
            new ((2,5), (Keys.ShiftKey, "SHIFT")) { Shape = KeyShape.LShift, Pos = KeyPos.NextLine },
            new ((8,7), Keys.Z), new ((7,7), Keys.X), new ((7,6), Keys.C), new ((6,7), Keys.V),new ((6,6), Keys.B), new ((5,6), Keys.N),new ((4,6), Keys.M),
            new ((4,7), (Keys.Oemcomma, ","), (Keys.Oemcomma | Keys.Shift, "<")),
            new ((3,7), (Keys.OemPeriod, "."), (Keys.OemPeriod | Keys.Shift, ">")),
            new ((3,6), (Keys.Oem2, "/"), (Keys.Oem2 | Keys.Shift, "?")),
            new ((2,6), (Keys.Oem5, "\\")),
            new ((2,5), (Keys.ShiftKey, "SHIFT")) { Shape = KeyShape.RShift },

            // Fifth row)
            new ((5,7), (Keys.Space, "")) { Shape = KeyShape.Space, Pos = KeyPos.Space },
            new ((2,7), (Keys.ControlKey, "CTRL")) { Shape = KeyShape.NormalGreen },

            // Arrows
            new ((0,0), (Keys.Up, "↑")) {Pos = KeyPos.ArrowUp },
            new ((1,0), (Keys.Left, "←")) { Pos = KeyPos.ArrowLeft },
            new ((1,1), (Keys.End, "COPY")) { Shape = KeyShape.NormalGreen },
            new ((0,1), (Keys.Right, "→")),
            new ((0,2), (Keys.Down, "↓")) { Pos = KeyPos.ArrowDown },

            // Numpad
            new ((1,2), (Keys.NumPad7, "7")) { Pos = KeyPos.Numpad7 },
            new ((1,3), (Keys.NumPad8, "8")),
            new ((0,3), (Keys.NumPad9, "9")),
            new ((2,4), (Keys.NumPad4, "4")) { Pos = KeyPos.NextLine },
            new ((1,4), (Keys.NumPad5, "5")),
            new ((0,4), (Keys.NumPad6, "6")),
            new ((1,5), (Keys.NumPad1, "1")) { Pos = KeyPos.NextLine },
            new ((1,6), (Keys.NumPad2, "2")),
            new ((0,5), (Keys.NumPad3, "3")),
            new ((1,7), (Keys.NumPad0, "0")) { Pos = KeyPos.NextLine },
            new ((0,7), (Keys.Decimal, ".")),
            new ((0,6), (Keys.Enter, "ENTER")) { Shape = KeyShape.NormalBlue },
          ];

        private static List<KeyInfo> shiftKeys = keys.Where(it => it.Key == Keys.ShiftKey).ToList();

        public CPCKeyboard()
        {
            keyup = new byte[10];
            for (int n = 0; n < 10; n++)
                keyup[n] = 0xFF;

            InitKeyBitmaps();
        }

        private void InitKeyBitmaps()
        {
            (Bitmap, Bitmap) split(int left, int top, int width, int height)
            {
                return
                (
                    Properties.Resources.Keyboard1.Clone(new Rectangle(left, top, width, height), System.Drawing.Imaging.PixelFormat.Format32bppArgb),
                    Properties.Resources.Keyboard2.Clone(new Rectangle(left, top, width, height), System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                );
            }

            Bitmap getSpaceBitmap(Bitmap singleKeyBitmap)
            {
                var spaceKeyBitmap = new Bitmap(singleKeyBitmap.Width * 9 /* 9 keys wide */, singleKeyBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(spaceKeyBitmap))
                {
                    g.DrawImage(singleKeyBitmap, new Rectangle(0, 0, 25, 64), new Rectangle(0, 0, 25, 64), GraphicsUnit.Pixel);
                    g.DrawImage(singleKeyBitmap, new Rectangle(25, 0, 476, 64), new Rectangle(25, 0, 12, 64), GraphicsUnit.Pixel);
                    g.DrawImage(singleKeyBitmap, new Rectangle(501, 0, 21, 64), new Rectangle(37, 0, 21, 64), GraphicsUnit.Pixel);
                }
                return spaceKeyBitmap;
            }

            InitializeComponent();
            keyBitmaps.Add(KeyShape.Normal, split(58, 0, 58, 64));
            keyBitmaps.Add(KeyShape.Esc, split(0, 0, 58, 64));
            keyBitmaps.Add(KeyShape.Del, split(175, 0, 89, 64));
            keyBitmaps.Add(KeyShape.Tab, split(0, 64, 89, 64));
            keyBitmaps.Add(KeyShape.Enter, split(148, 64, 116, 128));
            keyBitmaps.Add(KeyShape.CapsLock, split(0, 128, 106, 64));
            keyBitmaps.Add(KeyShape.LShift, split(0, 192, 136, 64));
            keyBitmaps.Add(KeyShape.RShift, split(136, 192, 128, 64));
            keyBitmaps.Add(KeyShape.NormalGreen, split(90, 64, 58, 64));
            keyBitmaps.Add(KeyShape.NormalBlue, split(116, 0, 58, 64));
            keyBitmaps.Add(KeyShape.Space, (getSpaceBitmap(keyBitmaps[KeyShape.Normal].Item1), getSpaceBitmap(keyBitmaps[KeyShape.Normal].Item2)));
        }

        private Bitmap DrawKey(KeyInfo key, Point p = default, Graphics graphics = null, Font smallFont = null, Font largeFont = null)
        {
            Graphics g = graphics ?? Graphics.FromHwnd(this.Handle);
            Font smallFont1 = smallFont ?? new Font(this.Font.FontFamily, 12);
            Font largeFont1 = largeFont ?? new Font(this.Font.FontFamily, 16);

            p = p == default ? key.Position : p;
            var bitmap = key.Down ? keyBitmaps[key.Shape].Item2 : keyBitmaps[key.Shape].Item1;
            g.DrawImage(bitmap, p);

            var glyphs = $"{key.SecondaryGlyph}\n{key.Glyph}".Trim().Split('\n');
            var font = glyphs.Length > 1 || glyphs[0].Length > 1 ? smallFont1 : largeFont1;

            int ty = -1;
            foreach (var glyph in glyphs)
            {
                var size = g.MeasureString(glyph, font);
                var th = (int)size.Height - 4;
                if (ty == -1) ty = p.Y + (bitmap.Height * 8 / 10 - glyphs.Length * th) / 2 + (key.Down ? 4 : 0);
                g.DrawString(glyph, font, Brushes.White, p.X + (bitmap.Width - (int)size.Width) / 2, ty);
                ty += th;
            }

            if (smallFont == null) smallFont1.Dispose();
            if (largeFont == null) largeFont1.Dispose();
            if (graphics == null) g.Dispose();
            return bitmap;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            var keySize = keyBitmaps[KeyShape.Normal].Item1.Size;
            Point drawPos = new Point(0, 0);
            Point offset = new Point(20, 20);
            int rowLeft = 0;

            using var smallFont = new Font(this.Font.FontFamily, keySize.Width * 12 / 64);
            using var largeFont = new Font(this.Font.FontFamily, keySize.Width * 16 / 64);

            foreach (var key in keys)
            {
                switch (key.Pos)
                {
                    case KeyPos.Esc:
                        rowLeft = offset.X;
                        drawPos = new Point(rowLeft, 140 * keySize.Height / 64 + offset.Y);
                        break;
                    case KeyPos.NextLine:
                        drawPos = new Point(rowLeft, drawPos.Y + keySize.Height);
                        break;
                    case KeyPos.Space:
                        var xKey = keys.First(it => it.Glyph == "X");
                        drawPos = new Point(xKey.Position.X, drawPos.Y + keySize.Height);
                        break;
                    case KeyPos.ArrowUp:
                        rowLeft = 1014 * keySize.Width / 58 + offset.X;
                        drawPos = new Point(rowLeft, offset.Y);
                        break;
                    case KeyPos.ArrowLeft:
                        drawPos = new Point(rowLeft - keySize.Width, drawPos.Y + keySize.Height);
                        break;
                    case KeyPos.ArrowDown:
                        drawPos = new Point(rowLeft, drawPos.Y + keySize.Height);
                        break;
                    case KeyPos.Numpad7:
                        rowLeft = 956 * keySize.Width / 58 + offset.X;
                        drawPos = new Point(rowLeft, 204 * keySize.Height / 64 + offset.Y);
                        break;
                }
                var bitmap = DrawKey(key, drawPos, g, smallFont, largeFont);
                key.Position = drawPos;
                key.Size = bitmap.Size;
                drawPos.X = drawPos.X + bitmap.Width;
            }
        }

        private void HandleKeyDown(KeyInfo key, bool delayed)
        {
            if (!key.Down)
            {
                key.Down = true;
                DrawKey(key);

                Action action = () =>
                {
                    keyup[key.MatrixPos.Item1] = (byte)(keyup[key.MatrixPos.Item1] & ~(1 << key.MatrixPos.Item2));
                    //Console.WriteLine($"Reset bit {key.MatrixPos.Item2} on column {key.MatrixPos.Item1}");
                };
                if (delayed)
                    delayedKeyActions[key.MatrixPos.Item1].Enqueue(action);
                else
                    keyActions[key.MatrixPos.Item1].Enqueue(action);
            }
        }

        private void HandleKeyUp(KeyInfo key)
        {
            if (key.Down)
            {
                key.Down = false;
                DrawKey(key);

                Action action = () =>
                {
                    keyup[key.MatrixPos.Item1] = (byte)(keyup[key.MatrixPos.Item1] | (1 << key.MatrixPos.Item2));
                    //Console.WriteLine($"Set bit {key.MatrixPos.Item2} on column {key.MatrixPos.Item1}");
                };
                keyActions[key.MatrixPos.Item1].Enqueue(action);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var key = keys.Where(it => it.Position.X <= e.X && it.Position.Y <= e.Y &&
                it.Position.X + it.Size.Width >= e.X &&
                it.Position.Y + it.Size.Height >= e.Y
            ).OrderBy(it => it.Size.Height).FirstOrDefault();
            if (key != null)
            {
                if (key.Key == Keys.ShiftKey)
                {
                    // Toggle the shift key
                    if (key.Down)
                        HandleKeyUp(key);
                    else
                        HandleKeyDown(key, false);
                }
                else
                {
                    HandleKeyDown(key, false);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            bool checkShift = false;
            foreach (var key in keys.Where(it => it.Key != Keys.ShiftKey))
            {
                if (key.Down)
                {
                    HandleKeyUp(key);
                    checkShift = true;
                }
            }

            if (checkShift)
            {
                foreach (var shiftKey in shiftKeys)
                {
                    HandleKeyUp(shiftKey);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }

        public void OnKeyDown(Keys keyData)
        {
            var key = keys.FirstOrDefault(it => it.Key == keyData || it.ShiftKey == keyData);
            if (key != null)
            {
                bool requiresShift = key.ShiftKey == keyData;
                bool shiftIsDown = shiftKeys.Any(it => it.Down);

                HandleKeyDown(key, requiresShift != shiftIsDown);

                if (key.ShiftKey == keyData)
                {
                    foreach (var shiftKey in shiftKeys)
                    {
                        HandleKeyDown(shiftKey, false);
                    }
                }
                else if (key.Key == keyData)
                {
                    foreach (var shiftKey in shiftKeys)
                    {
                        HandleKeyUp(shiftKey);
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            OnKeyUp(e.KeyData);
        }

        public void OnKeyUp(Keys keyData)
        {
            var key = keys.FirstOrDefault(it => it.Key == keyData || it.ShiftKey == keyData);
            if (key != null)
            {
                HandleKeyUp(key);

                if (key.ShiftKey == keyData)
                {
                    foreach (var shiftKey in shiftKeys)
                    {
                        HandleKeyUp(shiftKey);
                    }
                }
            }
        }

        public byte ReadKey(int column)
        {
            while (keyActions[column].TryDequeue(out Action action)) action();
            byte key = keyup[column];
            while (delayedKeyActions[column].TryDequeue(out Action action))
                keyActions[column].Enqueue(action);
            return key;
        }
    }
}
