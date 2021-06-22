using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Scheduler.Model.DBEntities;

namespace Scheduler.ViewModel
{
    public class ReminderViewModel : ViewModelBase
    {

        public ReminderViewModel()
        {
            GenerateReminder();
        }

        public void GenerateReminder()
        {
            var Now = DateTime.Now.ToLocalTime();

            // Lambda here is prefered, as it reduces the complexity of multiple if 
            // statements and improves readability.
            var remindAppointments = AllAppointments
                .Where(appt => appt.Start.AddMinutes(-15) <= Now)
                .Where(appt => appt.End >= Now);

            if (remindAppointments.Count() > 0)
            {
                foreach (Appointment remindAppointment in remindAppointments)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Title: {remindAppointment.Title}" );
                    sb.AppendLine($"Start Time: {remindAppointment.Start}" );
                    sb.AppendLine($"End Time: {remindAppointment.End}" );
                    sb.AppendLine($"Type: {remindAppointment.Type}" );
                    ReminderText = sb.ToString();
                }
            }
            else
            {
                ReminderText = "You have no appointments within the next 15 minutes.";
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

        private string _reminderText { get; set; }

        public string ReminderText
        {
            get { return _reminderText; }
            set
            {
                _reminderText = value;
                OnPropertyChanged(nameof(ReminderText));
            }
        }

        private Appointment _currentAppointment { get; set; }

        public Appointment CurrentAppointment
        {
            get { return _currentAppointment; }
            set
            {
                _currentAppointment = value;
                OnPropertyChanged(nameof(CurrentAppointment));
            }
        }
    }
}