using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Dialogs;

namespace TechnologistModule.Views
{
    public partial class ProductsView : UserControl
    {
        private readonly ApiService _api;
        private List<Product>? _products;

        public ProductsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddProduct();
            btnRefresh.Click += async (s, e) => await LoadData();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _products = await _api.GetProductsAsync();
                grid.ItemsSource = _products;
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

        private async Task AddProduct()
        {
            var dialog = new AddProductDialog(_api);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true && dialog.CreatedProduct != null)
            {
                await LoadData();
                MessageBox.Show("Продукт добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}