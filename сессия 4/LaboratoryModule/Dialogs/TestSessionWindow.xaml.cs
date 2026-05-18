using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LaboratoryModule.Models;
using LaboratoryModule.Services;
using Newtonsoft.Json;

namespace LaboratoryModule.Dialogs
{
    public partial class TestSessionWindow : Window
    {
        private readonly ApiService _api;
        private readonly int _batchId;
        private readonly string _batchType;
        private readonly string _batchNumber;
        private readonly string _materialName;
        private int? _currentTestId;
        private List<TestResult> _parameters = new();
        private string? _currentDecision;

        public TestSessionWindow(ApiService api, int batchId, string batchType, string batchNumber, string materialName)
        {
            InitializeComponent();
            _api = api;
            _batchId = batchId;
            _batchType = batchType;
            _batchNumber = batchNumber;
            _materialName = materialName;

            lblBatchNumber.Text = batchNumber;
            lblMaterialName.Text = materialName;
            lblControlType.Text = batchType == "raw_material" ? "Входной контроль сырья" : "Приемочный контроль ГП";
            lblCreatedDate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            btnAddParameter.Click += async (s, e) => await AddParameter();
            btnSaveProgress.Click += async (s, e) => await SaveProgress();
            btnComplete.Click += async (s, e) => await CompleteTest();
            btnProtocol.Click += (s, e) => ShowProtocol();
            btnCancel.Click += (s, e) => Close();

            Loaded += async (s, e) => await LoadTest();
        }

        private async Task LoadTest()
        {
            try
            {
                var existingTests = await _api.GetTestHistoryAsync();
                var existingInProgress = existingTests?.FirstOrDefault(t => t.BatchId == _batchId && t.Status != "completed");

                if (existingInProgress != null)
                {
                    _currentTestId = existingInProgress.Id;
                    await LoadParameters();
                    lblStatus.Text = "Испытание уже в работе";
                    return;
                }

                var request = new CreateTestRequest
                {
                    BatchId = _batchId,
                    BatchType = _batchType,
                    Comment = ""
                };

                var testId = await _api.CreateTestAndGetIdAsync(request);

                if (testId != null)
                {
                    _currentTestId = testId.Value;
                    await LoadParameters();
                    lblStatus.Text = "Новое испытание";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadParameters()
        {
            try
            {
                string sampleType = _batchType == "raw_material" ? "raw_material" : "finished_product";
                var standards = await _api.GetStandardParametersAsync(sampleType);

                if (standards == null || standards.Count == 0) return;

                _parameters = new List<TestResult>();
                foreach (var std in standards)
                {
                    _parameters.Add(new TestResult
                    {
                        ParameterName = std.ParameterName,
                        MinValue = std.MinValue,
                        MaxValue = std.MaxValue,
                        Unit = std.Unit,
                        IsValid = false,
                        MeasuredValue = null
                    });
                }

                // Загружаем сохранённые результаты, если есть
                if (_currentTestId.HasValue)
                {
                    var savedResults = await _api.GetTestResultsAsync(_currentTestId.Value);
                    if (savedResults != null)
                    {
                        foreach (var saved in savedResults)
                        {
                            var param = _parameters.FirstOrDefault(p => p.ParameterName == saved.ParameterName);
                            if (param != null)
                            {
                                param.MeasuredValue = saved.MeasuredValue;
                                param.IsValid = saved.IsValid;
                            }
                        }
                    }
                }

                gridParameters.ItemsSource = null;
                gridParameters.ItemsSource = _parameters;

                // Активировать кнопку протокола, если есть хотя бы одно заполненное значение
                btnProtocol.IsEnabled = _parameters.Any(p => p.MeasuredValue.HasValue);

                // Активировать кнопку завершения, если все поля заполнены
                btnComplete.IsEnabled = _parameters.All(p => p.MeasuredValue.HasValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки параметров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddParameter()
        {
            if (_currentTestId == null)
            {
                MessageBox.Show("Сначала создайте испытание", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new AddParameterDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.NewParameter != null)
            {
                dialog.NewParameter.TestId = _currentTestId.Value;

                var request = new AddTestResultRequest
                {
                    ParameterName = dialog.NewParameter.ParameterName,
                    Value = dialog.NewParameter.MeasuredValue ?? 0,
                    Comment = dialog.NewParameter.AnalystComment ?? ""
                };

                var result = await _api.AddTestResultAsync(_currentTestId.Value, request);
                if (result != null)
                {
                    await LoadParameters();
                }
            }
        }

        private void TxtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var parameter = textBox?.Tag as TestResult;
            if (parameter == null) return;

            string text = textBox.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                parameter.MeasuredValue = null;
                parameter.IsValid = false;
            }
            else
            {
                text = text.Replace('.', ',');
                if (decimal.TryParse(text, out decimal value))
                {
                    parameter.MeasuredValue = value;
                    parameter.IsValid = value >= parameter.MinValue && value <= parameter.MaxValue;
                    textBox.Text = value.ToString();
                }
                else
                {
                    parameter.MeasuredValue = null;
                    parameter.IsValid = false;
                    MessageBox.Show("Введите корректное число (например: 123,45 или 123.45)", "Ошибка ввода");
                    textBox.Text = "";
                }
            }

            gridParameters.ItemsSource = null;
            gridParameters.ItemsSource = _parameters;

            var allFilled = _parameters.All(p => p.MeasuredValue.HasValue);
            btnComplete.IsEnabled = allFilled;

            // Активировать кнопку протокола, если есть хотя бы одно заполненное значение
            btnProtocol.IsEnabled = _parameters.Any(p => p.MeasuredValue.HasValue);
        }

        private async Task SaveProgress()
        {
            if (_currentTestId == null) return;

            foreach (var param in _parameters)
            {
                if (param.MeasuredValue.HasValue)
                {
                    var request = new AddTestResultRequest
                    {
                        ParameterName = param.ParameterName,
                        Value = param.MeasuredValue.Value,
                        Comment = param.AnalystComment ?? ""
                    };
                    await _api.AddTestResultAsync(_currentTestId.Value, request);
                }
            }

            MessageBox.Show("Промежуточные результаты сохранены", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task CompleteTest()
        {
            var hasEmptyRequired = _parameters.Any(p => !p.MeasuredValue.HasValue);
            if (hasEmptyRequired)
            {
                MessageBox.Show("Заполните все параметры перед завершением испытания", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var allValid = _parameters.All(p => p.IsValid);
            var decisionDialog = new DecisionDialog(allValid);
            decisionDialog.Owner = this;

            if (decisionDialog.ShowDialog() == true)
            {
                _currentDecision = decisionDialog.Decision;

                var request = new CompleteTestRequest
                {
                    Decision = decisionDialog.Decision,
                    Comment = decisionDialog.Comment
                };

                await _api.CompleteTestAsync(_currentTestId!.Value, request);

                var decisionText = decisionDialog.Decision == "approved" ? "РАЗРЕШЕНО" : "ЗАБЛОКИРОВАНО";
                MessageBox.Show($"Испытание завершено. Решение: {decisionText}", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                btnProtocol.IsEnabled = true;
                btnComplete.IsEnabled = false;
                btnSaveProgress.IsEnabled = false;

                DialogResult = true;
                Close();
            }
        }

        private void ShowProtocol()
        {
            if (_parameters == null || _parameters.Count == 0)
            {
                MessageBox.Show("Нет данных для формирования протокола");
                return;
            }

            var protocol = new ProtocolDialog(_batchNumber, _materialName,
                _batchType == "raw_material" ? "сырье" : "готовая продукция",
                _parameters, _currentDecision ?? "pending", txtComment.Text);
            protocol.Owner = this;
            protocol.ShowDialog();
        }
    }
}