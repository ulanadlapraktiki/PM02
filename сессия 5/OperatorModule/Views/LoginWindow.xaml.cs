using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OperatorModule.Helpers;
using OperatorModule.Models;
using OperatorModule.Services;

namespace OperatorModule.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _api = new ApiService();
        private string _captchaCode = string.Empty;
        private DispatcherTimer? _cooldownTimer;
        private int _cooldownSeconds = 30;
        private bool _isCooldown = false;

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();

            btnLogin.Click += async (s, e) => await LoginAsync();
            btnRefresh.Click += (s, e) => RefreshCaptcha();

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
                ShowError($"Подождите {_cooldownSeconds} секунд");
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

        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            if (txtCaptcha.Text != _captchaCode)
            {
                ShowError("Неверный код с картинки");
                RefreshCaptcha();
                txtCaptcha.Clear();
                return;
            }

            btnLogin.IsEnabled = false;
            HideError();

            try
            {
                var resp = await _api.LoginAsync(txtLogin.Text, txtPassword.Password);
                if (resp != null && resp.userId > 0)
                {
                    // Проверяем роль - только аппаратчик или администратор
                    if (resp.role != "operator" && resp.role != "admin")
                    {
                        ShowError("Доступ запрещен. Требуется роль 'operator'");
                        btnLogin.IsEnabled = true;
                        return;
                    }

                    _api.SetCurrentUserId(resp.userId);

                    var user = new UserSession
                    {
                        Id = resp.userId,
                        Username = resp.username,
                        FullName = resp.fullName,
                        Role = resp.role
                    };

                    var main = new MainWindow(_api, user);
                    main.Show();
                    Close();
                }
                else
                {
                    ShowError("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            lblError.Visibility = Visibility.Collapsed;
        }
    }
}