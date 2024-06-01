using ExcelExport;
using System.Globalization;
using System.Windows;
using WordExport;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Series;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System.Drawing;
using System.IO;

namespace PendulumSimulation
{
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
