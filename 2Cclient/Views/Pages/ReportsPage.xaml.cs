using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Views.Pages.Reports;

namespace _2Cclient.Views.Pages
{
    public partial class ReportsPage : Page
    {
        public sealed class ReportTypeItem
        {
            public string Code { get; set; } = "";
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
        }

        public sealed class Vm
        {
            public List<ReportTypeItem> Types { get; } = new()
            {
                new()
                {
                    Code = ReportTypeCodes.ActualCostDistribution,
                    Title = "Ведомость распределения фактических затрат по видам выпущенной продукции",
                },
                new()
                {
                    Code = ReportTypeCodes.SalesStatement,
                    Title = "Ведомость продаж продукции",
                },
                new()
                {
                    Code = ReportTypeCodes.RealisedDeviationStatement,
                    Title = "Ведомость расчета отклонений фактической себестоимости от плановой по реализованной продукции",
                }
            };
        }

        public ReportsPage()
        {
            InitializeComponent();
            ReportsPageRoot.DataContext = new Vm();
        }

        private void Type_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            var code = b.Tag?.ToString();
            if (string.IsNullOrWhiteSpace(code)) return;

            var vm = (Vm)ReportsPageRoot.DataContext;
            var type = vm.Types.Find(x => x.Code == code);
            if (type == null) return;

            NavigationService?.Navigate(new ReportsListPage(type.Code, type.Title));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }

    public static class ReportTypeCodes
    {
        public const string ActualCostDistribution = "actual_cost_distribution"; // 1
        public const string SalesStatement = "sales_statement";                   // 2
        public const string RealisedDeviationStatement = "realised_deviation";    // 3
    }
}
