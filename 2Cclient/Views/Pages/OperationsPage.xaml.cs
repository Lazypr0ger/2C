using System.Windows;
using System.Windows.Controls;
using _2Cclient.Views.Pages.OperationsPages;

namespace _2Cclient.Views.Pages
{
    public partial class OperationsPage : Page
    {
        public OperationsPage()
        {
            InitializeComponent();
        }

        private void Income_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new IncomeOperationsPage());

        private void Sale_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new SaleOperationsPage());

        private void CostDistribution_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new CostDistributionOperationsPage());

        private void DeviationWriteOff_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new WriteOffDeviationOperationsPage());

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
