using System;
using System.Collections;
using System.Collections.Generic;

namespace Discretization
{
    public class Bin_orig
    {
        //Properties
        public int BinID { get; set; }
        public int Count { get; set; }
        public double Sum { get; set; }
        public double SquareSum { get; set; }
        public double Average
        {
            get
            {
                //Check for divide by zero
                if (Count <= 0)
                    return 0;

                return Sum / Count;
            }
        }
        public double StandardDeviation
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

        public double Low { get; protected set; }
        public double High { get; protected set; }
        public double NumStandardDeviations { get; set; }

        int CountOutTolerance { get; set; }
        int CountInTolerance { get; set; }
        double HighInTolerance
        {
            get
            {
                return Average + NumStandardDeviations * StandardDeviation;
            }
        }
        double LowInTolerance
        {
            get
            {
                return Average - NumStandardDeviations * StandardDeviation;
            }
        }

        //Constructors
        public Bin_orig() : this(double.NegativeInfinity, double.PositiveInfinity) { }
        public Bin_orig(double low, double high)
        {
            this.Low = low;
            this.High = high;
            this.NumStandardDeviations = 1; //Ideally slightly less than 33%. We want the data divided into 3 equal ranges.
        }

        //Methods
        public virtual void AddValue(double value)
        {
            //Check if value is in range
            if (value < this.Low || value >= this.High)
                return;

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
            //If the out-tolerance data is double the in-tolerance data, that means we have potentially 3 areas.
            if (this.Count > 50)
                return (this.CountOutTolerance >= 2 * this.CountInTolerance);
            else
                return false;
        }

        //Debug
        public override string ToString()
        {
            return string.Format("Avg:{0:N2} \tLow:{1:N2} \tHigh:{2:N2}", this.Average, this.Low, this.High);
        }
    }

    public class Bin
    {
        //Properties
        public int BinID { get; set; }
        public int Count { get; set; }
        public double Sum { get; set; }
        public double SquareSum { get; set; }
        public double Average
        {
            get
            {
                //Check for divide by zero
                if (Count <= 0)
                    return double.PositiveInfinity;

                return Sum / Count;
            }
        }
        public double StandardDeviation
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

        public double Low { get; private set; }
        public double High { get; private set; }
        public double NumStandardDeviations { get; private set; }
        public double AvgPlusNSigma { get
            {
                return this.Average + this.NumStandardDeviations * StandardDeviation;
            }
        }
        public double AvgMinusNSigma
        {
            get
            {
                return this.Average - this.NumStandardDeviations * StandardDeviation;
            }
        }

        public int Count1StdDev { get; set; }
        public double Percent1StdDev { get
            {
                return Convert.ToDouble(Count1StdDev) / this.Count;
            }
        }

        public int MinPointsForAction { get; set; }

        //Constructors
        public Bin() : this(double.NegativeInfinity, double.PositiveInfinity) { }
        public Bin(double low, double high) : this(low, high, 6) { }
        public Bin(double low, double high, double numStandardDeviations)
        {
            this.Low = low;
            this.High = high;
            this.NumStandardDeviations = numStandardDeviations;
            this.MinPointsForAction = 100;
        }

        //Methods
        public void AddValues(List<double> values)
        {
            foreach(double v in values)
                AddValue(v);
        }
        public void AddValue(double value)
        {
            //Check if value is in range
            if (value < this.Low || value >= this.High)
                return;

            //Update statistics
            this.Count += 1;
            this.Sum += value;
            this.SquareSum += Math.Pow(value, 2);

            //Track how many items fell in +-1 standard deviation. This will be used to check the shame of the gausian curve.
            if ((this.Average - StandardDeviation < value) && (value < this.Average + StandardDeviation))
                this.Count1StdDev += 1;

            //Reset Statistics if count is very high
            if (this.Count > 10000)
            {
                this.Count = this.Count / 10;
                this.Sum = this.Sum / 10;
                this.SquareSum = this.SquareSum / 100;
            }
                
        }

        public BinAction PickAction()
        {
            //Only provide a recommendation if the count is above the threshold.
            if (this.Count < MinPointsForAction)
                return BinAction.InsufficientData;

            //Calculate high and low of the recorded data
            double nSigmaLow = this.Average - NumStandardDeviations * StandardDeviation;
            double nSigmaHigh = this.Average + NumStandardDeviations * StandardDeviation;

            //No Action: If both ends within the bin range.
            if(this.Low != double.NegativeInfinity && this.High != double.PositiveInfinity)
                if (nSigmaLow > this.Low && nSigmaHigh < this.High)
                    return BinAction.None;
            

            //Split: If both ends are outside the bin range.
            if (nSigmaLow < this.Low && nSigmaHigh > this.High)
                return BinAction.SplitAtAvg;
            //Split: Low is -inf and +NSigma is below high
            if (this.Low == double.NegativeInfinity && nSigmaHigh < this.High)
                return BinAction.SplitAtNegNSigma;
            //Split: -NSigma is above low and high is +inf
            if (nSigmaLow > this.Low && this.High == double.PositiveInfinity)
                return BinAction.SplitAtPosNSigma;
            //Split: Distribution is too flat.
            if (Percent1StdDev < 0.50) //Ideally this should be 68.1% since it is looking for a gaussian distribution.
                return BinAction.SplitAtAvg;


            //MergeHigh: Right end overlaps other bin.
            if (nSigmaLow > this.Low && nSigmaHigh > this.High)
                return BinAction.MergeHigh;
            //MergeLow: Left end overlaps other bin.
            if (nSigmaLow < this.Low && nSigmaHigh < this.High)
                return BinAction.MergeLow;



            //This should never occur
            return BinAction.Undecided;
        }

        //Debug
        public override string ToString()
        {
            return string.Format("Low:{0:N2} \tHigh:{1:N2} \tAvg:{2:N2} \t-NSimga:{3:N2} \t+NSigma:{4:N2} \tCount:{5:N2}", this.Low, this.High, this.Average, this.AvgMinusNSigma, this.AvgPlusNSigma, this.Count);
        }
    }

    public enum BinAction
    {
        None,
        SplitAtAvg,
        SplitAtNegNSigma,
        SplitAtPosNSigma,
        MergeHigh,
        MergeLow,
        InsufficientData,
        Undecided //Probably an error. Not good!
    }

}
