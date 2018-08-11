using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discretization.Examples
{
    public class DiscretizerExamples
    {
        static Random rand = new Random();

        //Binary at 0 and 5
        public static Discretizer TwoValues(double maxNoise)
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
                    disc.GetBin(x);
            }

            return disc;
        }

        //0 to 100 with interval 1
        public static Discretizer Many100Values(double maxNoise)
        {
            //List of crisp values
            List<int> x_crisp = Enumerable.Range(0, 100).ToList();

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            for (int i = 0; i < 10000; i++)
            {
                //Generate a noisy value for each entry then randomize the order.
                List<double> x_noisy = DiscretizerExamples.GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
                //Add all values to discretizer
                foreach (double x in x_noisy)
                    disc.GetBin(x);
            }

            return disc;
        }
        public static Discretizer PickSteps(double min, double max, int steps, double maxNoise)
        {
            //List of crisp values
            List<double> x_crisp = Enumerable.Range(0, steps).Select(i => min + (max - min) * ((double)i / (steps - 1))).ToList();
            List<double> rangeCount = new List<double>();
            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            for (int i = 0; i < 10000; i++)
            {
                //Generate a noisy value for each entry then randomize the order.
                List<double> x_noisy = DiscretizerExamples.GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
                //Add all values to discretizer
                foreach (double x in x_noisy)
                    disc.GetBin(x);
                rangeCount.Add(disc.Bins.Count);
            }

            return disc;
        }
        //Methods
        public static List<double> GenerateNoisyData(List<int> origValues, double maxNoise, int numPerOrigValue)
        {
            return GenerateNoisyData(origValues.ConvertAll<double>(x => (double)x), maxNoise, numPerOrigValue);
        }
        public static List<double> GenerateNoisyData(List<double> origValues, double maxNoise, int numPerOrigValue)
        {
            //Create list of values with noise
            List<double> x_noisy = new List<double>();
            for (int i = 0; i < numPerOrigValue; i++)
            {
                foreach (double x in origValues)
                {
                    double factor = SampleGaussian(rand, 0, 1.0 / 6.0); //Generates a value between 0 and 1. We know that 6 sigma covers 99.999999% of values. So, 1/6 means 0 to 1.
                    x_noisy.Add(x + factor * maxNoise);
                }
            }

            return x_noisy;
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