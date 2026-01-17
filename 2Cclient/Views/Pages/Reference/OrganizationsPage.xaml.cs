using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages
{
    public partial class OrganizationsPage : Page
    {
        private List<OrganisationVM> _items = new();

        public OrganizationsPage()
        {
            InitializeComponent();
            LoadStub();
        }

        private void LoadStub()
        {
            _items = new List<OrganisationVM>
            {
                new() { Id="1", Name="ООО Ромашка", AccountNumOrg="40702810...", IsDeleted=false },
                new() { Id="2", Name="АО Север", AccountNumOrg="40702811...", IsDeleted=false },
                new() { Id="3", Name="ИП Иванов", AccountNumOrg="40802810...", IsDeleted=true },
            };

            OrganizationsList.ItemsSource = _items;
            OrganizationsList.SelectedItem = null;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (OrganizationsList.SelectedItem is OrganisationVM selected)
            {
                UpdateBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = !selected.IsDeleted;   // удалять только активные
                RestoreBtn.IsEnabled = selected.IsDeleted;   // восстанавливать только удаленные
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

        // Клик по пустому месту снимает выделение
        private void List_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;

            // если клик НЕ по элементу строки
            if (FindAncestor<ListViewItem>(depObj) == null)
            {
                OrganizationsList.SelectedItem = null;
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
        {
            NavigationService?.Navigate(new OrganisationEditPage());
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;
            NavigationService?.Navigate(new OrganisationEditPage(selected));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;
            if (selected.IsDeleted) return;
            selected.IsDeleted = true;

            // обновим UI (пока VM без INPC)
            OrganizationsList.ItemsSource = null;
            OrganizationsList.ItemsSource = _items;

            OrganizationsList.SelectedItem = null;
            UpdateButtons();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;

            selected.IsDeleted = false;

            OrganizationsList.ItemsSource = null;
            OrganizationsList.ItemsSource = _items;

            OrganizationsList.SelectedItem = null;
            UpdateButtons();
        }
    }
}
