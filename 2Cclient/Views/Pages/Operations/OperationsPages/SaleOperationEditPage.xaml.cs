using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages.Operations.OperationsPages
{
    public partial class SaleOperationEditPage : Page
    {
        private readonly ApiClient _api;
        private OperationVM? _editing;

        private List<OrganisationVM> _orgs = new();
        private List<ProductionVM> _products = new();
        private readonly List<ElementVM> _elements = new();

        public SaleOperationEditPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ApiClient>();

            Loaded += async (_, __) =>
            {
                await LoadDirectoriesAsync();
                InitDefaultsForCreate();
                RefreshItems();
            };
        }

        public SaleOperationEditPage(OperationVM op) : this()
        {
            _editing = op;
            Loaded += (_, __) => FillFromOperation(op);
        }

        private void InitDefaultsForCreate()
        {
            if (_editing != null) return;

            DatePicker.SelectedDate = DateTime.Now.Date;
            TimeBox.Text = DateTime.Now.ToString("HH:mm");
            CommentBox.Text = "";
        }

        private async Task LoadDirectoriesAsync()
        {
            _orgs = await _api.GetAsync<List<OrganisationVM>>("/ms/api/Organisation") ?? new();
            _products = await _api.GetAsync<List<ProductionVM>>("/ms/api/Production") ?? new();

            OrganisationBox.ItemsSource = _orgs.Where(x => !x.IsDeleted).ToList();
            ProductBox.ItemsSource = _products.Where(x => !x.IsDeleted).ToList();
        }

        private void FillFromOperation(OperationVM op)
        {
            NameDocumentBox.Text = op.NameDocument ?? "";
            CommentBox.Text = op.Comment ?? "";

            var dtUtc = op.DateOperation.Kind == DateTimeKind.Utc
                ? op.DateOperation
                : DateTime.SpecifyKind(op.DateOperation, DateTimeKind.Utc);

            var local = dtUtc.ToLocalTime();
            DatePicker.SelectedDate = local.Date;
            TimeBox.Text = local.ToString("HH:mm");

            OrganisationBox.SelectedValue = op.OrganisationId;

            _elements.Clear();
            foreach (var e in op.Elements ?? new List<ElementVM>())
                _elements.Add(new ElementVM
                {
                    Id = e.Id,
                    OperationId = op.Id,
                    ProductionId = e.ProductionId,
                    CountElement = e.CountElement,
                    Price = e.Price,
                    IsDeleted = false
                });

            RefreshItems();
        }

        private static bool TryParseInt(string? text, out int value)
            => int.TryParse((text ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        private static bool TryParseDecimal(string? text, out decimal value)
        {
            return decimal.TryParse((text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private bool TryGetUtcDateTime(out DateTime utc, out string error)
        {
            utc = default;
            error = "";

            if (DatePicker.SelectedDate == null)
            {
                error = "Дата не выбрана.";
                return false;
            }

            var timeText = (TimeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(timeText)) timeText = "00:00";

            if (!TimeSpan.TryParseExact(timeText, "hh\\:mm", CultureInfo.InvariantCulture, out var ts))
            {
                error = "Время должно быть в формате HH:mm.";
                return false;
            }

            var local = DatePicker.SelectedDate.Value.Date.Add(ts);
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);
            utc = local.ToUniversalTime();
            return true;
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedValue is not string prodId || string.IsNullOrWhiteSpace(prodId))
            {
                MessageBox.Show("Выберите продукт.");
                return;
            }

            if (!TryParseInt(CountBox.Text, out var count) || count <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом > 0.");
                return;
            }

            if (!TryParseDecimal(PriceBox.Text, out var price) || price <= 0m)
            {
                MessageBox.Show("Цена должна быть числом > 0.");
                return;
            }

            _elements.Add(new ElementVM
            {
                Id = null,
                OperationId = _editing?.Id ?? "",
                ProductionId = prodId,
                CountElement = count,
                Price = price,
                IsDeleted = false
            });

            CountBox.Text = "";
            PriceBox.Text = "";
            RefreshItems();
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not RowVM row) return;

            var idx = _elements.FindIndex(x =>
                x.ProductionId == row.ProductionId
                && x.CountElement == row.CountElement
                && x.Price == row.Price);

            if (idx >= 0) _elements.RemoveAt(idx);
            RefreshItems();
        }

        private void RefreshItems()
        {
            var map = _products.ToDictionary(x => x.Id, x => x.Name ?? x.Id);

            var rows = _elements.Select(e =>
            {
                var name = map.TryGetValue(e.ProductionId, out var n) ? n : e.ProductionId;
                var price = e.Price ?? 0m;
                var sum = e.CountElement * price; // ВАЖНО: правильная формула

                return new RowVM
                {
                    Id = e.Id,
                    ProductionId = e.ProductionId,
                    ProductionName = name,
                    CountElement = e.CountElement,
                    Price = price,
                    Sum = sum
                };
            }).ToList();

            ItemsList.ItemsSource = rows;
            TotalBox.Text = rows.Sum(x => x.Sum).ToString("N2");
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveBtn.IsEnabled = false;

                var name = (NameDocumentBox.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Заполните «Название документа».");
                    return;
                }

                if (OrganisationBox.SelectedValue is not string orgId || string.IsNullOrWhiteSpace(orgId))
                {
                    MessageBox.Show("Выберите покупателя (организацию).");
                    return;
                }

                if (_elements.Count == 0)
                {
                    MessageBox.Show("Добавьте хотя бы одну строку в состав документа.");
                    return;
                }

                if (!TryGetUtcDateTime(out var utc, out var err))
                {
                    MessageBox.Show(err);
                    return;
                }

                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = name,
                    DateOperation = utc,
                    Type = OperationType.Sale,
                    Comment = (CommentBox.Text ?? "").Trim(),
                    OrganisationId = orgId,
                    DepartamentId = null,
                    Elements = _elements.Select(x => new ElementBM
                    {
                        Id = x.Id,
                        OperationId = _editing?.Id,
                        ProductionId = x.ProductionId,
                        CountElement = x.CountElement,
                        Price = x.Price
                    }).ToList()
                };

                if (_editing == null)
                    await _api.PostAsync("/ms/api/Operation", bm);
                else
                    await _api.PutAsync("/ms/api/Operation", bm);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}");
            }
            finally
            {
                SaveBtn.IsEnabled = true;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true) NavigationService.GoBack();
        }

        private sealed class RowVM
        {
            public string? Id { get; set; }
            public string ProductionId { get; set; } = "";
            public string ProductionName { get; set; } = "";
            public int CountElement { get; set; }
            public decimal Price { get; set; }
            public decimal Sum { get; set; }

            public string PriceText => Price.ToString("N2");
            public string SumText => Sum.ToString("N2");
        }
    }
}
