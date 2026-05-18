using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnologistModule.Services;

namespace TechnologistModule.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly ApiService _api;

        public DashboardView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            progress.Visibility = Visibility.Visible;
            try
            {
                var data = await _api.GetDashboardAsync();
                if (data != null)
                {
                    txtProducts.Text = data.ActiveProducts.ToString();
                    txtRecipes.Text = data.ActiveRecipes.ToString();
                    txtBatches.Text = data.BatchesInProgress.ToString();
                    txtPending.Text = data.PendingQualityControl.ToString();
                    txtDeviations.Text = data.DeviationsToday.ToString();
                    lblUpdate.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";
                }
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
    }
}