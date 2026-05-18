using System;
using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class AddBatchDialog : Window
    {
        public ProductionBatch? CreatedBatch { get; private set; }
        private readonly ApiService _api;

        public AddBatchDialog(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => DialogResult = false;
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text))
            {
                MessageBox.Show("Введите номер партии", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var batch = new ProductionBatch
            {
                BatchNumber = txtNumber.Text,
                OrderId = int.TryParse(txtOrderId.Text, out int id) ? id : (int?)null,
                Status = "planned"
            };

            try
            {
                var result = await _api.CreateBatchAsync(batch);
                if (result != null)
                {
                    CreatedBatch = result;
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