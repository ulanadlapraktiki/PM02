using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Dialogs;
using TechnologistModule.Models;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class RecipesView : UserControl
    {
        private readonly ApiService _api;
        private List<Recipe>? _recipes;
        private Recipe? _selectedRecipe;

        public RecipesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            btnAdd.Click += async (s, e) => await AddRecipe();
            btnApprove.Click += async (s, e) => await ApproveRecipe();
            btnEdit.Click += async (s, e) => await EditRecipe();
            btnDelete.Click += async (s, e) => await DeleteRecipe();
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

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedRecipe = grid.SelectedItem as Recipe;
            bool isSelected = _selectedRecipe != null;
            btnEdit.IsEnabled = isSelected;
            btnDelete.IsEnabled = isSelected;
            btnApprove.IsEnabled = isSelected && _selectedRecipe?.Status != "active";
        }

        private async Task AddRecipe()
        {
            var dialog = new AddRecipeDialog(_api);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task ApproveRecipe()
        {
            if (_selectedRecipe == null)
            {
                MessageBox.Show("Выберите рецептуру");
                return;
            }

            try
            {
                await _api.ApproveRecipeAsync(_selectedRecipe.Id);
                await LoadData();
                MessageBox.Show("Рецептура утверждена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task EditRecipe()
        {
            if (_selectedRecipe == null) return;

            var dialog = new EditRecipeDialog(_api, _selectedRecipe);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
                await LoadData();
        }

        private async Task DeleteRecipe()
        {
            if (_selectedRecipe == null) return;

            if (MessageBox.Show($"Удалить рецептуру '{_selectedRecipe.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                await _api.DeleteRecipeAsync(_selectedRecipe.Id);
                await LoadData();
                MessageBox.Show("Рецептура удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}