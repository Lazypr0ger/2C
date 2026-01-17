using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Contracts.Enums;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages
{
    public partial class ProductEditPage : Page
    {
        private readonly bool _isEdit;
        private readonly ProductionVM? _original;

        public ProductEditPage()
        {
            InitializeComponent();
            _isEdit = false;
            TitleText.Text = "Добавить продукт";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";
            IsDeletedCheck.IsEnabled = false;

            TypeBox.ItemsSource = Enum.GetValues(typeof(TypeProduct));
            TypeBox.SelectedIndex = 0;
        }

        public ProductEditPage(ProductionVM vm)
        {
            InitializeComponent();
            _isEdit = true;
            _original = vm;

            TitleText.Text = "Обновить продукт";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            TypeBox.ItemsSource = Enum.GetValues(typeof(TypeProduct));

            CodeBox.Text = vm.Code;
            NameBox.Text = vm.Name;
            TypeBox.SelectedItem = vm.Type;
            CostBox.Text = vm.PlannedCost.ToString(CultureInfo.InvariantCulture);
            IsDeletedCheck.IsChecked = vm.IsDeleted;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var code = CodeBox.Text.Trim();
            var name = NameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Код не должен быть пустым");
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название не должно быть пустым");
                return;
            }

            if (!decimal.TryParse(CostBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var cost))
            {
                MessageBox.Show("Плановая стоимость должна быть числом (например 1234.56)");
                return;
            }

            var type = (TypeProduct)(TypeBox.SelectedItem ?? (TypeProduct)0);

            MessageBox.Show(_isEdit
                ? $"Сохранено (редактирование): {code} {name}"
                : $"Сохранено (создание): {code} {name}");

            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
