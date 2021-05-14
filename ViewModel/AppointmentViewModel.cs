using System.Collections.Generic;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight.CommandWpf;

using Microsoft.EntityFrameworkCore;

using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class AppointmentViewModel : ViewModelBase
    {

        public AppointmentViewModel()
        {
            EditAppointmentCommand = new RelayCommand<Appointment>(OnEditButton);
            SaveAppointmentCommand = new RelayCommand<Appointment>(OnSaveButton);
        }

        private void OnEditButton(Appointment appointment)
        {
            MessageBox.Show(SelectedAppointment.AppointmentId.ToString());
        }

        private void OnSaveButton(Appointment appointment)
        {
            var context = new DBContext();
            context.Update(appointment);
            context.SaveChanges();
        }



        public List<Appointment> AllAppointments
        {
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

        private List<Appointment> _allappointmentsloaded;
        public List<Appointment> AllAppointmentsLoaded
        {
            get { return _allappointmentsloaded;  }
            set { SetProperty(ref _allappointmentsloaded, value); }
        }

        public async void LoadAppointments()
        {
            var context = new DBContext();
            AllAppointmentsLoaded = await context.Appointment.ToListAsync();
        }

        public RelayCommand<string> GetAppointmentsCommand { get; private set; }
        public RelayCommand<Appointment> EditAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> SaveAppointmentCommand { get; private set; }

        private Appointment _selectedappointment;
        public Appointment SelectedAppointment
        {
            get { return _selectedappointment; }
            set
            {
                if (value != null && value != _selectedappointment)
                {
                    SetProperty(ref _selectedappointment, value);
                    OnPropertyChanged("SelectedAppointment");
                }
            }
        }
    }
}
