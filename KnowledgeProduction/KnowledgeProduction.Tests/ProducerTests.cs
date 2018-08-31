using System;
using Xunit;
using KnowledgeProduction;
using System.Collections.Generic;
using System.Reflection;

namespace KnowledgeProduction.Tests
{
    //Make the protected methods public for testing.
    public class Producer : KnowledgeProduction.Producer
    {
        new public void SaveInstance(KnowInstance theInstance)
        {
            base.SaveInstance(theInstance);
        }
        new public void SaveSequence(KnowInstance theInstance)
        {
            base.SaveSequence(theInstance);
        }
    }

    public class ProducerTests
    {
        private int _lastID = 0;
        private int GenerateId()
        {
            _lastID++;
            return _lastID;
        }

        #region Usage
        [Fact]
        public void Get_known_notNull()
        {
            Producer prod = new Producer() { GenerateIdDelegate = GenerateId };
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);
            KnowInstanceSymbol c6 = new KnowInstanceSymbol(GenerateId(), new List<KnowInstance> { c1, c2 });
            prod.SaveInstance(c1);
            prod.SaveInstance(c2);
            prod.SaveInstance(c3);
            prod.SaveInstance(c4);
            prod.SaveInstance(c5);
            prod.SaveInstance(c6);

            var result = prod.Get(c1, c2);

            Assert.Equal(c6, result);
        }
        [Fact]
        public void Get_unknown_null()
        {
            Producer prod = new Producer() { GenerateIdDelegate = GenerateId };
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);
            KnowInstanceSymbol c6 = new KnowInstanceSymbol(GenerateId(), new List<KnowInstance> { c1, c2 });
            prod.SaveInstance(c1);
            prod.SaveInstance(c2);
            prod.SaveInstance(c3);
            prod.SaveInstance(c4);
            prod.SaveInstance(c5);
            prod.SaveInstance(c6);

            var result = prod.Get(c1, c3);

            Assert.Equal(null, result);
        }
        #endregion

        #region Learning
        [Fact]
        public void Learn_LinearRampPattern_Count15()
        {
            Producer prod = new Producer();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);
            List<KnowInstance> inputStream = new List<KnowInstance> {
                c1,
                   c2,
                      c3,
                         c4,
                            c5,
                            c5,
                            c5,
                         c4,
                      c3,
                   c2,
                c1,

            };

            foreach (KnowInstance ki in inputStream)
                prod.Learn(ki);

            Assert.Equal(14, prod.KnowInstances.Count);
            //5 values
            //4 sequences for ramp-up.
            //1 for holding
            //4 sequences for ramp-down.
        }
        [Fact]
        public void Learn_OnOffPattern_Count4()
        {
            Producer prod = new Producer();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), (0));
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), (1));
            List<KnowInstance> inputData = new List<KnowInstance> {
                c1,
                c1,
                c1,
                    c2,
                    c2,
                    c2,
                    c2,
                    c2,
                c1,
                c1,
                c1,
                c1,
                    c2,
                    c2,
                    c2,
            };

            foreach (var k in inputData)
                prod.Learn(k);

            Assert.Equal(6, prod.KnowInstances.Count);
            //2 values
            //1 switch from off to on
            //1 switch to on to off
            //1 holding on
            //1 holding off

        }

        [Fact]
        public void SaveInstance_5UniqueItems_Count5()
        {
            Producer prod = new Producer() { GenerateIdDelegate = GenerateId };
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);

            prod.SaveInstance(c1);
            prod.SaveInstance(c2);
            prod.SaveInstance(c3);
            prod.SaveInstance(c4);
            prod.SaveInstance(c5);

            Assert.Equal(5, prod.KnowInstances.Count);
        }
        [Fact]
        public void SaveInstance_5RepeatedItems_Count5()
        {
            Producer prod = new Producer();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);

            prod.SaveInstance(c1);
            prod.SaveInstance(c2);
            prod.SaveInstance(c3);
            prod.SaveInstance(c4);
            prod.SaveInstance(c5);

            prod.SaveInstance(c1);
            prod.SaveInstance(c2);
            prod.SaveInstance(c3);
            prod.SaveInstance(c4);
            prod.SaveInstance(c5);

            Assert.Equal(5, prod.KnowInstances.Count);
        }

        [Fact]
        public void SaveSequence_Pattern_Count9()
        {
            Producer prod = new Producer();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);
            List<KnowInstance> inputStream = new List<KnowInstance> {
                c1,
                   c2,
                      c3,
                         c4,
                            c5,
                            c5,
                            c5,
                         c4,
                      c3,
                   c2,
                c1,

            };

            foreach (KnowInstance ki in inputStream)
                prod.SaveSequence(ki);

            Assert.Equal(9, prod.KnowInstances.Count);

        }
        #endregion

        #region Updating
        [Fact]
        public void Add()
        {
            Producer prod = new Producer() { GenerateIdDelegate = GenerateId };
            prod.Add(15, 5.0);

            KnowInstance result = prod.Get(15);

            Assert.NotNull(result);
        }

        [Fact]
        public void Remove()
        {
            Producer prod = new Producer() { GenerateIdDelegate = GenerateId };
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateId(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateId(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateId(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateId(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateId(), 5);
            KnowInstanceSymbol c6 = new KnowInstanceSymbol(GenerateId(), new List<KnowInstance> { c1, c2 });
            prod.SaveInstance(c1);
            prod.SaveInstance(c2);
            prod.SaveInstance(c3);
            prod.SaveInstance(c4);
            prod.SaveInstance(c5);
            prod.SaveInstance(c6);

            prod.Remove(c1.ID);
            KnowInstance result = prod.Get(c1.ID);

            Assert.Null(result);
        }
        #endregion
    }
}
