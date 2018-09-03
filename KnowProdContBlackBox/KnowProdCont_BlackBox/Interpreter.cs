using System;
using System.Collections.Generic;
using System.Text;
using Discretization;
using System.Threading;
using KnowledgeProduction;
using IdManagement;
using BlackBoxModeling;
using System.Linq;

namespace KnowProdContBlackBox
{
    public class Interpreter
    {
        //Fields
        private ProducerBlackBox prodBlackBox;
        private Thread samplingThread;

        //Properties
        public Queue<Dictionary<string, KnowInstance>> MemoryIOState { get; set; } = new Queue<Dictionary<string, KnowInstance>>();
        public int MemorySize { get; set; } = 20;

        //Properties - Inner black box
        public int TimeCurrent_ms
        {
            get
            {
                return this.prodBlackBox.TimeCurrent_ms;
            }
        }
        public int TimeInterval_ms
        {
            get
            {
                return this.prodBlackBox.TimeInterval_ms;
            }
        }
        public List<string> InputNames
        {
            get
            {
                return prodBlackBox.InputNames; ;
            }
        }
        public List<string> OutputNames
        {
            get
            {
                return prodBlackBox.OutputNames;
            }
        }
        public event EventHandler OnStarting
        {
            add { this.prodBlackBox.OnStarting += value; }
            remove { this.prodBlackBox.OnStarting -= value; }
        }
        public event EventHandler OnStarted
        {
            add { this.prodBlackBox.OnStarted += value; }
            remove { this.prodBlackBox.OnStarted -= value; }
        }
        public event EventHandler<Producer.KnowInstanceRemovedEventArgs> OnKnowInstanceRemoved
        {
            add { this.prodBlackBox.OnKnowInstanceRemoved += value; }
            remove { this.prodBlackBox.OnKnowInstanceRemoved -= value; }
        }

        //Constructors
        public Interpreter(ProducerBlackBox prodBlackBox)
        {
            this.prodBlackBox = prodBlackBox;

            //Create thread for sampling and interpreting inputs and outputs
            this.samplingThread = CreateSamplingThread();
            prodBlackBox.OnStarting += ProdBlackBox_OnStarting; //Start when black box starts
        }

        //Methods
        private void ProdBlackBox_OnStarting(object sender, EventArgs e)
        {
            samplingThread.Start();
        }
        private Thread CreateSamplingThread()
        {
            //Create the background sampling thread
            Thread samplingThread = new Thread
            (delegate ()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    //Get state of inputs and outputs
                    var ioState = prodBlackBox.InputAndOutput;
                    AddToMemory(ioState);

                    //Send through interpreter
                    var ioStateInter = Interpret(_prevIOState, ioState);
                    AddToMemory(ioStateInter);

                    //Update prev state
                    _prevIOState = ioState;

                    //Wait until next sample time
                    Thread.Sleep(prodBlackBox.TimeInterval_ms/2);
                }
            });
            samplingThread.Name = "InterpreterSampling";
            //samplingThread.IsBackground = true;

            return samplingThread;
        }
        private void AddToMemory(Dictionary<string, KnowInstance> ioState)
        {
            //Check memory length
            if (MemoryIOState.Count == MemorySize)
                MemoryIOState.Dequeue();

            //Add the new item
            MemoryIOState.Enqueue(ioState);

            //Trigger event
            var inputState = ioState.Where(p => InputNames.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            var outputState = ioState.Where(p => OutputNames.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            OnAddedToMemory?.Invoke(this, new AddedToMemoryEventArgs(inputState, outputState));
        }
        private Dictionary<string, KnowInstance> Interpret(Dictionary<string, KnowInstance> state1, Dictionary<string, KnowInstance> state2)
        {
            //if first state is null, just return second state
            if (state1 == null)
                return state2;

            //Disconnect original states
            state1 = new Dictionary<string, KnowInstance>(state1);
            state2 = new Dictionary<string, KnowInstance>(state2);

            //Combine values and run through producer for interpretation.
            Dictionary<string, KnowInstance> ioInterpretation = new Dictionary<string, KnowInstance>(state2);
            foreach (string ioName in state1.Keys)
            {
                Producer prod = prodBlackBox.Producers[ioName];
                KnowInstance k1 = state1[ioName];
                KnowInstance k2 = state2[ioName];
                KnowInstance interpretation = prod.Get(k1, k2);
                if (interpretation != null)
                    ioInterpretation[ioName] = interpretation;
            }

            return ioInterpretation;
        }

        //Events
        public event EventHandler<AddedToMemoryEventArgs> OnAddedToMemory;
        public class AddedToMemoryEventArgs : EventArgs
        {
            //Fields
            public Dictionary<string, KnowInstance> inputState;
            public Dictionary<string, KnowInstance> outputState;

            //Constructor
            public AddedToMemoryEventArgs(Dictionary<string, KnowInstance> inputState, Dictionary<string, KnowInstance> outputState)
            {
                this.inputState = inputState;
                this.outputState = outputState;
            }
        }

        //Methods - cache
        private Dictionary<string, KnowInstance> _prevIOState = null;
    }
}
