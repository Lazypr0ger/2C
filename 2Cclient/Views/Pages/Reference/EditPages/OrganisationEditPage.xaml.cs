using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Contracts.BindingModels;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;
using _2Cclient.UI;

namespace _2Cclient.Views.Pages
{
    public partial class OrganisationEditPage : Page
    {
        private readonly OrganisationApi _api;
        private readonly OrganisationVM? _editing;

        // буквы/цифры/пробел/дефис/подчёркивание
        private static readonly Regex NameRegex = new(@"^[\p{L}\p{Nd}\s\-_]+$", RegexOptions.Compiled);

        public OrganisationEditPage()
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<OrganisationApi>();
            _editing = null;

            TitleText.Text = "Добавить организацию";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";

            IsDeletedCheck.IsChecked = false;
            IsDeletedCheck.IsEnabled = false;

            Loaded += (_, __) =>
            {
                FieldValidation.ClearError(NameBox);
                FieldValidation.ClearError(AccountBox);
                NameBox.Focus();
            };

            DataObject.AddPastingHandler(NameBox, NameBox_OnPaste);
            DataObject.AddPastingHandler(AccountBox, AccountBox_OnPaste);
        }

        public OrganisationEditPage(OrganisationVM vm) : this()
        {
            _editing = vm;

            TitleText.Text = "Обновить организацию";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            AccountBox.Text = vm.AccountNumOrg;

            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = false;
        }

        // -------- Name --------
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
            FieldValidation.EnforceMaxLen_TextChanged(NameBox, FieldValidation.MaxNameLen, "Название организации");

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

        // -------- Account (20 digits) --------
        private void AccountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.OrgAccount_PreviewTextInput(sender, e);

        private void AccountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // при вводе — максимум 20, снимаем ошибки
            FieldValidation.EnforceMaxLen_TextChanged(AccountBox, FieldValidation.OrgAccountLen, "Счёт организации");
            if (!string.IsNullOrWhiteSpace(AccountBox.Text))
                FieldValidation.ClearError(AccountBox);
        }

        private void AccountBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.OrgAccount_OnPaste(sender, e);

        private void AccountBox_LostFocus(object sender, RoutedEventArgs e)
            => FieldValidation.OrgAccount_LostFocus(AccountBox);

        private bool ValidateForm()
        {
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

            // Account строго 20 цифр
            FieldValidation.OrgAccount_LostFocus(AccountBox);
            if (Validation.GetHasError(AccountBox))
                return false;

            return true;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            SetBusy(true);

            try
            {
                var name = NameBox.Text.Trim();
                var account = AccountBox.Text.Trim();

                if (_editing is null)
                {
                    var model = new OrganisationBM
                    {
                        Name = name,
                        AccountNumOrg = account
                    };
                    await _api.CreateAsync(model);
                }
                else
                {
                    var model = new OrganisationBM
                    {
                        Id = _editing.Id,
                        Name = name,
                        AccountNumOrg = account,
                        IsDeleted = _editing.IsDeleted
                    };
                    await _api.UpdateAsync(model);
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
            NameBox.IsEnabled = !isBusy;
            AccountBox.IsEnabled = !isBusy;
            SaveBtn.IsEnabled = !isBusy;
        }
    }
}
