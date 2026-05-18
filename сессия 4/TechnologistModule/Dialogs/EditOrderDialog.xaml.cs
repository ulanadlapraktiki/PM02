using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class EditOrderDialog : Window
    {
        public ProductionOrder? UpdatedOrder { get; private set; }
        private readonly ApiService _api;
        private readonly ProductionOrder _originalOrder;

        public EditOrderDialog(ApiService api, ProductionOrder order)
        {
            InitializeComponent();
            _api = api;
            _originalOrder = order;

            txtQuantity.Text = order.PlannedQuantityKg.ToString();
            dpDate.SelectedDate = order.PlannedStartDate;

            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => Close();
        }

        private async Task Save()
        {
            UpdatedOrder = new ProductionOrder
            {
                Id = _originalOrder.Id,
                OrderNumber = _originalOrder.OrderNumber,
                PlannedQuantityKg = decimal.TryParse(txtQuantity.Text, out decimal q) ? q : 0,
                PlannedStartDate = dpDate.SelectedDate,
                Status = _originalOrder.Status,
                RecipeId = _originalOrder.RecipeId
            };

            var result = await _api.UpdateOrderAsync(_originalOrder.Id, UpdatedOrder);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}