using System.Windows;
using System.Windows.Controls;

namespace _2Cclient.Views.Pages
{
    public partial class DirectoriesPage : Page
    {
        public DirectoriesPage() => InitializeComponent();

        private void Organizations_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new OrganizationsPage());

        private void Departaments_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new DepartamentsPage());

        private void Products_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new ProductsPage());
    }
}
