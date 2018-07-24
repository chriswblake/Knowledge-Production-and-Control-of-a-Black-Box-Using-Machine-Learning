namespace TestingForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.layoutInput = new System.Windows.Forms.TableLayoutPanel();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBlackBoxes = new System.Windows.Forms.ComboBox();
            this.layoutBBInputs = new System.Windows.Forms.TableLayoutPanel();
            this.chartResults = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.input1 = new System.Windows.Forms.Label();
            this.input2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tableLayout.SuspendLayout();
            this.layoutInput.SuspendLayout();
            this.layoutBBInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartResults)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Controls.Add(this.layoutInput, 0, 0);
            this.tableLayout.Controls.Add(this.chartResults, 1, 0);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayout.Size = new System.Drawing.Size(1879, 1014);
            this.tableLayout.TabIndex = 0;
            // 
            // layoutInput
            // 
            this.layoutInput.AutoSize = true;
            this.layoutInput.ColumnCount = 2;
            this.layoutInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutInput.Controls.Add(this.btnStart, 0, 0);
            this.layoutInput.Controls.Add(this.btnStop, 1, 0);
            this.layoutInput.Controls.Add(this.label1, 0, 1);
            this.layoutInput.Controls.Add(this.cmbBlackBoxes, 0, 2);
            this.layoutInput.Controls.Add(this.layoutBBInputs, 0, 3);
            this.layoutInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutInput.Location = new System.Drawing.Point(3, 3);
            this.layoutInput.Name = "layoutInput";
            this.layoutInput.RowCount = 5;
            this.tableLayout.SetRowSpan(this.layoutInput, 2);
            this.layoutInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutInput.Size = new System.Drawing.Size(404, 244);
            this.layoutInput.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(196, 68);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(205, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(196, 68);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.layoutInput.SetColumnSpan(this.label1, 2);
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 31);
            this.label1.TabIndex = 2;
            this.label1.Text = "Black Box Inputs";
            // 
            // cmbBlackBoxes
            // 
            this.layoutInput.SetColumnSpan(this.cmbBlackBoxes, 2);
            this.cmbBlackBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbBlackBoxes.FormattingEnabled = true;
            this.cmbBlackBoxes.Location = new System.Drawing.Point(3, 108);
            this.cmbBlackBoxes.Name = "cmbBlackBoxes";
            this.cmbBlackBoxes.Size = new System.Drawing.Size(398, 33);
            this.cmbBlackBoxes.TabIndex = 3;
            this.cmbBlackBoxes.SelectedIndexChanged += new System.EventHandler(this.cmbBlackBoxes_SelectedIndexChanged);
            // 
            // layoutBBInputs
            // 
            this.layoutBBInputs.AutoSize = true;
            this.layoutBBInputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layoutBBInputs.ColumnCount = 2;
            this.layoutInput.SetColumnSpan(this.layoutBBInputs, 2);
            this.layoutBBInputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutBBInputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutBBInputs.Controls.Add(this.input1, 0, 0);
            this.layoutBBInputs.Controls.Add(this.input2, 0, 1);
            this.layoutBBInputs.Controls.Add(this.textBox1, 1, 0);
            this.layoutBBInputs.Controls.Add(this.textBox2, 1, 1);
            this.layoutBBInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutBBInputs.Location = new System.Drawing.Point(3, 147);
            this.layoutBBInputs.Name = "layoutBBInputs";
            this.layoutBBInputs.RowCount = 2;
            this.layoutBBInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutBBInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutBBInputs.Size = new System.Drawing.Size(398, 74);
            this.layoutBBInputs.TabIndex = 4;
            // 
            // chartResults
            // 
            chartArea1.AxisX.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.Title = "Time [ms]";
            chartArea1.AxisY.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.Name = "ChartArea1";
            this.chartResults.ChartAreas.Add(chartArea1);
            this.chartResults.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartResults.Legends.Add(legend1);
            this.chartResults.Location = new System.Drawing.Point(413, 3);
            this.chartResults.Name = "chartResults";
            this.tableLayout.SetRowSpan(this.chartResults, 2);
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartResults.Series.Add(series1);
            this.chartResults.Size = new System.Drawing.Size(1463, 1008);
            this.chartResults.TabIndex = 1;
            this.chartResults.Text = "chart1";
            // 
            // input1
            // 
            this.input1.AutoSize = true;
            this.input1.Location = new System.Drawing.Point(3, 0);
            this.input1.Name = "input1";
            this.input1.Size = new System.Drawing.Size(70, 25);
            this.input1.TabIndex = 0;
            this.input1.Text = "label2";
            // 
            // input2
            // 
            this.input2.AutoSize = true;
            this.input2.Location = new System.Drawing.Point(3, 37);
            this.input2.Name = "input2";
            this.input2.Size = new System.Drawing.Size(70, 25);
            this.input2.TabIndex = 1;
            this.input2.Text = "label3";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(202, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 31);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(202, 40);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 31);
            this.textBox2.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1879, 1014);
            this.Controls.Add(this.tableLayout);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.layoutInput.ResumeLayout(false);
            this.layoutInput.PerformLayout();
            this.layoutBBInputs.ResumeLayout(false);
            this.layoutBBInputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.TableLayoutPanel layoutInput;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartResults;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBlackBoxes;
        private System.Windows.Forms.TableLayoutPanel layoutBBInputs;
        private System.Windows.Forms.Label input1;
        private System.Windows.Forms.Label input2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

