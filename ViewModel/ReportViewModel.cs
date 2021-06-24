using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scheduler.Model;
using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {

        private async Task GenerateMonthlyReport()
        {
            DateTime thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime previousMonth = thisMonth.AddMonths(-1);
            DateTime nextMonth = thisMonth.AddMonths(2).AddMilliseconds(-1);
            List<int> months = new List<int>() {
                previousMonth.Month, thisMonth.Month, nextMonth.Month
            };
                
            List<MonthlyReportModel> monthlyReport = new List<MonthlyReportModel>();

            var currentAppointments = AllAppointments.Where(appt =>
                appt.Start.Month >= previousMonth.Month && appt.Start.Month <= nextMonth.Month)
                .OrderBy(appt => appt.Start).ToList();


            foreach (var month in months)
            {
                // Lambda: This lambda lets me do this logic concisely, instead of having
                // to do the extended version of the logic over a dozen lines.
                // This is much more readable and concise with the lambda.
                var counts = currentAppointments
                    .Where(appt => appt.Start.Month == month)
                    .GroupBy(appt => appt.Type)
                    .Select(appt => new { Value = appt.Key, Count = appt.Count() });

                foreach (var currentCount in counts)
                {
                        monthlyReport.Add(
                            new MonthlyReportModel()
                            {
                                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                                AppointmentType = currentCount.Value,
                                AppointmentTypeCount = currentCount.Count
                            }
                    );
                }


            }

            MonthlyReport = new ObservableCollection<MonthlyReportModel>(monthlyReport);
        }

        private ObservableCollection<MonthlyReportModel> _monthlyReport;
        public ObservableCollection<MonthlyReportModel> MonthlyReport
        {
            get { return _monthlyReport; }
            set
            {
                SetProperty(ref _monthlyReport, value);
                OnPropertyChanged(nameof(MonthlyReport));
            }
        }

        private async Task GenerateConsultantSchedule()
        {
            List<ConsultantReportModel> consultantReport = new List<ConsultantReportModel>();

            foreach (User consultant in AllUsers)
            {
                AllAppointments.Where(appt => appt.UserId == consultant.UserId)
                    .OrderBy(appt => appt.Start).ToList()
                    .ForEach(
                        appt => consultantReport.Add(
                            new ConsultantReportModel()
                            {
                                Consultant = consultant.UserName,
                                Appointment = appt.Start,
                                AppointmentType = appt.Type,
                                CustomerName =
                                    AllCustomers.Where(cust => appt.CustomerId == cust.CustomerId).FirstOrDefault().CustomerName
                            }
                        )
                    );
                ConsultantReport = new ObservableCollection<ConsultantReportModel>(consultantReport);
            }
        }

        private ObservableCollection<ConsultantReportModel> _consultantReport;
        public ObservableCollection<ConsultantReportModel> ConsultantReport
        {
            get { return _consultantReport; }
            set
            {
                SetProperty(ref _consultantReport, value);
                OnPropertyChanged(nameof(ConsultantReport));
            }
        }

        private async Task GenerateFraudReport()
        {
            var text = new StringBuilder();
            text.AppendLine("Fraud Detection: Customers with Most Lunch appointments (All Time)");
            text.AppendLine("");

            int counter = 0;
            Customer frequentCustomer = null;
            foreach (Customer customer in AllCustomers)
            {
                int currentCount = AllAppointments.Where(appt => appt.CustomerId == customer.CustomerId).Count();
                if (currentCount > counter)
                {
                    counter = currentCount;
                    frequentCustomer = customer;
                }
            }
            
            text.AppendLine($"Number of Lunches:\t{counter}");
            text.AppendLine($"Frequent Customer:\t{frequentCustomer.CustomerName}");

            var listOfFrequentLunches = AllAppointments.Where(appt => appt.CustomerId == frequentCustomer.CustomerId);

            foreach (var appt in listOfFrequentLunches)
            {
                text.AppendLine($"Date:\t{appt.Start.Date:MM/dd/yyyy}");
            }

            FraudReport = text.ToString();
        }

        public ObservableCollection<Customer> AllCustomers
        {
            get
            {
                var context = new DBContext();
                return new ObservableCollection<Customer>(context.Customer.ToList());
            }
            set
            {
                var context = new DBContext();
                context.Customer.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

        public ObservableCollection<Appointment> AllAppointments
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

                return new ObservableCollection<Appointment>(appointments);
            }
            set
            {
                var context = new DBContext();
                context.Appointment.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

        public ObservableCollection<User> AllUsers
        {
            get
            {
                var context = new DBContext();
                return new ObservableCollection<User>(context.User.ToList());
            }
            set
            {
                var context = new DBContext();
                context.User.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

        private string _fraudReport;

        public string FraudReport
        {
            get { return _fraudReport; }
            set
            {
                SetProperty(ref _fraudReport, value);
                OnPropertyChanged(nameof(FraudReport));
            }
        }

        private bool _monthlyReportSelected;

        public bool MonthlyReportSelected
        {
            get { return _monthlyReportSelected; }
            set
            {
                SetProperty(ref _monthlyReportSelected, value);
                OnPropertyChanged(nameof(MonthlyReportSelected));
                var _ = GenerateMonthlyReport();
            }
        }

        private bool _consultantReportSelected;

        public bool ConsultantReportSelected
        {
            get { return _consultantReportSelected; }
            set
            {
                SetProperty(ref _consultantReportSelected, value);
                OnPropertyChanged(nameof(ConsultantReportSelected));
                var _ = GenerateConsultantSchedule();
            }
        }

        private bool _fraudReportSelected;

        public bool FraudReportSelected
        {
            get { return _fraudReportSelected; }
            set
            {
                SetProperty(ref _fraudReportSelected, value);
                OnPropertyChanged(nameof(FraudReportSelected));
                var _ = GenerateFraudReport();
            }
        }

        private object _tabControlSelectedItem;

        public object TabControlSelectedItem
        {
            get { return _tabControlSelectedItem; }
            set
            {
                SetProperty(ref _tabControlSelectedItem, value);
            }
        }
    }
}
