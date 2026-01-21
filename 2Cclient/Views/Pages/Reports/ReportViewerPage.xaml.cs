using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Contracts.DTO.Reports;
using Contracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;

namespace _2Cclient.Views.Pages.Reports
{
    public partial class ReportViewerPage : Page
    {
        private readonly string _reportId;

        private CancellationTokenSource? _cts;

 
        public ReportViewerPage(string reportId)
        {
            InitializeComponent();
            _reportId = reportId;

            Loaded += async (_, __) =>
            {
                await LoadAndRenderAsync();
            };
        }

        public ReportViewerPage(string reportId, string typeCode, string name, DateTime from, DateTime to, DateTime buildDate, string comment)
            : this(reportId)
        {

            Loaded += (_, __) =>
            {
                TitleText.Text = name;
                PeriodText.Text = $"за период с {from:dd.MM.yyyy} по {to:dd.MM.yyyy}";

                var extra = $"Дата формирования: {buildDate:dd.MM.yyyy}";
                if (!string.IsNullOrWhiteSpace(comment))
                    extra += $" • {comment.Trim()}";

                ExtraLineText.Text = extra;
                ExtraLineText.Visibility = Visibility.Visible;

                if (TryGetTableTitle(out var t))
                    t.Text = "Таблица отчёта";
            };
        }

        private async Task LoadAndRenderAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                SetLoadingState(true);

                var api = App.Services.GetRequiredService<ApiClient>();

   
                var report = await api.GetAsync<ReportResultDto>($"/ms/api/Report/id/{_reportId}", _cts.Token);

                Render(report);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчёта:\n{ex.Message}", "Отчёты", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void Render(ReportResultDto report)
        {

            TitleText.Text = report.Name;

            PeriodText.Text = $"за период с {report.From:dd.MM.yyyy} по {report.To:dd.MM.yyyy}";

            ExtraLineText.Text = $"Дата формирования: {report.BuildDate:dd.MM.yyyy HH:mm}";
            ExtraLineText.Visibility = Visibility.Visible;

            if (TryGetTableTitle(out var tableTitle))
                tableTitle.Text = GetTableTitle(report.TypeCode);

            // Таблица + итоги
            switch (report.TypeCode)
            {
                case ReportTypeCodes.ActualCostDistribution:
                    RenderActualCostDistribution(report);
                    break;

                case ReportTypeCodes.SalesStatement:
                    RenderSalesStatement(report);
                    break;

                case ReportTypeCodes.RealisedDeviationStatement:
                    RenderRealisedDeviation(report);
                    break;

                default:
                    TableHost.Content = MakeInfoBlock($"Неизвестный тип отчёта: {report.TypeCode}");
                    SetTotals("", "", "");
                    break;
            }
        }

        private void RenderActualCostDistribution(ReportResultDto r)
        {
            var list = CreateTable();

            var gv = new GridView();
            ApplyHeaderStyle(gv);

            gv.Columns.Add(MakeCol("Код продукции", 140, "ProductionCode"));
            gv.Columns.Add(MakeCol("Название продукции", 260, "ProductionName"));
            gv.Columns.Add(MakeColRight("Количество выпущенной продукции", 220, "Quantity", "{0:N0}"));
            gv.Columns.Add(MakeColRight("Плановая себестоимость продукции", 240, "PlanCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Отклонение от плановой себестоимости", 260, "Deviation", "{0:N4}"));
            gv.Columns.Add(MakeColRight("Фактическая себестоимость", 220, "ActualCost", "{0:N4}"));

            list.View = gv;
            list.ItemsSource = r.ActualCostDistributionRows ?? [];

            TableHost.Content = list;

     
            var totalQty = r.TotalQty;
            var totalPlan = r.Total1;
            var totalDev = r.Total2;
            var totalFact = r.Total3;

            var totalActualCosts = r.TotalActualCosts ?? totalFact;

            SetTotals(
                $"Кол-во: {FormatNumber(totalQty, "N0")}",
                $"План: {FormatNumber(totalPlan, "N2")} • Откл.: {FormatNumber(totalDev, "N4")}",
                $"Факт: {FormatNumber(totalFact, "N4")} • Общ. факт. затраты: {FormatNumber(totalActualCosts, "N2")}"
            );
        }

        private void RenderSalesStatement(ReportResultDto r)
        {
            var list = CreateTable();

            var gv = new GridView();
            ApplyHeaderStyle(gv);

            gv.Columns.Add(MakeCol("Код продукции", 140, "ProductionCode"));
            gv.Columns.Add(MakeCol("Название продукции", 280, "ProductionName"));
            gv.Columns.Add(MakeColRight("Продано на сумму", 200, "SoldAmount", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Себестоимость продаж", 220, "SalesCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Прибыль/убыток", 200, "ProfitOrLoss", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = r.SalesStatementRows ?? [];

            TableHost.Content = list;

            SetTotals(
                $"Продано: {FormatNumber(r.Total1, "N2")}",
                $"Себест.: {FormatNumber(r.Total2, "N2")}",
                $"П/У: {FormatNumber(r.Total3, "N2")}"
            );
        }

        // ---------------------------
        // Report 3: RealisedDeviationStatement
        // ---------------------------
        private void RenderRealisedDeviation(ReportResultDto r)
        {
            var list = CreateTable();

            var gv = new GridView();
            ApplyHeaderStyle(gv);

            gv.Columns.Add(MakeCol("Код продукции", 140, "ProductionCode"));
            gv.Columns.Add(MakeCol("Название продукции", 280, "ProductionName"));
            gv.Columns.Add(MakeColRight("Количество реализованной продукции", 240, "Quantity", "{0:N0}"));
            gv.Columns.Add(MakeColRight("Плановая себестоимость продажи", 260, "PlanSalesCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Отклонение от плановой себестоимости", 260, "Deviation", "{0:N4}"));
            gv.Columns.Add(MakeColRight("Фактическая себестоимость", 240, "ActualSalesCost", "{0:N4}"));

            list.View = gv;
            list.ItemsSource = r.RealisedDeviationRows ?? [];

            TableHost.Content = list;

            SetTotals(
                $"Кол-во: {FormatNumber(r.TotalQty, "N0")}",
                $"План: {FormatNumber(r.Total1, "N2")} • Откл.: {FormatNumber(r.Total2, "N4")}",
                $"Факт: {FormatNumber(r.Total3, "N4")}"
            );
        }

      

        private ListView CreateTable()
        {
            var list = new ListView
            {
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0)
            };

            if (Application.Current.Resources["ReportTableListStyle"] is Style tableStyle)
                list.Style = tableStyle;

            return list;
        }


        private void ApplyHeaderStyle(GridView gv)
        {

            if (Application.Current.Resources["ReportGridHeaderStyle"] is Style headerStyle)
                gv.ColumnHeaderContainerStyle = headerStyle;
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

            if (Application.Current.Resources["ReportCellTextStyle"] is Style cellStyle)
                f.SetValue(FrameworkElement.StyleProperty, cellStyle);
            else
            {
                f.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                f.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                f.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            }

            if (horizontalRight)
                f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);

            var b = new Binding(path);
            if (!string.IsNullOrWhiteSpace(stringFormat))
                b.StringFormat = stringFormat;

            f.SetBinding(TextBlock.TextProperty, b);

            return new DataTemplate { VisualTree = f };
        }

        private void SetTotals(string t1, string t2, string t3)
        {
            Total1Text.Text = t1 ?? "";
            Total2Text.Text = t2 ?? "";
            Total3Text.Text = t3 ?? "";
        }

        private void SetLoadingState(bool loading)
        {

        }

        private UIElement MakeInfoBlock(string text)
        {
            return new Border
            {
                Padding = new Thickness(12),
                Child = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap
                }
            };
        }

        private static string GetTableTitle(ReportTypeCodes type)
        {
            return type switch
            {
                ReportTypeCodes.ActualCostDistribution => "Ведомость распределения фактических затрат",
                ReportTypeCodes.SalesStatement => "Ведомость продаж продукции",
                ReportTypeCodes.RealisedDeviationStatement => "Ведомость отклонений фактической себестоимости (реализация)",
                _ => "Таблица отчёта"
            };
        }

        private static string FormatNumber(decimal value, string format)
            => value.ToString(format, CultureInfo.GetCultureInfo("ru-RU"));

        private bool TryGetTableTitle(out TextBlock tb)
        {
            tb = null!;
            try
            {
                tb = (TextBlock)FindName("TableTitleText");
                return tb != null;
            }
            catch { return false; }
        }

        // ---------------------------
        // Buttons
        // ---------------------------ф
        private async void Reload_Click(object sender, RoutedEventArgs e)
        {
            await LoadAndRenderAsync();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
