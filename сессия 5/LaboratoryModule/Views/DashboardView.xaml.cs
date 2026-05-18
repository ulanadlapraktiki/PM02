using System;
using System.Windows;
using System.Windows.Controls;
using LaboratoryModule.Services;
using LaboratoryModule.Models;

namespace LaboratoryModule.Views
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
                    txtPendingRaw.Text = data.PendingRawMaterials.ToString();
                    txtPendingFinished.Text = data.PendingFinishedProducts.ToString();
                    txtInProgress.Text = data.TestsInProgress.ToString();
                    txtCompletedToday.Text = data.CompletedToday.ToString();
                    txtBlocked.Text = data.BlockedBatches.ToString();
                    txtApproved.Text = data.ApprovedBatches.ToString();
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