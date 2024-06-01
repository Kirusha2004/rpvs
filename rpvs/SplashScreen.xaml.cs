using PendulumSimulation;
using System;
using System.Windows;

namespace PendulumSimulation
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SplashScreenWindow splashScreen = new SplashScreenWindow(); // Создаем экземпляр SplashScreenWindow
        splashScreen.Closed += SplashScreen_Closed; // Добавляем обработчик события закрытия
        splashScreen.Show(); // Показываем SplashScreenWindow
    }

    private void SplashScreen_Closed(object sender, EventArgs e)
    {
        MainWindow mainWindow = new MainWindow(); // Создаем экземпляр MainWindow
        mainWindow.Show(); // Показываем MainWindow только после закрытия SplashScreenWindow
    }
}
