using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TechnologistModule.Helpers;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly ApiService _api;
        private string _captchaCode = string.Empty;
        private DispatcherTimer? _cooldownTimer;
        private int _cooldownSeconds = 30;
        private bool _isCooldown = false;

        public RegisterWindow(ApiService api)
        {
            InitializeComponent();
            _api = api;
            GenerateCaptcha();

            btnRegister.Click += async (s, e) => await Register();
            btnCancel.Click += (s, e) => Close();
            btnRefreshCaptcha.Click += (s, e) => RefreshCaptcha();

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
                MessageBox.Show($"Подождите {_cooldownSeconds} секунд");
                return;
            }

            GenerateCaptcha();
            txtCaptcha.Clear();

            _isCooldown = true;
            _cooldownSeconds = 30;
            btnRefreshCaptcha.IsEnabled = false;
            btnRefreshCaptcha.Content = $"{_cooldownSeconds}с";
            _cooldownTimer?.Start();
        }

        private void CooldownTimer_Tick(object sender, EventArgs e)
        {
            _cooldownSeconds--;
            if (_cooldownSeconds <= 0)
            {
                _cooldownTimer?.Stop();
                _isCooldown = false;
                btnRefreshCaptcha.IsEnabled = true;
                btnRefreshCaptcha.Content = "⟳";
            }
            else
            {
                btnRefreshCaptcha.Content = $"{_cooldownSeconds}с";
            }
        }

        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblError.Text = "Введите логин";
                return;
            }

            // Остальная логика регистрации...
        }
    }
}