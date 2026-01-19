using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api.HistoryApi;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages.Reference.HistoryPages
{
    public partial class DepartamentHistoryPage : Page
    {
        private readonly DepartamentHistoryApi _api;
        private readonly DepartamentVM _departament;
        private List<DepartamentHistoryVM> _items = new();

        public DepartamentHistoryPage(DepartamentVM departament)
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<DepartamentHistoryApi>();
            _departament = departament;

            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async Task LoadFromServerAsync()
        {
            try
            {
                TitleText.Text = $"История подразделения: {_departament.Name}";
                SubtitleText.Text = "";

                var data = await _api.GetHistoryAsync(_departament.Id);
                _items = data ?? new List<DepartamentHistoryVM>();

                var notActual = _items
                    .Where(x => x.ValidTo != null)
                    .OrderByDescending(x => x.ValidFrom)
                    .ToList();

                HistoryList.ItemsSource = notActual;
                HistoryList.SelectedItem = null;
                MakeActualBtn.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории:\n{ex.Message}");
            }
        }

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MakeActualBtn.IsEnabled = HistoryList.SelectedItem is DepartamentHistoryVM;
        }

        private async void MakeActual_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryList.SelectedItem is not DepartamentHistoryVM selected) return;

            try
            {
                MakeActualBtn.IsEnabled = false;

                await _api.RestoreFromHistoryAsync(selected.Id);

                // после восстановления можно вернуться назад в список подразделений
                if (NavigationService?.CanGoBack == true)
                    NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка восстановления версии:\n{ex.Message}");
                MakeActualBtn.IsEnabled = true;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
