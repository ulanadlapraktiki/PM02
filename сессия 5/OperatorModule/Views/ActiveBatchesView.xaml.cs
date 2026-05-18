using OperatorModule.Models;
using OperatorModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OperatorModule.Views
{
    public partial class ActiveBatchesView : UserControl
    {
        private readonly ApiService _api;
        private List<ActiveBatchDto>? _batches;

        public ActiveBatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            btnRefresh.Click += async (s, e) => await LoadData();
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var runningBatches = await _api.GetRunningBatchesAsync();
                if (runningBatches == null || runningBatches.Count == 0)
                {
                    grid.ItemsSource = null;
                    return;
                }

                _batches = new List<ActiveBatchDto>();
                foreach (var batch in runningBatches)
                {
                    var steps = await _api.GetStepsByBatchAsync(batch.Id);
                    var currentStep = steps?.FirstOrDefault(s => s.StartedAt != null && s.CompletedAt == null);

                    _batches.Add(new ActiveBatchDto
                    {
                        Id = batch.Id,
                        BatchNumber = batch.BatchNumber,
                        ProductName = await _api.GetProductNameByOrderId(batch.OrderId),
                        LineNumber = 1,
                        CurrentStep = currentStep?.StepName ?? "Ожидание",
                        BatchStatus = batch.Status == "running" ? "В работе" : batch.Status,
                        StepStatus = currentStep?.StartedAt != null ? "in_progress" : "pending",
                        HasWarning = false,
                        HasCriticalDeviation = false
                    });
                }

                grid.ItemsSource = _batches;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = grid.SelectedItem as ActiveBatchDto;
            if (selected != null)
                OpenBatchProgram(selected.Id);
        }

        private void BtnGoToProgram_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var batch = button?.Tag as ActiveBatchDto;
            if (batch != null)
                OpenBatchProgram(batch.Id);
        }

        private void OpenBatchProgram(int batchId)
        {
            var programView = new BatchProgramView(_api, batchId);
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.ShowBatchProgramView(programView);
        }
    }
}