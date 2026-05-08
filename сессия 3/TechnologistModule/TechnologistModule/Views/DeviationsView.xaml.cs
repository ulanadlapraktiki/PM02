using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TechnologistModule.Views
{
    public partial class DeviationsView : UserControl
    {
        private readonly ApiService _api;
        private List<Deviation>? _deviations;

        public DeviationsView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            btnRefresh.Click += async (s, e) => await LoadData();
            btnResolve.Click += async (s, e) => await Resolve();
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _deviations = await _api.GetDeviationsAsync();
                grid.ItemsSource = _deviations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                progress.Visibility = Visibility.Collapsed;
            }
        }

        private async Task Resolve()
        {
            var selected = grid.SelectedItem as Deviation;
            if (selected == null)
            {
                MessageBox.Show("Выберите отклонение");
                return;
            }

            string comment = Interaction.InputBox("Комментарий к решению:", "Закрытие отклонения", "");
            if (string.IsNullOrWhiteSpace(comment)) return;

            try
            {
                await _api.ResolveDeviationAsync(selected.Id, comment);
                await LoadData();
                MessageBox.Show("Отклонение закрыто", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }

    public class DeviationStatusConverter : IValueConverter
    {
        public static readonly DeviationStatusConverter Instance = new DeviationStatusConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? "Активно" : "Закрыто";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}