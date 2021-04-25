using System;
using System.Windows;

using GalaSoft.MvvmLight.CommandWpf;

using Scheduler.Properties;
using Scheduler.View;

namespace Scheduler.ViewModel
{
    public class LoginWindowViewModel : BindBase
    {
        private Window loginWindow;
        private string _UserName;
        public string UserName
        {
            get
            {
                return this._UserName;
            }

            set
            {
                if (!string.Equals(this._UserName, value))
                {
                    this._UserName = value;
                }
            }

        }
        private string _Password;
        public string Password
        {
            get
            {
                return this._Password;
            }

            set
            {
                if (!string.Equals(this._Password, value))
                {
                    this._Password = value;
                }
            }
        }

        public void ValidateLogin(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new Exception(Resources.MessageEmptyUserName);
            }
            if (string.IsNullOrEmpty(Password))
            {
                throw new Exception(Resources.MessageEmptyPassword);
            }
        }

        public RelayCommand<string> LoginCommand { get; private set; }

        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<string>(Login);
        }

        public void Login(string login)
        {
            //ValidateLogin(UserName, Password);
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