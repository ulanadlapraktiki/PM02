using OperatorModule.Dialogs;
using OperatorModule.Models;
using OperatorModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OperatorModule.Views
{
    public partial class BatchProgramView : UserControl
    {
        private readonly ApiService _api;
        private readonly int _batchId;
        private List<StepItem> _steps = new();

        public BatchProgramView(ApiService api, int batchId)
        {
            InitializeComponent();
            _api = api;
            _batchId = batchId;
            Loaded += async (s, e) => await LoadData();
            btnCompleteBatch.Click += async (s, e) => await CompleteBatch();
            btnBack.Click += (s, e) => GoBack();
        }

        private async Task LoadData()
        {
            try
            {
                var batch = await _api.GetProductionBatchAsync(_batchId);
                if (batch != null)
                {
                    lblBatchNumber.Text = $"Партия: {batch.BatchNumber}";
                    lblProduct.Text = await _api.GetProductNameByOrderId(batch.OrderId);
                    lblStatus.Text = batch.Status == "running" ? "В работе" : batch.Status;
                    lblTime.Text = batch.StartTime?.ToString("dd.MM.yyyy HH:mm") ?? "";
                }

                var steps = await _api.GetStepsByBatchAsync(_batchId);
                if (steps != null)
                {
                    _steps = steps.Select(s => new StepItem
                    {
                        Id = s.Id,
                        StepOrder = s.StepOrder,
                        StepName = s.StepName,
                        Status = s.CompletedAt != null ? "completed" : (s.StartedAt != null ? "in_progress" : "pending"),
                        PlannedTempC = s.PlannedTempC,
                        PlannedPressureBar = s.PlannedPressureBar,
                        PlannedDurationMin = s.PlannedDurationMin
                    }).ToList();

                    listSteps.ItemsSource = _steps;
                    UpdateCompleteBatchButton();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void ListSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSteps.SelectedItem == null) return;
            var step = listSteps.SelectedItem as StepItem;
            if (step == null) return;

            if (step.Status == "pending")
            {
                var dialog = new StepExecutionDialog(step);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    await _api.StartStepAsync(step.Id);

                    var request = new CompleteStepRequest
                    {
                        StepId = step.Id,
                        ActualTempC = dialog.ActualTempC,
                        ActualDurationMin = dialog.ActualDurationMin,
                        ActualPressureBar = dialog.ActualPressureBar,
                        Comment = dialog.Comment
                    };

                    await _api.CompleteStepAsync(request);

                    // Перезагружаем данные
                    await LoadData();
                }
            }
        }

        private void UpdateCompleteBatchButton()
        {
            if (_steps == null || _steps.Count == 0)
            {
                btnCompleteBatch.IsEnabled = false;
                return;
            }
            bool allCompleted = _steps.All(s => s.Status == "completed");
            btnCompleteBatch.IsEnabled = allCompleted;
        }

        private async Task CompleteBatch()
        {
            var result = MessageBox.Show("Завершить партию?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _api.CompleteBatchAsync(_batchId);
                    if (success)
                    {
                        MessageBox.Show("Партия завершена!");
                        GoBack();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при завершении партии: API вернул false", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при завершении партии: {ex.Message}\n{ex.StackTrace}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GoBack()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.ShowActiveBatchesView();
        }
        private string GetInstruction(string stepName)
        {
            switch (stepName)
            {
                case "Смешивание": return "Загрузите компоненты. Температура 45°C, 30 минут.";
                case "Выдержка": return "Выдержка при 60°C в течение 2 часов.";
                case "Экструзия": return "Подайте смесь в экструдер. Контролируйте параметры.";
                case "Охлаждение": return "Охлаждение до 25°C, 60 минут.";
                default: return "Выполните операцию по инструкции.";
            }
        }
    }

    public class StepItem
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Instruction { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public decimal? PlannedTempC { get; set; }
        public decimal? PlannedPressureBar { get; set; }
        public int? PlannedDurationMin { get; set; }
    }
}