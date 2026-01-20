using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public partial class ManualPostingPage : Page
    {
        private readonly ApiClient _api;
        private OperationVM? _editing;

        public ManualPostingPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ApiClient>();

            Loaded += async (_, __) =>
            {
                await LoadDepartamentsAsync();
                InitDefaultsForCreate();

                // Снимаем возможные ошибки при первом открытии
                FieldValidation.ClearError(NameDocumentBox);
                FieldValidation.ClearError(TimeBox);
                FieldValidation.ClearError(AmountBox);
                FieldValidation.ClearError(DepartamentBox);
            };

            // paste restrictions
            DataObject.AddPastingHandler(AmountBox, AmountBox_OnPaste);
            DataObject.AddPastingHandler(TimeBox, TimeBox_OnPaste);
            DataObject.AddPastingHandler(NameDocumentBox, NameDocumentBox_OnPaste);
        }

        public ManualPostingPage(OperationVM op) : this()
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
            AmountBox.Text = "";
        }

        private async Task LoadDepartamentsAsync()
        {
            var deps = await _api.GetAsync<List<DepartamentVM>>("/ms/api/Departament");
            DepartamentBox.ItemsSource = deps ?? new List<DepartamentVM>();
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

            // важно: SelectedValuePath="Id", SelectedValue = string Id
            DepartamentBox.SelectedValue = op.DepartamentId;

            AmountBox.Text = op.TotalAmountDocument.ToString(CultureInfo.InvariantCulture);

            FieldValidation.ClearError(NameDocumentBox);
            FieldValidation.ClearError(TimeBox);
            FieldValidation.ClearError(AmountBox);
            FieldValidation.ClearError(DepartamentBox);
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

        // -------------------- Amount --------------------
        private void AmountBox_PreviewKeyDown(object sender, KeyEventArgs e)
            => FieldValidation.Amount_PreviewKeyDown(sender, e);

        private void AmountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => FieldValidation.Amount_PreviewTextInput(sender, e);

        private void AmountBox_TextChanged(object sender, TextChangedEventArgs e)
            => FieldValidation.ClearErrorOnTyping(AmountBox);

        private void AmountBox_LostFocus(object sender, RoutedEventArgs e)
            => FieldValidation.Amount_LostFocus(AmountBox);

        private void AmountBox_OnPaste(object sender, DataObjectPastingEventArgs e)
            => FieldValidation.Amount_OnPaste(sender, e);

        // -------------------- Date + Time -> UTC --------------------
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
            if (string.IsNullOrWhiteSpace(timeText))
                timeText = "00:00";

            if (!FieldValidation.TimeStrictRegex.IsMatch(timeText))
            {
                FieldValidation.SetError(TimeBox, "Время в формате HH:mm (например 12:00)");
                error = "Время должно быть в формате HH:mm.";
                return false;
            }

            var ts = TimeSpan.ParseExact(timeText, "hh\\:mm", CultureInfo.InvariantCulture);

            var local = DatePicker.SelectedDate.Value.Date.Add(ts);
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);
            utc = local.ToUniversalTime();
            return true;
        }

        // -------------------- Save --------------------
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveBtn.IsEnabled = false;

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

                // 2) Departament
                if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
                {
                    FieldValidation.SetError(DepartamentBox, "Выберите подразделение");
                    return;
                }

                FieldValidation.ClearError(DepartamentBox);

                // 3) Amount
                if (!FieldValidation.TryParseDecimalStrict(AmountBox.Text, out var amount) || amount <= 0m)
                {
                    FieldValidation.SetError(AmountBox, "Сумма должна быть числом > 0");
                    return;
                }

                FieldValidation.ClearError(AmountBox);
                AmountBox.Text = amount.ToString("0.##", CultureInfo.InvariantCulture);

                // 4) DateTime
                if (!TryGetUtcDateTime(out var utc, out _))
                    return;

                // 5) create bm
                var bm = new OperationBM
                {
                    Id = _editing?.Id,
                    NameDocument = name,
                    DateOperation = utc,
                    Type = OperationType.ActualCosts,
                    Comment = (CommentBox.Text ?? "").Trim(),
                    DepartamentId = depId,
                    OrganisationId = null,
                    TotalAmountDocument = amount,
                    Elements = new List<ElementBM>()
                };

                if (_editing == null)
                    await _api.PostAsync("/ms/api/Operation", bm);
                else
                    await _api.PutAsync("/ms/api/Operation", bm);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}");
            }
            finally
            {
                SaveBtn.IsEnabled = true;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true) NavigationService.GoBack();
        }
    }
}
