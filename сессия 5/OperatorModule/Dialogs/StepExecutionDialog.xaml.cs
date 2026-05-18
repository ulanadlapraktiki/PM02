using OperatorModule.Models;
using OperatorModule.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OperatorModule.Dialogs
{
    public partial class StepExecutionDialog : Window
    {
        private readonly StepItem _step;
        private readonly Dictionary<string, TextBox> _paramBoxes = new();
        private decimal? _actualTempC;
        private decimal? _actualPressureBar;
        private int? _actualDurationMin;

        public decimal? ActualTempC => _actualTempC;
        public decimal? ActualPressureBar => _actualPressureBar;
        public int? ActualDurationMin => _actualDurationMin;
        public string Comment { get; private set; } = string.Empty;

        public StepExecutionDialog(StepItem step)
        {
            InitializeComponent();
            _step = step;

            lblStepName.Text = $"Шаг {step.StepOrder}: {step.StepName}";
            lblInstruction.Text = step.Instruction;

            // Добавляем параметры
            if (step.PlannedTempC.HasValue)
            {
                AddParameter("Температура", step.PlannedTempC.Value, "°C", 5);
            }

            if (step.PlannedPressureBar.HasValue)
            {
                AddParameter("Давление", step.PlannedPressureBar.Value, "бар", 0.5m);
            }

            if (step.PlannedDurationMin.HasValue)
            {
                AddParameter("Длительность", step.PlannedDurationMin.Value, "мин", 0);
            }

            btnComplete.Click += (s, e) => Complete();
            btnCancel.Click += (s, e) => Close();
        }

        private void AddParameter(string name, decimal planned, string unit, decimal tolerance)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 10),
                BorderBrush = new SolidColorBrush(Color.FromRgb(222, 226, 230)),
                BorderThickness = new Thickness(1)
            };

            var panel = new StackPanel();

            var titlePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            titlePanel.Children.Add(new TextBlock { Text = "🔧", FontSize = 14, Margin = new Thickness(0, 0, 8, 0) });
            titlePanel.Children.Add(new TextBlock { Text = $"{name}", FontWeight = FontWeights.SemiBold, FontSize = 14 });
            panel.Children.Add(titlePanel);

            panel.Children.Add(new TextBlock { Text = $"План: {planned} {unit}", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(0, 0, 0, 5) });

            var box = new TextBox
            {
                Height = 38,
                Padding = new Thickness(10, 5, 0, 0),
                FontSize = 14,
                BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                BorderThickness = new Thickness(1)
            };
            panel.Children.Add(box);

            decimal min = planned - tolerance;
            decimal max = planned + tolerance;
            panel.Children.Add(new TextBlock
            {
                Text = $"Допуск: {min} - {max} {unit}",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                Margin = new Thickness(0, 5, 0, 0)
            });

            border.Child = panel;
            panelParams.Children.Add(border);
            _paramBoxes[name] = box;
        }

        private void Complete()
        {
            // Сбор значений
            if (_paramBoxes.ContainsKey("Температура"))
            {
                if (decimal.TryParse(_paramBoxes["Температура"].Text, out var val))
                    _actualTempC = val;
                else
                { ShowError("Введите температуру"); return; }
            }

            if (_paramBoxes.ContainsKey("Давление"))
            {
                if (decimal.TryParse(_paramBoxes["Давление"].Text, out var val))
                    _actualPressureBar = val;
                else
                { ShowError("Введите давление"); return; }
            }

            if (_paramBoxes.ContainsKey("Длительность"))
            {
                if (int.TryParse(_paramBoxes["Длительность"].Text, out var val))
                    _actualDurationMin = val;
                else
                { ShowError("Введите длительность"); return; }
            }

            Comment = txtComment.Text;

            DialogResult = true;
            Close();
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Двойной клик по заголовку - закрыть окно (опционально)
                Close();
            }
            else
            {
                DragMove();
            }
        }

        // Обработчик для кнопки закрытия
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}