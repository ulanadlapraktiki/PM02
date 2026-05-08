using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TechnologistModule.Views;

namespace TechnologistModule
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _api;
        private readonly UserSession _user;
        private Button? _activeBtn;

        public MainWindow(ApiService api, UserSession user)
        {
            InitializeComponent();
            _api = api;
            _user = user;
            lblUser.Text = $"{_user.FullName}\n{_user.Role}";

            btnDashboard.Click += (s, e) => ShowView(new DashboardView(_api), btnDashboard);
            btnProducts.Click += (s, e) => ShowView(new ProductsView(_api), btnProducts);
            btnRecipes.Click += (s, e) => ShowView(new RecipesView(_api), btnRecipes);
            btnOrders.Click += (s, e) => ShowView(new OrdersView(_api), btnOrders);
            btnBatches.Click += (s, e) => ShowView(new BatchesView(_api), btnBatches);
            btnDeviations.Click += (s, e) => ShowView(new DeviationsView(_api), btnDeviations);
            btnReports.Click += (s, e) => ShowView(new ReportsView(_api), btnReports);
            btnLogout.Click += (s, e) => Logout();

            ShowView(new DashboardView(_api), btnDashboard);
        }

        private void ShowView(UserControl view, Button btn)
        {
            contentControl.Content = view;
            if (_activeBtn != null) _activeBtn.Background = Brushes.Transparent;
            btn.Background = new SolidColorBrush(Color.FromRgb(52, 73, 94));
            _activeBtn = btn;
        }

        private void Logout()
        {
            if (MessageBox.Show("Выйти?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                Close();
            }
        }
    }
}