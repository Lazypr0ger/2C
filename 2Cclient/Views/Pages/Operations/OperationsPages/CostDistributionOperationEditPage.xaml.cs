using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _2Cclient.Views.Pages.Operations.OperationsPages
{
    public partial class CostDistributionOperationEditPage : Page
    {
        private readonly ApiClient _api;
        private OperationVM? _editing;

        public CostDistributionOperationEditPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ApiClient>();

            Loaded += (_, __) =>
            {
                if (_editing == null)
                {
                    DatePicker.SelectedDate = DateTime.Now.Date;
                    TimeBox.Text = DateTime.Now.ToString("HH:mm");
                    CommentBox.Text = "";
                }
            };
        }

        public CostDistributionOperationEditPage(OperationVM op) : this()
        {
            _editing = op;
            Loaded += (_, __) => FillFromOperation(op);
        }

        private void FillFromOperation(OperationVM op)
        {
            NameDocumentBox.Text = op.NameDocument ?? "";
            CommentBox.Text = op.Comment ?? "";

            var dtUtc = op.DateOperation.Kind == DateTimeKind.Utc
                ? op.DateOperation
                : DateTime.SpecifyKind(op.DateOperation, DateTimeKind.Utc);

            var local = dtUtc.ToLocalTime();
            DatePicker.SelectedDate = local.Date;
            TimeBox.Text = local.ToString("HH:mm");
        }

        private bool TryGetUtcDateTime(out DateTime utc, out string error)
        {
            utc = default;
            error = "";

            if (DatePicker.SelectedDate == null)
            {
                error = "Дата не выбрана.";
                return false;
            }

            var timeText = (TimeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(timeText)) timeText = "00:00";

            if (!TimeSpan.TryParseExact(timeText, "hh\\:mm", CultureInfo.InvariantCulture, out var ts))
            {
                error = "Время должно быть в формате HH:mm.";
                return false;
            }

            var local = DatePicker.SelectedDate.Value.Date.Add(ts);
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);
            utc = local.ToUniversalTime();
            return true;
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyBtn.IsEnabled = false;

                var name = (NameDocumentBox.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Заполните «Название документа».");
                    return;
                }

                if (!TryGetUtcDateTime(out var utc, out var err))
                {
                    MessageBox.Show(err);
                    return;
                }

                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = name,
                    DateOperation = utc,
                    Type = OperationType.AllocateActualCost,
                    Comment = (CommentBox.Text ?? "").Trim(),
                    OrganisationId = null,
                    DepartamentId = null
                };

                if (_editing == null)
                    await _api.PostAsync("/ms/api/Operation", bm);
                else
                    await _api.PutAsync("/ms/api/Operation", bm);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения:\n{ex.Message}");
            }
            finally
            {
                ApplyBtn.IsEnabled = true;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true) NavigationService.GoBack();
        }
    }
}
