//using OxyPlot;
//using OxyPlot.Series;
//using System;
//using System.Collections.Generic;
//using System.Windows.Threading;

//namespace PendulumSimulation
//{
//    public class PendulumSimulator
//    {
//        public double Time { get; private set; } = 0.0;
//        public double Angle { get; private set; } = 0.0;
//        public double Dt { get; private set; } = 0.01;
//        public double Gravity { get; private set; } = 9.8;
//        public double Length { get; private set; } = 1.0;
//        public double InitialAngle { get; private set; } = 0.0;

//        public LineSeries Series { get; } = new LineSeries { Title = "Angle", MarkerType = MarkerType.None };
//        private DispatcherTimer Timer { get; } = new DispatcherTimer();

//        private Pendulum2DView pendulum2DView;
//        private Pendulum3DModel pendulum3DModel;
//        private PlotInitializer plotInitializer;

//        public PendulumSimulator(Pendulum2DView pendulum2DView, Pendulum3DModel pendulum3DModel, PlotInitializer plotInitializer)
//        {
//            this.pendulum2DView = pendulum2DView;
//            this.pendulum3DModel = pendulum3DModel;
//            this.plotInitializer = plotInitializer;

//            Timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 500);
//            Timer.Tick += Timer_Tick;
//        }

//        public void Start(double length, double gravity, double initialAngle, double dt)
//        {
//            Length = length;
//            Gravity = gravity;
//            InitialAngle = initialAngle;
//            Dt = dt;

//            Angle = InitialAngle;
//            Time = 0.0;
//            Series.Points.Clear();

//            Timer.Start();
//        }

//        public void Stop()
//        {
//            Timer.Stop();
//        }

//        public void Reset()
//        {
//            Stop();
//            Time = 0.0;
//            Angle = InitialAngle;
//            Series.Points.Clear();
//        }

//        public List<(double Time, double Angle)> GetPoints()
//        {
//            var points = new List<(double Time, double Angle)>();
//            foreach (var point in Series.Points)
//            {
//                points.Add((point.X, point.Y));
//            }
//            return points;
//        }

//        private void Timer_Tick(object? sender, EventArgs e)
//        {
//            Time += Dt;
//            Angle = InitialAngle * Math.Cos(Math.Sqrt(Gravity / Length) * Time);

//            Series.Points.Add(new DataPoint(Time, Angle)); // Изменение 1: Удаление лишней переменной

//            pendulum2DView.UpdatePendulum(Angle);
//            pendulum3DModel.UpdatePendulum(Angle);
//            plotInitializer.UpdatePlot();
//        }

//        public void SetView(Pendulum2DView pendulum2DView, Pendulum3DModel pendulum3DModel, PlotInitializer plotInitializer)
//        {
//            this.pendulum2DView = pendulum2DView;
//            this.pendulum3DModel = pendulum3DModel;
//            this.plotInitializer = plotInitializer;
//        }
//    }
//}
