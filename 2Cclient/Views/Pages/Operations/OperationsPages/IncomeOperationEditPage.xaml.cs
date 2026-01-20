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
    public partial class IncomeOperationEditPage : Page
    {
        private readonly ApiClient _api;
        private OperationVM? _editing;

        private List<DepartamentVM> _departaments = new();
        private List<ProductionVM> _allProducts = new();

        // Локальный "черновик" строк документа
        private readonly List<ElementVM> _elements = new();

        // Count: только цифры
        private static readonly Regex DigitsOnlyRegex = new(@"^\d+$", RegexOptions.Compiled);

        public IncomeOperationEditPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ApiClient>();

            Loaded += async (_, __) =>
            {
                await LoadDirectoriesAsync();
                InitDefaultsForCreate();

                RefreshProductsByDepartament();
                RefreshItems();

                // очистка подсветок при первом входе
                FieldValidation.ClearError(NameDocumentBox);
                FieldValidation.ClearError(TimeBox);
                FieldValidation.ClearError(DepartamentBox);
                FieldValidation.ClearError(ProductBox);
                FieldValidation.ClearError(CountBox);
            };

            // paste restrictions
            DataObject.AddPastingHandler(NameDocumentBox, NameDocumentBox_OnPaste);
            DataObject.AddPastingHandler(TimeBox, TimeBox_OnPaste);
            DataObject.AddPastingHandler(CountBox, CountBox_OnPasteDigitsOnly);
        }

        public IncomeOperationEditPage(OperationVM op) : this()
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
            NameDocumentBox.Text = "";
            CountBox.Text = "";
        }

        private async Task LoadDirectoriesAsync()
        {
            _departaments = await _api.GetAsync<List<DepartamentVM>>("/ms/api/Departament") ?? new();
            _allProducts = await _api.GetAsync<List<ProductionVM>>("/ms/api/Production") ?? new();

            DepartamentBox.ItemsSource = _departaments;
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

            DepartamentBox.SelectedValue = op.DepartamentId;

            _elements.Clear();
            foreach (var e in op.Elements ?? new List<ElementVM>())
            {
                _elements.Add(new ElementVM
                {
                    Id = e.Id,
                    OperationId = op.Id ?? "",
                    ProductionId = e.ProductionId,
                    ProductionName = e.ProductionName,
                    CountElement = e.CountElement,
                    Price = null,
                    IsDeleted = false
                });
            }

            RefreshProductsByDepartament();
            RefreshItems();

            FieldValidation.ClearError(NameDocumentBox);
            FieldValidation.ClearError(TimeBox);
            FieldValidation.ClearError(DepartamentBox);
            FieldValidation.ClearError(ProductBox);
            FieldValidation.ClearError(CountBox);
        }

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

        // -------------------- Departament/Product --------------------
        private void DepartamentBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FieldValidation.ClearError(DepartamentBox);
            RefreshProductsByDepartament();
        }

        private void RefreshProductsByDepartament()
        {
            var depId = DepartamentBox.SelectedValue as string;

            var list = string.IsNullOrWhiteSpace(depId)
                ? new List<ProductionVM>()
                : _allProducts.Where(p => p.DepartamentId == depId && !p.IsDeleted).ToList();

            ProductBox.ItemsSource = list;

            // выбранный продукт по умолчанию
            ProductBox.SelectedIndex = list.Count > 0 ? 0 : -1;
        }

        // -------------------- Count (int > 0) --------------------
        private void CountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // только цифры
            e.Handled = e.Text.Any(ch => !char.IsDigit(ch));
        }

        private void CountBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void CountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // при вводе снимаем ошибку
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

        // -------------------- Items list --------------------
        private void RefreshItems()
        {
            var map = _allProducts.ToDictionary(x => x.Id, x => x.Name ?? x.Id);

            ItemsList.ItemsSource = _elements.Select(e => new RowVM
            {
                Id = e.Id,
                ProductionId = e.ProductionId,
                ProductionName = map.TryGetValue(e.ProductionId, out var n) ? n : e.ProductionId,
                CountElement = e.CountElement
            }).ToList();
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            // 1) departament
            if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
            {
                FieldValidation.SetError(DepartamentBox, "Сначала выберите подразделение");
                return;
            }
            FieldValidation.ClearError(DepartamentBox);

            // 2) product
            if (ProductBox.SelectedValue is not string prodId || string.IsNullOrWhiteSpace(prodId))
            {
                FieldValidation.SetError(ProductBox, "Выберите продукт");
                return;
            }
            FieldValidation.ClearError(ProductBox);

            // защита от несоответствия подразделения (если вдруг список не обновился)
            var prod = _allProducts.FirstOrDefault(p => p.Id == prodId);
            if (prod == null)
            {
                FieldValidation.SetError(ProductBox, "Продукт не найден");
                return;
            }
            if (prod.DepartamentId != depId)
            {
                FieldValidation.SetError(ProductBox, "Продукт не относится к выбранному подразделению");
                return;
            }

            // 3) count
            if (!TryParsePositiveInt(CountBox.Text, out var count))
            {
                FieldValidation.SetError(CountBox, "Количество должно быть целым числом > 0");
                return;
            }
            FieldValidation.ClearError(CountBox);

            // UX: если такой продукт уже добавлен — увеличиваем количество
            var existing = _elements.FirstOrDefault(x => x.ProductionId == prodId);
            if (existing != null)
            {
                existing.CountElement += count;
            }
            else
            {
                _elements.Add(new ElementVM
                {
                    Id = null,
                    OperationId = _editing?.Id ?? "",
                    ProductionId = prodId,
                    ProductionName = prod.Name,
                    CountElement = count,
                    Price = null,
                    IsDeleted = false
                });
            }

            CountBox.Text = "";
            RefreshItems();
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not RowVM row) return;

            // Сначала пробуем удалить по Id (если есть), иначе по ProductionId
            if (!string.IsNullOrWhiteSpace(row.Id))
            {
                var idxById = _elements.FindIndex(x => x.Id == row.Id);
                if (idxById >= 0) _elements.RemoveAt(idxById);
            }
            else
            {
                var idx = _elements.FindIndex(x => x.ProductionId == row.ProductionId);
                if (idx >= 0) _elements.RemoveAt(idx);
            }

            RefreshItems();
        }

        // -------------------- Date + Time -> UTC --------------------
        private bool TryGetUtcDateTime(out DateTime utc)
        {
            utc = default;

            if (DatePicker.SelectedDate == null)
            {
                // DatePicker у тебя без validated-style — просто message/return
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

                // 1) name
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

                // 2) departament
                if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
                {
                    FieldValidation.SetError(DepartamentBox, "Выберите подразделение");
                    return;
                }
                FieldValidation.ClearError(DepartamentBox);

                // 3) items
                if (_elements.Count == 0)
                {
                    MessageBox.Show("Добавьте хотя бы одну строку в состав документа.");
                    return;
                }

                // 4) datetime
                if (!TryGetUtcDateTime(out var utc))
                    return;

                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = name,
                    DateOperation = utc,
                    Type = OperationType.ReceiptFromProduction,
                    Comment = (CommentBox.Text ?? "").Trim(),
                    DepartamentId = depId,
                    OrganisationId = null,
                    TotalAmountDocument = null, // для поступления на склад может быть null (сервер посчитает/не нужно)
                    Elements = _elements.Select(x => new ElementBM
                    {
                        Id = x.Id,
                        OperationId = _editing?.Id,
                        ProductionId = x.ProductionId,
                        CountElement = x.CountElement,
                        Price = null,
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

        private sealed class RowVM
        {
            public string? Id { get; set; }
            public string ProductionId { get; set; } = "";
            public string ProductionName { get; set; } = "";
            public int CountElement { get; set; }
        }
    }
}
