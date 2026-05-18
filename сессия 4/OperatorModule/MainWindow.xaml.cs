using OperatorModule.Dialogs;
using OperatorModule.Models;
using OperatorModule.Services;
using OperatorModule.Views;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OperatorModule
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
            lblShift.Text = "Смена: Дневная";

            LoadAvatar();

            btnAvatar.Click += async (s, e) => await ChangeAvatar();
            btnActiveBatches.Click += (s, e) => ShowView(new ActiveBatchesView(_api), btnActiveBatches);
            btnEquipment.Click += (s, e) => ShowView(new EquipmentView(), btnEquipment);
            btnLog.Click += (s, e) => ShowView(new BatchLogView(), btnLog);
            btnReport.Click += (s, e) => ShowView(new ReportProblemView(), btnReport);
            btnLogout.Click += (s, e) => Logout();

            ShowView(new ActiveBatchesView(_api), btnActiveBatches);
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

        public void ShowActiveBatchesView()
        {
            ShowView(new ActiveBatchesView(_api), btnActiveBatches);
        }

        public void ShowBatchProgramView(BatchProgramView view)
        {
            contentControl.Content = view;
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

    public class UserSession
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}