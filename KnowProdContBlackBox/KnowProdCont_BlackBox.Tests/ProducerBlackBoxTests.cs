﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using Xunit;
using BlackBoxModeling;
using IdManagement;
using KnowledgeProduction;
using Discretization;
using static Discretization.DataGeneration;

namespace KnowProdContBlackBox.Tests
{
    public class ProducerBlackBoxTests
    {
        [Fact]
        public void Constructor_NBins()
        {
            IdManager idManager = new IdManager();
            BlackBox trigExample = new BlackBoxModeling.Samples.TrigFunctions() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(trigExample, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);

            Assert.Single(prodBlackBox.Producers["x"].KnowInstances);
            Assert.Single(prodBlackBox.Producers["sin"].KnowInstances);
            Assert.Single(prodBlackBox.Producers["cos"].KnowInstances);
            Assert.Single(prodBlackBox.Producers["tan"].KnowInstances);
        }

        [Theory]
        [InlineData(500)]
        public void Learning_sinFunction_SeeAssert(int iterations)
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
            BlackBox trigBlackBox = new BlackBoxModeling.Samples.TrigFunctions() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(trigBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Random rand = new Random();
            trigBlackBox.Start();

            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double x_crisp = x_values[rand.Next(0, 10)];
                trigBlackBox.Input["x"] = GenerateNoisyValue(rand, x_crisp, 0.001);

                //Wait
                Thread.Sleep(trigBlackBox.TimeInterval_ms);
            }

            //Count KnowledgeInstanceValues, but ignore higher level knowledge.
            var resultX = prodBlackBox.Producers["x"].KnowInstances.Values.OfType<KnowInstanceValue>().ToList().Count;
            var resultSin = prodBlackBox.Producers["sin"].KnowInstances.Values.OfType<KnowInstanceValue>().ToList().Count;
            var resultCos = prodBlackBox.Producers["cos"].KnowInstances.Values.OfType<KnowInstanceValue>().ToList().Count;
            var resultTan = prodBlackBox.Producers["tan"].KnowInstances.Values.OfType<KnowInstanceValue>().ToList().Count;

            //Check that counts match
            Assert.Equal(discBlackBox.Discretizers["x"].Bins.Count,   resultX);
            Assert.Equal(discBlackBox.Discretizers["sin"].Bins.Count, resultSin);
            Assert.Equal(discBlackBox.Discretizers["cos"].Bins.Count, resultCos);
            Assert.Equal(discBlackBox.Discretizers["tan"].Bins.Count, resultTan);

            //Check that each ID in the discretizer exists in the producer
            List<string> ioNames = trigBlackBox.Input.Select(p => p.Key).ToList();
            foreach (string ioName in ioNames)
            {
                //Get discretizer and producer
                var disc = discBlackBox.Discretizers[ioName];
                var prod = prodBlackBox.Producers[ioName];

                //Check for each bin id in producer
                foreach (Bin bin in disc.Bins)
                {
                    KnowInstance ki = prod.Get(bin.BinID);
                    Assert.NotNull(ki);
                }
            }
        }

        [Theory]
        [InlineData(500)]
        public void Learn_OnOffPattern_MatchingIDs(int iterations)
        {
            List<double> bool1 = new List<double> {
                0.0,
                0.0,
                0.0,
                    5.0,
                    5.0,
                    5.0,
                    5.0,
                    5.0,
                0.0,
                0.0,
                0.0,
                0.0,
                    5.0,
                    5.0,
                    5.0,
            };
            List<double> bool2 = new List<double> {
                0.0,
                    5.0,
            };
            IdManager idManager = new IdManager();
            BlackBox logicBlackBox = new BlackBoxModeling.Samples.Logic() { TimeInterval_ms = 10 };
            DiscreteBlackBox discBlackBox = new DiscreteBlackBox(logicBlackBox, idManager);
            ProducerBlackBox prodBlackBox = new ProducerBlackBox(discBlackBox, idManager);
            Random rand = new Random();
            logicBlackBox.Start();

            #region Train
            for (int i = 0; i < iterations; i++)
            {
                //Change input
                double bool1_crisp = bool1[i % bool1.Count];
                double bool2_crisp = bool2[rand.Next(0, 2)];
                logicBlackBox.Input["bool1"] = GenerateNoisyValue(rand, bool1_crisp, 0.001);
                logicBlackBox.Input["bool2"] = GenerateNoisyValue(rand, bool2_crisp, 0.001);

                //Wait until next sample time
                Thread.Sleep(logicBlackBox.TimeInterval_ms);
            }
            #endregion

            //Check that there are 8 items for each
            Assert.Equal(8, prodBlackBox.Producers["bool1"].KnowInstances.Count);
            Assert.Equal(8, prodBlackBox.Producers["bool2"].KnowInstances.Count);
            Assert.Equal(8, prodBlackBox.Producers["and"].KnowInstances.Count);
            Assert.Equal(8, prodBlackBox.Producers["or"].KnowInstances.Count);
            Assert.Equal(8, prodBlackBox.Producers["xor"].KnowInstances.Count);
            //2 values (0.0, 5.0) aka (off, on)
            //1 switch from off to on
            //1 switch to on to off
            //1 holding on
            //1 holding off

            //Check that each ID in the discretizer exists in the producer
            List<string> ioNames = logicBlackBox.Input.Select(p => p.Key).ToList();
            foreach (string ioName in ioNames)
            {
                //Get discretizer and producer
                var disc = discBlackBox.Discretizers[ioName];
                var prod = prodBlackBox.Producers[ioName];

                //Check for each bin id in producer
                foreach (Bin bin in disc.Bins)
                {
                    KnowInstance ki = prod.Get(bin.BinID);
                    Assert.NotNull(ki);
                }
            }
            

        }
    }
}
