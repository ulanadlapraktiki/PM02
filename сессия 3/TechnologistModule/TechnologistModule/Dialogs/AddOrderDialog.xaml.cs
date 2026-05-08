using System;
using System.Threading.Tasks;
using System.Windows;

namespace TechnologistModule.Dialogs
{
    public partial class AddOrderDialog : Window
    {
        public ProductionOrder? CreatedOrder { get; private set; }
        private readonly ApiService _api;

        public AddOrderDialog(ApiService api)
        {
            InitializeComponent();
            _api = api;

            // Устанавливаем дату по умолчанию в коде
            dpDate.SelectedDate = DateTime.Today;

            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => DialogResult = false;
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text))
            {
                MessageBox.Show("Введите номер заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var order = new ProductionOrder
            {
                OrderNumber = txtNumber.Text,
                PlannedQuantityKg = decimal.TryParse(txtQuantity.Text, out decimal q) ? q : 1000,
                PlannedStartDate = dpDate.SelectedDate,
                Status = "planned"
            };

            try
            {
                var result = await _api.CreateOrderAsync(order);
                if (result != null)
                {
                    CreatedOrder = result;
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