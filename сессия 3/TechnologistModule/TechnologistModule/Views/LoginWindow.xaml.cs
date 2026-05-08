using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TechnologistModule.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _api = new ApiService();
        private string _captchaCode = string.Empty;
        private UserSession? _currentUser;

        // Ограничение попыток
        private int _failedAttempts = 0;
        private DateTime? _blockUntil = null;
        private const int MaxFailedAttempts = 3;
        private const int BlockMinutes = 5;

        private readonly string _attemptsFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "login_attempts.json");

        public UserSession? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public LoginWindow()
        {
            InitializeComponent();
            LoadFailedAttempts();
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
            bitmap.StreamSource = new MemoryStream(bytes);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            imgCaptcha.Source = bitmap;
        }

        private void LoadFailedAttempts()
        {
            if (File.Exists(_attemptsFile))
            {
                try
                {
                    var json = File.ReadAllText(_attemptsFile);
                    var data = JsonSerializer.Deserialize<AttemptsData>(json);
                    if (data != null)
                    {
                        _failedAttempts = data.FailedAttempts;
                        _blockUntil = data.BlockUntil;
                    }
                }
                catch { }
            }
        }

        private void SaveFailedAttempts()
        {
            var data = new AttemptsData
            {
                FailedAttempts = _failedAttempts,
                BlockUntil = _blockUntil
            };
            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(_attemptsFile, json);
        }

        private bool IsBlocked()
        {
            if (_blockUntil.HasValue && DateTime.Now < _blockUntil.Value)
            {
                var remainingSeconds = (int)(_blockUntil.Value - DateTime.Now).TotalSeconds;
                var minutes = remainingSeconds / 60;
                var seconds = remainingSeconds % 60;

                lblError.Text = $"Слишком много неудачных попыток.\nДоступ заблокирован на {minutes} мин {seconds} сек.";

                btnLogin.IsEnabled = false;

                var timer = new System.Timers.Timer(remainingSeconds * 1000);
                timer.Elapsed += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _failedAttempts = 0;
                        _blockUntil = null;
                        SaveFailedAttempts();
                        btnLogin.IsEnabled = true;
                        lblError.Text = "";
                    });
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();

                return true;
            }

            if (_blockUntil.HasValue && DateTime.Now >= _blockUntil.Value)
            {
                _failedAttempts = 0;
                _blockUntil = null;
                SaveFailedAttempts();
                btnLogin.IsEnabled = true;
            }

            return false;
        }

        private void ResetFailedAttempts()
        {
            _failedAttempts = 0;
            _blockUntil = null;
            SaveFailedAttempts();
        }

        private void IncrementFailedAttempts()
        {
            _failedAttempts++;

            if (_failedAttempts >= MaxFailedAttempts)
            {
                _blockUntil = DateTime.Now.AddMinutes(BlockMinutes);
                lblError.Text = $"Превышено количество попыток ({MaxFailedAttempts}).\nДоступ заблокирован на {BlockMinutes} минут.";
                SaveFailedAttempts();

                btnLogin.IsEnabled = false;

                var timer = new System.Timers.Timer(BlockMinutes * 60 * 1000);
                timer.Elapsed += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _failedAttempts = 0;
                        _blockUntil = null;
                        SaveFailedAttempts();
                        btnLogin.IsEnabled = true;
                        lblError.Text = "";
                    });
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            else
            {
                SaveFailedAttempts();
            }
        }

        private async Task LoginAsync()
        {
            if (IsBlocked())
            {
                GenerateCaptcha();
                txtCaptcha.Clear();
                return;
            }

            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                lblError.Text = "Введите логин и пароль";
                return;
            }

            if (txtCaptcha.Text != _captchaCode)
            {
                lblError.Text = "Неверный код подтверждения";
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
                    ResetFailedAttempts();

                    _currentUser = new UserSession
                    {
                        Id = resp.userId,
                        Username = resp.username,
                        FullName = resp.fullName ?? resp.username,
                        Role = resp.role ?? "operator"
                    };

                    var main = new MainWindow(_api, _currentUser);
                    main.Show();
                    Close();
                }
                else
                {
                    IncrementFailedAttempts();
                    var remaining = MaxFailedAttempts - _failedAttempts;
                    lblError.Text = remaining > 0
                        ? $"Неверный логин или пароль. Осталось попыток: {remaining}"
                        : $"Неверный логин или пароль. Доступ заблокирован на {BlockMinutes} мин.";

                    GenerateCaptcha();
                    txtCaptcha.Clear();
                    txtPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка подключения: {ex.Message}";
                GenerateCaptcha();
            }
            finally
            {
                if (!IsBlocked())
                {
                    btnLogin.IsEnabled = true;
                }
            }
        }
    }

    public class AttemptsData
    {
        public int FailedAttempts { get; set; }
        public DateTime? BlockUntil { get; set; }
    }

    public class UserSession
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}