using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class ReportsView : UserControl
    {
        private readonly ApiService _api;

        public ReportsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dpStart.SelectedDate = DateTime.Now.AddDays(-30);
            dpEnd.SelectedDate = DateTime.Now;

            btnGenerate.Click += async (s, e) => await GenerateReport();
            btnExportExcel.Click += async (s, e) => await ExportToExcel();

            btnToday.Click += (s, e) => SetDateRange(0);
            btnWeek.Click += (s, e) => SetDateRange(7);
            btnMonth.Click += (s, e) => SetDateRange(30);
            btnQuarter.Click += (s, e) => SetDateRange(90);

            Loaded += async (s, e) => await GenerateReport();
        }

        private void SetDateRange(int days)
        {
            dpStart.SelectedDate = DateTime.Now.AddDays(-days);
            dpEnd.SelectedDate = DateTime.Now;
            _ = GenerateReport();
        }

        private async Task GenerateReport()
        {
            try
            {
                progress.Visibility = Visibility.Visible;
                btnGenerate.IsEnabled = false;

                var start = dpStart.SelectedDate ?? DateTime.Now.AddDays(-30);
                var end = dpEnd.SelectedDate ?? DateTime.Now;
                var reportType = cmbReportType.SelectedIndex;

                lblDateRange.Text = $"Период: {start:dd.MM.yyyy} - {end:dd.MM.yyyy}";

                switch (reportType)
                {
                    case 0: await LoadBatchReport(start, end); break;
                    case 1: await LoadDeviationsReport(start, end); break;
                    case 2: await LoadRecipesReport(); break;
                    case 3: await LoadProductsReport(); break;
                    case 4: await LoadExtruderReport(); break;
                    default: await LoadBatchReport(start, end); break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                progress.Visibility = Visibility.Collapsed;
                btnGenerate.IsEnabled = true;
            }
        }

        private async Task LoadBatchReport(DateTime start, DateTime end)
        {
            var data = await _api.GetBatchesAsync();
            if (data == null) return;

            var filtered = data.Where(b => (b.StartTime >= start && b.StartTime <= end) ||
                                           (b.EndTime >= start && b.EndTime <= end))
                               .Select(b => new
                               {
                                   Номер = b.BatchNumber,
                                   Статус = b.Status,
                                   Начало = b.StartTime?.ToString("dd.MM.yyyy HH:mm"),
                                   Окончание = b.EndTime?.ToString("dd.MM.yyyy HH:mm"),
                                   Количество_кг = b.ActualQuantityKg
                               }).ToList();

            grid.ItemsSource = filtered;
            lblTotalInfo.Text = $"Всего партий: {filtered.Count}";
            lblTotalSum.Text = $"Общее количество: {filtered.Sum(x => x.Количество_кг):N0} кг";
        }

        private async Task LoadDeviationsReport(DateTime start, DateTime end)
        {
            var data = await _api.GetDeviationsAsync();
            if (data == null) return;

            var filtered = data.Where(d => d.ReportedAt >= start && d.ReportedAt <= end)
                               .Select(d => new
                               {
                                   Партия = d.BatchId,
                                   Тип = d.DeviationType,
                                   Описание = d.Description,
                                   Важность = d.Severity,
                                   Дата = d.ReportedAt.ToString("dd.MM.yyyy HH:mm"),
                                   Статус = d.ResolvedAt == null ? "Активно" : "Закрыто"
                               }).ToList();

            grid.ItemsSource = filtered;
            lblTotalInfo.Text = $"Всего отклонений: {filtered.Count}";
            lblTotalSum.Text = $"Активных: {filtered.Count(x => x.Статус == "Активно")}";
        }

        private async Task LoadRecipesReport()
        {
            var data = await _api.GetRecipesAsync();
            if (data == null) return;

            var filtered = data.Select(r => new
            {
                Название = r.Name,
                Версия = r.Version,
                Статус = r.Status,
                Продукт = r.ProductId,
                Дата = r.CreatedAt.ToString("dd.MM.yyyy")
            }).ToList();

            grid.ItemsSource = filtered;
            lblTotalInfo.Text = $"Всего рецептур: {filtered.Count}";
            lblTotalSum.Text = $"Активных: {filtered.Count(x => x.Статус == "active")}";
        }

        private async Task LoadProductsReport()
        {
            var data = await _api.GetProductsAsync();
            if (data == null) return;

            var filtered = data.Select(p => new
            {
                Код = p.Code,
                Наименование = p.Name,
                Тип = p.ProductType,
                Форма = p.Form,
                Статус = p.Status
            }).ToList();

            grid.ItemsSource = filtered;
            lblTotalInfo.Text = $"Всего продуктов: {filtered.Count}";
            lblTotalSum.Text = "";
        }

        private async Task LoadExtruderReport()
        {
            var data = await _api.GetExtruderProgramsAsync();
            if (data == null) return;

            var filtered = data.Select(p => new
            {
                Название = p.Name,
                Описание = p.Description,
                Статус = p.Status,
                Рецептура = p.RecipeId,
                Дата = p.CreatedAt.ToString("dd.MM.yyyy")
            }).ToList();

            grid.ItemsSource = filtered;
            lblTotalInfo.Text = $"Всего программ: {filtered.Count}";
            lblTotalSum.Text = $"Активных: {filtered.Count(x => x.Статус == "active")}";
        }

        private async Task ExportToExcel()
        {
            if (grid.ItemsSource == null)
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Отчет_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                // Установка лицензии EPPlus
                ExcelPackage.License.SetNonCommercialPersonal("TechnologistModule");

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Отчет");

                var data = grid.ItemsSource as IEnumerable<object>;
                var list = data?.ToList();
                if (list != null && list.Count > 0)
                {
                    var props = list[0].GetType().GetProperties();
                    for (int i = 0; i < props.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = props[i].Name;
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    for (int row = 0; row < list.Count; row++)
                    {
                        var item = list[row];
                        for (int col = 0; col < props.Length; col++)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = props[col].GetValue(item)?.ToString();
                        }
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                // Сохраняем файл
                var fileInfo = new FileInfo(dialog.FileName);
                package.SaveAs(fileInfo);

                MessageBox.Show("Отчет экспортирован", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}