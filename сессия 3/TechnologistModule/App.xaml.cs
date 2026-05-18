using System;
using System.Windows;
using System.Windows.Data;

namespace TechnologistModule
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }

    public class ResolvedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? "Активно" : "Закрыто";
        }

        public object ConvertBack(object value, Type targetValue, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}