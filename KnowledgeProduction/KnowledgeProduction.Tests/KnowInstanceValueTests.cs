using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeProduction.Tests
{
    public class KnowInstanceValueTests
    {
        private int _lastID = 0;
        private int GenerateID()
        {
            _lastID++;
            return _lastID;
        }

        [Fact]
        public void GetHashCode_5_5()
        {
            var kiv = new KnowInstanceValue(5, "some content");

            var result = kiv.GetHashCode();

            Assert.Equal(5, result);
        }

        //This test is no longer correct, becaue the hashcode is always based on the IDs.
        //[Theory]
        //[InlineData(1)]
        //[InlineData(1.01)]
        //[InlineData("text data")]
        //public void GetHashcode_DifferentContentTypes(object value)
        //{
        //    var kiv = new KnowInstanceValue(GenerateID(), value);

        //    var result = kiv.GetHashCode();

        //    Assert.Equal(value.GetHashCode(), kiv.GetHashCode());
        //}
    }
}
