using System.Windows.Controls;
using System.Windows.Shapes;
using System;
using System.Windows.Media;

namespace PendulumSimulation
{
    public class Pendulum2DView
    {
        private Line pendulumLine;
        private Ellipse pendulumBall;
        private double length;
        private Canvas PendulumCanvas;

        public Pendulum2DView(Canvas canvas, double length)
        {
            this.length = length;
            this.PendulumCanvas = canvas;
            Initialize2DPendulum(canvas);
        }

        private void Initialize2DPendulum(Canvas canvas)
        {
            pendulumLine = new Line
            {
                Stroke = new LinearGradientBrush(System.Windows.Media.Colors.Gray, System.Windows.Media.Colors.White, 90),
                StrokeThickness = 2
            };

            Canvas.SetLeft(pendulumLine, canvas.ActualWidth / 2);
            Canvas.SetTop(pendulumLine, canvas.ActualHeight / 2);

            pendulumBall = new Ellipse
            {
                Fill = new RadialGradientBrush(System.Windows.Media.Colors.Red, System.Windows.Media.Colors.DarkRed),
                Width = 20,
                Height = 20
            };

            Canvas.SetLeft(pendulumBall, canvas.ActualWidth / 2 - 10);
            Canvas.SetTop(pendulumBall, canvas.ActualHeight / 2 - 10);

            canvas.Children.Add(pendulumLine);
            canvas.Children.Add(pendulumBall);
        }

        public void UpdatePendulum(double angle)
        {
            double x = length * Math.Sin(angle) * 100;
            double y = length * Math.Cos(angle) * 100;

            pendulumLine.X1 = PendulumCanvas.ActualWidth / 2;
            pendulumLine.Y1 = PendulumCanvas.ActualHeight / 2;
            pendulumLine.X2 = PendulumCanvas.ActualWidth / 2 + x;
            pendulumLine.Y2 = PendulumCanvas.ActualHeight / 2 + y;

            Canvas.SetLeft(pendulumBall, PendulumCanvas.ActualWidth / 2 + x - 10);
            Canvas.SetTop(pendulumBall, PendulumCanvas.ActualHeight / 2 + y - 10);
        }

        public void UpdateLength(double length)
        {
            this.length = length;
        }
    }
}
