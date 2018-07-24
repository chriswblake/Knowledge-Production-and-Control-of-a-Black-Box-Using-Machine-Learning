using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BlackBoxModeling;
using System.Threading;

namespace TestingForm
{
    public partial class Form1 : Form
    {
        //Fields
        static List<BlackBox> BlackBoxOptions = new List<BlackBox>();
        List<Thread> ioThreads = new List<Thread>();
        int chartUpdateInterval_ms = 500;


        //Constructors
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Create list of black boxes
            BlackBoxOptions.Add(new BlackBoxModeling.Samples.TrigFunctions());
            BlackBoxOptions.Add(new BlackBoxModeling.Samples.RoboticArm());

            //Set to combo box
            cmbBlackBoxes.DisplayMember = "Name";
            cmbBlackBoxes.DataSource = BlackBoxOptions;
        }

        //Methods
        private void btnStart_Click(object sender, EventArgs e)
        {
            //Retrieve selected black box
            BlackBox bb = (BlackBox)cmbBlackBoxes.SelectedItem;

            //Start the black box
            bb.Start();

            //Start updating the charts
            foreach (var io in ioThreads)
            {
                if (io.ThreadState == ThreadState.Unstarted)
                    io.Start();
                if (io.ThreadState == ThreadState.Suspended)
                    io.Resume();
            }
        }
        private void addSeriesFromBlackBox(Chart theChart, BlackBox bb, Dictionary<string, object> ioValues, string legendPrefix)
        {
            foreach (var io in ioValues.ToList())
            {
                //Create series on chart
                string seriesName = legendPrefix + ":" + io.Key;
                Series theSeries = new Series(seriesName) { LegendText = seriesName, ChartType = SeriesChartType.FastLine };
                theChart.Series.Add(theSeries);

                //Add a thread to update the value on the chart
                Thread ioThread = new Thread
                (delegate ()
                {
                    while (true)
                    {
                        int t = bb.TimeCurrent_ms;
                        double ioValue = Convert.ToDouble(ioValues[io.Key]);

                        //Wait until on the correct thread
                        theChart.BeginInvoke((MethodInvoker)delegate {
                            theSeries.Points.AddXY(t, ioValue);
                        });

                        Thread.Sleep(chartUpdateInterval_ms);
                    }
                });
                //ioThread.IsBackground = true;

                //Add thread to list and start
                ioThreads.Add(ioThread);
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            foreach (Thread t in ioThreads)
                t.Suspend();

            foreach (BlackBox bb in BlackBoxOptions)
                bb.Stop();
        }
        private void addInputToForm(BlackBox bb, string title, string inputKey, double defaultValue)
        {
            //Create controls
            Label inputLabel = new Label() { Text = title };
            TextBox inputBox = new TextBox() { Text = defaultValue.ToString() };
            inputBox.TextChanged += delegate {
                try
                {
                    bb.Input[inputKey] = Convert.ToDouble(inputBox.Text);
                }
                catch { }
            };

            //Find next row
            layoutBBInputs.RowCount += 1;
            int currentRow = layoutBBInputs.RowCount-1;

            //Add controls
            layoutBBInputs.Controls.Add(inputLabel);
            layoutBBInputs.Controls.Add(inputBox);

            //Change position
            layoutBBInputs.RowStyles[1].SizeType = SizeType.AutoSize;
            layoutBBInputs.SetRow(inputLabel, currentRow);
            layoutBBInputs.SetColumn(inputLabel, 0);
            layoutBBInputs.SetRow(inputBox, currentRow);
            layoutBBInputs.SetColumn(inputBox, 1);
        }

        private void cmbBlackBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Stop all existing black boxes
            foreach (var b in BlackBoxOptions)
                b.Stop();

            //Stop and remove all ioThreads
            foreach (var io in ioThreads)
                    io.Abort();
            ioThreads.Clear();

            //Remove existing inputs
            layoutBBInputs.Controls.Clear();
            layoutBBInputs.RowCount = 0;

            //Retrieve selected black box
            BlackBox bb = (BlackBox) cmbBlackBoxes.SelectedItem;

            //Add controls to form
            foreach (var input in bb.Input)
                addInputToForm(bb, input.Key, input.Key, 0);

            //Add series to chart
            chartResults.Series.Clear();
            addSeriesFromBlackBox(chartResults, bb, bb.Input, "i");
            addSeriesFromBlackBox(chartResults, bb, bb.Output, "o");
        }

        
    }
}
