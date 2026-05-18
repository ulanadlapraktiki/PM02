using System.Windows;
using OperatorModule.Views; 

namespace OperatorModule
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var login = new LoginWindow();
            login.Show();
        }
    }
}