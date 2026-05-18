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
    public partial class ExtruderView : UserControl
    {
        private readonly ApiService _api;
        private List<ExtruderProgram>? _programs;
        private ExtruderProgram? _selectedProgram;
        private ExtruderProgramZone? _selectedZone;

        public ExtruderView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddProgram();
            btnEdit.Click += async (s, e) => await EditProgram();
            btnDelete.Click += async (s, e) => await DeleteProgram();
            btnActivate.Click += async (s, e) => await ActivateProgram();
            btnRefresh.Click += async (s, e) => await LoadData();
            btnAddZone.Click += async (s, e) => await AddZone();
            btnDeleteZone.Click += async (s, e) => await DeleteZone();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _programs = await _api.GetExtruderProgramsAsync();
                grid.ItemsSource = _programs;
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
            _selectedProgram = grid.SelectedItem as ExtruderProgram;
            bool isSelected = _selectedProgram != null;
            btnEdit.IsEnabled = isSelected;
            btnDelete.IsEnabled = isSelected;
            btnActivate.IsEnabled = isSelected && _selectedProgram?.Status != "active";
            btnAddZone.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;

            if (isSelected)
                LoadZones();
            else
                zonesGrid.ItemsSource = null;
        }

        private async void ZonesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedZone = zonesGrid.SelectedItem as ExtruderProgramZone;
            btnDeleteZone.Visibility = _selectedZone != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task LoadZones()
        {
            if (_selectedProgram == null) return;

            try
            {
                var zones = await _api.GetExtruderProgramZonesAsync(_selectedProgram.Id);
                zonesGrid.ItemsSource = zones;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки зон: {ex.Message}");
            }
        }

        private async Task AddProgram()
        {
            var dialog = new AddExtruderProgramDialog(_api);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task EditProgram()
        {
            if (_selectedProgram == null) return;

            var dialog = new EditExtruderProgramDialog(_api, _selectedProgram);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task DeleteProgram()
        {
            if (_selectedProgram == null) return;

            if (MessageBox.Show($"Удалить программу '{_selectedProgram.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteExtruderProgramAsync(_selectedProgram.Id);
                await LoadData();
                MessageBox.Show("Программа удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task ActivateProgram()
        {
            if (_selectedProgram == null) return;

            try
            {
                await _api.ActivateExtruderProgramAsync(_selectedProgram.Id);
                await LoadData();
                MessageBox.Show("Программа активирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task AddZone()
        {
            if (_selectedProgram == null) return;

            var dialog = new AddExtruderZoneDialog(_api, _selectedProgram.Id);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadZones();
        }

        private async Task DeleteZone()
        {
            if (_selectedZone == null) return;

            if (MessageBox.Show("Удалить зону?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteExtruderZoneAsync(_selectedZone.Id);
                await LoadZones();
                _selectedZone = null;
                btnDeleteZone.Visibility = Visibility.Collapsed;
                MessageBox.Show("Зона удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}