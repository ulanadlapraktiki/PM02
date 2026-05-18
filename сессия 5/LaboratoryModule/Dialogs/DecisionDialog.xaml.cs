using System.Windows;
using System.Windows.Media;

namespace LaboratoryModule.Dialogs
{
    public partial class DecisionDialog : Window
    {
        public string Decision { get; private set; } = string.Empty;
        public string Comment { get; private set; } = string.Empty;

        public DecisionDialog(bool allParametersValid)
        {
            InitializeComponent();

            if (!allParametersValid)
            {
                rbApprove.IsEnabled = false;
                rbBlock.IsChecked = true;
                lblStatus.Text = "❌ Есть отклонения от нормы!";
                statusBorder.Background = new SolidColorBrush(Colors.Red);
            }

            btnConfirm.Click += (s, e) => Confirm();
            btnCancel.Click += (s, e) => Close();
        }

        private void Confirm()
        {
            if (rbBlock.IsChecked == true && string.IsNullOrWhiteSpace(txtComment.Text))
            {
                lblError.Text = "При блокировке партии необходимо указать причину!";
                return;
            }

            Decision = rbApprove.IsChecked == true ? "approved" : "blocked";
            Comment = txtComment.Text;

            DialogResult = true;
            Close();
        }
    }
}