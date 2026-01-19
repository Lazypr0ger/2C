using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class OrganizationsPage : Page
    {
        private List<OrganisationVM> _items = new();
        private readonly OrganisationApi _api;

        public OrganizationsPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<OrganisationApi>();
            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async Task LoadFromServerAsync()
        {
            try
            {
                var data = await _api.GetAllAsync();
                _items = data ?? new List<OrganisationVM>();

                OrganizationsList.ItemsSource = _items;
                OrganizationsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки организаций через Ocelot:\n{ex.Message}");
            }
        }

        private void UpdateButtons()
        {
            if (OrganizationsList.SelectedItem is OrganisationVM selected)
            {
                UpdateBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = !selected.IsDeleted;
                RestoreBtn.IsEnabled = selected.IsDeleted;
                HistoryBtn.IsEnabled = true;
            }
            else
            {
                UpdateBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
                RestoreBtn.IsEnabled = false;
                HistoryBtn.IsEnabled = false;
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;
            NavigationService?.Navigate(new OrganisationHistoryPage(selected));
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateButtons();

        // Клик по пустому месту снимает выделение
        private void List_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;

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
            => NavigationService?.Navigate(new OrganisationEditPage());

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;
            NavigationService?.Navigate(new OrganisationEditPage(selected));
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;
            if (selected.IsDeleted) return;

            try
            {
                SetBusy(true);

                await _api.SoftDeleteAsync(selected.Id);
                await LoadFromServerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления:\n{ex.Message}");
                UpdateButtons();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (OrganizationsList.SelectedItem is not OrganisationVM selected) return;
            if (!selected.IsDeleted) return;

            try
            {
                SetBusy(true);

                await _api.RestoreAsync(selected.Id);
                await LoadFromServerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка восстановления:\n{ex.Message}");
                UpdateButtons();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void SetBusy(bool isBusy)
        {
            OrganizationsList.IsHitTestVisible = !isBusy;

            AddBtn.IsEnabled = !isBusy;

            if (isBusy)
            {
                UpdateBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
                RestoreBtn.IsEnabled = false;
            }
            else
            {
                UpdateButtons();
            }
        }
    }
}
