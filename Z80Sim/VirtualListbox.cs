using Assembler;
using Disassembler;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Z80TestConsole
{
    public class VirtualListbox : ListBox
    {
        int? selectedAddress;

        public class RequestPageEventArgs : EventArgs
        {
            public enum PagingType
            {
                LineUp,
                LineDown,
                PageUp,
                PageDown,
                ToStart,
                ToEnd,
                ScrollUp,
                ScrollDown
            }

            public PagingType PageType { get; set; }

            public RequestPageEventArgs(PagingType pageType)
            {
                this.PageType = pageType;
            }
        }

        public event EventHandler<RequestPageEventArgs> RequestPage;

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0x200000;
                return cp;
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            selectedAddress = (SelectedItem == null) ? (int?)null : (SelectedItem as BaseResult).Address;
        }

        public void SetSelectedAddress()
        {
            SelectedItem = selectedAddress.HasValue ? Items.Cast<BaseResult>().Where(al => al.Address == selectedAddress.Value).FirstOrDefault() : null;
        }

        public int? SelectedAddress
        {
            get { return selectedAddress; }
            set { selectedAddress = value; SetSelectedAddress(); }
        }

        private enum VirtualKeys : int
        {
            VK_PRIOR = 0x21,  // PAGE UP key
            VK_NEXT = 0x22, // PAGE DOWN key
            VK_END = 0x23, // END key
            VK_HOME = 0x24, // HOME key
            VK_UP = 0x26, // UP ARROW key
            VK_DOWN = 0x28
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x100: // WM_KEYDOWN
                    switch ((VirtualKeys)m.WParam)
                    {
                        case VirtualKeys.VK_PRIOR:
                            if (SelectedIndex != 0)
                                SelectedIndex = 0;
                            else
                                RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.PageUp));
                            break;
                        case VirtualKeys.VK_NEXT:
                            if (SelectedIndex != Items.Count - 1)
                                SelectedIndex = Items.Count - 1;
                            else
                                RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.PageDown));
                            break;
                        case VirtualKeys.VK_END:
                            RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.ToEnd));
                            SelectedIndex = Items.Count - 1;
                            break;
                        case VirtualKeys.VK_HOME:
                            RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.ToStart));
                            SelectedIndex = 0;
                            break;
                        case VirtualKeys.VK_UP:
                            if (SelectedIndex != 0)
                                SelectedIndex--;
                            else
                                RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.LineUp));
                            break;
                        case VirtualKeys.VK_DOWN:
                            if (SelectedIndex != Items.Count - 1)
                                SelectedIndex++;
                            else
                                RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.LineDown));
                            break;
                    }
                    break;
                case 0x020A: //WM_MOUSEWHEEL
                    int delta = 4 * (int)(short)((long)m.WParam >> 16) / 120;
                    if (delta < 0)
                        for (int step = 0; step < -delta; step++)
                            RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.ScrollDown));
                    else if (delta > 0)
                        for (int step = 0; step < delta; step++)
                            RequestPage?.Invoke(this, new RequestPageEventArgs(RequestPageEventArgs.PagingType.ScrollUp));
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
