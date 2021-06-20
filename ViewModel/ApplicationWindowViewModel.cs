using GalaSoft.MvvmLight.Command;

namespace Scheduler.ViewModel
{

    public class ApplicationWindowViewModel : ViewModelBase
    {
        private AppointmentViewModel _appointmentViewModel = new AppointmentViewModel();
        private CustomerViewModel _customerViewModel = new CustomerViewModel();
        private ReportViewModel _reportViewModel = new ReportViewModel();

        public ApplicationWindowViewModel()
        {
            NavCommand = new RelayCommand<string>(OnNav);
        }

        private ViewModelBase _CurrentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set { SetProperty(ref _CurrentViewModel, value); }
        }

        public RelayCommand<string> NavCommand { get; private set; }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "Appointment":
                    CurrentViewModel = _appointmentViewModel;
                    break;
                case "Customer":
                    CurrentViewModel = _customerViewModel;
                    break;
                case "Report":
                    CurrentViewModel = _reportViewModel;
                    break;
            }
        }

    }
}