using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using IdManagement;
using KnowledgeProduction;
using KnowProdContBlackBox;
using RLDT;

namespace KnowProdContBlackBox.Tests
{
    public class KnowInstanceWithMetaDataTests
    {
        [Fact]
        public void Constructors_AreEqual_SameHashCodes()
        {
            IdManager idManager = new IdManager();
            var ki = new KnowInstanceValue(5, 5.0);
            var kim1 = new KnowInstanceWithMetaData(ki, idManager);
            idManager.SetName(5, "On");
            var kim2 = new KnowInstanceWithMetaData(ki, idManager);

            var fvp1 = new FeatureValuePair("input1", ki);
            var fvpm1 = new FeatureValuePair("input1", kim1);
            var fvpm2 = new FeatureValuePair("input1", kim2);

            var result1 = fvp1.GetHashCode();
            var result2 = fvpm1.GetHashCode();
            var result3 = fvpm2.GetHashCode();

            Assert.Equal(fvp1.GetHashCode(), fvpm1.GetHashCode());
            Assert.Equal(fvpm1, fvpm2);
        }
    }
}
