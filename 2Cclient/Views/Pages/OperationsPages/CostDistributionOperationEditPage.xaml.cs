using System;
using System.Collections.Generic;
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
    public partial class CostDistributionOperationEditPage : Page
    {
        private readonly OperationApi _opApi;
        private readonly DepartamentApi _depApi;
        private readonly OperationVM? _editing;

        public CostDistributionOperationEditPage()
        {
            InitializeComponent();
            _opApi = App.Services.GetRequiredService<OperationApi>();
            _depApi = App.Services.GetRequiredService<DepartamentApi>();
            _editing = null;

            Loaded += async (_, __) => await InitAsync();
        }

        public CostDistributionOperationEditPage(OperationVM vm)
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
                NameDocumentBox.Text = _editing.NameDocument;
                DateOperationPicker.SelectedDate = _editing.DateOperation.ToLocalTime().Date;
                DepartamentBox.SelectedValue = _editing.DepartamentId;

                // Период пока не используется сервером (если хочешь — можно заполнить)
                StartDatePicker.SelectedDate = null;
                EndDatePicker.SelectedDate = null;
            }
            else
            {
                NameDocumentBox.Text = "Распределение фактической себестоимости";
                DateOperationPicker.SelectedDate = DateTime.Today;
            }
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
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

            SetBusy(true);

            try
            {
                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = NameDocumentBox.Text.Trim(),
                    DateOperation = ToUtc(DateOperationPicker.SelectedDate.Value),
                    Type = OperationType.AllocateActualCost,
                    DepartamentId = DepartamentBox.SelectedValue as string,
                    OrganisationId = null,
                    TotalAmountDocument = 0m,
                    Elements = new List<ElementBM>(),
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
            ApplyBtn.IsEnabled = !busy;
            NameDocumentBox.IsEnabled = !busy;
            DateOperationPicker.IsEnabled = !busy;
            DepartamentBox.IsEnabled = !busy;
        }

        private static DateTime ToUtc(DateTime localDate)
        {
            var local = DateTime.SpecifyKind(localDate.Date, DateTimeKind.Local);
            return local.ToUniversalTime();
        }
    }
}
