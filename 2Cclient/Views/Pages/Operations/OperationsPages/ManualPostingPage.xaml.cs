using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        // Сумма: цифры + один '.' или ',' (без минуса)
        private static readonly Regex AmountAllowedRegex = new(@"^[0-9]*([.,][0-9]*)?$", RegexOptions.Compiled);

        // Время строго HH:mm
        private static readonly Regex TimeStrictRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

        public ManualPostingPage()
        {
            InitializeComponent();
            _api = App.Services.GetRequiredService<ApiClient>();

            Loaded += async (_, __) =>
            {
                await LoadDepartamentsAsync();
                InitDefaultsForCreate();

                // нейтральная подсветка
                MarkValid(NameDocumentBox, true);
                MarkValid(TimeBox, true);
                MarkValid(AmountBox, true);
                MarkValid(DepartamentBox, true);
                MarkValid(DatePicker, true);
            };

            // Запрещаем вставку мусора
            DataObject.AddPastingHandler(AmountBox, AmountBox_OnPaste);
            DataObject.AddPastingHandler(TimeBox, TimeBox_OnPaste);
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

            DepartamentBox.SelectedValue = op.DepartamentId;

            AmountBox.Text = op.TotalAmountDocument.ToString(CultureInfo.InvariantCulture);
            MarkValid(AmountBox, true);
            MarkValid(TimeBox, true);
        }

        // -------------------- UI подсветка валидности (без MessageBox) --------------------
        private void MarkValid(Control control, bool ok, string? tooltip = null)
        {
            // Если хочешь — заведи отдельные ресурсы (FieldErrorBrush / FieldOkBrush)
            // Сейчас используем Danger/BorderBrush из темы.
            var errorBrush = (Brush)FindResource("Danger");
            var okBrush = (Brush)FindResource("Accent");
            var neutralBrush = (Brush)FindResource("Border");

            control.BorderBrush = ok ? okBrush : errorBrush;
            control.ToolTip = ok ? null : (tooltip ?? "Некорректное значение");

            // если пусто и ok=true, можно вернуть нейтральный бордер
            // (чтобы не было постоянно Accent)
            if (ok && string.IsNullOrWhiteSpace(GetControlText(control)))
                control.BorderBrush = neutralBrush;
        }

        private static string GetControlText(Control c) =>
            c switch
            {
                TextBox tb => tb.Text ?? "",
                ComboBox cb => cb.Text ?? "",
                DatePicker dp => dp.Text ?? "",
                _ => ""
            };

        // -------------------- Время --------------------
        private void TimeBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void TimeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // только цифры (двоеточие поставим сами)
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = TimeBox;
            if (tb == null) return;

            // Снимаем подсветку ошибки, пока пользователь печатает
            // (жёстко проверим при Save или LostFocus)
            if (!string.IsNullOrWhiteSpace(tb.Text))
                MarkValid(tb, true);

            var raw = new string((tb.Text ?? "").Where(char.IsDigit).ToArray());
            if (raw.Length > 4) raw = raw[..4];

            string formatted = raw.Length switch
            {
                0 => "",
                1 => raw,
                2 => raw, // "12"
                3 => raw[..2] + ":" + raw[2..], // "12:3"
                4 => raw[..2] + ":" + raw[2..], // "12:34"
                _ => raw
            };

            if (tb.Text != formatted)
            {
                var caret = tb.CaretIndex;
                tb.Text = formatted;
                tb.CaretIndex = Math.Min(tb.Text.Length, caret);
            }
        }

        private void TimeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var t = (TimeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(t))
            {
                TimeBox.Text = "00:00";
                MarkValid(TimeBox, true);
                return;
            }

            // ВАЖНО: никакого Focus() и MessageBox() тут — иначе ловим цикл
            if (!TimeStrictRegex.IsMatch(t))
            {
                MarkValid(TimeBox, false, "Время в формате HH:mm (например 12:00)");
                return;
            }

            MarkValid(TimeBox, true);
        }

        private void TimeBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = (e.DataObject.GetData(DataFormats.UnicodeText) as string) ?? "";
            // допускаем вставку только цифр
            if (text.Any(ch => !char.IsDigit(ch)))
                e.CancelCommand();
        }

        // -------------------- Сумма --------------------
        private void AmountBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void AmountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            var current = tb.Text ?? "";

            var selectionStart = tb.SelectionStart;
            var selectionLength = tb.SelectionLength;

            var next = current.Remove(selectionStart, selectionLength).Insert(selectionStart, e.Text);

            // Разрешаем только "12" / "12." / "12.3" / "12,3"
            e.Handled = !AmountAllowedRegex.IsMatch(next);
        }

        private void AmountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Пока печатает — не ругаемся, просто сбрасываем красное
            if (!string.IsNullOrWhiteSpace(AmountBox.Text))
                MarkValid(AmountBox, true);
        }

        private void AmountBox_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText)) ?? "";
            text = text.Trim();

            if (!AmountAllowedRegex.IsMatch(text))
                e.CancelCommand();
        }

        private void AmountBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = (AmountBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(txt))
            {
                MarkValid(AmountBox, false, "Введите сумму > 0");
                return;
            }

            txt = txt.Replace(',', '.');

            if (!decimal.TryParse(txt, NumberStyles.Number, CultureInfo.InvariantCulture, out var val) || val <= 0m)
            {
                // НЕ делаем Focus/SelectAll и НЕ MessageBox — только подсветка
                MarkValid(AmountBox, false, "Сумма должна быть числом > 0");
                return;
            }

            MarkValid(AmountBox, true);
            AmountBox.Text = val.ToString("0.##", CultureInfo.InvariantCulture);
        }

        // -------------------- Date + Time -> UTC --------------------
        private bool TryGetUtcDateTime(out DateTime utc, out string error)
        {
            utc = default;
            error = "";

            if (DatePicker.SelectedDate == null)
            {
                error = "Дата не выбрана.";
                MarkValid(DatePicker, false, error);
                return false;
            }

            MarkValid(DatePicker, true);

            var timeText = (TimeBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(timeText))
                timeText = "00:00";

            if (!TimeStrictRegex.IsMatch(timeText))
            {
                error = "Время должно быть в формате HH:mm (например 12:00).";
                MarkValid(TimeBox, false, error);
                return false;
            }

            MarkValid(TimeBox, true);

            var ts = TimeSpan.ParseExact(timeText, "hh\\:mm", CultureInfo.InvariantCulture);

            var local = DatePicker.SelectedDate.Value.Date.Add(ts);
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);
            utc = local.ToUniversalTime();
            return true;
        }

        private static bool TryParseDecimalStrict(string? text, out decimal value)
        {
            value = 0m;
            var t = (text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(t)) return false;

            t = t.Replace(',', '.');

            if (!AmountAllowedRegex.IsMatch(t)) return false;

            return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

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
                    MarkValid(NameDocumentBox, false, "Название документа обязательно");
                    MessageBox.Show("Заполните «Название документа».");
                    NameDocumentBox.Focus();
                    return;
                }
                MarkValid(NameDocumentBox, true);

                // 2) Departament
                if (DepartamentBox.SelectedValue is not string depId || string.IsNullOrWhiteSpace(depId))
                {
                    MarkValid(DepartamentBox, false, "Выберите подразделение");
                    MessageBox.Show("Выберите подразделение.");
                    DepartamentBox.Focus();
                    return;
                }
                MarkValid(DepartamentBox, true);

                // 3) Amount
                if (!TryParseDecimalStrict(AmountBox.Text, out var amount) || amount <= 0m)
                {
                    MarkValid(AmountBox, false, "Сумма должна быть > 0");
                    MessageBox.Show("Сумма должна быть числом > 0 (decimal).");
                    AmountBox.Focus();
                    AmountBox.SelectAll();
                    return;
                }
                MarkValid(AmountBox, true);
                AmountBox.Text = amount.ToString("0.##", CultureInfo.InvariantCulture);

                // 4) DateTime
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

                // Сумма: кладем в TotalAmountDocument
                var p = bm.GetType().GetProperty("TotalAmountDocument", BindingFlags.Public | BindingFlags.Instance);
                if (p == null || !p.CanWrite)
                {
                    MessageBox.Show("В OperationBM отсутствует поле TotalAmountDocument. Добавь его в BM, иначе сервер не получит сумму.");
                    return;
                }
                p.SetValue(bm, amount);

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
