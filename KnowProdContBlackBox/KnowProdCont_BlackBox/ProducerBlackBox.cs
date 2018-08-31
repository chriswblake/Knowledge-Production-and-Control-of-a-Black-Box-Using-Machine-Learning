using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using IdManagement;
using Discretization;
using KnowledgeProduction;
using System.Linq;

namespace KnowProdContBlackBox
{
    public class ProducerBlackBox
    {
        //Fields
        private DiscreteBlackBox discBlackBox;
        private IdManager idManager;
        public Dictionary<string, Producer> Producers = new Dictionary<string, Producer>();
        private Thread samplingThread;

        //Properties
        public int TimeCurrent_ms
        {
            get
            {
                return this.discBlackBox.TimeCurrent_ms;
            }
        }
        public int TimeInterval_ms
        {
            get
            {
                return this.discBlackBox.TimeInterval_ms;
            }
        }
        public Dictionary<string, KnowInstanceValue> Input
        {
            get
            {
                return ConvertToKI(this.discBlackBox.Input);
            }
        }
        public Dictionary<string, KnowInstanceValue> Output
        {
            get
            {
                return ConvertToKI(this.discBlackBox.Output);
            }
        }
        public List<string> InputNames
        {
            get
            {
                return discBlackBox.Input.Keys.ToList();
            }
        }
        public List<string> OutputNames
        {
            get
            {
                return discBlackBox.Output.Keys.ToList();
            }
        }
        public event EventHandler OnStarting
        {
            add { this.discBlackBox.OnStarting += value; }
            remove { this.discBlackBox.OnStarting -= value; }
        }
        public event EventHandler OnStarted
        {
            add { this.discBlackBox.OnStarted += value; }
            remove { this.discBlackBox.OnStarted -= value; }
        }

        //Constructors
        public ProducerBlackBox(DiscreteBlackBox blackBox, IdManager idManager)
        {
            this.discBlackBox = blackBox;
            this.idManager = idManager;

            //Create a producer for each input and output. Subscribe to changes in the discretizer
            foreach (var inputName in blackBox.Input.Keys)
            {
                Producers.Add(inputName, new Producer() { GenerateIdDelegate = idManager.GenerateId });
                blackBox.Discretizers[inputName].OnMergeBins += Discretizer_OnMergeBins;
                blackBox.Discretizers[inputName].OnSplitBin += Discretizer_OnSplitBin;
            }
            foreach (var outputName in blackBox.Output.Keys)
            {
                Producers.Add(outputName, new Producer() { GenerateIdDelegate = idManager.GenerateId });
                blackBox.Discretizers[outputName].OnMergeBins += Discretizer_OnMergeBins;
                blackBox.Discretizers[outputName].OnSplitBin += Discretizer_OnSplitBin;
            }

            //Create knowledge instances for existing bins
            foreach(var d in discBlackBox.Discretizers)
            {
                string key = d.Key;
                Discretizer disc = d.Value;
                Producer prod = Producers[key];
                foreach(Bin theBin in disc.Bins)
                    prod.Add(theBin.BinID, theBin);
            }

            //Create thread for sampling inputs and outputs
            this.samplingThread = CreateSamplingThread();
            discBlackBox.OnStarting += DiscBlackBox_OnStarting; //Start when black box starts
        }

        //Methods - Sync with discretizers
        private void Discretizer_OnMergeBins(object sender, Discretizer.MergeBinsEventArgs e)
        { 
            Discretizer disc = (Discretizer)sender;

            //Find key
            string key = discBlackBox.Discretizers.Where(p => p.Value == disc).First().Key; //This is not a fast method and should be replaced someday.

            //Find producer
            Producer prod = Producers[key];

            //Remove old items
            prod.Remove(e.OrigBinLow.BinID);
            prod.Remove(e.OrigBinHigh.BinID);

            //Create new item
            prod.Add(e.NewBin.BinID, e.NewBin);
        }
        private void Discretizer_OnSplitBin(object sender, Discretizer.SplitBinEventArgs e)
        {
            Discretizer disc = (Discretizer)sender;

            //Find key
            string key = discBlackBox.Discretizers.Where(p => p.Value == disc).First().Key; //This is not a fast method and should be replaced someday.

            //Find producer
            Producer prod = Producers[key];

            //Remove old items
            prod.Remove(e.OrigBin.BinID);

            //Create new item
            prod.Add(e.NewBinLow.BinID, e.NewBinLow);
            prod.Add(e.NewBinHigh.BinID, e.NewBinHigh);
        }

        //Methods - Sampling/Learning
        private Dictionary<string, KnowInstanceValue> ConvertToKI(Dictionary<string, Bin> inputState)
        {
            var inputStateConverted = new Dictionary<string, KnowInstanceValue>();
            foreach (var i in inputState)
            {
                string key = i.Key;
                Bin theBin = i.Value;
                KnowInstanceValue kiv = (KnowInstanceValue) this.Producers[key].Get(theBin.BinID);
                inputStateConverted[key] = kiv;
            }

            return inputStateConverted;
        }
        private void DiscBlackBox_OnStarting(object sender, EventArgs e)
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
                    var inputState = ConvertToKI(discBlackBox.Input);
                    var outputState = ConvertToKI(discBlackBox.Output);

                    //Submit to related producers
                    foreach (var i in inputState)
                    {
                        string inputName = i.Key;
                        KnowInstance ki = i.Value;
                        Producer prod = this.Producers[inputName];
                        prod.Learn(ki);
                    }
                    foreach (var o in outputState)
                    {
                        string outputName = o.Key;
                        KnowInstance ki = o.Value;
                        Producer prod = this.Producers[outputName];
                        prod.Learn(ki);
                    }

                    //Wait until next sample time
                    Thread.Sleep(discBlackBox.TimeInterval_ms);
                }
            });
            samplingThread.IsBackground = true;

            return samplingThread;
        }

    }
}
