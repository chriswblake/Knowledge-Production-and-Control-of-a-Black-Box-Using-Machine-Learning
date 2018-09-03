using System;
using System.Collections.Generic;
using System.Text;
using BlackBoxModeling;
using Discretization;
using IdManagement;
using System.Linq;
using System.Threading;

namespace KnowProdContBlackBox
{
    public class DiscreteBlackBox
    {
        //Fields
        private BlackBox blackBox;
        private IdManager idManager;
        public Dictionary<string, Discretizer> Discretizers = new Dictionary<string, Discretizer>();
        private Dictionary<string, Thread> learningThreads = new Dictionary<string, Thread>();

        //Properties
        public int TimeCurrent_ms
        {
            get
            {
                return this.blackBox.TimeCurrent_ms;
            }
        }
        public int TimeInterval_ms {
            get
            {
                return this.blackBox.TimeInterval_ms;
            }
        }
        public Dictionary<string, Bin> InputAndOutput
        {
            get
            {
                return ConvertToBin(blackBox.InputAndOuput);
            }
        }
        public List<string> InputNames
        {
            get
            {
                return blackBox.Input.Keys.ToList();
            }
        }
        public List<string> OutputNames
        {
            get
            {
                return blackBox.Output.Keys.ToList();
            }
        }
        public event EventHandler OnStarting
        {
            add { this.blackBox.OnStarting += value; }
            remove { this.blackBox.OnStarting -= value; }
        }
        public event EventHandler OnStarted
        {
            add { this.blackBox.OnStarted += value; }
            remove { this.blackBox.OnStarted -= value; }
        }

        //Constructor
        public DiscreteBlackBox(BlackBox blackBox, IdManager idManager)
        {
            this.blackBox = blackBox;
            this.idManager = idManager;

            //Create a discretizer for each input and output
            foreach (var inputName in blackBox.Input.Keys)
            { 
                Discretizers.Add(inputName, new Discretizer(idManager.GenerateId) { Name = inputName });
                learningThreads.Add(inputName, CreateLearningThread("input", inputName));

            }
            foreach (var outputName in blackBox.Output.Keys)
            {
                Discretizers.Add(outputName, new Discretizer(idManager.GenerateId) { Name = outputName });
                learningThreads.Add(outputName, CreateLearningThread("output", outputName));
            }

            //Subscribe to start event
            this.blackBox.OnStarting += BlackBox_OnStarting;
        }

        //Methods - Usage
        public void Start()
        {
            blackBox.Start();
        }

        //Methods - Learning
        private void BlackBox_OnStarting(object sender, EventArgs e)
        {
            //Start learning threads
            foreach (Thread t in learningThreads.Values)
                t.Start();
        }
        private Dictionary<string, Bin> ConvertToBin(Dictionary<string, object> ioState)
        {
            var inputStateConverted = new Dictionary<string, Bin>();
            foreach (var io in ioState)
            {
                string ioName = io.Key;

                if (io.Value != null)
                {
                    double value = Convert.ToDouble(io.Value);
                    Bin bin = this.Discretizers[ioName].GetBin(value);
                    inputStateConverted[ioName] = bin;
                }else
                    inputStateConverted[ioName] = null;
            }

            return inputStateConverted;
        }
        private Thread CreateLearningThread(string source, string name)
        {
            //Get discretizer and value
            Discretizer disc = Discretizers[name];
            Func<double> getValue;
            switch(source)
            {
                case "input":
                    getValue = delegate { return Convert.ToDouble(blackBox.Input[name]); };
                    break;
                case "output":
                    getValue = delegate { return Convert.ToDouble(blackBox.Output[name]); };
                    break;
                default:
                    throw new InvalidOperationException("'source' parameter must be 'input' or 'output'");
            }

            //Create the background sampling thread
            Thread learningThread = new Thread
            (delegate ()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    //Sample a value and submit for learning
                    disc.Learn(getValue());

                    //Sample the value twice as fast as the black box changes
                    Thread.Sleep(blackBox.TimeInterval_ms / 2);
                }
            });
            learningThread.Name = "DiscreteBlackBox_LearningThread_"+name;
            //samplingThread.IsBackground = true;

            return learningThread;
        }
    }
}
