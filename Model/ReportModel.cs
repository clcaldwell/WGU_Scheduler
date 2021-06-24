using System;

namespace Scheduler.Model
{
    public class ReportModel
    {
    }

    public class ConsultantReportModel
    {
        public string Consultant { get; set; }
        public DateTime Appointment { get; set; }
        public string AppointmentType { get; set; }
        public string CustomerName { get; set; }
    }

    public class MonthlyReportModel
    {
        public string Month { get; set; }
        public string AppointmentType { get; set; }
        public int AppointmentTypeCount { get; set; }
    }
}
