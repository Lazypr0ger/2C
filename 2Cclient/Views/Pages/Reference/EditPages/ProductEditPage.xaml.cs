using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;

namespace _2Cclient.Views.Pages
{
    public partial class ProductEditPage : Page
    {
        private readonly ProductionApi _api;
        private readonly DepartamentApi _departamentApi;

        private readonly ProductionVM? _editing;

        private List<DepartamentVM> _departaments = new();

        // CREATE
        public ProductEditPage()
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<ProductionApi>();
            _departamentApi = App.Services.GetRequiredService<DepartamentApi>();
            _editing = null;

            TitleText.Text = "Добавить продукт";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";

            IsDeletedCheck.IsChecked = false;
            IsDeletedCheck.IsEnabled = false;

            InitTypeCombo();

            Loaded += async (_, __) => await LoadDepartamentsAsync();
        }

        // UPDATE
        public ProductEditPage(ProductionVM vm)
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<ProductionApi>();
            _departamentApi = App.Services.GetRequiredService<DepartamentApi>();
            _editing = vm;

            TitleText.Text = "Обновить продукт";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            CodeBox.Text = vm.Code;
            NameBox.Text = vm.Name;
            CostBox.Text = vm.PlannedCost.ToString(CultureInfo.InvariantCulture);

            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = false; // удаление/восстановление через кнопки списка

            InitTypeCombo();
            TypeBox.SelectedValue = vm.Type;

            Loaded += async (_, __) => await LoadDepartamentsAsync();
        }

        private void InitTypeCombo()
        {

            TypeBox.Items.Clear();

            AddType(TypeProduct.None, "— Не выбран —");
            AddType(TypeProduct.product_1, "Продукт 1");
            AddType(TypeProduct.product_2, "Продукт 2");
            AddType(TypeProduct.product_3, "Продукт 3");
            AddType(TypeProduct.product_4, "Продукт 4");
            AddType(TypeProduct.product_5, "Продукт 5");

            TypeBox.DisplayMemberPath = "Content";
            TypeBox.SelectedValuePath = "Tag";

            TypeBox.SelectedValue = _editing?.Type ?? TypeProduct.None;
        }

        private void AddType(TypeProduct value, string title)
        {
            TypeBox.Items.Add(new ComboBoxItem { Tag = value, Content = title });
        }

        private async Task LoadDepartamentsAsync()
        {
            try
            {
                var data = await _departamentApi.GetAllAsync();
                _departaments = data ?? new List<DepartamentVM>();

                // выбираем только активные
                var active = _departaments.Where(d => !d.IsDeleted).ToList();

                DepartamentBox.ItemsSource = active;

                if (_editing != null)
                    DepartamentBox.SelectedValue = _editing.DepartamentId;
                else
                    DepartamentBox.SelectedIndex = active.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки подразделений:\n{ex.Message}");
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
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

            if (TypeBox.SelectedValue is not TypeProduct type || type == TypeProduct.None)
            {
                MessageBox.Show("Выберите тип продукта");
                return;
            }

            if (DepartamentBox.SelectedValue is not string departamentId || string.IsNullOrWhiteSpace(departamentId))
            {
                MessageBox.Show("Выберите подразделение");
                return;
            }

            if (!decimal.TryParse(CostBox.Text.Trim().Replace(',', '.'),
                                  NumberStyles.Any,
                                  CultureInfo.InvariantCulture,
                                  out var cost))
            {
                MessageBox.Show("Плановая стоимость должна быть числом");
                return;
            }

            SetBusy(true);

            try
            {
                if (_editing is null)
                {
                    var bm = new ProductionBM
                    {
                        Code = code,
                        Name = name,
                        Type = type,
                        PlannedCost = cost,
                        DepartamentId = departamentId
                        // Id = null -> сервер сгенерит
                        // IsDeleted -> сервер выставит false
                    };

                    await _api.CreateAsync(bm);
                }
                else
                {
                    var bm = new ProductionBM
                    {
                        Id = _editing.Id,
                        Code = code,
                        Name = name,
                        Type = type,
                        PlannedCost = cost,
                        DepartamentId = departamentId,
                        IsDeleted = _editing.IsDeleted
                    };

                    await _api.UpdateAsync(bm);
                }

                if (NavigationService?.CanGoBack == true)
                    NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}");
                SetBusy(false);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void SetBusy(bool isBusy)
        {
            CodeBox.IsEnabled = !isBusy;
            NameBox.IsEnabled = !isBusy;
            CostBox.IsEnabled = !isBusy;
            TypeBox.IsEnabled = !isBusy;
            DepartamentBox.IsEnabled = !isBusy;

            IsDeletedCheck.IsEnabled = false;
        }
    }
}
