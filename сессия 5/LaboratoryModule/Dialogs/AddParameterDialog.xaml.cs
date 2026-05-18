using System.Windows;
using LaboratoryModule.Models;

namespace LaboratoryModule.Dialogs
{
    public partial class AddParameterDialog : Window
    {
        public TestResult? NewParameter { get; private set; }

        public AddParameterDialog()
        {
            InitializeComponent();
            btnSave.Click += (s, e) => Save();
            btnCancel.Click += (s, e) => Close();
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(txtParameterName.Text))
            {
                MessageBox.Show("Введите наименование параметра");
                return;
            }

            if (!decimal.TryParse(txtMinValue.Text, out decimal min))
            {
                MessageBox.Show("Введите корректное минимальное значение");
                return;
            }

            if (!decimal.TryParse(txtMaxValue.Text, out decimal max))
            {
                MessageBox.Show("Введите корректное максимальное значение");
                return;
            }

            NewParameter = new TestResult
            {
                ParameterName = txtParameterName.Text,
                MinValue = min,
                MaxValue = max,
                Unit = txtUnit.Text,
                IsValid = false
            };

            DialogResult = true;
            Close();
        }
    }
}