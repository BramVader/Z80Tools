﻿namespace Z80TestConsole
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBoxStack = new System.Windows.Forms.GroupBox();
            this.stackListBox = new Z80TestConsole.VirtualListbox();
            this.contextMenuAssembly = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gotoAssemblyAddressItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stackAddressScroller = new System.Windows.Forms.VScrollBar();
            this.groupBoxRegisters = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxIFF2 = new System.Windows.Forms.CheckBox();
            this.checkBoxIFF1 = new System.Windows.Forms.CheckBox();
            this.textBoxTiming = new System.Windows.Forms.TextBox();
            this.textBoxRegPC = new System.Windows.Forms.TextBox();
            this.textBoxRegSP = new System.Windows.Forms.TextBox();
            this.textBoxRegIY = new System.Windows.Forms.TextBox();
            this.textBoxRegIX = new System.Windows.Forms.TextBox();
            this.textBoxRegIYL = new System.Windows.Forms.TextBox();
            this.textBoxRegIXL = new System.Windows.Forms.TextBox();
            this.textBoxStates = new System.Windows.Forms.TextBox();
            this.textBoxRegR = new System.Windows.Forms.TextBox();
            this.textBoxRegI = new System.Windows.Forms.TextBox();
            this.textBoxRegIXH = new System.Windows.Forms.TextBox();
            this.textBoxRegIYH = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.signedCheckBox = new System.Windows.Forms.CheckBox();
            this.hexadecimalCheckBox = new System.Windows.Forms.CheckBox();
            this.tabRegisters = new System.Windows.Forms.TabControl();
            this.tabRegisterSet1 = new System.Windows.Forms.TabPage();
            this.checkBoxSet0FlagS = new System.Windows.Forms.CheckBox();
            this.checkBoxSet0FlagPV = new System.Windows.Forms.CheckBox();
            this.checkBoxSet0FlagN = new System.Windows.Forms.CheckBox();
            this.checkBoxSet0FlagHC = new System.Windows.Forms.CheckBox();
            this.checkBoxSet0FlagCY = new System.Windows.Forms.CheckBox();
            this.checkBoxSet0FlagZ = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.textBoxSet0RegA = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegB = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegC = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegD = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegE = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegH = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegL = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegBC = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegDE = new System.Windows.Forms.TextBox();
            this.textBoxSet0RegHL = new System.Windows.Forms.TextBox();
            this.tabRegisterSet2 = new System.Windows.Forms.TabPage();
            this.checkBoxSet1FlagS = new System.Windows.Forms.CheckBox();
            this.checkBoxSet1FlagPV = new System.Windows.Forms.CheckBox();
            this.checkBoxSet1FlagN = new System.Windows.Forms.CheckBox();
            this.checkBoxSet1FlagHC = new System.Windows.Forms.CheckBox();
            this.checkBoxSet1FlagCY = new System.Windows.Forms.CheckBox();
            this.checkBoxSet1FlagZ = new System.Windows.Forms.CheckBox();
            this.textBoxSet1RegA = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegB = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegC = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegD = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegE = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegH = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegL = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegBC = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegDE = new System.Windows.Forms.TextBox();
            this.textBoxSet1RegHL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxDisassembly = new System.Windows.Forms.GroupBox();
            this.disassemblyListBox = new Z80TestConsole.VirtualListbox();
            this.disassemblyAddressScroller = new System.Windows.Forms.VScrollBar();
            this.groupBoxMemory = new System.Windows.Forms.GroupBox();
            this.memoryListBox = new Z80TestConsole.VirtualListbox();
            this.contextMenuMemory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gotoMemoryAddressItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryAddressScroller = new System.Windows.Forms.VScrollBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRun = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRunToCursor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStepOver = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStepInto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemToggleBreakpoint = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemBreakpointDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.runButton = new System.Windows.Forms.ToolStripButton();
            this.pauseButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.resetButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.runToCursorButton = new System.Windows.Forms.ToolStripButton();
            this.stepIntoButton = new System.Windows.Forms.ToolStripButton();
            this.stepOverButton = new System.Windows.Forms.ToolStripButton();
            this.groupBoxMemorySelect = new System.Windows.Forms.GroupBox();
            this.memoryCheckedBox = new System.Windows.Forms.CheckedListBox();
            this.checkBoxUpdateScreen = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.MainPanel = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.groupBoxStack.SuspendLayout();
            this.contextMenuAssembly.SuspendLayout();
            this.groupBoxRegisters.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabRegisters.SuspendLayout();
            this.tabRegisterSet1.SuspendLayout();
            this.tabRegisterSet2.SuspendLayout();
            this.groupBoxDisassembly.SuspendLayout();
            this.groupBoxMemory.SuspendLayout();
            this.contextMenuMemory.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBoxMemorySelect.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxStack
            // 
            this.groupBoxStack.Controls.Add(this.stackListBox);
            this.groupBoxStack.Controls.Add(this.stackAddressScroller);
            this.groupBoxStack.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBoxStack.Location = new System.Drawing.Point(569, 80);
            this.groupBoxStack.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxStack.Name = "groupBoxStack";
            this.groupBoxStack.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxStack.Size = new System.Drawing.Size(124, 404);
            this.groupBoxStack.TabIndex = 1;
            this.groupBoxStack.TabStop = false;
            this.groupBoxStack.Text = "Stack";
            // 
            // stackListBox
            // 
            this.stackListBox.ContextMenuStrip = this.contextMenuAssembly;
            this.stackListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.stackListBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stackListBox.FormattingEnabled = true;
            this.stackListBox.ItemHeight = 14;
            this.stackListBox.Location = new System.Drawing.Point(4, 19);
            this.stackListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stackListBox.Name = "stackListBox";
            this.stackListBox.SelectedAddress = null;
            this.stackListBox.Size = new System.Drawing.Size(95, 382);
            this.stackListBox.TabIndex = 6;
            this.stackListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.StackListBox_DrawItem);
            // 
            // contextMenuAssembly
            // 
            this.contextMenuAssembly.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuAssembly.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoAssemblyAddressItem});
            this.contextMenuAssembly.Name = "contextMenuAssembly";
            this.contextMenuAssembly.Size = new System.Drawing.Size(197, 26);
            // 
            // gotoAssemblyAddressItem
            // 
            this.gotoAssemblyAddressItem.Name = "gotoAssemblyAddressItem";
            this.gotoAssemblyAddressItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoAssemblyAddressItem.Size = new System.Drawing.Size(196, 22);
            this.gotoAssemblyAddressItem.Text = "Goto Address...";
            this.gotoAssemblyAddressItem.Click += new System.EventHandler(this.GotoAddressItem_Click);
            // 
            // stackAddressScroller
            // 
            this.stackAddressScroller.AccessibleDescription = "";
            this.stackAddressScroller.Dock = System.Windows.Forms.DockStyle.Right;
            this.stackAddressScroller.LargeChange = 32;
            this.stackAddressScroller.Location = new System.Drawing.Point(99, 19);
            this.stackAddressScroller.Maximum = 65535;
            this.stackAddressScroller.Name = "stackAddressScroller";
            this.stackAddressScroller.Size = new System.Drawing.Size(21, 382);
            this.stackAddressScroller.TabIndex = 5;
            this.stackAddressScroller.Scroll += new System.Windows.Forms.ScrollEventHandler(this.StackAddressScroller_Scroll);
            // 
            // groupBoxRegisters
            // 
            this.groupBoxRegisters.Controls.Add(this.panel1);
            this.groupBoxRegisters.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBoxRegisters.Location = new System.Drawing.Point(693, 0);
            this.groupBoxRegisters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxRegisters.Name = "groupBoxRegisters";
            this.groupBoxRegisters.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxRegisters.Size = new System.Drawing.Size(309, 484);
            this.groupBoxRegisters.TabIndex = 2;
            this.groupBoxRegisters.TabStop = false;
            this.groupBoxRegisters.Text = "Registers";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.checkBoxIFF2);
            this.panel1.Controls.Add(this.checkBoxIFF1);
            this.panel1.Controls.Add(this.textBoxTiming);
            this.panel1.Controls.Add(this.textBoxRegPC);
            this.panel1.Controls.Add(this.textBoxRegSP);
            this.panel1.Controls.Add(this.textBoxRegIY);
            this.panel1.Controls.Add(this.textBoxRegIX);
            this.panel1.Controls.Add(this.textBoxRegIYL);
            this.panel1.Controls.Add(this.textBoxRegIXL);
            this.panel1.Controls.Add(this.textBoxStates);
            this.panel1.Controls.Add(this.textBoxRegR);
            this.panel1.Controls.Add(this.textBoxRegI);
            this.panel1.Controls.Add(this.textBoxRegIXH);
            this.panel1.Controls.Add(this.textBoxRegIYH);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label32);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.signedCheckBox);
            this.panel1.Controls.Add(this.hexadecimalCheckBox);
            this.panel1.Controls.Add(this.tabRegisters);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 462);
            this.panel1.TabIndex = 49;
            // 
            // checkBoxIFF2
            // 
            this.checkBoxIFF2.AutoSize = true;
            this.checkBoxIFF2.Location = new System.Drawing.Point(113, 364);
            this.checkBoxIFF2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxIFF2.Name = "checkBoxIFF2";
            this.checkBoxIFF2.Size = new System.Drawing.Size(47, 19);
            this.checkBoxIFF2.TabIndex = 48;
            this.checkBoxIFF2.Text = "IFF2";
            this.checkBoxIFF2.UseVisualStyleBackColor = true;
            // 
            // checkBoxIFF1
            // 
            this.checkBoxIFF1.AutoSize = true;
            this.checkBoxIFF1.Location = new System.Drawing.Point(113, 334);
            this.checkBoxIFF1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxIFF1.Name = "checkBoxIFF1";
            this.checkBoxIFF1.Size = new System.Drawing.Size(47, 19);
            this.checkBoxIFF1.TabIndex = 47;
            this.checkBoxIFF1.Text = "IFF1";
            this.checkBoxIFF1.UseVisualStyleBackColor = true;
            // 
            // textBoxTiming
            // 
            this.textBoxTiming.Location = new System.Drawing.Point(63, 435);
            this.textBoxTiming.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxTiming.Multiline = true;
            this.textBoxTiming.Name = "textBoxTiming";
            this.textBoxTiming.Size = new System.Drawing.Size(159, 84);
            this.textBoxTiming.TabIndex = 34;
            this.textBoxTiming.WordWrap = false;
            // 
            // textBoxRegPC
            // 
            this.textBoxRegPC.Location = new System.Drawing.Point(197, 360);
            this.textBoxRegPC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegPC.Name = "textBoxRegPC";
            this.textBoxRegPC.Size = new System.Drawing.Size(60, 23);
            this.textBoxRegPC.TabIndex = 21;
            this.textBoxRegPC.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegPC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegPC.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegPC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegSP
            // 
            this.textBoxRegSP.Location = new System.Drawing.Point(197, 330);
            this.textBoxRegSP.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegSP.Name = "textBoxRegSP";
            this.textBoxRegSP.Size = new System.Drawing.Size(60, 23);
            this.textBoxRegSP.TabIndex = 19;
            this.textBoxRegSP.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegSP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegSP.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegSP.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegIY
            // 
            this.textBoxRegIY.Location = new System.Drawing.Point(197, 300);
            this.textBoxRegIY.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegIY.Name = "textBoxRegIY";
            this.textBoxRegIY.Size = new System.Drawing.Size(60, 23);
            this.textBoxRegIY.TabIndex = 17;
            this.textBoxRegIY.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegIY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegIY.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegIY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegIX
            // 
            this.textBoxRegIX.Location = new System.Drawing.Point(197, 270);
            this.textBoxRegIX.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegIX.Name = "textBoxRegIX";
            this.textBoxRegIX.Size = new System.Drawing.Size(60, 23);
            this.textBoxRegIX.TabIndex = 14;
            this.textBoxRegIX.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegIX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegIX.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegIX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegIYL
            // 
            this.textBoxRegIYL.Location = new System.Drawing.Point(113, 300);
            this.textBoxRegIYL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegIYL.Name = "textBoxRegIYL";
            this.textBoxRegIYL.Size = new System.Drawing.Size(50, 23);
            this.textBoxRegIYL.TabIndex = 16;
            this.textBoxRegIYL.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegIYL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegIYL.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegIYL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegIXL
            // 
            this.textBoxRegIXL.Location = new System.Drawing.Point(113, 270);
            this.textBoxRegIXL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegIXL.Name = "textBoxRegIXL";
            this.textBoxRegIXL.Size = new System.Drawing.Size(50, 23);
            this.textBoxRegIXL.TabIndex = 13;
            this.textBoxRegIXL.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegIXL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegIXL.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegIXL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxStates
            // 
            this.textBoxStates.Location = new System.Drawing.Point(63, 404);
            this.textBoxStates.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxStates.Name = "textBoxStates";
            this.textBoxStates.Size = new System.Drawing.Size(50, 23);
            this.textBoxStates.TabIndex = 20;
            // 
            // textBoxRegR
            // 
            this.textBoxRegR.Location = new System.Drawing.Point(33, 360);
            this.textBoxRegR.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegR.Name = "textBoxRegR";
            this.textBoxRegR.Size = new System.Drawing.Size(50, 23);
            this.textBoxRegR.TabIndex = 20;
            this.textBoxRegR.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegR.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegR.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegR.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegI
            // 
            this.textBoxRegI.Location = new System.Drawing.Point(33, 330);
            this.textBoxRegI.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegI.Name = "textBoxRegI";
            this.textBoxRegI.Size = new System.Drawing.Size(50, 23);
            this.textBoxRegI.TabIndex = 18;
            this.textBoxRegI.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegI.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegI.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegI.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegIXH
            // 
            this.textBoxRegIXH.Location = new System.Drawing.Point(33, 270);
            this.textBoxRegIXH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegIXH.Name = "textBoxRegIXH";
            this.textBoxRegIXH.Size = new System.Drawing.Size(50, 23);
            this.textBoxRegIXH.TabIndex = 12;
            this.textBoxRegIXH.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegIXH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegIXH.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegIXH.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxRegIYH
            // 
            this.textBoxRegIYH.Location = new System.Drawing.Point(33, 300);
            this.textBoxRegIYH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxRegIYH.Name = "textBoxRegIYH";
            this.textBoxRegIYH.Size = new System.Drawing.Size(50, 23);
            this.textBoxRegIYH.TabIndex = 15;
            this.textBoxRegIYH.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxRegIYH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxRegIYH.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxRegIYH.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(173, 364);
            this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 30;
            this.label12.Text = "PC";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(173, 334);
            this.label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 15);
            this.label11.TabIndex = 31;
            this.label11.Text = "SP";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(178, 304);
            this.label18.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(17, 15);
            this.label18.TabIndex = 32;
            this.label18.Text = "IY";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(5, 274);
            this.label13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(26, 15);
            this.label13.TabIndex = 33;
            this.label13.Text = "IXH";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(11, 438);
            this.label32.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(44, 15);
            this.label32.TabIndex = 25;
            this.label32.Text = "Timing";
            this.label32.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(178, 274);
            this.label15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 15);
            this.label15.TabIndex = 27;
            this.label15.Text = "IX";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(11, 409);
            this.label22.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(38, 15);
            this.label22.TabIndex = 25;
            this.label22.Text = "States";
            this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 304);
            this.label16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(26, 15);
            this.label16.TabIndex = 26;
            this.label16.Text = "IYH";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(16, 364);
            this.label19.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(14, 15);
            this.label19.TabIndex = 25;
            this.label19.Text = "R";
            this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(19, 334);
            this.label20.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(10, 15);
            this.label20.TabIndex = 24;
            this.label20.Text = "I";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(88, 304);
            this.label17.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(23, 15);
            this.label17.TabIndex = 29;
            this.label17.Text = "IYL";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(88, 274);
            this.label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 15);
            this.label14.TabIndex = 23;
            this.label14.Text = "IXL";
            // 
            // signedCheckBox
            // 
            this.signedCheckBox.AutoSize = true;
            this.signedCheckBox.Checked = true;
            this.signedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.signedCheckBox.Enabled = false;
            this.signedCheckBox.Location = new System.Drawing.Point(107, 3);
            this.signedCheckBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.signedCheckBox.Name = "signedCheckBox";
            this.signedCheckBox.Size = new System.Drawing.Size(62, 19);
            this.signedCheckBox.TabIndex = 0;
            this.signedCheckBox.Text = "Signed";
            this.signedCheckBox.UseVisualStyleBackColor = true;
            this.signedCheckBox.CheckedChanged += new System.EventHandler(this.SignedCheckBox_CheckedChanged);
            // 
            // hexadecimalCheckBox
            // 
            this.hexadecimalCheckBox.AutoSize = true;
            this.hexadecimalCheckBox.Checked = true;
            this.hexadecimalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hexadecimalCheckBox.Location = new System.Drawing.Point(5, 3);
            this.hexadecimalCheckBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.hexadecimalCheckBox.Name = "hexadecimalCheckBox";
            this.hexadecimalCheckBox.Size = new System.Drawing.Size(95, 19);
            this.hexadecimalCheckBox.TabIndex = 0;
            this.hexadecimalCheckBox.Text = "Hexadecimal";
            this.hexadecimalCheckBox.UseVisualStyleBackColor = true;
            this.hexadecimalCheckBox.CheckedChanged += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            // 
            // tabRegisters
            // 
            this.tabRegisters.Controls.Add(this.tabRegisterSet1);
            this.tabRegisters.Controls.Add(this.tabRegisterSet2);
            this.tabRegisters.Location = new System.Drawing.Point(5, 30);
            this.tabRegisters.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabRegisters.Name = "tabRegisters";
            this.tabRegisters.SelectedIndex = 0;
            this.tabRegisters.Size = new System.Drawing.Size(266, 229);
            this.tabRegisters.TabIndex = 1;
            // 
            // tabRegisterSet1
            // 
            this.tabRegisterSet1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabRegisterSet1.Controls.Add(this.checkBoxSet0FlagS);
            this.tabRegisterSet1.Controls.Add(this.checkBoxSet0FlagPV);
            this.tabRegisterSet1.Controls.Add(this.checkBoxSet0FlagN);
            this.tabRegisterSet1.Controls.Add(this.checkBoxSet0FlagHC);
            this.tabRegisterSet1.Controls.Add(this.checkBoxSet0FlagCY);
            this.tabRegisterSet1.Controls.Add(this.checkBoxSet0FlagZ);
            this.tabRegisterSet1.Controls.Add(this.label21);
            this.tabRegisterSet1.Controls.Add(this.label23);
            this.tabRegisterSet1.Controls.Add(this.label24);
            this.tabRegisterSet1.Controls.Add(this.label25);
            this.tabRegisterSet1.Controls.Add(this.label26);
            this.tabRegisterSet1.Controls.Add(this.label27);
            this.tabRegisterSet1.Controls.Add(this.label28);
            this.tabRegisterSet1.Controls.Add(this.label29);
            this.tabRegisterSet1.Controls.Add(this.label30);
            this.tabRegisterSet1.Controls.Add(this.label31);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegA);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegB);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegC);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegD);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegE);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegH);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegL);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegBC);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegDE);
            this.tabRegisterSet1.Controls.Add(this.textBoxSet0RegHL);
            this.tabRegisterSet1.Location = new System.Drawing.Point(4, 24);
            this.tabRegisterSet1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabRegisterSet1.Name = "tabRegisterSet1";
            this.tabRegisterSet1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabRegisterSet1.Size = new System.Drawing.Size(258, 201);
            this.tabRegisterSet1.TabIndex = 0;
            this.tabRegisterSet1.Text = "Register set 1";
            // 
            // checkBoxSet0FlagS
            // 
            this.checkBoxSet0FlagS.AutoSize = true;
            this.checkBoxSet0FlagS.Location = new System.Drawing.Point(120, 135);
            this.checkBoxSet0FlagS.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet0FlagS.Name = "checkBoxSet0FlagS";
            this.checkBoxSet0FlagS.Size = new System.Drawing.Size(66, 19);
            this.checkBoxSet0FlagS.TabIndex = 49;
            this.checkBoxSet0FlagS.Text = "S - Sign";
            this.checkBoxSet0FlagS.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagPV
            // 
            this.checkBoxSet0FlagPV.AutoSize = true;
            this.checkBoxSet0FlagPV.Location = new System.Drawing.Point(120, 155);
            this.checkBoxSet0FlagPV.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet0FlagPV.Name = "checkBoxSet0FlagPV";
            this.checkBoxSet0FlagPV.Size = new System.Drawing.Size(134, 19);
            this.checkBoxSet0FlagPV.TabIndex = 50;
            this.checkBoxSet0FlagPV.Text = "PV - Parity/Overflow";
            this.checkBoxSet0FlagPV.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagN
            // 
            this.checkBoxSet0FlagN.AutoSize = true;
            this.checkBoxSet0FlagN.Location = new System.Drawing.Point(120, 175);
            this.checkBoxSet0FlagN.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet0FlagN.Name = "checkBoxSet0FlagN";
            this.checkBoxSet0FlagN.Size = new System.Drawing.Size(93, 19);
            this.checkBoxSet0FlagN.TabIndex = 51;
            this.checkBoxSet0FlagN.Text = "N - Add/Sub";
            this.checkBoxSet0FlagN.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagHC
            // 
            this.checkBoxSet0FlagHC.AutoSize = true;
            this.checkBoxSet0FlagHC.Location = new System.Drawing.Point(10, 175);
            this.checkBoxSet0FlagHC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet0FlagHC.Name = "checkBoxSet0FlagHC";
            this.checkBoxSet0FlagHC.Size = new System.Drawing.Size(107, 19);
            this.checkBoxSet0FlagHC.TabIndex = 48;
            this.checkBoxSet0FlagHC.Text = "HC - Half Carry";
            this.checkBoxSet0FlagHC.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagCY
            // 
            this.checkBoxSet0FlagCY.AutoSize = true;
            this.checkBoxSet0FlagCY.Location = new System.Drawing.Point(10, 155);
            this.checkBoxSet0FlagCY.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet0FlagCY.Name = "checkBoxSet0FlagCY";
            this.checkBoxSet0FlagCY.Size = new System.Drawing.Size(80, 19);
            this.checkBoxSet0FlagCY.TabIndex = 47;
            this.checkBoxSet0FlagCY.Text = "CY - Carry";
            this.checkBoxSet0FlagCY.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet0FlagZ
            // 
            this.checkBoxSet0FlagZ.AutoSize = true;
            this.checkBoxSet0FlagZ.Location = new System.Drawing.Point(10, 135);
            this.checkBoxSet0FlagZ.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet0FlagZ.Name = "checkBoxSet0FlagZ";
            this.checkBoxSet0FlagZ.Size = new System.Drawing.Size(68, 19);
            this.checkBoxSet0FlagZ.TabIndex = 46;
            this.checkBoxSet0FlagZ.Text = "Z - Zero";
            this.checkBoxSet0FlagZ.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(4, 10);
            this.label21.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(14, 15);
            this.label21.TabIndex = 36;
            this.label21.Text = "B";
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(4, 40);
            this.label23.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(15, 15);
            this.label23.TabIndex = 37;
            this.label23.Text = "D";
            this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(84, 10);
            this.label24.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(15, 15);
            this.label24.TabIndex = 38;
            this.label24.Text = "C";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(4, 70);
            this.label25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(16, 15);
            this.label25.TabIndex = 39;
            this.label25.Text = "H";
            this.label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(164, 70);
            this.label26.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(22, 15);
            this.label26.TabIndex = 40;
            this.label26.Text = "HL";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(164, 40);
            this.label27.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(21, 15);
            this.label27.TabIndex = 41;
            this.label27.Text = "DE";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(84, 40);
            this.label28.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(13, 15);
            this.label28.TabIndex = 42;
            this.label28.Text = "E";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(164, 10);
            this.label29.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(22, 15);
            this.label29.TabIndex = 43;
            this.label29.Text = "BC";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(84, 70);
            this.label30.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(13, 15);
            this.label30.TabIndex = 44;
            this.label30.Text = "L";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(4, 100);
            this.label31.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(15, 15);
            this.label31.TabIndex = 45;
            this.label31.Text = "A";
            this.label31.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxSet0RegA
            // 
            this.textBoxSet0RegA.Location = new System.Drawing.Point(24, 96);
            this.textBoxSet0RegA.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegA.Name = "textBoxSet0RegA";
            this.textBoxSet0RegA.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegA.TabIndex = 11;
            this.textBoxSet0RegA.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegA.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegA.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegA.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegB
            // 
            this.textBoxSet0RegB.Location = new System.Drawing.Point(24, 6);
            this.textBoxSet0RegB.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegB.Name = "textBoxSet0RegB";
            this.textBoxSet0RegB.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegB.TabIndex = 2;
            this.textBoxSet0RegB.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegB.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegC
            // 
            this.textBoxSet0RegC.Location = new System.Drawing.Point(104, 6);
            this.textBoxSet0RegC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegC.Name = "textBoxSet0RegC";
            this.textBoxSet0RegC.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegC.TabIndex = 3;
            this.textBoxSet0RegC.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegC.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegD
            // 
            this.textBoxSet0RegD.Location = new System.Drawing.Point(24, 36);
            this.textBoxSet0RegD.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegD.Name = "textBoxSet0RegD";
            this.textBoxSet0RegD.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegD.TabIndex = 5;
            this.textBoxSet0RegD.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegD.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegD.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegE
            // 
            this.textBoxSet0RegE.Location = new System.Drawing.Point(104, 36);
            this.textBoxSet0RegE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegE.Name = "textBoxSet0RegE";
            this.textBoxSet0RegE.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegE.TabIndex = 6;
            this.textBoxSet0RegE.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegE.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegE.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegH
            // 
            this.textBoxSet0RegH.Location = new System.Drawing.Point(24, 66);
            this.textBoxSet0RegH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegH.Name = "textBoxSet0RegH";
            this.textBoxSet0RegH.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegH.TabIndex = 8;
            this.textBoxSet0RegH.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegH.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegH.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegL
            // 
            this.textBoxSet0RegL.Location = new System.Drawing.Point(104, 66);
            this.textBoxSet0RegL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegL.Name = "textBoxSet0RegL";
            this.textBoxSet0RegL.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet0RegL.TabIndex = 9;
            this.textBoxSet0RegL.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegL.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegBC
            // 
            this.textBoxSet0RegBC.Location = new System.Drawing.Point(188, 6);
            this.textBoxSet0RegBC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegBC.Name = "textBoxSet0RegBC";
            this.textBoxSet0RegBC.Size = new System.Drawing.Size(60, 23);
            this.textBoxSet0RegBC.TabIndex = 4;
            this.textBoxSet0RegBC.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegBC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegBC.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegBC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegDE
            // 
            this.textBoxSet0RegDE.Location = new System.Drawing.Point(188, 36);
            this.textBoxSet0RegDE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegDE.Name = "textBoxSet0RegDE";
            this.textBoxSet0RegDE.Size = new System.Drawing.Size(60, 23);
            this.textBoxSet0RegDE.TabIndex = 7;
            this.textBoxSet0RegDE.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegDE.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegDE.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegDE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet0RegHL
            // 
            this.textBoxSet0RegHL.Location = new System.Drawing.Point(188, 66);
            this.textBoxSet0RegHL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet0RegHL.Name = "textBoxSet0RegHL";
            this.textBoxSet0RegHL.Size = new System.Drawing.Size(60, 23);
            this.textBoxSet0RegHL.TabIndex = 10;
            this.textBoxSet0RegHL.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet0RegHL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet0RegHL.Leave += new System.EventHandler(this.RegisterLeave);
            this.textBoxSet0RegHL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // tabRegisterSet2
            // 
            this.tabRegisterSet2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabRegisterSet2.Controls.Add(this.checkBoxSet1FlagS);
            this.tabRegisterSet2.Controls.Add(this.checkBoxSet1FlagPV);
            this.tabRegisterSet2.Controls.Add(this.checkBoxSet1FlagN);
            this.tabRegisterSet2.Controls.Add(this.checkBoxSet1FlagHC);
            this.tabRegisterSet2.Controls.Add(this.checkBoxSet1FlagCY);
            this.tabRegisterSet2.Controls.Add(this.checkBoxSet1FlagZ);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegA);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegB);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegC);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegD);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegE);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegH);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegL);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegBC);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegDE);
            this.tabRegisterSet2.Controls.Add(this.textBoxSet1RegHL);
            this.tabRegisterSet2.Controls.Add(this.label1);
            this.tabRegisterSet2.Controls.Add(this.label4);
            this.tabRegisterSet2.Controls.Add(this.label2);
            this.tabRegisterSet2.Controls.Add(this.label7);
            this.tabRegisterSet2.Controls.Add(this.label9);
            this.tabRegisterSet2.Controls.Add(this.label6);
            this.tabRegisterSet2.Controls.Add(this.label5);
            this.tabRegisterSet2.Controls.Add(this.label3);
            this.tabRegisterSet2.Controls.Add(this.label8);
            this.tabRegisterSet2.Controls.Add(this.label10);
            this.tabRegisterSet2.Location = new System.Drawing.Point(4, 24);
            this.tabRegisterSet2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabRegisterSet2.Name = "tabRegisterSet2";
            this.tabRegisterSet2.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabRegisterSet2.Size = new System.Drawing.Size(258, 201);
            this.tabRegisterSet2.TabIndex = 1;
            this.tabRegisterSet2.Text = "Register set 2";
            // 
            // checkBoxSet1FlagS
            // 
            this.checkBoxSet1FlagS.AutoSize = true;
            this.checkBoxSet1FlagS.Location = new System.Drawing.Point(120, 135);
            this.checkBoxSet1FlagS.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet1FlagS.Name = "checkBoxSet1FlagS";
            this.checkBoxSet1FlagS.Size = new System.Drawing.Size(66, 19);
            this.checkBoxSet1FlagS.TabIndex = 55;
            this.checkBoxSet1FlagS.Text = "S - Sign";
            this.checkBoxSet1FlagS.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagPV
            // 
            this.checkBoxSet1FlagPV.AutoSize = true;
            this.checkBoxSet1FlagPV.Location = new System.Drawing.Point(120, 155);
            this.checkBoxSet1FlagPV.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet1FlagPV.Name = "checkBoxSet1FlagPV";
            this.checkBoxSet1FlagPV.Size = new System.Drawing.Size(134, 19);
            this.checkBoxSet1FlagPV.TabIndex = 56;
            this.checkBoxSet1FlagPV.Text = "PV - Parity/Overflow";
            this.checkBoxSet1FlagPV.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagN
            // 
            this.checkBoxSet1FlagN.AutoSize = true;
            this.checkBoxSet1FlagN.Location = new System.Drawing.Point(120, 175);
            this.checkBoxSet1FlagN.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet1FlagN.Name = "checkBoxSet1FlagN";
            this.checkBoxSet1FlagN.Size = new System.Drawing.Size(93, 19);
            this.checkBoxSet1FlagN.TabIndex = 57;
            this.checkBoxSet1FlagN.Text = "N - Add/Sub";
            this.checkBoxSet1FlagN.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagHC
            // 
            this.checkBoxSet1FlagHC.AutoSize = true;
            this.checkBoxSet1FlagHC.Location = new System.Drawing.Point(10, 175);
            this.checkBoxSet1FlagHC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet1FlagHC.Name = "checkBoxSet1FlagHC";
            this.checkBoxSet1FlagHC.Size = new System.Drawing.Size(107, 19);
            this.checkBoxSet1FlagHC.TabIndex = 54;
            this.checkBoxSet1FlagHC.Text = "HC - Half Carry";
            this.checkBoxSet1FlagHC.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagCY
            // 
            this.checkBoxSet1FlagCY.AutoSize = true;
            this.checkBoxSet1FlagCY.Location = new System.Drawing.Point(10, 155);
            this.checkBoxSet1FlagCY.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet1FlagCY.Name = "checkBoxSet1FlagCY";
            this.checkBoxSet1FlagCY.Size = new System.Drawing.Size(80, 19);
            this.checkBoxSet1FlagCY.TabIndex = 53;
            this.checkBoxSet1FlagCY.Text = "CY - Carry";
            this.checkBoxSet1FlagCY.UseVisualStyleBackColor = true;
            // 
            // checkBoxSet1FlagZ
            // 
            this.checkBoxSet1FlagZ.AutoSize = true;
            this.checkBoxSet1FlagZ.Location = new System.Drawing.Point(9, 135);
            this.checkBoxSet1FlagZ.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxSet1FlagZ.Name = "checkBoxSet1FlagZ";
            this.checkBoxSet1FlagZ.Size = new System.Drawing.Size(68, 19);
            this.checkBoxSet1FlagZ.TabIndex = 52;
            this.checkBoxSet1FlagZ.Text = "Z - Zero";
            this.checkBoxSet1FlagZ.UseVisualStyleBackColor = true;
            // 
            // textBoxSet1RegA
            // 
            this.textBoxSet1RegA.Location = new System.Drawing.Point(24, 96);
            this.textBoxSet1RegA.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegA.Name = "textBoxSet1RegA";
            this.textBoxSet1RegA.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegA.TabIndex = 11;
            this.textBoxSet1RegA.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegA.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegA.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegA.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegB
            // 
            this.textBoxSet1RegB.Location = new System.Drawing.Point(24, 6);
            this.textBoxSet1RegB.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegB.Name = "textBoxSet1RegB";
            this.textBoxSet1RegB.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegB.TabIndex = 2;
            this.textBoxSet1RegB.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegB.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegC
            // 
            this.textBoxSet1RegC.Location = new System.Drawing.Point(104, 6);
            this.textBoxSet1RegC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegC.Name = "textBoxSet1RegC";
            this.textBoxSet1RegC.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegC.TabIndex = 3;
            this.textBoxSet1RegC.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegC.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegD
            // 
            this.textBoxSet1RegD.Location = new System.Drawing.Point(24, 36);
            this.textBoxSet1RegD.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegD.Name = "textBoxSet1RegD";
            this.textBoxSet1RegD.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegD.TabIndex = 5;
            this.textBoxSet1RegD.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegD.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegD.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegE
            // 
            this.textBoxSet1RegE.Location = new System.Drawing.Point(104, 36);
            this.textBoxSet1RegE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegE.Name = "textBoxSet1RegE";
            this.textBoxSet1RegE.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegE.TabIndex = 6;
            this.textBoxSet1RegE.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegE.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegE.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegH
            // 
            this.textBoxSet1RegH.Location = new System.Drawing.Point(24, 66);
            this.textBoxSet1RegH.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegH.Name = "textBoxSet1RegH";
            this.textBoxSet1RegH.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegH.TabIndex = 8;
            this.textBoxSet1RegH.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegH.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegH.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegL
            // 
            this.textBoxSet1RegL.Location = new System.Drawing.Point(104, 66);
            this.textBoxSet1RegL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegL.Name = "textBoxSet1RegL";
            this.textBoxSet1RegL.Size = new System.Drawing.Size(50, 23);
            this.textBoxSet1RegL.TabIndex = 9;
            this.textBoxSet1RegL.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegL.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegBC
            // 
            this.textBoxSet1RegBC.Location = new System.Drawing.Point(188, 6);
            this.textBoxSet1RegBC.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegBC.Name = "textBoxSet1RegBC";
            this.textBoxSet1RegBC.Size = new System.Drawing.Size(68, 23);
            this.textBoxSet1RegBC.TabIndex = 4;
            this.textBoxSet1RegBC.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegBC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegBC.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegBC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegDE
            // 
            this.textBoxSet1RegDE.Location = new System.Drawing.Point(188, 36);
            this.textBoxSet1RegDE.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegDE.Name = "textBoxSet1RegDE";
            this.textBoxSet1RegDE.Size = new System.Drawing.Size(68, 23);
            this.textBoxSet1RegDE.TabIndex = 7;
            this.textBoxSet1RegDE.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegDE.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegDE.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegDE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // textBoxSet1RegHL
            // 
            this.textBoxSet1RegHL.Location = new System.Drawing.Point(188, 66);
            this.textBoxSet1RegHL.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxSet1RegHL.Name = "textBoxSet1RegHL";
            this.textBoxSet1RegHL.Size = new System.Drawing.Size(68, 23);
            this.textBoxSet1RegHL.TabIndex = 10;
            this.textBoxSet1RegHL.Enter += new System.EventHandler(this.RegisterEnter);
            this.textBoxSet1RegHL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegisterKeyPress);
            this.textBoxSet1RegHL.Leave += new System.EventHandler(this.HexadecimalCheckBox_CheckedChanged);
            this.textBoxSet1RegHL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RegisterMouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "B";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 40);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "D";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "C";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 70);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(16, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "H";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(164, 70);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(22, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "HL";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(164, 40);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "DE";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(84, 40);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "E";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "BC";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(84, 70);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "L";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 100);
            this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 15);
            this.label10.TabIndex = 35;
            this.label10.Text = "A";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBoxDisassembly
            // 
            this.groupBoxDisassembly.Controls.Add(this.disassemblyListBox);
            this.groupBoxDisassembly.Controls.Add(this.disassemblyAddressScroller);
            this.groupBoxDisassembly.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDisassembly.Location = new System.Drawing.Point(0, 80);
            this.groupBoxDisassembly.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxDisassembly.Name = "groupBoxDisassembly";
            this.groupBoxDisassembly.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxDisassembly.Size = new System.Drawing.Size(569, 404);
            this.groupBoxDisassembly.TabIndex = 3;
            this.groupBoxDisassembly.TabStop = false;
            this.groupBoxDisassembly.Text = "Disassembly";
            // 
            // disassemblyListBox
            // 
            this.disassemblyListBox.ContextMenuStrip = this.contextMenuAssembly;
            this.disassemblyListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disassemblyListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.disassemblyListBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.disassemblyListBox.FormattingEnabled = true;
            this.disassemblyListBox.ItemHeight = 14;
            this.disassemblyListBox.Location = new System.Drawing.Point(4, 19);
            this.disassemblyListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.disassemblyListBox.Name = "disassemblyListBox";
            this.disassemblyListBox.SelectedAddress = null;
            this.disassemblyListBox.Size = new System.Drawing.Size(540, 382);
            this.disassemblyListBox.TabIndex = 0;
            this.disassemblyListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DisassemblyListBox_DrawItem);
            this.disassemblyListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.DisassemblyListBox_MeasureItem);
            this.disassemblyListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DisassemblyListBox_MouseDown);
            this.disassemblyListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DisassemblyListBox_MouseMove);
            this.disassemblyListBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DisassemblyListBox_MouseUp);
            // 
            // disassemblyAddressScroller
            // 
            this.disassemblyAddressScroller.AccessibleDescription = "";
            this.disassemblyAddressScroller.Dock = System.Windows.Forms.DockStyle.Right;
            this.disassemblyAddressScroller.LargeChange = 32;
            this.disassemblyAddressScroller.Location = new System.Drawing.Point(544, 19);
            this.disassemblyAddressScroller.Maximum = 65535;
            this.disassemblyAddressScroller.Name = "disassemblyAddressScroller";
            this.disassemblyAddressScroller.Size = new System.Drawing.Size(21, 382);
            this.disassemblyAddressScroller.TabIndex = 4;
            this.disassemblyAddressScroller.Scroll += new System.Windows.Forms.ScrollEventHandler(this.DisassemblyAddressScroller_Scroll);
            // 
            // groupBoxMemory
            // 
            this.groupBoxMemory.Controls.Add(this.memoryListBox);
            this.groupBoxMemory.Controls.Add(this.memoryAddressScroller);
            this.groupBoxMemory.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxMemory.Location = new System.Drawing.Point(0, 533);
            this.groupBoxMemory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxMemory.Name = "groupBoxMemory";
            this.groupBoxMemory.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxMemory.Size = new System.Drawing.Size(1002, 172);
            this.groupBoxMemory.TabIndex = 5;
            this.groupBoxMemory.TabStop = false;
            this.groupBoxMemory.Text = "Memory";
            // 
            // memoryListBox
            // 
            this.memoryListBox.ContextMenuStrip = this.contextMenuMemory;
            this.memoryListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.memoryListBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.memoryListBox.FormattingEnabled = true;
            this.memoryListBox.Location = new System.Drawing.Point(4, 19);
            this.memoryListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.memoryListBox.Name = "memoryListBox";
            this.memoryListBox.SelectedAddress = null;
            this.memoryListBox.Size = new System.Drawing.Size(973, 150);
            this.memoryListBox.TabIndex = 0;
            this.memoryListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.MemoryListBox_DrawItem);
            // 
            // contextMenuMemory
            // 
            this.contextMenuMemory.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuMemory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoMemoryAddressItem});
            this.contextMenuMemory.Name = "contextMenuAssembly";
            this.contextMenuMemory.Size = new System.Drawing.Size(197, 26);
            // 
            // gotoMemoryAddressItem
            // 
            this.gotoMemoryAddressItem.Name = "gotoMemoryAddressItem";
            this.gotoMemoryAddressItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoMemoryAddressItem.Size = new System.Drawing.Size(196, 22);
            this.gotoMemoryAddressItem.Text = "Goto Address...";
            this.gotoMemoryAddressItem.Click += new System.EventHandler(this.GotoMemoryAddressItem_Click);
            // 
            // memoryAddressScroller
            // 
            this.memoryAddressScroller.AccessibleDescription = "";
            this.memoryAddressScroller.Dock = System.Windows.Forms.DockStyle.Right;
            this.memoryAddressScroller.LargeChange = 64;
            this.memoryAddressScroller.Location = new System.Drawing.Point(977, 19);
            this.memoryAddressScroller.Maximum = 65535;
            this.memoryAddressScroller.Name = "memoryAddressScroller";
            this.memoryAddressScroller.Size = new System.Drawing.Size(21, 150);
            this.memoryAddressScroller.SmallChange = 16;
            this.memoryAddressScroller.TabIndex = 6;
            this.memoryAddressScroller.Scroll += new System.Windows.Forms.ScrollEventHandler(this.MemoryAddressScroller_Scroll);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1002, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRomToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openRomToolStripMenuItem
            // 
            this.openRomToolStripMenuItem.Name = "openRomToolStripMenuItem";
            this.openRomToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.openRomToolStripMenuItem.Text = "&Open Rom...";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemRun,
            this.menuItemRunToCursor,
            this.menuItemStepOver,
            this.menuItemStepInto,
            this.toolStripMenuItem2,
            this.resetToolStripMenuItem,
            this.restartToolStripMenuItem,
            this.toolStripMenuItem3,
            this.menuItemToggleBreakpoint,
            this.menuItemBreakpointDialog});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // menuItemRun
            // 
            this.menuItemRun.Image = ((System.Drawing.Image)(resources.GetObject("menuItemRun.Image")));
            this.menuItemRun.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuItemRun.Name = "menuItemRun";
            this.menuItemRun.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.menuItemRun.Size = new System.Drawing.Size(213, 24);
            this.menuItemRun.Text = "&Run";
            this.menuItemRun.Click += new System.EventHandler(this.RunClick);
            // 
            // menuItemRunToCursor
            // 
            this.menuItemRunToCursor.Image = ((System.Drawing.Image)(resources.GetObject("menuItemRunToCursor.Image")));
            this.menuItemRunToCursor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuItemRunToCursor.Name = "menuItemRunToCursor";
            this.menuItemRunToCursor.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.menuItemRunToCursor.Size = new System.Drawing.Size(213, 24);
            this.menuItemRunToCursor.Text = "&Run to cursor";
            this.menuItemRunToCursor.Click += new System.EventHandler(this.RunToCursorClick);
            // 
            // menuItemStepOver
            // 
            this.menuItemStepOver.Image = ((System.Drawing.Image)(resources.GetObject("menuItemStepOver.Image")));
            this.menuItemStepOver.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuItemStepOver.Name = "menuItemStepOver";
            this.menuItemStepOver.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.menuItemStepOver.Size = new System.Drawing.Size(213, 24);
            this.menuItemStepOver.Text = "Step &over";
            this.menuItemStepOver.Click += new System.EventHandler(this.StepOverClick);
            // 
            // menuItemStepInto
            // 
            this.menuItemStepInto.Image = ((System.Drawing.Image)(resources.GetObject("menuItemStepInto.Image")));
            this.menuItemStepInto.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuItemStepInto.Name = "menuItemStepInto";
            this.menuItemStepInto.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.menuItemStepInto.Size = new System.Drawing.Size(213, 24);
            this.menuItemStepInto.Text = "&Step into";
            this.menuItemStepInto.Click += new System.EventHandler(this.StepIntoClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(210, 6);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Enabled = false;
            this.resetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("resetToolStripMenuItem.Image")));
            this.resetToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(213, 24);
            this.resetToolStripMenuItem.Text = "Stop Debugging";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.StopClick);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Enabled = false;
            this.restartToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restartToolStripMenuItem.Image")));
            this.restartToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F5)));
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(213, 24);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.ResetClick);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(210, 6);
            // 
            // menuItemToggleBreakpoint
            // 
            this.menuItemToggleBreakpoint.Image = ((System.Drawing.Image)(resources.GetObject("menuItemToggleBreakpoint.Image")));
            this.menuItemToggleBreakpoint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuItemToggleBreakpoint.Name = "menuItemToggleBreakpoint";
            this.menuItemToggleBreakpoint.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.menuItemToggleBreakpoint.Size = new System.Drawing.Size(213, 24);
            this.menuItemToggleBreakpoint.Text = "&Toggle breakpoint";
            this.menuItemToggleBreakpoint.Click += new System.EventHandler(this.ToggleBreakPointClick);
            // 
            // menuItemBreakpointDialog
            // 
            this.menuItemBreakpointDialog.Name = "menuItemBreakpointDialog";
            this.menuItemBreakpointDialog.Size = new System.Drawing.Size(213, 24);
            this.menuItemBreakpointDialog.Text = "&Breakpoints...";
            this.menuItemBreakpointDialog.Click += new System.EventHandler(this.BreakpointDialogClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runButton,
            this.pauseButton,
            this.stopButton,
            this.resetButton,
            this.toolStripSeparator1,
            this.runToCursorButton,
            this.stepIntoButton,
            this.stepOverButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1002, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // runButton
            // 
            this.runButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runButton.Image = ((System.Drawing.Image)(resources.GetObject("runButton.Image")));
            this.runButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.runButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(23, 22);
            this.runButton.Text = "Run";
            this.runButton.ToolTipText = "Run";
            this.runButton.Click += new System.EventHandler(this.RunClick);
            // 
            // pauseButton
            // 
            this.pauseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseButton.Enabled = false;
            this.pauseButton.Image = ((System.Drawing.Image)(resources.GetObject("pauseButton.Image")));
            this.pauseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.pauseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(23, 22);
            this.pauseButton.Text = "toolStripButton2";
            this.pauseButton.Click += new System.EventHandler(this.PauseClick);
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "toolStripButton3";
            this.stopButton.Click += new System.EventHandler(this.StopClick);
            // 
            // resetButton
            // 
            this.resetButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resetButton.Image = ((System.Drawing.Image)(resources.GetObject("resetButton.Image")));
            this.resetButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.resetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(23, 22);
            this.resetButton.Text = "Reset";
            this.resetButton.Click += new System.EventHandler(this.ResetClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // runToCursorButton
            // 
            this.runToCursorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runToCursorButton.Image = ((System.Drawing.Image)(resources.GetObject("runToCursorButton.Image")));
            this.runToCursorButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.runToCursorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runToCursorButton.Name = "runToCursorButton";
            this.runToCursorButton.Size = new System.Drawing.Size(23, 22);
            this.runToCursorButton.Text = "Run to cursor";
            this.runToCursorButton.Click += new System.EventHandler(this.RunToCursorClick);
            // 
            // stepIntoButton
            // 
            this.stepIntoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stepIntoButton.Image = ((System.Drawing.Image)(resources.GetObject("stepIntoButton.Image")));
            this.stepIntoButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stepIntoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepIntoButton.Name = "stepIntoButton";
            this.stepIntoButton.Size = new System.Drawing.Size(23, 22);
            this.stepIntoButton.Text = "Step into";
            this.stepIntoButton.Click += new System.EventHandler(this.StepIntoClick);
            // 
            // stepOverButton
            // 
            this.stepOverButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stepOverButton.Image = ((System.Drawing.Image)(resources.GetObject("stepOverButton.Image")));
            this.stepOverButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stepOverButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepOverButton.Name = "stepOverButton";
            this.stepOverButton.Size = new System.Drawing.Size(23, 22);
            this.stepOverButton.Text = "Step over";
            this.stepOverButton.Click += new System.EventHandler(this.StepOverClick);
            // 
            // groupBoxMemorySelect
            // 
            this.groupBoxMemorySelect.Controls.Add(this.memoryCheckedBox);
            this.groupBoxMemorySelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxMemorySelect.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMemorySelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxMemorySelect.Name = "groupBoxMemorySelect";
            this.groupBoxMemorySelect.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBoxMemorySelect.Size = new System.Drawing.Size(693, 80);
            this.groupBoxMemorySelect.TabIndex = 3;
            this.groupBoxMemorySelect.TabStop = false;
            this.groupBoxMemorySelect.Text = "Memory selector";
            // 
            // memoryCheckedBox
            // 
            this.memoryCheckedBox.ColumnWidth = 200;
            this.memoryCheckedBox.FormattingEnabled = true;
            this.memoryCheckedBox.Items.AddRange(new object[] {
            "RAM:  0x0000 - 0xFFFF",
            "LROM: 0x0000 - 0x3FFF",
            "UROM: 0xC000 - 0xFFFF"});
            this.memoryCheckedBox.Location = new System.Drawing.Point(7, 22);
            this.memoryCheckedBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.memoryCheckedBox.MultiColumn = true;
            this.memoryCheckedBox.Name = "memoryCheckedBox";
            this.memoryCheckedBox.Size = new System.Drawing.Size(661, 40);
            this.memoryCheckedBox.TabIndex = 0;
            this.memoryCheckedBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.MemoryCheckBox_ItemCheck);
            // 
            // checkBoxUpdateScreen
            // 
            this.checkBoxUpdateScreen.AutoSize = true;
            this.checkBoxUpdateScreen.Location = new System.Drawing.Point(312, 37);
            this.checkBoxUpdateScreen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBoxUpdateScreen.Name = "checkBoxUpdateScreen";
            this.checkBoxUpdateScreen.Size = new System.Drawing.Size(102, 19);
            this.checkBoxUpdateScreen.TabIndex = 8;
            this.checkBoxUpdateScreen.Text = "Update Screen";
            this.checkBoxUpdateScreen.UseVisualStyleBackColor = true;
            this.checkBoxUpdateScreen.CheckedChanged += new System.EventHandler(this.CheckBoxUpdateScreen_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.groupBoxDisassembly);
            this.MainPanel.Controls.Add(this.groupBoxStack);
            this.MainPanel.Controls.Add(this.groupBoxMemorySelect);
            this.MainPanel.Controls.Add(this.groupBoxRegisters);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 49);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1002, 484);
            this.MainPanel.TabIndex = 9;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 530);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1002, 3);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 705);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.checkBoxUpdateScreen);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.groupBoxMemory);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "Z80 Emulator";
            this.groupBoxStack.ResumeLayout(false);
            this.contextMenuAssembly.ResumeLayout(false);
            this.groupBoxRegisters.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabRegisters.ResumeLayout(false);
            this.tabRegisterSet1.ResumeLayout(false);
            this.tabRegisterSet1.PerformLayout();
            this.tabRegisterSet2.ResumeLayout(false);
            this.tabRegisterSet2.PerformLayout();
            this.groupBoxDisassembly.ResumeLayout(false);
            this.groupBoxMemory.ResumeLayout(false);
            this.contextMenuMemory.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBoxMemorySelect.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

