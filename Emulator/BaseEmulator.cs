using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emulator
{
    public abstract class BaseEmulator
    {
        public class OnCpuStepEventArgs: EventArgs
        {
            public long StateCounter { get; set; }
        }

        public class OnBreakpointHitEventArgs: EventArgs
        {
            public Breakpoint Breakpoint { get; set; }
        }
        
        protected BaseRegisters registers;
        protected bool pauseRequest;
        protected bool running;
        protected object syncRun = new();
        protected ManualResetEvent runComplete = new(true);
        protected int targetAddress = -1;
        protected long totalStates;
        protected long statesTaken;
        protected double timeTaken;             // [s]
        protected double clockFrequency;        // [MHz]
        protected long targetStatesPerSecond;   // Desired number of states per second, given clockFrequency
        protected Breakpoint[] breakpoints;
        
        public abstract void Emulate();
        public abstract void Reset();

        public BaseEmulator()
        {
            ClockFrequency = 3.3;
            breakpoints = new Breakpoint[0x10000];
        }

        public int TargetAddress
        {
            get { return targetAddress; }
            set { targetAddress = value; }
        }

        public long StatesTaken
        {
            get { return statesTaken; }
        }

        public void UpdateBreakpoints(IEnumerable<Breakpoint> breakpoints)
        {
            for (int n = 0; n < 0x10000; n++)
                this.breakpoints[n] = null;
            foreach (var breakpoint in breakpoints)
            {
                this.breakpoints[breakpoint.Address & 0xFFFF] = breakpoint;
            }
        }

        public double TimeTaken
        {
            get { return timeTaken; }
        }

        public double ClockFrequency
        {
            get { return clockFrequency; }
            set 
            { 
                clockFrequency = value;
                targetStatesPerSecond = (long)(clockFrequency * 1E6);
            }
        }

        public event EventHandler<OnCpuStepEventArgs> OnCpuStep;
        public event EventHandler OnRunComplete;
        public event EventHandler<OnBreakpointHitEventArgs> OnBreakpointHit;

        public void Run()
        {
            if (!running)
            {
                lock (syncRun)
                {
                    if (!running)
                    {
                        try
                        {
                            running = true;
                            runComplete.Reset();
                            pauseRequest = false;
                            new Thread(new ThreadStart(() =>
                                {
                                    long stateStart = totalStates;
                                    var stopWatch = new Stopwatch();
                                    stopWatch.Start();
                                    var spinWait = new SpinWait();
                                    Breakpoint breakpoint = null;
                                    while (!pauseRequest && registers.PC != targetAddress)
                                    {
                                        Emulate();
                                        OnCpuStep?.Invoke(this, new OnCpuStepEventArgs { StateCounter = totalStates });
                                        long actualStatesPerSecond = (totalStates - stateStart) * Stopwatch.Frequency / stopWatch.ElapsedTicks;
                                        if (actualStatesPerSecond > targetStatesPerSecond)
                                            spinWait.SpinOnce();    // Introduce a short delay keep up with the set ClockFrequency
                                        if (breakpoints[registers.PC] != null)
                                        {
                                            breakpoint = breakpoints[registers.PC];
                                            break;
                                        }
                                    }
                                    stopWatch.Stop();
                                    timeTaken = stopWatch.ElapsedTicks / (double)Stopwatch.Frequency;
                                    statesTaken = totalStates - stateStart;

                                    targetAddress = -1;
                                    runComplete.Set();
                                    OnRunComplete?.Invoke(this, new EventArgs());
                                    if (breakpoint != null && OnBreakpointHit != null)
                                        OnBreakpointHit(this, new OnBreakpointHitEventArgs { Breakpoint = breakpoint });
                                }
                            )).Start();
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            running = false;
                        }
                    }
                }
            }
        }

        public void Pause()
        {
            pauseRequest = true;
            runComplete.WaitOne();
        }

        public BaseRegisters Registers
        {
            get { return registers; }
        }

        public Func<int, byte> ReadMemory { get; set; }
        public Action<int, byte> WriteMemory { get; set; }
        public Func<int, byte> ReadInput { get; set; }
        public Action<int, byte> WriteOutput { get; set; }
    }
}
