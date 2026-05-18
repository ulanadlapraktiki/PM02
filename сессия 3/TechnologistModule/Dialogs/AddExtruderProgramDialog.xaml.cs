using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class AddExtruderProgramDialog : Window
    {
        public ExtruderProgram? CreatedProgram { get; private set; }
        private readonly ApiService _api;

        public AddExtruderProgramDialog(ApiService api)
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

            var program = new ExtruderProgram
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                RecipeId = int.TryParse(txtRecipeId.Text, out int r) ? r : (int?)null,
                EquipmentId = int.TryParse(txtEquipmentId.Text, out int e) ? e : (int?)null,
                Status = "draft"
            };

            var result = await _api.CreateExtruderProgramAsync(program);
            if (result != null)
            {
                CreatedProgram = result;
                DialogResult = true;
                Close();
            }
        }
    }
}