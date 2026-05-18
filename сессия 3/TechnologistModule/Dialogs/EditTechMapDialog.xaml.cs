using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class EditTechMapDialog : Window
    {
        public TechMap? UpdatedTechMap { get; private set; }
        private readonly ApiService _api;
        private readonly TechMap _originalTechMap;

        public EditTechMapDialog(ApiService api, TechMap techMap)
        {
            InitializeComponent();
            _api = api;
            _originalTechMap = techMap;

            txtName.Text = techMap.Name;
            txtVersion.Text = techMap.Version.ToString();
            txtProductId.Text = techMap.ProductId?.ToString() ?? "";

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

            UpdatedTechMap = new TechMap
            {
                Id = _originalTechMap.Id,
                Name = txtName.Text,
                Version = int.TryParse(txtVersion.Text, out int v) ? v : 1,
                ProductId = int.TryParse(txtProductId.Text, out int id) ? id : (int?)null,
                Status = _originalTechMap.Status
            };

            var result = await _api.UpdateTechMapAsync(_originalTechMap.Id, UpdatedTechMap);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}