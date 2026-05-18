using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LaboratoryModule.Helpers;
using LaboratoryModule.Services;
using LaboratoryModule.Models;
using LaboratoryModule.Dialogs;

namespace LaboratoryModule.Views
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
                return;
            }

            btnLogin.IsEnabled = false;
            lblError.Text = "";

            try
            {
                var resp = await _api.LoginAsync(txtLogin.Text, txtPassword.Password);
                if (resp != null && resp.userId > 0)
                {
                    if (resp.role != "laboratory" && resp.role != "admin" && resp.role != "technologist")
                    {
                        lblError.Text = "Доступ запрещен. Требуется роль 'laboratory'";
                        btnLogin.IsEnabled = true;
                        return;
                    }

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
}