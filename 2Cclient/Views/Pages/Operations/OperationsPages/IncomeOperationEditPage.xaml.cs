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
    public partial class IncomeOperationEditPage : Page
    {
        private readonly ApiClient _api;
        private OperationVM? _editing;

        private List<DepartamentVM> _departaments = new();
        private List<ProductionVM> _allProducts = new();
        private readonly List<ElementVM> _elements = new();

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
            };
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
                _elements.Add(new ElementVM
                {
                    Id = e.Id,
                    OperationId = op.Id,
                    ProductionId = e.ProductionId,
                    CountElement = e.CountElement,
                    Price = null,
                    IsDeleted = false
                });

            RefreshProductsByDepartament();
            RefreshItems();
        }

        private void DepartamentBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => RefreshProductsByDepartament();

        private void RefreshProductsByDepartament()
        {
            var depId = DepartamentBox.SelectedValue as string;
            var list = string.IsNullOrWhiteSpace(depId)
                ? new List<ProductionVM>()
                : _allProducts.Where(p => p.DepartamentId == depId && !p.IsDeleted).ToList();

            ProductBox.ItemsSource = list;
            ProductBox.SelectedIndex = list.Count > 0 ? 0 : -1;
        }

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

        private static bool TryParseInt(string? text, out int value)
            => int.TryParse((text ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

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
            if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
            {
                MessageBox.Show("Сначала выберите подразделение.");
                return;
            }

            if (ProductBox.SelectedValue is not string prodId || string.IsNullOrWhiteSpace(prodId))
            {
                MessageBox.Show("Выберите продукт.");
                return;
            }

            // защита от несоответствия подразделения
            var prod = _allProducts.FirstOrDefault(p => p.Id == prodId);
            if (prod == null)
            {
                MessageBox.Show("Продукт не найден.");
                return;
            }
            if (prod.DepartamentId != depId)
            {
                MessageBox.Show("Выбранный продукт не относится к выбранному подразделению.");
                return;
            }

            if (!TryParseInt(CountBox.Text, out var count) || count <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом > 0.");
                return;
            }

            _elements.Add(new ElementVM
            {
                Id = null,
                OperationId = _editing?.Id ?? "",
                ProductionId = prodId,
                CountElement = count,
                Price = null,
                IsDeleted = false
            });

            CountBox.Text = "";
            RefreshItems();
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not RowVM row) return;

            var idx = _elements.FindIndex(x => x.ProductionId == row.ProductionId && x.CountElement == row.CountElement);
            if (idx >= 0) _elements.RemoveAt(idx);

            RefreshItems();
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

                if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
                {
                    MessageBox.Show("Выберите подразделение.");
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
                    Type = OperationType.ReceiptFromProduction,
                    Comment = (CommentBox.Text ?? "").Trim(),
                    DepartamentId = depId,
                    OrganisationId = null,
                    Elements = _elements.Select(x => new ElementBM
                    {
                        Id = x.Id,
                        OperationId = _editing?.Id,
                        ProductionId = x.ProductionId,
                        CountElement = x.CountElement,
                        Price = null
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
        }
    }
}
