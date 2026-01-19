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
    public partial class ProductsPage : Page
    {
        private List<ProductionVM> _items = new();
        private readonly ProductionApi _api;

        public ProductsPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ProductionApi>();
            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async Task LoadFromServerAsync()
        {
            try
            {
                var data = await _api.GetAllAsync();
                _items = data ?? new List<ProductionVM>();

                ProductsList.ItemsSource = _items;
                ProductsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продуктов через Ocelot:\n{ex.Message}");
            }
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

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not ProductionVM selected) return;
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
            if (ProductsList.SelectedItem is not ProductionVM selected) return;
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
            // без белого "disabled" эффекта
            ProductsList.IsHitTestVisible = !isBusy;

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
