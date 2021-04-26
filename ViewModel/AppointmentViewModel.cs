using System.Collections.Generic;
using System.Linq;

using GalaSoft.MvvmLight.CommandWpf;

using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class AppointmentViewModel : BindBase
    {
        public List<Appointment> AllAppointments {
            get
            {
                var context = new DBContext();
                return context.Appointment.ToList();
            }
            set
            {
                var context = new DBContext();
                context.Appointment.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

        public RelayCommand<string> GetAppointmentsCommand { get; private set; }

    }
}
