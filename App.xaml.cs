using System.Windows;

using Scheduler.View;

namespace Scheduler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Window AppWindow = new LoginWindow();
            AppWindow.Show();
        }
    }
}
