using System;
using System.Collections.Generic;
using System.Text;

namespace Discretization
{
    public class Discretizer
    {
        //Properties
        public List<Bin> Bins = new List<Bin>();

        //Constuctors
        public Discretizer()
        {
            //Create first bin to accept all values
            Bins.Add(new Bin());
        }

        //Methods
        public Bin GetBin(double value)
        {
            //Find appropriate bin
            Bin theBin = Bins.Find(p => value >= p.Low && value < p.High); //inclusive low. exclusive high

            //Add this value to that bin
            theBin.AddValue(value);

            //Determine the action to take
            switch (theBin.PickAction())
            {
                case BinAction.SplitAtAvg:
                    SplitBin(theBin, theBin.Average);
                    break;
                case BinAction.SplitAtNegNSigma:
                    { 
                    double splitPoint = theBin.Average - 1.05 * theBin.NumStandardDeviations * theBin.StandardDeviation;
                    SplitBin(theBin, splitPoint);
                    }
                    break;
                case BinAction.SplitAtPosNSigma:
                    { 
                    double splitPoint = theBin.Average + 1.05 * theBin.NumStandardDeviations * theBin.StandardDeviation;
                    SplitBin(theBin, splitPoint);
                    }
                    break;

                case BinAction.MergeHigh:
                    Bin binHigh = Bins.Find(p => p.Low == theBin.High);
                    MergeBins(theBin, binHigh);
                    break;
                case BinAction.MergeLow:
                    Bin binLow = Bins.Find(p => p.High == theBin.Low);
                    MergeBins(binLow, theBin);
                    break;
            }

            //Return that bin as the selected bin
            return theBin;
        }
        //public void PerformAction(Bin theBin)
        //{
        //    //Calculate high and low of the recorded data
        //    double sigmaLow = theBin.Average - theBin.NumStandardDeviations * theBin.StandardDeviation;
        //    double sigmaHigh = theBin.Average + theBin.NumStandardDeviations * theBin.StandardDeviation;

        //    switch (theAction)
        //    { 
        //        case BinAction.Split:
        //            //Initial bin
        //            if (theBin.Low == double.NegativeInfinity && theBin.High == double.PositiveInfinity)
        //            {
        //                //Create 3 bins
        //                Bin binLow = new Bin(double.NegativeInfinity, sigmaLow);
        //                Bin binMid = new Bin(sigmaLow, sigmaHigh);
        //                Bin binHigh = new Bin(sigmaHigh, double.PositiveInfinity);
        //                Bins.AddRange(new Bin[] { binLow, binMid, binHigh});
        //                Bins.Remove(theBin);
        //                return;
        //            }

        //            //Both ends are outside the bin range.
        //            if (sigmaLow < theBin.Low && sigmaHigh > theBin.High)
        //            {
        //                //Create  bins
        //                Bin binLow = new Bin(theBin.Low, theBin.Average);
        //                Bin binHigh = new Bin(theBin.Average, theBin.High);
        //                Bins.AddRange(new Bin[] { binLow, binHigh });
        //                Bins.Remove(theBin);
        //                return;
        //            }

        //            break;
        //    }
        //}
        public List<Bin> SplitBin(Bin theBin, double splitPoint)
        {
            //Validate split point
            if (splitPoint <= theBin.Low || splitPoint >= theBin.High)
                return new List<Bin>(); //Do nothing

            //Create bins
            Bin binLow = new Bin(theBin.Low, splitPoint, theBin.NumStandardDeviations) { MinPointsForAction = theBin.MinPointsForAction };
            Bin binHigh = new Bin(splitPoint, theBin.High, theBin.NumStandardDeviations) { MinPointsForAction = theBin.MinPointsForAction };

            //Check if data statistics can be kept
            if (binLow.Low <= theBin.AvgMinusNSigma && binLow.High > theBin.AvgPlusNSigma)
            {
                binLow.Count = theBin.Count;
                binLow.Sum = theBin.Sum;
                binLow.SquareSum = theBin.SquareSum;
            }
            else if (binHigh.Low <= theBin.AvgMinusNSigma && binHigh.High > theBin.AvgPlusNSigma)
            {
                binHigh.Count = theBin.Count;
                binHigh.Sum = theBin.Sum;
                binHigh.SquareSum = theBin.SquareSum;
            }

            //Update Bins list
            Bins.Remove(theBin);
            Bins.Add(binLow);
            Bins.Add(binHigh);

            //Return new bins
            return new List<Bin> {binLow, binHigh};
        }
        public Bin MergeBins(Bin bin1, Bin bin2)
        {
            //Get new settings for bin
            double newLow = Math.Min(bin1.Low, bin2.Low);
            double newHigh = Math.Max(bin1.High, bin2.High);
            double newStdDev = Math.Max(bin1.NumStandardDeviations, bin2.NumStandardDeviations);
            int newMinPointsForAction = Math.Max(bin1.MinPointsForAction, bin2.MinPointsForAction);

            //Create Bin
            Bin combinedBin = new Bin(newLow, newHigh, newStdDev) { MinPointsForAction = newMinPointsForAction };

            //Combine data
            combinedBin.Count = bin1.Count + bin2.Count;
            combinedBin.Sum = bin1.Sum + bin2.Sum;
            combinedBin.SquareSum = bin1.SquareSum + bin2.SquareSum;
            
            //Update Bins list
            Bins.Remove(bin1);
            Bins.Remove(bin2);
            Bins.Add(combinedBin);

            return combinedBin;
        }

        //Debug
        public override string ToString()
        {
            return string.Format("Bins={0}", Bins.Count);
        }
    }
}
