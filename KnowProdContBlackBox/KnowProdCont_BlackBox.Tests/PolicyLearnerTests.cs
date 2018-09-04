using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using IdManagement;
using BlackBoxModeling;
using KnowledgeProduction;
using static Discretization.DataGeneration;
using System.Threading;
using RLDT;
using System.IO;
using Discretization;

namespace KnowProdContBlackBox.Tests
{
    public class PolicyLearnerTests
    {
        [Fact]
        public void Constructor()
        {
            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Interpreter interpreter = new Interpreter(prodBlackBox);
            PolicyLearner policyLearner = new PolicyLearner(interpreter, idManager);

            Assert.Equal(3, policyLearner.Policies.Count);
        }

        [Theory]
        [InlineData(1000)]
        public void Learn_LogicOperators(int iterations)
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
        }
    }
}