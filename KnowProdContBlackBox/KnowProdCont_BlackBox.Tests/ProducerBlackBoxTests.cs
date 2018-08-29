using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BlackBoxModeling;
using IdManagement;
using KnowledgeProduction;
using static Discretization.DataGeneration;

namespace KnowProdContBlackBox.Tests
{
    public class ProducerBlackBoxTests
    {
        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        public void Learning_sinFunction_LongTest_SeeAssert(int iterations)
        {
            List<double> x_values = new List<double> {
                Math.PI * 0.1,
                Math.PI * 0.2,
                Math.PI * 0.3,
                Math.PI * 0.4,
                Math.PI * 0.5,
                Math.PI * 0.6,
                Math.PI * 0.7,
                Math.PI * 0.8,
                Math.PI * 0.9,
                Math.PI * 1.0
            };
            IdManager idManager = new IdManager();
            BlackBox trigExample = new BlackBoxModeling.Samples.TrigFunctions() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(trigExample, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Random rand = new Random();
            prodBlackBox.Start();

            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double x_crisp = x_values[rand.Next(0, 10)];
                trigExample.Input["x"] = GenerateNoisyValue(rand, x_crisp, 0.001);

                //Wait
                System.Threading.Thread.Sleep(trigExample.TimeInterval_ms);
            }

            Assert.Equal(discBlackBox.Discretizers["x"].Bins.Count,   prodBlackBox.Producers["x"].KnowInstances.Count);
            Assert.Equal(discBlackBox.Discretizers["sin"].Bins.Count, prodBlackBox.Producers["sin"].KnowInstances.Count);
            Assert.Equal(discBlackBox.Discretizers["cos"].Bins.Count, prodBlackBox.Producers["cos"].KnowInstances.Count);
            Assert.Equal(discBlackBox.Discretizers["tan"].Bins.Count, prodBlackBox.Producers["tan"].KnowInstances.Count);
        }
    }
}
