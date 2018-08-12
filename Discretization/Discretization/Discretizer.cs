using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Discretization
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Discretizer
    {
        //Properties
        public List<Bin> Bins {get; set;} = new List<Bin>() {new Bin()};
        public List<Bin> BinsOrderedByLow { get { return this.Bins.OrderBy(p => p.Low).ToList(); }}
        public List<Bin> BinsFirst10 { get { return BinsOrderedByLow.Take(5).ToList(); } }
        public List<Bin> BinsLast10 { get { return BinsOrderedByLow.Skip(Bins.Count-5).Take(5).ToList(); } }

        //Constuctors
        public Discretizer() {}
        [JsonConstructor]
        private Discretizer(string notUsed)
        {
            //Clear initial bins
            this.Bins = new List<Bin>();
        }
        public static Discretizer FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Discretizer>(json);
        }
        
        //Methods
        public Bin GetBin(double value)
        {
            return GetBin(value, true);
        }
        public Bin GetBin(double value, bool enableTraining)
        {
            //Find appropriate bin
            Bin theBin = Bins.Find(p => value >= p.Low && value < p.High); //inclusive low. exclusive high

            //Return the bin if not training
            if(!enableTraining)
                return theBin;

            //Add this value to that bin
            theBin.AddValue(value);

            //Determine the action to take
            BinAction theAction = theBin.PickAction();
            switch (theAction)
            {
                case BinAction.SplitAtAvg:
                    SplitBin(theBin, theBin.Average);
                    break;
                case BinAction.SplitAtNegNSigma:
                    { 
                        double splitPoint = theBin.Average -  (6+0.1) * theBin.StandardDeviation; //buffer of 10% of 1 stddev
                        SplitBin(theBin, splitPoint);
                    }
                    break;
                case BinAction.SplitAtPosNSigma:
                    { 
                        double splitPoint = theBin.Average + (6+0.1)*theBin.StandardDeviation; //buffer of 10% of 1 stddev
                        SplitBin(theBin, splitPoint);
                    }
                    break;
                case BinAction.MergeHigh:
                    { 
                        //Find the next highest bin
                        Bin binHigh = Bins.Find(p => p.Low == theBin.High);
                        //Keep the statistics if merging with one of the end bin (the bins with infinity as a high).
                        bool keepStatistics = false;
                        if (theBin.Low==double.NegativeInfinity || binHigh.High == double.PositiveInfinity)
                            keepStatistics = true;
                        //Merge the bins
                        MergeBins(theBin, binHigh, keepStatistics);
                    }
                    break;
                case BinAction.MergeLow:
                    { 
                        //Find the next lowest bin
                        Bin binLow = Bins.Find(p => p.High == theBin.Low);
                        //Keep the statistics if merging with one of the end bin (the bins with infinity as a high).
                        bool keepStatistics = false;
                        if (theBin.High == double.PositiveInfinity || binLow.Low == double.NegativeInfinity)
                            keepStatistics = true;
                        //Merge the bins
                        MergeBins(binLow, theBin, keepStatistics);
                    }
                    break;
            }
            
            //Log bin development
            if(LogDevelopment)
                DevelopmentHistory.Add(new HistoryItem(this.Bins, theBin, theAction, value));

            //Return that bin as the selected bin
            return theBin;
        }
        public List<Bin> SplitBin(Bin theBin, double splitPoint)
        {
            //Validate split point
            if (splitPoint <= theBin.Low || splitPoint >= theBin.High)
                return new List<Bin>(); //Do nothing

            //Create bins
            double newMinPointsForAction = 1.05*theBin.MinPointsForAction;
            Bin binLow = new Bin(theBin.Low, splitPoint) { MinPointsForAction = newMinPointsForAction };
            Bin binHigh = new Bin(splitPoint, theBin.High) { MinPointsForAction = newMinPointsForAction };

            //Check if data statistics can be kept
            if (binLow.Low <= theBin.StdDevsNeg[6] && binLow.High > theBin.StdDevsPos[6])
            {
                binLow.Count = theBin.Count;
                binLow.Sum = theBin.Sum;
                binLow.SquareSum = theBin.SquareSum;
            }
            else if (binHigh.Low <= theBin.StdDevsNeg[6] && binHigh.High > theBin.StdDevsPos[6])
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
            return MergeBins(bin1, bin2, false);
        }
        public Bin MergeBins(Bin bin1, Bin bin2, bool keepStatistics)
        {
            //Get settings for new bin
            double newLow = Math.Min(bin1.Low, bin2.Low);
            double newHigh = Math.Max(bin1.High, bin2.High);
            double newMinPointsForAction = Math.Max(bin1.MinPointsForAction, bin2.MinPointsForAction);
            newMinPointsForAction *= 1.05;

            //Create Bin
            Bin combinedBin = new Bin(newLow, newHigh) { MinPointsForAction = newMinPointsForAction };

            //Combine data
            if(keepStatistics)
            {
                combinedBin.Count = bin1.Count + bin2.Count;
                //combinedBin.Count1StdDev = bin1.Count1StdDev + bin2.Count1StdDev;
                combinedBin.Sum = bin1.Sum + bin2.Sum;
                combinedBin.SquareSum = bin1.SquareSum + bin2.SquareSum;
            }
            //Update Bins list
            Bins.Remove(bin1);
            Bins.Remove(bin2);
            Bins.Add(combinedBin);

            return combinedBin;
        }

        //Support
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public override bool Equals(object obj)
        {
            //Check null
            if(obj == null) return false;

            //Check that it is a Bin
            if(obj.GetType() != typeof(Discretizer)) return false;
            
            //Convert to a Bin
            Discretizer that = (Discretizer) obj;

            //Check all properties
            if(!that.Bins.Count.Equals(this.Bins.Count)) return false;

            //Check bins are equal
            List<Bin> thisBins = this.Bins.OrderBy(p=> p.Low).ToList();
            List<Bin> thatBins = that.Bins.OrderBy(p=> p.Low).ToList();
            for(int i=0; i<thisBins.Count; i++)
                if(!thatBins[i].Equals(thisBins[i])) return false;

            //Passed all tests
            return true;
        }

        //Debug
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("Bins={0}", Bins.Count);
            }
        }
        public bool LogDevelopment = true;
        public List<HistoryItem> DevelopmentHistory = new List<HistoryItem>();
        public List<HistoryItem> DevelopmentHistory_NoWaiting
        {
            get
            {
                return DevelopmentHistory.Where(p => p.Action != BinAction.InsufficientData).ToList();
            }
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class HistoryItem
    {
        public List<Bin> Bins = null;
        public Bin ModifiedBin = null;
        public BinAction Action;
        public double value;

        public HistoryItem(List<Bin> resultBins, Bin modifiedBin, BinAction binAction, double value)
        {
            this.Bins = resultBins.ToList();
            this.ModifiedBin = modifiedBin.Clone();
            this.Action = binAction;
            this.value = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            { 
                return string.Format("Bins={0}, {1}: {2} {3}", this.Bins.Count, this.Action, this.ModifiedBin.DebuggerDisplay);
            }
        }
    }
}