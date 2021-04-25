using System.Windows;

using GalaSoft.MvvmLight.CommandWpf;

using Scheduler.View;

namespace Scheduler.ViewModel
{
    public class LoginWindowViewModel : BindBase
    {
        private Window loginWindow;

        public RelayCommand<string> LoginCommand { get; private set; }

        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<string>(Login);
        }

        public void Login(string login)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                {
                   loginWindow = window;
                }
            }

            Window AppWindow = new ApplicationWindow();
            AppWindow.Show();
            loginWindow.Close();
            AppWindow.Focus();
        }

    }
}