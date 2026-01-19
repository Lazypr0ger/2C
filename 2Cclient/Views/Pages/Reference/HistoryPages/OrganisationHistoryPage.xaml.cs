using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using _2Cclient.Services.Api.HistoryApi;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class OrganisationHistoryPage : Page
    {
        private readonly OrganisationHistoryApi _api;
        private readonly OrganisationVM _organisation;
        private List<OrganisationHistoryVM> _items = new();

        public OrganisationHistoryPage(OrganisationVM organisation)
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<OrganisationHistoryApi>();
            _organisation = organisation;

            Loaded += async (_, __) => await LoadFromServerAsync();
        }

        private async Task LoadFromServerAsync()
        {
            try
            {
                TitleText.Text = $"История организации: {_organisation.Name}";
                SubtitleText.Text = "";

                var data = await _api.GetHistoryAsync(_organisation.Id);
                _items = data ?? new List<OrganisationHistoryVM>();

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
            if (HistoryList.SelectedItem is not OrganisationHistoryVM selected)
            {
                MakeActualBtn.IsEnabled = false;
                return;
            }

            MakeActualBtn.IsEnabled = selected.ValidTo != null;
        }

        private async void MakeActual_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryList.SelectedItem is not OrganisationHistoryVM selected) return;

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
