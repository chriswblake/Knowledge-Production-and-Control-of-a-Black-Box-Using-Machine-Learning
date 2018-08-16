using System;
using Xunit;
using KnowledgeProduction;
using System.Collections.Generic;

namespace KnowledgeProduction.Tests
{
    public class KnowInstanceSymbolTests
    {
        [Fact]
        public void GetTheoreticalHashCode_SameItems_SameHashCode()
        {
            KnowInstanceValue c1 = new KnowInstanceValue(-5.0);
            KnowInstanceValue c2 = new KnowInstanceValue(5.0);

            KnowInstanceSymbol c3 = new KnowInstanceSymbol(c1, c2);
            int result = KnowInstanceSymbol.GetTheoreticalHashCode(c1, c2);

            Assert.Equal(c3.GetHashCode(), result);

        }
    }
}
