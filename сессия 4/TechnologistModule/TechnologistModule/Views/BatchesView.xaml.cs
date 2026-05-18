using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Dialogs;

namespace TechnologistModule.Views
{
    public partial class BatchesView : UserControl
    {
        private readonly ApiService _api;
        private List<ProductionBatch>? _batches;

        public BatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddBatch();
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

        private async Task AddBatch()
        {
            var dialog = new AddBatchDialog(_api);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true && dialog.CreatedBatch != null)
            {
                await LoadData();
                MessageBox.Show("Партия создана", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task Start()
        {
            var selected = grid.SelectedItem as ProductionBatch;
            if (selected == null)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            try
            {
                await _api.StartBatchAsync(selected.Id);
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
            var selected = grid.SelectedItem as ProductionBatch;
            if (selected == null)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            try
            {
                await _api.CompleteBatchAsync(selected.Id);
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