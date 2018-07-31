using System;
using System.Collections.Generic;
using Xunit;


namespace Discretization.Tests
{
    public class BinTest
    {   //MethodName_StateUnderTest_ExpectedBehavior

        [Fact]
        public void Bin_EmptyConstructorDefault_LowNegInfHighPosInf()
        {
            Bin theBin = new Bin();

            Assert.Equal(double.NegativeInfinity, theBin.Low);
            Assert.Equal(double.PositiveInfinity, theBin.High);
        }
        [Fact]
        public void Bin_ConstructorWithValues_Low0High10()
        {
            Bin theBin = new Bin(0, 10);

            Assert.Equal(0, theBin.Low);
            Assert.Equal(10, theBin.High);
        }

        [Fact]
        public void Sum_NonEmptyListOfValues_TwentyTwo()
        {
            Bin theBin = new Bin();
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.Sum;

            Assert.Equal(21.0, result);
        }

        [Fact]
        public void SquareSum_NonEmptyListOfValues_SixtyNine()
        {
            Bin theBin = new Bin();
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.SquareSum;

            Assert.Equal(69.0, result);
        }

        [Fact]
        public void Average_NoValues_PosInfinity()
        {
            Bin theBin = new Bin();

            double result = theBin.Average;

            Assert.Equal(double.PositiveInfinity, result);
        }
        [Fact]
        public void Average_NonEmptyListOfValues_One()
        {
            Bin theBin = new Bin();
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.Average;

            Assert.Equal(3.0, result);
        }

        [Fact]
        public void StandardDeviation_NoValues_Zero()
        {
            Bin theBin = new Bin();

            double result = theBin.StandardDeviation;

            Assert.Equal(double.PositiveInfinity, result);
        }
        [Fact]
        public void StandardDeviation_NonEmptyListOfValues_One()
        { 
            Bin theBin = new Bin();
            theBin.AddValues(new List<double> { 2, 2, 2, 3, 4, 4, 4 });

            double result = theBin.StandardDeviation;

            Assert.Equal(1.0, result);
        }

        [Fact]
        public void PickAction_LessThanMinDataPoints_InsufficientData()
        {
            Bin theBin = new Bin(0, 10, 3);
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

            Bin theBin = new Bin(0, 10, 3);
            theBin.MinPointsForAction = 3;
            theBin.AddValues(new List<double> {
                3.2,
                4.0,
                5.0,
                6.0,
                6.8,
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

            Bin theBin = new Bin(0, 10, 3);
            theBin.MinPointsForAction = 3;
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
        public void PickAction_LowEqualsNegInfAndNSigmaBelowHigh_Split()
        {
            // Test
            // Low = -Inf
            // Avg+nSigma < High

            Bin theBin = new Bin(double.NegativeInfinity, 10, 3);
            theBin.MinPointsForAction = 3;
            theBin.AddValues(new List<double> {
                1.0,
                2.0,
                3.0,
                4.0,
                5.0,
            });

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.SplitAtNegNSigma, result);
        }
        [Fact]
        public void PickAction_NSigmaAboveLowAndHighEqualsPosInf_Split()
        {
            // Test
            // Avg-nSigma > Low
            // High = +Inf

            Bin theBin = new Bin(0, double.PositiveInfinity, 3);
            theBin.MinPointsForAction = 3;
            theBin.AddValues(new List<double> {
                5.0,
                6.0,
                7.0,
                8.0,
                9.0,
            });

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.SplitAtPosNSigma, result);
        }
        //[Fact]
        //public void PickAction_LowEqualsNegInfAndHighEqualsPosInf_Split()
        //{
        //    // Test
        //    // Low = -Inf
        //    // High = +Inf

        //    Bin theBin = new Bin(double.NegativeInfinity, double.PositiveInfinity, 3);
        //    theBin.MinPointsForAction = 3;
        //    theBin.AddValues(new List<double> {
        //        2.0,
        //        3.0,
        //        5.0,
        //        7.0,
        //        8.0,
        //    });

        //    BinAction result = theBin.PickAction();

        //    Assert.Equal(BinAction.Split, result);
        //}
        [Fact]
        public void PickAction_NSigmaAboveLowAndNSigmaAboveHigh_MergeHigh()
        {
            // Test
            // Avg-nSigma > Low
            // Avg+nSigma > High

            Bin theBin = new Bin(0, 10, 3);
            theBin.MinPointsForAction = 3;
            theBin.AddValues(new List<double> {
                5.0,
                6.0,
                7.0,
                8.0,
                9.0,
            });
            theBin.Count1StdDev = theBin.Count; //This is to override the check for a flat distribution. Ideally, a better dataset should be used for testing.
            
            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.MergeHigh, result);
        }
        [Fact]
        public void PickAction_NSigmaBelowLowAndNSigmaBelowHigh_MergeLow()
        {
            // Test
            // Avg-nSigma < Low
            // Avg+nSigma < High

            Bin theBin = new Bin(0, 10, 3);
            theBin.MinPointsForAction = 3;
            theBin.AddValues(new List<double> {
                1.0,
                2.0,
                3.0,
                4.0,
                5.0,
            });
            theBin.Count1StdDev = theBin.Count; //This is to override the check for a flat distribution. Ideally, a better dataset should be used for testing.

            BinAction result = theBin.PickAction();

            Assert.Equal(BinAction.MergeLow, result);
        }
    }
}
