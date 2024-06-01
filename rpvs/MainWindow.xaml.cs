using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ExcelExport;
using WordExport;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System.Drawing;
using System.Windows.Shapes;

using PendulumSimulationLibrary;
using OxyPlot.Wpf;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace PendulumSimulation
{
    public class PendulumSimulator
    {
        public double Time { get; private set; } = 0.0;
        public double Angle { get; private set; } = 0.0;
        public double Dt { get; private set; } = 0.01;
        public double Gravity { get; private set; } = 9.8;
        public double Length { get; private set; } = 1.0;
        public double InitialAngle { get; private set; } = 0.0;

        public LineSeries Series { get; } = new LineSeries { Title = "Angle", MarkerType = MarkerType.None };
        private DispatcherTimer Timer { get; } = new DispatcherTimer();

        private Pendulum2DView pendulum2DView;
        private Pendulum3DModel pendulum3DModel;
        private PlotInitializer plotInitializer;

        public PendulumSimulator(Pendulum2DView pendulum2DView, Pendulum3DModel pendulum3DModel, PlotInitializer plotInitializer)
        {
            this.pendulum2DView = pendulum2DView;
            this.pendulum3DModel = pendulum3DModel;
            this.plotInitializer = plotInitializer;

            Timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 500);
            Timer.Tick += Timer_Tick;
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

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Time += Dt;
            Angle = InitialAngle * Math.Cos(Math.Sqrt(Gravity / Length) * Time);

            Series.Points.Add(new DataPoint(Time, Angle));

            pendulum2DView.UpdatePendulum(Angle);
            pendulum3DModel.UpdatePendulum(Angle);
            plotInitializer.UpdatePlot();
        }

        public void SetView(Pendulum2DView pendulum2DView, Pendulum3DModel pendulum3DModel, PlotInitializer plotInitializer)
        {
            this.pendulum2DView = pendulum2DView;
            this.pendulum3DModel = pendulum3DModel;
            this.plotInitializer = plotInitializer;
        }
    }

    public partial class MainWindow : Window
    {
        private PendulumSimulator simulator;
        private PlotInitializer plotInitializer;
        private Pendulum3DModel pendulum3DModel;
        private Pendulum2DView pendulum2DView;
        private ExcelExporter excelExporter = new ExcelExporter();
        private WordExporter wordExporter = new WordExporter();

        public MainWindow()
        {
            var splashScreen = new SplashScreenWindow();
            splashScreen.ShowDialog();

            InitializeComponent();

            simulator = new PendulumSimulator(null, null, null);

            pendulum2DView = new Pendulum2DView(PendulumCanvas, simulator.Length);
            pendulum3DModel = new Pendulum3DModel(PendulumViewport, simulator.Length);

            plotInitializer = new PlotInitializer(simulator.Series, _plotView);

            simulator.SetView(pendulum2DView, pendulum3DModel, plotInitializer);
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                simulator.Start(
                    double.Parse(LengthInput.Text, CultureInfo.InvariantCulture),
                    double.Parse(GravityInput.Text, CultureInfo.InvariantCulture),
                    double.Parse(AngleInput.Text, CultureInfo.InvariantCulture) * Math.PI / 180.0,
                    double.Parse(DtInput.Text, CultureInfo.InvariantCulture)
                );
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            simulator.Stop();
        }

        private void ResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            simulator.Reset();
            plotInitializer.UpdatePlot();
            pendulum2DView.UpdatePendulum(simulator.Angle);
            pendulum3DModel.UpdatePendulum(simulator.Angle);
        }

        private void LengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LengthInput != null && pendulum2DView != null && pendulum3DModel != null)
            {
                double length = e.NewValue;
                LengthInput.Text = length.ToString("F2", CultureInfo.InvariantCulture);
                pendulum2DView.UpdateLength(length);
                pendulum3DModel.UpdateLength(length);
            }
        }

        private void GravitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GravityInput != null)
            {
                double gravity = e.NewValue;
                GravityInput.Text = gravity.ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        private void AngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AngleInput != null)
            {
                double initialAngle = e.NewValue;
                AngleInput.Text = initialAngle.ToString("F1", CultureInfo.InvariantCulture);
            }
        }

        private void DtSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DtInput != null)
            {
                double dt = e.NewValue;
                DtInput.Text = dt.ToString("F3", CultureInfo.InvariantCulture);
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = @"C:\Users\yodas\source\repos\rpvs\rpvs\pendulum_data.xlsx";
                var points = simulator.GetPoints();
                var plotImage = CapturePlotImage();
                excelExporter.ExportPointsToExcel(points, filePath, plotImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = @"C:\Users\yodas\source\repos\rpvs\rpvs\pendulum_data.docx";
                var points = simulator.GetPoints();
                var plotImage = CapturePlotImage();
                wordExporter.ExportPointsToWord(points, filePath, plotImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private Bitmap CapturePlotImage()
        {
            var exporter = new PngExporter { Width = 600, Height = 400 };
            using (var stream = new MemoryStream())
            {
                exporter.Export(_plotView.Model, stream);
                stream.Position = 0;
                return new Bitmap(stream);
            }
        }

        private void ContactEmail_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://mail.google.com/mail/u/0/?view=cm&fs=1&to=yodas7212@gmail.com",
                UseShellExecute = true
            });
        }

        private void ContactTelegram_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://t.me/Kirusha_2004",
                UseShellExecute = true
            });
        }
    }
}
