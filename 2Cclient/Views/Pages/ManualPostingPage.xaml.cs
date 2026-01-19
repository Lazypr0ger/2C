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

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class ManualPostingPage : Page
    {
        private readonly OperationApi _opApi;
        private readonly DepartamentApi _depApi;
        private readonly OperationVM? _editing;

        // CREATE
        public ManualPostingPage()
        {
            InitializeComponent();

            _opApi = App.Services.GetRequiredService<OperationApi>();
            _depApi = App.Services.GetRequiredService<DepartamentApi>();
            _editing = null;

            Loaded += async (_, __) => await InitAsync();
        }

        // OPEN/EDIT  ✅ вот это и убирает твою ошибку компиляции
        public ManualPostingPage(OperationVM vm)
        {
            InitializeComponent();

            _opApi = App.Services.GetRequiredService<OperationApi>();
            _depApi = App.Services.GetRequiredService<DepartamentApi>();
            _editing = vm;

            Loaded += async (_, __) => await InitAsync();
        }

        private async Task InitAsync()
        {
            try
            {
                var deps = await _depApi.GetAllAsync();
                DepartamentBox.ItemsSource = deps ?? new List<DepartamentVM>();
                DepartamentBox.DisplayMemberPath = "Name";
                DepartamentBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки подразделений:\n{ex.Message}");
            }

            if (_editing != null)
            {
                TitleText.Text = "Ручная проводка (редактирование)";
                SubtitleText.Text = "Измените параметры и нажмите «Провести»";

                NameDocumentBox.Text = _editing.NameDocument;
                DateOperationPicker.SelectedDate = _editing.DateOperation.ToLocalTime().Date;
                DepartamentBox.SelectedValue = _editing.DepartamentId;
                AmountBox.Text = _editing.TotalAmountDocument.ToString("0.##", CultureInfo.InvariantCulture);
            }
            else
            {
                TitleText.Text = "Ручная проводка (Дт20 Кт10)";
                SubtitleText.Text = "Введите параметры и нажмите «Провести»";

                NameDocumentBox.Text = "Фактические затраты";
                DateOperationPicker.SelectedDate = DateTime.Today;
                AmountBox.Text = string.Empty;
            }
        }

        private async void Post_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameDocumentBox.Text))
            {
                MessageBox.Show("Название документа не должно быть пустым");
                return;
            }

            if (DateOperationPicker.SelectedDate is null)
            {
                MessageBox.Show("Укажите дату операции");
                return;
            }

            if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
            {
                MessageBox.Show("Выберите подразделение (аналитика сч.20)");
                return;
            }

            var raw = (AmountBox.Text ?? string.Empty).Trim().Replace(',', '.');
            if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) || amount <= 0m)
            {
                MessageBox.Show("Сумма должна быть числом > 0");
                return;
            }

            SetBusy(true);

            try
            {
                var bm = new OperationBM
                {
                    Id = _editing?.Id, // null -> create
                    NameDocument = NameDocumentBox.Text.Trim(),
                    DateOperation = ToUtc(DateOperationPicker.SelectedDate.Value),
                    Type = OperationType.ActualCosts,          // 1
                    DepartamentId = depId,                     // аналитика 20
                    OrganisationId = null,
                    TotalAmountDocument = amount,              // важно для твоего сервера (иначе "TotalAmountDocument is empty")
                    Elements = new List<ElementBM>(),          // для 1 операции не нужны
                    IsDeleted = _editing?.IsDeleted ?? false
                };

                if (_editing is null)
                    await _opApi.CreateAsync(bm);
                else
                    await _opApi.UpdateAsync(bm);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проведения:\n{ex.Message}");
                SetBusy(false);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();

        private void SetBusy(bool busy)
        {
            PostBtn.IsEnabled = !busy;
            NameDocumentBox.IsEnabled = !busy;
            DateOperationPicker.IsEnabled = !busy;
            DepartamentBox.IsEnabled = !busy;
            AmountBox.IsEnabled = !busy;
        }

        private static DateTime ToUtc(DateTime localDate)
        {
            var local = DateTime.SpecifyKind(localDate.Date, DateTimeKind.Local);
            return local.ToUniversalTime();
        }
    }
}
