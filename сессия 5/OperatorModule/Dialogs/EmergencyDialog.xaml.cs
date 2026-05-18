using System.Windows;
using System.Windows.Controls;

namespace OperatorModule.Dialogs
{
    public partial class EmergencyDialog : Window
    {
        public string Reason { get; private set; } = string.Empty;
        public string Comment { get; private set; } = string.Empty;

        public EmergencyDialog()
        {
            InitializeComponent();

            btnConfirm.Click += (s, e) => Confirm();
            btnCancel.Click += (s, e) => Close();
        }

        private void Confirm()
        {
            Reason = (cmbReason.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Не указано";
            Comment = txtComment.Text;

            if (string.IsNullOrWhiteSpace(Comment))
            {
                lblError.Text = "Пожалуйста, укажите комментарий к аварийной остановке!";
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}