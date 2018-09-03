using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RLDT;
using KnowledgeProduction;
using IdManagement;
using System.Threading;

namespace KnowProdContBlackBox
{
    public class PolicyLearner
    {
        //Fields
        private Interpreter interpreter;
        private IdManager idManager;

        //Properties
        public Dictionary<string, Policy> Policies = new Dictionary<string, Policy>();

        //Constructor
        public PolicyLearner(Interpreter interpreter, IdManager idManager)
        {
            //Save blackbox
            this.interpreter = interpreter;
            this.idManager = idManager;

            //Create a policy for each output
            foreach(var outputName in this.interpreter.OutputNames)
            {
                Policies.Add(outputName, new Policy());
            }

            //As items are placed in memory send them to the learner.
            this.interpreter.OnAddedToMemory += Interpreter_OnAddedToMemory;
            this.interpreter.OnKnowInstanceRemoved += Interpreter_OnKnowInstanceRemoved;
        }

        //Events
        private void Interpreter_OnAddedToMemory(object sender, Interpreter.AddedToMemoryEventArgs e)
        {
            Learn(e.inputState, e.outputState);
        }
        private void Interpreter_OnKnowInstanceRemoved(object sender, Producer.KnowInstanceRemovedEventArgs e)
        {
            Producer prod = e.SourceProducer;
            KnowInstance ki = e.SourceKnowInstance;

            //If producer is an output
            if (Policies.ContainsKey(prod.Name))
            {
                FeatureValuePair label = new FeatureValuePair(prod.Name, ki);
                Policies[prod.Name].RemoveLabel(label);
                return;
            }

            //If feature is an input, send it to all policies
            foreach (string outputName in this.interpreter.OutputNames)
            {
                Policy policy = this.Policies[outputName];
                FeatureValuePair fvp = new FeatureValuePair(prod.Name, ki);

                policy.RemoveStatesWithFeature(fvp);
                policy.RemoveQueriesWithFeature(fvp);
            }
        }

        //Methods - Learning
        private DataVectorTraining ConvertToDataVectorTraining(Dictionary<string, KnowInstance> input, string outputName, KnowInstance output)
        {
            DataVectorTraining dvt = new DataVectorTraining();

            //Input data becomes Features
            foreach(var i in input.Where(p=> p.Value != null))
            {
                string inputName = i.Key;
                KnowInstanceWithMetaData ki = new KnowInstanceWithMetaData(i.Value, idManager);
                dvt.AddFeature(inputName, ki, 0);
            }

            //Output data becomes Label
            if (output != null)
                dvt.SetLabel(outputName, new KnowInstanceWithMetaData(output, idManager));
            else
                dvt.Label = null;

            return dvt;
        }
        private void Learn(Dictionary<string, KnowInstance> inputState, Dictionary<string, KnowInstance> outputState)
        {
            //Submit each output to the respective learner
            foreach (var o in outputState)
            {
                //Get relavent parts
                string outputName = o.Key;
                KnowInstance label = o.Value;
                Policy policy = this.Policies[outputName];
                DataVectorTraining dvt = ConvertToDataVectorTraining(inputState, outputName, label);
                policy.Learn(dvt);
            }
        }        
    }
}
