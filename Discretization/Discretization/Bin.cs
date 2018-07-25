using System;

namespace Discretization
{
    public class Bin
    {
        //Properties
        int BinID { get; set; }
        int Count { get; set; }
        double Sum { get; set; }
        double SquareSum { get; set; }
        double Average {
            get
            {
                //Check for divide by zero
                if (Count <= 0)
                    return 0;

                return Sum/Count;
            }
        }
        double StandardDeviation
        {
            get
            {
                //Check for divide by zero
                if (Count <= 1)
                    return double.PositiveInfinity;

                //Compute standard deviation (N-1)
                double numerator = (SquareSum) + (-2 * Average * Sum) + (Count * Math.Pow(Average, 2));
                double denominator = Count-1;
                return Math.Sqrt(numerator / denominator);
            }
        }

        //Properties - Range
        double Low { get; set; }
        double High { get; set; }
        double NumStandardDeviations { get; set; }
        int CountOutTolerance { get; set; }

        //Constructors
        public Bin() : this(double.NegativeInfinity, double.PositiveInfinity) { }
        public Bin(double low, double high)
        {
            this.Low = low;
            this.High = high;
        }

        //Methods
        public void AddValue(double value)
        {
            //Update statistics
            this.Count += 1;
            this.Sum += value;
            this.SquareSum += Math.Pow(value, 2);

            //Check if in tolerance
            double differance = Math.Abs(value - Average);
            double distance = differance / this.StandardDeviation;
            if (distance > this.NumStandardDeviations)
                CountOutTolerance += 1;

        }
    }
}
