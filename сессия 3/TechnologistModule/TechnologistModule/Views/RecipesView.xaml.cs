using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Dialogs;

namespace TechnologistModule.Views
{
    public partial class RecipesView : UserControl
    {
        private readonly ApiService _api;
        private List<Recipe>? _recipes;

        public RecipesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddRecipe();
            btnApprove.Click += async (s, e) => await Approve();
            btnRefresh.Click += async (s, e) => await LoadData();

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                _recipes = await _api.GetRecipesAsync();
                grid.ItemsSource = _recipes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                progress.Visibility = Visibility.Collapsed;
            }
        }

        private async Task AddRecipe()
        {
            var dialog = new AddRecipeDialog(_api);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true && dialog.CreatedRecipe != null)
            {
                await LoadData();
                MessageBox.Show("Рецептура создана", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task Approve()
        {
            var selected = grid.SelectedItem as Recipe;
            if (selected == null)
            {
                MessageBox.Show("Выберите рецептуру");
                return;
            }

            try
            {
                await _api.ApproveRecipeAsync(selected.Id);
                await LoadData();
                MessageBox.Show("Рецептура утверждена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}