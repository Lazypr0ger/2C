using System;
using System.Windows;
using System.Windows.Controls;
using Contracts.BindingModels;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;

namespace _2Cclient.Views.Pages
{
    public partial class OrganisationEditPage : Page
    {
        private readonly OrganisationApi _api;

        // null -> Create, not null -> Update
        private readonly OrganisationVM? _editing;

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
        }

        // UPDATE
        public OrganisationEditPage(OrganisationVM vm)
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<OrganisationApi>();
            _editing = vm;

            TitleText.Text = "Обновить организацию";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            AccountBox.Text = vm.AccountNumOrg;

            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = false; // удаление/восстановление отдельными кнопками в списке
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            var account = AccountBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название не должно быть пустым");
                return;
            }

            if (string.IsNullOrWhiteSpace(account))
            {
                MessageBox.Show("Номер счёта не должен быть пустым");
                return;
            }

            SetBusy(true);

            try
            {
                if (_editing is null)
                {
                    // CREATE
                    var model = new OrganisationBM
                    {
                        Name = name,
                        AccountNumOrg = account
                        // Id = null -> генерит сервер
                        // IsDeleted -> сервер выставит false
                    };

                    await _api.CreateAsync(model);
                }
                else
                {
                    // UPDATE (IsDeleted не трогаем — отдельные трассы)
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
            IsDeletedCheck.IsEnabled = _editing != null && !isBusy;
        }
    }
}
