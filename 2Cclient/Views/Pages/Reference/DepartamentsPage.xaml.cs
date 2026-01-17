using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages
{
    public partial class DepartamentsPage : Page
    {
        private List<DepartamentVM> _items = new();

        public DepartamentsPage()
        {
            InitializeComponent();
            LoadStub();
        }

        private void LoadStub()
        {
            _items = new List<DepartamentVM>
            {
                new() { Id="d1", Name="Производство", IsDeleted=false },
                new() { Id="d2", Name="Склад", IsDeleted=false },
                new() { Id="d3", Name="Продажи", IsDeleted=false },
                new() { Id="d4", Name="ИТ-отдел", IsDeleted=true },
            };

            DepartamentsList.ItemsSource = _items;
            DepartamentsList.SelectedItem = null;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (DepartamentsList.SelectedItem is DepartamentVM selected)
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
                DepartamentsList.SelectedItem = null;
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
            => NavigationService?.Navigate(new DepartamentEditPage());
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (DepartamentsList.SelectedItem is not DepartamentVM selected) return;
            NavigationService?.Navigate(new DepartamentEditPage(selected));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DepartamentsList.SelectedItem is not DepartamentVM selected) return;

            selected.IsDeleted = true;

            DepartamentsList.ItemsSource = null;
            DepartamentsList.ItemsSource = _items;

            DepartamentsList.SelectedItem = null;
            UpdateButtons();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (DepartamentsList.SelectedItem is not DepartamentVM selected) return;

            selected.IsDeleted = false;

            DepartamentsList.ItemsSource = null;
            DepartamentsList.ItemsSource = _items;

            DepartamentsList.SelectedItem = null;
            UpdateButtons();
        }
    }
}
