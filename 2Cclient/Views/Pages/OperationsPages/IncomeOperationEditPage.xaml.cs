using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Contracts.Enums;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class IncomeOperationEditPage : Page
    {
        private readonly TypeDocument _type = TypeDocument.SuppliesDocument;

        private readonly List<IncomeRowVM> _rows = new();

        // CREATE
        public IncomeOperationEditPage()
        {
            InitializeComponent();
            InitCommon();
            DateOperationPicker.SelectedDate = DateTime.Today;

            ItemsList.ItemsSource = _rows;
        }

        // REPOST / EDIT
        public IncomeOperationEditPage(OperationVM vm)
        {
            InitializeComponent();
            InitCommon();

            NameDocumentBox.Text = vm.NameDocument;
            DateOperationPicker.SelectedDate = vm.DateOperation;

            // Заглушка: строки документа (позже получим по vm.Id через API)
            ItemsList.ItemsSource = _rows;
        }

        private void InitCommon()
        {
            // Заглушка: позже сюда придут продукты с API (/Production)
            ProductBox.ItemsSource = new List<SimpleProductVM>
            {
                new() { Id="1", Name="Продукт 1" },
                new() { Id="2", Name="Продукт 2" },
                new() { Id="3", Name="Продукт 3" }
            };
            ProductBox.DisplayMemberPath = "Name";
            ProductBox.SelectedValuePath = "Id";
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedItem is not SimpleProductVM product)
            {
                MessageBox.Show("Выберите продукт");
                return;
            }

            var rawCount = CountBox.Text.Trim();
            if (!int.TryParse(rawCount, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) || count <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом > 0");
                return;
            }

            var existing = _rows.FirstOrDefault(x => x.ProductId == product.Id);
            if (existing != null)
            {
                existing.Count += count;
                RefreshRows();
            }
            else
            {
                _rows.Add(new IncomeRowVM
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Count = count
                });
                RefreshRows();
            }

            CountBox.Text = "";
            ProductBox.SelectedItem = null;
        }

        private void RowDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not IncomeRowVM row) return;

            _rows.Remove(row);
            RefreshRows();
        }

        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // оставлено намеренно: логика удаления в строке
        }

        private void RefreshRows()
        {
            ItemsList.ItemsSource = null;
            ItemsList.ItemsSource = _rows;
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

            if (_rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один продукт в состав документа");
                return;
            }

            // OrganisationId = null (по требованию)
            MessageBox.Show(
                $"Сохранение (заглушка)\n" +
                $"Type={_type}\n" +
                $"Name={name}\n" +
                $"Date={DateOperationPicker.SelectedDate:dd.MM.yyyy}\n" +
                $"OrganisationId=null\n" +
                $"Rows:\n" +
                string.Join("\n", _rows.Select(r => $" - {r.ProductName}: {r.Count}")));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private class SimpleProductVM
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
        }

        private class IncomeRowVM
        {
            public string ProductId { get; set; } = "";
            public string ProductName { get; set; } = "";
            public int Count { get; set; }
        }
    }
}
