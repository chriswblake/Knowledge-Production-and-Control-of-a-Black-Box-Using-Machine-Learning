using System;

namespace Discretization
{
    public class Bin : IComparable
    {
        //Properties
        int BinID { get; set; }
        int Count { get; set; }
        double Sum { get; set; }
        double SquareSum { get; set; }
        public double Average {
            get
            {
                //Check for divide by zero
                if (Count <= 0)
                    return 0;

                return Sum / Count;
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
                double denominator = Count - 1;
                return Math.Sqrt(numerator / denominator);
            }
        }

        //Properties - Range
        /// <summary>
        /// The lowest allowed value to be assigned to this bin. (inclusive)
        /// </summary>
        public double Low { get; private set; }
        /// <summary>
        /// The maximum value allowed to be assigned to this bin. (exclusive).
        /// </summary>
        public double High { get; private set; }
        /// <summary>
        /// Defines the accuracy requirement for values considered out of tolerance. A higher value means a larger
        /// window around the average will be accepted as in tolerance.
        /// 1 Standard Deviation  => 68.3%
        /// 2 Standard Deviations => 95.5%
        /// 3 Standard Deviations => 99.7%
        /// 4 Standard Deviations => 99.9%
        /// </summary>
        public double NumStandardDeviations { get; set; }
        /// <summary>
        /// The number of values that have been assigned to this bin but were not within
        /// the accepted tolerance range (number of standard deviations).
        /// </summary>
        int CountOutTolerance { get; set; }
        int CountInTolerance { get; set; }

        //Constructors
        public Bin() : this(double.NegativeInfinity, double.PositiveInfinity) { }
        public Bin(double low, double high)
        {
            this.Low = low;
            this.High = high;
            this.NumStandardDeviations = 1;
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
            if (distance < this.NumStandardDeviations)
                this.CountInTolerance += 1;
            else
                this.CountOutTolerance += 1;
        }
        public bool ShouldSplit()
        {
            //If more values have been experienced out of the tolerance area, this bin should be split.
            return (this.CountOutTolerance > 2 * this.CountInTolerance);
        }

        //Debug
        public override string ToString()
        {
            return string.Format("Avg:{0:N2} \tLow:{1:N2} \tHigh:{2:N2}", this.Average, this.Low, this.High);
        }

        //Icomparable
        public int CompareTo(object obj)
        {
            return this.Average.CompareTo(obj);
        }
    }
}
