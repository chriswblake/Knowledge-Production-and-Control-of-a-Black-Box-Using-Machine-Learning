using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xunit;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Discretization;
using static Discretization.DataGeneration;
using System.Linq;

namespace KnowProdContBlackBox.Experiments
{
    public class DiscretizationExperiments : Experiment
    {
        #region Experiments
        //[Theory]
        //[InlineData(0.01)]
        //[InlineData(0.1)]
        //[InlineData(0.2)]
        //[InlineData(0.5)]
        //[InlineData(0.9)]
        //[InlineData(2.0)]
        //[InlineData(2.4)]
        public void TwoBinsVsIncreasingNoise(double maxNoise)
        {
            //Values to generate from
            List<int> x_crisp = new List<int> { 0, 5 };
            int passes = 10000;

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            Random rand = new Random();
            for (int i = 0; i < passes; i++)
            {
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
                foreach (double x in x_noisy)
                    disc.Learn(x);
            }

            //Check results
            Assert.Equal(x_crisp.Count + 2, disc.Bins.Count);

            //Save graphic
            CreatePDF(disc.Bins, Path.Combine(ResultsDir, "TwoBinsVsIncreasingNoise_" + maxNoise + ".pdf"));
        }

        //[Theory]
        //[InlineData(0.01)]
        //[InlineData(0.1)]
        //[InlineData(0.2)]
        //[InlineData(0.5)]
        //[InlineData(0.8)]
        //[InlineData(1.0)]
        //[InlineData(2.0)]
        //[InlineData(2.4)]
        //[InlineData(2.49)] //May take a few tries until correct
        //[InlineData(2.5)] //May take several tries until correct
        //[InlineData(2.6)] //Doesn't work
        //[InlineData(3.0)] //doesn't work
        public void FourBinsVsIncreasingNoise(double maxNoise)
        {
            //Values to generate from
            List<int> x_crisp = new List<int> { 0, 5, 10, 15 };
            int passes = 10000;

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            Random rand = new Random();
            for (int i = 0; i < passes; i++)
            {
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
                foreach (double x in x_noisy)
                    disc.Learn(x);
            }

            //Check results
            Assert.Equal(x_crisp.Count + 2, disc.Bins.Count);

            //Save graphic
            CreatePDF(disc.Bins, Path.Combine(ResultsDir, "FourBinsVsIncreasingNoise_" + maxNoise+".pdf"));
        }

        //[Theory]
        //[InlineData(5)]
        //[InlineData(2.5)]
        //[InlineData(1)]
        //[InlineData(0.5)]
        //[InlineData(0.1)]
        public void BinsVsResolution(double resolution)
        {
            //Values to generate from
            List<double> x_crisp = new List<double>();
            for (double d = 0; d <= 10; d += resolution)
                x_crisp.Add(d);
            int passes = 10000;
            double maxNoise = 0.01;

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            Random rand = new Random();
            for (int i = 0; i < passes; i++)
            {
                List<double> x_noisy = GenerateNoisyData(x_crisp, maxNoise, 1).OrderBy(p => rand.NextDouble()).ToList();
                foreach (double x in x_noisy)
                    disc.Learn(x);
            }

            //Check results
            Assert.Equal(x_crisp.Count + 2, disc.Bins.Count);

            //Save graphic
            string noiseThousands = Convert.ToInt64(maxNoise * 1000).ToString();
            CreatePDF(disc.Bins, Path.Combine(ResultsDir, "BinsVsResolution_" + resolution + ".pdf"));
        }

        //[Fact]
        public void AdaptiveResolution()
        {
            //Values to generate from
            List<double> x_crisp1 = new List<double> {
                5.0, 5.5,
                6.0, 6.5,
                7.0, 7.5,
                8.0, 8.5,
                9.0, 9.5, 10.0
            };
            List<double> x_crisp2 = new List<double> {
                0,
                15
            };
            int passes = 10000;

            //Add all values to the discretizer
            Discretizer disc = new Discretizer();
            Random rand = new Random();
            for (int i = 0; i < passes; i++)
            {
                List<double> x_noisy = GenerateNoisyData(x_crisp1, 0.2, 1);
                List<double> x_noisy2 = GenerateNoisyData(x_crisp2, 0.3, 1);
                foreach (double x in x_noisy.Concat(x_noisy2).OrderBy(p => rand.NextDouble()).ToList())
                    disc.Learn(x);
            }

            //Start over until sucessfull
            if (x_crisp1.Count + x_crisp2.Count + 2 != disc.Bins.Count)
            { 
                AdaptiveResolution();
                return;
            }

            //Check results
            Assert.Equal(x_crisp1.Count + x_crisp2.Count + 2, disc.Bins.Count);

            //Save graphic
            CreatePDF(disc.Bins, Path.Combine(ResultsDir, "AdaptiveResolution.pdf"));
        }

        //[Theory]
        //[InlineData(0.1)]
        //[InlineData(0.5)]
        //[InlineData(1.0)]
        //[InlineData(2.5)]
        //[InlineData(4.0)]
        //[InlineData(5.0)]
        public void InnerBinDistributionVsNoise(double maxNoise)
        {
            //Create bins
            Random rand = new Random();
            List<Bin> bins = new List<Bin>();
            Bin bin1 = new Bin(0, 0, 10);
            bins.Add(bin1);
            for (int i = 0; i < 10000; i++)
                bin1.AddValue(GenerateNoisyValue(rand, 5.0, maxNoise));

            CreatePDF(bins, Path.Combine(ResultsDir, "InnerBinDistributionVsNoise_"+maxNoise+".pdf"));
        }

        #endregion

        //var values = DiscretizerExamples.GenerateNoisyData(new List<double>() { 0 }, 10, 1000);
        //SaveValuesToFile(values, @"data.txt");

        //Methods - Examples
        //public static List<Bin> TwoValuesExample()
        //{
        //    Discretizer disc = DiscretizerExamples.TwoValues(2.0);
        //    return disc.Bins;
        //}
        //public static List<Bin> ManyValuesExample()
        //{
        //    Discretizer disc = DiscretizerExamples.PickSteps(0, 100, 11, 1.0);
        //    return disc.Bins;
        //}

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
        public static void CreatePDF(List<Bin> bins, string filePath)
        {
            CreatePDF(bins, filePath, 600, 100);
        }
        public static void CreatePDF(List<Bin> bins, string filePath, int width, int height)
        {
            //Create empty chart
            var myModel = new PlotModel()
            {
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
                Minimum = -0.001,
                MaximumRange = 1.05,
                IsAxisVisible = true
            });

            //Add graphics for each bin
            Random rand = new Random();
            foreach (Bin theBin in bins)
            {
                //check for infinity and NaN
                if (theBin.Average == double.NegativeInfinity || theBin.Average == double.NaN || theBin.Average == double.PositiveInfinity)
                    continue;

                #region Series for bin limits
                var seriesLimits = new LineSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 0.8,
                    MarkerFill = OxyColor.FromArgb(100, 0, 0, 0),
                    LineStyle = LineStyle.None,
                };
                for(double i=0; i<=1; i+=0.048)
                { 
                    seriesLimits.Points.Add(new DataPoint(theBin.Low, i));
                    seriesLimits.Points.Add(new DataPoint(theBin.High, i));
                }
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
                    StrokeThickness = 0,
                    //Fill = OxyColor.FromArgb(255, 150, 150, 150),
                    Fill = OxyColor.FromArgb(255, Convert.ToByte(rand.Next(0,256)), Convert.ToByte(rand.Next(0, 256)), Convert.ToByte(rand.Next(0, 256))),
                    BoxWidth = Math.Abs(theBin.InnerBins[1] - theBin.InnerBins[0]) * 0.95
                };

                //Add first entry using bin.Low
                seriesInnerBinsDistribution.Items.Add(new BoxPlotItem(
                        (theBin.Low + theBin.InnerBins[0]) / 2.0, //x location
                        0, //Lower Whisker
                        0, //Box Bottom
                        0, //Median
                        theBin.InnerBinsPercent[0], //Box Top
                        theBin.InnerBinsPercent[0] //Upper Whisker
                    )
                );

                //Add all other inner bins.
                for (int i = 1; i < theBin.InnerBins.Length; i++)
                {
                    //Calculate position and size
                    double xPos = (theBin.InnerBins[i - 1] + theBin.InnerBins[i]) / 2.0;
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
                seriesLabels.Points.Add(new DataPoint(theBin.Average, (theBin.StdDevsNegPercent[1] + theBin.StdDevsPosPercent[1]) / 2.0));
                //seriesLabels.Points.Add(new DataPoint(theBin.Low, 0));
                //seriesLabels.Points.Add(new DataPoint(theBin.High, 0));
                #endregion

                //Add the series
                myModel.Series.Add(seriesInnerBinsDistribution);
                myModel.Series.Add(seriesGaussianDistribution);
                myModel.Series.Add(seriesLimits);
                myModel.Series.Add(seriesLabels);
            }

            //Save chart to file as PDF
            using (var stream = File.Create(filePath))
            {
                var exporter = new PdfExporter { Width = width, Height = height };
                exporter.Export(myModel, stream);
            }

        }
    }
}
