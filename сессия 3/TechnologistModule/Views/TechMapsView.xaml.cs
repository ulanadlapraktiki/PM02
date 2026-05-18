using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Dialogs;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class TechMapsView : UserControl
    {
        private readonly ApiService _api;
        private List<TechMap>? _techMaps;
        private TechMap? _selectedTechMap;

        public TechMapsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddTechMap();
            btnEdit.Click += async (s, e) => await EditTechMap();
            btnDelete.Click += async (s, e) => await DeleteTechMap();
            btnApprove.Click += async (s, e) => await ApproveTechMap();
            btnRefresh.Click += async (s, e) => await LoadData();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _techMaps = await _api.GetTechMapsAsync();
                grid.ItemsSource = _techMaps;
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
            _selectedTechMap = grid.SelectedItem as TechMap;
            bool isSelected = _selectedTechMap != null;
            btnEdit.IsEnabled = isSelected;
            btnDelete.IsEnabled = isSelected;
            btnApprove.IsEnabled = isSelected && _selectedTechMap?.Status != "active";
        }

        private async Task AddTechMap()
        {
            var dialog = new AddTechMapDialog(_api);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task EditTechMap()
        {
            if (_selectedTechMap == null) return;

            var dialog = new EditTechMapDialog(_api, _selectedTechMap);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task DeleteTechMap()
        {
            if (_selectedTechMap == null) return;

            if (MessageBox.Show($"Удалить тех. карту '{_selectedTechMap.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteTechMapAsync(_selectedTechMap.Id);
                await LoadData();
                MessageBox.Show("Технологическая карта удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task ApproveTechMap()
        {
            if (_selectedTechMap == null)
            {
                MessageBox.Show("Выберите технологическую карту");
                return;
            }

            try
            {
                // Нужно получить реальный ID текущего пользователя
                int approvedBy = 1;
                await _api.ApproveTechMapAsync(_selectedTechMap.Id, approvedBy);
                await LoadData();
                MessageBox.Show("Технологическая карта утверждена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}