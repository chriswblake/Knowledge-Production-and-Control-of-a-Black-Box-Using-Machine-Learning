using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Discretization.Tests
{
    public class DiscretizerTest
    {
        [Fact]
        public void SplitBins_DataOnlyinHighRange_KeepStatistics()
        {
            Discretizer disc = new Discretizer();
            disc.Bins.Clear();
            Bin bin1 = new Bin(double.NegativeInfinity,-10, 3);
            Bin bin2 = new Bin(-10, 10, 3);
            Bin bin3 = new Bin(10, double.PositiveInfinity, 3);
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
            Bin bin1 = new Bin(double.NegativeInfinity, -10, 3);
            Bin bin2 = new Bin(-10, 10, 3);
            Bin bin3 = new Bin(10, double.PositiveInfinity, 3);
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

            List<Bin> results = disc.SplitBin(bin2, 7);
            Bin resultBin1 = results[0];
            Bin resultBin2 = results[1];

            Assert.Equal(4, disc.Bins.Count);
            Assert.Equal(3, resultBin1.Average);
            Assert.Equal(1, resultBin1.StandardDeviation);
            Assert.Equal(0, resultBin2.Count);
        }
        [Fact]
        public void SplitBins_DataAcrossNewRanges_LoseStatistics()
        {
            Discretizer disc = new Discretizer();
            disc.Bins.Clear();
            Bin bin1 = new Bin(double.NegativeInfinity, 0, 3);
            Bin bin2 = new Bin(0, 10, 3);
            Bin bin3 = new Bin(10, double.PositiveInfinity, 3);
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
            Bin bin1 = new Bin(double.NegativeInfinity, 0, 3);
            Bin bin2 = new Bin(0, 2.5, 3);          
            Bin bin3 = new Bin(2.5, double.PositiveInfinity, 3);
            bin2.AddValues(new List<double> {
                2,
                2,
                2,
            });
            bin2.MinPointsForAction = 2;
            bin3.AddValues(new List<double> {
                3,
                4,
                4,
                4,
            });
            bin3.MinPointsForAction = 3;
            disc.Bins.AddRange(new List<Bin> {bin1, bin2, bin3});

            Bin combinedBin = disc.MergeBins(bin2, bin3);

            Assert.Equal(2, disc.Bins.Count);
            Assert.Equal(3, combinedBin.Average);
            Assert.Equal(1, combinedBin.StandardDeviation);
            Assert.Equal(0, combinedBin.Low);
            Assert.Equal(double.PositiveInfinity, combinedBin.High);
            Assert.Equal(3, combinedBin.MinPointsForAction);

        }
        [Fact]
        public void GetBin_100RepeatedNoisyValues_102Bins()
        {
            //Values to generate from
            var x_source = Enumerable.Range(0, 100).ToArray();

            //Create list of values with noise
            Random rand = new Random();
            double noise = 0.01;
            List<double> x_noisy = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                foreach (int x in x_source)
                {
                    //Generate random number with a flat distribution
                    //double relNoise = 2 * noise * rand.NextDouble();
                    //x_noisy.Add((x - noise) + relNoise);

                    //Generate random number with rough gaussian distribution
                    double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
                    double u2 = 1.0 - rand.NextDouble();
                    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

                    //Pick Sign
                    double sign = 1;
                    if (rand.NextDouble() < 0.5)
                        sign = -1;
                    //Create Point
                    x_noisy.Add(x + sign * randStdNormal * noise);
                }
            }
            

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            for(int i =0; i<5; i++)
                foreach (double x in x_noisy)
                    disc.GetBin(x);

            //Check
            Assert.InRange(disc.Bins.Count,100,105); //Ideally 102, but there is a randomization factor so it may occasionally go higher or lower.
        }
    }
}
