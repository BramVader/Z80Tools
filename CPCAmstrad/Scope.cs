using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CPCAmstrad
{
    public partial class Scope : Form
    {
        private const int maxMemorySize = 1024 * 1024;

        public enum ChannelType
        {
            Bit,
            Int
        }

        public class Channel
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public Color Color { get; set; }
            public ChannelType ChannelType { get; set; }
        }

        public class BitChannel : Channel
        {
        }

        public class IntChannel : Channel
        {
            public long Min { get; set; }
            public long Max { get; set; }
        }

        public class Record
        {
            public long TimeStamp { get; set; }
            public object[] Data { get; set; }
        }

        protected List<Channel> channels;
        protected List<Record> memory;
        protected bool started;
        protected int memoryPointer;
        protected int xOffset;

        protected TimeSpan displayOffset;
        protected double displayZoom = 1.0;  // 1 µs per pixel

        public Scope()
        {
            channels = new List<Channel>();
            InitializeComponent();
            displayZoom = 1;
        }

        public BitChannel AddBitChannel(string name)
        {
            var channel = new BitChannel() { Name = name, ChannelType = ChannelType.Bit, Color = Color.Blue };
            channel.Index = channels.Count;
            channels.Add(channel);
            return channel;
        }

        public IntChannel AddIntChannel(string name, long min, long max)
        {
            var channel = new IntChannel() { Name = name, Min = min, Max = max, ChannelType = ChannelType.Int, Color = Color.Blue };
            channel.Index = channels.Count;
            channels.Add(channel);
            return channel;
        }

        public void Start()
        {
            if (started)
            {
                Stop();
            }
            memory = new List<Record>();
            started = true;
        }

        public void Stop()
        {
            if (started)
            {
                started = false;
            }
        }

        public void RecordData(long timestamp, object[] data)
        {
            if (started)
            {
                if (memory.Count < maxMemorySize)
                {
                    memory.Add(new Record() { TimeStamp = timestamp, Data = data });
                    memoryPointer = 0;
                }
                else
                {
                    memory[memoryPointer] = new Record() { TimeStamp = timestamp, Data = data };
                    memoryPointer = (memoryPointer + 1) % maxMemorySize;
                }
            }
        }

        public void Draw(Graphics g, RectangleF rect)
        {
            if (memory == null)
                return;

            Font font = this.Font;
            float textwidth = 0;
            float yOffset = 0f;



            int m1 = xOffset;
            int m2 = xOffset + (int)(rect.Width * displayZoom);
            for (int n = 0; n < channels.Count; n++)
            {
                Channel ch = channels[n];
                SizeF size = g.MeasureString(ch.Name, font);
                textwidth = Math.Max(textwidth, size.Width);
                g.DrawString(ch.Name, font, Brushes.Black, 0f, yOffset);
                float xoffset = textwidth + 8f;
                float dispWidth = rect.Width - xOffset;

                switch (ch.ChannelType)
                {
                    case ChannelType.Bit:
                        using (Pen pen = new Pen(ch.Color))
                        {
                            bool v1 = false;
                            float x1 = 0f;
                            for (int m = m1; m < m2; m++)
                            {
                                int m3 = (memoryPointer + m) % memory.Count;
                                float x2 = xoffset + (int)((m - m1) * displayZoom);
                                bool v2 = (bool)(memory[m3].Data[n]);
                                if (m > m1)
                                {
                                    if (v1 != v2)
                                    {
                                        g.DrawLine(pen, x2, yOffset, x2, yOffset + 16f);
                                    }
                                    g.DrawLine(pen, x1, yOffset + (v1 ? 0f : 16f), x2, yOffset + (v1 ? 0f : 16f));
                                }
                                v1 = v2;
                                x1 = x2;
                            }
                        }
                        yOffset += 20f; 
                        break;
                    case ChannelType.Int:
                        var intCh = (IntChannel)ch;
                        using (Pen pen = new Pen(ch.Color))
                        {
                            float v1 = 0f;
                            float x1 = 0f;
                            for (int m = m1; m < m2; m++)
                            {
                                int m3 = (memoryPointer + m) % memory.Count;
                                float x2 = xOffset + (int)((m - m1) * displayZoom);
                                float v2 = ((long)(memory[m3].Data[n]) - intCh.Min) * 64L / (intCh.Max - intCh.Min);
                                if (m > m1)
                                {
                                    g.DrawLine(pen, x1, yOffset + v1, x2, yOffset + v2);
                                }
                                v1 = v2;
                                x1 = x2;
                            }
                        }
                        yOffset += 80; 
                        break;

                }
            }
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            Start();
            buttonRecord.Enabled = false;
            buttonPause.Enabled = true;
        }

        private void Analyze()
        {
            int firstIndex = memory.Count < maxMemorySize ? 0 : memoryPointer;
            int lastIndex = memory.Count < maxMemorySize ? memory.Count - 1 : (memoryPointer + maxMemorySize - 1) % maxMemorySize;
            if (lastIndex < 0)
                return;

            var firstRecord = memory[firstIndex];
            var lastRecord = memory[lastIndex];

            TimeSpan firstTime = TimeSpan.FromSeconds((double)firstRecord.TimeStamp / Stopwatch.Frequency);
            TimeSpan lastTime = TimeSpan.FromSeconds((double)lastRecord.TimeStamp / Stopwatch.Frequency);

            if (displayOffset < firstTime)
                displayOffset = firstTime;

            if (displayOffset > lastTime)
                displayOffset = lastTime;

            displayZoom = (lastTime - firstTime).TotalMilliseconds * 1000.0 / (scopeBox.Width - 50);
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            Stop();
            buttonRecord.Enabled = true;
            buttonPause.Enabled = false;
            Analyze();
            scopeBox.Invalidate();
        }

        private void scopeBox_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics, scopeBox.ClientRectangle);
        }
    }
}
