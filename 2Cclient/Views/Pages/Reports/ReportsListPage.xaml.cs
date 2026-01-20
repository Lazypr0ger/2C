using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private async Task LoadAsync()
        {
            try
            {
                // TODO: заменить на реальный API:
                // var api = App.Services.GetRequiredService<ReportApi>();
                // _all = (await api.GetListAsync(_typeCode)).ToList();

                await Task.Delay(50);

                // DEMO
                var now = DateTime.Now;
                _all = new()
                {
                    new()
                    {
                        Id="r1",
                        TypeCode=_typeCode,
                        Name=_typeTitle,
                        From=DateTime.Today.AddDays(-30),
                        To=DateTime.Today,
                        BuildDate=DateTime.Today,
                        CreatedAt=now.AddHours(-2),
                        Status="Готов",
                        Comment="Авто"
                    },
                    new()
                    {
                        Id="r2",
                        TypeCode=_typeCode,
                        Name=_typeTitle,
                        From=DateTime.Today.AddDays(-7),
                        To=DateTime.Today,
                        BuildDate=DateTime.Today,
                        CreatedAt=now.AddDays(-1),
                        Status="Готов",
                        Comment="По запросу"
                    }
                };

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
                .OrderByDescending(x => x.CreatedAt)
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
            NavigationService?.Navigate(new ReportViewerPage(r.Id, r.TypeCode, r.Name));
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsList.SelectedItem is not ReportListItemVM r) return;

            if (MessageBox.Show($"Удалить отчёт?\n\n{r.Name}\n{r.PeriodText}",
                    "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                // TODO: api.DeleteAsync(r.Id);
                await Task.Delay(50);

                _all.RemoveAll(x => x.Id == r.Id);
                ApplyFilter();
                ReportsList.SelectedItem = null;
                UpdateButtons();
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
