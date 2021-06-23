using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        private bool _gridDisplay = true;

        private bool _calenderByMonthDisplay = false;

        private bool _calenderByWeekDisplay = false;

        private List<Appointment> _allappointmentsloaded;
        private Customer _selectedcustomer;

        enum Mode
        {
            Add,
            Edit,
            View
        }

        enum Display
        {
            Grid,
            CalenderByMonth,
            CalendarByWeek
        }

        private void SetMode(Mode mode)
        {
            if (mode == Mode.Add)
            {
                AddMode = true;
                EditMode = true;
                ViewMode = false;
                SelectedAppointment = null;
                ModifyAppointmentSelected = true;
                CanSave = true;
            }
            if (mode == Mode.Edit)
            {
                EditMode = true;
                ViewMode = false;
                ModifyAppointmentSelected = true;
                CanSave = true;
            }
            if (mode == Mode.View)
            {
                EditMode = false;
                AddMode = false;
                ViewMode = true;
                CanSave = false;
            }
        }

        private void SetDisplay(Display display)
        {
            if (display == Display.Grid)
            {
                GridDisplay = true;
                CalenderByMonthDisplay = false;
                CalenderByWeekDisplay = false;
            }
            if (display == Display.CalenderByMonth)
            {
                GridDisplay = false;
                CalenderByMonthDisplay = true;
                CalenderByWeekDisplay = false;
            }
            if (display == Display.CalendarByWeek)
            {
                GridDisplay = false;
                CalenderByMonthDisplay = false;
                CalenderByWeekDisplay = true;
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
                    "\r\n Start:" + appointment.Start.ToLocalTime().ToString() +
                    "\r\n End:" + appointment.End.ToLocalTime().ToString(),
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
                Appointment NewAppointment = new Appointment
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
                    CreateDate = appointment.CreateDate.ToUniversalTime(),
                    CreatedBy = appointment.CreatedBy,
                    LastUpdate = appointment.LastUpdate.ToUniversalTime(),
                    LastUpdateBy = appointment.LastUpdateBy
                };

                // Set any string null properties to empty string
                foreach (var propertyInfo in NewAppointment.GetType().GetProperties())
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        if (propertyInfo.GetValue(NewAppointment, null) == null)
                        {
                            propertyInfo.SetValue(NewAppointment, string.Empty, null);
                        }
                    }
                }

                context.Add(NewAppointment);
            }
            else
            {
                appointment.Start = appointment.Start.ToUniversalTime();
                appointment.End = appointment.End.ToUniversalTime();
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

        private void OnGridRadioButton(Appointment appointment)
        {
            SetDisplay(Display.Grid);
        }

        private void OnCalendarByMonthRadioButton(Appointment appointment)
        {
            SetDisplay(Display.CalenderByMonth);
        }

        private void OnCalendarByWeekRadioButton(Appointment appointment)
        {
            SetDisplay(Display.CalendarByWeek);
        }

        public AppointmentViewModel()
        {
            AddAppointmentCommand = new RelayCommand<Appointment>(OnAddButton);
            EditAppointmentCommand = new RelayCommand<Appointment>(OnEditButton);
            DeleteAppointmentCommand = new RelayCommand<Appointment>(OnDeleteButton);

            SaveAppointmentCommand = new RelayCommand<Appointment>(OnSaveButton);
            CancelAppointmentCommand = new RelayCommand<Appointment>(OnCancelButton);

            SetGridCommand = new RelayCommand<Appointment>(OnGridRadioButton);
            SetCalendarByMonthCommand = new RelayCommand<Appointment>(OnCalendarByMonthRadioButton);
            SetCalendarByWeekCommand = new RelayCommand<Appointment>(OnCalendarByWeekRadioButton);
        }

        public RelayCommand<string> GetAppointmentsCommand { get; private set; }
        public RelayCommand<Appointment> AddAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> EditAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> DeleteAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> SaveAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> CancelAppointmentCommand { get; private set; }
        public RelayCommand<Appointment> SetGridCommand { get; private set; }
        public RelayCommand<Appointment> SetCalendarByMonthCommand { get; private set; }
        public RelayCommand<Appointment> SetCalendarByWeekCommand { get; private set; }

        public bool AddMode
        {
            get { return _addMode; }
            set
            {
                _addMode = value;
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

        private bool _canSave { get; set; }

        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                _canSave = value;
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public bool GridDisplay
        {
            get { return _gridDisplay; }
            set
            {
                _gridDisplay = value;
                OnPropertyChanged(nameof(GridDisplay));
            }
        }

        public bool CalenderByMonthDisplay
        {
            get { return _calenderByMonthDisplay; }
            set
            {
                _calenderByMonthDisplay = value;
                OnPropertyChanged(nameof(CalenderByMonthDisplay));
            }
        }

        public bool CalenderByWeekDisplay
        {
            get { return _calenderByWeekDisplay; }
            set
            {
                _calenderByWeekDisplay = value;
                OnPropertyChanged(nameof(CalenderByWeekDisplay));
            }
        }

        public List<Appointment> AllAppointments
        {
            get
            {
                var context = new DBContext();
                List<Appointment> appointments = context.Appointment.ToList();
                foreach (Appointment appointment in appointments)
                {
                    appointment.Start = appointment.Start.ToLocalTime();
                    appointment.End = appointment.End.ToLocalTime();
                }

                return appointments;
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
            List<Appointment> appointments = await context.Appointment.ToListAsync();
            foreach (Appointment appointment in appointments)
            {
                appointment.Start = appointment.Start.ToLocalTime();
                appointment.End = appointment.End.ToLocalTime();
            }
            AllAppointmentsLoaded = appointments;

            DateTime Now = DateTime.Now.ToLocalTime();

            WeeklyAppointments = appointments
                .Where(appt => ISOWeek.GetWeekOfYear(appt.Start) == ISOWeek.GetWeekOfYear(Now))
                .Where(appt => appt.Start.Year == Now.Year).ToList();

            MonthlyAppointments = appointments
                .Where(appt => appt.Start.Month == Now.Month)
                .Where(appt => appt.Start.Year == Now.Year).ToList();

        }

        private List<Appointment> _weeklyAppointments;
        public List<Appointment> WeeklyAppointments
        {
            get { return _weeklyAppointments; }
            set { SetProperty(ref _weeklyAppointments, value); }
        }

        private List<Appointment> _monthlyAppointments;
        public List<Appointment> MonthlyAppointments
        {
            get { return _monthlyAppointments; }
            set { SetProperty(ref _monthlyAppointments, value); }
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
                    SelectedAppointment.Start = SelectedAppointment.Start.ToLocalTime();
                    SelectedAppointment.End = SelectedAppointment.End.ToLocalTime();
                } 

                    OnPropertyChanged(nameof(SelectedAppointment));
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

        private object _tabControlSelectedItem;

        public object TabControlSelectedItem
        {
            get { return _tabControlSelectedItem; }
            set
            {
                SetProperty(ref _tabControlSelectedItem, value);
                OnPropertyChanged(nameof(TabControlSelectedItem));
            }
        }

        private bool _gridSelected;

        public bool GridSelected
        {
            get { return _gridSelected; }
            set
            {
                SetProperty(ref _gridSelected, value);
                OnPropertyChanged(nameof(GridSelected));
            }
        }

        private bool _monthlyCalendarSelected;

        public bool MonthlyCalendarSelected
        {
            get { return _monthlyCalendarSelected; }
            set
            {
                SetProperty(ref _monthlyCalendarSelected, value);
                OnPropertyChanged(nameof(MonthlyCalendarSelected));
            }
        }

        private bool _weeklyCalendarSelected;

        public bool WeeklyCalendarSelected
        {
            get { return _weeklyCalendarSelected; }
            set
            {
                SetProperty(ref _weeklyCalendarSelected, value);
                OnPropertyChanged(nameof(WeeklyCalendarSelected));
            }
        }

        private bool _modifyAppointmentSelected;

        public bool ModifyAppointmentSelected
        {
            get { return _modifyAppointmentSelected; }
            set
            {
                SetProperty(ref _modifyAppointmentSelected, value);
                OnPropertyChanged(nameof(ModifyAppointmentSelected));
            }
        }

        private string _selectedMonth = DateTime.Today.ToString("MMMM");

        public string SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                SetProperty(ref _selectedMonth, value);
                OnPropertyChanged(nameof(SelectedMonth));
            }
        }

        private string _selectedYear = DateTime.Today.Year.ToString();

        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
                OnPropertyChanged(nameof(SelectedYear));
            }
        }

        public ObservableCollection<string> Months
        {
            get
            {
                return new ObservableCollection<string>(
                    new List<string>
                    {
                        "January",
                        "February",
                        "March",
                        "April",
                        "May",
                        "June",
                        "July",
                        "August",
                        "September",
                        "October",
                        "November",
                        "December"
                    }
                );
            }
            set { }
        }

        public ObservableCollection<string> Years
        {
            get
            {
                List<string> years = new List<string>();
                for (int i = -3; i < 3; i++)
                {
                    years.Add(
                        DateTime.Today.AddYears(i).Year.ToString()
                    );
                }

                return new ObservableCollection<string>(years);
            }
            set { }
        }

    }
}
