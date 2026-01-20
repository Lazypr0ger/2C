using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _2Cclient.Services.Api;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public abstract class BaseOperationListPage : Page
    {
        protected readonly OperationApi _api;

        protected List<OperationVM> _all = new();
        protected List<OperationVM> _filtered = new();

        protected abstract OperationType Type { get; }
        protected abstract string Title { get; }
        protected abstract string Subtitle { get; }

        protected BaseOperationListPage()
        {
            _api = App.Services.GetRequiredService<OperationApi>();
        }

        protected void SetHeader(TextBlock title, TextBlock subtitle)
        {
            title.Text = Title;
            subtitle.Text = Subtitle;
        }

        protected void SetDefaultPeriod(DatePicker from, DatePicker to)
        {
            var now = DateTime.Now;
            from.SelectedDate = new DateTime(now.Year, now.Month, 1);
            to.SelectedDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        }

        protected static DateTime? ToUtcStart(DateTime? d)
        {
            if (!d.HasValue) return null;
            var local = DateTime.SpecifyKind(d.Value.Date, DateTimeKind.Local);
            return local.ToUniversalTime();
        }

        protected static DateTime? ToUtcEndInclusive(DateTime? d)
        {
            if (!d.HasValue) return null;
            var localEnd = DateTime.SpecifyKind(d.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Local);
            return localEnd.ToUniversalTime();
        }

        protected async Task LoadAsync(DatePicker from, DatePicker to, ListView list, TextBox? searchBox = null)
        {
            var fromUtc = ToUtcStart(from.SelectedDate);
            var toUtc = ToUtcEndInclusive(to.SelectedDate);

            var data = await _api.GetAllAsync(fromUtc, toUtc);
            _all = (data ?? new List<OperationVM>()).Where(x => x.Type == Type).ToList();

            ApplyFilter(list, searchBox?.Text);
        }

        protected void ApplyFilter(ListView list, string? query)
        {
            var q = (query ?? "").Trim().ToLowerInvariant();
            IEnumerable<OperationVM> data = _all;

            if (!string.IsNullOrWhiteSpace(q))
            {
                data = data.Where(x =>
                       (x.NameDocument ?? "").ToLowerInvariant().Contains(q)
                    || (x.Comment ?? "").ToLowerInvariant().Contains(q)
                    || (x.Id ?? "").ToLowerInvariant().Contains(q));
            }

            // стабильная сортировка: по дате, затем по id
            data = data
                .OrderByDescending(x => x.DateOperation)
                .ThenByDescending(x => x.Id);

            var rows = data.Select(x => new OperationRowVM(x)).ToList();
            list.ItemsSource = rows;
        }

        protected static void UpdateCrudButtons(
            ListView list,
            Button openBtn,
            Button updateBtn,
            Button deleteBtn,
            Button restoreBtn)
        {
            if (list.SelectedItem is not OperationRowVM row)
            {
                openBtn.IsEnabled = false;
                updateBtn.IsEnabled = false;
                deleteBtn.IsEnabled = false;
                restoreBtn.IsEnabled = false;
                return;
            }

            openBtn.IsEnabled = true;
            updateBtn.IsEnabled = true;

            deleteBtn.IsEnabled = !row.IsDeleted;
            restoreBtn.IsEnabled = row.IsDeleted;
        }

        protected static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T typed) return typed;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        protected sealed class OperationRowVM
        {
            public OperationVM Source { get; }

            public string Id => Source.Id;
            public string NameDocument => Source.NameDocument;
            public string Comment => Source.Comment ?? "";
            public decimal TotalAmountDocument => Source.TotalAmountDocument;
            public bool IsDeleted => Source.IsDeleted;

            public string DateLocalText
            {
                get
                {
                    var dt = Source.DateOperation;
                    if (dt.Kind == DateTimeKind.Unspecified)
                        dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                    var local = dt.ToLocalTime();
                    return local.ToString("dd.MM.yyyy HH:mm");
                }
            }

            public OperationRowVM(OperationVM src) => Source = src;
        }
    }
}
