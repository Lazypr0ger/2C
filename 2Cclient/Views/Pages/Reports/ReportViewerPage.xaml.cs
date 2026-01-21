using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _2Cclient.Views.Pages.Reports
{
    public partial class ReportViewerPage : Page
    {
        private readonly string _reportId;
        private readonly string _typeCode;
        private readonly string _name;
        private readonly DateTime _from;
        private readonly DateTime _to;
        private readonly DateTime _buildDate;
        private readonly string _comment;

        // ✅ Универсальные VM для демо-таблиц (пока нет сервера)
        private sealed class Row1
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public decimal Qty { get; set; }
            public decimal PlanSum { get; set; }
            public decimal Deviation { get; set; }
            public decimal FactSum { get; set; }
        }

        private sealed class Row2
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public decimal SoldSum { get; set; }
            public decimal CostSum { get; set; }
            public decimal Profit { get; set; }
        }

        private sealed class Row3
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public decimal Qty { get; set; }
            public decimal PlanSaleCost { get; set; }
            public decimal Deviation { get; set; }
            public decimal FactCost { get; set; }
        }

        public ReportViewerPage(string reportId, string typeCode, string name, DateTime from, DateTime to, DateTime buildDate, string comment)
        {
            InitializeComponent();
            _reportId = reportId;
            _typeCode = typeCode;
            _name = name;
            _from = from;
            _to = to;
            _buildDate = buildDate;
            _comment = comment ?? "";

            Loaded += async (_, __) =>
            {
                TitleText.Text = _name;
                PeriodText.Text = $"за период с {_from:dd.MM.yyyy} по {_to:dd.MM.yyyy}";
                ExtraLineText.Text = $"Дата формирования: {_buildDate:dd.MM.yyyy}" + (string.IsNullOrWhiteSpace(_comment) ? "" : $" • {_comment}");
                ExtraLineText.Visibility = Visibility.Visible;

                await LoadAndRenderAsync();
            };
        }

        private System.Threading.Tasks.Task LoadAndRenderAsync()
        {
            // TODO: позже заменим на API:
            // var api = App.Services.GetRequiredService<ReportApi>();
            // var data = await api.GetReportAsync(_reportId);
            // Render(data);

            // Пока DEMO-рендер по типу:
            RenderDemoByType(_typeCode);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private void RenderDemoByType(string typeCode)
        {
            // Сброс totals
            Total1Text.Text = "";
            Total2Text.Text = "";
            Total3Text.Text = "";

            // 3 отчёта из ТЗ:
            // 1) распределение фактических затрат (AllocateActualCost report)
            // 2) ведомость продаж
            // 3) отклонения фактической себестоимости по реализованной продукции

            switch (typeCode)
            {
                case "R1":
                case "FACT_COST_DISTRIBUTION":
                    RenderReport1();
                    break;

                case "R2":
                case "SALES_STATEMENT":
                    RenderReport2();
                    break;

                case "R3":
                case "DEVIATION_SALES_COST":
                    RenderReport3();
                    break;

                default:
                    // fallback: покажем 1-й
                    RenderReport1();
                    break;
            }
        }

        private void RenderReport1()
        {
            // DEMO
            var rows = new List<Row1>
            {
                new() { Code="P001", Name="ДЕТАЛЬ А", Qty=10, PlanSum=12000, FactSum=12800 },
                new() { Code="P002", Name="ДЕТАЛЬ Б", Qty=7,  PlanSum=9000,  FactSum=8280 },
            };

            foreach (var r in rows)
                r.Deviation = r.FactSum - r.PlanSum;

            var list = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)Resources["RowStyle"],
                Margin = new Thickness(0, 6, 0, 0)
            };

            var gv = new GridView();
            gv.Columns.Add(MakeCol("Код продукции", 120, "Code"));
            gv.Columns.Add(MakeCol("Название продукции", 220, "Name"));
            gv.Columns.Add(MakeColRight("Количество выпущенной продукции", 170, "Qty", "{0:N0}"));
            gv.Columns.Add(MakeColRight("Плановая себестоимость продукции", 200, "PlanSum", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Отклонение от плановой себестоимости", 220, "Deviation", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Фактическая себестоимость", 190, "FactSum", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = rows;
            TableHost.Content = list;

            var totalPlan = rows.Sum(x => x.PlanSum);
            var totalDev = rows.Sum(x => x.Deviation);
            var totalFact = rows.Sum(x => x.FactSum);

            Total1Text.Text = $"План: {totalPlan:N2}";
            Total2Text.Text = $"Откл.: {totalDev:N2}";
            Total3Text.Text = $"Факт: {totalFact:N2}";
        }

        private void RenderReport2()
        {
            var rows = new List<Row2>
            {
                new() { Code="P001", Name="ДЕТАЛЬ А", SoldSum=45000, CostSum=32800 },
                new() { Code="P002", Name="ДЕТАЛЬ Б", SoldSum=30000, CostSum=21000 },
            };

            foreach (var r in rows)
                r.Profit = r.SoldSum - r.CostSum;

            var list = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)Resources["RowStyle"],
                Margin = new Thickness(0, 6, 0, 0)
            };

            var gv = new GridView();
            gv.Columns.Add(MakeCol("Код продукции", 140, "Code"));
            gv.Columns.Add(MakeCol("Название продукции", 260, "Name"));
            gv.Columns.Add(MakeColRight("Продано на сумму", 180, "SoldSum", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Себестоимость продаж", 220, "CostSum", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Прибыль/убыток", 200, "Profit", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = rows;
            TableHost.Content = list;

            Total1Text.Text = $"Продажи: {rows.Sum(x => x.SoldSum):N2}";
            Total2Text.Text = $"Себест.: {rows.Sum(x => x.CostSum):N2}";
            Total3Text.Text = $"П/У: {rows.Sum(x => x.Profit):N2}";
        }

        private void RenderReport3()
        {
            var rows = new List<Row3>
            {
                new() { Code="P001", Name="ДЕТАЛЬ А", Qty=6, PlanSaleCost=19000, FactCost=21000 },
                new() { Code="P002", Name="ДЕТАЛЬ Б", Qty=4, PlanSaleCost=12000, FactCost=11000 },
            };

            foreach (var r in rows)
                r.Deviation = r.FactCost - r.PlanSaleCost;

            var list = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)Resources["RowStyle"],
                Margin = new Thickness(0, 6, 0, 0)
            };

            var gv = new GridView();
            gv.Columns.Add(MakeCol("Код продукции", 140, "Code"));
            gv.Columns.Add(MakeCol("Название продукции", 260, "Name"));
            gv.Columns.Add(MakeColRight("Количество реализованной продукции", 220, "Qty", "{0:N0}"));
            gv.Columns.Add(MakeColRight("Плановая себестоимость продажи", 240, "PlanSaleCost", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Отклонение от плановой себестоимости", 240, "Deviation", "{0:N2}"));
            gv.Columns.Add(MakeColRight("Фактическая себестоимость", 220, "FactCost", "{0:N2}"));

            list.View = gv;
            list.ItemsSource = rows;
            TableHost.Content = list;

            Total1Text.Text = $"План: {rows.Sum(x => x.PlanSaleCost):N2}";
            Total2Text.Text = $"Откл.: {rows.Sum(x => x.Deviation):N2}";
            Total3Text.Text = $"Факт: {rows.Sum(x => x.FactCost):N2}";
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

        private async void Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadAndRenderAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления:\n{ex.Message}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
