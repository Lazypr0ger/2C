using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using _2Cclient.Views.Pages.Reference.HistoryPages;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class DepartamentsPage : Page
    {
        private List<DepartamentVM> _items = new();
        private readonly DepartamentApi _api;

        public DepartamentsPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<DepartamentApi>();
            Loaded += async (_, __) => await LoadFromServerAsync();
        }
        private async Task LoadFromServerAsync()
        {
            try
            {
                // Можно добавить индикатор “loading” позже
                var data = await _api.GetAllAsync();
                _items = data ?? new List<DepartamentVM>();

                DepartamentsList.ItemsSource = _items;
                DepartamentsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                // Пока просто сообщение (позже сделаем красивый баннер в UI)
                MessageBox.Show($"Ошибка загрузки подразделений через Ocelot:\n{ex.Message}");
            }
        }
        private void UpdateButtons()
        {
            if (DepartamentsList.SelectedItem is DepartamentVM selected)
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

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DepartamentsList.SelectedItem is not DepartamentVM selected) return;

            try
            {
                DeleteBtn.IsEnabled = false;
                RestoreBtn.IsEnabled = false;
                UpdateBtn.IsEnabled = false;
                HistoryBtn.IsEnabled = false;

                await _api.SoftDeleteAsync(selected.Id);
                await LoadFromServerAsync(); // обновляем из БД
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления:\n{ex.Message}");
                UpdateButtons();
            }
        }

        private async void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (DepartamentsList.SelectedItem is not DepartamentVM selected) return;

            try
            {
                DeleteBtn.IsEnabled = false;
                RestoreBtn.IsEnabled = false;
                UpdateBtn.IsEnabled = false;
                HistoryBtn.IsEnabled = false;

                await _api.RestoreAsync(selected.Id);
                await LoadFromServerAsync(); // обновляем из БД
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка восстановления:\n{ex.Message}");
                UpdateButtons();
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (DepartamentsList.SelectedItem is not DepartamentVM selected) return;
            NavigationService?.Navigate(new DepartamentHistoryPage(selected));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
