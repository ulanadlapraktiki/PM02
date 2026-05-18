using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class EditProductDialog : Window
    {
        public Product? UpdatedProduct { get; private set; }
        private readonly ApiService _api;
        private readonly Product _originalProduct;

        public EditProductDialog(ApiService api, Product product)
        {
            InitializeComponent();
            _api = api;
            _originalProduct = product;

            txtCode.Text = product.Code;
            txtName.Text = product.Name;

            // Установка выбранных значений
            for (int i = 0; i < cmbType.Items.Count; i++)
            {
                var item = cmbType.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == product.ProductType)
                {
                    cmbType.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cmbForm.Items.Count; i++)
            {
                var item = cmbForm.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == product.Form)
                {
                    cmbForm.SelectedIndex = i;
                    break;
                }
            }

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
            if (cmbType.SelectedItem is ComboBoxItem typeItem)
            {
                selectedType = typeItem.Content.ToString();
            }

            string selectedForm = "";
            if (cmbForm.SelectedItem is ComboBoxItem formItem)
            {
                selectedForm = formItem.Content.ToString();
            }

            UpdatedProduct = new Product
            {
                Id = _originalProduct.Id,
                Code = txtCode.Text,
                Name = txtName.Text,
                ProductType = selectedType,
                Form = selectedForm,
                Status = _originalProduct.Status
            };

            var result = await _api.UpdateProductAsync(_originalProduct.Id, UpdatedProduct);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}