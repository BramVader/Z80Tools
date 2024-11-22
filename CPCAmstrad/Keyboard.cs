using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPCAmstrad
{
    public class Keyboard
    {
        private struct Map
        {
            public int Column { get; set; }     // PC0..3 outputs - 0..9
            public int RowMask { get; set; }    // AY-3-8912 inputs - IO0..IO7 - bitmask
        }
        
        // See keyboard diagram e.g. in CPC464 intern
        private Keys[,] matrix = new Keys[8, 10]
            {
                { Keys.Up,        Keys.Left,    Keys.Delete,           Keys.Oemtilde,    Keys.D0,       Keys.D8,    Keys.D6, Keys.D4, Keys.D1,      Keys.None },
                { Keys.Right,     Keys.End,     Keys.OemOpenBrackets,  Keys.OemMinus,    Keys.D9,       Keys.D7,    Keys.D5, Keys.D3, Keys.D2,      Keys.None },
                { Keys.Down,      Keys.NumPad7, Keys.Return,           Keys.Oem7,        Keys.O,        Keys.U,     Keys.R,  Keys.E,  Keys.Escape,  Keys.None },
                { Keys.NumPad9,   Keys.NumPad8, Keys.Oem6,             Keys.P,           Keys.I,        Keys.Y,     Keys.T,  Keys.W,  Keys.Q,       Keys.None },
                { Keys.NumPad6,   Keys.NumPad5, Keys.NumPad4,          Keys.Oemplus,     Keys.L,        Keys.H,     Keys.G,  Keys.S,  Keys.Tab,     Keys.None },
                { Keys.NumPad3,   Keys.NumPad1, Keys.ShiftKey,         Keys.Oem1,        Keys.K,        Keys.J,     Keys.F,  Keys.D,  Keys.A,       Keys.None },
                { Keys.Enter,     Keys.NumPad2, Keys.Oem5,             Keys.OemQuestion, Keys.M,        Keys.N,     Keys.B,  Keys.C,  Keys.Capital, Keys.None },
                { Keys.Decimal,   Keys.NumPad0, Keys.ControlKey,       Keys.OemPeriod,   Keys.Oemcomma, Keys.Space, Keys.V,  Keys.X,  Keys.Z,       Keys.Back }
            };

        // Inverse logic, up = bit set in byte
        private byte[] keyup;

        private Dictionary<Keys, List<Map>> keymap;


        public Keyboard()
        {
            keymap = new Dictionary<Keys, List<Map>>();
            for (int column = 0; column < 10; column++)
            {
                for (int row = 0; row < 8; row++)
                {
                    Keys key = matrix[row, column];
                    List<Map> mapping;
                    if (!keymap.TryGetValue(key, out mapping))
                    {
                        mapping = new List<Map>();
                        keymap.Add(key, mapping);
                    }
                    mapping.Add(new Map { Column = column, RowMask = 1 << row });
                }
            }
            keyup = new byte[16];
            for (int n = 0; n < 16; n++)
                keyup[n] = 0xFF;
        }

        public void KeyDown(Keys key)
        {
            List<Map> mapping;
            if (keymap.TryGetValue(key, out mapping))
            {
                foreach (var map in mapping)
                {
                    keyup[map.Column] = (byte)(keyup[map.Column] & ~map.RowMask);
                }
            }
        }

        public void KeyUp(Keys key)
        {
            List<Map> mapping;
            if (keymap.TryGetValue(key, out mapping))
            {
                foreach (var map in mapping)
                {
                    keyup[map.Column] = (byte)(keyup[map.Column] | map.RowMask);
                }
            }
        }

        public byte ReadKey(int column)
        {
            return keyup[column];
        }

    }
}
