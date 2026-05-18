using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class EditExtruderProgramDialog : Window
    {
        public ExtruderProgram? UpdatedProgram { get; private set; }
        private readonly ApiService _api;
        private readonly ExtruderProgram _originalProgram;

        public EditExtruderProgramDialog(ApiService api, ExtruderProgram program)
        {
            InitializeComponent();
            _api = api;
            _originalProgram = program;

            txtName.Text = program.Name;
            txtDescription.Text = program.Description;
            txtRecipeId.Text = program.RecipeId?.ToString() ?? "";
            txtEquipmentId.Text = program.EquipmentId?.ToString() ?? "";

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

            UpdatedProgram = new ExtruderProgram
            {
                Id = _originalProgram.Id,
                Name = txtName.Text,
                Description = txtDescription.Text,
                RecipeId = int.TryParse(txtRecipeId.Text, out int r) ? r : (int?)null,
                EquipmentId = int.TryParse(txtEquipmentId.Text, out int e) ? e : (int?)null,
                Status = _originalProgram.Status
            };

            var result = await _api.UpdateExtruderProgramAsync(_originalProgram.Id, UpdatedProgram);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}