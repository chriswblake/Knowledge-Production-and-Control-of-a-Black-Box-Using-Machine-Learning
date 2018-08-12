using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Discretization
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Bin
    {
        //Properties
        public int BinID { get; set; }
        public double Low { get; set; } = double.NegativeInfinity;
        public double High { get; set; } = double.PositiveInfinity;
        public double MinPointsForAction { get; set; } = 2;
        
        #region Base Statistics
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
        #endregion
        
        #region Gaussian Details
        public int Count1StdDev { get; set; }
        public double Percent1StdDev { get
            {
                return Convert.ToDouble(Count1StdDev) / this.Count;
                //return StdDevsNegPercent[1] + StdDevsPosPercent[1];
                //return StdDevsPercent[1];
            }
        }
        #endregion

        #region Gaussian Shape
        public double[] StdDevsNeg
        {
            get
            {
                if (_StdDevsNeg == null)
                    _StdDevsNeg = new double[7] {
                        this.Average - 0*this.StandardDeviation,
                        this.Average - 1*this.StandardDeviation,
                        this.Average - 2*this.StandardDeviation,
                        this.Average - 3*this.StandardDeviation,
                        this.Average - 4*this.StandardDeviation,
                        this.Average - 5*this.StandardDeviation,
                        this.Average - 6*this.StandardDeviation,
                    };

                return _StdDevsNeg;
            }
        }
        public double[] StdDevsPos
        {
            get
            {
                if (_StdDevsPos == null)
                    _StdDevsPos = new double[7] {
                        this.Average + 0*this.StandardDeviation,
                        this.Average + 1*this.StandardDeviation,
                        this.Average + 2*this.StandardDeviation,
                        this.Average + 3*this.StandardDeviation,
                        this.Average + 4*this.StandardDeviation,
                        this.Average + 5*this.StandardDeviation,
                        this.Average + 6*this.StandardDeviation,
                    };

                return _StdDevsPos;
            }
        }
        public double[] StdDevsNegCount = new double[7] { 0, 0, 0, 0, 0, 0, 0 }; //Length 7 for tracking up to 6 standard deviations, including perfectly on average (0 Std Dev).
        public double[] StdDevsPosCount = new double[7] { 0, 0, 0, 0, 0, 0, 0 }; //Length 7 for tracking up to 6 standard deviations, including perfectly on average (0 Std Dev).
        public double[] StdDevsNegPercent
        {
            get
            {
                if (_StdDevsNegPercent == null)
                    _StdDevsNegPercent = new double[7] {
                        StdDevsNegCount[0] / Count,
                        StdDevsNegCount[1] / Count,
                        StdDevsNegCount[2] / Count,
                        StdDevsNegCount[3] / Count,
                        StdDevsNegCount[4] / Count,
                        StdDevsNegCount[5] / Count,
                        StdDevsNegCount[6] / Count,
                    };

                return _StdDevsNegPercent;
            }
        }
        public double[] StdDevsPosPercent
        {
            get
            {
                if (_StdDevsPosPercent == null)
                    _StdDevsPosPercent = new double[7] {
                        StdDevsPosCount[0] / Count,
                        StdDevsPosCount[1] / Count,
                        StdDevsPosCount[2] / Count,
                        StdDevsPosCount[3] / Count,
                        StdDevsPosCount[4] / Count,
                        StdDevsPosCount[5] / Count,
                        StdDevsPosCount[6] / Count,
                    };

                return _StdDevsPosPercent;
            }
        }
        public double[] StdDevsCount
        {
            get
            {
                return new double[7] {
                    StdDevsNegCount[0] + StdDevsPosCount[0],
                    StdDevsNegCount[1] + StdDevsPosCount[1],
                    StdDevsNegCount[2] + StdDevsPosCount[2],
                    StdDevsNegCount[3] + StdDevsPosCount[3],
                    StdDevsNegCount[4] + StdDevsPosCount[4],
                    StdDevsNegCount[5] + StdDevsPosCount[5],
                    StdDevsNegCount[6] + StdDevsPosCount[6],
                };
            }
        }
        public double[] StdDevsPercent
        {
            get
            {
                return new double[7] {
                    StdDevsNegPercent[0] + StdDevsPosPercent[0],
                    StdDevsNegPercent[1] + StdDevsPosPercent[1],
                    StdDevsNegPercent[2] + StdDevsPosPercent[2],
                    StdDevsNegPercent[3] + StdDevsPosPercent[3],
                    StdDevsNegPercent[4] + StdDevsPosPercent[4],
                    StdDevsNegPercent[5] + StdDevsPosPercent[5],
                    StdDevsNegPercent[6] + StdDevsPosPercent[6],
                };
            }
        }

        //Cache
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] _StdDevsNeg = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] _StdDevsPos = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] _StdDevsNegPercent = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] _StdDevsPosPercent = null;

        //Methods
        private void Gaussian_Counter(double value)
        {
            //Clear cache
            _StdDevsNeg = null;
            _StdDevsPos = null;
            _StdDevsNegPercent = null;
            _StdDevsPosPercent = null;

            //Count value in negative sigma ranges
            double[] NegSigma = this.StdDevsNeg;
            for (int sigma = 1; sigma < NegSigma.Length; sigma++)
            {
                if (NegSigma[sigma] <= value && value < NegSigma[sigma - 1])
                    StdDevsNegCount[sigma]++;
            }

            //Count value in positive sigma ranges
            double[] PosSigma = this.StdDevsPos;
            for (int sigma = 1; sigma < PosSigma.Length; sigma++)
            {
                if (PosSigma[sigma - 1] <= value && value < PosSigma[sigma])
                    StdDevsPosCount[sigma]++;
            }
        }

        #endregion

        #region InnerBins Shape
        //Properties inner Bin distribution
        public double[] InnerBins
        {
            get
            {
                if (_InnerBins == null)
                {
                    if (this.High != double.PositiveInfinity && this.Low != double.NegativeInfinity)
                    {
                        double step = (this.High - this.Low) / 7.0;
                        _InnerBins = new double[7] {
                            this.Low + 1*step,
                            this.Low + 2*step,
                            this.Low + 3*step,
                            this.Low + 4*step,
                            this.Low + 5*step,
                            this.Low + 6*step,
                            this.Low + 7*step,
                            };
                    }else
                    {
                        _InnerBins = new double[7] {
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            };
                    }
                }
                    

                return _InnerBins;
            }
        }
        public double[] InnerBinsCount = new double[7] { 0, 0, 0, 0, 0, 0, 0 }; //Length 7 for tracking up to 6 standard deviations, including perfectly on average (0 Std Dev).
        public double[] InnerBinsPercent
        {
            get
            {
                if (_InnerBinsPercent == null)
                {
                    double countInnerBins = InnerBinsCount.Sum();
                    _InnerBinsPercent = new double[7] {
                        InnerBinsCount[0] / countInnerBins,
                        InnerBinsCount[1] / countInnerBins,
                        InnerBinsCount[2] / countInnerBins,
                        InnerBinsCount[3] / countInnerBins,
                        InnerBinsCount[4] / countInnerBins,
                        InnerBinsCount[5] / countInnerBins,
                        InnerBinsCount[6] / countInnerBins,
                    };
                }
                    

                return _InnerBinsPercent;
            }
        }

        //Cache - Inner Bin Distribution
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] _InnerBinsPercent = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] _InnerBins = null;
        
        //Methods
        private void InnerBins_Counter(double value)
        {
            //Check limits
            if (this.Low == double.NegativeInfinity || this.High == double.PositiveInfinity)
                return;

            //Clear cache
            _InnerBinsPercent = null;

            for(int i=0; i< InnerBins.Length; i++)
            {
                if (value < InnerBins[i])
                { 
                    InnerBinsCount[i]++;
                    return;
                }
            }
        }
        #endregion

        //Constructors
        public Bin() { }
        public Bin(double low, double high)
        {
            this.Low = low;
            this.High = high;
        }
        public static Bin FromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Bin>(json);
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
            if ((this.Average - StandardDeviation <= value) && (value < this.Average + StandardDeviation))
                this.Count1StdDev += 1;

            //Counters for shapes
            Gaussian_Counter(value);
            InnerBins_Counter(value);

            //Reset Statistics if count is very high
            //if (this.Count > 10000)
            //{
            //    this.Count = this.Count / 10;
            //    this.Count1StdDev = this.Count1StdDev / 10;
            //    this.Sum = this.Sum / 10;
            //    this.SquareSum = this.SquareSum / 100;
            //}
                
        }
        public BinAction PickAction()
        {
            //Only provide a recommendation if the count is above the threshold.
            if (this.Count < MinPointsForAction)
                return BinAction.InsufficientData;

            //Calculate high and low of the recorded data
            double nSigmaLow = this.StdDevsNeg[6];
            double nSigmaHigh = this.StdDevsPos[6];

            //No Action: If both ends within the bin range.
            if (this.Low != double.NegativeInfinity && this.High != double.PositiveInfinity)
                if (nSigmaLow > this.Low && nSigmaHigh < this.High)
                    if (Percent1StdDev > 0.60) //Ideally this should be 68.1% since it is looking for a gaussian distribution.
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
            if (Percent1StdDev < 0.60) //Ideally this should be 68.1% since it is looking for a gaussian distribution.
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

        //Support
        public Bin Clone()
        {
            Bin c = new Bin()
            {
                BinID = BinID,
                Low = this.Low,
                High = this.High,
                MinPointsForAction = this.MinPointsForAction,
                Count = this.Count,
                Sum = this.Sum,
                SquareSum = this.SquareSum,
                InnerBinsCount = this.InnerBinsCount,
                StdDevsNegCount = this.StdDevsNegCount,
                StdDevsPosCount = this. StdDevsPosCount,
                Count1StdDev = this.Count1StdDev,
            };
            return c;
        }
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        public override bool Equals(object obj)
        {
            //Check null
            if(obj == null) return false;

            //Check that it is a Bin
            if(obj.GetType() != typeof(Bin)) return false;
            
            //Convert to a Bin
            Bin that = (Bin) obj;

            //Check all properties
            if(!that.BinID.Equals(this.BinID)) return false;
            if(!that.Low.Equals(this.Low)) return false;
            if(!that.High.Equals(this.High)) return false;
            if(!that.MinPointsForAction.Equals(this.MinPointsForAction)) return false;

            if(!that.Count.Equals(this.Count)) return false;
            if(!that.Sum.Equals(this.Sum)) return false;
            if(!that.SquareSum.Equals(this.SquareSum)) return false;
            if(!that.Average.Equals(this.Average)) return false;
            if(!that.StandardDeviation.Equals(this.StandardDeviation)) return false;

            if(!that.Count1StdDev.Equals(this.Count1StdDev)) return false;
            if(!that.Percent1StdDev.Equals(this.Percent1StdDev)) return false;

            if(!that.StdDevsNeg.SequenceEqual(this.StdDevsNeg)) return false;
            if(!that.StdDevsPos.SequenceEqual(this.StdDevsPos)) return false;
            if(!that.StdDevsNegCount.SequenceEqual(this.StdDevsNegCount)) return false;
            if(!that.StdDevsPosCount.SequenceEqual(this.StdDevsPosCount)) return false;
            if(!that.StdDevsNegPercent.SequenceEqual(this.StdDevsNegPercent)) return false;
            if(!that.StdDevsPosPercent.SequenceEqual(this.StdDevsPosPercent)) return false;
            if(!that.StdDevsCount.SequenceEqual(this.StdDevsCount)) return false;
            if(!that.StdDevsPercent.SequenceEqual(this.StdDevsPercent)) return false;

            if(!that.InnerBins.SequenceEqual(this.InnerBins)) return false;
            if(!that.InnerBinsCount.SequenceEqual(this.InnerBinsCount)) return false;
            if(!that.InnerBinsPercent.SequenceEqual(this.InnerBinsPercent)) return false;

            //Passed all tests
            return true;
        }

        //Debug
        public string DebuggerDisplay
        {
            get
            {
                string s = "";
                
                double avgMinus6Sigma = this.StdDevsNeg[6];
                double avgPlus6Sigma = this.StdDevsPos[6];

                //Add low and lower sigma
                if (this.Low <= avgMinus6Sigma)
                    s += string.Format("[{0:N2} ({1:N2})", this.Low, avgMinus6Sigma);
                else
                    s += string.Format("({0:N2}) [{1:N2}", avgMinus6Sigma, this.Low);

                //Add the average
                s += string.Format(", |{0:N2}|, ", this.Average);

                //Add high and upper sigma
                if (avgPlus6Sigma <= this.High)
                    s += string.Format("({0:N2}) {1:N2}]", avgPlus6Sigma, this.High);
                else
                    s += string.Format("{0:N2}] ({1:N2})", this.High, avgPlus6Sigma);

                //Add action
                //s = s + " ==> "+ PickAction().ToString();

                //BinCount
                s += string.Format(" [{0}]", this.Count);
                //s += string.Format(" [{0:N1}]", this.Percent1StdDev);
                return s;// string.Format("({0:N3}{3:N3}) \t|{1:N3}| \t({4:N3}){2:N3}({4:N3}) (-NStdDev)Low|Avg|High(+NStdDev) \tCount:{5:N2}", this.Low, this.Average, this.High, this.AvgMinusNSigma, this.AvgPlusNSigma, this.Count);
            }
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
