using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TechnologistModule.Helpers;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _api = new ApiService();
        private string _captchaCode = string.Empty;
        private DispatcherTimer? _cooldownTimer;
        private int _cooldownSeconds = 30;
        private bool _isCooldown = false;
        public UserSession? CurrentUser { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();

            btnLogin.Click += async (s, e) => await LoginAsync();
            btnRefresh.Click += (s, e) => RefreshCaptcha();
            btnRegister.Click += (s, e) => OpenRegisterWindow();

            _cooldownTimer = new DispatcherTimer();
            _cooldownTimer.Interval = TimeSpan.FromSeconds(1);
            _cooldownTimer.Tick += CooldownTimer_Tick!;
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

        private void RefreshCaptcha()
        {
            if (_isCooldown)
            {
                MessageBox.Show($"Подождите {_cooldownSeconds} секунд", "Предупреждение");
                return;
            }

            GenerateCaptcha();
            txtCaptcha.Clear();

            _isCooldown = true;
            _cooldownSeconds = 30;
            btnRefresh.IsEnabled = false;
            btnRefresh.Content = $"{_cooldownSeconds}с";
            _cooldownTimer?.Start();
        }

        private void CooldownTimer_Tick(object sender, EventArgs e)
        {
            _cooldownSeconds--;
            if (_cooldownSeconds <= 0)
            {
                _cooldownTimer?.Stop();
                _isCooldown = false;
                btnRefresh.IsEnabled = true;
                btnRefresh.Content = "⟳";
            }
            else
            {
                btnRefresh.Content = $"{_cooldownSeconds}с";
            }
        }

        private void OpenRegisterWindow()
        {
            var registerWindow = new RegisterWindow(_api);
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
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
                RefreshCaptcha();
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
                    // Устанавливаем ID пользователя в ApiService для передачи в заголовках запросов
                    _api.SetCurrentUserId(resp.userId); 

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
                    RefreshCaptcha();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка: {ex.Message}";
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