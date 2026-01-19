using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages
{
    public partial class ManualPostingPage : Page
    {
        private readonly DepartamentApi _depApi;
        private readonly OperationApi _opApi;

        public ManualPostingPage()
        {
            InitializeComponent();
            _depApi = App.Services.GetRequiredService<DepartamentApi>();
            _opApi = App.Services.GetRequiredService<OperationApi>();

            Loaded += async (_, __) => await InitAsync();
        }

        private async Task InitAsync()
        {
            try
            {
                DateBox.SelectedDate = DateTime.Today;

                var deps = await _depApi.GetAllAsync();
                DepartamentBox.ItemsSource = deps ?? new List<DepartamentVM>();
                DepartamentBox.SelectedIndex = (DepartamentBox.Items.Count > 0) ? 0 : -1;

                CommentBox.Text = "Накопление фактических затрат Дт20 Кт10";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки подразделений:\n{ex.Message}");
            }
        }

        // DatePicker → UTC
        private static DateTime ToUtc(DateTime localDate)
        {
            var local = DateTime.SpecifyKind(localDate, DateTimeKind.Local);
            return local.ToUniversalTime();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateBox.SelectedDate is null)
                {
                    MessageBox.Show("Укажите дату");
                    return;
                }

                if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
                {
                    MessageBox.Show("Выберите подразделение");
                    return;
                }

                var amountText = (AmountBox.Text ?? "").Trim().Replace(',', '.');
                if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) || amount <= 0m)
                {
                    MessageBox.Show("Введите корректную сумму > 0");
                    return;
                }

                var comment = (CommentBox.Text ?? "").Trim();

                // Важно: для OperationType.ActualCosts у тебя TotalAmountDocument обязателен
                var model = new OperationBM
                {
                    NameDocument = "Ручная проводка (Дт20 Кт10)",
                    DateOperation = ToUtc(DateBox.SelectedDate.Value),
                    Type = OperationType.ActualCosts,
                    TotalAmountDocument = amount,
                    DepartamentId = depId,
                    OrganisationId = null,
                    Elements = new List<ElementBM>() // для этой операции строки не нужны
                };

                await _opApi.CreateAsync(model);

                // Возвращаемся назад
                if (NavigationService?.CanGoBack == true)
                    NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проведения:\n{ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
