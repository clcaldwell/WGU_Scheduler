using System.Windows.Controls;

using Scheduler.ViewModel;

namespace Scheduler.View
{
    public partial class AppointmentView : UserControl
    {
        public AppointmentView()
        {
            this.DataContext = new AppointmentViewModel();
            InitializeComponent();
        }

        private void customerBox_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {

        }
    }
}
