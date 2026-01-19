using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Contracts.Enums;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class SaleOperationEditPage : Page
    {
        private readonly OperationType _type = OperationType.Sale;

        private readonly List<SaleRowVM> _rows = new();

        public SaleOperationEditPage()
        {
            InitializeComponent();
            DateOperationPicker.SelectedDate = DateTime.Today;

            // Заглушки (позже API /Production и /Organisation)
            ProductBox.ItemsSource = new List<SimpleProductVM>
            {
                new() { Id="1", Name="Продукт 1" },
                new() { Id="2", Name="Продукт 2" },
                new() { Id="3", Name="Продукт 3" }
            };
            ProductBox.DisplayMemberPath = "Name";
            ProductBox.SelectedValuePath = "Id";

            OrganisationBox.ItemsSource = new List<SimpleOrgVM>
            {
                new() { Id="1", Name="Организация 1" },
                new() { Id="2", Name="Организация 2" }
            };
            OrganisationBox.DisplayMemberPath = "Name";
            OrganisationBox.SelectedValuePath = "Id";

            ItemsList.ItemsSource = _rows;
            TotalBox.Text = "0";
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedItem is not SimpleProductVM product)
            {
                MessageBox.Show("Выберите продукт");
                return;
            }

            if (!int.TryParse(CountBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) || count <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом > 0");
                return;
            }

            if (!decimal.TryParse(PriceBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price <= 0)
            {
                MessageBox.Show("Цена реализации должна быть числом > 0");
                return;
            }

            _rows.Add(new SaleRowVM
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Count = count,
                Price = RoundMoney(price),
            });

            RefreshRows();

            CountBox.Text = "";
            PriceBox.Text = "";
            ProductBox.SelectedItem = null;
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not SaleRowVM row) return;

            _rows.Remove(row);
            RefreshRows();
        }

        private void RefreshRows()
        {
            foreach (var r in _rows)
                r.Sum = RoundMoney(r.Price * r.Count);

            ItemsList.ItemsSource = null;
            ItemsList.ItemsSource = _rows;

            TotalBox.Text = RoundMoney(_rows.Sum(x => x.Sum)).ToString(CultureInfo.InvariantCulture);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameDocumentBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название документа не должно быть пустым");
                return;
            }

            if (DateOperationPicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату операции");
                return;
            }

            if (OrganisationBox.SelectedItem is not SimpleOrgVM org)
            {
                MessageBox.Show("Выберите организацию / контрагента");
                return;
            }

            if (_rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один продукт в состав документа");
                return;
            }

            MessageBox.Show(
                $"Сохранение (заглушка)\n" +
                $"Type={_type}\n" +
                $"Name={name}\n" +
                $"Date={DateOperationPicker.SelectedDate:dd.MM.yyyy}\n" +
                $"Organisation={org.Name}\n" +
                $"Total={TotalBox.Text}\n" +
                $"Rows:\n" +
                string.Join("\n", _rows.Select(r => $" - {r.ProductName}: {r.Count} x {r.Price} = {r.Sum}")));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        private class SimpleProductVM
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
        }

        private class SimpleOrgVM
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
        }

        private class SaleRowVM
        {
            public string ProductId { get; set; } = "";
            public string ProductName { get; set; } = "";
            public int Count { get; set; }
            public decimal Price { get; set; }
            public decimal Sum { get; set; }
        }
    }
}
