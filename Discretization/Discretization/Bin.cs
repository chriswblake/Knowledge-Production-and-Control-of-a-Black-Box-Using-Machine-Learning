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
        public double Low { get; private set; }
        public double High { get; private set; }
        public double MinPointsForAction { get; set; }
        
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
        public Bin() : this(double.NegativeInfinity, double.PositiveInfinity) { }
        public Bin(double low, double high) : this(low, high, 6) { }
        public Bin(double low, double high, double numStandardDeviations)
        {
            this.Low = low;
            this.High = high;
            this.NumStandardDeviations = numStandardDeviations;
            this.MinPointsForAction = 2;
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
            double nSigmaLow = this.Average - NumStandardDeviations * StandardDeviation;
            double nSigmaHigh = this.Average + NumStandardDeviations * StandardDeviation;

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
            };
            return c;
        }

        //Debug
        public string DebuggerDisplay
        {
            get
            {
                string s = "";
                
                //Add low and lower sigma
                if (this.Low <= this.AvgMinusNSigma)
                    s += string.Format("[{0:N2} ({1:N2})", this.Low, this.AvgMinusNSigma);
                else
                    s += string.Format("({0:N2}) [{1:N2}", this.AvgMinusNSigma, this.Low);

                //Add the average
                s += string.Format(", |{0:N2}|, ", this.Average);

                //Add high and upper sigma
                if (this.AvgPlusNSigma <= this.High)
                    s += string.Format("({0:N2}) {1:N2}]", this.AvgPlusNSigma, this.High);
                else
                    s += string.Format("{0:N2}] ({1:N2})", this.High, this.AvgPlusNSigma);

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
