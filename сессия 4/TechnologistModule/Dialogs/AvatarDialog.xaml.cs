using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
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
            else
            {
                // Аватар по умолчанию (иконка пользователя)
                var defaultAvatar = GenerateDefaultAvatar();
                imgAvatar.Source = defaultAvatar;
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

        private BitmapImage GenerateDefaultAvatar()
        {
            // Создаем простую заглушку (синий круг с буквой)
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawEllipse(Brushes.SteelBlue, null, new System.Windows.Point(70, 70), 70, 70);
                var typeface = new Typeface("Arial");
                var formattedText = new FormattedText("U", System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, typeface, 60, Brushes.White, 96);
                drawingContext.DrawText(formattedText, new System.Windows.Point(35, 35));
            }

            var renderTarget = new RenderTargetBitmap(140, 140, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);

            var bitmap = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            using (var stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(stream);
                stream.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }
    }
}