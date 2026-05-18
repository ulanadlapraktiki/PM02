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
    public partial class OrdersView : UserControl
    {
        private readonly ApiService _api;
        private List<ProductionOrder>? _orders;
        private ProductionOrder? _selectedOrder;

        public OrdersView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddOrder();
            btnEdit.Click += async (s, e) => await EditOrder();
            btnDelete.Click += async (s, e) => await DeleteOrder();
            btnStart.Click += async (s, e) => await Start();
            btnRefresh.Click += async (s, e) => await LoadData();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _orders = await _api.GetOrdersAsync();
                grid.ItemsSource = _orders;
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
            _selectedOrder = grid.SelectedItem as ProductionOrder;
            bool isSelected = _selectedOrder != null;
            btnEdit.IsEnabled = isSelected;
            btnDelete.IsEnabled = isSelected;
            btnStart.IsEnabled = isSelected && _selectedOrder?.Status == "planned";
        }

        private async Task AddOrder()
        {
            var dialog = new AddOrderDialog(_api);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task EditOrder()
        {
            if (_selectedOrder == null) return;

            var dialog = new EditOrderDialog(_api, _selectedOrder);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task DeleteOrder()
        {
            if (_selectedOrder == null) return;

            if (MessageBox.Show($"Удалить заказ '{_selectedOrder.OrderNumber}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteOrderAsync(_selectedOrder.Id);
                await LoadData();
                MessageBox.Show("Заказ удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task Start()
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            try
            {
                await _api.StartOrderAsync(_selectedOrder.Id);
                await LoadData();
                MessageBox.Show("Заказ запущен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}