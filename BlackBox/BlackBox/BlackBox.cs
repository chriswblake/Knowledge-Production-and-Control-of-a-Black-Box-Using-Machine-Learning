﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace BlackBoxModeling
{
    public abstract class BlackBox
    {
        //Fields
        private Dictionary<string, object> input = new Dictionary<string, object>();
        private Dictionary<string, object> output = new Dictionary<string, object>();
        private int timeCurrent_ms = 0;

        //Properties
        public string Name { get; set; }
        public int TimeInterval_ms { get; set; }
        public int TimeCurrent_ms { get { return timeCurrent_ms; } }
        public Thread RunThread { get; set; }
        public Dictionary<string, object> Input { get { return input; } }
        public Dictionary<string, object> Output { get { return output; } }
        public Dictionary<string, object> InputAndOuput
        {
            get
            {
                lock(processingLock )
                {
                    var i = new Dictionary<string, object>(input);
                    var o = new Dictionary<string, object>(output);
                    return i.Concat(o).ToDictionary(p => p.Key, p => p.Value);
                }
                    
            }
        }
        private static Object processingLock = new object();

        //Constructors
        public BlackBox() : this("Undefined")
        { }
        public BlackBox(string name)
        {
            this.Name = name;
            this.TimeInterval_ms = 100;

            //Start()
        }

        //Methods
        public void Start()
        {
            //Define thread for running
            RunThread = new Thread
                (delegate ()
                {
                    while (RunThread.IsAlive)
                    {
                        lock(processingLock)
                            Run();
                        Thread.Sleep(TimeInterval_ms);
                        timeCurrent_ms += TimeInterval_ms;
                    }
                });
            RunThread.Name = "BlackBoxRun";
            //RunThread.IsBackground = true;

            //Start black box
            OnStarting?.Invoke(this, new EventArgs()); //trigger pre-start event
            RunThread.Start();
            OnStarted?.Invoke(this, new EventArgs()); //trigger post-start event
        }
        public void Stop()
        {
            if(RunThread != null)
                RunThread.Abort();
        }
        public void Restart()
        {
            //Kill Thread if it is alive
            Stop();
            
            //Start new thread
            Start();
        }
        public void AddInput(string name)
        {
            input.Add(name, null);
        }
        public void AddOutput(string name)
        {
            output.Add(name, null);
        }
        public abstract void Run();

        //Events
        public event EventHandler OnStarting;
        public event EventHandler OnStarted;

        //Debug
        public override string ToString()
        {
            return string.Format("{0}: In:{1} Out:{2}", this.Name, this.Input.Count, this.Output.Count);
        }
    }
}
