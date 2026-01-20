using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _2Cclient.Services.Api;
using _2Cclient.UI;
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
                InitDefaultsForCreate();

                FieldValidation.ClearError(NameDocumentBox);
                FieldValidation.ClearError(TimeBox);
            };

            // paste restrictions
            DataObject.AddPastingHandler(TimeBox, TimeBox_OnPaste);
            DataObject.AddPastingHandler(NameDocumentBox, NameDocumentBox_OnPaste);
        }

        public CostDistributionOperationEditPage(OperationVM op) : this()
        {
            _editing = op;
            Loaded += (_, __) => FillFromOperation(op);
        }

        private void InitDefaultsForCreate()
        {
            if (_editing != null) return;

            DatePicker.SelectedDate = DateTime.Now.Date;
            TimeBox.Text = DateTime.Now.ToString("HH:mm");
            CommentBox.Text = "";
            NameDocumentBox.Text = "";
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

            FieldValidation.ClearError(NameDocumentBox);
            FieldValidation.ClearError(TimeBox);
        }

        // -------------------- Name --------------------
        private void NameDocumentBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Name_PreviewTextInput(sender, e);

        private void NameDocumentBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.Name_TextChanged(NameDocumentBox);

        private void NameDocumentBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Name_OnPaste(sender, e);

        // -------------------- Time --------------------
        private void TimeBox_PreviewKeyDown(object sender, KeyEventArgs e)
            => FieldValidation.Time_PreviewKeyDown(sender, e);

        private void TimeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Time_PreviewTextInput(sender, e);

        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.Time_TextChanged(TimeBox);

        private void TimeBox_LostFocus(object sender, RoutedEventArgs e)
            => FieldValidation.Time_LostFocus(TimeBox);

        private void TimeBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Time_OnPasteDigitsOnly(sender, e);

        // -------------------- Date + Time -> UTC --------------------
        private bool TryGetUtcDateTime(out DateTime utc)
        {
            utc = default;

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Дата не выбрана.");
                return false;
            }

            var timeText = (TimeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(timeText))
                timeText = "00:00";

            if (!FieldValidation.TimeStrictRegex.IsMatch(timeText))
            {
                FieldValidation.SetError(TimeBox, "Время в формате HH:mm (например 12:00)");
                return false;
            }

            var ts = TimeSpan.ParseExact(timeText, "hh\\:mm", CultureInfo.InvariantCulture);
            var local = DatePicker.SelectedDate.Value.Date.Add(ts);
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);

            utc = local.ToUniversalTime();
            return true;
        }

        // -------------------- Apply --------------------
        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyBtn.IsEnabled = false;

                // 1) Name
                var name = (NameDocumentBox.Text ?? "").Trim();
                name = Regex.Replace(name, @"\s+", " ");

                if (string.IsNullOrWhiteSpace(name))
                {
                    FieldValidation.SetError(NameDocumentBox, "Название документа обязательно");
                    return;
                }

                if (!FieldValidation.NameAllowedRegex.IsMatch(name))
                {
                    FieldValidation.SetError(NameDocumentBox, "Только русские/английские буквы и пробел");
                    return;
                }

                FieldValidation.ClearError(NameDocumentBox);

                // 2) DateTime
                if (!TryGetUtcDateTime(out var utc))
                    return;

                // 3) BM
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
