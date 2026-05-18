using LaboratoryModule.Dialogs;
using LaboratoryModule.Models;
using LaboratoryModule.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace LaboratoryModule.Views
{
    public partial class FinishedProductBatchesView : UserControl
    {
        private readonly ApiService _api;
        private List<ProductionBatch>? _batches;

        public FinishedProductBatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            btnRefresh.Click += async (s, e) => await LoadData();
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _batches = await _api.GetFinishedProductBatchesForControlAsync();
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
            // Можно добавить логику при выборе строки
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = grid.SelectedItem as ProductionBatch;
            if (selected == null) return;

            var testWindow = new TestSessionWindow(_api, selected.Id, "finished_product",
                selected.BatchNumber, selected.ProductName);
            testWindow.Owner = Window.GetWindow(this);
            testWindow.ShowDialog();

            _ = LoadData();
        }

        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var batch = button?.Tag as ProductionBatch;
            if (batch == null) return;

            var testWindow = new TestSessionWindow(_api, batch.Id, "finished_product",
                batch.BatchNumber, batch.ProductName);
            testWindow.Owner = Window.GetWindow(this);
            testWindow.ShowDialog();

            _ = LoadData();
        }
    }
}