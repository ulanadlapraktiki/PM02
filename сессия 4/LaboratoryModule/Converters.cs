using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LaboratoryModule
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string ?? "";
            return status switch
            {
                "quality_pending" => new SolidColorBrush(Colors.Orange),
                "in_progress" => new SolidColorBrush(Colors.Blue),
                "approved" or "completed" => new SolidColorBrush(Colors.Green),
                "blocked" => new SolidColorBrush(Colors.Red),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}