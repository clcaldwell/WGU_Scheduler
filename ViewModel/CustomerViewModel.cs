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

        public CustomerViewModel()
        {
            EditCustomerCommand = new RelayCommand<Customer>(OnEditButton);
            SaveCustomerCommand = new RelayCommand<Customer>(OnSaveButton);
        }

        private void OnEditButton(Customer customer)
        {
            MessageBox.Show(SelectedCustomer.CustomerId.ToString());
        }

        private void OnSaveButton(Customer customer)
        {
            var context = new DBContext();
            context.Update(customer);
            context.SaveChanges();
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

        private List<Customer> _allcustomersloaded;
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

        public RelayCommand<string> GetCustomersCommand { get; private set; }
        public RelayCommand<Customer> EditCustomerCommand { get; private set; }
        public RelayCommand<Customer> SaveCustomerCommand { get; private set; }

        private Customer _selectedcustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedcustomer; }
            set
            {
                if (value != null && value != _selectedcustomer)
                {
                    SetProperty(ref _selectedcustomer, value);
                    OnPropertyChanged("SelectedCustomer");
                }
            }
        }
    }
}
