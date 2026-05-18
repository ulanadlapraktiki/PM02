using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class AddTechMapDialog : Window
    {
        public TechMap? CreatedTechMap { get; private set; }
        private readonly ApiService _api;

        public AddTechMapDialog(ApiService api)
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

            var techMap = new TechMap
            {
                Name = txtName.Text,
                Version = int.TryParse(txtVersion.Text, out int v) ? v : 1,
                ProductId = int.TryParse(txtProductId.Text, out int id) ? id : (int?)null,
                Status = "draft"
            };

            var result = await _api.CreateTechMapAsync(techMap);
            if (result != null)
            {
                CreatedTechMap = result;
                DialogResult = true;
                Close();
            }
        }
    }
}