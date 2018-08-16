using System;
using Xunit;
using KnowledgeProduction;
using System.Collections.Generic;

namespace KnowledgeProduction.Tests
{
    public class KnowInstanceSymbolTests
    {
        private int _lastID = 0;
        private int GenerateID()
        {
            _lastID++;
            return _lastID;
        }

        [Fact]
        public void GetTheoreticalHashCode_SameItems_SameHashCode()
        {
            KnowInstanceValue c1 = new KnowInstanceValue(GenerateID(), -5.0);
            KnowInstanceValue c2 = new KnowInstanceValue(GenerateID(), 5.0);

            KnowInstanceSymbol c3 = new KnowInstanceSymbol(GenerateID(), c1, c2);
            int result = KnowInstanceSymbol.GetTheoreticalHashCode(c1, c2);

            Assert.Equal(c3.GetHashCode(), result);

        }
    }
}
