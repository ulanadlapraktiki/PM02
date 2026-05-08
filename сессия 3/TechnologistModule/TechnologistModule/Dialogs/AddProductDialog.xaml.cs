using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TechnologistModule.Dialogs
{
    public partial class AddProductDialog : Window
    {
        public Product? CreatedProduct { get; private set; }
        private readonly ApiService _api;

        public AddProductDialog(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => DialogResult = false;

            Loaded += (s, e) => txtCode.Focus();
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование продукта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = new Product
            {
                Code = txtCode.Text,
                Name = txtName.Text,
                ProductType = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "",
                Form = (cmbForm.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "",
                Status = "active"
            };

            try
            {
                var result = await _api.CreateProductAsync(product);
                if (result != null)
                {
                    CreatedProduct = result;
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}