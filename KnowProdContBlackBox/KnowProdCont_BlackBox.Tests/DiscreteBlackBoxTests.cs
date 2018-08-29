using System;
using System.Text;
using Xunit;
using System.Collections.Generic;
using BlackBoxModeling;
using IdManagement;
using static Discretization.DataGeneration;

namespace KnowProdContBlackBox.Tests
{
    public class DiscreteBlackBoxTests
    {
        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        //[InlineData(5000)]
        public void LearningThread_sinFunction_LongTest_SeeAssert(int iterations)
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
            BlackBox trigExample = new BlackBoxModeling.Samples.TrigFunctions() { TimeInterval_ms = 10 };
            DiscreteBlackBox blackBox = new DiscreteBlackBox(trigExample, new IdManager());
            Random rand = new Random();
            blackBox.Start();


            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double x_crisp = x_values[rand.Next(0, 10)];
                trigExample.Input["x"] = GenerateNoisyValue(rand, x_crisp, 0.001);

                //Wait
                System.Threading.Thread.Sleep(trigExample.TimeInterval_ms);
            }

            Assert.InRange(blackBox.Discretizers["x"].Bins.Count, 12, 14);
            Assert.InRange(blackBox.Discretizers["sin"].Bins.Count, 8, 10);
            Assert.InRange(blackBox.Discretizers["cos"].Bins.Count, 12, 14);
            //Assert.Equal(12, blackBox.Discretizers["tan"].Bins.Count); This has many values because it goes to infinity.
        }
    }
}
