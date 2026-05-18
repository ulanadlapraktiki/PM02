using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
                    txtActiveProducts.Text = data.ActiveProducts.ToString();
                    txtActiveRecipes.Text = data.ActiveRecipes.ToString();
                    txtBatchesRunning.Text = data.BatchesInProgress.ToString();
                    txtPendingQC.Text = data.PendingQualityControl.ToString();
                    txtDeviationsToday.Text = data.DeviationsToday.ToString();
                    txtUnresolved.Text = data.UnresolvedDeviations.ToString();
                    lblLastUpdate.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";
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