using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _2Cclient.Services.Api;
using _2Cclient.UI;
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

        private List<OrganisationVM> _organisations = new();
        private List<ProductionVM> _products = new();
        private readonly List<ElementVM> _elements = new();
        private readonly List<SaleRow> _rows = new();

        private static readonly Regex DigitsOnlyRegex = new(@"^\d+$", RegexOptions.Compiled);

        public SaleOperationEditPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ApiClient>();

            Loaded += async (_, __) =>
            {
                await LoadDirectoriesAsync();
                InitDefaultsForCreate();

                RefreshItems();
                RecalcTotal();

                FieldValidation.ClearError(NameDocumentBox);
                FieldValidation.ClearError(TimeBox);
                FieldValidation.ClearError(OrganisationBox);
                FieldValidation.ClearError(ProductBox);
                FieldValidation.ClearError(CountBox);
                FieldValidation.ClearError(PriceBox);
            };

            // paste restrictions
            DataObject.AddPastingHandler(NameDocumentBox, NameDocumentBox_OnPaste);
            DataObject.AddPastingHandler(TimeBox, TimeBox_OnPaste);
            DataObject.AddPastingHandler(CountBox, CountBox_OnPasteDigitsOnly);
            DataObject.AddPastingHandler(PriceBox, PriceBox_OnPaste);
        }

        public SaleOperationEditPage(OperationVM op) : this()
        {
            _editing = op;
            Loaded += (_, __) => FillFromOperation(op);
        }

        private async Task LoadDirectoriesAsync()
        {
            _organisations = await _api.GetAsync<List<OrganisationVM>>("/ms/api/Organisation") ?? new();
            _products = await _api.GetAsync<List<ProductionVM>>("/ms/api/Production") ?? new();

            OrganisationBox.ItemsSource = _organisations.Where(x => !x.IsDeleted).ToList();
            ProductBox.ItemsSource = _products.Where(x => !x.IsDeleted).ToList();

            OrganisationBox.SelectedIndex = OrganisationBox.Items.Count > 0 ? 0 : -1;
            ProductBox.SelectedIndex = ProductBox.Items.Count > 0 ? 0 : -1;
        }

        private void InitDefaultsForCreate()
        {
            if (_editing != null) return;

            DatePicker.SelectedDate = DateTime.Now.Date;
            TimeBox.Text = DateTime.Now.ToString("HH:mm");
            CommentBox.Text = "";
            NameDocumentBox.Text = "";
            CountBox.Text = "";
            PriceBox.Text = "";
            TotalBox.Text = "";
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

            _rows.Clear();

            foreach (var e in op.Elements ?? new List<ElementVM>())
            {
                _rows.Add(new SaleRow
                {
                    ProductionId = e.ProductionId,
                    ProductionName = e.ProductionName ?? ResolveProductName(e.ProductionId),
                    Count = e.CountElement,
                    Price = (e.Price ?? 0m)
                });
            }

            RefreshItems();
            RecalcTotal();

            FieldValidation.ClearError(NameDocumentBox);
            FieldValidation.ClearError(TimeBox);
            FieldValidation.ClearError(OrganisationBox);
            FieldValidation.ClearError(ProductBox);
            FieldValidation.ClearError(CountBox);
            FieldValidation.ClearError(PriceBox);
        }

        private string ResolveProductName(string productId)
            => _products.FirstOrDefault(x => x.Id == productId)?.Name ?? productId;

        // -------------------- Name --------------------
        private void NameDocumentBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Name_PreviewTextInput(sender, e);

        private void NameDocumentBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.Name_TextChanged(NameDocumentBox);

        private void NameDocumentBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Name_OnPaste(sender, e);

        // -------------------- Time --------------------
        private void TimeBox_PreviewKeyDown(object sender, KeyEventArgs e)
            => FieldValidation.Time_PreviewKeyDown(sender, e);

        private void TimeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Time_PreviewTextInput(sender, e);

        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.Time_TextChanged(TimeBox);

        private void TimeBox_LostFocus(object sender, RoutedEventArgs e)
            => FieldValidation.Time_LostFocus(TimeBox);

        private void TimeBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Time_OnPasteDigitsOnly(sender, e);

        // -------------------- Organisation --------------------
        private void OrganisationBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FieldValidation.ClearError(OrganisationBox);
        }

        // -------------------- Count (int > 0) --------------------
        private void CountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(ch => !char.IsDigit(ch));
        }

        private void CountBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void CountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CountBox.Text))
                FieldValidation.ClearError(CountBox);
        }

        private void CountBox_OnPasteDigitsOnly(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText)) ?? "";
            text = text.Trim();

            if (string.IsNullOrWhiteSpace(text) || text.Any(ch => !char.IsDigit(ch)))
                e.CancelCommand();
        }

        private static bool TryParsePositiveInt(string? text, out int value)
        {
            value = 0;
            var t = (text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(t)) return false;
            if (!DigitsOnlyRegex.IsMatch(t)) return false;

            return int.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value > 0;
        }

        // -------------------- Price (decimal > 0) --------------------
        private void PriceBox_PreviewKeyDown(object sender, KeyEventArgs e)
            => FieldValidation.Amount_PreviewKeyDown(sender, e);

        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Amount_PreviewTextInput(sender, e);

        private void PriceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            FieldValidation.ClearErrorOnTyping(PriceBox);

        }

        private void PriceBox_LostFocus(object sender, RoutedEventArgs e)
            => FieldValidation.ValidateSalePrice(PriceBox);

        private void PriceBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Amount_OnPaste(sender, e);

        // -------------------- Rows --------------------
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            // product
            if (ProductBox.SelectedValue is not string prodId || string.IsNullOrWhiteSpace(prodId))
            {
                FieldValidation.SetError(ProductBox, "Выберите продукт");
                return;
            }
            FieldValidation.ClearError(ProductBox);

            // count
            if (!TryParsePositiveInt(CountBox.Text, out var count))
            {
                FieldValidation.SetError(CountBox, "Количество должно быть целым числом > 0");
                return;
            }
            FieldValidation.ClearError(CountBox);

            if (!FieldValidation.ValidateSalePrice(PriceBox))
                return;

            FieldValidation.TryParseDecimalStrict(PriceBox.Text,out var price);

            var prodName = ResolveProductName(prodId);

            // UX: если тот же продукт и та же цена — суммируем количество
            var existing = _rows.FirstOrDefault(x => x.ProductionId == prodId && x.Price == price);
            if (existing != null)
            {
                existing.Count += count;
            }
            else
            {
                _rows.Add(new SaleRow
                {
                    ProductionId = prodId,
                    ProductionName = prodName,
                    Count = count,
                    Price = price
                });
            }

            CountBox.Text = "";
            PriceBox.Text = "";

            RefreshItems();
            RecalcTotal();
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not SaleRow row) return;

            var idx = _rows.FindIndex(r =>
                r.ProductionId == row.ProductionId &&
                r.Price == row.Price &&
                r.Count == row.Count); // или добавь нормальный Id строки

            if (idx >= 0)
                _rows.RemoveAt(idx);

            RefreshItems();
            RecalcTotal();
        }


        private void RefreshItems()
        {
            ItemsList.ItemsSource = _rows
                .Select(r => new SaleRow
                {
                    ProductionId = r.ProductionId,
                    ProductionName = r.ProductionName,
                    Count = r.Count,
                    Price = r.Price
                })
                .ToList();
        }

        private void RecalcTotal()
        {
            var total = _rows.Sum(x => x.Sum);
            TotalBox.Text = total.ToString("0.##", CultureInfo.InvariantCulture);
        }

        // -------------------- Date + Time -> UTC --------------------
        private bool TryGetUtcDateTime(out DateTime utc)
        {
            utc = default;

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Дата не выбрана.");
                return false;
            }

            var timeText = (TimeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(timeText))
                timeText = "00:00";

            if (!FieldValidation.TimeStrictRegex.IsMatch(timeText))
            {
                FieldValidation.SetError(TimeBox, "Время в формате HH:mm (например 12:00)");
                return false;
            }

            FieldValidation.ClearError(TimeBox);

            var ts = TimeSpan.ParseExact(timeText, "hh\\:mm", CultureInfo.InvariantCulture);

            var local = DatePicker.SelectedDate.Value.Date.Add(ts);
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);
            utc = local.ToUniversalTime();
            return true;
        }

        // -------------------- Save --------------------
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveBtn.IsEnabled = false;

                // name
                var name = (NameDocumentBox.Text ?? "").Trim();
                name = Regex.Replace(name, @"\s+", " ");

                if (string.IsNullOrWhiteSpace(name))
                {
                    FieldValidation.SetError(NameDocumentBox, "Название документа обязательно");
                    return;
                }

                if (!FieldValidation.NameAllowedRegex.IsMatch(name))
                {
                    FieldValidation.SetError(NameDocumentBox, "Только русские/английские буквы и пробел");
                    return;
                }

                FieldValidation.ClearError(NameDocumentBox);

                // organisation (buyer)
                if (OrganisationBox.SelectedValue is not string orgId || string.IsNullOrWhiteSpace(orgId))
                {
                    FieldValidation.SetError(OrganisationBox, "Выберите покупателя");
                    return;
                }
                FieldValidation.ClearError(OrganisationBox);

                // rows
                if (_rows.Count == 0)
                {
                    MessageBox.Show("Добавьте хотя бы одну строку в состав документа.");
                    return;
                }

                // datetime
                if (!TryGetUtcDateTime(out var utc))
                    return;

                // total for server (может пригодиться для контроля)
                var total = _rows.Sum(x => x.Sum);

                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = name,
                    DateOperation = utc,
                    Type = OperationType.Sale, // <-- ВАЖНО: проверь, что enum именно так называется у тебя
                    Comment = (CommentBox.Text ?? "").Trim(),
                    OrganisationId = orgId,
                    DepartamentId = null,
                    TotalAmountDocument = total,
                    Elements = _rows.Select(r => new ElementBM
                    {
                        Id = null,
                        OperationId = _editing?.Id,
                        ProductionId = r.ProductionId,
                        CountElement = r.Count,
                        Price = r.Price,
                        IsDeleted = false
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
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private sealed class SaleRow
        {
            public string ProductionId { get; set; } = "";
            public string ProductionName { get; set; } = "";

            public int Count { get; set; }
            public decimal Price { get; set; }

            public decimal Sum => Count * Price;

            public int CountElement => Count;

            public string PriceText => Price.ToString("0.##", CultureInfo.InvariantCulture);
            public string SumText => Sum.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
