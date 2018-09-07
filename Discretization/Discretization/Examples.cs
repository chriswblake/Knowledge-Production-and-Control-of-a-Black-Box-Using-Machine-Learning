using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Discretization.DataGeneration;

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
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
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
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
                //Add all values to discretizer
                foreach (double x in x_noisy)
                    disc.GetBin(x);
                rangeCount.Add(disc.Bins.Count);
            }

            return disc;
        }
    }
}