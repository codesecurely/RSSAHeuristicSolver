using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace RSAHeuristicSolver
{
    class ChartGenerator
    {
        public void TestChart()
        {
            // set up some data
            var xvals = new[]
                {
                    new DateTime(2012, 4, 4),
                    new DateTime(2012, 4, 5),
                    new DateTime(2012, 4, 6),
                    new DateTime(2012, 4, 7)
                };
            var yvals = new[] { 1, 3, 7, 12 };

            // create the chart
            var chart = new Chart();
            chart.Size = new Size(600, 250);

            var chartArea = new ChartArea();
            chartArea.AxisX.LabelStyle.Format = "dd/MMM\nhh:mm";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.LabelStyle.Font = new Font("Consolas", 8);
            chartArea.AxisY.LabelStyle.Font = new Font("Consolas", 8);
            chart.ChartAreas.Add(chartArea);

            var series = new Series();
            series.Name = "Series1";
            series.ChartType = SeriesChartType.FastLine;
            series.XValueType = ChartValueType.DateTime;
            chart.Series.Add(series);

            // bind the datapoints
            chart.Series["Series1"].Points.DataBindXY(xvals, yvals);

            // copy the series and manipulate the copy
            chart.DataManipulator.CopySeriesValues("Series1", "Series2");
            chart.DataManipulator.FinancialFormula(
                FinancialFormula.WeightedMovingAverage,
                "Series2"
            );
            chart.Series["Series2"].ChartType = SeriesChartType.FastLine;

            // draw!
            chart.Invalidate();

            // write out a file
            chart.SaveImage("chart.png", ChartImageFormat.Png);
        }
        public void GenerateChart(List<ResultHelper> resultsSA, List<ResultHelper> resultsGreedy, string chartName, string measureType, string XAxisTitle)
        {

            string YAxisTitle = null;
            // set up some data
            string[] xvals = new string[resultsSA.Count];
            for (int i = 0; i < xvals.Length; i++)
                xvals[i] = resultsSA[i].Label;

            double[] yvalsSA = new double[resultsSA.Count];
            double[] yvalsGreedy = new double[resultsGreedy.Count];

            if (measureType.Equals("energy"))
            {
                for (int i = 0; i < yvalsSA.Length; i++)
                    yvalsSA[i] = resultsSA[i].AvgEnergy;

                for (int i = 0; i < yvalsGreedy.Length; i++)
                    yvalsGreedy[i] = resultsGreedy[i].AvgEnergy;

                YAxisTitle = "Maksymalna liczba zaalokowanych slotów";
            }
            else if (measureType.Equals("sum"))
            {
                for (int i = 0; i < yvalsSA.Length; i++)
                    yvalsSA[i] = resultsSA[i].AvgSum;

                for (int i = 0; i < yvalsGreedy.Length; i++)
                    yvalsGreedy[i] = resultsGreedy[i].AvgSum;

                YAxisTitle = "Suma zaalokowanych slotów";
            }
            else if (measureType.Equals("avg"))
            {
                for (int i = 0; i < yvalsSA.Length; i++)
                    yvalsSA[i] = resultsSA[i].AvgAvg;

                for (int i = 0; i < yvalsGreedy.Length; i++)
                    yvalsGreedy[i] = resultsGreedy[i].AvgAvg;

                YAxisTitle = "Średnia zaalokowanych slotów";
            }

            // create the chart
            var chart = new Chart();
            chart.Size = new Size(800, 700);
            chart.Name = "Wykres";



            var chartArea = new ChartArea();
            chartArea.Name = "Wykres";
            chartArea.AxisX.LabelStyle.Format = "F";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            chartArea.AxisX.LabelStyle.Font = new Font("Consolas", 10);
            chartArea.AxisY.LabelStyle.Font = new Font("Consolas", 10);

            chartArea.AxisX.TitleFont = new Font("Consolas", 12);
            chartArea.AxisY.TitleFont = new Font("Consolas", 12);

            chartArea.AxisX.Title = XAxisTitle;
            chartArea.AxisY.Title = YAxisTitle;

            chartArea.AxisX.Interval = 1;

            chart.ChartAreas.Add(chartArea);

            var greedy = new Series();
            greedy.Name = "Greedy";
            greedy.ChartType = SeriesChartType.Bar;
            greedy.XValueType = ChartValueType.Auto;
            chart.Series.Add(greedy);

            var SA = new Series();
            SA.Name = "SA";
            SA.ChartType = SeriesChartType.Bar;
            SA.XValueType = ChartValueType.Auto;
            chart.Series.Add(SA);

            // bind the datapoints
            chart.Series["Greedy"].Points.DataBindXY(xvals, yvalsGreedy);
            chart.Series["SA"].Points.DataBindXY(xvals, yvalsSA);

            // Create a new legend called "Legend2".
            chart.Legends.Add(new Legend("Legend2"));

            // Set Docking of the Legend chart to the Default Chart Area.
            chart.Legends["Legend2"].DockedToChartArea = "Wykres";

            // Assign the legend to Series1.
            chart.Series["Greedy"].Legend = "Legend2";
            chart.Series["Greedy"].IsVisibleInLegend = true;
            // draw!
            chart.Invalidate();

            // write out a file
            chart.SaveImage(chartName+".png", ChartImageFormat.Png);
        }
    }
    }

