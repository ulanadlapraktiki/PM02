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
    public partial class ProductsView : UserControl
    {
        private readonly ApiService _api;
        private List<Product>? _products;
        private Product? _selectedProduct;

        public ProductsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddProduct();
            btnEdit.Click += async (s, e) => await EditProduct();
            btnDelete.Click += async (s, e) => await DeleteProduct();
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

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = grid.SelectedItem as Product;
            btnEdit.IsEnabled = _selectedProduct != null;
            btnDelete.IsEnabled = _selectedProduct != null;
        }

        private async Task AddProduct()
        {
            var dialog = new AddProductDialog(_api);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task EditProduct()
        {
            if (_selectedProduct == null) return;

            var dialog = new EditProductDialog(_api, _selectedProduct);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task DeleteProduct()
        {
            if (_selectedProduct == null) return;

            if (MessageBox.Show($"Удалить продукт '{_selectedProduct.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteProductAsync(_selectedProduct.Id);
                await LoadData();
                MessageBox.Show("Продукт удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}