using BdosCpm;
using Disassembler;
using Emulator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Z80TestConsole
{
    public partial class MainForm : Form
    {

        private readonly Color currentColor = Color.FromArgb(252, 238, 127);
        private readonly Color breakpointColor = Color.FromArgb(171, 97, 107);
        private readonly Color breakpointTextColor = Color.FromArgb(255, 255, 255);
        private readonly Color breakpointBorderColor = Color.FromArgb(125, 122, 223);
        private readonly Color nameColor = Color.FromArgb(0, 0, 0);
        private readonly Color commentColor = Color.FromArgb(80, 80, 80);

        private readonly BaseDisassembler disassembler;
        private readonly ToolTip toolTip1;
        private readonly List<Breakpoint> breakpoints;
        private readonly BaseRegisters lastRegisters = new Z80Core.Z80Registers();
        private readonly HardwareModel model;
        private readonly bool[] memorySwitch;
        private readonly bool[] lastEmulatorMemorySwitch;

        private string hoverOperand = null;

        public MainForm()
        {
            InitializeComponent();

            toolTip1 = new ToolTip
            {
                InitialDelay = 400,
                AutomaticDelay = 400,
                AutoPopDelay = 400,
                ReshowDelay = 400
            };

            breakpoints = new List<Breakpoint>();
            //(var model, var symbols) = LoadCpc464HardwareModel();
            (var model, var symbols) = LoadBdosHardwareModel();

            memorySwitch = (bool[])model.MemorySwitch.Clone();
            lastEmulatorMemorySwitch = (bool[])model.MemorySwitch.Clone();
            //InitRam();

            model.Emulator.GetRegisters<Z80Core.Z80Registers>().CloneTo(lastRegisters);
            model.Emulator.OnRunComplete += (sender, args) =>
            {
                if (this.InvokeRequired)
                {
                    Action del = RunComplete;
                    this.Invoke(del);
                }
                else
                {
                    this.RunComplete();
                }
            };

            disassembler = new Z80Core.Z80Disassembler(false, symbols);
            this.model = model;

            UpdateMemory(0);
            UpdateDisassembly(Registers.PC);
            UpdateStack(Registers.SP);

            UpdateRegisters();
            disassemblyListBox.RequestPage += RequestAssemblyPage;
            stackListBox.RequestPage += RequestStackPage;
            memoryListBox.RequestPage += RequestMemoryPage;

            UpdateMemoryCheckboxList(memorySwitch);

            stackListBox.SelectedAddress = 0;
        }

        private Z80Core.Z80Registers Registers =>
            model.Emulator.GetRegisters<Z80Core.Z80Registers>();

        private (HardwareModel, Symbols) LoadCpc464HardwareModel()
        {
            return (new CPCAmstrad.CPC464Model(), Symbols.Load("Jumptable.txt"));
        }

        private (HardwareModel, Symbols) LoadBdosHardwareModel()
        {
            var model = new BdosModel();
            return (model, model.Symbols);
        }

        private void InitRam()
        {
            model.MemoryModel.Write(

            new byte[]
            { 0xA7,

              0x3E, 0xF0,
              0x47,
              0xCB, 0xD0

            }, 0, model.MemoryModel.Descriptors.GetEnabled(model.MemoryModel.Descriptors["RAM"]));
        }

        private void UpdateMemoryCheckboxList(bool[] enabled)
        {
            memoryCheckedBox.Items.Clear();
            foreach (var descriptor in model.MemoryModel.Descriptors)
            {
                memoryCheckedBox.Items.Add(String.Format("{0}: 0x{1:X4} - 0x{2:X4}", descriptor.Name, descriptor.Offset, descriptor.Offset + descriptor.Length - 1), enabled[descriptor.Index]);
            }
        }

        // Get bitmap from resource
        private Dictionary<string, Bitmap> bitmaps = null;
        private Bitmap GetBitmap(string name)
        {
            if (bitmaps == null)
                bitmaps = new Dictionary<string, Bitmap>();
            if (!bitmaps.TryGetValue(name, out Bitmap bitmap))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fullName = assembly.GetManifestResourceNames().Where(nm => nm.Contains(name + ".png")).First();
                bitmap = (Bitmap)Bitmap.FromStream(assembly.GetManifestResourceStream(fullName));
                bitmaps.Add(name, bitmap);
            }
            return bitmap;
        }

        private static bool IsSameAddress(int address1, MemoryDescriptor memory1, int address2, MemoryDescriptor memory2)
        {
            return (address1 == address2) && (memory1 == memory2);
        }

        private void UpdateMemory()
        {
            var memoryLine = memoryListBox.Items[0] as MemoryLine;
            UpdateMemory(memoryLine.Address);
        }

        private void UpdateMemory(int address)
        {
            int nrItems = memoryListBox.ClientSize.Height / memoryListBox.ItemHeight;
            memoryListBox.Items.Clear();
            for (int n = 0; n < nrItems; n++)
            {
                memoryListBox.Items.Add(new MemoryLine { Address = address });
                address += 16;
            }
        }

        private void UpdateRegisters()
        {
            static void setText(TextBox textbox, int value, int? digits, bool changed)
            {
                textbox.ForeColor = changed ? Color.Red : Color.Black;
                textbox.Text = digits switch
                {
                    0 => value.ToString(),
                    -2 => ((sbyte)value).ToString(),
                    -4 => ((short)value).ToString(),
                    _ => value.ToString("X" + digits),
                };
            }
            static void setCheck(CheckBox checkbox, bool value, bool changed)
            {
                checkbox.ForeColor = changed ? Color.Red : Color.Black;
                checkbox.Checked = value;
            }
            int hx2 = hexadecimalCheckBox.Checked ? 2 : signedCheckBox.Checked ? -2 : 0;
            int hx4 = hexadecimalCheckBox.Checked ? 4 : signedCheckBox.Checked ? -4 : 0;

            var regs = Registers;
            var lastregs = (lastRegisters as Z80Core.Z80Registers);
            setText(textBoxSet0RegA, regs.A, hx2, regs.A != lastregs.A);
            setText(textBoxSet0RegB, regs.B, hx2, regs.B != lastregs.B);
            setText(textBoxSet0RegC, regs.C, hx2, regs.C != lastregs.C);
            setText(textBoxSet0RegD, regs.D, hx2, regs.D != lastregs.D);
            setText(textBoxSet0RegE, regs.E, hx2, regs.E != lastregs.E);
            setText(textBoxSet0RegH, regs.H, hx2, regs.H != lastregs.H);
            setText(textBoxSet0RegL, regs.L, hx2, regs.L != lastregs.L);
            setText(textBoxSet0RegBC, regs.BC, hx4, regs.BC != lastregs.BC);
            setText(textBoxSet0RegDE, regs.DE, hx4, regs.DE != lastregs.DE);
            setText(textBoxSet0RegHL, regs.HL, hx4, regs.HL != lastregs.HL);
            setCheck(checkBoxSet0FlagZ, regs.Z, regs.Z != lastregs.Z);
            setCheck(checkBoxSet0FlagCY, regs.CY, regs.CY != lastregs.CY);
            setCheck(checkBoxSet0FlagHC, regs.HC, regs.HC != lastregs.HC);
            setCheck(checkBoxSet0FlagS, regs.S, regs.S != lastregs.S);
            setCheck(checkBoxSet0FlagPV, regs.PV, regs.PV != lastregs.PV);
            setCheck(checkBoxSet0FlagN, regs.N, regs.N != lastregs.N);

            regs.ExAf(); regs.Exx();
            lastregs.ExAf(); lastregs.Exx();
            setText(textBoxSet1RegA, regs.A, hx2, regs.A != lastregs.A);
            setText(textBoxSet1RegB, regs.B, hx2, regs.B != lastregs.B);
            setText(textBoxSet1RegC, regs.C, hx2, regs.C != lastregs.C);
            setText(textBoxSet1RegD, regs.D, hx2, regs.D != lastregs.D);
            setText(textBoxSet1RegE, regs.E, hx2, regs.E != lastregs.E);
            setText(textBoxSet1RegH, regs.H, hx2, regs.H != lastregs.H);
            setText(textBoxSet1RegL, regs.L, hx2, regs.L != lastregs.L);
            setText(textBoxSet1RegBC, regs.BC, hx4, regs.BC != lastregs.BC);
            setText(textBoxSet1RegDE, regs.DE, hx4, regs.DE != lastregs.DE);
            setText(textBoxSet1RegHL, regs.HL, hx4, regs.HL != lastregs.HL);
            setCheck(checkBoxSet1FlagZ, regs.Z, regs.Z != lastregs.Z);
            setCheck(checkBoxSet1FlagCY, regs.CY, regs.CY != lastregs.CY);
            setCheck(checkBoxSet1FlagHC, regs.HC, regs.HC != lastregs.HC);
            setCheck(checkBoxSet1FlagS, regs.S, regs.S != lastregs.S);
            setCheck(checkBoxSet1FlagPV, regs.PV, regs.PV != lastregs.PV);
            setCheck(checkBoxSet1FlagN, regs.N, regs.N != lastregs.N);
            regs.ExAf(); regs.Exx();
            lastregs.ExAf(); lastregs.Exx();


            setCheck(checkBoxIFF1, regs.Iff1, regs.Iff1 != lastregs.Iff1);
            setCheck(checkBoxIFF2, regs.Iff2, regs.Iff2 != lastregs.Iff2);

            setText(textBoxRegIXH, regs.IXH, hx2, regs.IXH != lastregs.IXH);
            setText(textBoxRegIXL, regs.IXL, hx2, regs.IXL != lastregs.IXL);
            setText(textBoxRegIX, regs.IX, hx4, regs.IX != lastregs.IX);
            setText(textBoxRegIYH, regs.IYH, hx2, regs.IYH != lastregs.IYH);
            setText(textBoxRegIYL, regs.IYL, hx2, regs.IYL != lastregs.IYL);
            setText(textBoxRegIY, regs.IY, hx4, regs.IY != lastregs.IY);

            setText(textBoxRegPC, regs.PC, hx4, regs.PC != lastregs.PC);
            setText(textBoxRegSP, regs.SP, hx4, regs.SP != lastregs.SP);
            setText(textBoxStates, regs.States, 0, regs.SP != lastregs.SP);
        }

        private void UpdateDisassembly(int address)
        {
            int nrItems = disassemblyListBox.ClientSize.Height / disassemblyListBox.ItemHeight;
            disassemblyListBox.Items.Clear();
            while (disassemblyListBox.Items.Count < nrItems)
            {
                var line = disassembler.Disassemble((adr) => model.MemoryModel.Read(adr, memorySwitch), address);
                address = (ushort)(address + line.Opcodes.Length);
                disassemblyListBox.Items.Add(line);
            }
            disassemblyListBox.SetSelectedAddress();
        }

        /// <summary>
        /// Corrects the address by checking if the address is somewhere inbetween a range of opcodes of an instruction.
        /// if correctUp = false, the address is corrected to the first opcode of the instructen, if true the address is corrected to
        /// the next instruction.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="bank"></param>
        /// <param name="correctUp"></param>
        /// <returns></returns>
        private int CorrectAddress(int address, bool correctUp)
        {
            ushort adr1 = (ushort)(address - 20);
            ushort adr2 = adr1;
            while ((ushort)(address - adr1) < 0x8000)
            {
                var line = disassembler.Disassemble((adr) => model.MemoryModel.Read(adr, memorySwitch), adr1);
                adr2 = adr1;
                adr1 = (ushort)(adr1 + line.Opcodes.Length);
            }
            return correctUp ? adr1 : adr2;
        }

        private void ScrollIntoView(ushort address)
        {
            var first = disassemblyListBox.Items[0] as DisassemblyResult;
            var last = disassemblyListBox.Items[^1] as DisassemblyResult;
            var range = (ushort)(last.Address - first.Address);
            var nrange = (ushort)(address - first.Address);
            if (nrange >= range)
            {
                UpdateDisassembly(address);
            }
        }

        private bool CheckMemorySwitch()
        {
            bool changed = false;
            for (int n = 0; n < model.MemorySwitch.Length; n++)
            {
                if (model.MemorySwitch[n] != lastEmulatorMemorySwitch[n])
                {
                    memorySwitch[n] = model.MemorySwitch[n];
                    lastEmulatorMemorySwitch[n] = model.MemorySwitch[n];
                    memoryCheckedBox.SetItemCheckState(n, model.MemorySwitch[n] ? CheckState.Checked : CheckState.Unchecked);
                    changed = true;
                }
            }
            return changed;
        }

        private void UpdateAll()
        {
            CheckMemorySwitch();
            disassemblyListBox.SelectedAddress = Registers.PC;
            ScrollIntoView(Registers.PC);
            disassemblyListBox.Refresh();
            UpdateRegisters();
            UpdateMemory();
            UpdateStack(Registers.SP);
        }

        private void StartRun()
        {
            // Disable controls;
            hexadecimalCheckBox.Enabled = false;
            stackListBox.Enabled = false;
            stackAddressScroller.Enabled = false;
            disassemblyListBox.Enabled = false;
            disassemblyAddressScroller.Enabled = false;
            memoryListBox.Enabled = false;
            openRomToolStripMenuItem.Enabled = false;
            resetToolStripMenuItem.Enabled = false;
            menuItemRun.Enabled = false;
            menuItemRunToCursor.Enabled = false;
            menuItemStepOver.Enabled = false;
            menuItemStepInto.Enabled = false;
            menuItemToggleBreakpoint.Enabled = false;
            menuItemBreakpointDialog.Enabled = false;
            runButton.Enabled = false;
            pauseButton.Enabled = true;
            stopButton.Enabled = true;
            runToCursorButton.Enabled = false;
            restartToolStripMenuItem.Enabled = false;
            stepIntoButton.Enabled = false;
            stepOverButton.Enabled = false;

            textBoxRegPC.Text = ""; textBoxRegPC.Enabled = false;
            textBoxRegSP.Text = ""; textBoxRegSP.Enabled = false;
            textBoxRegIY.Text = ""; textBoxRegIY.Enabled = false;
            textBoxRegIX.Text = ""; textBoxRegIX.Enabled = false;
            textBoxRegIYL.Text = ""; textBoxRegIYL.Enabled = false;
            textBoxRegIXL.Text = ""; textBoxRegIXL.Enabled = false;
            textBoxRegR.Text = ""; textBoxRegR.Enabled = false;
            textBoxRegI.Text = ""; textBoxRegI.Enabled = false;
            textBoxRegIXH.Text = ""; textBoxRegIXH.Enabled = false;
            textBoxRegIYH.Text = ""; textBoxRegIYH.Enabled = false;
            textBoxStates.Text = ""; textBoxStates.Enabled = false;

            textBoxSet0RegA.Text = ""; textBoxSet0RegA.Enabled = false;
            textBoxSet0RegB.Text = ""; textBoxSet0RegB.Enabled = false;
            textBoxSet0RegC.Text = ""; textBoxSet0RegC.Enabled = false;
            textBoxSet0RegD.Text = ""; textBoxSet0RegD.Enabled = false;
            textBoxSet0RegE.Text = ""; textBoxSet0RegE.Enabled = false;
            textBoxSet0RegH.Text = ""; textBoxSet0RegH.Enabled = false;
            textBoxSet0RegL.Text = ""; textBoxSet0RegL.Enabled = false;
            textBoxSet0RegBC.Text = ""; textBoxSet0RegBC.Enabled = false;
            textBoxSet0RegDE.Text = ""; textBoxSet0RegDE.Enabled = false;
            textBoxSet0RegHL.Text = ""; textBoxSet0RegHL.Enabled = false;

            textBoxSet1RegA.Text = ""; textBoxSet1RegA.Enabled = false;
            textBoxSet1RegB.Text = ""; textBoxSet1RegB.Enabled = false;
            textBoxSet1RegC.Text = ""; textBoxSet1RegC.Enabled = false;
            textBoxSet1RegD.Text = ""; textBoxSet1RegD.Enabled = false;
            textBoxSet1RegE.Text = ""; textBoxSet1RegE.Enabled = false;
            textBoxSet1RegH.Text = ""; textBoxSet1RegH.Enabled = false;
            textBoxSet1RegL.Text = ""; textBoxSet1RegL.Enabled = false;
            textBoxSet1RegBC.Text = ""; textBoxSet1RegBC.Enabled = false;
            textBoxSet1RegDE.Text = ""; textBoxSet1RegDE.Enabled = false;
            textBoxSet1RegHL.Text = ""; textBoxSet1RegHL.Enabled = false;

            checkBoxSet0FlagS.Checked = false; checkBoxSet0FlagS.Enabled = false;
            checkBoxSet0FlagPV.Checked = false; checkBoxSet0FlagPV.Enabled = false;
            checkBoxSet0FlagN.Checked = false; checkBoxSet0FlagN.Enabled = false;
            checkBoxSet0FlagHC.Checked = false; checkBoxSet0FlagHC.Enabled = false;
            checkBoxSet0FlagCY.Checked = false; checkBoxSet0FlagCY.Enabled = false;
            checkBoxSet0FlagZ.Checked = false; checkBoxSet0FlagZ.Enabled = false;

            checkBoxSet1FlagS.Checked = false; checkBoxSet1FlagS.Enabled = false;
            checkBoxSet1FlagPV.Checked = false; checkBoxSet1FlagPV.Enabled = false;
            checkBoxSet1FlagN.Checked = false; checkBoxSet1FlagN.Enabled = false;
            checkBoxSet1FlagHC.Checked = false; checkBoxSet1FlagHC.Enabled = false;
            checkBoxSet1FlagCY.Checked = false; checkBoxSet1FlagCY.Enabled = false;
            checkBoxSet1FlagZ.Checked = false; checkBoxSet1FlagZ.Enabled = false;

            checkBoxIFF1.Checked = false; checkBoxIFF1.Enabled = false;
            checkBoxIFF2.Checked = false; checkBoxIFF2.Enabled = false;

            signedCheckBox.Enabled = false;
            memoryCheckedBox.Enabled = false;

            model.Emulator.Run();
        }

        private void RunComplete()
        {
            // Enable controls;
            hexadecimalCheckBox.Enabled = true;
            stackListBox.Enabled = true;
            stackAddressScroller.Enabled = true;
            disassemblyListBox.Enabled = true;
            disassemblyAddressScroller.Enabled = true;
            memoryListBox.Enabled = true;
            openRomToolStripMenuItem.Enabled = true;
            resetToolStripMenuItem.Enabled = true;
            menuItemRun.Enabled = true;
            menuItemRunToCursor.Enabled = true;
            menuItemStepOver.Enabled = true;
            menuItemStepInto.Enabled = true;
            menuItemToggleBreakpoint.Enabled = true;
            menuItemBreakpointDialog.Enabled = true;
            runButton.Enabled = true;
            pauseButton.Enabled = false;
            stopButton.Enabled = false;
            runToCursorButton.Enabled = true;
            restartToolStripMenuItem.Enabled = true;
            stepIntoButton.Enabled = true;
            stepOverButton.Enabled = true;

            textBoxRegPC.Enabled = true;
            textBoxRegSP.Enabled = true;
            textBoxRegIY.Enabled = true;
            textBoxRegIX.Enabled = true;
            textBoxRegIYL.Enabled = true;
            textBoxRegIXL.Enabled = true;
            textBoxRegR.Enabled = true;
            textBoxRegI.Enabled = true;
            textBoxRegIXH.Enabled = true;
            textBoxRegIYH.Enabled = true;
            textBoxStates.Enabled = true;
            checkBoxIFF1.Enabled = true;
            checkBoxIFF2.Enabled = true;

            textBoxSet0RegA.Enabled = true;
            textBoxSet0RegB.Enabled = true;
            textBoxSet0RegC.Enabled = true;
            textBoxSet0RegD.Enabled = true;
            textBoxSet0RegE.Enabled = true;
            textBoxSet0RegH.Enabled = true;
            textBoxSet0RegL.Enabled = true;
            textBoxSet0RegBC.Enabled = true;
            textBoxSet0RegDE.Enabled = true;
            textBoxSet0RegHL.Enabled = true;

            textBoxSet1RegA.Enabled = true;
            textBoxSet1RegB.Enabled = true;
            textBoxSet1RegC.Enabled = true;
            textBoxSet1RegD.Enabled = true;
            textBoxSet1RegE.Enabled = true;
            textBoxSet1RegH.Enabled = true;
            textBoxSet1RegL.Enabled = true;
            textBoxSet1RegBC.Enabled = true;
            textBoxSet1RegDE.Enabled = true;
            textBoxSet1RegHL.Enabled = true;

            checkBoxSet0FlagS.Enabled = true;
            checkBoxSet0FlagPV.Enabled = true;
            checkBoxSet0FlagN.Enabled = true;
            checkBoxSet0FlagHC.Enabled = true;
            checkBoxSet0FlagCY.Enabled = true;
            checkBoxSet0FlagZ.Enabled = true;

            checkBoxSet1FlagS.Enabled = true;
            checkBoxSet1FlagPV.Enabled = true;
            checkBoxSet1FlagN.Enabled = true;
            checkBoxSet1FlagHC.Enabled = true;
            checkBoxSet1FlagCY.Enabled = true;
            checkBoxSet1FlagZ.Enabled = true;

            signedCheckBox.Enabled = true;
            memoryCheckedBox.Enabled = true;

            UpdateAll();

            textBoxTiming.Text = String.Format("States: {0}\r\nTime: {1:0.000}\r\nProc. Freq: {2:0.000} MHz",
                model.Emulator.StatesTaken,
                model.Emulator.TimeTaken,
                ((double)model.Emulator.StatesTaken) / model.Emulator.TimeTaken / 1E6);
        }

        #region Events

        private void HexadecimalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRegisters();
            signedCheckBox.Enabled = !hexadecimalCheckBox.Checked;
        }

        private void SignedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRegisters();
        }

        private void RunClick(object sender, EventArgs e)
        {
            StartRun();
        }

        private void PauseClick(object sender, EventArgs e)
        {
            model.Emulator.Pause();
            UpdateAll();
        }

        private void StopClick(object sender, EventArgs e)
        {
            model.Emulator.Pause();
            UpdateAll();
        }

        private void ResetClick(object sender, EventArgs e)
        {
            model.Emulator.Pause();
            model.Reset();
            UpdateAll();
        }

        private void RunToCursorClick(object sender, EventArgs e)
        {
            if (disassemblyListBox.SelectedAddress.HasValue)
            {
                model.Emulator.TargetAddress = disassemblyListBox.SelectedAddress.Value;
                StartRun();
            }

        }

        private void StepIntoClick(object sender, EventArgs e)
        {
            Registers.CloneTo(lastRegisters);
            model.Emulator.Emulate();
            UpdateAll();
        }

        private void StepOverClick(object sender, EventArgs e)
        {
            Registers.CloneTo(lastRegisters);
            var instruction = disassembler.Disassemble(model.MemoryModel.ReadMemory, Registers.PC);
            model.Emulator.TargetAddress = Registers.PC + instruction.Opcodes.Length;
            StartRun();
        }

        private void ToggleBreakPointClick(object sender, EventArgs e)
        {

        }

        private void BreakpointDialogClick(object sender, EventArgs e)
        {

        }

        private void DisassemblyListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var assemblyLine = disassemblyListBox.Items[e.Index] as DisassemblyResult;
            int height = e.ItemHeight;
            if (!String.IsNullOrEmpty(assemblyLine.Name)) height += (3 * e.ItemHeight) / 2;
            if (!String.IsNullOrEmpty(assemblyLine.Comment)) height += e.ItemHeight;
            e.ItemHeight = height;
        }

        private void DisassemblyListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            float itemheight = disassemblyListBox.ItemHeight;
            var font = disassemblyListBox.Font;

            using var breakpointTextBrush = new SolidBrush(breakpointTextColor);
            using var breakpointBrush = new SolidBrush(breakpointColor);
            using var currentBrush = new SolidBrush(currentColor);
            using var nameBrush = new SolidBrush(nameColor);
            using var commentBrush = new SolidBrush(commentColor);
            using var fontBold = new Font(font, FontStyle.Bold);
            using var breakpointBorderPen = new Pen(breakpointBorderColor, 2f);

            if (disassemblyListBox.Items[e.Index] is DisassemblyResult assemblyLine)
            {
                var memory1 = model.MemoryModel.GetMemoryDescriptor(Registers.PC, model.MemorySwitch);
                var memory2 = model.MemoryModel.GetMemoryDescriptor(assemblyLine.Address, memorySwitch);

                var bpIndex = breakpoints.BinarySearch(new Breakpoint { Address = assemblyLine.Address }, new Breakpoint.Comparer());
                var drawBreakpoint = (bpIndex >= 0);
                var drawCurrent = IsSameAddress(Registers.PC, memory1, assemblyLine.Address, memory2);
                var drawCurrentDrag = grabPC != -1 && (ushort)grabPC == assemblyLine.Address;

                float yoffset = e.Bounds.Top;
                if (!String.IsNullOrEmpty(assemblyLine.Name))
                {
                    yoffset += itemheight * 0.5f;
                    e.Graphics.DrawString(assemblyLine.Name, fontBold, nameBrush, new PointF(20f, yoffset));
                    yoffset += itemheight;
                }
                if (!String.IsNullOrEmpty(assemblyLine.Comment))
                {
                    e.Graphics.DrawString(assemblyLine.Comment, font, commentBrush, new PointF(20f, yoffset));
                    yoffset += itemheight;
                }
                if (drawBreakpoint || drawCurrent)
                {
                    var bpImage = drawBreakpoint && drawCurrent ? GetBitmap("CurrentBreakpoint") :
                        drawBreakpoint ? GetBitmap("Breakpoint") : GetBitmap("Current");
                    e.Graphics.DrawImageUnscaled(bpImage, 0, (int)yoffset - 1);
                }
                if (drawCurrentDrag)
                {
                    e.Graphics.DrawImageUnscaled(GetBitmap("CurrentDrag"), 0, (int)yoffset - 1);
                }
                var textColor = drawBreakpoint && !drawCurrent ? breakpointTextBrush : Brushes.Black;
                string[] strings = new[] {
                    assemblyLine.Address.ToString("X4"),        // Address
                    String.Join("", assemblyLine.Opcodes.Select(op => op.ToString("X2"))),  // Opcodes
                    assemblyLine.Mnemonic, // Mnemonics
                    assemblyLine.Operands, // Operands
                };
                float[] xoffsets = new[] { 20f, 60f, 120f, 160f };

                if (drawBreakpoint || drawCurrent)
                {
                    var textSize1 = e.Graphics.MeasureString(strings[0], font);
                    var textSize2 = e.Graphics.MeasureString(strings[3], font);
                    var rect = new RectangleF(xoffsets[0], yoffset, xoffsets[3] - xoffsets[0] + textSize2.Width, e.Bounds.Height);
                    e.Graphics.FillRectangle(drawCurrent ? currentBrush : breakpointBrush, rect.X, rect.Y, rect.Width, itemheight);
                    if (drawBreakpoint && drawCurrent)
                        e.Graphics.DrawRectangle(breakpointBorderPen, rect.X, rect.Y, rect.Width, itemheight);
                }
                for (int n = 0; n < strings.Length; n++)
                {
                    e.Graphics.DrawString(strings[n], font, textColor, new PointF(xoffsets[n], yoffset));
                }
            }
        }

        private void DisassemblyAddressScroller_Scroll(object sender, ScrollEventArgs e)
        {
            int adr = CorrectAddress((ushort)e.NewValue, e.NewValue > e.OldValue);
            UpdateDisassembly(adr);
            e.NewValue = (int)adr;
        }

        void RequestAssemblyPage(object sender, VirtualListbox.RequestPageEventArgs e)
        {
            int newAdr = 0;
            if (disassemblyListBox.SelectedAddress.HasValue)
            {
                switch (e.PageType)
                {
                    case VirtualListbox.RequestPageEventArgs.PagingType.PageUp:
                        newAdr = CorrectAddress((ushort)(disassemblyListBox.SelectedAddress.Value - 0x20), false);
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = newAdr;
                        disassemblyAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.PageDown:
                        newAdr = CorrectAddress(disassemblyListBox.SelectedAddress.Value, false);
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = (disassemblyListBox.Items[^1] as DisassemblyResult).Address;
                        disassemblyAddressScroller.Value = disassemblyListBox.SelectedAddress.Value;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.LineUp:
                        newAdr = CorrectAddress((ushort)(disassemblyListBox.SelectedAddress.Value - 1), false);
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = newAdr;
                        disassemblyAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.LineDown:
                        newAdr = (disassemblyListBox.Items[1] as DisassemblyResult).Address;
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = (disassemblyListBox.Items[^1] as DisassemblyResult).Address;
                        disassemblyAddressScroller.Value = disassemblyListBox.SelectedAddress.Value;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.ToStart:
                        newAdr = 0;
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = newAdr;
                        disassemblyAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.ToEnd:
                        newAdr = CorrectAddress(0xFFE0, true);
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = newAdr;
                        disassemblyAddressScroller.Value = newAdr;
                        return;
                }
            }
            switch (e.PageType)
            {

                case VirtualListbox.RequestPageEventArgs.PagingType.ScrollUp:
                    newAdr = CorrectAddress((disassemblyListBox.Items[0] as DisassemblyResult).Address - 1, false);
                    UpdateDisassembly(newAdr);
                    disassemblyAddressScroller.Value = newAdr;
                    return;
                case VirtualListbox.RequestPageEventArgs.PagingType.ScrollDown:
                    newAdr = (disassemblyListBox.Items[1] as DisassemblyResult).Address;
                    UpdateDisassembly(newAdr);
                    disassemblyAddressScroller.Value = newAdr;
                    return;


            }
        }

        void RequestMemoryPage(object sender, VirtualListbox.RequestPageEventArgs e)
        {
            if (memoryListBox.SelectedAddress.HasValue)
            {
                int newAdr = 0;
                switch (e.PageType)
                {
                    case VirtualListbox.RequestPageEventArgs.PagingType.PageUp:
                        newAdr = (ushort)(memoryListBox.SelectedAddress.Value - memoryListBox.Items.Count * 16);
                        UpdateMemory(newAdr);
                        memoryListBox.SelectedAddress = newAdr;
                        memoryAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.PageDown:
                        newAdr = memoryListBox.SelectedAddress.Value;
                        UpdateMemory(newAdr);
                        memoryListBox.SelectedAddress = (memoryListBox.Items[^1] as MemoryLine).Address;
                        memoryAddressScroller.Value = memoryListBox.SelectedAddress.Value;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.LineUp:
                        newAdr = (ushort)(memoryListBox.SelectedAddress.Value - 16);
                        UpdateMemory(newAdr);
                        memoryListBox.SelectedAddress = newAdr;
                        memoryAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.LineDown:
                        newAdr = (memoryListBox.Items[1] as MemoryLine).Address;
                        UpdateMemory(newAdr);
                        memoryListBox.SelectedAddress = (memoryListBox.Items[^1] as MemoryLine).Address;
                        memoryAddressScroller.Value = memoryListBox.SelectedAddress.Value;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.ToStart:
                        newAdr = 0;
                        UpdateMemory(newAdr);
                        memoryListBox.SelectedAddress = newAdr;
                        memoryAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.ToEnd:
                        newAdr = (ushort)(0x10000 - memoryListBox.Items.Count * 16);
                        UpdateMemory(newAdr);
                        memoryListBox.SelectedAddress = newAdr;
                        memoryAddressScroller.Value = newAdr;
                        return;
                }
            }
        }

        #endregion

        private void MemoryListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            var memoryLine = memoryListBox.Items[e.Index] as MemoryLine;
            var font = memoryListBox.Font;
            float yoffset = e.Bounds.Top;
            var sb = new StringBuilder();
            sb.Append(memoryLine.Address.ToString("X4")).Append(' ');
            for (int n = 0; n < 16; n++)
            {
                if (n == 8)
                    sb.Append(' ');
                sb.Append(' ').Append(model.MemoryModel.Read((memoryLine.Address + n) % model.MemoryModel.AddressSpace, memorySwitch).ToString("X2"));
            }
            sb.Append(' ');
            for (int n = 0; n < 16; n++)
            {
                if (n == 8)
                    sb.Append(' ');
                char ch = (char)(model.MemoryModel.Read((memoryLine.Address + n) % model.MemoryModel.AddressSpace, memorySwitch));
                if (ch < 32 || ch > 127) ch = '·';
                sb.Append(ch);
            }
            e.Graphics.DrawString(sb.ToString(), font, Brushes.Black, 20f, yoffset);
        }

        private void MemoryAddressScroller_Scroll(object sender, ScrollEventArgs e)
        {
            ushort adr = (ushort)e.NewValue;
            UpdateMemory(adr);
        }

        private int grabPC = -1;
        private int origPC = -1;

        private void DisassemblyListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < 20)
            {
                var itemIndex = disassemblyListBox.IndexFromPoint(e.Location);
                if (itemIndex >= 0)
                {
                    if (disassemblyListBox.Items[itemIndex] is DisassemblyResult item)
                    {
                        var memory1 = model.MemoryModel.GetMemoryDescriptor(Registers.PC, model.MemorySwitch);
                        var memory2 = model.MemoryModel.GetMemoryDescriptor(item.Address, memorySwitch);
                        if (IsSameAddress(Registers.PC, memory1, item.Address, memory2))
                        {
                            origPC = item.Address;
                            grabPC = origPC;
                            disassemblyListBox.Refresh();
                            disassemblyListBox.Capture = true;
                        }
                    }
                }
            }

        }

        private static HashSet<string> callInstructions = new HashSet<string>
        {
            "CALL", "JP", "JR", "DJNZ"
        };

        private string GetItemInfo(string mnemonic, string operand, int index)
        {
            if (Char.IsLetterOrDigit(operand[index]))
            {
                int index1 = index;
                int? offset = null;
                int? byteVal = null;
                int? wordVal = null;
                bool isConstant = false;
                bool isCodeAddress = callInstructions.Contains(mnemonic) && !operand.Contains('(');
                while (index1 > 0 && Char.IsLetterOrDigit(operand[index1 - 1])) index1--;
                int index2 = index;
                while (index2 < operand.Length - 1 && Char.IsLetterOrDigit(operand[index2 + 1])) index2++;
                string oper1 = operand[index1..(index2 + 1)];

                if ((oper1 == "IX" || oper1 == "IY")
                    && index2 < operand.Length - 2
                    && operand[index2 + 1] == ' '
                    && operand[index2 + 2] == '+')
                {
                    index2 += 3;
                    int index3 = index2;
                    while (index2 < operand.Length - 1 && Char.IsDigit(operand[index2 + 1])) index2++;
                    offset = Int32.Parse(operand[index3..(index2 + 1)]);
                }
                while (index1 > 0 && Char.IsLetterOrDigit(operand[index1 - 1])) index1--;

                // Operand is a hexadecimal number?
                if (oper1.StartsWith("0x"))
                {
                    int val = Int32.Parse(oper1[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    if (oper1.Length == 4)
                        byteVal = val;
                    else
                        wordVal = val;
                    isConstant = true;
                }
                else
                // Operand is a symbol?
                {
                    var symbol = disassembler.Symbols.FindSymbols(oper1).FirstOrDefault();
                    if (symbol != null)
                        wordVal = symbol.Value;
                }
                // Operand is a register?
                if (byteVal == null && wordVal == null)
                {
                    if (oper1.Contains("AF"))
                        isCodeAddress = true;
                    var regprops = Registers.GetType().GetProperties();
                    var byteProp = regprops.FirstOrDefault(it => it.Name == oper1 && it.PropertyType == typeof(byte));
                    if (byteProp != null)
                        byteVal = (int)(byte)byteProp.GetValue(Registers);
                    else
                    {
                        var wordProp = regprops.FirstOrDefault(it => it.Name == oper1 && it.PropertyType == typeof(ushort));
                        if (wordProp != null)
                            wordVal = (int)(ushort)wordProp.GetValue(Registers);
                    }
                }

                if (wordVal != null)
                {
                    int target = (wordVal.Value + offset.GetValueOrDefault()) & 0xFFFF;
                    return (isConstant ? "" : $"{oper1} = 0x{wordVal:X4}\n") +
                        (!offset.HasValue ? "" : $"{oper1} + {offset} = 0x{target:X4}\n") +
                        (isCodeAddress ? "" : $"(0x{target:X4}) = 0x{model.MemoryModel.Read(target, memorySwitch):X2}\n")
                        .Trim('\n');
                }
                if (byteVal != null)
                {
                    return isConstant ? "" : $"{oper1} = {byteVal:X2}\r\n";
                }
            }
            return null;
        }


        private void DisassemblyListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (grabPC != -1)
            {
                var itemIndex = disassemblyListBox.IndexFromPoint(e.Location);
                if (itemIndex >= 0)
                {
                    if (disassemblyListBox.Items[itemIndex] is DisassemblyResult item)
                    {
                        grabPC = item.Address;
                        disassemblyListBox.Refresh();
                    }
                }
            }
            else
            {
                hoverOperand = null;

                var itemIndex = disassemblyListBox.IndexFromPoint(e.Location);
                if (itemIndex >= 0
                    && disassemblyListBox.Items[itemIndex] is DisassemblyResult assemblyLine
                    && !String.IsNullOrEmpty(assemblyLine.Operands)
                )
                {
                    using var graphics = Graphics.FromHwnd(this.Handle);
                    var textSize2 = graphics.MeasureString(assemblyLine.Operands, disassemblyListBox.Font);
                    int textIndex = (int)((e.Location.X - 160f) * assemblyLine.Operands.Length / textSize2.Width);
                    if (textIndex >= 0 && textIndex < assemblyLine.Operands.Length)
                    {
                        hoverOperand = GetItemInfo(assemblyLine.Mnemonic, assemblyLine.Operands, textIndex);
                        if (hoverOperand != null)
                        {
                            toolTip1.Show(hoverOperand, disassemblyListBox, disassemblyListBox.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y + 16)));
                        }
                    }
                    else
                    {

                    }
                }
                if (hoverOperand == null)
                {
                    toolTip1.Hide(disassemblyListBox);
                }
            }
        }

        private void DisassemblyListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (grabPC != -1 && grabPC != origPC)
            {
                Registers.PC = (ushort)grabPC;
                UpdateRegisters();
                grabPC = -1;
                disassemblyListBox.Refresh();
                disassemblyListBox.Capture = false;
            }
            else
                if (e.X < 20)
            {
                var itemIndex = disassemblyListBox.IndexFromPoint(e.Location);
                if (itemIndex >= 0 && disassemblyListBox.Items[itemIndex] is DisassemblyResult item)
                {
                    // Toggle breakpoint
                    var newBp = new Breakpoint() { Address = item.Address };
                    var bpIndex = breakpoints.BinarySearch(newBp, new Breakpoint.Comparer());
                    if (bpIndex < 0)
                    {
                        breakpoints.Insert(~bpIndex, newBp);
                        model.Emulator.AddBreakpoint(newBp);
                    }
                    else
                    {
                        breakpoints.RemoveAt(bpIndex);
                        model.Emulator.RemoveBreakpoint(newBp.Address);
                    }
                    disassemblyListBox.Refresh();
                }
            }
        }

        private void MemoryCheckBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            memorySwitch[e.Index] = e.NewValue == CheckState.Checked;
            var first = disassemblyListBox.Items[0] as DisassemblyResult;
            UpdateDisassembly(first.Address);
            UpdateMemory();
        }

        private void GotoAddressItem_Click(object sender, EventArgs e)
        {
            var inputbox = new InputBox("Goto Address", "Address")
            {
                Value = Registers.PC.ToString("X4")
            };
            bool ok = false;
            while (!ok)
            {
                if (inputbox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int newAdr = 0;
                    try
                    {
                        newAdr = Convert.ToInt32(inputbox.Value, 16);
                        ok = true;
                    }
                    catch
                    {
                    }
                    if (ok)
                    {
                        UpdateDisassembly(newAdr);
                        disassemblyListBox.SelectedAddress = newAdr;
                        disassemblyAddressScroller.Value = newAdr;
                    }
                }
                else ok = true;
            }
        }

        private void GotoMemoryAddressItem_Click(object sender, EventArgs e)
        {
            var inputbox = new InputBox("Goto Address", "Address")
            {
                Value = Registers.PC.ToString("X4")
            };
            bool ok = false;
            while (!ok)
            {
                if (inputbox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int newAdr = 0;
                    try
                    {
                        newAdr = Convert.ToInt32(inputbox.Value, 16);
                        ok = true;
                    }
                    catch
                    {
                    }
                    if (ok)
                    {
                        UpdateMemory(newAdr);
                    }
                }
                else ok = true;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            (model as CPCAmstrad.CPC464Model)?.CPCScreen?.Map();
            UpdateAll();
        }

        private void CheckBoxUpdateScreen_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBoxUpdateScreen.Checked;
        }

        private void RequestStackPage(object sender, VirtualListbox.RequestPageEventArgs e)
        {
            if (stackListBox.SelectedAddress.HasValue)
            {
                int newAdr;
                int nrItems = stackListBox.ClientSize.Height / stackListBox.ItemHeight;
                switch (e.PageType)
                {
                    case VirtualListbox.RequestPageEventArgs.PagingType.PageUp:
                        newAdr = (stackListBox.SelectedAddress.Value - nrItems * 2) & 0xFFFF;
                        UpdateStack(newAdr);
                        stackListBox.SelectedAddress = newAdr;
                        stackAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.PageDown:
                        newAdr = (stackListBox.SelectedAddress.Value + nrItems * 2) & 0xFFFF;
                        UpdateStack(newAdr);
                        stackListBox.SelectedAddress = newAdr;
                        stackAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.LineUp:
                        newAdr = (stackListBox.SelectedAddress.Value - 2) & 0xFFFF;
                        UpdateStack(newAdr);
                        stackListBox.SelectedAddress = newAdr;
                        stackAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.LineDown:
                        newAdr = (stackListBox.SelectedAddress.Value + 2) & 0xFFFF;
                        UpdateStack(newAdr);
                        stackListBox.SelectedAddress = newAdr;
                        stackAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.ToStart:
                        newAdr = 0;
                        UpdateStack(newAdr);
                        stackListBox.SelectedAddress = newAdr;
                        stackAddressScroller.Value = newAdr;
                        return;
                    case VirtualListbox.RequestPageEventArgs.PagingType.ToEnd:
                        newAdr = 0x10000 - nrItems * 2;
                        UpdateStack(newAdr);
                        stackListBox.SelectedAddress = newAdr;
                        stackAddressScroller.Value = newAdr;
                        return;
                }
            }
        }

        private void UpdateStack(int address)
        {
            int nrItems = stackListBox.ClientSize.Height / stackListBox.ItemHeight;
            stackListBox.Items.Clear();

            address -= nrItems;
            int offset = ((address & 1) + ((Registers as Z80Core.Z80Registers).SP & 1)) & 1;
            address += offset;
            for (int n = 0; n < nrItems; n++)
            {
                stackListBox.Items.Add(new MemoryLine { Address = address });
                address = (address + 2) & 0xFFFF;
            }
            stackListBox.SetSelectedAddress();
        }

        private void StackListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            float itemheight = stackListBox.ItemHeight;
            using var currentBrush = new SolidBrush(currentColor);
            using var breakpointBorderPen = new Pen(breakpointBorderColor, 2f);
            if (e.Index >= 0)
            {
                var memoryLine = stackListBox.Items[e.Index] as MemoryLine;
                var font = stackListBox.Font;
                float yoffset = e.Bounds.Top;

                int value = model.MemoryModel.Read((memoryLine.Address) & 0xFFFF, memorySwitch) |
                    (model.MemoryModel.Read((memoryLine.Address + 1) & 0xFFFF, memorySwitch) << 8);

                string text = $"{memoryLine.Address:X4}: {value:X4}";

                bool drawCurrent = Registers.SP == memoryLine.Address;
                if (drawCurrent)
                {
                    var bpImage = GetBitmap("Current");
                    e.Graphics.DrawImageUnscaled(bpImage, 0, (int)yoffset - 1);
                    var textSize = e.Graphics.MeasureString(text, font);
                    var rect = new RectangleF(20f, yoffset, textSize.Width, e.Bounds.Height);
                    e.Graphics.FillRectangle(currentBrush, rect.X, rect.Y, rect.Width, itemheight);
                    e.Graphics.DrawRectangle(breakpointBorderPen, rect.X, rect.Y, rect.Width, itemheight);
                }

                e.Graphics.DrawString(text, font, Brushes.Black, 20f, yoffset);
            }
        }

        private void StackAddressScroller_Scroll(object sender, ScrollEventArgs e)
        {
            stackListBox.SelectedAddress = e.NewValue;
            UpdateStack(e.NewValue);
        }

        private void RegisterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && sender is TextBox textbox)
            {
                string name = textbox.Name;
                bool swap = false;
                if (name.StartsWith("textBoxSet0Reg"))
                {
                    name = name.Substring("textBoxSet0Reg".Length);
                }
                else if (name.StartsWith("textBoxSet1Reg"))
                {
                    name = name.Substring("textBoxSet1Reg".Length);
                    swap = true;
                }
                else if (name.StartsWith("textBoxReg"))
                {
                    name = name.Substring("textBoxReg".Length);
                }
                else
                {
                    return;
                }
                var property = typeof(Z80Core.Z80Registers).GetProperty(name);
                if (property == null)
                    return;
                if (swap) Registers.Exx();
                if (!Int32.TryParse(textbox.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value))
                    return;
                try
                {
                    property.SetValue(Registers, Convert.ChangeType(value, property.PropertyType));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (swap) Registers.Exx();
                UpdateRegisters();
                Registers.CloneTo(lastRegisters);
                e.Handled = true;

                if (name == "PC")
                    UpdateDisassembly(Registers.PC);
                if (name == "SP")
                    UpdateStack(Registers.SP);
            }
        }

        private TextBox focusedTextbox = null;

        private void RegisterLeave(object sender, EventArgs e)
        {
            focusedTextbox = null;
            UpdateRegisters();
            Registers.CloneTo(lastRegisters);
        }

        private void RegisterEnter(object sender, EventArgs e)
        {
            if (MouseButtons == MouseButtons.None)
            {
                focusedTextbox = sender as TextBox;
                focusedTextbox.SelectAll();
            }
        }

        private void RegisterMouseUp(object sender, MouseEventArgs e)
        {
            var thisTextBox = sender as TextBox;
            if (thisTextBox.SelectionLength == 0)
            {
                focusedTextbox = thisTextBox;
                thisTextBox.SelectAll();
            }
        }

    }
}
