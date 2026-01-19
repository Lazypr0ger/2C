using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class WriteOffDeviationOperationEditPage : Page
    {
        private readonly OperationApi _opApi;
        private readonly OperationVM? _editing;

        public WriteOffDeviationOperationEditPage()
        {
            InitializeComponent();
            _opApi = App.Services.GetRequiredService<OperationApi>();
            _editing = null;

            Loaded += (_, __) => Init();
        }

        public WriteOffDeviationOperationEditPage(OperationVM vm)
        {
            InitializeComponent();
            _opApi = App.Services.GetRequiredService<OperationApi>();
            _editing = vm;

            Loaded += (_, __) => Init();
        }

        private void Init()
        {
            if (_editing != null)
            {
                NameDocumentBox.Text = _editing.NameDocument;
                DateOperationPicker.SelectedDate = _editing.DateOperation.ToLocalTime().Date;
            }
            else
            {
                NameDocumentBox.Text = "Списание отклонений";
                DateOperationPicker.SelectedDate = DateTime.Today;
            }
        }

        private async void Execute_Click(object sender, RoutedEventArgs e)
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
                    Type = OperationType.WriteOffDeviations,
                    OrganisationId = null,
                    DepartamentId = null,
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
                MessageBox.Show($"Ошибка выполнения:\n{ex.Message}");
                SetBusy(false);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();

        private void SetBusy(bool busy)
        {
            ExecuteBtn.IsEnabled = !busy;
            NameDocumentBox.IsEnabled = !busy;
            DateOperationPicker.IsEnabled = !busy;
        }

        private static DateTime ToUtc(DateTime localDate)
        {
            var local = DateTime.SpecifyKind(localDate.Date, DateTimeKind.Local);
            return local.ToUniversalTime();
        }
    }
}
