using System.Windows;
using System.Windows.Controls;
using _2Cclient.Views.Pages.Reports;

namespace _2Cclient.Views.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        private void Type_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            var typeCode = b.Tag?.ToString() ?? "";
            var title = GetTitle(typeCode);

            NavigationService?.Navigate(new ReportsListPage(typeCode, title));
        }

        private static string GetTitle(string typeCode) => typeCode switch
        {
            "actual_cost_distribution" => "Ведомость распределения фактических затрат по видам выпущенной продукции",
            "sales_statement" => "Ведомость продаж продукции",
            "realised_deviation" => "Ведомость расчета отклонений фактической себестоимости от плановой по реализованной продукции",
            _ => "Отчёт"
        };

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
