using System;
using System.Collections.Generic;
using System.Text;
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
        }

        //Methods - Operation
        public void Start()
        {
            discBlackBox.Start();
        }

        //Methods - Learning
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

        //Methods - Operation
        public void SetInput(KnowInstanceValue kiv)
        {

        }

    }
}
