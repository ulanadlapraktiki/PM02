using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class EditRecipeDialog : Window
    {
        public Recipe? UpdatedRecipe { get; private set; }
        private readonly ApiService _api;
        private readonly Recipe _originalRecipe;

        public EditRecipeDialog(ApiService api, Recipe recipe)
        {
            InitializeComponent();
            _api = api;
            _originalRecipe = recipe;

            txtName.Text = recipe.Name;
            txtVersion.Text = recipe.Version.ToString();
            txtProductId.Text = recipe.ProductId?.ToString() ?? "";

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

            UpdatedRecipe = new Recipe
            {
                Id = _originalRecipe.Id,
                Name = txtName.Text,
                Version = int.TryParse(txtVersion.Text, out int v) ? v : 1,
                ProductId = int.TryParse(txtProductId.Text, out int id) ? id : (int?)null,
                Status = _originalRecipe.Status
            };

            var result = await _api.UpdateRecipeAsync(_originalRecipe.Id, UpdatedRecipe);
            if (result != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}