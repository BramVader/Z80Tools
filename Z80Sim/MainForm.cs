using BdosCpm;
using Disassembler;
using Emulator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
        private readonly List<Breakpoint> breakpoints;
        private readonly BaseRegisters lastRegisters = new Z80Core.Z80Registers();
        private readonly HardwareModel model;
        private readonly bool[] memorySwitch;
        private readonly bool[] lastEmulatorMemorySwitch;

        public MainForm()
        {
            InitializeComponent();

            breakpoints = new List<Breakpoint>();
            //(var model, var symbols) = LoadCpc464HardwareModel();
            (var model, var symbols) = LoadBdosHardwareModel();

            memorySwitch = (bool[])model.MemorySwitch.Clone();
            lastEmulatorMemorySwitch = (bool[])model.MemorySwitch.Clone();
            //InitRam();

            model.Emulator.Registers.CloneTo(lastRegisters);
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
            UpdateDisassembly(0);

            UpdateRegisters();
            disassemblyListBox.RequestPage += RequestAssemblyPage;
            memoryListBox.RequestPage += RequestMemoryPage;

            UpdateMemoryCheckboxList(memorySwitch);
        }

        private (HardwareModel, Symbols) LoadCpc464HardwareModel()
        {
            return (new CPCAmstrad.CPC464Model(), Symbols.Load("Jumptable.txt"));
        }

        private (HardwareModel, Symbols) LoadBdosHardwareModel()
        {
            var model = new BdosModel();
            return (new BdosModel(), model.Symbols);
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

            var regs = (model.Emulator.Registers as Z80Core.Z80Registers);
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
            disassemblyListBox.SelectedAddress = model.Emulator.Registers.PC;
            ScrollIntoView(model.Emulator.Registers.PC);
            disassemblyListBox.Refresh();
            UpdateRegisters();
            UpdateMemory();
        }

        private void StartRun()
        {
            // Disable controls;
            hexadecimalCheckBox.Enabled = false;
            listBoxStack.Enabled = false;
            disassemblyListBox.Enabled = false;
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
            listBoxStack.Enabled = true;
            disassemblyListBox.Enabled = true;
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
        }

        private void StopClick(object sender, EventArgs e)
        {

        }

        private void RestartClick(object sender, EventArgs e)
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
            model.Emulator.Registers.CloneTo(lastRegisters);
            model.Emulator.Emulate();
            UpdateAll();
        }

        private void StepOverClick(object sender, EventArgs e)
        {
            model.Emulator.Registers.CloneTo(lastRegisters);
            var instruction = disassembler.Disassemble(model.MemoryModel.ReadMemory, model.Emulator.Registers.PC);
            model.Emulator.TargetAddress = model.Emulator.Registers.PC + instruction.Opcodes.Length;
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
            var breakpointTextBrush = new SolidBrush(breakpointTextColor);
            var breakpointBrush = new SolidBrush(breakpointColor);
            var currentBrush = new SolidBrush(currentColor);
            var nameBrush = new SolidBrush(nameColor);
            var commentBrush = new SolidBrush(commentColor);
            var font = disassemblyListBox.Font;
            var fontBold = new Font(font, FontStyle.Bold);

            var breakpointBorderPen = new Pen(breakpointBorderColor, 2f);

            if (disassemblyListBox.Items[e.Index] is DisassemblyResult assemblyLine)
            {
                var memory1 = model.MemoryModel.GetMemoryDescriptor(model.Emulator.Registers.PC, model.MemorySwitch);
                var memory2 = model.MemoryModel.GetMemoryDescriptor(assemblyLine.Address, memorySwitch);

                var bpIndex = breakpoints.BinarySearch(new Breakpoint { Address = assemblyLine.Address }, new Breakpoint.Comparer());
                var drawBreakpoint = (bpIndex >= 0);
                var drawCurrent = IsSameAddress(model.Emulator.Registers.PC, memory1, assemblyLine.Address, memory2);
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
            breakpointTextBrush.Dispose();
            breakpointBrush.Dispose();
            currentBrush.Dispose();
            nameBrush.Dispose();
            commentBrush.Dispose();
            fontBold.Dispose();
        }

        private void DisassemblyAddressScroller_Scroll(object sender, ScrollEventArgs e)
        {
            int adr = CorrectAddress((ushort)e.NewValue, e.NewValue > e.OldValue);
            UpdateDisassembly(adr);
            e.NewValue = (int)adr;
        }

        void RequestAssemblyPage(object sender, VirtualListbox.RequestPageEventArgs e)
        {
            if (disassemblyListBox.SelectedAddress.HasValue)
            {
                int newAdr = 0;
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
            var font = disassemblyListBox.Font;
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
                        var memory1 = model.MemoryModel.GetMemoryDescriptor(model.Emulator.Registers.PC, model.MemorySwitch);
                        var memory2 = model.MemoryModel.GetMemoryDescriptor(item.Address, memorySwitch);
                        if (IsSameAddress(model.Emulator.Registers.PC, memory1, item.Address, memory2))
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

        private void DisassemblyListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (grabPC != -1)
            {
                var itemIndex = disassemblyListBox.IndexFromPoint(e.Location);
                Text = itemIndex.ToString();
                if (itemIndex >= 0)
                {
                    if (disassemblyListBox.Items[itemIndex] is DisassemblyResult item)
                    {
                        grabPC = item.Address;
                        disassemblyListBox.Refresh();
                    }
                }
            }
        }

        private void DisassemblyListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (grabPC != -1 && grabPC != origPC)
            {
                model.Emulator.Registers.PC = (ushort)grabPC;
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
                Value = model.Emulator.Registers.PC.ToString("X4")
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
                Value = model.Emulator.Registers.PC.ToString("X4")
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
            (model as CPCAmstrad.CPC464Model).CPCScreen.Map();
        }

        private void CheckBoxUpdateScreen_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBoxUpdateScreen.Checked;
        }

        private void openRomToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
