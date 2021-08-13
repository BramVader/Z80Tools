using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExpressionTreeViewer;

namespace ExpressionTreeViewerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            RenderExpression();
        }

        private static Dictionary<FieldType, Color> colors = new Dictionary<FieldType,Color>
        {
            { FieldType.Default, Color.Black },
            { FieldType.Comment, Color.Green },
            { FieldType.Identifier, Color.Black },
            { FieldType.Keyword, Color.Blue },
            { FieldType.Number, Color.Green },
            { FieldType.String, Color.Brown },
            { FieldType.Type, Color.DarkCyan }
        };

        public void RenderExpression()
        {
            var emulator = new Z80Core.Z80Emulator();
            var disassembler = new Z80Core.Z80Disassembler(true);
            StringBuilder sb = new StringBuilder();
            for (int opcode = 0; opcode < 256; opcode++)
            {
                if (opcode != 0xED && opcode != 0xCB && opcode != 0xFD && opcode != 0xDD)
                {
                    byte[] opcodes = new byte[] { (byte)opcode, (byte)0x11, (byte)0x22, (byte)0x33, (byte)0x44 };
                    var result = disassembler.Disassemble(adr => opcodes[adr], 0);
                    sb.Append(@"\trowd\trgaph70\trleft-108\trbrdrt\brdrs\brdrw10 \trbrdrl\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \clbrdrt\brdrw15\brdrs\clbrdrl\brdrw15\brdrs\clbrdrb\brdrw15\brdrs\clbrdrr\brdrw15\brdrs \cellx1800\clbrdrt\brdrw15\brdrs\clbrdrl\brdrw15\brdrs\clbrdrb\brdrw15\brdrs\clbrdrr\brdrw15\brdrs \cellx3500\clbrdrt\brdrw15\brdrs\clbrdrl\brdrw15\brdrs\clbrdrb\brdrw15\brdrs\clbrdrr\brdrw15\brdrs \cellx20000\pard\intbl\ltrpar\sl276\slmult1\lang1043\f0\fs22 ");
                    sb.Append(String.Join(" ", result.Opcodes.Select(oc => oc.ToString("X2")))).Append(@"\cell ");
                    sb.Append(result.Mnemonic).Append(@" ").Append(result.Operands).Append(@"\cell ");
                    var expr = emulator.GetExpression(opcodes);
                    sb.Append(@"\par\intbl ").Append(expr.Evaluate(true)).Append(@"\cell\row ");
                }
            }
            
            string rtfheader = @"{\rtf1\ansi " +
                @"{\colortbl;" +
                String.Join("", ((IEnumerable<FieldType>)Enum.GetValues(typeof(FieldType))).Select(i => colors[i]).Select(c => String.Format(@"\red{0}\green{1}\blue{2};", c.R, c.G, c.B))) +
                @"}\cf1 ";

            string rtffooter = "}";

            myRichTextBox1.Rtf = rtfheader + sb.ToString() + rtffooter;
        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            myRichTextBox1.Text = myRichTextBox1.Rtf;
        }
    }
}
