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
            };
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

            DepartamentBox.SelectedValue = op.DepartamentId;

            AmountBox.Text = op.TotalAmountDocument.ToString(CultureInfo.InvariantCulture);
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

        private static bool TryParseDecimal(string? text, out decimal value)
        {
            return decimal.TryParse((text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveBtn.IsEnabled = false;

                var name = (NameDocumentBox.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Заполните «Название документа».");
                    return;
                }

                if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
                {
                    MessageBox.Show("Выберите подразделение.");
                    return;
                }

                if (!TryParseDecimal(AmountBox.Text, out var amount) || amount <= 0m)
                {
                    MessageBox.Show("Сумма должна быть числом > 0.");
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
                    Type = OperationType.ActualCosts,
                    Comment = (CommentBox.Text ?? "").Trim(),
                    DepartamentId = depId,
                    OrganisationId = null,
                    Elements = new List<ElementBM>()
                };

                // Важно: для ActualCosts у тебя в BL проверка TotalAmountDocument > 0.
                // В BM этого поля нет → сервер должен брать сумму из dto.TotalAmountDocument.
                // Поэтому на контроллере/маппинге должно попасть в dto.TotalAmountDocument.
                // Если у тебя BM->Dto не кладёт сумму — добавь в BM поле TotalAmountDocument.
                // Но ты писал, что “дописал поле и работает”, значит BM уже расширен.
                // Если поле есть — раскомментируй:
                // bm.TotalAmountDocument = amount;

                // Если у тебя BM без суммы — временный костыль: в комментарий нельзя.
                // Поэтому считаем что поле есть:
                bm.GetType().GetProperty("TotalAmountDocument")?.SetValue(bm, amount);

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
