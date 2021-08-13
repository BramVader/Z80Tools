using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPCAmstrad
{
    public class PrinterPort
    {
        public byte Data { get; set; }
        public bool Strobe { get; set; }
        public bool Busy { get; set; }
    }
}
