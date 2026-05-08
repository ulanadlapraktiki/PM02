using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TechnologistModule.Views
{
    public partial class ReportsView : UserControl
    {
        private readonly ApiService _api;

        public ReportsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnGenerate.Click += async (s, e) => await LoadReport();
            btnRefresh.Click += async (s, e) => await LoadReport();

            Loaded += async (s, e) => await LoadReport();
        }

        private async Task LoadReport()
        {
            progress.Visibility = Visibility.Visible;
            btnGenerate.IsEnabled = false;

            try
            {
                var data = await _api.GetBatchReportAsync();

                if (data != null && data.Count > 0)
                {
                    grid.ItemsSource = data;

                    int count = data.Count;
                    decimal totalQuantity = data.Sum(x => x.ActualQuantityKg);
                    int completed = data.Count(x => x.Status == "completed");
                    int running = data.Count(x => x.Status == "running");
                    int planned = data.Count(x => x.Status == "planned");
                    int pending = data.Count(x => x.Status == "quality_pending");

                    MessageBox.Show($"📊 Найдено {count} партий\n\n" +
                                   $"📦 Всего кг: {totalQuantity}\n" +
                                   $"✅ Завершено: {completed}\n" +
                                   $"▶️ В работе: {running}\n" +
                                   $"⏳ Ожидают контроля: {pending}\n" +
                                   $"📋 Запланировано: {planned}",
                                   "Отчет", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    grid.ItemsSource = null;
                    MessageBox.Show("Нет данных", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progress.Visibility = Visibility.Collapsed;
                btnGenerate.IsEnabled = true;
            }
        }
    }
}