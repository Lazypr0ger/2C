using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Contracts.Enums;
using Contracts.ViewModels.Reports;

namespace _2Cclient.Views.Pages.Reports
{
    public partial class ReportViewerPage : Page
    {
        private readonly ReportResultVM _report;
        private readonly string _comment;

        public ReportViewerPage(ReportResultVM report, string comment)
        {
            InitializeComponent();
            _report = report ?? throw new ArgumentNullException(nameof(report));
            _comment = comment ?? "";

            Loaded += (_, __) =>
            {
                TitleText.Text = _report.Name;
                PeriodText.Text = $"за период с {_report.From:dd.MM.yyyy} по {_report.To:dd.MM.yyyy}";
                ExtraLineText.Text =
                    $"Дата формирования: {_report.BuildDate:dd.MM.yyyy}" +
                    (string.IsNullOrWhiteSpace(_comment) ? "" : $" • {_comment}");
                ExtraLineText.Visibility = Visibility.Visible;

                Render(_report);
            };
        }

        private void Render(ReportResultVM report)
        {
            Total1Text.Text = "";
            Total2Text.Text = "";
            Total3Text.Text = "";

            switch (report.TypeCode)
            {
                case ReportTypeCodes.ActualCostDistribution:
                    RenderReport1(report);
                    break;

                case ReportTypeCodes.SalesStatement:
                    RenderReport2(report);
                    break;

                case ReportTypeCodes.RealisedDeviationStatement:
                    RenderReport3(report);
                    break;

                default:
                    RenderReport1(report);
                    break;
            }
        }

        // 1) распределение фактических затрат
        private void RenderReport1(ReportResultVM report)
        {
            var rows = report.ActualCostDistributionRows ?? new();

            var list = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)Resources["RowStyle"],
                Margin = new Thickness(0, 6, 0, 0)
            };

            var gv = new GridView();
            gv.Columns.Add(MakeCol("Код продукции", 120, "ProductionCode"));
            gv.Columns.Add(MakeCol("Название продукции", 220, "ProductionName"));
            gv.Columns.Add(MakeColRight("Количество выпущенной продукции", 170, "Quantity", "{0:N0}"));
            gv.Columns.Add(MakeColRight("Плановая себестоимость продукции", 200, "PlanCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Отклонение от плановой себестоимости", 220, "Deviation", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Фактическая себестоимость", 190, "ActualCost", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = rows;
            TableHost.Content = list;

            if (report.TotalActualCosts.HasValue)
            {
                Total1Text.Text = $"Общая сумма фактических затрат: {report.TotalActualCosts.Value:N2}";
            }
            else
            {
                Total1Text.Text = $"Кол-во: {report.TotalQty:N0}";
            }

            Total2Text.Text = $"План: {report.Total1:N2} • Откл.: {report.Total2:N2}";
            Total3Text.Text = $"Факт: {report.Total3:N2}";
        }

        // 2) ведомость продаж
        private void RenderReport2(ReportResultVM report)
        {
            var rows = report.SalesStatementRows ?? new();

            var list = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)Resources["RowStyle"],
                Margin = new Thickness(0, 6, 0, 0)
            };

            var gv = new GridView();
            gv.Columns.Add(MakeCol("Код продукции", 140, "ProductionCode"));
            gv.Columns.Add(MakeCol("Название продукции", 260, "ProductionName"));
            gv.Columns.Add(MakeColRight("Продано на сумму", 180, "SoldAmount", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Себестоимость продаж", 220, "SalesCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Прибыль/убыток", 200, "ProfitOrLoss", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = rows;
            TableHost.Content = list;

            Total1Text.Text = $"Продажи: {report.Total1:N2}";
            Total2Text.Text = $"Себест.: {report.Total2:N2}";
            Total3Text.Text = $"П/У: {report.Total3:N2}";
        }

        // 3) отклонения по реализованной продукции
        private void RenderReport3(ReportResultVM report)
        {
            var rows = report.RealisedDeviationRows ?? new();

            var list = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)Resources["RowStyle"],
                Margin = new Thickness(0, 6, 0, 0)
            };

            var gv = new GridView();
            gv.Columns.Add(MakeCol("Код продукции", 140, "ProductionCode"));
            gv.Columns.Add(MakeCol("Название продукции", 260, "ProductionName"));
            gv.Columns.Add(MakeColRight("Количество реализованной продукции", 220, "Quantity", "{0:N0}"));
            gv.Columns.Add(MakeColRight("Плановая себестоимость продажи", 240, "PlanSalesCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Отклонение от плановой себестоимости", 240, "Deviation", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Фактическая себестоимость", 220, "ActualSalesCost", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = rows;
            TableHost.Content = list;

            Total1Text.Text = $"План: {report.Total1:N2}";
            Total2Text.Text = $"Откл.: {report.Total2:N2}";
            Total3Text.Text = $"Факт: {report.Total3:N2}";
        }

        private static GridViewColumn MakeCol(string header, double width, string path)
        {
            return new GridViewColumn
            {
                Header = header,
                Width = width,
                CellTemplate = MakeTemplate(path, null, horizontalRight: false)
            };
        }

        private static GridViewColumn MakeColRight(string header, double width, string path, string fmt)
        {
            return new GridViewColumn
            {
                Header = header,
                Width = width,
                CellTemplate = MakeTemplate(path, fmt, horizontalRight: true)
            };
        }

        private static DataTemplate MakeTemplate(string path, string? stringFormat, bool horizontalRight)
        {
            var f = new FrameworkElementFactory(typeof(TextBlock));
            f.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            f.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            f.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);

            if (horizontalRight)
            {
                f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
                f.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            }

            var b = new System.Windows.Data.Binding(path);
            if (!string.IsNullOrWhiteSpace(stringFormat))
                b.StringFormat = stringFormat;

            f.SetBinding(TextBlock.TextProperty, b);

            return new DataTemplate { VisualTree = f };
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            // Сейчас отчёт уже сформирован и отдан сервером.
            // Если хочешь реальную перезагрузку с API — добавим GetById и storage отчётов.
            Render(_report);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
