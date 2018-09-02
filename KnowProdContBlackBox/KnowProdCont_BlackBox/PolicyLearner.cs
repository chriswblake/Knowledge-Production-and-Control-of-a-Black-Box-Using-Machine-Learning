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

            //As knowledge items are removed, remove them from the policy
            this.interpreter.OnKnowInstanceRemoving += Interpreter_OnKnowInstanceRemoving;
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
                dvt.Features.Add(new FeatureValuePairWithImportance(inputName, ki, 0));
            }

            //Output data becomes Label
            if (output != null)
                dvt.Label = new FeatureValuePair(outputName, new KnowInstanceWithMetaData(output, idManager));
            else
                dvt.Label = null;

            return dvt;
        }
        private void Interpreter_OnAddedToMemory(object sender, Interpreter.AddedToMemoryEventArgs e)
        {
            var inputState = e.inputState;
            var outputState = e.outputState;

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
        private void Interpreter_OnKnowInstanceRemoving(object sender, ProducerBlackBox.KnowInstanceRemovingEventArgs e)
        {
            string ioName = e.ProducerName; //The name of the input or output.
            KnowInstance ki = e.RemovedKnowInstance; //The piece of removed knowledge.

            //Create equivalent feature value pair
            FeatureValuePair fvp = new FeatureValuePair(ioName, ki);

            //Remove from all policies //Slow: this should be parallelized
            foreach (Policy thePolicy in this.Policies.Values)
                thePolicy.RemoveFeatureValuePair(fvp);
        }
    }
}
