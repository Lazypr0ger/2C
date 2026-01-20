// TransactionLogPage.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using _2Cclient.Views.Pages.OperationsPages;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class TransactionLogPage : Page
    {
        private readonly TransactionLogApi _api;

        private List<TransactionLogVM> _all = new();
        private List<TransactionLogVM> _filtered = new();
        private bool _sortDateDesc = true;

        public TransactionLogPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<TransactionLogApi>();

            Loaded += async (_, __) =>
            {
                SetDefaultPeriod();
                await LoadFromServerAsync();
            };
        }

        private void SetDefaultPeriod()
        {
            var now = DateTime.Now;
            FromDate.SelectedDate = new DateTime(now.Year, now.Month, 1);
            ToDate.SelectedDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        }

        // DatePicker дает Kind=Unspecified → конвертим в UTC перед запросом
        private static DateTime? ToUtcStart(DateTime? d)
        {
            if (!d.HasValue) return null;
            var local = DateTime.SpecifyKind(d.Value.Date, DateTimeKind.Local);
            return local.ToUniversalTime();
        }

        private static DateTime? ToUtcEndInclusive(DateTime? d)
        {
            if (!d.HasValue) return null;
            var localEnd = DateTime.SpecifyKind(d.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Local);
            return localEnd.ToUniversalTime();
        }

        private async Task LoadFromServerAsync()
        {
            try
            {
                var fromUtc = ToUtcStart(FromDate.SelectedDate);
                var toUtc = ToUtcEndInclusive(ToDate.SelectedDate);

                var data = await _api.GetViewAsync(fromUtc, toUtc);
                _all = data ?? new List<TransactionLogVM>();

                ApplyLocalFilter();
                LogsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки журнала проводок:\n{ex.Message}");
            }
        }
        private void DateHeader_Click(object sender, RoutedEventArgs e)
        {
            _sortDateDesc = !_sortDateDesc;
            ApplyLocalFilter();
        }

        private void ApplyLocalFilter()
        {
            var q = (SearchBox.Text ?? string.Empty).Trim();
            IEnumerable<TransactionLogVM> data = _all;

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.ToLowerInvariant();
                data = data.Where(x =>
                       (x.Comment ?? "").ToLowerInvariant().Contains(q)
                    || (x.ChartDebNum ?? "").ToLowerInvariant().Contains(q)
                    || (x.ChartCredNum ?? "").ToLowerInvariant().Contains(q)
                    || (x.Subconto1DebName ?? "").ToLowerInvariant().Contains(q)
                    || (x.Subconto1CredName ?? "").ToLowerInvariant().Contains(q)
                    || (x.OperationId ?? "").ToLowerInvariant().Contains(q)
                );
            }

            // сортировка по дате (стабильно + вторичный ключ, чтобы не "прыгало")
            data = _sortDateDesc
                ? data.OrderByDescending(x => x.DateOperation).ThenByDescending(x => x.Id)
                : data.OrderBy(x => x.DateOperation).ThenBy(x => x.Id);

            _filtered = data.ToList();
            LogsList.ItemsSource = _filtered;

            UpdateDateHeaderUi();
        }
        private void UpdateDateHeaderUi()
        {
            if (LogsList?.View is not GridView gv) return;

            // Первая колонка у тебя "Дата" - если не первая, поменяй индекс
            var col = gv.Columns.FirstOrDefault();
            if (col?.Header is GridViewColumnHeader header)
            {
                // header.Content у нас StackPanel -> [TextBlock "Дата", TextBlock arrow]
                if (header.Content is StackPanel sp && sp.Children.Count >= 2 && sp.Children[1] is TextBlock arrow)
                {
                    arrow.Text = _sortDateDesc ? " ▼" : " ▲";
                }
            }
        }


        private void UpdateButtons()
        {
            if (LogsList.SelectedItem is TransactionLogVM s)
                OpenOpBtn.IsEnabled = !string.IsNullOrWhiteSpace(s.OperationId);
            else
                OpenOpBtn.IsEnabled = false;
        }

        private void LogsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateButtons();

        private void LogsList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;
            if (FindAncestor<ListViewItem>(depObj) == null)
            {
                LogsList.SelectedItem = null;
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

        private async void Reload_Click(object sender, RoutedEventArgs e)
            => await LoadFromServerAsync();

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            SetDefaultPeriod();
            await LoadFromServerAsync();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyLocalFilter();

        private void ManualPosting_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ManualPostingPage());
        }


        private async void OpenOperation_Click(object sender, RoutedEventArgs e)
        {
            if (LogsList.SelectedItem is not TransactionLogVM s) return;
            if (string.IsNullOrWhiteSpace(s.OperationId)) return;

            try
            {
                OpenOpBtn.IsEnabled = false;

                var opApi = App.Services.GetRequiredService<OperationApi>();
                var op = await opApi.GetByIdAsync(s.OperationId);

                // Роутер по типу
                switch (op.Type)
                {
                    case Contracts.Enums.OperationType.ActualCosts:
                        NavigationService?.Navigate(new ManualPostingPage(op));
                        break;

                    case Contracts.Enums.OperationType.ReceiptFromProduction:
                        NavigationService?.Navigate(new IncomeOperationEditPage(op));
                        break;

                    case Contracts.Enums.OperationType.Sale:
                        NavigationService?.Navigate(new SaleOperationEditPage(op));
                        break;

                    case Contracts.Enums.OperationType.AllocateActualCost:
                        NavigationService?.Navigate(new CostDistributionOperationEditPage(op));
                        break;

                    case Contracts.Enums.OperationType.WriteOffDeviations:
                        NavigationService?.Navigate(new WriteOffDeviationOperationEditPage(op));
                        break;

                    default:
                        MessageBox.Show($"Неизвестный тип операции: {op.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия документа:\n{ex.Message}");
            }
            finally
            {
                OpenOpBtn.IsEnabled = true;
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
