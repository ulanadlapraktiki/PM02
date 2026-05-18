using LaboratoryModule.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LaboratoryModule.Dialogs
{
    public partial class AvatarDialog : Window
    {
        private readonly ApiService _api;
        private readonly int _userId;
        private byte[]? _selectedAvatar;
        private BitmapImage? _previewImage;

        public AvatarDialog(ApiService api, int userId, byte[]? currentAvatar)
        {
            InitializeComponent();
            _api = api;
            _userId = userId;

            if (currentAvatar != null && currentAvatar.Length > 0)
            {
                _previewImage = ByteArrayToBitmapImage(currentAvatar);
                imgAvatar.Source = _previewImage;
            }

            btnSelect.Click += (s, e) => SelectImage();
            btnSave.Click += async (s, e) => await SaveAvatar();
            btnCancel.Click += (s, e) => Close();
        }

        private void SelectImage()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            dialog.Title = "Выберите изображение для аватара";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var bytes = File.ReadAllBytes(dialog.FileName);

                    if (bytes.Length > 2 * 1024 * 1024)
                    {
                        lblInfo.Text = "Файл слишком большой (максимум 2MB)";
                        return;
                    }

                    _selectedAvatar = bytes;
                    _previewImage = ByteArrayToBitmapImage(bytes);
                    imgAvatar.Source = _previewImage;
                    lblInfo.Text = "";
                }
                catch (Exception ex)
                {
                    lblInfo.Text = $"Ошибка: {ex.Message}";
                }
            }
        }

        private async Task SaveAvatar()
        {
            if (_selectedAvatar == null)
            {
                lblInfo.Text = "Выберите изображение";
                return;
            }

            btnSave.IsEnabled = false;
            lblInfo.Text = "Сохранение...";

            try
            {
                var success = await _api.UpdateAvatarAsync(_userId, _selectedAvatar);
                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    lblInfo.Text = "Ошибка сохранения";
                }
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Ошибка: {ex.Message}";
            }
            finally
            {
                btnSave.IsEnabled = true;
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
    }
}