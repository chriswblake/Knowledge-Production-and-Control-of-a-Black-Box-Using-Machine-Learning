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

        //Properties - IO state
        public Queue<Dictionary<string, KnowInstance>> MemoryInput { get; set; } = new Queue<Dictionary<string, KnowInstance>>();
        public Queue<Dictionary<string, KnowInstance>> MemoryOutput { get; set; } = new Queue<Dictionary<string, KnowInstance>>();
        public int MemorySize { get; set; } = 20;
        private Thread samplingThread;

        //Properties - Interpratations
        //private Producer prodHighInput;
        //private Producer prodHighOutput;
        //public Queue<KnowInstance> MemoryInterpretationsInput = new Queue<KnowInstance>();
        //public Queue<KnowInstance> MemoryInterpretationsOutput = new Queue<KnowInstance>();

        //Constructors
        public Interpreter(ProducerBlackBox prodBlackBox)
        {
            this.prodBlackBox = prodBlackBox;

            //Create thread for sampling and interpreting inputs and outputs
            this.samplingThread = CreateSamplingThread();
            prodBlackBox.OnStarting += ProdBlackBox_OnStarting; //Start when black box starts
            
        }

        //Methods
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
        }
        private Dictionary<string, KnowInstance> Interpret(Dictionary<string, KnowInstance> state1, Dictionary<string, KnowInstance> state2)
        {
            //if first state is null, just return second state
            if (state1 == null)
                return state2;

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
                    AddToMemory(inputState, outputState);

                    //Send through interpreter
                    var inputStateInter = Interpret(_prevInputState, inputState);
                    var outputStateInter = Interpret(_prevOutputState, outputState);
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

        //Events
        private void ProdBlackBox_OnStarting(object sender, EventArgs e)
        {
            samplingThread.Start();
        }

        //Methods - cache
        private Dictionary<string, KnowInstance> _prevInputState = null;
        private Dictionary<string, KnowInstance> _prevOutputState = null;
    }
}
