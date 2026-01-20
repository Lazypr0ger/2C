using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _2Cclient.Views.Pages.Reports
{
    public partial class ReportViewerPage : Page
    {
        private readonly string _reportId;
        private readonly string _typeCode;
        private readonly string _reportName;

        // Универсальный документ отчёта (пример контракта фронта).
        // На сервере можно вернуть ровно это.
        public sealed class ReportDocumentVM
        {
            public string Id { get; set; } = "";
            public string TypeCode { get; set; } = "";
            public string Name { get; set; } = "";
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public DateTime BuildDate { get; set; }

            // Для отчёта №1:
            public decimal? TotalActualCosts { get; set; }

            public List<Row1> Rows1 { get; set; } = new();
            public List<Row2> Rows2 { get; set; } = new();
            public List<Row3> Rows3 { get; set; } = new();
        }

        // 1) Распределение фактических затрат по выпущенной продукции
        public sealed class Row1
        {
            public string ProductCode { get; set; } = "";
            public string ProductName { get; set; } = "";
            public decimal QtyReleased { get; set; }
            public decimal PlannedCost { get; set; }
            public decimal DeviationFromPlan { get; set; }
            public decimal ActualCost { get; set; }
        }

        // 2) Продажи продукции
        public sealed class Row2
        {
            public string ProductCode { get; set; } = "";
            public string ProductName { get; set; } = "";
            public decimal SoldAmount { get; set; }          // Продано на сумму
            public decimal SalesCost { get; set; }           // Себестоимость продаж
            public decimal ProfitLoss { get; set; }          // Прибыль/убыток
        }

        // 3) Отклонения фактической от плановой по реализованной продукции
        public sealed class Row3
        {
            public string ProductCode { get; set; } = "";
            public string ProductName { get; set; } = "";
            public decimal QtySold { get; set; }
            public decimal PlannedSalesCost { get; set; }
            public decimal DeviationFromPlan { get; set; }
            public decimal ActualCost { get; set; }
        }

        private ReportDocumentVM? _doc;

        public ReportViewerPage(string reportId, string typeCode, string reportName)
        {
            InitializeComponent();
            _reportId = reportId;
            _typeCode = typeCode;
            _reportName = reportName;

            Loaded += async (_, __) =>
            {
                TitleText.Text = _reportName;
                await LoadAsync();
            };
        }

        private async Task LoadAsync()
        {
            try
            {
                // TODO: Реальный API:
                // var api = App.Services.GetRequiredService<ReportApi>();
                // _doc = await api.GetDocumentAsync(_reportId);

                await Task.Delay(120);
                _doc = BuildDemoDocument();

                RenderHeader(_doc);
                RenderTable(_doc);
                RenderTotals(_doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчёта:\n{ex.Message}");
            }
        }

        private void RenderHeader(ReportDocumentVM doc)
        {
            var from = doc.From.HasValue ? doc.From.Value.ToString("dd.MM.yyyy") : "—";
            var to = doc.To.HasValue ? doc.To.Value.ToString("dd.MM.yyyy") : "—";

            // По твоим формулировкам:
            if (doc.TypeCode == ReportTypeCodes.SalesStatement)
                PeriodText.Text = $"Ведомость продаж продукции за период {from} — {to}";
            else
                PeriodText.Text = $"За период с {from} по {to}";

            // Доп строка для отчёта №1:
            if (doc.TypeCode == ReportTypeCodes.ActualCostDistribution)
            {
                ExtraLineText.Visibility = Visibility.Visible;
                ExtraLineText.Text = $"Общая сумма фактических затрат: {(doc.TotalActualCosts ?? 0m):N2}";
            }
            else
            {
                ExtraLineText.Visibility = Visibility.Collapsed;
                ExtraLineText.Text = "";
            }
        }

        private void RenderTable(ReportDocumentVM doc)
        {
            // Рендерим разные GridView по типу
            switch (doc.TypeCode)
            {
                case ReportTypeCodes.ActualCostDistribution:
                    TableHost.Content = BuildTable1(doc.Rows1);
                    break;

                case ReportTypeCodes.SalesStatement:
                    TableHost.Content = BuildTable2(doc.Rows2);
                    break;

                case ReportTypeCodes.RealisedDeviationStatement:
                    TableHost.Content = BuildTable3(doc.Rows3);
                    break;

                default:
                    TableHost.Content = new TextBlock
                    {
                        Text = $"Неизвестный тип отчёта: {doc.TypeCode}",
                        Foreground = (System.Windows.Media.Brush)FindResource("Text")
                    };
                    break;
            }
        }

        private void RenderTotals(ReportDocumentVM doc)
        {
            // Внизу 3 поля итога (под “Итого ____ ____ ____”)
            // Чтобы соответствовать ведомостям, покажем 3 самых “суммируемых” столбца.

            if (doc.TypeCode == ReportTypeCodes.ActualCostDistribution)
            {
                var sumPlan = doc.Rows1.Sum(x => x.PlannedCost);
                var sumDev = doc.Rows1.Sum(x => x.DeviationFromPlan);
                var sumAct = doc.Rows1.Sum(x => x.ActualCost);

                Total1Text.Text = $"План: {sumPlan:N2}";
                Total2Text.Text = $"Откл.: {sumDev:N2}";
                Total3Text.Text = $"Факт: {sumAct:N2}";
                return;
            }

            if (doc.TypeCode == ReportTypeCodes.SalesStatement)
            {
                var sumSold = doc.Rows2.Sum(x => x.SoldAmount);
                var sumCost = doc.Rows2.Sum(x => x.SalesCost);
                var sumPL = doc.Rows2.Sum(x => x.ProfitLoss);

                Total1Text.Text = $"Продано: {sumSold:N2}";
                Total2Text.Text = $"Себест.: {sumCost:N2}";
                Total3Text.Text = $"П/У: {sumPL:N2}";
                return;
            }

            if (doc.TypeCode == ReportTypeCodes.RealisedDeviationStatement)
            {
                var sumPlan = doc.Rows3.Sum(x => x.PlannedSalesCost);
                var sumDev = doc.Rows3.Sum(x => x.DeviationFromPlan);
                var sumAct = doc.Rows3.Sum(x => x.ActualCost);

                Total1Text.Text = $"План: {sumPlan:N2}";
                Total2Text.Text = $"Откл.: {sumDev:N2}";
                Total3Text.Text = $"Факт: {sumAct:N2}";
                return;
            }

            Total1Text.Text = "";
            Total2Text.Text = "";
            Total3Text.Text = "";
        }

        // ----- TABLE BUILDERS (WPF) -----

        private FrameworkElement BuildTable1(List<Row1> rows)
        {
            var lv = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)FindResource("RowStyle"),
                ItemsSource = rows
            };

            var gv = new GridView();

            gv.Columns.Add(MakeCol("Код продукции", 120, "ProductCode", false));
            gv.Columns.Add(MakeCol("Название продукции", 320, "ProductName", false, wrap: true));
            gv.Columns.Add(MakeCol("Количество выпущенной продукции", 180, "QtyReleased", true));
            gv.Columns.Add(MakeCol("Плановая себестоимость продукции", 190, "PlannedCost", true, money: true));
            gv.Columns.Add(MakeCol("Отклонение от плановой себестоимости", 220, "DeviationFromPlan", true, money: true));
            gv.Columns.Add(MakeCol("Фактическая себестоимость", 180, "ActualCost", true, money: true));

            lv.View = gv;
            return lv;
        }

        private FrameworkElement BuildTable2(List<Row2> rows)
        {
            var lv = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)FindResource("RowStyle"),
                ItemsSource = rows
            };

            var gv = new GridView();

            gv.Columns.Add(MakeCol("Код продукции", 120, "ProductCode", false));
            gv.Columns.Add(MakeCol("Название продукции", 360, "ProductName", false, wrap: true));
            gv.Columns.Add(MakeCol("Продано на сумму", 170, "SoldAmount", true, money: true));
            gv.Columns.Add(MakeCol("Себестоимость продаж", 190, "SalesCost", true, money: true));
            gv.Columns.Add(MakeCol("Прибыль/убыток", 170, "ProfitLoss", true, money: true));

            lv.View = gv;
            return lv;
        }

        private FrameworkElement BuildTable3(List<Row3> rows)
        {
            var lv = new ListView
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                ItemContainerStyle = (Style)FindResource("RowStyle"),
                ItemsSource = rows
            };

            var gv = new GridView();

            gv.Columns.Add(MakeCol("Код продукции", 120, "ProductCode", false));
            gv.Columns.Add(MakeCol("Название продукции", 320, "ProductName", false, wrap: true));
            gv.Columns.Add(MakeCol("Количество реализованной продукции", 200, "QtySold", true));
            gv.Columns.Add(MakeCol("Плановая себестоимость продажи", 210, "PlannedSalesCost", true, money: true));
            gv.Columns.Add(MakeCol("Отклонение от плановой себестоимости", 230, "DeviationFromPlan", true, money: true));
            gv.Columns.Add(MakeCol("Фактическая себестоимость", 180, "ActualCost", true, money: true));

            lv.View = gv;
            return lv;
        }

        private GridViewColumn MakeCol(string header, double width, string path, bool right, bool money = false, bool wrap = false)
        {
            var col = new GridViewColumn { Header = header, Width = width };

            var fef = new FrameworkElementFactory(typeof(TextBlock));
            fef.SetValue(TextBlock.StyleProperty, (Style)FindResource(wrap ? "CellWrap" : "CellWrap"));
            fef.SetValue(TextBlock.TextWrappingProperty, wrap ? TextWrapping.Wrap : TextWrapping.NoWrap);
            fef.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            if (right) fef.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);

            var binding = new System.Windows.Data.Binding(path);
            if (money) binding.StringFormat = "{0:N2}";
            fef.SetBinding(TextBlock.TextProperty, binding);

            col.CellTemplate = new DataTemplate { VisualTree = fef };
            return col;
        }

        // ----- DEMO DATA -----
        private ReportDocumentVM BuildDemoDocument()
        {
            var now = DateTime.Now.Date;

            if (_typeCode == ReportTypeCodes.ActualCostDistribution)
            {
                var rows = new List<Row1>
                {
                    new(){ ProductCode="P001", ProductName="Продукт 1", QtyReleased=100, PlannedCost=120000, DeviationFromPlan=5000, ActualCost=125000 },
                    new(){ ProductCode="P002", ProductName="Продукт 2", QtyReleased=40,  PlannedCost=60000,  DeviationFromPlan=-2000, ActualCost=58000  },
                };

                return new ReportDocumentVM
                {
                    Id = _reportId,
                    TypeCode = _typeCode,
                    Name = _reportName,
                    From = now.AddDays(-30),
                    To = now,
                    BuildDate = now,
                    TotalActualCosts = rows.Sum(x => x.ActualCost),
                    Rows1 = rows
                };
            }

            if (_typeCode == ReportTypeCodes.SalesStatement)
            {
                var rows = new List<Row2>
                {
                    new(){ ProductCode="P001", ProductName="Продукт 1", SoldAmount=200000, SalesCost=140000, ProfitLoss=60000 },
                    new(){ ProductCode="P002", ProductName="Продукт 2", SoldAmount=90000,  SalesCost=95000,  ProfitLoss=-5000 },
                };

                return new ReportDocumentVM
                {
                    Id = _reportId,
                    TypeCode = _typeCode,
                    Name = _reportName,
                    From = now.AddDays(-30),
                    To = now,
                    BuildDate = now,
                    Rows2 = rows
                };
            }

            // realised deviation
            var rows3 = new List<Row3>
            {
                new(){ ProductCode="P001", ProductName="Продукт 1", QtySold=70, PlannedSalesCost=84000, DeviationFromPlan=4000, ActualCost=88000 },
                new(){ ProductCode="P002", ProductName="Продукт 2", QtySold=25, PlannedSalesCost=37500, DeviationFromPlan=-1500, ActualCost=36000 },
            };

            return new ReportDocumentVM
            {
                Id = _reportId,
                TypeCode = _typeCode,
                Name = _reportName,
                From = now.AddDays(-30),
                To = now,
                BuildDate = now,
                Rows3 = rows3
            };
        }

        private async void Reload_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
