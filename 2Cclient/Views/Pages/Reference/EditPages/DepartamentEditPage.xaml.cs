using System;
using System.Windows;
using System.Windows.Controls;
using Contracts.ViewModels;
using Contracts.BindingModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;

namespace _2Cclient.Views.Pages
{
    public partial class DepartamentEditPage : Page
    {
        private readonly DepartamentApi _api;

        // если null → Create, если не null → Update
        private readonly DepartamentVM? _editing;

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
        }

        // UPDATE
        public DepartamentEditPage(DepartamentVM vm)
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<DepartamentApi>();
            _editing = vm;

            TitleText.Text = "Обновить подразделение";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            IsDeletedCheck.IsChecked = vm.IsDeleted;
            IsDeletedCheck.IsEnabled = true;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название не должно быть пустым");
                return;
            }

            SetBusy(true);

            try
            {
                if (_editing is null)
                {
                    // CREATE
                    var model = new DepartamentBM
                    {
                        Name = name
                        // Id = null → сервер сам создаст
                        // IsDeleted по умолчанию false
                    };

                    await _api.CreateAsync(model);
                }
                else
                {
                    // UPDATE
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
        }
    }
}
