using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight.CommandWpf;

using Scheduler.Model.DBEntities;
using Scheduler.Properties;
using Scheduler.View;

namespace Scheduler.ViewModel
{
    public class LoginWindowViewModel : ViewModelBase
    {
        public const string LogFile = "logins.txt";

        private Window loginWindow;

        private string _userName;
        private string _password;

        public string UserName
        {
            get => _userName;
            set
            {
                if (!string.Equals(_userName, value))
                {
                    SetProperty(ref _userName, value);
                    OnPropertyChanged();
                }
            }

        }

        public string Password
        {
            get => _password;
            set
            {
                if (!string.Equals(_password, value))
                {
                    SetProperty(ref _password, value);
                    OnPropertyChanged();
                }
            }
        }

        public void ValidateLogin(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                LogLogin(false, Resources.MessageEmptyUserName);
                throw new Exception (Resources.MessageEmptyUserName);
            }
            if (string.IsNullOrEmpty(Password))
            {
                LogLogin(false, Resources.MessageEmptyPassword);
                throw new Exception (Resources.MessageEmptyPassword);
            }

            //successful login - placed here to allow for faster login if no errors
            if (AllUsers.Exists(usr => usr.UserName == UserName && usr.Password == Password))
            {
                return;
            }

            if (!AllUsers.Exists(usr => usr.UserName == UserName))
            {
                LogLogin(false, Resources.MessageUserDoesNotExist);
                throw new Exception(Resources.MessageUserDoesNotExist);
            }
            if (AllUsers.Exists(usr => usr.UserName == UserName && usr.Password != Password))
            {
                LogLogin(false, Resources.MessageWrongPassword);
                throw new Exception(Resources.MessageWrongPassword);
            }

        }

        public void LogLogin(bool success, string reason = null)
        {
            var message = new string("");
            if (success)
            {
                message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ff} - Success: {UserName}";
            }
            else if (!success)
            {
                message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ff} - Failure: {UserName} ," + 
                    $" ErrorMessage: {reason}";
            }

            File.AppendAllText(LogFile, message + Environment.NewLine);
        }
        
        public RelayCommand<string> LoginCommand { get; private set; }
        
        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<string>(TryLogin);
        }

        public void TryLogin(string login)
        {
            try
            {
                ValidateLogin(UserName, Password);
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            LogLogin(true);

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

        public List<User> AllUsers
        {
            get
            {
                var context = new DBContext();
                List<User> users = context.User.ToList();
                return users;
            }
            set
            {
                var context = new DBContext();
                context.User.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

    }
}