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
                MessageBox.Show(ServerErrorPresenter.ToUserMessage(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // -------- Code (max 20) --------
        private void CodeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!CodeRegex.IsMatch(e.Text))
            {
                e.Handled = true;
                FieldValidation.SetError(CodeBox, "Код: A-Z, 0-9, '.', '_', '-'");
                return;
            }

            FieldValidation.EnforceMaxLen_PreviewTextInput(sender, e, FieldValidation.MaxNameLen);
        }

        private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldValidation.EnforceMaxLen_TextChanged(CodeBox, FieldValidation.MaxNameLen, "Код продукта");
            var t = CodeBox.Text ?? "";
            if (!string.IsNullOrWhiteSpace(t) && !CodeRegex.IsMatch(t))
                FieldValidation.SetError(CodeBox, "Код: A-Z, 0-9, '.', '_', '-'");
        }

        private void CodeBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText) ?? "").Trim();

            if (text.Length == 0)
            {
                e.CancelCommand();
                return;
            }

            if (!CodeRegex.IsMatch(text))
            {
                e.CancelCommand();
                FieldValidation.SetError(CodeBox, "Код: A-Z, 0-9, '.', '_', '-'");
                return;
            }

            FieldValidation.EnforceMaxLen_OnPaste(sender, e, FieldValidation.MaxNameLen);
        }

        // -------- Name (max 20) --------
        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!NameRegex.IsMatch(e.Text))
            {
                e.Handled = true;
                FieldValidation.SetError(NameBox, "Разрешены буквы/цифры/пробел/дефис/подчёркивание");
                return;
            }

            FieldValidation.EnforceMaxLen_PreviewTextInput(sender, e, FieldValidation.MaxNameLen);
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldValidation.EnforceMaxLen_TextChanged(NameBox, FieldValidation.MaxNameLen, "Название продукта");
            var t = NameBox.Text ?? "";
            if (!string.IsNullOrWhiteSpace(t) && !NameRegex.IsMatch(t))
                FieldValidation.SetError(NameBox, "Разрешены буквы/цифры/пробел/дефис/подчёркивание");
        }

        private void NameBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText) ?? "");
            text = Regex.Replace(text.Trim(), @"\s+", " ");

            if (text.Length == 0)
            {
                e.CancelCommand();
                return;
            }

            if (!NameRegex.IsMatch(text))
            {
                e.CancelCommand();
                FieldValidation.SetError(NameBox, "Разрешены буквы/цифры/пробел/дефис/подчёркивание");
                return;
            }

            FieldValidation.EnforceMaxLen_OnPaste(sender, e, FieldValidation.MaxNameLen);
        }

        // -------- Type / Departament --------
        private void TypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => FieldValidation.ClearError(TypeBox);

        private void DepartamentBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => FieldValidation.ClearError(DepartamentBox);

        // -------- Cost --------
        private void CostBox_PreviewKeyDown(object sender, KeyEventArgs e)
            => FieldValidation.Amount_PreviewKeyDown(sender, e);

        private void CostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Amount_PreviewTextInput(sender, e);

        private void CostBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(CostBox);

        private void CostBox_LostFocus(object sender, RoutedEventArgs e)
             => FieldValidation.ValidatePlannedCost(CostBox);

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
            if (code.Length > FieldValidation.MaxNameLen)
            {
                FieldValidation.SetError(CodeBox, $"Код: максимум {FieldValidation.MaxNameLen} символов");
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
            var name = Regex.Replace((NameBox.Text ?? "").Trim(), @"\s+", " ");
            if (string.IsNullOrWhiteSpace(name))
            {
                FieldValidation.SetError(NameBox, "Название не должно быть пустым");
                return false;
            }
            if (name.Length > FieldValidation.MaxNameLen)
            {
                FieldValidation.SetError(NameBox, $"Название: максимум {FieldValidation.MaxNameLen} символов");
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
            // Cost (плановая стоимость <= 1 000 000)
            if (!FieldValidation.TryParseDecimalStrict(CostBox.Text, out var cost) || cost <= 0m)
            {
                FieldValidation.SetError(CostBox, "Плановая стоимость должна быть числом > 0");
                return false;
            }
            if (cost > FieldValidation.MaxPlannedCost)
            {
                FieldValidation.SetError(CostBox, "Сумма слишком велика (максимум 1 000 000)");
                return false;
            }

            FieldValidation.ClearError(CostBox);
            CostBox.Text = cost.ToString("0.##", CultureInfo.InvariantCulture);


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
                MessageBox.Show(ServerErrorPresenter.ToUserMessage(ex),
                    "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Warning);
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
