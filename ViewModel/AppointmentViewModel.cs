using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight.CommandWpf;

using Microsoft.EntityFrameworkCore;

using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class AppointmentViewModel : ViewModelBase
    {
        private Appointment _selectedappointment;

        private bool _addMode = false;

        private bool _editMode = false;

        private bool _viewMode = true;

        private List<Appointment> _allappointmentsloaded;
        private Customer _selectedcustomer;

        enum Mode
        {
            Add,
            Edit,
            View
        }

        private void SetMode(Mode mode)
        {
            if (mode == Mode.Add)
            {
                EditMode = true;
                ViewMode = false;
            }
            if (mode == Mode.Edit)
            {
                EditMode = true;
                ViewMode = false;
            }
            if (mode == Mode.View)
            {
                EditMode = false;
                ViewMode = true;
            }
        }

        private void OnAddButton(Appointment appointment)
        {
            SetMode(Mode.Add);
        }

        private void OnEditButton(Appointment appointment)
        {
            SetMode(Mode.Edit);
        }

        private void OnDeleteButton(Appointment appointment)
        {
            if (MessageBox.Show("Are you sure you want to delete this appointment?" +
                    "\r\n Id:" + appointment.AppointmentId +
                    "\r\n Title:" + appointment.Title +
                    "\r\n Location:" + appointment.Location +
                    "\r\n Contact:" + appointment.Contact +
                    "\r\n Start:" + appointment.Start.ToString() +
                    "\r\n End:" + appointment.End.ToString(),
                "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var context = new DBContext();
                context.Remove(appointment);
                context.SaveChanges();
                SetMode(Mode.View);
                LoadAppointments();
            }
        }

        private void OnSaveButton(Appointment appointment)
        {
            var context = new DBContext();
            if (AddMode)
            {
                int NextId = AllAppointments.OrderByDescending(a => a.AppointmentId).FirstOrDefault().AppointmentId + 1;
                MessageBox.Show("NextAppointmentId: " + NextId);
                /* Appointment NewAppointment = new Appointment
                {
                    AppointmentId = NextId,
                    CustomerId = appointment.CustomerId,
                    UserId = appointment.UserId,
                    Title = appointment.Title,
                    Description = appointment.Description,
                    Location = appointment.Location,
                    Contact = appointment.Contact,
                    Type = appointment.Type,
                    Url = appointment.Url,
                    Start = appointment.Start,
                    End = appointment.End,
                    CreateDate = appointment.CreateDate,
                    CreatedBy = appointment.CreatedBy,
                    LastUpdate = appointment.LastUpdate,
                    LastUpdateBy = appointment.LastUpdateBy
                };
 
                context.Add(NewAppointment); */
            }
            else
            {
                context.Update(appointment);
            }

            context.SaveChanges();
            LoadAppointments();
            SetMode(Mode.View);
        }

        private void OnCancelButton(Appointment appointment)
        {
            LoadAppointments();
            SetMode(Mode.View);
        }

        public AppointmentViewModel()
        {
            AddAppointmentCommand = new RelayCommand<Appointment>(OnAddButton);
            EditAppointmentCommand = new RelayCommand<Appointment>(OnEditButton);
            DeleteAppointmentCommand = new RelayCommand<Appointment>(OnDeleteButton);

            SaveAppointmentCommand = new RelayCommand<Appointment>(OnSaveButton);
            CancelAppointmentCommand = new RelayCommand<Appointment>(OnCancelButton);
        }

        public RelayCommand<string> GetAppointmentsCommand { get; private set; }
        public RelayCommand<Appointment> AddAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> EditAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> DeleteAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> SaveAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> CancelAppointmentCommand { get; private set; }

        public bool AddMode
        {
            get { return _addMode; }
            set
            {
                _viewMode = value;
                OnPropertyChanged(nameof(AddMode));
            }
        }

        public bool ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                OnPropertyChanged(nameof(ViewMode));
            }
        }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                OnPropertyChanged(nameof(EditMode));
            }
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

        public List<Appointment> AllAppointmentsLoaded
        {
            get { return _allappointmentsloaded; }
            set { SetProperty(ref _allappointmentsloaded, value); }
        }

        public async void LoadAppointments()
        {
            var context = new DBContext();
            AllAppointmentsLoaded = await context.Appointment.ToListAsync();
        }

        public Appointment SelectedAppointment
        {
            get { return _selectedappointment; }
            set
            {
                if (value != null && value != _selectedappointment)
                {
                    SetProperty(ref _selectedappointment, value);

                    var context = new DBContext();
                    SelectedCustomer = context.Customer.Find(value.CustomerId);

                    OnPropertyChanged(nameof(SelectedAppointment));
                }
            }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedcustomer; }
            set
            {
                if (value != null && value != _selectedcustomer)
                {
                    SetProperty(ref _selectedcustomer, value);

                    if (SelectedAppointment != null && SelectedAppointment.CustomerId != value.CustomerId)
                    {
                        SelectedAppointment.CustomerId = value.CustomerId;
                    }

                    OnPropertyChanged(nameof(SelectedCustomer));
                }
            }
        }

        public ObservableCollection<Customer> AllCustomers
        {
            get
            {
                var context = new DBContext();
                return new ObservableCollection<Customer>(context.Customer.ToList());
            }
            set { }
        }

        private int _customerIndex;

        public int CustomerIndex { get => _customerIndex; set => SetProperty(ref _customerIndex, value); }

    }
}
