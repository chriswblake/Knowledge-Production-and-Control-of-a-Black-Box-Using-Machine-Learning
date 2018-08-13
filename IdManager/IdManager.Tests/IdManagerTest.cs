using System;
using Xunit;
using IdManager;

namespace IdManager.Tests
{
    public class IdManagerTest
    {
        [Fact]
        public void GetNewId_2TwoRequests_SecondIsGreater()
        {
            var id1 = IdManager.GetNewId();
            var id2 = IdManager.GetNewId();

            var result = id2 > id1;

            Assert.True(result);
        }
    }
}
