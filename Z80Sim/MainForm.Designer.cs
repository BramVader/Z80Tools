using System;

namespace Z80TestConsole
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            groupBoxStack = new System.Windows.Forms.GroupBox();
            stackListBox = new VirtualListbox();
            contextMenuAssembly = new System.Windows.Forms.ContextMenuStrip(components);
            gotoAssemblyAddressItem = new System.Windows.Forms.ToolStripMenuItem();
            stackAddressScroller = new System.Windows.Forms.VScrollBar();
            groupBoxRegisters = new System.Windows.Forms.GroupBox();
            panel1 = new System.Windows.Forms.Panel();
            checkBoxIFF2 = new System.Windows.Forms.CheckBox();
            checkBoxIFF1 = new System.Windows.Forms.CheckBox();
            textBoxTiming = new System.Windows.Forms.TextBox();
            textBoxRegPC = new System.Windows.Forms.TextBox();
            textBoxRegSP = new System.Windows.Forms.TextBox();
            textBoxRegIY = new System.Windows.Forms.TextBox();
            textBoxRegIX = new System.Windows.Forms.TextBox();
            textBoxRegIYL = new System.Windows.Forms.TextBox();
            textBoxRegIXL = new System.Windows.Forms.TextBox();
            textBoxStates = new System.Windows.Forms.TextBox();
            textBoxRegR = new System.Windows.Forms.TextBox();
            textBoxRegI = new System.Windows.Forms.TextBox();
            textBoxRegIXH = new System.Windows.Forms.TextBox();
            textBoxRegIYH = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label32 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            signedCheckBox = new System.Windows.Forms.CheckBox();
            hexadecimalCheckBox = new System.Windows.Forms.CheckBox();
            tabRegisters = new System.Windows.Forms.TabControl();
            tabRegisterSet1 = new System.Windows.Forms.TabPage();
            checkBoxSet0FlagS = new System.Windows.Forms.CheckBox();
            checkBoxSet0FlagPV = new System.Windows.Forms.CheckBox();
            checkBoxSet0FlagN = new System.Windows.Forms.CheckBox();
            checkBoxSet0FlagHC = new System.Windows.Forms.CheckBox();
            checkBoxSet0FlagCY = new System.Windows.Forms.CheckBox();
            checkBoxSet0FlagZ = new System.Windows.Forms.CheckBox();
            label21 = new System.Windows.Forms.Label();
            label23 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            label25 = new System.Windows.Forms.Label();
            label26 = new System.Windows.Forms.Label();
            label27 = new System.Windows.Forms.Label();
            label28 = new System.Windows.Forms.Label();
            label29 = new System.Windows.Forms.Label();
            label30 = new System.Windows.Forms.Label();
            label31 = new System.Windows.Forms.Label();
            textBoxSet0RegA = new System.Windows.Forms.TextBox();
            textBoxSet0RegB = new System.Windows.Forms.TextBox();
            textBoxSet0RegC = new System.Windows.Forms.TextBox();
            textBoxSet0RegD = new System.Windows.Forms.TextBox();
            textBoxSet0RegE = new System.Windows.Forms.TextBox();
            textBoxSet0RegH = new System.Windows.Forms.TextBox();
            textBoxSet0RegL = new System.Windows.Forms.TextBox();
            textBoxSet0RegBC = new System.Windows.Forms.TextBox();
            textBoxSet0RegDE = new System.Windows.Forms.TextBox();
            textBoxSet0RegHL = new System.Windows.Forms.TextBox();
            tabRegisterSet2 = new System.Windows.Forms.TabPage();
            checkBoxSet1FlagS = new System.Windows.Forms.CheckBox();
            checkBoxSet1FlagPV = new System.Windows.Forms.CheckBox();
            checkBoxSet1FlagN = new System.Windows.Forms.CheckBox();
            checkBoxSet1FlagHC = new System.Windows.Forms.CheckBox();
            checkBoxSet1FlagCY = new System.Windows.Forms.CheckBox();
            checkBoxSet1FlagZ = new System.Windows.Forms.CheckBox();
            textBoxSet1RegA = new System.Windows.Forms.TextBox();
            textBoxSet1RegB = new System.Windows.Forms.TextBox();
            textBoxSet1RegC = new System.Windows.Forms.TextBox();
            textBoxSet1RegD = new System.Windows.Forms.TextBox();
            textBoxSet1RegE = new System.Windows.Forms.TextBox();
            textBoxSet1RegH = new System.Windows.Forms.TextBox();
            textBoxSet1RegL = new System.Windows.Forms.TextBox();
            textBoxSet1RegBC = new System.Windows.Forms.TextBox();
            textBoxSet1RegDE = new System.Windows.Forms.TextBox();
            textBoxSet1RegHL = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            groupBoxDisassembly = new System.Windows.Forms.GroupBox();
            disassemblyListBox = new VirtualListbox();
            disassemblyAddressScroller = new System.Windows.Forms.VScrollBar();
            groupBoxMemory = new System.Windows.Forms.GroupBox();
            memoryListBox = new VirtualListbox();
            contextMenuMemory = new System.Windows.Forms.ContextMenuStrip(components);
            gotoMemoryAddressItem = new System.Windows.Forms.ToolStripMenuItem();
            memoryAddressScroller = new System.Windows.Forms.VScrollBar();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuItemRun = new System.Windows.Forms.ToolStripMenuItem();
            menuItemRunToCursor = new System.Windows.Forms.ToolStripMenuItem();
            menuItemStepOver = new System.Windows.Forms.ToolStripMenuItem();
            menuItemStepInto = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            menuItemToggleBreakpoint = new System.Windows.Forms.ToolStripMenuItem();
            menuItemBreakpointDialog = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            runButton = new System.Windows.Forms.ToolStripButton();
            pauseButton = new System.Windows.Forms.ToolStripButton();
            stopButton = new System.Windows.Forms.ToolStripButton();
            resetButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            runToCursorButton = new System.Windows.Forms.ToolStripButton();
            stepIntoButton = new System.Windows.Forms.ToolStripButton();
            stepOverButton = new System.Windows.Forms.ToolStripButton();
            groupBoxMemorySelect = new System.Windows.Forms.GroupBox();
            memoryCheckedBox = new System.Windows.Forms.CheckedListBox();
            checkBoxUpdateScreen = new System.Windows.Forms.CheckBox();
            timer1 = new System.Windows.Forms.Timer(components);
            MainPanel = new System.Windows.Forms.Panel();
            splitter1 = new System.Windows.Forms.Splitter();
            assemblyFindReferences = new System.Windows.Forms.ToolStripMenuItem();
            groupBoxStack.SuspendLayout();
            contextMenuAssembly.SuspendLayout();
            groupBoxRegisters.SuspendLayout();
            panel1.SuspendLayout();
            tabRegisters.SuspendLayout();
            tabRegisterSet1.SuspendLayout();
            tabRegisterSet2.SuspendLayout();
            groupBoxDisassembly.SuspendLayout();
            groupBoxMemory.SuspendLayout();
            contextMenuMemory.SuspendLayout();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            groupBoxMemorySelect.SuspendLayout();
            MainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxStack
            // 
            groupBoxStack.Controls.Add(stackListBox);
            groupBoxStack.Controls.Add(stackAddressScroller);
            groupBoxStack.Dock = System.Windows.Forms.DockStyle.Right;
            groupBoxStack.Location = new System.Drawing.Point(569, 80);
            groupBoxStack.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxStack.Name = "groupBoxStack";
            groupBoxStack.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxStack.Size = new System.Drawing.Size(124, 404);
            groupBoxStack.TabIndex = 1;
            groupBoxStack.TabStop = false;
            groupBoxStack.Text = "Stack";
            // 
            // stackListBox
            // 
            stackListBox.ContextMenuStrip = contextMenuAssembly;
            stackListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            stackListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            stackListBox.Font = new System.Drawing.Font("Consolas", 9F);
            stackListBox.FormattingEnabled = true;
            stackListBox.ItemHeight = 14;
            stackListBox.Location = new System.Drawing.Point(4, 19);
            stackListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            stackListBox.Name = "stackListBox";
            stackListBox.SelectedAddress = null;
            stackListBox.Size = new System.Drawing.Size(95, 382);
            stackListBox.TabIndex = 6;
            stackListBox.DrawItem += StackListBox_DrawItem;
            // 
            // contextMenuAssembly
            // 
            contextMenuAssembly.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuAssembly.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { gotoAssemblyAddressItem, assemblyFindReferences });
            contextMenuAssembly.Name = "contextMenuAssembly";
            contextMenuAssembly.Size = new System.Drawing.Size(197, 70);
            // 
            // gotoAssemblyAddressItem
            // 
            gotoAssemblyAddressItem.Name = "gotoAssemblyAddressItem";
            gotoAssemblyAddressItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G;
            gotoAssemblyAddressItem.Size = new System.Drawing.Size(196, 22);
            gotoAssemblyAddressItem.Text = "Goto Address...";
            gotoAssemblyAddressItem.Click += GotoAddressItem_Click;
            // 
            // stackAddressScroller
            // 
            stackAddressScroller.AccessibleDescription = "";
            stackAddressScroller.Dock = System.Windows.Forms.DockStyle.Right;
            stackAddressScroller.LargeChange = 32;
            stackAddressScroller.Location = new System.Drawing.Point(99, 19);
            stackAddressScroller.Maximum = 65535;
            stackAddressScroller.Name = "stackAddressScroller";
            stackAddressScroller.Size = new System.Drawing.Size(21, 382);
            stackAddressScroller.TabIndex = 5;
            stackAddressScroller.Scroll += StackAddressScroller_Scroll;
            // 
            // groupBoxRegisters
            // 
            groupBoxRegisters.Controls.Add(panel1);
            groupBoxRegisters.Dock = System.Windows.Forms.DockStyle.Right;
            groupBoxRegisters.Location = new System.Drawing.Point(693, 0);
            groupBoxRegisters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxRegisters.Name = "groupBoxRegisters";
            groupBoxRegisters.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxRegisters.Size = new System.Drawing.Size(309, 484);
            groupBoxRegisters.TabIndex = 2;
            groupBoxRegisters.TabStop = false;
            groupBoxRegisters.Text = "Registers";
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(checkBoxIFF2);
            panel1.Controls.Add(checkBoxIFF1);
            panel1.Controls.Add(textBoxTiming);
            panel1.Controls.Add(textBoxRegPC);
            panel1.Controls.Add(textBoxRegSP);
            panel1.Controls.Add(textBoxRegIY);
            panel1.Controls.Add(textBoxRegIX);
            panel1.Controls.Add(textBoxRegIYL);
            panel1.Controls.Add(textBoxRegIXL);
            panel1.Controls.Add(textBoxStates);
            panel1.Controls.Add(textBoxRegR);
            panel1.Controls.Add(textBoxRegI);
            panel1.Controls.Add(textBoxRegIXH);
            panel1.Controls.Add(textBoxRegIYH);
            panel1.Controls.Add(label12);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(label13);
            panel1.Controls.Add(label32);
            panel1.Controls.Add(label15);
            panel1.Controls.Add(label22);
            panel1.Controls.Add(label16);
            panel1.Controls.Add(label19);
            panel1.Controls.Add(label20);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label14);
            panel1.Controls.Add(signedCheckBox);
            panel1.Controls.Add(hexadecimalCheckBox);
            panel1.Controls.Add(tabRegisters);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(4, 19);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(301, 462);
            panel1.TabIndex = 49;
            // 
            // checkBoxIFF2
            // 
            checkBoxIFF2.AutoSize = true;
            checkBoxIFF2.Location = new System.Drawing.Point(113, 364);
            checkBoxIFF2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxIFF2.Name = "checkBoxIFF2";
            checkBoxIFF2.Size = new System.Drawing.Size(47, 19);
            checkBoxIFF2.TabIndex = 48;
            checkBoxIFF2.Text = "IFF2";
            checkBoxIFF2.UseVisualStyleBackColor = true;
            // 
            // checkBoxIFF1
            // 
            checkBoxIFF1.AutoSize = true;
            checkBoxIFF1.Location = new System.Drawing.Point(113, 334);
            checkBoxIFF1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxIFF1.Name = "checkBoxIFF1";
            checkBoxIFF1.Size = new System.Drawing.Size(47, 19);
            checkBoxIFF1.TabIndex = 47;
            checkBoxIFF1.Text = "IFF1";
            checkBoxIFF1.UseVisualStyleBackColor = true;
            // 
            // textBoxTiming
            // 
            textBoxTiming.Location = new System.Drawing.Point(63, 435);
            textBoxTiming.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxTiming.Multiline = true;
            textBoxTiming.Name = "textBoxTiming";
            textBoxTiming.Size = new System.Drawing.Size(159, 84);
            textBoxTiming.TabIndex = 34;
            textBoxTiming.WordWrap = false;
            // 
            // textBoxRegPC
            // 
            textBoxRegPC.Location = new System.Drawing.Point(197, 360);
            textBoxRegPC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegPC.Name = "textBoxRegPC";
            textBoxRegPC.Size = new System.Drawing.Size(60, 23);
            textBoxRegPC.TabIndex = 21;
            textBoxRegPC.Enter += RegisterEnter;
            textBoxRegPC.KeyPress += RegisterKeyPress;
            textBoxRegPC.Leave += RegisterLeave;
            textBoxRegPC.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegSP
            // 
            textBoxRegSP.Location = new System.Drawing.Point(197, 330);
            textBoxRegSP.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegSP.Name = "textBoxRegSP";
            textBoxRegSP.Size = new System.Drawing.Size(60, 23);
            textBoxRegSP.TabIndex = 19;
            textBoxRegSP.Enter += RegisterEnter;
            textBoxRegSP.KeyPress += RegisterKeyPress;
            textBoxRegSP.Leave += RegisterLeave;
            textBoxRegSP.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegIY
            // 
            textBoxRegIY.Location = new System.Drawing.Point(197, 300);
            textBoxRegIY.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegIY.Name = "textBoxRegIY";
            textBoxRegIY.Size = new System.Drawing.Size(60, 23);
            textBoxRegIY.TabIndex = 17;
            textBoxRegIY.Enter += RegisterEnter;
            textBoxRegIY.KeyPress += RegisterKeyPress;
            textBoxRegIY.Leave += RegisterLeave;
            textBoxRegIY.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegIX
            // 
            textBoxRegIX.Location = new System.Drawing.Point(197, 270);
            textBoxRegIX.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegIX.Name = "textBoxRegIX";
            textBoxRegIX.Size = new System.Drawing.Size(60, 23);
            textBoxRegIX.TabIndex = 14;
            textBoxRegIX.Enter += RegisterEnter;
            textBoxRegIX.KeyPress += RegisterKeyPress;
            textBoxRegIX.Leave += RegisterLeave;
            textBoxRegIX.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegIYL
            // 
            textBoxRegIYL.Location = new System.Drawing.Point(113, 300);
            textBoxRegIYL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegIYL.Name = "textBoxRegIYL";
            textBoxRegIYL.Size = new System.Drawing.Size(50, 23);
            textBoxRegIYL.TabIndex = 16;
            textBoxRegIYL.Enter += RegisterEnter;
            textBoxRegIYL.KeyPress += RegisterKeyPress;
            textBoxRegIYL.Leave += RegisterLeave;
            textBoxRegIYL.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegIXL
            // 
            textBoxRegIXL.Location = new System.Drawing.Point(113, 270);
            textBoxRegIXL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegIXL.Name = "textBoxRegIXL";
            textBoxRegIXL.Size = new System.Drawing.Size(50, 23);
            textBoxRegIXL.TabIndex = 13;
            textBoxRegIXL.Enter += RegisterEnter;
            textBoxRegIXL.KeyPress += RegisterKeyPress;
            textBoxRegIXL.Leave += RegisterLeave;
            textBoxRegIXL.MouseUp += RegisterMouseUp;
            // 
            // textBoxStates
            // 
            textBoxStates.Location = new System.Drawing.Point(63, 404);
            textBoxStates.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxStates.Name = "textBoxStates";
            textBoxStates.Size = new System.Drawing.Size(50, 23);
            textBoxStates.TabIndex = 20;
            // 
            // textBoxRegR
            // 
            textBoxRegR.Location = new System.Drawing.Point(33, 360);
            textBoxRegR.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegR.Name = "textBoxRegR";
            textBoxRegR.Size = new System.Drawing.Size(50, 23);
            textBoxRegR.TabIndex = 20;
            textBoxRegR.Enter += RegisterEnter;
            textBoxRegR.KeyPress += RegisterKeyPress;
            textBoxRegR.Leave += RegisterLeave;
            textBoxRegR.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegI
            // 
            textBoxRegI.Location = new System.Drawing.Point(33, 330);
            textBoxRegI.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegI.Name = "textBoxRegI";
            textBoxRegI.Size = new System.Drawing.Size(50, 23);
            textBoxRegI.TabIndex = 18;
            textBoxRegI.Enter += RegisterEnter;
            textBoxRegI.KeyPress += RegisterKeyPress;
            textBoxRegI.Leave += RegisterLeave;
            textBoxRegI.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegIXH
            // 
            textBoxRegIXH.Location = new System.Drawing.Point(33, 270);
            textBoxRegIXH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegIXH.Name = "textBoxRegIXH";
            textBoxRegIXH.Size = new System.Drawing.Size(50, 23);
            textBoxRegIXH.TabIndex = 12;
            textBoxRegIXH.Enter += RegisterEnter;
            textBoxRegIXH.KeyPress += RegisterKeyPress;
            textBoxRegIXH.Leave += RegisterLeave;
            textBoxRegIXH.MouseUp += RegisterMouseUp;
            // 
            // textBoxRegIYH
            // 
            textBoxRegIYH.Location = new System.Drawing.Point(33, 300);
            textBoxRegIYH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxRegIYH.Name = "textBoxRegIYH";
            textBoxRegIYH.Size = new System.Drawing.Size(50, 23);
            textBoxRegIYH.TabIndex = 15;
            textBoxRegIYH.Enter += RegisterEnter;
            textBoxRegIYH.KeyPress += RegisterKeyPress;
            textBoxRegIYH.Leave += RegisterLeave;
            textBoxRegIYH.MouseUp += RegisterMouseUp;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(173, 364);
            label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(22, 15);
            label12.TabIndex = 30;
            label12.Text = "PC";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(173, 334);
            label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(20, 15);
            label11.TabIndex = 31;
            label11.Text = "SP";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(178, 304);
            label18.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(17, 15);
            label18.TabIndex = 32;
            label18.Text = "IY";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(5, 274);
            label13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(26, 15);
            label13.TabIndex = 33;
            label13.Text = "IXH";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new System.Drawing.Point(11, 438);
            label32.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label32.Name = "label32";
            label32.Size = new System.Drawing.Size(44, 15);
            label32.TabIndex = 25;
            label32.Text = "Timing";
            label32.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(178, 274);
            label15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(17, 15);
            label15.TabIndex = 27;
            label15.Text = "IX";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(11, 409);
            label22.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(38, 15);
            label22.TabIndex = 25;
            label22.Text = "States";
            label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(5, 304);
            label16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(26, 15);
            label16.TabIndex = 26;
            label16.Text = "IYH";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(16, 364);
            label19.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(14, 15);
            label19.TabIndex = 25;
            label19.Text = "R";
            label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(19, 334);
            label20.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(10, 15);
            label20.TabIndex = 24;
            label20.Text = "I";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(88, 304);
            label17.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(23, 15);
            label17.TabIndex = 29;
            label17.Text = "IYL";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(88, 274);
            label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(23, 15);
            label14.TabIndex = 23;
            label14.Text = "IXL";
            // 
            // signedCheckBox
            // 
            signedCheckBox.AutoSize = true;
            signedCheckBox.Checked = true;
            signedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            signedCheckBox.Enabled = false;
            signedCheckBox.Location = new System.Drawing.Point(107, 3);
            signedCheckBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            signedCheckBox.Name = "signedCheckBox";
            signedCheckBox.Size = new System.Drawing.Size(62, 19);
            signedCheckBox.TabIndex = 0;
            signedCheckBox.Text = "Signed";
            signedCheckBox.UseVisualStyleBackColor = true;
            signedCheckBox.CheckedChanged += SignedCheckBox_CheckedChanged;
            // 
            // hexadecimalCheckBox
            // 
            hexadecimalCheckBox.AutoSize = true;
            hexadecimalCheckBox.Checked = true;
            hexadecimalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            hexadecimalCheckBox.Location = new System.Drawing.Point(5, 3);
            hexadecimalCheckBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            hexadecimalCheckBox.Name = "hexadecimalCheckBox";
            hexadecimalCheckBox.Size = new System.Drawing.Size(95, 19);
            hexadecimalCheckBox.TabIndex = 0;
            hexadecimalCheckBox.Text = "Hexadecimal";
            hexadecimalCheckBox.UseVisualStyleBackColor = true;
            hexadecimalCheckBox.CheckedChanged += HexadecimalCheckBox_CheckedChanged;
            // 
            // tabRegisters
            // 
            tabRegisters.Controls.Add(tabRegisterSet1);
            tabRegisters.Controls.Add(tabRegisterSet2);
            tabRegisters.Location = new System.Drawing.Point(5, 30);
            tabRegisters.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            tabRegisters.Name = "tabRegisters";
            tabRegisters.SelectedIndex = 0;
            tabRegisters.Size = new System.Drawing.Size(266, 229);
            tabRegisters.TabIndex = 1;
            // 
            // tabRegisterSet1
            // 
            tabRegisterSet1.BackColor = System.Drawing.SystemColors.ButtonFace;
            tabRegisterSet1.Controls.Add(checkBoxSet0FlagS);
            tabRegisterSet1.Controls.Add(checkBoxSet0FlagPV);
            tabRegisterSet1.Controls.Add(checkBoxSet0FlagN);
            tabRegisterSet1.Controls.Add(checkBoxSet0FlagHC);
            tabRegisterSet1.Controls.Add(checkBoxSet0FlagCY);
            tabRegisterSet1.Controls.Add(checkBoxSet0FlagZ);
            tabRegisterSet1.Controls.Add(label21);
            tabRegisterSet1.Controls.Add(label23);
            tabRegisterSet1.Controls.Add(label24);
            tabRegisterSet1.Controls.Add(label25);
            tabRegisterSet1.Controls.Add(label26);
            tabRegisterSet1.Controls.Add(label27);
            tabRegisterSet1.Controls.Add(label28);
            tabRegisterSet1.Controls.Add(label29);
            tabRegisterSet1.Controls.Add(label30);
            tabRegisterSet1.Controls.Add(label31);
            tabRegisterSet1.Controls.Add(textBoxSet0RegA);
            tabRegisterSet1.Controls.Add(textBoxSet0RegB);
            tabRegisterSet1.Controls.Add(textBoxSet0RegC);
            tabRegisterSet1.Controls.Add(textBoxSet0RegD);
            tabRegisterSet1.Controls.Add(textBoxSet0RegE);
            tabRegisterSet1.Controls.Add(textBoxSet0RegH);
            tabRegisterSet1.Controls.Add(textBoxSet0RegL);
            tabRegisterSet1.Controls.Add(textBoxSet0RegBC);
            tabRegisterSet1.Controls.Add(textBoxSet0RegDE);
            tabRegisterSet1.Controls.Add(textBoxSet0RegHL);
            tabRegisterSet1.Location = new System.Drawing.Point(4, 24);
            tabRegisterSet1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            tabRegisterSet1.Name = "tabRegisterSet1";
            tabRegisterSet1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            tabRegisterSet1.Size = new System.Drawing.Size(258, 201);
            tabRegisterSet1.TabIndex = 0;
            tabRegisterSet1.Text = "Register set 1";
            // 
            // checkBoxSet0FlagS
            // 
            checkBoxSet0FlagS.AutoSize = true;
            checkBoxSet0FlagS.Location = new System.Drawing.Point(120, 135);
            checkBoxSet0FlagS.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet0FlagS.Name = "checkBoxSet0FlagS";
            checkBoxSet0FlagS.Size = new System.Drawing.Size(66, 19);
            checkBoxSet0FlagS.TabIndex = 49;
            checkBoxSet0FlagS.Text = "S - Sign";
            checkBoxSet0FlagS.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagPV
            // 
            checkBoxSet0FlagPV.AutoSize = true;
            checkBoxSet0FlagPV.Location = new System.Drawing.Point(120, 155);
            checkBoxSet0FlagPV.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet0FlagPV.Name = "checkBoxSet0FlagPV";
            checkBoxSet0FlagPV.Size = new System.Drawing.Size(134, 19);
            checkBoxSet0FlagPV.TabIndex = 50;
            checkBoxSet0FlagPV.Text = "PV - Parity/Overflow";
            checkBoxSet0FlagPV.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagN
            // 
            checkBoxSet0FlagN.AutoSize = true;
            checkBoxSet0FlagN.Location = new System.Drawing.Point(120, 175);
            checkBoxSet0FlagN.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet0FlagN.Name = "checkBoxSet0FlagN";
            checkBoxSet0FlagN.Size = new System.Drawing.Size(93, 19);
            checkBoxSet0FlagN.TabIndex = 51;
            checkBoxSet0FlagN.Text = "N - Add/Sub";
            checkBoxSet0FlagN.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagHC
            // 
            checkBoxSet0FlagHC.AutoSize = true;
            checkBoxSet0FlagHC.Location = new System.Drawing.Point(10, 175);
            checkBoxSet0FlagHC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet0FlagHC.Name = "checkBoxSet0FlagHC";
            checkBoxSet0FlagHC.Size = new System.Drawing.Size(107, 19);
            checkBoxSet0FlagHC.TabIndex = 48;
            checkBoxSet0FlagHC.Text = "HC - Half Carry";
            checkBoxSet0FlagHC.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagCY
            // 
            checkBoxSet0FlagCY.AutoSize = true;
            checkBoxSet0FlagCY.Location = new System.Drawing.Point(10, 155);
            checkBoxSet0FlagCY.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet0FlagCY.Name = "checkBoxSet0FlagCY";
            checkBoxSet0FlagCY.Size = new System.Drawing.Size(80, 19);
            checkBoxSet0FlagCY.TabIndex = 47;
            checkBoxSet0FlagCY.Text = "CY - Carry";
            checkBoxSet0FlagCY.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagZ
            // 
            checkBoxSet0FlagZ.AutoSize = true;
            checkBoxSet0FlagZ.Location = new System.Drawing.Point(10, 135);
            checkBoxSet0FlagZ.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet0FlagZ.Name = "checkBoxSet0FlagZ";
            checkBoxSet0FlagZ.Size = new System.Drawing.Size(68, 19);
            checkBoxSet0FlagZ.TabIndex = 46;
            checkBoxSet0FlagZ.Text = "Z - Zero";
            checkBoxSet0FlagZ.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new System.Drawing.Point(4, 10);
            label21.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(14, 15);
            label21.TabIndex = 36;
            label21.Text = "B";
            label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new System.Drawing.Point(4, 40);
            label23.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(15, 15);
            label23.TabIndex = 37;
            label23.Text = "D";
            label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(84, 10);
            label24.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(15, 15);
            label24.TabIndex = 38;
            label24.Text = "C";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(4, 70);
            label25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(16, 15);
            label25.TabIndex = 39;
            label25.Text = "H";
            label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new System.Drawing.Point(164, 70);
            label26.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label26.Name = "label26";
            label26.Size = new System.Drawing.Size(22, 15);
            label26.TabIndex = 40;
            label26.Text = "HL";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new System.Drawing.Point(164, 40);
            label27.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label27.Name = "label27";
            label27.Size = new System.Drawing.Size(21, 15);
            label27.TabIndex = 41;
            label27.Text = "DE";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new System.Drawing.Point(84, 40);
            label28.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label28.Name = "label28";
            label28.Size = new System.Drawing.Size(13, 15);
            label28.TabIndex = 42;
            label28.Text = "E";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new System.Drawing.Point(164, 10);
            label29.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(22, 15);
            label29.TabIndex = 43;
            label29.Text = "BC";
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new System.Drawing.Point(84, 70);
            label30.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label30.Name = "label30";
            label30.Size = new System.Drawing.Size(13, 15);
            label30.TabIndex = 44;
            label30.Text = "L";
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new System.Drawing.Point(4, 100);
            label31.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label31.Name = "label31";
            label31.Size = new System.Drawing.Size(15, 15);
            label31.TabIndex = 45;
            label31.Text = "A";
            label31.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxSet0RegA
            // 
            textBoxSet0RegA.Location = new System.Drawing.Point(24, 96);
            textBoxSet0RegA.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegA.Name = "textBoxSet0RegA";
            textBoxSet0RegA.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegA.TabIndex = 11;
            textBoxSet0RegA.Enter += RegisterEnter;
            textBoxSet0RegA.KeyPress += RegisterKeyPress;
            textBoxSet0RegA.Leave += RegisterLeave;
            textBoxSet0RegA.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegB
            // 
            textBoxSet0RegB.Location = new System.Drawing.Point(24, 6);
            textBoxSet0RegB.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegB.Name = "textBoxSet0RegB";
            textBoxSet0RegB.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegB.TabIndex = 2;
            textBoxSet0RegB.Enter += RegisterEnter;
            textBoxSet0RegB.KeyPress += RegisterKeyPress;
            textBoxSet0RegB.Leave += RegisterLeave;
            textBoxSet0RegB.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegC
            // 
            textBoxSet0RegC.Location = new System.Drawing.Point(104, 6);
            textBoxSet0RegC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegC.Name = "textBoxSet0RegC";
            textBoxSet0RegC.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegC.TabIndex = 3;
            textBoxSet0RegC.Enter += RegisterEnter;
            textBoxSet0RegC.KeyPress += RegisterKeyPress;
            textBoxSet0RegC.Leave += RegisterLeave;
            textBoxSet0RegC.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegD
            // 
            textBoxSet0RegD.Location = new System.Drawing.Point(24, 36);
            textBoxSet0RegD.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegD.Name = "textBoxSet0RegD";
            textBoxSet0RegD.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegD.TabIndex = 5;
            textBoxSet0RegD.Enter += RegisterEnter;
            textBoxSet0RegD.KeyPress += RegisterKeyPress;
            textBoxSet0RegD.Leave += RegisterLeave;
            textBoxSet0RegD.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegE
            // 
            textBoxSet0RegE.Location = new System.Drawing.Point(104, 36);
            textBoxSet0RegE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegE.Name = "textBoxSet0RegE";
            textBoxSet0RegE.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegE.TabIndex = 6;
            textBoxSet0RegE.Enter += RegisterEnter;
            textBoxSet0RegE.KeyPress += RegisterKeyPress;
            textBoxSet0RegE.Leave += RegisterLeave;
            textBoxSet0RegE.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegH
            // 
            textBoxSet0RegH.Location = new System.Drawing.Point(24, 66);
            textBoxSet0RegH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegH.Name = "textBoxSet0RegH";
            textBoxSet0RegH.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegH.TabIndex = 8;
            textBoxSet0RegH.Enter += RegisterEnter;
            textBoxSet0RegH.KeyPress += RegisterKeyPress;
            textBoxSet0RegH.Leave += RegisterLeave;
            textBoxSet0RegH.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegL
            // 
            textBoxSet0RegL.Location = new System.Drawing.Point(104, 66);
            textBoxSet0RegL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegL.Name = "textBoxSet0RegL";
            textBoxSet0RegL.Size = new System.Drawing.Size(50, 23);
            textBoxSet0RegL.TabIndex = 9;
            textBoxSet0RegL.Enter += RegisterEnter;
            textBoxSet0RegL.KeyPress += RegisterKeyPress;
            textBoxSet0RegL.Leave += RegisterLeave;
            textBoxSet0RegL.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegBC
            // 
            textBoxSet0RegBC.Location = new System.Drawing.Point(188, 6);
            textBoxSet0RegBC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegBC.Name = "textBoxSet0RegBC";
            textBoxSet0RegBC.Size = new System.Drawing.Size(60, 23);
            textBoxSet0RegBC.TabIndex = 4;
            textBoxSet0RegBC.Enter += RegisterEnter;
            textBoxSet0RegBC.KeyPress += RegisterKeyPress;
            textBoxSet0RegBC.Leave += RegisterLeave;
            textBoxSet0RegBC.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegDE
            // 
            textBoxSet0RegDE.Location = new System.Drawing.Point(188, 36);
            textBoxSet0RegDE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegDE.Name = "textBoxSet0RegDE";
            textBoxSet0RegDE.Size = new System.Drawing.Size(60, 23);
            textBoxSet0RegDE.TabIndex = 7;
            textBoxSet0RegDE.Enter += RegisterEnter;
            textBoxSet0RegDE.KeyPress += RegisterKeyPress;
            textBoxSet0RegDE.Leave += RegisterLeave;
            textBoxSet0RegDE.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet0RegHL
            // 
            textBoxSet0RegHL.Location = new System.Drawing.Point(188, 66);
            textBoxSet0RegHL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet0RegHL.Name = "textBoxSet0RegHL";
            textBoxSet0RegHL.Size = new System.Drawing.Size(60, 23);
            textBoxSet0RegHL.TabIndex = 10;
            textBoxSet0RegHL.Enter += RegisterEnter;
            textBoxSet0RegHL.KeyPress += RegisterKeyPress;
            textBoxSet0RegHL.Leave += RegisterLeave;
            textBoxSet0RegHL.MouseUp += RegisterMouseUp;
            // 
            // tabRegisterSet2
            // 
            tabRegisterSet2.BackColor = System.Drawing.SystemColors.ButtonFace;
            tabRegisterSet2.Controls.Add(checkBoxSet1FlagS);
            tabRegisterSet2.Controls.Add(checkBoxSet1FlagPV);
            tabRegisterSet2.Controls.Add(checkBoxSet1FlagN);
            tabRegisterSet2.Controls.Add(checkBoxSet1FlagHC);
            tabRegisterSet2.Controls.Add(checkBoxSet1FlagCY);
            tabRegisterSet2.Controls.Add(checkBoxSet1FlagZ);
            tabRegisterSet2.Controls.Add(textBoxSet1RegA);
            tabRegisterSet2.Controls.Add(textBoxSet1RegB);
            tabRegisterSet2.Controls.Add(textBoxSet1RegC);
            tabRegisterSet2.Controls.Add(textBoxSet1RegD);
            tabRegisterSet2.Controls.Add(textBoxSet1RegE);
            tabRegisterSet2.Controls.Add(textBoxSet1RegH);
            tabRegisterSet2.Controls.Add(textBoxSet1RegL);
            tabRegisterSet2.Controls.Add(textBoxSet1RegBC);
            tabRegisterSet2.Controls.Add(textBoxSet1RegDE);
            tabRegisterSet2.Controls.Add(textBoxSet1RegHL);
            tabRegisterSet2.Controls.Add(label1);
            tabRegisterSet2.Controls.Add(label4);
            tabRegisterSet2.Controls.Add(label2);
            tabRegisterSet2.Controls.Add(label7);
            tabRegisterSet2.Controls.Add(label9);
            tabRegisterSet2.Controls.Add(label6);
            tabRegisterSet2.Controls.Add(label5);
            tabRegisterSet2.Controls.Add(label3);
            tabRegisterSet2.Controls.Add(label8);
            tabRegisterSet2.Controls.Add(label10);
            tabRegisterSet2.Location = new System.Drawing.Point(4, 24);
            tabRegisterSet2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            tabRegisterSet2.Name = "tabRegisterSet2";
            tabRegisterSet2.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            tabRegisterSet2.Size = new System.Drawing.Size(258, 201);
            tabRegisterSet2.TabIndex = 1;
            tabRegisterSet2.Text = "Register set 2";
            // 
            // checkBoxSet1FlagS
            // 
            checkBoxSet1FlagS.AutoSize = true;
            checkBoxSet1FlagS.Location = new System.Drawing.Point(120, 135);
            checkBoxSet1FlagS.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet1FlagS.Name = "checkBoxSet1FlagS";
            checkBoxSet1FlagS.Size = new System.Drawing.Size(66, 19);
            checkBoxSet1FlagS.TabIndex = 55;
            checkBoxSet1FlagS.Text = "S - Sign";
            checkBoxSet1FlagS.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagPV
            // 
            checkBoxSet1FlagPV.AutoSize = true;
            checkBoxSet1FlagPV.Location = new System.Drawing.Point(120, 155);
            checkBoxSet1FlagPV.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet1FlagPV.Name = "checkBoxSet1FlagPV";
            checkBoxSet1FlagPV.Size = new System.Drawing.Size(134, 19);
            checkBoxSet1FlagPV.TabIndex = 56;
            checkBoxSet1FlagPV.Text = "PV - Parity/Overflow";
            checkBoxSet1FlagPV.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagN
            // 
            checkBoxSet1FlagN.AutoSize = true;
            checkBoxSet1FlagN.Location = new System.Drawing.Point(120, 175);
            checkBoxSet1FlagN.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet1FlagN.Name = "checkBoxSet1FlagN";
            checkBoxSet1FlagN.Size = new System.Drawing.Size(93, 19);
            checkBoxSet1FlagN.TabIndex = 57;
            checkBoxSet1FlagN.Text = "N - Add/Sub";
            checkBoxSet1FlagN.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagHC
            // 
            checkBoxSet1FlagHC.AutoSize = true;
            checkBoxSet1FlagHC.Location = new System.Drawing.Point(10, 175);
            checkBoxSet1FlagHC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet1FlagHC.Name = "checkBoxSet1FlagHC";
            checkBoxSet1FlagHC.Size = new System.Drawing.Size(107, 19);
            checkBoxSet1FlagHC.TabIndex = 54;
            checkBoxSet1FlagHC.Text = "HC - Half Carry";
            checkBoxSet1FlagHC.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagCY
            // 
            checkBoxSet1FlagCY.AutoSize = true;
            checkBoxSet1FlagCY.Location = new System.Drawing.Point(10, 155);
            checkBoxSet1FlagCY.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet1FlagCY.Name = "checkBoxSet1FlagCY";
            checkBoxSet1FlagCY.Size = new System.Drawing.Size(80, 19);
            checkBoxSet1FlagCY.TabIndex = 53;
            checkBoxSet1FlagCY.Text = "CY - Carry";
            checkBoxSet1FlagCY.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagZ
            // 
            checkBoxSet1FlagZ.AutoSize = true;
            checkBoxSet1FlagZ.Location = new System.Drawing.Point(9, 135);
            checkBoxSet1FlagZ.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            checkBoxSet1FlagZ.Name = "checkBoxSet1FlagZ";
            checkBoxSet1FlagZ.Size = new System.Drawing.Size(68, 19);
            checkBoxSet1FlagZ.TabIndex = 52;
            checkBoxSet1FlagZ.Text = "Z - Zero";
            checkBoxSet1FlagZ.UseVisualStyleBackColor = true;
            // 
            // textBoxSet1RegA
            // 
            textBoxSet1RegA.Location = new System.Drawing.Point(24, 96);
            textBoxSet1RegA.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegA.Name = "textBoxSet1RegA";
            textBoxSet1RegA.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegA.TabIndex = 11;
            textBoxSet1RegA.Enter += RegisterEnter;
            textBoxSet1RegA.KeyPress += RegisterKeyPress;
            textBoxSet1RegA.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegA.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegB
            // 
            textBoxSet1RegB.Location = new System.Drawing.Point(24, 6);
            textBoxSet1RegB.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegB.Name = "textBoxSet1RegB";
            textBoxSet1RegB.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegB.TabIndex = 2;
            textBoxSet1RegB.Enter += RegisterEnter;
            textBoxSet1RegB.KeyPress += RegisterKeyPress;
            textBoxSet1RegB.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegB.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegC
            // 
            textBoxSet1RegC.Location = new System.Drawing.Point(104, 6);
            textBoxSet1RegC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegC.Name = "textBoxSet1RegC";
            textBoxSet1RegC.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegC.TabIndex = 3;
            textBoxSet1RegC.Enter += RegisterEnter;
            textBoxSet1RegC.KeyPress += RegisterKeyPress;
            textBoxSet1RegC.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegC.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegD
            // 
            textBoxSet1RegD.Location = new System.Drawing.Point(24, 36);
            textBoxSet1RegD.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegD.Name = "textBoxSet1RegD";
            textBoxSet1RegD.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegD.TabIndex = 5;
            textBoxSet1RegD.Enter += RegisterEnter;
            textBoxSet1RegD.KeyPress += RegisterKeyPress;
            textBoxSet1RegD.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegD.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegE
            // 
            textBoxSet1RegE.Location = new System.Drawing.Point(104, 36);
            textBoxSet1RegE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegE.Name = "textBoxSet1RegE";
            textBoxSet1RegE.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegE.TabIndex = 6;
            textBoxSet1RegE.Enter += RegisterEnter;
            textBoxSet1RegE.KeyPress += RegisterKeyPress;
            textBoxSet1RegE.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegE.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegH
            // 
            textBoxSet1RegH.Location = new System.Drawing.Point(24, 66);
            textBoxSet1RegH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegH.Name = "textBoxSet1RegH";
            textBoxSet1RegH.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegH.TabIndex = 8;
            textBoxSet1RegH.Enter += RegisterEnter;
            textBoxSet1RegH.KeyPress += RegisterKeyPress;
            textBoxSet1RegH.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegH.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegL
            // 
            textBoxSet1RegL.Location = new System.Drawing.Point(104, 66);
            textBoxSet1RegL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegL.Name = "textBoxSet1RegL";
            textBoxSet1RegL.Size = new System.Drawing.Size(50, 23);
            textBoxSet1RegL.TabIndex = 9;
            textBoxSet1RegL.Enter += RegisterEnter;
            textBoxSet1RegL.KeyPress += RegisterKeyPress;
            textBoxSet1RegL.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegL.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegBC
            // 
            textBoxSet1RegBC.Location = new System.Drawing.Point(188, 6);
            textBoxSet1RegBC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegBC.Name = "textBoxSet1RegBC";
            textBoxSet1RegBC.Size = new System.Drawing.Size(68, 23);
            textBoxSet1RegBC.TabIndex = 4;
            textBoxSet1RegBC.Enter += RegisterEnter;
            textBoxSet1RegBC.KeyPress += RegisterKeyPress;
            textBoxSet1RegBC.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegBC.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegDE
            // 
            textBoxSet1RegDE.Location = new System.Drawing.Point(188, 36);
            textBoxSet1RegDE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegDE.Name = "textBoxSet1RegDE";
            textBoxSet1RegDE.Size = new System.Drawing.Size(68, 23);
            textBoxSet1RegDE.TabIndex = 7;
            textBoxSet1RegDE.Enter += RegisterEnter;
            textBoxSet1RegDE.KeyPress += RegisterKeyPress;
            textBoxSet1RegDE.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegDE.MouseUp += RegisterMouseUp;
            // 
            // textBoxSet1RegHL
            // 
            textBoxSet1RegHL.Location = new System.Drawing.Point(188, 66);
            textBoxSet1RegHL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            textBoxSet1RegHL.Name = "textBoxSet1RegHL";
            textBoxSet1RegHL.Size = new System.Drawing.Size(68, 23);
            textBoxSet1RegHL.TabIndex = 10;
            textBoxSet1RegHL.Enter += RegisterEnter;
            textBoxSet1RegHL.KeyPress += RegisterKeyPress;
            textBoxSet1RegHL.Leave += HexadecimalCheckBox_CheckedChanged;
            textBoxSet1RegHL.MouseUp += RegisterMouseUp;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 10);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(14, 15);
            label1.TabIndex = 0;
            label1.Text = "B";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 40);
            label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(15, 15);
            label4.TabIndex = 0;
            label4.Text = "D";
            label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(84, 10);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(15, 15);
            label2.TabIndex = 0;
            label2.Text = "C";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(4, 70);
            label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(16, 15);
            label7.TabIndex = 0;
            label7.Text = "H";
            label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(164, 70);
            label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(22, 15);
            label9.TabIndex = 0;
            label9.Text = "HL";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(164, 40);
            label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(21, 15);
            label6.TabIndex = 0;
            label6.Text = "DE";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(84, 40);
            label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(13, 15);
            label5.TabIndex = 0;
            label5.Text = "E";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(164, 10);
            label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(22, 15);
            label3.TabIndex = 0;
            label3.Text = "BC";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(84, 70);
            label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(13, 15);
            label8.TabIndex = 0;
            label8.Text = "L";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(4, 100);
            label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(15, 15);
            label10.TabIndex = 35;
            label10.Text = "A";
            label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBoxDisassembly
            // 
            groupBoxDisassembly.Controls.Add(disassemblyListBox);
            groupBoxDisassembly.Controls.Add(disassemblyAddressScroller);
            groupBoxDisassembly.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxDisassembly.Location = new System.Drawing.Point(0, 80);
            groupBoxDisassembly.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxDisassembly.Name = "groupBoxDisassembly";
            groupBoxDisassembly.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxDisassembly.Size = new System.Drawing.Size(569, 404);
            groupBoxDisassembly.TabIndex = 3;
            groupBoxDisassembly.TabStop = false;
            groupBoxDisassembly.Text = "Disassembly";
            // 
            // disassemblyListBox
            // 
            disassemblyListBox.ContextMenuStrip = contextMenuAssembly;
            disassemblyListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            disassemblyListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            disassemblyListBox.Font = new System.Drawing.Font("Consolas", 9F);
            disassemblyListBox.FormattingEnabled = true;
            disassemblyListBox.ItemHeight = 14;
            disassemblyListBox.Location = new System.Drawing.Point(4, 19);
            disassemblyListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            disassemblyListBox.Name = "disassemblyListBox";
            disassemblyListBox.SelectedAddress = null;
            disassemblyListBox.Size = new System.Drawing.Size(540, 382);
            disassemblyListBox.TabIndex = 0;
            disassemblyListBox.DrawItem += DisassemblyListBox_DrawItem;
            disassemblyListBox.MeasureItem += DisassemblyListBox_MeasureItem;
            disassemblyListBox.MouseDown += DisassemblyListBox_MouseDown;
            disassemblyListBox.MouseMove += DisassemblyListBox_MouseMove;
            disassemblyListBox.MouseUp += DisassemblyListBox_MouseUp;
            // 
            // disassemblyAddressScroller
            // 
            disassemblyAddressScroller.AccessibleDescription = "";
            disassemblyAddressScroller.Dock = System.Windows.Forms.DockStyle.Right;
            disassemblyAddressScroller.LargeChange = 32;
            disassemblyAddressScroller.Location = new System.Drawing.Point(544, 19);
            disassemblyAddressScroller.Maximum = 65535;
            disassemblyAddressScroller.Name = "disassemblyAddressScroller";
            disassemblyAddressScroller.Size = new System.Drawing.Size(21, 382);
            disassemblyAddressScroller.TabIndex = 4;
            disassemblyAddressScroller.Scroll += DisassemblyAddressScroller_Scroll;
            // 
            // groupBoxMemory
            // 
            groupBoxMemory.Controls.Add(memoryListBox);
            groupBoxMemory.Controls.Add(memoryAddressScroller);
            groupBoxMemory.Dock = System.Windows.Forms.DockStyle.Bottom;
            groupBoxMemory.Location = new System.Drawing.Point(0, 533);
            groupBoxMemory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxMemory.Name = "groupBoxMemory";
            groupBoxMemory.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxMemory.Size = new System.Drawing.Size(1002, 172);
            groupBoxMemory.TabIndex = 5;
            groupBoxMemory.TabStop = false;
            groupBoxMemory.Text = "Memory";
            // 
            // memoryListBox
            // 
            memoryListBox.ContextMenuStrip = contextMenuMemory;
            memoryListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            memoryListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            memoryListBox.Font = new System.Drawing.Font("Consolas", 9F);
            memoryListBox.FormattingEnabled = true;
            memoryListBox.Location = new System.Drawing.Point(4, 19);
            memoryListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            memoryListBox.Name = "memoryListBox";
            memoryListBox.SelectedAddress = null;
            memoryListBox.Size = new System.Drawing.Size(973, 150);
            memoryListBox.TabIndex = 0;
            memoryListBox.DrawItem += MemoryListBox_DrawItem;
            // 
            // contextMenuMemory
            // 
            contextMenuMemory.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuMemory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { gotoMemoryAddressItem });
            contextMenuMemory.Name = "contextMenuAssembly";
            contextMenuMemory.Size = new System.Drawing.Size(197, 26);
            // 
            // gotoMemoryAddressItem
            // 
            gotoMemoryAddressItem.Name = "gotoMemoryAddressItem";
            gotoMemoryAddressItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G;
            gotoMemoryAddressItem.Size = new System.Drawing.Size(196, 22);
            gotoMemoryAddressItem.Text = "Goto Address...";
            gotoMemoryAddressItem.Click += GotoMemoryAddressItem_Click;
            // 
            // memoryAddressScroller
            // 
            memoryAddressScroller.AccessibleDescription = "";
            memoryAddressScroller.Dock = System.Windows.Forms.DockStyle.Right;
            memoryAddressScroller.LargeChange = 64;
            memoryAddressScroller.Location = new System.Drawing.Point(977, 19);
            memoryAddressScroller.Maximum = 65535;
            memoryAddressScroller.Name = "memoryAddressScroller";
            memoryAddressScroller.Size = new System.Drawing.Size(21, 150);
            memoryAddressScroller.SmallChange = 16;
            memoryAddressScroller.TabIndex = 6;
            memoryAddressScroller.Scroll += MemoryAddressScroller_Scroll;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, debugToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(1002, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openRomToolStripMenuItem, toolStripMenuItem1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // openRomToolStripMenuItem
            // 
            openRomToolStripMenuItem.Name = "openRomToolStripMenuItem";
            openRomToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            openRomToolStripMenuItem.Text = "&Open Rom...";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItemRun, menuItemRunToCursor, menuItemStepOver, menuItemStepInto, toolStripMenuItem2, resetToolStripMenuItem, restartToolStripMenuItem, toolStripMenuItem3, menuItemToggleBreakpoint, menuItemBreakpointDialog });
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            debugToolStripMenuItem.Text = "&Debug";
            // 
            // menuItemRun
            // 
            menuItemRun.Image = (System.Drawing.Image)resources.GetObject("menuItemRun.Image");
            menuItemRun.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            menuItemRun.Name = "menuItemRun";
            menuItemRun.ShortcutKeys = System.Windows.Forms.Keys.F5;
            menuItemRun.Size = new System.Drawing.Size(213, 24);
            menuItemRun.Text = "&Run";
            menuItemRun.Click += RunClick;
            // 
            // menuItemRunToCursor
            // 
            menuItemRunToCursor.Image = (System.Drawing.Image)resources.GetObject("menuItemRunToCursor.Image");
            menuItemRunToCursor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            menuItemRunToCursor.Name = "menuItemRunToCursor";
            menuItemRunToCursor.ShortcutKeys = System.Windows.Forms.Keys.F4;
            menuItemRunToCursor.Size = new System.Drawing.Size(213, 24);
            menuItemRunToCursor.Text = "&Run to cursor";
            menuItemRunToCursor.Click += RunToCursorClick;
            // 
            // menuItemStepOver
            // 
            menuItemStepOver.Image = (System.Drawing.Image)resources.GetObject("menuItemStepOver.Image");
            menuItemStepOver.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            menuItemStepOver.Name = "menuItemStepOver";
            menuItemStepOver.ShortcutKeys = System.Windows.Forms.Keys.F10;
            menuItemStepOver.Size = new System.Drawing.Size(213, 24);
            menuItemStepOver.Text = "Step &over";
            menuItemStepOver.Click += StepOverClick;
            // 
            // menuItemStepInto
            // 
            menuItemStepInto.Image = (System.Drawing.Image)resources.GetObject("menuItemStepInto.Image");
            menuItemStepInto.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            menuItemStepInto.Name = "menuItemStepInto";
            menuItemStepInto.ShortcutKeys = System.Windows.Forms.Keys.F11;
            menuItemStepInto.Size = new System.Drawing.Size(213, 24);
            menuItemStepInto.Text = "&Step into";
            menuItemStepInto.Click += StepIntoClick;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(210, 6);
            // 
            // resetToolStripMenuItem
            // 
            resetToolStripMenuItem.Enabled = false;
            resetToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("resetToolStripMenuItem.Image");
            resetToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5;
            resetToolStripMenuItem.Size = new System.Drawing.Size(213, 24);
            resetToolStripMenuItem.Text = "Stop Debugging";
            resetToolStripMenuItem.Click += StopClick;
            // 
            // restartToolStripMenuItem
            // 
            restartToolStripMenuItem.Enabled = false;
            restartToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("restartToolStripMenuItem.Image");
            restartToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            restartToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5;
            restartToolStripMenuItem.Size = new System.Drawing.Size(213, 24);
            restartToolStripMenuItem.Text = "Restart";
            restartToolStripMenuItem.Click += ResetClick;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(210, 6);
            // 
            // menuItemToggleBreakpoint
            // 
            menuItemToggleBreakpoint.Image = (System.Drawing.Image)resources.GetObject("menuItemToggleBreakpoint.Image");
            menuItemToggleBreakpoint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            menuItemToggleBreakpoint.Name = "menuItemToggleBreakpoint";
            menuItemToggleBreakpoint.ShortcutKeys = System.Windows.Forms.Keys.F9;
            menuItemToggleBreakpoint.Size = new System.Drawing.Size(213, 24);
            menuItemToggleBreakpoint.Text = "&Toggle breakpoint";
            menuItemToggleBreakpoint.Click += ToggleBreakPointClick;
            // 
            // menuItemBreakpointDialog
            // 
            menuItemBreakpointDialog.Name = "menuItemBreakpointDialog";
            menuItemBreakpointDialog.Size = new System.Drawing.Size(213, 24);
            menuItemBreakpointDialog.Text = "&Breakpoints...";
            menuItemBreakpointDialog.Click += BreakpointDialogClick;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { runButton, pauseButton, stopButton, resetButton, toolStripSeparator1, runToCursorButton, stepIntoButton, stepOverButton });
            toolStrip1.Location = new System.Drawing.Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1002, 25);
            toolStrip1.TabIndex = 7;
            toolStrip1.Text = "toolStrip1";
            // 
            // runButton
            // 
            runButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            runButton.Image = (System.Drawing.Image)resources.GetObject("runButton.Image");
            runButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            runButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            runButton.Name = "runButton";
            runButton.Size = new System.Drawing.Size(23, 22);
            runButton.Text = "Run";
            runButton.ToolTipText = "Run";
            runButton.Click += RunClick;
            // 
            // pauseButton
            // 
            pauseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            pauseButton.Enabled = false;
            pauseButton.Image = (System.Drawing.Image)resources.GetObject("pauseButton.Image");
            pauseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            pauseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new System.Drawing.Size(23, 22);
            pauseButton.Text = "toolStripButton2";
            pauseButton.Click += PauseClick;
            // 
            // stopButton
            // 
            stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            stopButton.Image = (System.Drawing.Image)resources.GetObject("stopButton.Image");
            stopButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            stopButton.Name = "stopButton";
            stopButton.Size = new System.Drawing.Size(23, 22);
            stopButton.Text = "toolStripButton3";
            stopButton.Click += StopClick;
            // 
            // resetButton
            // 
            resetButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resetButton.Image = (System.Drawing.Image)resources.GetObject("resetButton.Image");
            resetButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            resetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            resetButton.Name = "resetButton";
            resetButton.Size = new System.Drawing.Size(23, 22);
            resetButton.Text = "Reset";
            resetButton.Click += ResetClick;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // runToCursorButton
            // 
            runToCursorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            runToCursorButton.Image = (System.Drawing.Image)resources.GetObject("runToCursorButton.Image");
            runToCursorButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            runToCursorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            runToCursorButton.Name = "runToCursorButton";
            runToCursorButton.Size = new System.Drawing.Size(23, 22);
            runToCursorButton.Text = "Run to cursor";
            runToCursorButton.Click += RunToCursorClick;
            // 
            // stepIntoButton
            // 
            stepIntoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            stepIntoButton.Image = (System.Drawing.Image)resources.GetObject("stepIntoButton.Image");
            stepIntoButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            stepIntoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            stepIntoButton.Name = "stepIntoButton";
            stepIntoButton.Size = new System.Drawing.Size(23, 22);
            stepIntoButton.Text = "Step into";
            stepIntoButton.Click += StepIntoClick;
            // 
            // stepOverButton
            // 
            stepOverButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            stepOverButton.Image = (System.Drawing.Image)resources.GetObject("stepOverButton.Image");
            stepOverButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            stepOverButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            stepOverButton.Name = "stepOverButton";
            stepOverButton.Size = new System.Drawing.Size(23, 22);
            stepOverButton.Text = "Step over";
            stepOverButton.Click += StepOverClick;
            // 
            // groupBoxMemorySelect
            // 
            groupBoxMemorySelect.Controls.Add(memoryCheckedBox);
            groupBoxMemorySelect.Dock = System.Windows.Forms.DockStyle.Top;
            groupBoxMemorySelect.Location = new System.Drawing.Point(0, 0);
            groupBoxMemorySelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxMemorySelect.Name = "groupBoxMemorySelect";
            groupBoxMemorySelect.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxMemorySelect.Size = new System.Drawing.Size(693, 80);
            groupBoxMemorySelect.TabIndex = 3;
            groupBoxMemorySelect.TabStop = false;
            groupBoxMemorySelect.Text = "Memory selector";
            // 
            // memoryCheckedBox
            // 
            memoryCheckedBox.ColumnWidth = 200;
            memoryCheckedBox.FormattingEnabled = true;
            memoryCheckedBox.Items.AddRange(new object[] { "RAM:  0x0000 - 0xFFFF", "LROM: 0x0000 - 0x3FFF", "UROM: 0xC000 - 0xFFFF" });
            memoryCheckedBox.Location = new System.Drawing.Point(7, 22);
            memoryCheckedBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            memoryCheckedBox.MultiColumn = true;
            memoryCheckedBox.Name = "memoryCheckedBox";
            memoryCheckedBox.Size = new System.Drawing.Size(661, 40);
            memoryCheckedBox.TabIndex = 0;
            memoryCheckedBox.ItemCheck += MemoryCheckBox_ItemCheck;
            // 
            // checkBoxUpdateScreen
            // 
            checkBoxUpdateScreen.AutoSize = true;
            checkBoxUpdateScreen.Location = new System.Drawing.Point(312, 37);
            checkBoxUpdateScreen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxUpdateScreen.Name = "checkBoxUpdateScreen";
            checkBoxUpdateScreen.Size = new System.Drawing.Size(102, 19);
            checkBoxUpdateScreen.TabIndex = 8;
            checkBoxUpdateScreen.Text = "Update Screen";
            checkBoxUpdateScreen.UseVisualStyleBackColor = true;
            checkBoxUpdateScreen.CheckedChanged += CheckBoxUpdateScreen_CheckedChanged;
            // 
            // timer1
            // 
            timer1.Interval = 20;
            timer1.Tick += Timer1_Tick;
            // 
            // MainPanel
            // 
            MainPanel.Controls.Add(groupBoxDisassembly);
            MainPanel.Controls.Add(groupBoxStack);
            MainPanel.Controls.Add(groupBoxMemorySelect);
            MainPanel.Controls.Add(groupBoxRegisters);
            MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            MainPanel.Location = new System.Drawing.Point(0, 49);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new System.Drawing.Size(1002, 484);
            MainPanel.TabIndex = 9;
            // 
            // splitter1
            // 
            splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter1.Location = new System.Drawing.Point(0, 530);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(1002, 3);
            splitter1.TabIndex = 10;
            splitter1.TabStop = false;
            // 
            // assemblyFindReferences
            // 
            assemblyFindReferences.Name = "assemblyFindReferences";
            assemblyFindReferences.Size = new System.Drawing.Size(196, 22);
            assemblyFindReferences.Text = "Find References...";
            assemblyFindReferences.Click += FindReferences_Click;

            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1002, 705);
            Controls.Add(splitter1);
            Controls.Add(checkBoxUpdateScreen);
            Controls.Add(MainPanel);
            Controls.Add(groupBoxMemory);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Z80 Emulator";
            groupBoxStack.ResumeLayout(false);
            contextMenuAssembly.ResumeLayout(false);
            groupBoxRegisters.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabRegisters.ResumeLayout(false);
            tabRegisterSet1.ResumeLayout(false);
            tabRegisterSet1.PerformLayout();
            tabRegisterSet2.ResumeLayout(false);
            tabRegisterSet2.PerformLayout();
            groupBoxDisassembly.ResumeLayout(false);
            groupBoxMemory.ResumeLayout(false);
            contextMenuMemory.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            groupBoxMemorySelect.ResumeLayout(false);
            MainPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxStack;
        private System.Windows.Forms.GroupBox groupBoxRegisters;
        private System.Windows.Forms.CheckBox hexadecimalCheckBox;
        private System.Windows.Forms.GroupBox groupBoxDisassembly;
        private VirtualListbox disassemblyListBox;
        private System.Windows.Forms.GroupBox groupBoxMemory;
        private VirtualListbox memoryListBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRomToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem menuItemRun;
        private System.Windows.Forms.ToolStripMenuItem menuItemRunToCursor;
        private System.Windows.Forms.ToolStripMenuItem menuItemStepOver;
        private System.Windows.Forms.ToolStripMenuItem menuItemStepInto;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuItemToggleBreakpoint;
        private System.Windows.Forms.ToolStripMenuItem menuItemBreakpointDialog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton runButton;
        private System.Windows.Forms.ToolStripButton pauseButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripButton resetButton;
        private System.Windows.Forms.ToolStripButton runToCursorButton;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton stepIntoButton;
        private System.Windows.Forms.ToolStripButton stepOverButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.VScrollBar disassemblyAddressScroller;
        private System.Windows.Forms.TabControl tabRegisters;
        private System.Windows.Forms.TabPage tabRegisterSet1;
        private System.Windows.Forms.TextBox textBoxRegPC;
        private System.Windows.Forms.TextBox textBoxRegSP;
        private System.Windows.Forms.TextBox textBoxRegIY;
        private System.Windows.Forms.TextBox textBoxRegIX;
        private System.Windows.Forms.TextBox textBoxRegIYL;
        private System.Windows.Forms.TextBox textBoxRegIXL;
        private System.Windows.Forms.TextBox textBoxRegR;
        private System.Windows.Forms.TextBox textBoxRegI;
        private System.Windows.Forms.TextBox textBoxRegIXH;
        private System.Windows.Forms.TextBox textBoxRegIYH;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabRegisterSet2;
        private System.Windows.Forms.TextBox textBoxSet0RegA;
        private System.Windows.Forms.TextBox textBoxSet0RegB;
        private System.Windows.Forms.TextBox textBoxSet0RegC;
        private System.Windows.Forms.TextBox textBoxSet0RegD;
        private System.Windows.Forms.TextBox textBoxSet0RegE;
        private System.Windows.Forms.TextBox textBoxSet0RegH;
        private System.Windows.Forms.TextBox textBoxSet0RegL;
        private System.Windows.Forms.TextBox textBoxSet0RegBC;
        private System.Windows.Forms.TextBox textBoxSet0RegDE;
        private System.Windows.Forms.TextBox textBoxSet0RegHL;
        private System.Windows.Forms.TextBox textBoxSet1RegA;
        private System.Windows.Forms.TextBox textBoxSet1RegB;
        private System.Windows.Forms.TextBox textBoxSet1RegC;
        private System.Windows.Forms.TextBox textBoxSet1RegD;
        private System.Windows.Forms.TextBox textBoxSet1RegE;
        private System.Windows.Forms.TextBox textBoxSet1RegH;
        private System.Windows.Forms.TextBox textBoxSet1RegL;
        private System.Windows.Forms.TextBox textBoxSet1RegBC;
        private System.Windows.Forms.TextBox textBoxSet1RegDE;
        private System.Windows.Forms.TextBox textBoxSet1RegHL;
        private System.Windows.Forms.CheckBox checkBoxSet0FlagS;
        private System.Windows.Forms.CheckBox checkBoxSet0FlagPV;
        private System.Windows.Forms.CheckBox checkBoxSet0FlagN;
        private System.Windows.Forms.CheckBox checkBoxSet0FlagHC;
        private System.Windows.Forms.CheckBox checkBoxSet0FlagCY;
        private System.Windows.Forms.CheckBox checkBoxSet0FlagZ;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox checkBoxSet1FlagS;
        private System.Windows.Forms.CheckBox checkBoxSet1FlagPV;
        private System.Windows.Forms.CheckBox checkBoxSet1FlagN;
        private System.Windows.Forms.CheckBox checkBoxSet1FlagHC;
        private System.Windows.Forms.CheckBox checkBoxSet1FlagCY;
        private System.Windows.Forms.CheckBox checkBoxSet1FlagZ;
        private System.Windows.Forms.VScrollBar memoryAddressScroller;
        private System.Windows.Forms.CheckBox signedCheckBox;
        private System.Windows.Forms.GroupBox groupBoxMemorySelect;
        private System.Windows.Forms.CheckedListBox memoryCheckedBox;
        private System.Windows.Forms.TextBox textBoxStates;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textBoxTiming;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.CheckBox checkBoxIFF2;
        private System.Windows.Forms.CheckBox checkBoxIFF1;
        private System.Windows.Forms.ContextMenuStrip contextMenuAssembly;
        private System.Windows.Forms.ToolStripMenuItem gotoAssemblyAddressItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuMemory;
        private System.Windows.Forms.ToolStripMenuItem gotoMemoryAddressItem;
        private System.Windows.Forms.CheckBox checkBoxUpdateScreen;
        private System.Windows.Forms.Timer timer1;
        private VirtualListbox stackListBox;
        private System.Windows.Forms.VScrollBar stackAddressScroller;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem assemblyFindReferences;
    }
}

