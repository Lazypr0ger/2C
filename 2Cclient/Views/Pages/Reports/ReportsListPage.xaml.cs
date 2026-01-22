using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using Contracts.Enums;
using Contracts.ViewModels.Reports;

namespace _2Cclient.Views.Pages.Reports
{
    public partial class ReportsListPage : Page
    {
        private readonly string _typeCode;
        private readonly string _typeTitle;

        public sealed class ReportListItemVM
        {
            public string Id { get; set; } = "";
            public string TypeCode { get; set; } = "";
            public string Name { get; set; } = "";
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public DateTime BuildDate { get; set; }

            // у нас на сервере пока н
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; } = "Готов";
            public string? Comment { get; set; }

            public string PeriodText =>
                (From.HasValue && To.HasValue)
                    ? $"с {From:dd.MM.yyyy} по {To:dd.MM.yyyy}"
                    : "—";
        }

        private List<ReportListItemVM> _all = new();
        private List<ReportListItemVM> _filtered = new();

        public ReportsListPage(string typeCode, string typeTitle)
        {
            InitializeComponent();
            _typeCode = typeCode;
            _typeTitle = typeTitle;

            Loaded += async (_, __) =>
            {
                TitleText.Text = _typeTitle;
                SubtitleText.Text = "Список сформированных отчётов";
                await LoadAsync();
            };
        }

        // Маппим string из UI в enum сервера
        private static ReportTypeCodes MapTypeCode(string typeCode) => typeCode switch
        {
            "actual_cost_distribution" => ReportTypeCodes.ActualCostDistribution,
            "sales_statement" => ReportTypeCodes.SalesStatement,
            "realised_deviation" => ReportTypeCodes.RealisedDeviationStatement,
            _ => ReportTypeCodes.ActualCostDistribution
        };

        private async Task LoadAsync()
        {
            try
            {
                var api = (ReportApi)App.Services.GetService(typeof(ReportApi))!;
                var typeEnum = MapTypeCode(_typeCode);

                var list = await api.GetListAsync(typeEnum);


                _all = (list)
                    .Select(x => new ReportListItemVM
                    {
                        Id = x.Id,
                        TypeCode = x.TypeCode.ToString(),
                        Name = x.Name,
                        From = x.From,
                        To = x.To,
                        BuildDate = x.BuildDate,

                        // совместимость с колонками на UI:
                        CreatedAt = x.BuildDate,
                        Status = "Готов",
                        Comment = null
                    })
                    .ToList();

                ApplyFilter();
                ReportsList.SelectedItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка отчётов:\n{ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            var q = (SearchBox.Text ?? "").Trim().ToLowerInvariant();
            IEnumerable<ReportListItemVM> data = _all;

            if (!string.IsNullOrWhiteSpace(q))
            {
                data = data.Where(x =>
                       (x.Name ?? "").ToLowerInvariant().Contains(q)
                    || (x.Comment ?? "").ToLowerInvariant().Contains(q)
                    || (x.Status ?? "").ToLowerInvariant().Contains(q)
                    || (x.PeriodText ?? "").ToLowerInvariant().Contains(q));
            }

            _filtered = data
                .OrderByDescending(x => x.BuildDate)
                .ThenByDescending(x => x.Id)
                .ToList();

            ReportsList.ItemsSource = _filtered;
        }

        private void UpdateButtons()
        {
            var has = ReportsList.SelectedItem is ReportListItemVM;
            OpenBtn.IsEnabled = has;
            DeleteBtn.IsEnabled = has;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private async void Reload_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            await LoadAsync();
        }

        private void ReportsList_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateButtons();

        private void ReportsList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            if (FindAncestor<ListViewItem>(dep) == null)
            {
                ReportsList.SelectedItem = null;
                UpdateButtons();
            }
        }

        private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T t) return t;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new ReportCreatePage(_typeCode, _typeTitle));

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsList.SelectedItem is not ReportListItemVM r) return;

            NavigationService?.Navigate(new ReportViewerPage(r.Id));
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsList.SelectedItem is not ReportListItemVM r) return;

            if (MessageBox.Show($"Удалить отчёт?\n\n{r.Name}\n{r.PeriodText}",
                    "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                var api = (ReportApi)App.Services.GetService(typeof(ReportApi))!;
                await api.DeleteAsync(r.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления:\n{ex.Message}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
