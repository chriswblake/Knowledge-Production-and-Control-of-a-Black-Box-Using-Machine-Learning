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
        public Dictionary<string, KnowInstance> InputAndOutput
        {
            get
            {
                return ConvertToKI(this.discBlackBox.InputAndOutput);
            }
        }
        public List<string> InputNames
        {
            get
            {
                return discBlackBox.InputNames;;
            }
        }
        public List<string> OutputNames
        {
            get
            {
                return discBlackBox.OutputNames;
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
        public ProducerBlackBox(DiscreteBlackBox discBlackBox, IdManager idManager)
        {
            this.discBlackBox = discBlackBox;
            this.idManager = idManager;
        
            //Create a producer for each input and output. Subscribe to changes in the discretizer
            foreach (var ioName in discBlackBox.InputAndOutput.Keys)
            {
                Producers.Add(ioName, new Producer() { GenerateIdDelegate = idManager.GenerateId });
                discBlackBox.Discretizers[ioName].OnMergeBins += Discretizer_OnMergeBins;
                discBlackBox.Discretizers[ioName].OnSplitBin += Discretizer_OnSplitBin;
            }

            //Create knowledge instances for existing bins
            foreach (var d in this.discBlackBox.Discretizers)
            {
                string key = d.Key;
                Discretizer disc = d.Value;
                Producer prod = Producers[key];
                foreach(Bin theBin in disc.Bins)
                    prod.Add(theBin.BinID, theBin);
            }

            //Create thread for sampling inputs and outputs
            this.samplingThread = CreateSamplingThread();
            this.discBlackBox.OnStarting += DiscBlackBox_OnStarting; //Start when black box starts
        }

        //Methods - Sync with discretizers
        private void Discretizer_OnMergeBins(object sender, Discretizer.MergeBinsEventArgs e)
        { 
            Discretizer disc = (Discretizer)sender;

            //Find key
            string prodName = discBlackBox.Discretizers.Where(p => p.Value == disc).First().Key; //This is not a fast method and should be replaced someday.

            //Find producer
            Producer prod = Producers[prodName];

            //Trigger events
            OnKnowInstanceRemoving?.Invoke(this, new KnowInstanceRemovingEventArgs(prodName, prod.Get(e.OrigBinLow.BinID)));
            OnKnowInstanceRemoving?.Invoke(this, new KnowInstanceRemovingEventArgs(prodName, prod.Get(e.OrigBinHigh.BinID)));

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

        //Methods - Events
        public event EventHandler<KnowInstanceRemovingEventArgs> OnKnowInstanceRemoving;
        public class KnowInstanceRemovingEventArgs :EventArgs
        {
            public string ProducerName { get; private set; }
            public KnowInstance RemovedKnowInstance { get; private set; }
            public KnowInstanceRemovingEventArgs(string producerName, KnowInstance ki)
            {
                this.ProducerName = producerName;
                this.RemovedKnowInstance = ki;
            }
        }

        //Methods - Sampling/Learning
        private Dictionary<string, KnowInstance> ConvertToKI(Dictionary<string, Bin> ioState)
        {
            var inputStateConverted = new Dictionary<string, KnowInstance>();
            foreach (var io in ioState)
            {
                string ioName = io.Key;
                if (io.Value != null)
                {
                    Bin theBin = io.Value;
                    KnowInstanceValue kiv = (KnowInstanceValue)this.Producers[ioName].Get(theBin.BinID);
                    inputStateConverted[ioName] = kiv;
                }
                else
                    inputStateConverted[ioName] = null;
                
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
                    //Get state of black box
                    var ioStateBins = discBlackBox.InputAndOutput;
                    var ioStateKI = ConvertToKI(ioStateBins);

                    //Submit to producers
                    foreach (var i in ioStateKI)
                    {
                        string ioName = i.Key;
                        KnowInstance ki = i.Value;
                        Producer prod = this.Producers[ioName];
                        prod.Learn(ki);
                    }

                    //Wait until next sample time
                    Thread.Sleep(discBlackBox.TimeInterval_ms);
                }
            });
            samplingThread.Name = "ProducerBlackBoxSampling";
            //samplingThread.IsBackground = true;

            return samplingThread;
        }

    }
}
