using System;
using System.Collections.Generic;
using System.Text;

namespace Discretization
{
    public static class DataGeneration
    {
        //Methods
        public static List<double> GenerateNoisyData(List<int> x_crisp, double maxNoise, int numPerCrispValue)
        {
            return GenerateNoisyData(x_crisp.ConvertAll<double>(x => (double)x), maxNoise, numPerCrispValue);
        }
        public static List<double> GenerateNoisyData(List<double> x_crisp, double maxNoise, int numPerCrispValue)
        {
            //Create list of values with noise
            Random rand = new Random();
            List<double> x_noisy = new List<double>();

            foreach (double x in x_crisp)
            {
                for (int i = 0; i < numPerCrispValue; i++)
                {
                    double factor = SampleGaussian(rand, 0, 1.0 / 6.0); //Generates a value between 0 and 1. We know that 6 sigma covers 99.999999% of values. So, 1/6 std dev results in -1 to 1.
                    x_noisy.Add(x + factor * maxNoise);
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
