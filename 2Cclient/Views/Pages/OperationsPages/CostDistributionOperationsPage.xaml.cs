using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class CostDistributionOperationsPage : Page
    {
        private List<CostDistributionListItemVM> _items = new();

        public CostDistributionOperationsPage()
        {
            InitializeComponent();
            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async System.Threading.Tasks.Task LoadFromServerAsync()
        {
            try
            {
                // Заглушка: позже API (/Operation?type=CostDistributionDocument)
                await System.Threading.Tasks.Task.Delay(1);

                _items = new List<CostDistributionListItemVM>
                {
                    new()
                    {
                        Id = "1",
                        NameDocument = "Распределение за 01.2026",
                        DateOperation = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 25),
                        IsDeleted = false
                    }
                };

                OperationsList.ItemsSource = _items;
                OperationsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки операций распределения:\n{ex.Message}");
            }
        }

        private void UpdateButtons()
        {
            if (OperationsList.SelectedItem is CostDistributionListItemVM selected)
            {
                DeleteBtn.IsEnabled = !selected.IsDeleted;
                RepostBtn.IsEnabled = !selected.IsDeleted;
            }
            else
            {
                DeleteBtn.IsEnabled = false;
                RepostBtn.IsEnabled = false;
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateButtons();

        private void List_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;
            if (FindAncestor<ListViewItem>(depObj) == null)
            {
                OperationsList.SelectedItem = null;
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

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CostDistributionOperationEditPage());
        }

        private void Repost_Click(object sender, RoutedEventArgs e)
        {
            if (OperationsList.SelectedItem is not CostDistributionListItemVM selected) return;
            NavigationService?.Navigate(new CostDistributionOperationEditPage());
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OperationsList.SelectedItem is not CostDistributionListItemVM selected) return;

            try
            {
                DeleteBtn.IsEnabled = false;
                RepostBtn.IsEnabled = false;

                await System.Threading.Tasks.Task.Delay(1);

                selected.IsDeleted = true;

                OperationsList.ItemsSource = null;
                OperationsList.ItemsSource = _items;
                OperationsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления:\n{ex.Message}");
                UpdateButtons();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private class CostDistributionListItemVM
        {
            public string Id { get; set; } = "";
            public string NameDocument { get; set; } = "";
            public DateTime DateOperation { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
