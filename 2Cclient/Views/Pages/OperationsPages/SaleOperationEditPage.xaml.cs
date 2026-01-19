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

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class SaleOperationEditPage : Page
    {
        private readonly OperationApi _opApi;
        private readonly ProductionApi _prodApi;
        private readonly OrganisationApi _orgApi;

        private readonly OperationVM? _editing;
        private readonly List<RowVM> _rows = new();

        public SaleOperationEditPage()
        {
            InitializeComponent();
            _opApi = App.Services.GetRequiredService<OperationApi>();
            _prodApi = App.Services.GetRequiredService<ProductionApi>();
            _orgApi = App.Services.GetRequiredService<OrganisationApi>();
            _editing = null;

            Loaded += async (_, __) => await InitAsync();
        }

        public SaleOperationEditPage(OperationVM vm)
        {
            InitializeComponent();
            _opApi = App.Services.GetRequiredService<OperationApi>();
            _prodApi = App.Services.GetRequiredService<ProductionApi>();
            _orgApi = App.Services.GetRequiredService<OrganisationApi>();
            _editing = vm;

            Loaded += async (_, __) => await InitAsync();
        }

        private async Task InitAsync()
        {
            try
            {
                var products = await _prodApi.GetAllAsync();
                ProductBox.ItemsSource = products ?? new List<ProductionVM>();
                ProductBox.DisplayMemberPath = "Name";
                ProductBox.SelectedValuePath = "Id";

                var orgs = await _orgApi.GetAllAsync();
                OrganisationBox.ItemsSource = orgs ?? new List<OrganisationVM>();
                OrganisationBox.DisplayMemberPath = "Name";
                OrganisationBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников:\n{ex.Message}");
            }

            if (_editing != null)
            {
                NameDocumentBox.Text = _editing.NameDocument;
                DateOperationPicker.SelectedDate = _editing.DateOperation.ToLocalTime().Date;
                OrganisationBox.SelectedValue = _editing.OrganisationId;

                _rows.Clear();
                foreach (var e in _editing.Elements ?? new List<ElementVM>())
                {
                    _rows.Add(new RowVM
                    {
                        Id = e.Id,
                        ProductionId = e.ProductionId,
                        Count = e.CountElement,
                        Price = e.Price ?? 0m
                    });
                }

                ItemsList.ItemsSource = _rows;
                RecalcTotal();
            }
            else
            {
                NameDocumentBox.Text = "Реализация";
                DateOperationPicker.SelectedDate = DateTime.Today;
                ItemsList.ItemsSource = _rows;
                TotalBox.Text = "0";
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedValue is not string prodId || string.IsNullOrWhiteSpace(prodId))
            {
                MessageBox.Show("Выберите продукт");
                return;
            }

            if (!int.TryParse(CountBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var cnt) || cnt <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом > 0");
                return;
            }

            if (!decimal.TryParse(PriceBox.Text.Trim().Replace(',', '.'),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price <= 0m)
            {
                MessageBox.Show("Цена должна быть числом > 0");
                return;
            }

            _rows.Add(new RowVM
            {
                ProductionId = prodId,
                Count = cnt,
                Price = price
            });

            CountBox.Text = string.Empty;
            PriceBox.Text = string.Empty;

            ItemsList.ItemsSource = null;
            ItemsList.ItemsSource = _rows;

            RecalcTotal();
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not RowVM row) return;
            _rows.Remove(row);

            ItemsList.ItemsSource = null;
            ItemsList.ItemsSource = _rows;

            RecalcTotal();
        }

        private void RecalcTotal()
        {
            var total = _rows.Sum(x => x.Sum);
            TotalBox.Text = total.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameDocumentBox.Text))
            {
                MessageBox.Show("Название документа не должно быть пустым");
                return;
            }

            if (DateOperationPicker.SelectedDate is null)
            {
                MessageBox.Show("Укажите дату операции");
                return;
            }

            if (OrganisationBox.SelectedValue is not string orgId || string.IsNullOrWhiteSpace(orgId))
            {
                MessageBox.Show("Выберите контрагента");
                return;
            }

            if (_rows.Count == 0)
            {
                MessageBox.Show("Добавьте строки документа");
                return;
            }

            SetBusy(true);

            try
            {
                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = NameDocumentBox.Text.Trim(),
                    DateOperation = ToUtc(DateOperationPicker.SelectedDate.Value),
                    Type = OperationType.Sale,
                    OrganisationId = orgId,
                    DepartamentId = null,
                    TotalAmountDocument = 0m,
                    Elements = _rows.Select(r => new ElementBM
                    {
                        Id = r.Id,
                        ProductionId = r.ProductionId,
                        CountElement = r.Count,
                        Price = r.Price,
                        IsDeleted = false
                    }).ToList(),
                    IsDeleted = _editing?.IsDeleted ?? false
                };

                if (_editing is null)
                    await _opApi.CreateAsync(bm);
                else
                    await _opApi.UpdateAsync(bm);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}");
                SetBusy(false);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();

        private void SetBusy(bool busy)
        {
            NameDocumentBox.IsEnabled = !busy;
            DateOperationPicker.IsEnabled = !busy;
            OrganisationBox.IsEnabled = !busy;

            ProductBox.IsEnabled = !busy;
            WarehouseBox.IsEnabled = !busy;
            CountBox.IsEnabled = !busy;
            PriceBox.IsEnabled = !busy;
        }

        private static DateTime ToUtc(DateTime localDate)
        {
            var local = DateTime.SpecifyKind(localDate.Date, DateTimeKind.Local);
            return local.ToUniversalTime();
        }

        private sealed class RowVM
        {
            public string? Id { get; set; }
            public string ProductionId { get; set; } = string.Empty;
            public int Count { get; set; }
            public decimal Price { get; set; }
            public decimal Sum => Count * Price;
        }
    }
}
