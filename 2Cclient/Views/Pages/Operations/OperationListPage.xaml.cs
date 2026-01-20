using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using _2Cclient.Views.Pages.Operations.OperationsPages;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class OperationListPage : Page
    {
        private readonly OperationApi _api;
        private readonly OperationType _type;

        private List<OperationVM> _all = new();
        private List<OperationVM> _filtered = new();

        public OperationListPage(OperationType type)
        {
            InitializeComponent();

            _type = type;
            _api = App.Services.GetRequiredService<OperationApi>();

            Loaded += async (_, __) =>
            {
                ApplyHeader();
                SetDefaultPeriod();
                await LoadAsync();
                UpdateButtons();
            };
        }

        private void ApplyHeader()
        {
            var (t, s) = GetHeader(_type);
            TitleText.Text = t;
            SubtitleText.Text = s;
        }

        private static (string title, string subtitle) GetHeader(OperationType type) => type switch
        {
            OperationType.ActualCosts => ("Операции: Фактические затраты", "Документы Дт20 Кт10"),
            OperationType.ReceiptFromProduction => ("Операции: Поступление на склад", "Документы Дт43 Кт20"),
            OperationType.Sale => ("Операции: Реализация продукции", "Документы реализации"),
            OperationType.AllocateActualCost => ("Операции: Распределение фактической себестоимости", "Операция месяца (Дт43 Кт20)"),
            OperationType.WriteOffDeviations => ("Операции: Списание отклонений", "Операция месяца (Дт90 Кт43)"),
            _ => ("Операции", "")
        };

        private void SetDefaultPeriod()
        {
            var now = DateTime.Now;
            FromDate.SelectedDate = new DateTime(now.Year, now.Month, 1);
            ToDate.SelectedDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        }

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

        private async Task LoadAsync()
        {
            try
            {
                var fromUtc = ToUtcStart(FromDate.SelectedDate);
                var toUtc = ToUtcEndInclusive(ToDate.SelectedDate);

                var data = await _api.GetAllAsync(fromUtc, toUtc);
                _all = (data ?? new List<OperationVM>()).Where(x => x.Type == _type).ToList();

                ApplyFilter();
                List.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки операций:\n{ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            var q = (SearchBox.Text ?? "").Trim().ToLowerInvariant();
            IEnumerable<OperationVM> data = _all;

            if (!string.IsNullOrWhiteSpace(q))
            {
                data = data.Where(x =>
                       (x.NameDocument ?? "").ToLowerInvariant().Contains(q)
                    || (x.Comment ?? "").ToLowerInvariant().Contains(q)
                    || (x.Id ?? "").ToLowerInvariant().Contains(q));
            }

            data = data
                .OrderByDescending(x => x.DateOperation)
                .ThenByDescending(x => x.Id);

            _filtered = data.ToList();
            List.ItemsSource = _filtered.Select(x => new RowVM(x)).ToList();
        }

        private void UpdateButtons()
        {
            if (List.SelectedItem is not RowVM row)
            {
                OpenBtn.IsEnabled = false;
                UpdateBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
                RestoreBtn.IsEnabled = false;
                return;
            }

            OpenBtn.IsEnabled = true;
            UpdateBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = !row.IsDeleted;
            RestoreBtn.IsEnabled = row.IsDeleted;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
            UpdateButtons();
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateButtons();

        private void List_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;
            if (FindAncestor<ListViewItem>(depObj) == null)
            {
                List.SelectedItem = null;
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
            NavigateToEdit(_type, op: null);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (List.SelectedItem is not RowVM row) return;
            NavigateToEdit(_type, row.Source);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (List.SelectedItem is not RowVM row) return;
            NavigateToEdit(_type, row.Source);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (List.SelectedItem is not RowVM row) return;

            try
            {
                await _api.DeleteAsync(row.Source.Id);
                await LoadAsync();
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления:\n{ex.Message}");
            }
        }

        private async void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (List.SelectedItem is not RowVM row) return;

            try
            {
                await _api.RecoveryAsync(row.Source.Id);
                await LoadAsync();
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка восстановления:\n{ex.Message}");
            }
        }

        private void NavigateToEdit(OperationType type, OperationVM? op)
        {
            // Create: op == null
            switch (type)
            {
                case OperationType.ActualCosts:
                    NavigationService?.Navigate(op == null ? new ManualPostingPage() : new ManualPostingPage(op));
                    break;

                case OperationType.ReceiptFromProduction:
                    NavigationService?.Navigate(op == null ? new IncomeOperationEditPage() : new IncomeOperationEditPage(op));
                    break;

                case OperationType.Sale:
                    NavigationService?.Navigate(op == null ? new SaleOperationEditPage() : new SaleOperationEditPage(op));
                    break;

                case OperationType.AllocateActualCost:
                    NavigationService?.Navigate(op == null ? new CostDistributionOperationEditPage() : new CostDistributionOperationEditPage(op));
                    break;

                case OperationType.WriteOffDeviations:
                    NavigationService?.Navigate(op == null ? new WriteOffDeviationOperationEditPage() : new WriteOffDeviationOperationEditPage(op));
                    break;

                default:
                    MessageBox.Show($"Неизвестный тип операции: {type}");
                    break;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true) NavigationService.GoBack();
        }

        private sealed class RowVM
        {
            public OperationVM Source { get; }
            public bool IsDeleted => Source.IsDeleted;

            public string NameDocument => Source.NameDocument;
            public string Comment => Source.Comment ?? "";
            public decimal TotalAmountDocument => Source.TotalAmountDocument;

            public string DateLocalText
            {
                get
                {
                    var dt = Source.DateOperation;
                    if (dt.Kind == DateTimeKind.Unspecified)
                        dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                    return dt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
                }
            }

            public RowVM(OperationVM src) => Source = src;
        }
    }
}
