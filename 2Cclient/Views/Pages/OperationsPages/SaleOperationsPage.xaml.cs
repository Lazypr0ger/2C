using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class SaleOperationsPage : Page
    {
        private List<SaleOperationListItemVM> _items = new();

        public SaleOperationsPage()
        {
            InitializeComponent();
            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async System.Threading.Tasks.Task LoadFromServerAsync()
        {
            try
            {
                // Заглушка: позже будет API (/Operation?type=SalesDocument)
                await System.Threading.Tasks.Task.Delay(1);

                _items = new List<SaleOperationListItemVM>
                {
                    new()
                    {
                        Id = "1",
                        NameDocument = "Реализация №1 (очень длинное название документа для проверки обрезки)",
                        OrganisationName = "Организация 1 (очень длинное название для проверки)",
                        DateOperation = DateTime.Today,
                        TotalAmount = 12500.50m,
                        IsDeleted = false
                    },
                    new()
                    {
                        Id = "2",
                        NameDocument = "Реализация №2",
                        OrganisationName = "Организация 2",
                        DateOperation = DateTime.Today.AddDays(-3),
                        TotalAmount = 800m,
                        IsDeleted = true
                    }
                };

                OperationsList.ItemsSource = _items;
                OperationsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки операций реализации:\n{ex.Message}");
            }
        }

        private void UpdateButtons()
        {
            if (OperationsList.SelectedItem is SaleOperationListItemVM selected)
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
            NavigationService?.Navigate(new SaleOperationEditPage());
        }

        private void Repost_Click(object sender, RoutedEventArgs e)
        {
            if (OperationsList.SelectedItem is not SaleOperationListItemVM selected) return;

            // Если появится конструктор перепроведения — подставим его сюда
            // NavigationService?.Navigate(new SaleOperationEditPage(selected));
            NavigationService?.Navigate(new SaleOperationEditPage());
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OperationsList.SelectedItem is not SaleOperationListItemVM selected) return;

            try
            {
                DeleteBtn.IsEnabled = false;
                RepostBtn.IsEnabled = false;

                // Заглушка: позже DELETE /Operation/{id}
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

        private class SaleOperationListItemVM
        {
            public string Id { get; set; } = ""; // не показываем в UI
            public string NameDocument { get; set; } = "";
            public string OrganisationName { get; set; } = "";
            public DateTime DateOperation { get; set; }
            public decimal TotalAmount { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
