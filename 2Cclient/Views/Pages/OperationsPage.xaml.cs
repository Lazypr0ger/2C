using System.Windows;
using System.Windows.Controls;
using Contracts.Enums;

namespace _2Cclient.Views.Pages
{
    public partial class OperationsPage : Page
    {
        public OperationsPage()
        {
            InitializeComponent();
        }

        private void Go(OperationType type)
            => NavigationService?.Navigate(new OperationListPage(type));

        private void ActualCosts_Click(object sender, RoutedEventArgs e) => Go(OperationType.ActualCosts);
        private void Receipt_Click(object sender, RoutedEventArgs e) => Go(OperationType.ReceiptFromProduction);
        private void Sale_Click(object sender, RoutedEventArgs e) => Go(OperationType.Sale);
        private void Allocate_Click(object sender, RoutedEventArgs e) => Go(OperationType.AllocateActualCost);
        private void WriteOff_Click(object sender, RoutedEventArgs e) => Go(OperationType.WriteOffDeviations);
    }
}
