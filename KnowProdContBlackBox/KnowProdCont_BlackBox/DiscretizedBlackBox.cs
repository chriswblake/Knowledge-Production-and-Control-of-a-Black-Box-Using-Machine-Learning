﻿using System;
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

        //Constructor
        public DiscreteBlackBox(BlackBox blackBox, IdManager idManager)
        {
            this.blackBox = blackBox;
            this.idManager = idManager;

            //Create a discretizer for each input and output
            foreach (var inputName in blackBox.Input.Keys)
                Discretizers.Add(inputName, new Discretizer() { GenerateIdDelegate = idManager.GenerateId });
            foreach (var outputName in blackBox.Output.Keys)
                Discretizers.Add(outputName, new Discretizer() { GenerateIdDelegate = idManager.GenerateId });

            //Create a sampling thread for each input and output
            foreach (string inputName in blackBox.Input.Keys)
                learningThreads.Add(inputName, CreateLearningThread("input", inputName));
            foreach (string outputName in blackBox.Output.Keys)
                learningThreads.Add(outputName, CreateLearningThread("output", outputName));
        }

        //Methods
        public Dictionary<string, Bin> Input
        {
            get
            {
                Dictionary<string, Bin> dict = new Dictionary<string, Bin>();
                foreach(var input in blackBox.Input)
                {
                    Discretizer disc = Discretizers[input.Key];
                    double value = Convert.ToDouble(blackBox.Input[input.Key]);
                    Bin bin = disc.GetBin(value);
                    dict[input.Key] = bin;
                }
                return dict;
            }
        }
        public Dictionary<string, Bin> Output
        {
            get
            {
                Dictionary<string, Bin> dict = new Dictionary<string, Bin>();
                foreach (var output in blackBox.Output)
                {
                    Discretizer disc = Discretizers[output.Key];
                    double value = Convert.ToDouble(blackBox.Output[output.Key]);
                    Bin bin = disc.GetBin(value);
                    dict[output.Key] = bin;
                }
                return dict;
            }
        }

        //Methods
        public void Start()
        {
            //Start learning threads
            foreach (Thread t in learningThreads.Values)
                t.Start();

            //Start the black box
            blackBox.Start();
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
            Thread samplingThread = new Thread
            (delegate ()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    //Sample a value and submit for learning
                    disc.Learn(getValue());

                    //Sample the value twice as fast as the black box changes
                    Thread.Sleep(blackBox.TimeInterval_ms / 2);

                    //If discretizers' sizes are not changing, slow down sampling
                    // - Not yet implemented
                }
            });
            samplingThread.IsBackground = true;

            return samplingThread;
        }
    }
}
