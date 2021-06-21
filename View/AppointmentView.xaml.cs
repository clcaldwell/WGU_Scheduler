using System.Windows.Controls;

using Scheduler.ViewModel;

using Scheduler.XCalendar;

namespace Scheduler.View
{
    public partial class AppointmentView : UserControl
    {
        public AppointmentView()
        {
            this.DataContext = new AppointmentViewModel();
            InitializeComponent();
        }



        private void CustomerBox_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {

        }

        private void Calendar_DayChanged(object sender, DayChangedEventArgs e)
        {

        }
    }
}
