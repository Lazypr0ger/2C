using System;
using System.Windows;
using System.Windows.Controls;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using _2Cclient.Services.Api;

namespace _2Cclient.Views.Pages
{
    public partial class DepartamentEditPage : Page
    {
        private readonly DepartamentApi _api;

        private readonly bool _isEdit;
        private readonly DepartamentVM? _editing;

        // CREATE
        public DepartamentEditPage()
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<DepartamentApi>();

            _isEdit = false;
            TitleText.Text = "Добавить подразделение";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";

            IsDeletedCheck.IsChecked = false;
            IsDeletedCheck.IsEnabled = false; // при создании не даём удалять сразу
        }

        // EDIT
        public DepartamentEditPage(DepartamentVM vm)
        {
            InitializeComponent();

            _api = App.Services.GetRequiredService<DepartamentApi>();

            _isEdit = true;
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
                if (_isEdit)
                {
                    if (_editing is null)
                        throw new InvalidOperationException("Edit mode: editing VM is null");

                    var updated = new DepartamentVM
                    {
                        Id = _editing.Id,
                        Name = name,
                        IsDeleted = IsDeletedCheck.IsChecked == true
                    };

                    await _api.UpdateAsync(updated);
                }
                else
                {
                    // Id required => генерируем Guid
                    var created = new DepartamentVM
                    {
                        
                        Name = name,
                        IsDeleted = false
                    };

                    await _api.CreateAsync(name);
                    
                }

                // успех -> назад
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

            // чекбокс включаем только в режиме редактирования
            IsDeletedCheck.IsEnabled = _isEdit && !isBusy;
        }
    }
}
