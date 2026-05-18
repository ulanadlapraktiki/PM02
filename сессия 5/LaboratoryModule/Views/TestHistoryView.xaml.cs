using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LaboratoryModule.Models;
using LaboratoryModule.Services;
using LaboratoryModule.Dialogs;

namespace LaboratoryModule.Views
{
    public partial class TestHistoryView : UserControl
    {
        private readonly ApiService _api;
        private List<TestHistoryItem>? _tests;

        public TestHistoryView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            btnRefresh.Click += async (s, e) => await LoadData();
            btnExportExcel.Click += async (s, e) => await ExportToCsv();
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _tests = await _api.GetTestHistoryAsync();
                grid.ItemsSource = _tests;
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

        private async Task ExportToCsv()
        {
            if (_tests == null || _tests.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"История_испытаний_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();

                    // Заголовки
                    sb.AppendLine("ID;Номер партии;Тип;Статус;Решение;Лаборант;Дата;Комментарий");

                    // Данные
                    foreach (var item in _tests)
                    {
                        sb.AppendLine($"{item.Id};{item.BatchNumber};{item.SampleType};{item.Status};{item.Decision};{item.AnalystName};{item.CreatedAt:dd.MM.yyyy HH:mm};{item.Comment}");
                    }

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show($"Экспорт выполнен успешно!\nФайл: {dialog.FileName}", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}");
                }
            }
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = grid.SelectedItem as TestHistoryItem;
            if (selected == null) return;

            MessageBox.Show($"Партия: {selected.BatchNumber}\nРешение: {selected.Decision}\nКомментарий: {selected.Comment}\nДата: {selected.CreatedAt:dd.MM.yyyy HH:mm}",
                "Детали испытания", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}