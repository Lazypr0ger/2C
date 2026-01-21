using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Contracts.ViewModels;
using Contracts.BindingModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;
using _2Cclient.UI;

namespace _2Cclient.Views.Pages
{
    public partial class DepartamentEditPage : Page
    {
        private readonly DepartamentApi _api;
        private readonly DepartamentVM? _editing;

        // Разрешим буквы/цифры/пробел/дефис/подчёркивание
        private static readonly Regex NameRegex = new(@"^[\p{L}\p{Nd}\s\-_]+$", RegexOptions.Compiled);

        // CREATE
        public DepartamentEditPage()
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<DepartamentApi>();
            _editing = null;

            TitleText.Text = "Добавить подразделение";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";

            IsDeletedCheck.IsChecked = false;
            IsDeletedCheck.IsEnabled = false;

            Loaded += (_, __) =>
            {
                FieldValidation.ClearError(NameBox);
                NameBox.Focus();
            };

            DataObject.AddPastingHandler(NameBox, NameBox_OnPaste);
        }

        // UPDATE
        public DepartamentEditPage(DepartamentVM vm) : this()
        {
            _editing = vm;

            TitleText.Text = "Обновить подразделение";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = true;
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // запрещаем только "плохие" символы, пробелы/буквы/цифры оставляем
            e.Handled = !Regex.IsMatch(e.Text, @"^[\p{L}\p{Nd}\s\-_]+$");
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // как только начали печатать — убираем красное
            FieldValidation.ClearErrorOnTyping(NameBox);
        }

        private void NameBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = (e.DataObject.GetData(DataFormats.UnicodeText) as string) ?? "";
            text = text.Trim();

            if (text.Length == 0 || !NameRegex.IsMatch(text))
                e.CancelCommand();
        }

        private bool ValidateForm()
        {
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

                if (_editing is null)
                {
                    var model = new DepartamentBM { Name = name };
                    await _api.CreateAsync(model);
                }
                else
                {
                    var model = new DepartamentBM
                    {
                        Id = _editing.Id,
                        Name = name,
                        IsDeleted = IsDeletedCheck.IsChecked == true
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
            IsDeletedCheck.IsEnabled = _editing != null && !isBusy;
            SaveBtn.IsEnabled = !isBusy;
        }
    }
}
