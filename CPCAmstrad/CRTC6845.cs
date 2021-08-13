using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CPCAmstrad
{
    public class CRTC6845
    {
        private int[] registers = new int[18];
        private double cpuClockFrequency;   // [MHz]
        private long cpuStatesPerSecond;

        private long cpuStatesHSyncCycle;
        private long cpuStatesHSyncStart;
        private long cpuStatesHSyncStop;

        private long cpuStatesVSyncCycle;
        private long cpuStatesVSyncStart;
        private long cpuStatesVSyncStop;

        private int totalwidth, totalheight;
        private int width, height;
        private int rasterLines;

        public CRTC6845()
        {
            registers[0] = 63;
            registers[1] = 40;
            registers[2] = 46;
            registers[3] = 142;
            registers[4] = 38;
            registers[5] = 0;
            registers[6] = 25;
            registers[7] = 30;
            registers[8] = 0;
            registers[9] = 7;
            registers[10] = 0;
            registers[11] = 0;
            registers[12] = 48;
            registers[13] = 0;
            registers[14] = 192;
            registers[15] = 0;
            registers[16] = 0;
            registers[17] = 0;
        }

        public int RegisterSelect { get; set; }
        public int RegisterValue
        {
            get
            {
                if (RegisterSelect >= 0 && RegisterSelect < 18)
                    return registers[RegisterSelect];
                return 0xFF;
            }
            set
            {
                if (RegisterSelect >= 0 && RegisterSelect < 18)
                    registers[RegisterSelect] = value;
                HandleSetting();
            }
        }

        public void CalcSyncs()
        {
            long syncWidthH = HorizontalAndVerticalSyncWidths & 0xF;
            if (syncWidthH == 0) syncWidthH = 16;
            long syncWidthV = HorizontalAndVerticalSyncWidths >> 4;
            if (syncWidthV == 0) syncWidthV = 16;

            totalwidth = (HorizontalTotal + 1) * 16;
            width = (HorizontalDisplayed) * 16;
            rasterLines = (MaximumRasterAddress & 0x07) + 1;

            cpuStatesHSyncCycle = (HorizontalTotal + 1) * cpuStatesPerSecond / (long)1E6;
            cpuStatesHSyncStart = HorizontalSyncPosition * cpuStatesPerSecond / (long)1E6;
            cpuStatesHSyncStop = (syncWidthH * cpuStatesPerSecond / (long)1E6 + cpuStatesHSyncStart) % cpuStatesHSyncCycle;

            totalheight = ((VerticalTotal & 0x7F) + 1) * rasterLines + (VerticalTotalAdjust & 0x1F);
            height = (VerticalDisplayed & 0x7F) * rasterLines;
            cpuStatesVSyncCycle = totalheight * cpuStatesHSyncCycle;
            cpuStatesVSyncStart = (VerticalSyncPosition & 0x7F) * ((MaximumRasterAddress & 0x07) + 1) * cpuStatesHSyncCycle;
            cpuStatesVSyncStop = (cpuStatesVSyncStart + syncWidthV * cpuStatesHSyncCycle) % cpuStatesVSyncCycle;
        }

        public double CpuClockFrequency
        {
            get { return cpuClockFrequency; }
            set
            {
                cpuClockFrequency = value;
                cpuStatesPerSecond = (long)(value * 1E6);
                CalcSyncs();
            }
        }

        public bool GetHSync(long stateCounter)
        {
            long count = stateCounter % cpuStatesHSyncCycle;
            return cpuStatesHSyncStop > cpuStatesHSyncStart ?
                count >= cpuStatesHSyncStart && count < cpuStatesHSyncStop :
                count >= cpuStatesHSyncStart || count < cpuStatesHSyncStop;
        }

        public bool GetVSync(long stateCounter)
        {
            long count = stateCounter % cpuStatesVSyncCycle;
            return cpuStatesVSyncStop > cpuStatesVSyncStart ?
                count >= cpuStatesVSyncStart && count < cpuStatesVSyncStop :
                count >= cpuStatesVSyncStart || count < cpuStatesVSyncStop;
        }

        // Returns -1 when DISPTMG is 1.
        public int RamAddr(long stateCounter)
        {
            int x = (int)((stateCounter % cpuStatesHSyncCycle) * (long)1E6 / cpuStatesPerSecond);
            int y = (int)((stateCounter % cpuStatesVSyncCycle) / cpuStatesHSyncCycle);
            if (x > HorizontalDisplayed) return -1;
            if (y > height) return -1;
            return x + y * HorizontalDisplayed;
        }

        // Width of the screen, in characters. Should always be 63 (64 characters). 1 character == 1μs.
        public int HorizontalTotal
        {
            get { return registers[0]; }
            set { registers[0] = value; }
        }

        // Number of characters displayed. Once horizontal character count (HCC) matches this value, DISPTMG is set to 1.
        public int HorizontalDisplayed
        {
            get { return registers[1]; }
            set { registers[1] = value; }
        }

        // When to start the HSync signal.
        public int HorizontalSyncPosition
        {
            get { return registers[2]; }
            set { registers[2] = value; }
        }

        // HSync pulse width in characters (0 means 16 on some CRTC), should always be more than 8; VSync width in scan-lines. (0 means 16 on some CRTC. Not present on all CRTCs, fixed to 16 lines on these)
        public int HorizontalAndVerticalSyncWidths
        {
            get { return registers[3]; }
            set { registers[3] = value; }
        }

        // Height of the screen, in characters.
        public int VerticalTotal
        {
            get { return registers[4]; }
            set { registers[4] = value; }
        }

        // Measured in scanlines, can be used for smooth vertical scrolling on CPC.
        public int VerticalTotalAdjust
        {
            get { return registers[5]; }
            set { registers[5] = value; }
        }

        // Height of displayed screen in characters. Once vertical character count (VCC) matches this value, DISPTMG is set to 1.
        public int VerticalDisplayed
        {
            get { return registers[6]; }
            set { registers[6] = value; }
        }

        // When to start the VSync signal, in characters.
        public int VerticalSyncPosition
        {
            get { return registers[7]; }
            set { registers[7] = value; }
        }

        // 00: No interlace; 01: Interlace Sync Raster Scan Mode; 10: No Interlace; 11: Interlace Sync and Video Raster Scan Mode
        public int InterlaceAndSkew
        {
            get { return registers[8]; }
            set { registers[8] = value; }
        }

        // Maximum scan line address on CPC can hold between 0 and 7, higher values' upper bits are ignored
        public int MaximumRasterAddress
        {
            get { return registers[9]; }
            set { registers[9] = value; }
        }

        // Cursor not used on CPC. B = Blink On/Off; P = Blink Period Control (Slow/Fast). Sets first raster row of character that cursor is on to invert.
        public int CursorStartRaster
        {
            get { return registers[10]; }
            set { registers[10] = value; }
        }

        // Sets last raster row of character that cursor is on to invert
        public int CursorEndRaster
        {
            get { return registers[11]; }
            set { registers[11] = value; }
        }

        // Allows you to offset the start of screen memory, useful when using memory from address &0000.
        public int DisplayStartAddressHigh
        {
            get { return registers[12]; }
            set { registers[12] = value; }
        }

        // Allows you to offset the start of screen memory, useful when using memory from address &0000.
        public int DisplayStartAddressLow
        {
            get { return registers[13]; }
            set { registers[13] = value; }
        }

        // Cursor not supported on CPC
        public int CursorAddressHigh
        {
            get { return registers[14]; }
            set { registers[14] = value; }
        }

        // Cursor not supported on CPC
        public int CursorAddressLow
        {
            get { return registers[15]; }
            set { registers[15] = value; }
        }

        public int LightPenAddressHigh
        {
            get { return registers[16]; }
            set { registers[16] = value; }
        }

        public int LightPenAddressLow
        {
            get { return registers[17]; }
            set { registers[17] = value; }
        }

        public int VideoPage
        {
            get { return videoPage; }
        }

        public int VideoSize
        {
            get { return videoSize; }
        }

        public int VideoOffset
        {
            get { return videoOffset; }
        }

        protected int videoPage;
        protected int videoSize;
        protected int videoOffset;

        private void HandleSetting()
        {
            videoPage = (DisplayStartAddressHigh & 0x30) << 10;
            videoSize = (DisplayStartAddressHigh & 0x0C) == 0x0C ? 0x8000 : 0x4000;
            videoOffset = ((DisplayStartAddressHigh << 8) | (DisplayStartAddressLow)) & 0x0300;
            CalcSyncs();
        }
    }
}
