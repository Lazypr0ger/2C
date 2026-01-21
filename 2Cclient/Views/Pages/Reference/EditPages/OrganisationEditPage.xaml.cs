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

        private static readonly Regex NameRegex = new(@"^[\p{L}\p{Nd}\s\-_]+$", RegexOptions.Compiled);
        private static readonly Regex DigitsOnly = new(@"^\d+$", RegexOptions.Compiled);

        // CREATE
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

        // UPDATE
        public OrganisationEditPage(OrganisationVM vm) : this()
        {
            _editing = vm;

            TitleText.Text = "Обновить организацию";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            AccountBox.Text = vm.AccountNumOrg;

            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = false; // управление удалением в списке
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^[\p{L}\p{Nd}\s\-_]+$");

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(NameBox);

        private void AccountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");

        private void AccountBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(AccountBox);

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

        private void AccountBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText) ?? "").Trim();
            if (text.Length == 0 || !DigitsOnly.IsMatch(text))
                e.CancelCommand();
        }

        private bool ValidateForm()
        {
            var name = (NameBox.Text ?? "").Trim();
            name = Regex.Replace(name, @"\s+", " ");

            var account = (AccountBox.Text ?? "").Trim();

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

            if (string.IsNullOrWhiteSpace(account))
            {
                FieldValidation.SetError(AccountBox, "Номер счёта не должен быть пустым");
                return false;
            }
            if (!DigitsOnly.IsMatch(account))
            {
                FieldValidation.SetError(AccountBox, "Номер счёта должен содержать только цифры");
                return false;
            }
            FieldValidation.ClearError(AccountBox);

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
            NameBox.IsEnabled = !isBusy;
            AccountBox.IsEnabled = !isBusy;
            IsDeletedCheck.IsEnabled = _editing != null && !isBusy;
            SaveBtn.IsEnabled = !isBusy;
        }
    }
}
