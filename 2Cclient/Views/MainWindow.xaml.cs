using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using _2Cclient.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApiOptions _apiOptions;
        private readonly ApiClient _apiClient;

        public MainWindow()
        {
            InitializeComponent();

            _apiOptions = App.Services.GetRequiredService<ApiOptions>();
            _apiClient = App.Services.GetRequiredService<ApiClient>();

            Loaded += MainWindow_Loaded;
            NavList.SelectedIndex = 0;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateConnectionBadgeAsync();
        }

        private async Task UpdateConnectionBadgeAsync()
        {
            var baseUrl = (_apiOptions.BaseUrl ?? "").TrimEnd('/');

            SetConnecting(baseUrl);

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _apiClient.GetAsync<object>("/ms/api/Departament", cts.Token);

                SetConnected(baseUrl);
            }
            catch (OperationCanceledException)
            {
                SetDisconnected(baseUrl, "Timeout");
            }
            catch (HttpRequestException ex)
            {
                SetDisconnected(baseUrl, ex.Message);
            }
            catch (Exception ex)
            {
                SetDisconnected(baseUrl, ex.Message);
            }
        }

        private void SetConnecting(string baseUrl)
        {
            ConnectionStateText.Text = "ПОДКЛЮЧЕНИЕ…";
            ConnectionStateText.Foreground = (Brush)FindResource("Warning");

            ConnectionAddressText.Text = baseUrl;
            ConnectionAddressText.Foreground = (Brush)FindResource("Warning");
        }

        private void SetConnected(string baseUrl)
        {
            ConnectionStateText.Text = "ПОДКЛЮЧЕНО";
            ConnectionStateText.Foreground = (Brush)FindResource("Success");

            ConnectionAddressText.Text = baseUrl;
            ConnectionAddressText.Foreground = (Brush)FindResource("Text2");
        }

        private void SetDisconnected(string baseUrl, string reason)
        {
            ConnectionStateText.Text = "ОТКЛЮЧЕН";
            ConnectionStateText.Foreground = (Brush)FindResource("Danger");

            ConnectionAddressText.Text = $"{baseUrl} ({reason})";
            ConnectionAddressText.Foreground = (Brush)FindResource("Text2");
        }

        private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavList.SelectedItem is not ListBoxItem item)
                return;

            var tag = item.Tag?.ToString();

            switch (tag)
            {
                case "Directories":
                    PageTitleText.Text = "Справочники";
                    MainFrame.Navigate(new DirectoriesPage());
                    break;

                case "Operations":
                    PageTitleText.Text = "Операции";
                    MainFrame.Navigate(new OperationsPage());
                    break;

                case "Reports":
                    PageTitleText.Text = "Отчёты";
                    MainFrame.Navigate(new ReportsPage());
                    break;

                case "Ledger":
                    PageTitleText.Text = "Журнал проводок";
                    MainFrame.Navigate(new TransactionLogPage());
                    break;
                
                case "Charts":
                    PageTitleText.Text = "План Счетов";
                    MainFrame.Navigate(new ChartsOfAccountPage());
                    break;
            }
        }
    }
}
