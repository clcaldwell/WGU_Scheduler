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

        private string _UserName;
        private string _Password;

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
                LogLoginFailure(Resources.MessageEmptyUserName);
                throw new Exception (Resources.MessageEmptyUserName);
            }
            if (string.IsNullOrEmpty(Password))
            {
                LogLoginFailure(Resources.MessageEmptyPassword);
                throw new Exception (Resources.MessageEmptyPassword);
            }

            //successful login - placed here to allow for faster login if no errors
            if (AllUsers.Exists(usr => usr.UserName == UserName && usr.Password == Password))
            {
                return;
            }

            if (!AllUsers.Exists(usr => usr.UserName == UserName))
            {
                LogLoginFailure(Resources.MessageUserDoesNotExist);
                throw new Exception(Resources.MessageUserDoesNotExist);
            }
            if (AllUsers.Exists(usr => usr.UserName == UserName && usr.Password != Password))
            {
                LogLoginFailure(Resources.MessageWrongPassword);
                throw new Exception(Resources.MessageWrongPassword);
            }

        }

        public void LogLoginFailure(string Reason)
        {
            File.AppendAllText(LogFile,
                $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")} - Failure: {UserName} , ErrorMessage: {Reason}" + Environment.NewLine
            );
        }

        public void LogLoginSuccess()
        {
            File.AppendAllText(LogFile,
                $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")} - Success: {UserName}" + Environment.NewLine
            );
        }
        
        public RelayCommand<string> LoginCommand { get; private set; }
        
        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<string>(Login);
        }

        public void Login(string login)
        {
            try
            {
                ValidateLogin(UserName, Password);
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            LogLoginSuccess();
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