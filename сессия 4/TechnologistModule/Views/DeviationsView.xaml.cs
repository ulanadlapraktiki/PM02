using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class DeviationsView : UserControl
    {
        private readonly ApiService _api;
        private List<Deviation>? _deviations;
        private Deviation? _selectedDeviation;

        public DeviationsView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            btnRefresh.Click += async (s, e) => await LoadData();
            btnResolve.Click += async (s, e) => await Resolve();
            btnDelete.Click += async (s, e) => await DeleteDeviation();
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

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDeviation = grid.SelectedItem as Deviation;
            bool isSelected = _selectedDeviation != null;
            btnResolve.IsEnabled = isSelected && _selectedDeviation?.ResolvedAt == null;
            btnDelete.IsEnabled = isSelected;
        }

        private async Task Resolve()
        {
            if (_selectedDeviation == null) return;

            string comment = Interaction.InputBox("Комментарий к решению:", "Закрытие отклонения", "");
            if (string.IsNullOrWhiteSpace(comment)) return;

            try
            {
                await _api.ResolveDeviationAsync(_selectedDeviation.Id, comment);
                await LoadData();
                MessageBox.Show("Отклонение закрыто", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task DeleteDeviation()
        {
            if (_selectedDeviation == null) return;

            if (MessageBox.Show($"Удалить отклонение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteDeviationAsync(_selectedDeviation.Id);
                await LoadData();
                MessageBox.Show("Отклонение удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}