using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using KnowProdContBlackBox;
using Discretization;
using BlackBoxModeling;
using KnowledgeProduction;
using IdManagement;
using static Discretization.DataGeneration;
using System.Threading;

namespace KnowProdContBlackBox.Tests
{
    public class InterpreterTests
    {
        [Theory]
        [InlineData(500)]
        public void Learn_OnOffPattern_Count4(int iterations)
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

            #region Assert
            Assert.InRange(prodBlackBox.Producers["bool1"].KnowInstances.Count, 8, 10);
            List<KnowInstance> vocabBool1 = prodBlackBox.Producers["bool1"].KnowInstances.Values.OrderBy(p => p.ID).ToList();
            List<KnowInstance> memoryBool1 = interpreter.MemoryIOState.Select(p => p["bool1"]).ToList();
            foreach(KnowInstance ki in vocabBool1)
            {
                //Skip bins that have never been used.
                if(ki.GetType() == typeof(KnowInstanceValue))
                {
                    Bin bin = (Bin)ki.Content;
                    if (bin.Count == 0)
                        continue;
                }

                //Check that all vocabulary appear in the memory.
                Assert.Contains(ki, memoryBool1);
            }
            #endregion

        }
    }
}
