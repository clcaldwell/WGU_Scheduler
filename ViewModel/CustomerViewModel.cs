using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight.CommandWpf;

using Microsoft.EntityFrameworkCore;

using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class CustomerViewModel : ViewModelBase
    {
        private List<Customer> _allcustomersloaded;

        private Customer _selectedcustomer;
        private Address _selectedaddress;
        private City _selectedcity;
        private Country _selectedcountry;

        private bool _addMode = false;

        private bool _editMode = false;

        private bool _viewMode = true;

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
                AddMode = true;
                EditMode = true;
                ViewMode = false;

                if (SelectedCustomer == null)
                {
                    SelectedCustomer = AllCustomers.FirstOrDefault();
                }
            }
            if (mode == Mode.Edit)
            {
                EditMode = true;
                ViewMode = false;
            }
            if (mode == Mode.View)
            {
                AddMode = false;
                EditMode = false;
                ViewMode = true;
            }
        }

        private void OnAddButton(Customer customer)
        {
            SetMode(Mode.Add);
        }

        private void OnEditButton(Customer customer)
        {
            SetMode(Mode.Edit);
        }

        private void OnDeleteButton(Customer customer)
        {
            if (MessageBox.Show("Are you sure you want to delete this customer?" +
                    "\r\n Id:" + customer.CustomerId +
                    "\r\n Name:" + customer.CustomerName,
                "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var context = new DBContext();
                context.Remove(customer);
                context.SaveChanges();
                SetMode(Mode.View);
                LoadCustomers();
            }
        }

        private void OnSaveButton(Customer customer)
        {
            var context = new DBContext();
            if (AddMode)
            {
                int NextId = AllCustomers.OrderByDescending(a => a.CustomerId).FirstOrDefault().CustomerId + 1;
                MessageBox.Show("NextAppointmentId: " + NextId);

                Customer NewCustomer = new Customer
                {
                    CustomerId = NextId,
                    CustomerName = customer.CustomerName,
                    AddressId = customer.AddressId,
                    Active = customer.Active,
                    Address = customer.Address,
                    CreateDate = DateTime.Now.ToUniversalTime(),
                    CreatedBy = customer.CreatedBy,
                    LastUpdate = DateTime.Now.ToUniversalTime(),
                    LastUpdateBy = customer.LastUpdateBy
                }; 

                context.Add(NewCustomer);
            }
            else
            {
                //customer.LastUpdate = DateTime.Now.ToUniversalTime();

                context.Update(customer);
                context.Update(SelectedAddress);
                context.Update(SelectedCity);
                context.Update(SelectedCountry);
            }

            context.SaveChanges();
            LoadCustomers();
            SetMode(Mode.View);
        }

        private void OnCancelButton(Customer customer)
        {
            LoadCustomers();
            SetMode(Mode.View);
        }

        public CustomerViewModel()
        {
            AddCustomerCommand = new RelayCommand<Customer>(OnAddButton);
            EditCustomerCommand = new RelayCommand<Customer>(OnEditButton);
            DeleteCustomerCommand = new RelayCommand<Customer>(OnDeleteButton);

            SaveCustomerCommand = new RelayCommand<Customer>(OnSaveButton);
            CancelCustomerCommand = new RelayCommand<Customer>(OnCancelButton);
        }

        public RelayCommand<string> GetCustomersCommand { get; private set; }
        public RelayCommand<Customer> AddCustomerCommand { get; private set; }
        public RelayCommand<Customer> EditCustomerCommand { get; private set; }
        public RelayCommand<Customer> DeleteCustomerCommand { get; private set; }
        public RelayCommand<Customer> SaveCustomerCommand { get; private set; }
        public RelayCommand<Customer> CancelCustomerCommand { get; private set; }

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

        public List<Customer> AllCustomers
        {
            get
            {
                var context = new DBContext();
                return context.Customer.ToList();
            }
            set
            {
                var context = new DBContext();
                context.Customer.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

        public List<Customer> AllCustomersLoaded
        {
            get { return _allcustomersloaded; }
            set { SetProperty(ref _allcustomersloaded, value); }
        }

        public async void LoadCustomers()
        {
            var context = new DBContext();
            AllCustomersLoaded = await context.Customer.ToListAsync();
        }

        public Customer SelectedCustomer
        {
            get { return _selectedcustomer; }
            set
            {
                if (value != null && value != _selectedcustomer)
                {
                    SetProperty(ref _selectedcustomer, value);

                    var context = new DBContext();
                    SelectedAddress = context.Address.Find(value.AddressId);

                    OnPropertyChanged(nameof(SelectedCustomer));
                }
            }
        }

        public Address SelectedAddress
        {
            get { return _selectedaddress; }
            set
            {
                if (value != null && value != _selectedaddress)
                {
                    SetProperty(ref _selectedaddress, value);

                    var context = new DBContext();
                    SelectedCity = context.City.Find(value.CityId);

                    OnPropertyChanged(nameof(SelectedAddress));
                }
            }
        }

        public City SelectedCity
        {
            get { return _selectedcity; }
            set
            {
                if (value != null && value != _selectedcity)
                {
                    SetProperty(ref _selectedcity, value);

                    var context = new DBContext();
                    SelectedCountry = context.Country.Find(value.CountryId);

                    OnPropertyChanged(nameof(SelectedCity));
                }
            }
        }

        public Country SelectedCountry
        {
            get { return _selectedcountry; }
            set
            {
                if (value != null && value != _selectedcountry)
                {
                    SetProperty(ref _selectedcountry, value);
                    OnPropertyChanged(nameof(SelectedCountry));
                }
            }
        }

    }
}
