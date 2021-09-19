using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BdosCpm
{
    public partial class Console : Form
    {
        private SynchronizationContext synchronizationContext;

        public Console()
        {
            InitializeComponent();
            this.synchronizationContext = SynchronizationContext.Current;
        }

        public void Write(string st)
        {
            synchronizationContext.Send(new SendOrPostCallback(
                delegate (object state)
                {
                    consoleTextBox.AppendText((string)st);
                }
            ), st);
        }

        public void WriteLine()
        {
            WriteLine(String.Empty);
        }

        public void WriteLine(string st)
        {
            Write(st + "\r\n");
        }
    }
}
