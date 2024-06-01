using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace PendulumSimulation
{
    public class Pendulum3DModel
    {
        private ModelVisual3D pendulumModel = new ModelVisual3D();
        private RotateTransform3D rotation = new RotateTransform3D();
        private AxisAngleRotation3D axisRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);

        public Pendulum3DModel(HelixViewport3D viewport, double length)
        {
            Initialize3DModel(viewport, length);
        }

        private void Initialize3DModel(HelixViewport3D viewport, double length)
        {
            var xAxis = new PipeVisual3D { Point1 = new Point3D(-2, 0, 0), Point2 = new Point3D(2, 0, 0), Diameter = 0.01, Fill = System.Windows.Media.Brushes.White };
            var xAxisLabel = new TextVisual3D { Position = new Point3D(2, 0, 0), Text = "X Axis", Foreground = System.Windows.Media.Brushes.White };

            var yAxis = new PipeVisual3D { Point1 = new Point3D(0, -2, 0), Point2 = new Point3D(0, 2, 0), Diameter = 0.01, Fill = System.Windows.Media.Brushes.White };
            var yAxisLabel = new TextVisual3D { Position = new Point3D(0, 2, 0), Text = "Y Axis", Foreground = System.Windows.Media.Brushes.White };

            var zAxis = new PipeVisual3D { Point1 = new Point3D(0, 0, -2), Point2 = new Point3D(0, 0, 2), Diameter = 0.01, Fill = System.Windows.Media.Brushes.White };
            var zAxisLabel = new TextVisual3D { Position = new Point3D(0, 0, 2), Text = "Z Axis", Foreground = System.Windows.Media.Brushes.White };

            viewport.Children.Add(xAxis);
            viewport.Children.Add(xAxisLabel);
            viewport.Children.Add(yAxis);
            viewport.Children.Add(yAxisLabel);
            viewport.Children.Add(zAxis);
            viewport.Children.Add(zAxisLabel);

            var rod = new PipeVisual3D
            {
                Point1 = new Point3D(0, 0, 0),
                Point2 = new Point3D(0, -length, 0),
                Diameter = 0.02,
                Fill = new LinearGradientBrush(System.Windows.Media.Colors.DarkRed, System.Windows.Media.Colors.White, 90)
            };
            var ball = new SphereVisual3D
            {
                Center = new Point3D(0, -length, 0),
                Radius = 0.1,
                Fill = new RadialGradientBrush(System.Windows.Media.Colors.DarkRed, System.Windows.Media.Colors.White)
            };

            pendulumModel.Children.Clear();
            pendulumModel.Children.Add(rod);
            pendulumModel.Children.Add(ball);

            rotation.Rotation = axisRotation;
            pendulumModel.Transform = rotation;

            viewport.Children.Add(pendulumModel);
        }

        public void UpdateLength(double length)
        {
            var rod = pendulumModel.Children[0] as PipeVisual3D;
            if (rod != null)
            {
                rod.Point2 = new Point3D(0, -length, 0);
            }

            var ball = pendulumModel.Children[1] as SphereVisual3D;
            if (ball != null)
            {
                ball.Center = new Point3D(0, -length, 0);
            }
        }

        public void UpdatePendulum(double angle)
        {
            axisRotation.Angle = angle * 180 / Math.PI;
        }
    }
}
