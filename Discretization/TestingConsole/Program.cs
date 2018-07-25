using System;
using System.Collections.Generic;
using System.Linq;
using Discretization;

namespace TestingConsole
{
    class Program
    {
        static Discretizer disc = new Discretizer();

        static void Main(string[] args)
        {
            //Values to generate from
            var x_source = Enumerable.Range(0, 100).ToArray();

            //Create list of values with noise
            Random rand = new Random();
            double noise = 0.1;
            List<double> x_noisy = new List<double>();
            for(int i=0; i<100; i++)
            {
                foreach(int x in x_source)
                {
                    double relNoise = 2*noise*rand.NextDouble();
                    x_noisy.Add((x-noise)+relNoise);
                }
            }

            //Add values to discretizer;
            foreach(double x in x_noisy)
            {
                disc.GetBin(x);
            }

            var bins = disc.Bins.OrderBy(b => b.Average).ToList();

        }
    }
}
