using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LaboratoryModule.Models;
using LaboratoryModule.Services;
using LaboratoryModule.Dialogs;

namespace LaboratoryModule.Views
{
    public partial class RawMaterialBatchesView : UserControl
    {
        private readonly ApiService _api;
        private List<RawMaterialDto>? _allBatches;

        public RawMaterialBatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnRefresh.Click += async (s, e) => await LoadData();
            btnSearch.Click += (s, e) => ApplyFilters();
            btnReset.Click += (s, e) => ResetFilters();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _allBatches = await _api.GetRawMaterialBatchesAsync();
                await LoadSuppliers();
                ApplyFilters();
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

        private async Task LoadSuppliers()
        {
            if (_allBatches == null) return;
            var suppliers = _allBatches.Select(b => b.SupplierName).Distinct().ToList();
            cmbSupplier.Items.Clear();
            cmbSupplier.Items.Add("Все поставщики");
            foreach (var supplier in suppliers)
                cmbSupplier.Items.Add(supplier);
            cmbSupplier.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            if (_allBatches == null) return;

            var query = _allBatches.AsEnumerable();

            // Поиск по тексту
            if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Поиск по номеру или материалу")
            {
                var search = txtSearch.Text.ToLower();
                query = query.Where(b =>
                    (b.BatchNumber?.ToLower().Contains(search) ?? false) ||
                    (b.MaterialName?.ToLower().Contains(search) ?? false));
            }

            // Фильтр по статусу - ИСПРАВЛЕНО
            if (cmbStatus.SelectedIndex > 0)
            {
                string selectedStatus = cmbStatus.SelectedItem.ToString();
                string statusKey = selectedStatus switch
                {
                    "Ожидает анализа" => "awaiting",
                    "В работе" => "in_progress",
                    "Разрешено" => "approved",
                    "Заблокировано" => "blocked",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(statusKey))
                {
                    query = query.Where(b => b.LabStatus == statusKey);
                }
            }

            // Фильтр по поставщику
            if (cmbSupplier.SelectedIndex > 0 && cmbSupplier.SelectedItem.ToString() != "Все поставщики")
            {
                var supplier = cmbSupplier.SelectedItem.ToString();
                query = query.Where(b => b.SupplierName == supplier);
            }

            // Фильтр по дате
            if (dpStart.SelectedDate.HasValue)
                query = query.Where(b => b.ArrivalDate.Date >= dpStart.SelectedDate.Value.Date);
            if (dpEnd.SelectedDate.HasValue)
                query = query.Where(b => b.ArrivalDate.Date <= dpEnd.SelectedDate.Value.Date);

            grid.ItemsSource = query.ToList();
        }

        private void ResetFilters()
        {
            txtSearch.Text = "";
            cmbStatus.SelectedIndex = 0;
            cmbSupplier.SelectedIndex = 0;
            dpStart.SelectedDate = null;
            dpEnd.SelectedDate = null;
            ApplyFilters();
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить логику при выборе строки
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = grid.SelectedItem as RawMaterialDto;
            if (selected == null) return;

            var testWindow = new TestSessionWindow(_api, selected.Id, "raw_material",
                selected.BatchNumber, selected.MaterialName);
            testWindow.Owner = Window.GetWindow(this);
            testWindow.ShowDialog();

            _ = LoadData();
        }

        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var batch = button?.Tag as RawMaterialDto;
            if (batch == null) return;

            // Нужно найти соответствующий Id в production_batches
            var prodBatchId = GetProductionBatchIdByRawBatchNumber(batch.BatchNumber);

            var testWindow = new TestSessionWindow(_api, prodBatchId, "raw_material",
                batch.BatchNumber, batch.MaterialName);
            testWindow.Owner = Window.GetWindow(this);
            testWindow.ShowDialog();

            _ = LoadData();
        }

        private int GetProductionBatchIdByRawBatchNumber(string batchNumber)
        {
            // Временно: возвращаем Id из production_batches для RAW-003 = 29
            // Позже можно сделать запрос к API или БД
            return batchNumber switch
            {
                "RAW-001" => 27,
                "RAW-002" => 28,
                "RAW-003" => 29,
                "RAW-004" => 30,
                "RAW-005" => 31,
                _ => 0
            };
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Поиск по номеру или материалу")
            {
                txtSearch.Text = "";
                txtSearch.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Поиск по номеру или материалу";
                txtSearch.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}