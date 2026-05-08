using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Dialogs;

namespace TechnologistModule.Views
{
    public partial class OrdersView : UserControl
    {
        private readonly ApiService _api;
        private List<ProductionOrder>? _orders;

        public OrdersView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddOrder();
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

        private async Task AddOrder()
        {
            var dialog = new AddOrderDialog(_api);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true && dialog.CreatedOrder != null)
            {
                await LoadData();
                MessageBox.Show("Заказ создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task Start()
        {
            var selected = grid.SelectedItem as ProductionOrder;
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            try
            {
                await _api.StartOrderAsync(selected.Id);
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