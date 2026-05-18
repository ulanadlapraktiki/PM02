using System;
using System.Threading.Tasks;
using System.Windows;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Dialogs
{
    public partial class AddRecipeDialog : Window
    {
        public Recipe? CreatedRecipe { get; private set; }
        private readonly ApiService _api;

        public AddRecipeDialog(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnSave.Click += async (s, e) => await Save();
            btnCancel.Click += (s, e) => DialogResult = false;
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название рецептуры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var recipe = new Recipe
            {
                Name = txtName.Text,
                Version = int.TryParse(txtVersion.Text, out int v) ? v : 1,
                Status = "draft"
            };

            try
            {
                var result = await _api.CreateRecipeAsync(recipe);
                if (result != null)
                {
                    CreatedRecipe = result;
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