using System;
using System.Windows;
using System.Windows.Controls;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;

namespace _2Cclient.Views.Pages
{
    public partial class OrganisationEditPage : Page
    {
        private readonly OrganisationApi _api;

        private readonly bool _isEdit;
        private readonly OrganisationVM? _editing;

        public OrganisationEditPage()
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<OrganisationApi>();

            _isEdit = false;
            _editing = null;

            TitleText.Text = "Добавить организацию";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";

            IsDeletedCheck.IsChecked = false;
            IsDeletedCheck.IsEnabled = false;
        }

        public OrganisationEditPage(OrganisationVM vm)
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<OrganisationApi>();

            _isEdit = true;
            _editing = vm;

            TitleText.Text = "Обновить организацию";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            AccountBox.Text = vm.AccountNumOrg;
            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = true;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            var acc = AccountBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название не должно быть пустым");
                return;
            }

            if (string.IsNullOrWhiteSpace(acc))
            {
                MessageBox.Show("Счёт организации не должен быть пустым");
                return;
            }

            SetBusy(true);

            try
            {
                if (_isEdit)
                {
                    if (_editing is null)
                        throw new InvalidOperationException("Edit mode: editing VM is null");

                    var updated = new OrganisationVM
                    {
                        Id = _editing.Id,
                        Name = name,
                        AccountNumOrg = acc,
                        IsDeleted = IsDeletedCheck.IsChecked == true
                    };

                    await _api.UpdateAsync(updated);
                }
                else
                {
                    // CREATE: без Id
                    await _api.CreateAsync(name, acc);
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
            IsDeletedCheck.IsEnabled = _isEdit && !isBusy;
        }
    }
}
