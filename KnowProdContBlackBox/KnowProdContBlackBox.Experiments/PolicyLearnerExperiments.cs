using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using BlackBoxModeling;
using Discretization;
using static Discretization.DataGeneration;
using KnowledgeProduction;
using IdManagement;
using RLDT;

namespace KnowProdContBlackBox.Experiments
{
    public class PolicyLearnerExperiments : Experiment
    {
        [Theory]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(1500)]
        public void LogicOperations(int iterations)
        {
            List<double> bool1 = new List<double> {
                0.0,
                0.0,
                0.0,
                0.0,

                0.0,
                0.0,
                0.0,
                0.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,
            };
            List<double> bool2 = new List<double> {
                0.0,
                0.0,
                0.0,
                0.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,

                    5.0,
                    5.0,
                    5.0,
                    5.0,

                0.0,
                0.0,
                0.0,
                0.0,
            };
            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Interpreter interpreter = new Interpreter(prodBlackBox) { MemorySize = iterations * 2 };
            PolicyLearner policyLearner = new PolicyLearner(interpreter, idManager);
            Random rand = new Random();
            logicBlackBox.Start();

            #region Train
            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double bool1_crisp = bool1[i % bool1.Count];
                double bool2_crisp = bool2[i % bool2.Count];
                logicBlackBox.Input["bool1"] = GenerateNoisyValue(rand, bool1_crisp, 0.001);
                logicBlackBox.Input["bool2"] = GenerateNoisyValue(rand, bool2_crisp, 0.001);

                //Wait until next sample time
                Thread.Sleep(logicBlackBox.TimeInterval_ms);
            }
            #endregion

            //Get Discretizers and Producers
            var disc1 = discBlackBox.Discretizers["bool1"];
            var disc2 = discBlackBox.Discretizers["bool2"];
            var discand = discBlackBox.Discretizers["and"];
            var discor = discBlackBox.Discretizers["or"];
            var discxor = discBlackBox.Discretizers["xor"];
            var prod1 = prodBlackBox.Producers["bool1"];
            var prod2 = prodBlackBox.Producers["bool2"];
            var prodand = prodBlackBox.Producers["and"];
            var prodor = prodBlackBox.Producers["or"];
            var prodxor = prodBlackBox.Producers["xor"];
            Policy policyand = policyLearner.Policies["and"];
            Policy policyor = policyLearner.Policies["or"];
            Policy policyxor = policyLearner.Policies["xor"];

            //Check if training was sucessfull
            Assert.Equal(4, disc1.Bins.Count);
            Assert.Equal(4, disc2.Bins.Count);
            Assert.Equal(4, discand.Bins.Count);
            Assert.Equal(4, discor.Bins.Count);
            Assert.Equal(4, discxor.Bins.Count);
            Assert.Equal(4, discxor.Bins.Count);
            Assert.InRange(prod1.KnowInstances.Count, 8, 10);
            Assert.InRange(prod2.KnowInstances.Count, 8, 10);
            Assert.InRange(prodand.KnowInstances.Count, 8, 10);
            Assert.InRange(prodor.KnowInstances.Count, 8, 10);
            Assert.InRange(prodxor.KnowInstances.Count, 8, 10);

            //Name Entities
            #region bool1
            KnowInstance bool1_On = prod1.Get(disc1.GetBin(5.0).BinID);
            KnowInstance bool1_Off = prod1.Get(disc1.GetBin(0.0).BinID);
            KnowInstance bool1_StayOff = prod1.Get(bool1_Off, bool1_Off);
            KnowInstance bool1_StayOn = prod1.Get(bool1_On, bool1_On);
            KnowInstance bool1_SwitchOff = prod1.Get(bool1_On, bool1_Off);
            KnowInstance bool1_SwitchOn = prod1.Get(bool1_Off, bool1_On);
            idManager.SetName(bool1_On.ID, "True");
            idManager.SetName(bool1_Off.ID, "False");
            idManager.SetName(bool1_StayOff.ID, "Stay False");
            idManager.SetName(bool1_StayOn.ID, "Stay True");
            idManager.SetName(bool1_SwitchOff.ID, "True to False");
            idManager.SetName(bool1_SwitchOn.ID, "False to True");
            #endregion
            #region bool2
            KnowInstance bool2_On = prod2.Get(disc2.GetBin(5.0).BinID);
            KnowInstance bool2_Off = prod2.Get(disc2.GetBin(0.0).BinID);
            KnowInstance bool2_StayOff = prod2.Get(bool2_Off, bool2_Off);
            KnowInstance bool2_StayOn = prod2.Get(bool2_On, bool2_On);
            KnowInstance bool2_SwitchOff = prod2.Get(bool2_On, bool2_Off);
            KnowInstance bool2_SwitchOn = prod2.Get(bool2_Off, bool2_On);
            idManager.SetName(bool2_On.ID, "True");
            idManager.SetName(bool2_Off.ID, "False");
            idManager.SetName(bool2_StayOff.ID, "Stay False");
            idManager.SetName(bool2_StayOn.ID, "Stay True");
            idManager.SetName(bool2_SwitchOff.ID, "True to False");
            idManager.SetName(bool2_SwitchOn.ID, "False to True");
            #endregion
            #region booland
            KnowInstance and_On = prodand.Get(discand.GetBin(5.0).BinID);
            KnowInstance and_Off = prodand.Get(discand.GetBin(0.0).BinID);
            KnowInstance and_StayOff = prodand.Get(and_Off, and_Off);
            KnowInstance and_StayOn = prodand.Get(and_On, and_On);
            KnowInstance and_SwitchedOff = prodand.Get(and_On, and_Off);
            KnowInstance and_SwitchedOn = prodand.Get(and_Off, and_On);
            idManager.SetName(and_On.ID, "True");
            idManager.SetName(and_Off.ID, "False");
            idManager.SetName(and_StayOff.ID, "Stay False");
            idManager.SetName(and_StayOn.ID, "Stay True");
            idManager.SetName(and_SwitchedOff.ID, "True to False");
            idManager.SetName(and_SwitchedOn.ID, "False to True");
            #endregion
            #region boolor
            KnowInstance or_On = prodor.Get(discor.GetBin(5.0).BinID);
            KnowInstance or_Off = prodor.Get(discor.GetBin(0.0).BinID);
            KnowInstance or_StayOff = prodor.Get(or_Off, or_Off);
            KnowInstance or_StayOn = prodor.Get(or_On, or_On);
            KnowInstance or_SwitchedOff = prodor.Get(or_On, or_Off);
            KnowInstance or_SwitchedOn = prodor.Get(or_Off, or_On);
            idManager.SetName(or_On.ID, "True");
            idManager.SetName(or_Off.ID, "False");
            idManager.SetName(or_StayOff.ID, "Stay False");
            idManager.SetName(or_StayOn.ID, "Stay True");
            idManager.SetName(or_SwitchedOff.ID, "True to False");
            idManager.SetName(or_SwitchedOn.ID, "False to True");
            #endregion
            #region boolxor
            KnowInstance xor_On = prodxor.Get(discxor.GetBin(5.0).BinID);
            KnowInstance xor_Off = prodxor.Get(discxor.GetBin(0.0).BinID);
            KnowInstance xor_StayOff = prodxor.Get(xor_Off, xor_Off);
            KnowInstance xor_StayOn = prodxor.Get(xor_On, xor_On);
            KnowInstance xor_SwitchedOff = prodxor.Get(xor_On, xor_Off);
            KnowInstance xor_SwitchedOn = prodxor.Get(xor_Off, xor_On);
            idManager.SetName(xor_On.ID, "True");
            idManager.SetName(xor_Off.ID, "False");
            idManager.SetName(xor_StayOff.ID, "Stay False");
            idManager.SetName(xor_StayOn.ID, "Stay True");
            idManager.SetName(xor_SwitchedOff.ID, "True to False");
            idManager.SetName(xor_SwitchedOn.ID, "False to True");
            #endregion

            //Convert policies to html and save to file
            string htmlTree = HtmlTools.ToHtml(policyLearner, idManager);
            string savePath = Path.Combine(this.ResultsDir, "LogicOperations", iterations + ".html");
            File.WriteAllText(savePath, htmlTree);

            return;
        }
    }
}
