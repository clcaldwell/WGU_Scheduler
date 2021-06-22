using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {

        private async Task GenerateMonthlyReport()
        {
            var text = new StringBuilder();
            text.AppendLine("Appointment Types by Month: (Current Month +/- 2)");
            text.AppendLine("");
            DateTime thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime previousMonth = thisMonth.AddMonths(-2);
            DateTime nextMonth = thisMonth.AddMonths(3).AddMilliseconds(-1);

            var groupedByMonthList = AllAppointments
                .OrderBy(appt => appt.Start)
                .Where(appt => appt.Start >= previousMonth && appt.Start <= nextMonth)
                .GroupBy(appt => appt.Start.ToString("MMMM yyyy"));

            foreach (var group in groupedByMonthList)
            {
                text.AppendLine($"{group.Key}:");
                var groupedByTypeList = group.GroupBy(appt => appt.Type);

                foreach (var list in groupedByTypeList)
                {
                    text.AppendLine($"\t{list.Key}: {list.Count()}");
                }
                text.AppendLine("");
            }

            MonthlyReport = text.ToString();
        }

        private async Task GenerateConsultantSchedule()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Consultant Report: (Next 30 Days)");
            sb.AppendLine("");

            var today = DateTime.Today;

            var apptConsultants = AllAppointments
                .Where(appt => appt.Start >= today && appt.Start <= today.AddDays(30))
                .GroupBy(appt => appt.UserId);
            
            foreach (var apptConsultant in apptConsultants)
            {
                var orderedApptConsultant = apptConsultant
                    .OrderBy(appt => appt.Start);

                var user = AllUsers
                    .Where(user => user.UserId == apptConsultant.Key)
                    .Single().UserName;

                sb.AppendLine($"{user}: ");

                foreach (var appt in orderedApptConsultant)
                {
                    var customer = AllCustomers
                        .Where(customer => customer.CustomerId == appt.CustomerId)
                        .Single().CustomerName;

                    sb.AppendLine($"{customer} - \t{appt.Start}.");
                }
                sb.AppendLine();
            }

            ConsultantReport = sb.ToString();
        }

        private async Task GenerateCurrentWeekCustomerReport()
        {
            var text = new StringBuilder();
            text.AppendLine("Fraud Detection: Customers with Most Lunch appointments (All Time)");
            text.AppendLine("");

            int counter = 0;
            Customer frequentCustomer = null;
            foreach (Customer customer in AllCustomers)
            {
                // Lambda: This lambda lets me do this simple logic in one logical line, instead of having
                // to do the extended version of the logic over multiple lines. This is much more readable and concise with the lambda.
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

        public List<User> AllUsers
        {
            get
            {
                var context = new DBContext();
                List<User> users = context.User.ToList();
                
                return users;
            }
            set
            {
                var context = new DBContext();
                context.User.UpdateRange(value.ToList());
                context.SaveChanges();
            }
        }

        private string _monthlyReport;

        public string MonthlyReport
        {
            get { return _monthlyReport; }
            set
            {
                SetProperty(ref _monthlyReport, value);
                OnPropertyChanged(nameof(MonthlyReport));
            }
        }

        private string _consultantReport;

        public string ConsultantReport
        {
            get { return _consultantReport; }
            set
            {
                SetProperty(ref _consultantReport, value);
                OnPropertyChanged(nameof(ConsultantReport));
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
                GenerateMonthlyReport();
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
                GenerateConsultantSchedule();
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
                GenerateCurrentWeekCustomerReport();
            }
        }

        private object tabControlSelectedItem;

        public object TabControlSelectedItem { get => tabControlSelectedItem; set => SetProperty(ref tabControlSelectedItem, value); }
    }
}
