using System;
using System.Collections.Generic;
using System.Threading;

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
                        Run();
                        Thread.Sleep(TimeInterval_ms);
                        timeCurrent_ms += TimeInterval_ms;
                    }
                });
            RunThread.IsBackground = true;
            RunThread.Start();
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

        //Debug
        public override string ToString()
        {
            return string.Format("{0}: In:{1} Out:{2}", this.Name, this.Input.Count, this.Output.Count);
        }
    }
}
