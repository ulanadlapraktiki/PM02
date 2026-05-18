using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Models;
using TechnologistModule.Services;

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
            btnCancel.Click += (s, e) => Close();
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название");
                return;
            }

            // ПРАВИЛЬНОЕ ПОЛУЧЕНИЕ ЗНАЧЕНИЙ ИЗ COMBOBOX
            string selectedType = "";
            if (cmbType.SelectedItem is ComboBoxItem item)
            {
                selectedType = item.Content.ToString();
            }

            string selectedForm = "";
            if (cmbForm.SelectedItem is ComboBoxItem formItem)
            {
                selectedForm = formItem.Content.ToString();
            }

            var product = new Product
            {
                Code = txtCode.Text,
                Name = txtName.Text,
                ProductType = selectedType, 
                Form = selectedForm, 
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
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}