using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using static Discretization.DataGeneration;

namespace Discretization
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Discretizer
    {
        /// <summary>
        /// Known Issues:
        /// - When restoring from json, '_lastID' does not update until "GenerateID()"
        ///   is called at least one time.
        /// - "GenerateIdDelegate" cannot be saved during serialization (json). As such, it
        ///    resets to the default method when deserialized.
        /// </summary>

        //Properties
        public string Name { get; set; }
        public List<Bin> Bins {get; set;} 
        public List<Bin> BinsOrderedByLow { get { return this.Bins.OrderBy(p => p.Low).ToList(); }}
        [JsonIgnore]
        public Func<int> GenerateIdDelegate { get; set; } 

        //Cache - Properties
        private int _lastID = 0;

        //Constuctors
        public Discretizer()
        {
            //Default ID generator.
            this.GenerateIdDelegate = delegate ()
            {
                _lastID++;
                return _lastID;
            };

            //Start with initial bin, which should have limits of +- infinity.
            this.Bins = new List<Bin>() { new Bin(GenerateId()) };
        }
        public Discretizer(Func<int> generateIdDelegate)
        {
            //Default ID generator.
            this.GenerateIdDelegate = generateIdDelegate;

            //Start with initial bin, which should have limits of +- infinity.
            this.Bins = new List<Bin>() { new Bin(GenerateId()) };
        }
        [JsonConstructor]
        private Discretizer(string notUsed)
        {
            //Clear initial bins
            this.Bins = new List<Bin>();

            //Reset ID generator
            //_lastID = this.Bins.Max(p => p.BinID);
            this.GenerateIdDelegate = delegate ()
            {
                //reset _lastID if called the first time, after restore from json
                if (_lastID == 0)
                    if (this.Bins.Count > 0)
                        _lastID = this.Bins.Max(p => p.BinID);

                _lastID++;
                return _lastID;
            };
        }
        public static Discretizer FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Discretizer>(json);
        }

        //Methods - Operation
        public Bin GetBin(double value)
        {
            Bin theBin = Bins.ToList().Find(p => value >= p.Low && value < p.High); //inclusive low. exclusive high
            return theBin;
        }

        //Methods - Learning
        private int GenerateId()
        {
            return GenerateIdDelegate();
        }
        public Bin Learn(double value)
        {
            //Find appropriate bin
            Bin theBin = this.GetBin(value);
            
            //Add this value to that bin
            theBin.AddValue(value);

            //Determine the action to take
            BinAction theAction = theBin.PickAction();
            switch (theAction)
            {
                case BinAction.SplitAtAvg:
                    { 
                        SplitBin(theBin, theBin.Average);
                        break;
                    }
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
            if (LogDevelopment)
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
            Bin binLow = new Bin(GenerateId(), theBin.Low, splitPoint) { MinPointsForAction = newMinPointsForAction };
            Bin binHigh = new Bin(GenerateId(), splitPoint, theBin.High) { MinPointsForAction = newMinPointsForAction };

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
            Bins.Add(binLow);
            Bins.Add(binHigh);
            Bins.Remove(theBin);

            //Trigger Events
            OnBinAdded?.Invoke(this, new DiscretizerEventArgs()
            {
                SourceDiscretizer = this,
                SourceBin = binLow
            });
            OnBinAdded?.Invoke(this, new DiscretizerEventArgs()
            {
                SourceDiscretizer = this,
                SourceBin = binHigh
            });
            OnBinRemoved?.Invoke(this, new DiscretizerEventArgs()
            {
                SourceDiscretizer = this,
                SourceBin = theBin
            });

            //Return new bins
            return new List<Bin> {binLow, binHigh};
        }
        public Bin MergeBins(Bin binLow, Bin binHigh)
        {
            return MergeBins(binLow, binHigh, false);
        }
        public Bin MergeBins(Bin binLow, Bin binHigh, bool keepStatistics)
        {
            //Get settings for new bin
            double newLow = Math.Min(binLow.Low, binHigh.Low);
            double newHigh = Math.Max(binLow.High, binHigh.High);
            double newMinPointsForAction = Math.Max(binLow.MinPointsForAction, binHigh.MinPointsForAction);
            newMinPointsForAction *= 1.05;

            //Create Bin
            Bin combinedBin = new Bin(GenerateId(), newLow, newHigh) { MinPointsForAction = newMinPointsForAction };

            //Combine data
            if(keepStatistics)
            {
                combinedBin.Count = binLow.Count + binHigh.Count;
                //combinedBin.Count1StdDev = bin1.Count1StdDev + bin2.Count1StdDev;
                combinedBin.Sum = binLow.Sum + binHigh.Sum;
                combinedBin.SquareSum = binLow.SquareSum + binHigh.SquareSum;
            }

            //Update Bins list
            Bins.Add(combinedBin);
            Bins.Remove(binLow);
            Bins.Remove(binHigh);

            //Trigger event
            OnBinAdded?.Invoke(this, new DiscretizerEventArgs()
            {
                SourceDiscretizer = this,
                SourceBin = combinedBin
            });
            OnBinRemoved?.Invoke(this, new DiscretizerEventArgs()
            {
                SourceDiscretizer = this,
                SourceBin = binLow
            });
            OnBinRemoved?.Invoke(this, new DiscretizerEventArgs()
            {
                SourceDiscretizer = this,
                SourceBin = binHigh
            });

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

            //Check that it is correct type
            if(obj.GetType() != typeof(Discretizer)) return false;
            
            //Convert to correct type
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

        //Events
        public event EventHandler<DiscretizerEventArgs> OnBinAdded;
        public event EventHandler<DiscretizerEventArgs> OnBinRemoved;
        public class DiscretizerEventArgs : EventArgs
        {
            public Discretizer SourceDiscretizer { get; set; }
            public Bin SourceBin { get; set; }
        }

        //Debug
        public override string ToString()
        {
            return DebuggerDisplay;
        }
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("Bins={0}", Bins.Count);
            }
        }
        public bool LogDevelopment = false;
        public List<HistoryItem> DevelopmentHistory = new List<HistoryItem>();
        public List<HistoryItem> DevelopmentHistory_NoWaiting
        {
            get
            {
                return DevelopmentHistory.Where(p => p.Action != BinAction.InsufficientData && p.Action != BinAction.Undecided && p.Action != BinAction.None).ToList();
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
                return string.Format("Bins={0}, {1}: {2}", this.Bins.Count, this.Action, this.ModifiedBin.DebuggerDisplay);
            }
        }
    }
}