using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class EditBatchDialog : Window
    {
        public ProductionBatch? UpdatedBatch { get; private set; }
        private readonly ApiService _api;
        private readonly ProductionBatch _originalBatch;

        public EditBatchDialog(ApiService api, ProductionBatch batch)
        {
            InitializeComponent();
            _api = api;
            _originalBatch = batch;

            txtBatchNumber.Text = batch.BatchNumber;
            txtOrderId.Text = batch.OrderId?.ToString() ?? "";
            txtActualQuantity.Text = batch.ActualQuantityKg.ToString();

            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => Close();
        }

        private async Task Save()
        {
            if (!decimal.TryParse(txtActualQuantity.Text, out decimal quantity))
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            UpdatedBatch = new ProductionBatch
            {
                Id = _originalBatch.Id,
                BatchNumber = _originalBatch.BatchNumber,
                OrderId = _originalBatch.OrderId,
                ActualQuantityKg = quantity,
                Status = _originalBatch.Status,
                StartTime = _originalBatch.StartTime,
                EndTime = _originalBatch.EndTime
            };

            var result = await _api.UpdateBatchAsync(_originalBatch.Id, UpdatedBatch);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}