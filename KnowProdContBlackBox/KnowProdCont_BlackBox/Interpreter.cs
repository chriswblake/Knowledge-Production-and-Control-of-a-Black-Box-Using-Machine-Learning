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
        public Queue<Dictionary<string, KnowInstance>> MemoryInput { get; set; } = new Queue<Dictionary<string, KnowInstance>>();
        public Queue<Dictionary<string, KnowInstance>> MemoryOutput { get; set; } = new Queue<Dictionary<string, KnowInstance>>();
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
                return prodBlackBox.Input.Keys.ToList();
            }
        }
        public List<string> OutputNames
        {
            get
            {
                return prodBlackBox.Output.Keys.ToList();
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
        public event EventHandler<ProducerBlackBox.KnowInstanceRemovingEventArgs> OnKnowInstanceRemoving
        {
            add { this.prodBlackBox.OnKnowInstanceRemoving += value; }
            remove { this.prodBlackBox.OnKnowInstanceRemoving -= value; }
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
                    //Sample values of inputs and outputs
                    var inputState = prodBlackBox.Input.ToDictionary(d => d.Key, d => (KnowInstance)d.Value);
                    var outputState = prodBlackBox.Output.ToDictionary(d => d.Key, d => (KnowInstance)d.Value);

                    //Check for nulls (not identified) //Note: this should never happen but it does, so there is a bug somewhere.
                    bool validStates = true;
                    if (inputState.Select(p => p.Value).Contains(null))
                        validStates = false;
                    if (outputState.Select(p => p.Value).Contains(null))
                        validStates = false;

                    //Add to memory
                    if (validStates)
                        AddToMemory(inputState, outputState);

                    //Send through interpreter
                    var inputStateInter = Interpret(_prevInputState, inputState);
                    var outputStateInter = Interpret(_prevOutputState, outputState);

                    //Check for nulls (not identified) //Note: this should never happen but it does, so there is a bug somewhere.
                    validStates = true;
                    if (inputStateInter.Select(p => p.Value).Contains(null))
                        validStates = false;
                    if (outputStateInter.Select(p => p.Value).Contains(null))
                        validStates = false;

                    //Add to memory
                    if (validStates)
                        AddToMemory(inputStateInter, outputStateInter);



                    //Update prev state
                    _prevInputState = inputState;
                    _prevOutputState = outputState;

                    //Wait until next sample time
                    Thread.Sleep(prodBlackBox.TimeInterval_ms);
                }
            });
            samplingThread.IsBackground = true;

            return samplingThread;
        }
        private void AddToMemory(Dictionary<string, KnowInstance> inputState, Dictionary<string, KnowInstance> outputState)
        {
            //Check memory length
            if (MemoryInput.Count == MemorySize)
            {
                MemoryInput.Dequeue();
                MemoryOutput.Dequeue();
            }

            //Add the new item
            MemoryInput.Enqueue(inputState);
            MemoryOutput.Enqueue(outputState);

            //Trigger event
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
        private Dictionary<string, KnowInstance> _prevInputState = null;
        private Dictionary<string, KnowInstance> _prevOutputState = null;
    }
}
