using System.Globalization;

namespace WorkoutTracker.Services.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "N/A";
            }

            if (value is DateTime date)
            {
                return date.ToString("dddd, MM/dd/yyyy");
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}