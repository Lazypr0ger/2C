using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using _2Cclient.Services.Api.HistoryApi;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages.Reference.HistoryPages
{
    public partial class ProductionHistoryPage : Page
    {
        private readonly ProductionHistoryApi _api;
        private readonly ProductionVM _production;
        private List<ProductionHistoryVM> _items = new();

        public ProductionHistoryPage(ProductionVM production)
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ProductionHistoryApi>();
            _production = production;

            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async Task LoadFromServerAsync()
        {
            try
            {
                TitleText.Text = $"История продукции: {_production.Name}";
                SubtitleText.Text = "";

                var data = await _api.GetHistoryAsync(_production.Id);
                _items = data ?? new List<ProductionHistoryVM>();

                var ordered = _items
                    .OrderByDescending(x => x.ValidTo == null)
                    .ThenByDescending(x => x.ValidFrom)
                    .ToList();

                HistoryList.ItemsSource = ordered;
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
            if (HistoryList.SelectedItem is not ProductionHistoryVM selected)
            {
                MakeActualBtn.IsEnabled = false;
                return;
            }

            MakeActualBtn.IsEnabled = selected.ValidTo != null;
        }

        private async void MakeActual_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryList.SelectedItem is not ProductionHistoryVM selected) return;

            try
            {
                MakeActualBtn.IsEnabled = false;

                await _api.RestoreFromHistoryAsync(selected.Id);

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
