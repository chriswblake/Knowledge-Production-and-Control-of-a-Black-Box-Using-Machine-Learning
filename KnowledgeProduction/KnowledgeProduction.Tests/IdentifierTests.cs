using System;
using Xunit;
using KnowledgeProduction;
using System.Collections.Generic;

namespace KnowledgeProduction.Tests
{
    public class IdentifierTests
    {
        private int _lastID = 0;
        private int GenerateID()
        {
            _lastID++;
            return _lastID;
        }

        [Fact]
        public void saveInstance_5UniqueItems_Count5()
        {
            Identifier identifier = new Identifier() { GenerateIdDelegate = GenerateID };
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateID(), 1); 
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateID(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateID(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateID(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateID(), 5);

            identifier.SaveInstance(c1);
            identifier.SaveInstance(c2);
            identifier.SaveInstance(c3);
            identifier.SaveInstance(c4);
            identifier.SaveInstance(c5);

            Assert.Equal(5, identifier.KnowInstances.Count);
        }

        [Fact]
        public void SaveInstance_5RepeatedItems_Count5()
        {
            Identifier identifier = new Identifier();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateID(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateID(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateID(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateID(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateID(), 5);

            identifier.SaveInstance(c1);
            identifier.SaveInstance(c2);
            identifier.SaveInstance(c3);
            identifier.SaveInstance(c4);
            identifier.SaveInstance(c5);

            identifier.SaveInstance(c1);
            identifier.SaveInstance(c2);
            identifier.SaveInstance(c3);
            identifier.SaveInstance(c4);
            identifier.SaveInstance(c5);

            Assert.Equal(5, identifier.KnowInstances.Count);
        }

        [Fact]
        public void SaveSequence_Pattern_Count9()
        {
            Identifier identifier = new Identifier();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateID(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateID(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateID(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateID(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateID(), 5);
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

            foreach(KnowInstance ki in inputStream)
                identifier.SaveSequence(ki);

            Assert.Equal(9, identifier.KnowInstances.Count);

        }

        [Fact]
        public void Learn_LinearRampPattern_Count15()
        {
            Identifier identifier = new Identifier();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateID(), 1);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateID(), 2);
            KnowInstanceValue c3 = new KnowInstanceValue(GenerateID(), 3);
            KnowInstanceValue c4 = new KnowInstanceValue(GenerateID(), 4);
            KnowInstanceValue c5 = new KnowInstanceValue(GenerateID(), 5);
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
                identifier.Learn(ki);

            Assert.Equal(14, identifier.KnowInstances.Count);
            //5 values
            //4 sequences for ramp-up.
            //1 for holding
            //4 sequences for ramp-down.
        }

        [Fact]
        public void Learn_OnOffPattern_Count4()
        {
            Identifier identifier = new Identifier();
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateID(), (0));
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateID(), (1));
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
                identifier.Learn(k);

            Assert.Equal(6, identifier.KnowInstances.Count);
            //2 values
            //1 switch from off to on
            //1 switch to on to off
            //1 holding on
            //1 holding off

        }
    }
}
