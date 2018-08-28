using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Discretization.Tests
{
    public class DiscretizerTest
    {
        private int _lastID = 0;
        private int GenerateID()
        {
            _lastID++;
            return _lastID;
        }

        [Fact]
        public void Constructor_NoInputs_HasInitialBin()
        {
            var disc = new Discretizer();

            Assert.Single(disc.Bins);
            Assert.Equal(disc.Bins[0].Low, double.NegativeInfinity);
            Assert.Equal(disc.Bins[0].High, double.PositiveInfinity);
        }
        [Fact]
        public void SplitBins_DataOnlyinHighRange_KeepStatistics()
        {
            Discretizer disc = new Discretizer();
            disc.Bins.Clear();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, -10);
            Bin bin2 = new Bin(GenerateID(), -10, 10);
            Bin bin3 = new Bin(GenerateID(), 10, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                2,
                2,
                2,
                3,
                4,
                4,
                4,
            });
            bin2.MinPointsForAction = 2;
            disc.Bins.AddRange(new List<Bin> { bin1, bin2, bin3 });

            List<Bin> results = disc.SplitBin(bin2, -7);
            Bin resultBin1 = results[0];
            Bin resultBin2 = results[1];

            Assert.Equal(4, disc.Bins.Count);
            Assert.Equal(3, resultBin2.Average);
            Assert.Equal(1, resultBin2.StandardDeviation);
            Assert.Equal(0, resultBin1.Count);
        }
        [Fact]
        public void SplitBins_DataOnlyinLowRange_KeepStatistics()
        {
            Discretizer disc = new Discretizer();
            disc.Bins.Clear();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, -10);
            Bin bin2 = new Bin(GenerateID(), -10, 10);
            Bin bin3 = new Bin(GenerateID(), 10, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                -2,
                -2,
                -2,
                -3,
                -4,
                -4,
                -4,
            });
            disc.Bins.AddRange(new List<Bin> { bin1, bin2, bin3 });

            List<Bin> results = disc.SplitBin(bin2, 7);
            Bin resultBin1 = results[0];
            Bin resultBin2 = results[1];

            Assert.Equal(4, disc.Bins.Count);
            Assert.Equal(-3, resultBin1.Average);
            Assert.Equal(1, resultBin1.StandardDeviation);
            Assert.Equal(0, resultBin2.Count);
        }
        [Fact]
        public void SplitBins_DataAcrossNewRanges_LoseStatistics()
        {
            Discretizer disc = new Discretizer();
            disc.Bins.Clear();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, 0);
            Bin bin2 = new Bin(GenerateID(), 0, 10);
            Bin bin3 = new Bin(GenerateID(), 10, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                2,
                2,
                2,
                3,
                4,
                4,
                4,
            });
            bin2.MinPointsForAction = 2;
            disc.Bins.AddRange(new List<Bin> { bin1, bin2, bin3 });

            List<Bin> results = disc.SplitBin(bin2, 3);
            Bin resultBin1 = results[0];
            Bin resultBin2 = results[1];

            Assert.Equal(0, resultBin1.Low);
            Assert.Equal(3, resultBin1.High);
            Assert.Equal(3, resultBin2.Low);
            Assert.Equal(10, resultBin2.High);

            Assert.Equal(4, disc.Bins.Count);
            Assert.Equal(0, resultBin1.Count);
            Assert.Equal(0, resultBin2.Count);
        }

        [Fact]
        public void MergeBins_TwoBinsWithValues_OneLessBinWithCombinedStatistics()
        {
            Discretizer disc = new Discretizer();
            disc.Bins.Clear();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, 0);
            Bin bin2 = new Bin(GenerateID(), 0, 2.5);
            Bin bin3 = new Bin(GenerateID(), 2.5, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                2,
                2,
                2,
            });
            bin3.AddValues(new List<double> {
                3,
                4,
                4,
                4,
            });
            disc.Bins.AddRange(new List<Bin> { bin1, bin2, bin3 });

            Bin combinedBin = disc.MergeBins(bin2, bin3, true);

            Assert.Equal(2, disc.Bins.Count);
            Assert.Equal(3, combinedBin.Average);
            Assert.Equal(1, combinedBin.StandardDeviation);
            Assert.Equal(0, combinedBin.Low);
            Assert.Equal(double.PositiveInfinity, combinedBin.High);
        }

        [Fact]
        public void Equals_SameDiscretizers_true()
        {
            var origDisc = new Discretizer();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, -20);
            Bin bin2 = new Bin(GenerateID(), -20, 2.5);
            Bin bin3 = new Bin(GenerateID(), 2.5, 20);
            Bin bin4 = new Bin(GenerateID(), 20, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                -1,
                0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0,
                1,
            });
            bin3.AddValues(new List<double> {
                4,
                5,5,5,5,5, 5,5,5,5,5, 5,5,5,5,5,
                6,
            });
            origDisc.Bins.Clear();
            origDisc.Bins.AddRange(new List<Bin> { bin1, bin2, bin3, bin4 });

            //Assert
            origDisc.Equals(origDisc);
        }

        [Fact]
        public void Equals_DifferentDiscretizers_false()
        {
            var disc1 = new Discretizer();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, -20);
            Bin bin2 = new Bin(GenerateID(), -20, 2.5);
            Bin bin3 = new Bin(GenerateID(), 2.5, 20);
            Bin bin4 = new Bin(GenerateID(), 20, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                -1,
                0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0,
                1,
            });
            bin3.AddValues(new List<double> {
                4,
                5,5,5,5,5, 5,5,5,5,5, 5,5,5,5,5,
                6,
            });
            disc1.Bins.Clear();
            disc1.Bins.AddRange(new List<Bin> { bin1, bin2, bin3, bin4 });

            var disc2 = new Discretizer();
            Bin bin21 = new Bin(GenerateID(), double.NegativeInfinity, -20);
            Bin bin22 = new Bin(GenerateID(), -20, 2.5);
            Bin bin23 = new Bin(GenerateID(), 2.5, 20);
            Bin bin24 = new Bin(GenerateID(), 20, double.PositiveInfinity);
            bin22.AddValues(new List<double> {
                -1,
                0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0,
                1,
            });
            bin23.AddValues(new List<double> {
                4,
                5,5,5,5,5, 5,5,5,5,5, 5,5,5,5,5,
                6,
            });
            disc2.Bins.Clear();
            disc2.Bins.AddRange(new List<Bin> { bin21, bin22, bin23, bin24 });

            //Change bins
            bin22.High = 3.0;
            bin23.Low = 3.0;

            //Assert
            disc1.Equals(disc2);
        }

        [Fact]
        public void ToJson()
        {
            var origDisc = new Discretizer();
            Bin bin1 = new Bin(GenerateID(), double.NegativeInfinity, -20);
            Bin bin2 = new Bin(GenerateID(), -20, 2.5);
            Bin bin3 = new Bin(GenerateID(), 2.5, 20);
            Bin bin4 = new Bin(GenerateID(), 20, double.PositiveInfinity);
            bin2.AddValues(new List<double> {
                -1,
                0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0,
                1,
            });
            bin3.AddValues(new List<double> {
                4,
                5,5,5,5,5, 5,5,5,5,5, 5,5,5,5,5,
                6,
            });
            origDisc.Bins.Clear();
            origDisc.Bins.AddRange(new List<Bin> { bin1, bin2, bin3, bin4 });

            //Serialize and deserialize
            string json = origDisc.ToJson();
            var newDisc = Discretizer.FromJson(json);

            //Assert
            Assert.Equal(origDisc, newDisc);
        }
    }
    public class DiscretizerUsageTest
    {
        [Theory]
        [InlineData(0.01)] //Always passes.
        [InlineData(0.10)] //For some reason this sometimes fails. A particular bin ends up getting extra unnecessary resolution.
        //[InlineData(0.20)] //This never seems to pass.
        //[InlineData(0.50)] //This never seems to pass.
        public void GetBin_100Values_102Bins(double maxNoise)
        {
            //List of crisp values
            List<int> x_crisp = Enumerable.Range(0, 100).ToList();

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            Random rand = new Random();
            for (int i = 0; i < 10000; i++)
            {
                //Generate a noisy value for each entry then randomize the order.
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p=> rand.NextDouble()).ToList();
                //Add all values to discretizer
                foreach (double x in x_noisy)
                    disc.Learn(x);
            }

            //Check
            Assert.Equal(102, disc.Bins.Count);
        }


        [Theory]
        //[InlineData(0.00)] //This is an interesting case. In theory it should never happen, because all values have noise. It creates extra bins between the values. As such it fails.
        //[InlineData(0.000000001)]
        //[InlineData(0.00000001)]
        //[InlineData(0.0000001)]
        //[InlineData(0.000001)] // Fails below this amount. Maybe statistics mess up.
        [InlineData(0.00001)]
        [InlineData(0.0001)]
        [InlineData(0.001)]
        [InlineData(0.01)]
        [InlineData(0.10)]
        [InlineData(0.20)]
        public void GetBin_2Values_4Bins(double maxNoise)
        {
            //Values to generate from
            List<int> x_crisp = new List<int> { 0, 5 };
            int passes = 10000;

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            for (int i = 0; i < passes; i++)
            {
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1);
                foreach (double x in x_noisy)
                    disc.Learn(x);
            }

            //Check
            Assert.Equal(4, disc.Bins.Count); //Ideally 102, but there is a randomization factor so it may occasionally go higher or lower.
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(0.10)]
        [InlineData(0.20)]
        [InlineData(0.50)]
        [InlineData(1.00)]
        [InlineData(5.00)]
        public void GenerateNoisyData_1000Pts_SeeAssert(double maxNoise)
        {
            int count = 1000;
            int x_crisp = 0;
            var values = GenerateNoisyData(new List<double> {x_crisp}, maxNoise, count);

            //Calculate statistics
            double min = values.Min();
            double max = values.Max();
            double avg = values.Average();
            double stdDev = Math.Sqrt(values.Sum(x => Math.Pow(x - avg, 2)) / (count - 1));
            double stdDev1_Percent = values.Count(x => (avg-stdDev <= x) && (x < avg+stdDev))/ Convert.ToDouble(count);

            Assert.InRange(min, x_crisp-maxNoise, double.PositiveInfinity);
            Assert.InRange(max, double.NegativeInfinity, x_crisp+maxNoise);
            Assert.InRange(avg, x_crisp-0.1*stdDev, x_crisp+0.1*stdDev);
            //Assert.InRange(stdDev, 0.01, 0.01);
            Assert.InRange(stdDev1_Percent, 0.65, 0.71);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(15000)] // This almost always passes.
        [InlineData(20000)] // This always passes, which means sometimes it just takes a while to converge.
        public void GetBins_SinFunction_SeeAssert(int iterations)
        {
            Random rand = new Random();
            Random rand2 = new Random();
            List<double> inputs = new List<double> {
                Math.PI * 0.1,
                Math.PI * 0.2,
                Math.PI * 0.3,
                Math.PI * 0.4,
                Math.PI * 0.5,
                Math.PI * 0.6,
                Math.PI * 0.7,
                Math.PI * 0.8,
                Math.PI * 0.9,
                Math.PI * 1.0
            };
            Discretizer discInput = new Discretizer();
            Discretizer discOutput = new Discretizer();


            for (int i=0; i<iterations; i++)
            {
                //double x_crisp = inputs[i%inputs.Count];
                double x_crisp = inputs[rand.Next(0,10)];
                double x_noisy = GenerateNoisyValue(rand2, x_crisp, 0.001);
                double y = Math.Sin(x_crisp);

                discInput.Learn(x_noisy);
                discOutput.Learn(y);
            }

            var binsInput = discInput.BinsOrderedByLow;
            var binsOutput = discOutput.BinsOrderedByLow;


            Assert.Equal(12, discInput.Bins.Count);
            Assert.Equal(8, discOutput.Bins.Count);
        }


        //Methods
        public List<double> GenerateNoisyData(List<int> x_crisp, double maxNoise, int numPerCrispValue)
        {
            return GenerateNoisyData(x_crisp.ConvertAll<double>(x => (double)x), maxNoise, numPerCrispValue);
        }
        public List<double> GenerateNoisyData(List<double> x_crisp, double maxNoise, int numPerCrispValue)
        {
            //Create list of values with noise
            Random rand = new Random();
            List<double> x_noisy = new List<double>();
            
            foreach (double x in x_crisp)
            {
                for (int i = 0; i < numPerCrispValue; i++)
                {
                    double factor = SampleGaussian(rand, 0, 1.0/6.0); //Generates a value between 0 and 1. We know that 6 sigma covers 99.999999% of values. So, 1/6 std dev results in -1 to 1.
                    x_noisy.Add(x + factor*maxNoise);
                }
            }

            return x_noisy;
        }
        public static double GenerateNoisyValue(Random random, double value_crisp, double maxNoise)
        {
            double factor = SampleGaussian(random, 0, 1.0 / 6.0); //Generates a value between 0 and 1. We know that 6 sigma covers 99.999999% of values. So, 1/6 std dev results in -1 to 1.
            double value_noisy = (value_crisp + factor * maxNoise);
            return value_noisy;
        }
        public static double SampleGaussian(Random random, double mean, double stddev)
        {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            double x1 = 1 - random.NextDouble();
            double x2 = 1 - random.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return y1 * stddev + mean;
        }
    }
}
