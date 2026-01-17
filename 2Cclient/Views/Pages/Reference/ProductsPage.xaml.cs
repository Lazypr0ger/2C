using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contracts.Enums;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages
{
    public partial class ProductsPage : Page
    {
        private List<ProductionVM> _items = new();

        public ProductsPage()
        {
            InitializeComponent();
            LoadStub();
        }

        private void LoadStub()
        {
            // Если enum TypeProduct у тебя с другими именами — поменяй значения.
            _items = new List<ProductionVM>
            {
                new() { Id="p1", Code="PR-001", Type=(TypeProduct)0, Name="Сталь листовая 2мм", PlannedCost=1200.50m, DepartamentId="d1", IsDeleted=false },
                new() { Id="p2", Code="PR-002", Type=(TypeProduct)1, Name="Заготовка корпуса", PlannedCost=5400.00m, DepartamentId="d1", IsDeleted=false },
                new() { Id="p3", Code="PR-003", Type=(TypeProduct)2, Name="Изделие А (готовая продукция)", PlannedCost=15999.99m, DepartamentId="d2", IsDeleted=false },
                new() { Id="p4", Code="PR-004", Type=(TypeProduct)2, Name="Изделие B (архив)", PlannedCost=8900.00m, DepartamentId="d2", IsDeleted=true },
            };

            ProductsList.ItemsSource = _items;
            ProductsList.SelectedItem = null;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (ProductsList.SelectedItem is ProductionVM selected)
            {
                UpdateBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = !selected.IsDeleted;
                RestoreBtn.IsEnabled = selected.IsDeleted;
            }
            else
            {
                UpdateBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
                RestoreBtn.IsEnabled = false;
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateButtons();

        private void List_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;
            if (FindAncestor<ListViewItem>(depObj) == null)
            {
                ProductsList.SelectedItem = null;
                UpdateButtons();
            }
        }

        private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T typed) return typed;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
             => NavigationService?.Navigate(new ProductEditPage());

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not ProductionVM selected) return;
            NavigationService?.Navigate(new ProductEditPage(selected));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not ProductionVM selected) return;

            selected.IsDeleted = true;

            ProductsList.ItemsSource = null;
            ProductsList.ItemsSource = _items;

            ProductsList.SelectedItem = null;
            UpdateButtons();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not ProductionVM selected) return;

            selected.IsDeleted = false;

            ProductsList.ItemsSource = null;
            ProductsList.ItemsSource = _items;

            ProductsList.SelectedItem = null;
            UpdateButtons();
        }
    }
}
