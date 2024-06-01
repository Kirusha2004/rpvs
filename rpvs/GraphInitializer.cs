using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Drawing;
using System.IO;

namespace PendulumSimulation
{
    public class PlotInitializer
    {
        private PlotView plotView;
        private LineSeries series;

        public PlotInitializer(LineSeries series, PlotView plotView)
        {
            this.series = series;
            this.plotView = plotView;
            InitializeGraph();
        }

        private void InitializeGraph()
        {
            var model = new PlotModel { Title = "Pendulum Oscillation Graph" };

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time (s)",
                AxislineColor = OxyColors.Black,
                TextColor = OxyColors.Black,
                TitleColor = OxyColors.Black
            };
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Angle (Radians)",
                AxislineColor = OxyColors.Black,
                TextColor = OxyColors.Black,
                TitleColor = OxyColors.Black
            };

            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);

            model.PlotAreaBorderColor = OxyColors.White;
            model.TextColor = OxyColors.White;
            model.Background = OxyColors.White;

            model.Series.Add(series);

            plotView.Model = model;
        }

        public void UpdatePlot()
        {
            plotView.Model.InvalidatePlot(true); // Update the plot model
        }
    }
}
