using System.Windows.Controls;

using Scheduler.ViewModel;

namespace Scheduler.View
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            this.DataContext = new LoginViewModel();
            InitializeComponent();
        }
    }
}
