using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TechnologistModule.Dialogs;
using TechnologistModule.Services;
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

            lblUserName.Text = _user.FullName;
            lblUserRole.Text = _user.Role;

            // Загрузка аватара
            LoadAvatar();

            btnAvatar.Click += async (s, e) => await ChangeAvatar();

            btnDashboard.Click += (s, e) => ShowView(new DashboardView(_api), btnDashboard);
            btnProducts.Click += (s, e) => ShowView(new ProductsView(_api), btnProducts);
            btnRecipes.Click += (s, e) => ShowView(new RecipesView(_api), btnRecipes);
            btnTechMaps.Click += (s, e) => ShowView(new TechMapsView(_api), btnTechMaps);
            btnOrders.Click += (s, e) => ShowView(new OrdersView(_api), btnOrders);
            btnBatches.Click += (s, e) => ShowView(new BatchesView(_api), btnBatches);
            btnExtruder.Click += (s, e) => ShowView(new ExtruderView(_api), btnExtruder);
            btnDeviations.Click += (s, e) => ShowView(new DeviationsView(_api), btnDeviations);
            btnReports.Click += (s, e) => ShowView(new ReportsView(_api), btnReports);
            btnLogout.Click += (s, e) => Logout();

            ShowView(new DashboardView(_api), btnDashboard);
        }

        private async void LoadAvatar()
        {
            try
            {
                var userWithAvatar = await _api.GetUserWithAvatarAsync(_user.Id);
                if (userWithAvatar?.Avatar != null && userWithAvatar.Avatar.Length > 0)
                {
                    var bitmap = ByteArrayToBitmapImage(userWithAvatar.Avatar);
                    if (bitmap != null)
                        imgAvatar.Source = bitmap;
                }
                else
                {
                    // Аватар по умолчанию - синий круг с инициалами
                    imgAvatar.Source = null;
                }
            }
            catch { }
        }

        private async Task ChangeAvatar()
        {
            var currentUser = await _api.GetUserWithAvatarAsync(_user.Id);
            var dialog = new AvatarDialog(_api, _user.Id, currentUser?.Avatar);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                LoadAvatar();
                MessageBox.Show("Аватар обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void ShowView(UserControl view, Button btn)
        {
            contentControl.Content = view;
            if (_activeBtn != null)
                _activeBtn.Background = Brushes.Transparent;
            btn.Background = new SolidColorBrush(Color.FromRgb(52, 73, 94));
            _activeBtn = btn;
        }

        private void Logout()
        {
            if (MessageBox.Show("Выйти из системы?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var login = new LoginWindow();
                login.Show();
                Close();
            }
        }
    }
}