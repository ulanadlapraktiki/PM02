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
    public partial class BatchesView : UserControl
    {
        private readonly ApiService _api;
        private List<ProductionBatch>? _batches;
        private ProductionBatch? _selectedBatch;

        public BatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddBatch();
            btnEdit.Click += async (s, e) => await EditBatch();
            btnStart.Click += async (s, e) => await Start();
            btnComplete.Click += async (s, e) => await Complete();
            btnRefresh.Click += async (s, e) => await LoadData();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _batches = await _api.GetBatchesAsync();
                grid.ItemsSource = _batches;
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
            _selectedBatch = grid.SelectedItem as ProductionBatch;
            bool isSelected = _selectedBatch != null;
            btnEdit.IsEnabled = isSelected;
            btnDelete.IsEnabled = isSelected;

            if (_selectedBatch != null)
            {
                btnStart.IsEnabled = _selectedBatch.Status == "planned";
                btnComplete.IsEnabled = _selectedBatch.Status == "running";
            }
            else
            {
                btnStart.IsEnabled = false;
                btnComplete.IsEnabled = false;
            }
        }

        private async Task DeleteBatch()
        {
            if (_selectedBatch == null) return;

            if (MessageBox.Show($"Удалить партию '{_selectedBatch.BatchNumber}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteBatchAsync(_selectedBatch.Id);
                await LoadData();
                MessageBox.Show("Партия удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task AddBatch()
        {
            var dialog = new AddBatchDialog(_api);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task EditBatch()
        {
            if (_selectedBatch == null) return;

            var dialog = new EditBatchDialog(_api, _selectedBatch);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task Start()
        {
            if (_selectedBatch == null)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            try
            {
                await _api.StartBatchAsync(_selectedBatch.Id);
                await LoadData();
                MessageBox.Show("Партия запущена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task Complete()
        {
            if (_selectedBatch == null)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            try
            {
                await _api.CompleteBatchAsync(_selectedBatch.Id);
                await LoadData();
                MessageBox.Show("Партия завершена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}