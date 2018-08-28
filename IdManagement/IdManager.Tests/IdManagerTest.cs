using System;
using Xunit;
using IdManagement;

namespace IdManagement.Tests
{
    public class IdManagerTest
    {
        [Fact]
        public void GetNewId_2TwoRequests_SecondIsGreater()
        {
            var idManager = new IdManager();
            var id1 = idManager.GenerateId();
            var id2 = idManager.GenerateId();

            var result = id2 > id1;

            Assert.True(result);
        }
    }
}
