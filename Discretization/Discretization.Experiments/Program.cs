using System;
using System.Collections.Generic;
using System.IO;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Discretization.Examples;

namespace Discretization.Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            Exp_CreateSVG();

            //var values = DiscretizerExamples.GenerateNoisyData(new List<double>() { 0 }, 10, 1000);
            //SaveValuesToFile(values, @"data.txt");

            //Finished
            Console.WriteLine("Finished!");
        }

        //Experiments
        public static void Exp_CreateSVG()
        {
            //Create bins
            //var bins = new List<Bin> {OneValueLooseExample()};
            //var bins = new List<Bin> {OneValueTightExample()};
            //var bins = TwoValuesExample().Bins;
            //var bins = ManyValuesExample().Bins;
            
            //Save Chart
            //CreateSVG(bins, @"results/Example.svg");
        }

        //Methods - Examples
        public static Bin OneValueTightExample()
        {
            //Create bins
            Bin theBin = new Bin(0, 10);
            for(int i=0; i<100; i++)
                theBin.AddValues(new List<double>() {
                    //2,2,
                    4.5,4.6,4.7,4.8,4.9,
                    5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
                    5.1,5.2,5.3,5.4,5.5,
                    //8,8
                });

            return theBin;
        }
        public static Bin OneValueLooseExample()
        {
            //Create bins
            Bin theBin = new Bin(0, 10);
            for(int i=0; i<100; i++)
                theBin.AddValues(new List<double>() {
                    //1,1,1,1,1,1,1,1,1,1,1,
                    //2,2,2,2,2,2,2,2,2,2,2,
                    3,3,3,3,3,3,3,
                    4,4,4,4,4,4,4,4,4,4,4,
                    5,5,
                    6,6,6,6,6,6,6,6,6,6,6,
                    7,7,7,7,7,7,7,
                    //8,8,8,8,8,8,8,8,8,8,8,
                    //9,9,9,9,9,9,9,9,9,9,9,
                });

            return theBin;
        }
        public static Discretizer TwoValuesExample()
        {
            return DiscretizerExamples.TwoValues(2.0);
        }
        public static Discretizer ManyValuesExample()
        {
            return DiscretizerExamples.PickSteps(0, 100, 11, 1.0);
        }

        //Methods - Support
        public static void SaveValuesToFile(List<double> values, string filePath)
        {
            using (StreamWriter file =
            new StreamWriter(@"data.txt"))
            {
                foreach (double v in values)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(v.ToString());
                }
            }
        }
        public static void CreateSVG(List<Bin> bins, string filePath)
        {
            CreateSVG(bins, filePath, 600, 100);
        }
        public static void CreateSVG(List<Bin> bins, string filePath, int width, int height)
        {
            //Create empty chart
            var myModel = new PlotModel() {
                Background = OxyColor.FromArgb(255, 255, 255, 255)
            };
            myModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = true
            });
            myModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 1.05,
                IsAxisVisible = false
            });

            //Add graphics for each bin
            foreach (Bin theBin in bins)
            {
                //check for infinity and NaN
                if (theBin.Average == double.NegativeInfinity || theBin.Average == double.NaN || theBin.Average == double.PositiveInfinity)
                    continue;

                #region Series for bin limits
                var seriesLimits = new LineSeries
                {
                    MarkerType = MarkerType.Triangle,
                    MarkerSize = 5,
                    MarkerFill = OxyColor.FromArgb(255, 0, 0, 0),
                    LineStyle = LineStyle.None,
                };
                seriesLimits.Points.Add(new DataPoint(theBin.Low, 0));
                seriesLimits.Points.Add(new DataPoint(theBin.High, 0));
                #endregion

                #region Series for Gaussian distribution curve
                var seriesGaussianDistribution = new LineSeries
                {
                    MarkerType = MarkerType.None, //.Circle
                    //MarkerSize = 4,
                    //MarkerFill = OxyColor.FromArgb(255, 0, 0, 0),
                    LineStyle = LineStyle.Dash,
                    StrokeThickness = 2,
                    Smooth = true,
                    Color = OxyColor.FromArgb(255, 0, 0, 0)
                };
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsNeg[6], theBin.StdDevsNegPercent[6]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsNeg[5], theBin.StdDevsNegPercent[5]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsNeg[4], theBin.StdDevsNegPercent[4]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsNeg[3], theBin.StdDevsNegPercent[3]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsNeg[2], theBin.StdDevsNegPercent[2]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsNeg[1], theBin.StdDevsNegPercent[1]));
                //seriesGaussianDistribution.Points.Add(new DataPoint(theBin.Average, theBin.StdDevsNegPercent[0] + theBin.StdDevsPosPercent[0])); // For crisp values
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsPos[1], theBin.StdDevsPosPercent[1]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsPos[2], theBin.StdDevsPosPercent[2]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsPos[3], theBin.StdDevsPosPercent[3]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsPos[4], theBin.StdDevsPosPercent[4]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsPos[5], theBin.StdDevsPosPercent[5]));
                seriesGaussianDistribution.Points.Add(new DataPoint(theBin.StdDevsPos[6], theBin.StdDevsPosPercent[6]));
                #endregion

                #region Series for Inner Bins distribution boxes
                var seriesInnerBinsDistribution = new BoxPlotSeries()
                {
                    Stroke = OxyColors.Black,
                    StrokeThickness = 1,
                    Fill = OxyColor.FromArgb(255, 150, 150, 150),
                    BoxWidth = Math.Abs(theBin.InnerBins[1]-theBin.InnerBins[0])*0.95
                };

                //Add first entry using bin.Low
                seriesInnerBinsDistribution.Items.Add(new BoxPlotItem(
                        (theBin.Low + theBin.InnerBins[0])/2.0, //x location
                        0, //Lower Whisker
                        0, //Box Bottom
                        0, //Median
                        theBin.InnerBinsPercent[0], //Box Top
                        theBin.InnerBinsPercent[0] //Upper Whisker
                    )
                );

                //Add all other items.
                for(int i=1; i<theBin.InnerBins.Length; i++)
                {
                    //Calculate position and size
                    double xPos = (theBin.InnerBins[i-1] + theBin.InnerBins[i])/2.0;
                    double barHeight = theBin.InnerBinsPercent[i];
                
                    //Add to series
                    seriesInnerBinsDistribution.Items.Add(new BoxPlotItem(
                        xPos, //x location
                        0, //Lower Whisker
                        0, //Box Bottom
                        0, //Median
                        barHeight, //Box Top
                        barHeight //Upper Whisker
                    ));
                }

                #endregion

                #region Labels
                var seriesLabels = new LineSeries
                {
                    MarkerType = MarkerType.None,//Circle,
                    //MarkerFill = OxyColor.FromArgb(255, 255, 0, 0),
                    MarkerSize = 3,
                    LabelFormatString = "{0:N2}",
                    LineStyle = LineStyle.None,
                    
                };
                seriesLabels.Points.Add(new DataPoint(theBin.Average, (theBin.StdDevsNegPercent[1] + theBin.StdDevsPosPercent[1])/2.0));
                seriesLabels.Points.Add(new DataPoint(theBin.Low, 0));
                seriesLabels.Points.Add(new DataPoint(theBin.High, 0));
                #endregion

                //Add the series
                myModel.Series.Add(seriesInnerBinsDistribution);
                myModel.Series.Add(seriesGaussianDistribution);
                myModel.Series.Add(seriesLimits);
                myModel.Series.Add(seriesLabels);
            }

            //Save chart to file as SVG
            using (var stream = File.Create(filePath))
            {
                var exporter = new SvgExporter { Width = width, Height = height };
                exporter.Export(myModel, stream);
            }

        }
    }
}
