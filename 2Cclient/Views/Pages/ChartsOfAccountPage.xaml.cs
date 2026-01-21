using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class ChartsOfAccountPage : Page
    {
        private readonly ChartOfAccountApi _api;

        public ObservableCollection<ChartTileVM> Items { get; } = new();

        public ChartsOfAccountPage()
        {
            InitializeComponent();
            DataContext = this;

            _api = App.Services.GetRequiredService<ChartOfAccountApi>();

            Loaded += async (_, __) => await LoadAsync();
        }

        private async Task LoadAsync(CancellationToken ct = default)
        {
            try
            {
                Items.Clear();

                var list = await _api.GetAllAsync(ct) ?? new();

                foreach (var c in list.Where(x => !x.IsDeleted).OrderBy(x => x.NumChart))
                {
                    Items.Add(new ChartTileVM
                    {
                        Id = c.Id,
                        NumChart = c.NumChart ?? "",
                        Name = c.Name ?? "",
                        Subconto1 = c.Subconto1,
                        Subconto2 = c.Subconto2
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки плана счетов:\n{ex.Message}");
            }
        }

        public sealed class ChartTileVM
        {
            public string? Id { get; set; }
            public string NumChart { get; set; } = "";
            public string Name { get; set; } = "";

            public string? Subconto1 { get; set; }
            public string? Subconto2 { get; set; }

            public bool HasSubconto =>
                !string.IsNullOrWhiteSpace(Subconto1) || !string.IsNullOrWhiteSpace(Subconto2);

            public string SubcontoText
            {
                get
                {
                    var parts = new[] { Subconto1, Subconto2 }
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();

                    // если субконто нет — пусто (и блок будет скрыт в XAML)
                    return parts.Count == 0 ? "" : "Субконто: " + string.Join(", ", parts);
                }
            }
        }
    }
}
