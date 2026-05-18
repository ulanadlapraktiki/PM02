using LaboratoryModule.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;

namespace LaboratoryModule.Dialogs
{
    public partial class ProtocolDialog : Window
    {
        private readonly string _batchNumber;
        private readonly string _materialName;
        private readonly string _sampleType;
        private readonly List<TestResult> _results;
        private readonly string _decision;
        private readonly string _comment;

        public ProtocolDialog(string batchNumber, string materialName, string sampleType,
                              List<TestResult> results, string decision, string comment)
        {
            InitializeComponent();

            _batchNumber = batchNumber;
            _materialName = materialName;
            _sampleType = sampleType;
            _results = results;
            _decision = decision;
            _comment = comment;

            lblProtocolNumber.Text = $"П-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}";
            lblDate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            lblBatchNumber.Text = batchNumber;
            lblMaterial.Text = materialName;
            lblControlType.Text = sampleType == "сырье" ? "Входной контроль" : "Приемочный контроль";
            lblAnalyst.Text = "Лаборант Петрова";
            lblDecision.Text = decision == "approved" ? "✅ РАЗРЕШЕНО" : "❌ ЗАБЛОКИРОВАНО";
            lblDecision.Foreground = decision == "approved" ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            lblComment.Text = comment;

            gridResults.ItemsSource = _results;

            btnPrint.Click += (s, e) => PrintProtocol();
            btnSavePdf.Click += (s, e) => SaveAsPdf();
            btnClose.Click += (s, e) => Close();
        }

        private void PrintProtocol()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var fixedDoc = new FixedDocument();
                    var visual = gridResults.Parent as FrameworkElement;
                    var pageContent = new PageContent();
                    var fixedPage = new FixedPage();
                    fixedPage.Children.Add(visual);
                    ((IAddChild)pageContent).AddChild(fixedPage);
                    fixedDoc.Pages.Add(pageContent);
                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Протокол испытаний");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка печати: {ex.Message}");
            }
        }

        private void SaveAsPdf()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "HTML files (*.html)|*.html",
                    FileName = $"Протокол_{_batchNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.html"
                };

                if (dialog.ShowDialog() == true)
                {
                    // Создаём HTML содержимое
                    string html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Протокол испытаний {_batchNumber}</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 40px; }}
                    h1 {{ text-align: center; color: #2c3e50; }}
                    .info {{ margin-bottom: 20px; }}
                    .info p {{ margin: 5px 0; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                    th {{ background-color: #f2f2f2; }}
                    .ok {{ color: green; font-weight: bold; }}
                    .fail {{ color: red; font-weight: bold; }}
                    .signatures {{ margin-top: 40px; }}
                    .signatures p {{ margin-top: 20px; }}
                </style>
            </head>
            <body>
                <h1>ПРОТОКОЛ ИСПЫТАНИЙ</h1>
                
                <div class='info'>
                    <p><strong>Номер партии:</strong> {_batchNumber}</p>
                    <p><strong>Материал/Продукт:</strong> {_materialName}</p>
                    <p><strong>Тип контроля:</strong> {(_sampleType == "сырье" ? "Входной контроль" : "Приемочный контроль")}</p>
                    <p><strong>Дата:</strong> {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                    <p><strong>Лаборант:</strong> {lblAnalyst.Text}</p>
                    <p><strong>Решение:</strong> {lblDecision.Text}</p>
                </div>
                
                <table>
                    <tr>
                        <th>Параметр</th>
                        <th>Норма</th>
                        <th>Ед.изм</th>
                        <th>Значение</th>
                        <th>Статус</th>
                    </tr>";

                    foreach (var result in _results)
                    {
                        string statusClass = result.IsValid ? "ok" : "fail";
                        string statusText = result.IsValid ? "OK" : "Отклонение";
                        html += $@"
                    <tr>
                        <td>{result.ParameterName}</td>
                        <td>{result.MinValue} - {result.MaxValue}</td>
                        <td>{result.Unit}</td>
                        <td>{result.MeasuredValue}</td>
                        <td class='{statusClass}'>{statusText}</td>
                    </tr>";
                    }

                    html += $@"
                </table>
                
                <div class='info'>
                    <p><strong>Комментарий:</strong> {_comment}</p>
                </div>
                
                <div class='signatures'>
                    <p>Лаборант: ___________________</p>
                    <p>Начальник лаборатории: ___________________</p>
                </div>
            </body>
            </html>";

                    // Сохраняем HTML файл
                    File.WriteAllText(dialog.FileName, html, Encoding.UTF8);

                    // Открываем в браузере
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dialog.FileName) { UseShellExecute = true });

                    MessageBox.Show($"Протокол сохранен: {dialog.FileName}\nОткрывается в браузере для печати", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}