using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;
using _2Cclient.UI;

namespace _2Cclient.Views.Pages
{
    public partial class ProductEditPage : Page
    {
        private readonly ProductionApi _api;
        private readonly DepartamentApi _departamentApi;

        private readonly ProductionVM? _editing;
        private List<DepartamentVM> _departaments = new();

        // Код: латиница/цифры/дефис/подчёркивание/точка
        private static readonly Regex CodeRegex = new(@"^[A-Za-z0-9_.\-]+$", RegexOptions.Compiled);
        // Название: буквы/цифры/пробел/дефис/подчёркивание
        private static readonly Regex NameRegex = new(@"^[\p{L}\p{Nd}\s\-_]+$", RegexOptions.Compiled);

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

            Loaded += async (_, __) =>
            {
                await LoadDepartamentsAsync();
                FieldValidation.ClearError(CodeBox);
                FieldValidation.ClearError(NameBox);
                FieldValidation.ClearError(CostBox);
                FieldValidation.ClearError(TypeBox);
                FieldValidation.ClearError(DepartamentBox);
                CodeBox.Focus();
            };

            DataObject.AddPastingHandler(CodeBox, CodeBox_OnPaste);
            DataObject.AddPastingHandler(NameBox, NameBox_OnPaste);
            DataObject.AddPastingHandler(CostBox, CostBox_OnPaste);
        }

        // UPDATE
        public ProductEditPage(ProductionVM vm) : this()
        {
            _editing = vm;

            TitleText.Text = "Обновить продукт";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            CodeBox.Text = vm.Code;
            NameBox.Text = vm.Name;
            CostBox.Text = vm.PlannedCost.ToString(CultureInfo.InvariantCulture);

            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = false;

            InitTypeCombo();
            TypeBox.SelectedValue = vm.Type;
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

        // -------- Code --------
        private void CodeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^[A-Za-z0-9_.\-]+$");

        private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(CodeBox);

        private void CodeBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText) ?? "").Trim();
            if (text.Length == 0 || !CodeRegex.IsMatch(text))
                e.CancelCommand();
        }

        // -------- Name --------
        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^[\p{L}\p{Nd}\s\-_]+$");

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(NameBox);

        private void NameBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText) ?? "").Trim();
            if (text.Length == 0 || !NameRegex.IsMatch(text))
                e.CancelCommand();
        }

        // -------- Type / Departament --------
        private void TypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => FieldValidation.ClearError(TypeBox);

        private void DepartamentBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => FieldValidation.ClearError(DepartamentBox);

        // -------- Cost (используем те же правила, что Amount) --------
        private void CostBox_PreviewKeyDown(object sender, KeyEventArgs e)
            => FieldValidation.Amount_PreviewKeyDown(sender, e);

        private void CostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Amount_PreviewTextInput(sender, e);

        private void CostBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(CostBox);

        private void CostBox_LostFocus(object sender, RoutedEventArgs e)
            => FieldValidation.Amount_LostFocus(CostBox);

        private void CostBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Amount_OnPaste(sender, e);

        private bool ValidateForm()
        {
            // Code
            var code = (CodeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                FieldValidation.SetError(CodeBox, "Код не должен быть пустым");
                return false;
            }
            if (!CodeRegex.IsMatch(code))
            {
                FieldValidation.SetError(CodeBox, "Код: A-Z, 0-9, '.', '_', '-'");
                return false;
            }
            FieldValidation.ClearError(CodeBox);
            CodeBox.Text = code;

            // Name
            var name = (NameBox.Text ?? "").Trim();
            name = Regex.Replace(name, @"\s+", " ");
            if (string.IsNullOrWhiteSpace(name))
            {
                FieldValidation.SetError(NameBox, "Название не должно быть пустым");
                return false;
            }
            if (!NameRegex.IsMatch(name))
            {
                FieldValidation.SetError(NameBox, "Разрешены буквы/цифры/пробел/дефис/подчёркивание");
                return false;
            }
            FieldValidation.ClearError(NameBox);
            NameBox.Text = name;

            // Type
            if (TypeBox.SelectedValue is not TypeProduct type || type == TypeProduct.None)
            {
                FieldValidation.SetError(TypeBox, "Выберите тип продукта");
                return false;
            }
            FieldValidation.ClearError(TypeBox);

            // Departament
            if (DepartamentBox.SelectedValue is not string departamentId || string.IsNullOrWhiteSpace(departamentId))
            {
                FieldValidation.SetError(DepartamentBox, "Выберите подразделение");
                return false;
            }
            FieldValidation.ClearError(DepartamentBox);

            // Cost
            if (!FieldValidation.TryParseDecimalStrict(CostBox.Text, out var cost) || cost <= 0m)
            {
                FieldValidation.SetError(CostBox, "Плановая стоимость должна быть числом > 0");
                return false;
            }
            FieldValidation.ClearError(CostBox);
            CostBox.Text = cost.ToString("0.##", CultureInfo.InvariantCulture);

            return true;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            SetBusy(true);

            try
            {
                var code = CodeBox.Text.Trim();
                var name = NameBox.Text.Trim();
                var type = (TypeProduct)TypeBox.SelectedValue!;
                var departamentId = (string)DepartamentBox.SelectedValue!;

                FieldValidation.TryParseDecimalStrict(CostBox.Text, out var cost);

                if (_editing is null)
                {
                    var bm = new ProductionBM
                    {
                        Code = code,
                        Name = name,
                        Type = type,
                        PlannedCost = cost,
                        DepartamentId = departamentId
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
            SaveBtn.IsEnabled = !isBusy;
        }
    }
}
