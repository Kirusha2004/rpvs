using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Timers; // Используйте System.Timers, а не System.Threading

namespace PendulumSimulationLibrary
{
    public class PendulumSimulator
    {
        public double Time { get; private set; } = 0.0;
        public double Angle { get; private set; } = 0.0;
        public double Dt { get; private set; } = 0.01;
        public double Gravity { get; private set; } = 9.8;
        public double Length { get; private set; } = 1.0;
        public double InitialAngle { get; private set; } = 0.0;

        public LineSeries Series { get; } = new LineSeries { Title = "Angle", MarkerType = OxyPlot.MarkerType.None };
        private System.Timers.Timer Timer { get; } = new System.Timers.Timer(); // Укажите полное имя класса

        public event Action<double> PendulumUpdated;
        public event Action PlotUpdated;

        public PendulumSimulator()
        {
            Timer.Interval = 1000.0 / 500;
            Timer.Elapsed += Timer_Tick;
        }

        public void Start(double length, double gravity, double initialAngle, double dt)
        {
            Length = length;
            Gravity = gravity;
            InitialAngle = initialAngle;
            Dt = dt;

            Angle = InitialAngle;
            Time = 0.0;
            Series.Points.Clear();

            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        public void Reset()
        {
            Stop();
            Time = 0.0;
            Angle = InitialAngle;
            Series.Points.Clear();
        }

        public List<(double Time, double Angle)> GetPoints()
        {
            var points = new List<(double Time, double Angle)>();
            foreach (var point in Series.Points)
            {
                points.Add((point.X, point.Y));
            }
            return points;
        }

        private void Timer_Tick(object? sender, ElapsedEventArgs e)
        {
            Time += Dt;
            Angle = InitialAngle * Math.Cos(Math.Sqrt(Gravity / Length) * Time);

            Series.Points.Add(new OxyPlot.DataPoint(Time, Angle));

            PendulumUpdated?.Invoke(Angle);
            PlotUpdated?.Invoke();
        }
    }
}
