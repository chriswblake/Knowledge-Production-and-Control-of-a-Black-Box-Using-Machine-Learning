using System;
using System.Collections.Generic;
using Xunit;


namespace Discretization.Tests
{
    public class BinTest
    {   //MethodName_StateUnderTest_ExpectedBehavior

        private int _lastID = 0;
        private int GenerateID()
        {
            _lastID++;
            return _lastID;
        }

        #region Constructors
        [Fact]
        public void Bin_EmptyConstructorDefault_LowNegInfHighPosInf()
        {
            Bin theBin = new Bin(GenerateID());

            Assert.Equal(double.NegativeInfinity, theBin.Low);
            Assert.Equal(double.PositiveInfinity, theBin.High);
        }
        [Fact]
        public void Bin_ConstructorWithValues_Low0High10()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);

            Assert.Equal(0, theBin.Low);
            Assert.Equal(10, theBin.High);
        }
        [Fact]
        public void ToJson_JsonBin_SameBin()
        {
            var origBin = new Bin(GenerateID(), 0, 10);
            for (int i = 0; i < 1000; i++)
                origBin.AddValues(new List<double> {
                    1,1,1,1,
                    2,2,2,2,
                    3,3,3,3,
                    4,4,4,4,
                    5,5,5,5,
                    6,6,6,6,
                    7,7,7,7,
                    8,8,8,8,
                    9,9,9,9,
                });

            //Serialize then deserialize bin
            string json = origBin.ToJson();
            var newBin = Bin.FromJson(json);

            //Check
            Assert.Equal(origBin.BinID, newBin.BinID);
            Assert.Equal(origBin.Low, newBin.Low);
            Assert.Equal(origBin.High, newBin.High);
            Assert.Equal(origBin.MinPointsForAction, newBin.MinPointsForAction);
            
            Assert.Equal(origBin.Count, newBin.Count);
            Assert.Equal(origBin.Sum, newBin.Sum);
            Assert.Equal(origBin.SquareSum, newBin.SquareSum);
            Assert.Equal(origBin.Average, newBin.Average);
            Assert.Equal(origBin.StandardDeviation, newBin.StandardDeviation);

            Assert.Equal(origBin.Count1StdDev, newBin.Count1StdDev);
            Assert.Equal(origBin.Percent1StdDev, newBin.Percent1StdDev);

            Assert.Equal(origBin.StdDevsNeg, newBin.StdDevsNeg);
            Assert.Equal(origBin.StdDevsPos, newBin.StdDevsPos);
            Assert.Equal(origBin.StdDevsNegCount, newBin.StdDevsNegCount);
            Assert.Equal(origBin.StdDevsPosCount, newBin.StdDevsPosCount);
            Assert.Equal(origBin.StdDevsNegPercent, newBin.StdDevsNegPercent);
            Assert.Equal(origBin.StdDevsPosPercent, newBin.StdDevsPosPercent);
            Assert.Equal(origBin.StdDevsCount, newBin.StdDevsCount);
            Assert.Equal(origBin.StdDevsPercent, newBin.StdDevsPercent);

            Assert.Equal(origBin.InnerBins, newBin.InnerBins);
            Assert.Equal(origBin.InnerBinsCount, newBin.InnerBinsCount);
            Assert.Equal(origBin.InnerBinsPercent, newBin.InnerBinsPercent);

            Assert.Equal(origBin, newBin);
        }
        #endregion

        #region Statistics
        [Fact]
        public void Sum_2223444_21()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.Sum;

            Assert.Equal(21.0, result);
        }

        [Fact]
        public void SquareSum_2223444_69()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.SquareSum;

            Assert.Equal(69.0, result);
        }

        [Fact]
        public void Average_Empty_PosInf()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);

            double result = theBin.Average;

            Assert.Equal(double.PositiveInfinity, result);
        }
        [Fact]
        public void Average_2223444_3()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.Average;

            Assert.Equal(3.0, result);
        }

        [Fact]
        public void StandardDeviation_Empty_PosInf()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);

            double result = theBin.StandardDeviation;

            Assert.Equal(double.PositiveInfinity, result);
        }
        [Fact]
        public void StandardDeviation_2223444_1()
        { 
            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.StandardDeviation;

            Assert.Equal(1.0, result);
        }
        #endregion
        
        #region Distribution Inner Bins
        [Fact]
        public void InnerBinsPercent_DataNearHigh_SeeAssert()
        {
            var theBin = new Bin(GenerateID(), 0, 10);
            for(int i=0; i<100; i++)
                theBin.AddValues(new List<double>() {
                        5,
                        6,6,
                        7,7,7,7,7,7,
                        8,8,8,8,8,8,8,
                        9,9,9,9,9,9,9,9,
                    });

            Assert.InRange(theBin.InnerBinsPercent[0], 0, 0);
            Assert.InRange(theBin.InnerBinsPercent[1], 0, 0);
            Assert.InRange(theBin.InnerBinsPercent[2], 0, 0);
            Assert.InRange(theBin.InnerBinsPercent[3], 0.04, 0.05);
            Assert.InRange(theBin.InnerBinsPercent[4], 0.33, 0.34);
            Assert.InRange(theBin.InnerBinsPercent[5], 0.29, 0.30);
            Assert.InRange(theBin.InnerBinsPercent[6], 0.33, 0.34);
        }
        [Fact]
        public void InnerBinsPercent_DataNearLow_SeeAssert()
        {
            var theBin = new Bin(GenerateID(), 0, 10);
            for (int i = 0; i < 1000; i++)
                theBin.AddValues(new List<double> {
                    1,1,1,1,1,1,1,1,
                    2,2,2,2,2,2,2,
                    3,3,3,3,3,3,
                    4,4,
                    5,
                });
            
            Assert.InRange(theBin.InnerBinsPercent[0], 0.33, 0.34);
            Assert.InRange(theBin.InnerBinsPercent[1], 0.29, 0.30);
            Assert.InRange(theBin.InnerBinsPercent[2], 0.33, 0.34);
            Assert.InRange(theBin.InnerBinsPercent[3], 0.04, 0.05);
            Assert.InRange(theBin.InnerBinsPercent[4], 0, 0);
            Assert.InRange(theBin.InnerBinsPercent[5], 0, 0);
            Assert.InRange(theBin.InnerBinsPercent[6], 0, 0);
        }
        [Fact]
        public void InnerBinsPercent_UShapedDistribution_SeeAssert()
        {
            var theBin = new Bin(GenerateID(), 0, 10);
            for (int i = 0; i < 1000; i++)
                theBin.AddValues(new List<double> {
                    1,1,1,1,
                    2,2,2,
                    3,3,
                    4,
                    5,
                    6,
                    7,7,
                    8,8,8,
                    9,9,9,9,
                });
            
            Assert.InRange(theBin.InnerBinsPercent[0], 0.19, 0.20);
            Assert.InRange(theBin.InnerBinsPercent[1], 0.14, 0.15);
            Assert.InRange(theBin.InnerBinsPercent[2], 0.14, 0.15);
            Assert.InRange(theBin.InnerBinsPercent[3], 0.04, 0.05);
            Assert.InRange(theBin.InnerBinsPercent[4], 0.14, 0.15);
            Assert.InRange(theBin.InnerBinsPercent[5], 0.14, 0.15);
            Assert.InRange(theBin.InnerBinsPercent[6], 0.19, 0.20);
        }
        [Fact]
        public void InnerBinsPercent_FlatDistribution_SeeAssert()
        {
            var theBin = new Bin(GenerateID(), 0, 10);
            for (int i = 0; i < 1000; i++)
                theBin.AddValues(new List<double> {
                    1,1,1,1,
                    2,2,2,2,
                    3,3,3,3,
                    4,4,4,4,
                    5,5,5,5,
                    6,6,6,6,
                    7,7,7,7,
                    8,8,8,8,
                    9,9,9,9,
                });
            
            Assert.InRange(theBin.InnerBinsPercent[0], 0.11, 0.12);
            Assert.InRange(theBin.InnerBinsPercent[1], 0.11, 0.12);
            Assert.InRange(theBin.InnerBinsPercent[2], 0.22, 0.23);
            Assert.InRange(theBin.InnerBinsPercent[3], 0.11, 0.12);
            Assert.InRange(theBin.InnerBinsPercent[4], 0.22, 0.23);
            Assert.InRange(theBin.InnerBinsPercent[5], 0.11, 0.12);
            Assert.InRange(theBin.InnerBinsPercent[6], 0.11, 0.12);
        }
        #endregion
        
        #region Actions
        [Fact]
        public void PickAction_LessThanMinDataPoints_InsufficientData()
        {
            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.MinPointsForAction = 10;
            theBin.AddValues(new List<double> {
                3.2,
                4.0,
                5.0,
                6.0,
                6.8,
            });

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.InsufficientData, result);
        }
        [Fact]
        public void PickAction_AllValuesWithinNSigma_DoNothing()
        {
            // Test
            // Avg-nSigma > Low
            // Avg+nSigma < High

            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> {
                3,
                4,4,4,
                5,5,5,5,5, 5,5,5,5,5, 5,5,5,5,5,
                6,4,4,
                7
            });
            theBin.Count1StdDev = theBin.Count; //This is to override the check for a flat distribution. Ideally, a better dataset should be used for testing.
     
            BinAction result = theBin.PickAction();
            
            Assert.Equal(BinAction.None, result);
        }
        [Fact]
        public void PickAction_NSigmaBelowLowAndNSigmaAboveHigh_Split()
        {
            // Test
            // Avg-nSigma < Low
            // Avg+nSigma > High

            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> {
                2.0,
                3.0,
                5.0,
                7.0,
                8.0,
            });

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.SplitAtAvg, result);
        }
        [Fact]
        public void PickAction_LowIsNegInfAndPos6SigmaBelowHigh_Split()
        {
            // Test
            // Low = -Inf
            // Avg+nSigma < High

            Bin theBin = new Bin(GenerateID(), double.NegativeInfinity, 10);
            theBin.AddValues(new List<double> {
                1,
                2,2,2,
                3,3,3,3,3,
                4,4,4,
                5,
            });

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.SplitAtNegNSigma, result);
        }
        [Fact]
        public void PickAction_Neg6SigmaAboveLowAndHighEqualsPosInf_Split()
        {
            // Test
            // Avg-nSigma > Low
            // High = +Inf

            Bin theBin = new Bin(GenerateID(), 0, double.PositiveInfinity);
            theBin.AddValues(new List<double> {
                5,
                6,6,6,
                7,7,7,7,7,
                8,8,8,
                9,
            });

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.SplitAtPosNSigma, result);
        }
        [Fact]
        public void PickAction_Neg6SigmaAboveLowAndPos6SigmaAboveHigh_MergeHigh()
        {
            // Test
            // Avg-nSigma > Low
            // Avg+nSigma > High

            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> {
                5,
                6,6,6,
                7,7,7,7,7,
                8,8,8,
                9,
            });
            theBin.Count1StdDev = theBin.Count; //This is to override the check for a flat distribution. Ideally, a better dataset should be used for testing.
            
            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.MergeHigh, result);
        }
        [Fact]
        public void PickAction_Neg6SigmaBelowLowAndPos6SigmaBelowHigh_MergeLow()
        {
            // Test
            // Avg-nSigma < Low
            // Avg+nSigma < High

            Bin theBin = new Bin(GenerateID(), 0, 10);
            theBin.AddValues(new List<double> {
                1,
                2,2,2,
                3,3,3,3,3,
                4,4,4,
                5,
            });
            theBin.Count1StdDev = theBin.Count; //This is to override the check for a flat distribution. Ideally, a better dataset should be used for testing.

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.MergeLow, result);
        }
        #endregion
    }
}
