using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TechnologistModule;

namespace TechnologistModule.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _api = new ApiService();
        private string _captchaCode = string.Empty;
        public UserSession? CurrentUser { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
            btnLogin.Click += async (s, e) => await LoginAsync();
            btnRefresh.Click += (s, e) => GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            _captchaCode = CaptchaGenerator.GenerateCode(5);
            var bytes = CaptchaGenerator.GenerateImage(_captchaCode);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new System.IO.MemoryStream(bytes);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            imgCaptcha.Source = bitmap;
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                lblError.Text = "Введите логин и пароль";
                return;
            }

            if (txtCaptcha.Text != _captchaCode)
            {
                lblError.Text = "Неверный код";
                GenerateCaptcha();
                txtCaptcha.Clear();
                return;
            }

            btnLogin.IsEnabled = false;
            lblError.Text = "";

            try
            {
                var resp = await _api.LoginAsync(txtLogin.Text, txtPassword.Password);
                if (resp != null && resp.userId > 0)
                {
                    CurrentUser = new UserSession
                    {
                        Id = resp.userId,
                        Username = resp.username,
                        FullName = resp.fullName,
                        Role = resp.role
                    };
                    var main = new MainWindow(_api, CurrentUser);
                    main.Show();
                    Close();
                }
                else
                {
                    lblError.Text = "Неверный логин или пароль";
                }
            }
            catch
            {
                lblError.Text = "Ошибка подключения к серверу";
            }
            finally
            {
                btnLogin.IsEnabled = true;
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