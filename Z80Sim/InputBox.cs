using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Z80TestConsole
{
    public partial class InputBox : Form
    {
        public InputBox(string caption, string description)
        {
            InitializeComponent();
            Text = caption;
            label1.Text = description;
            textBox1.Focus();
        }

        public string Value
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void InputBox_Activated(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
