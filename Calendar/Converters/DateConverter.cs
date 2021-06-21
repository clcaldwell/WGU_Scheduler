using System;
using System.Globalization;
using System.Windows.Data;

namespace Scheduler.XCalendar.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime) value;

            string param = (string) parameter;

            return param.ToUpper() switch
            {
                "MONTH" => date.Month,
                "YEAR" => date.Year,
                "DAY" => date.Day,
                _ => date.ToString(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}