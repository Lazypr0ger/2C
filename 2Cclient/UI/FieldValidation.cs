using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace _2Cclient.UI
{
    public static class FieldValidation
    {
        // ===== Limits =====
        public const int MaxNameLen = 20;      
        public const int MaxCommentLen = 20;    
        public const int OrgAccountLen = 20;    
        public const int MaxFutureDays = 7;     

        // ===== Regex =====
        public static readonly Regex AmountAllowedRegex = new(@"^[0-9]*([.,][0-9]*)?$", RegexOptions.Compiled);
        public static readonly Regex TimeStrictRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

        // ТОЛЬКО буквы/пробел (для имён)
        public static readonly Regex NameAllowedRegex = new(@"^[A-Za-zА-Яа-яЁё\s]*$", RegexOptions.Compiled);

        // ТОЛЬКО цифры
        public static readonly Regex DigitsOnlyRegex = new(@"^\d*$", RegexOptions.Compiled);

        // ===== Validation (без Binding) =====
        public static void SetError(Control control, string message)
        {
            var dummy = new Binding("Tag") { Source = control };
            BindingOperations.SetBinding(control, FrameworkElement.TagProperty, dummy);

            var be = BindingOperations.GetBindingExpression(control, FrameworkElement.TagProperty);
            if (be == null) return;

            Validation.MarkInvalid(be, new ValidationError(new ExceptionValidationRule(), be, message, null));
        }

        public static void ClearError(Control control)
        {
            var be = BindingOperations.GetBindingExpression(control, FrameworkElement.TagProperty);
            if (be != null) Validation.ClearInvalid(be);
        }

        // =====================================================================
        // ===== Universal helpers for text length ограничения =====
        // =====================================================================

        /// <summary>Универсальный обработчик PreviewTextInput: режем ввод по MaxLen.</summary>
        public static void EnforceMaxLen_PreviewTextInput(object sender, TextCompositionEventArgs e, int maxLen)
        {
            if (sender is not TextBox tb) return;

            var current = tb.Text ?? "";
            var selectionStart = tb.SelectionStart;
            var selectionLength = tb.SelectionLength;

            // какой текст получится после ввода
            var next = current.Remove(selectionStart, selectionLength).Insert(selectionStart, e.Text);

            if (next.Length > maxLen)
            {
                e.Handled = true;
                SetError(tb, $"Максимум {maxLen} символов");
            }
            else
            {
                // пока печатает — не держим ошибку длины
                if (!string.IsNullOrWhiteSpace(tb.Text)) ClearError(tb);
            }
        }

        public static void EnforceMaxLen_OnPaste(object sender, DataObjectPastingEventArgs e, int maxLen)
        {
            if (sender is not TextBox tb) return;

            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var paste = (e.DataObject.GetData(DataFormats.UnicodeText) as string) ?? "";
            paste = paste.Trim();

            var current = tb.Text ?? "";
            var selectionStart = tb.SelectionStart;
            var selectionLength = tb.SelectionLength;

            var next = current.Remove(selectionStart, selectionLength).Insert(selectionStart, paste);

            if (next.Length > maxLen)
            {
                e.CancelCommand();
                SetError(tb, $"Максимум {maxLen} символов");
            }
        }

        public static void EnforceMaxLen_TextChanged(TextBox tb, int maxLen, string fieldTitle)
        {
            var t = tb.Text ?? "";
            if (t.Length > maxLen)
            {
                tb.Text = t.Substring(0, maxLen);
                tb.CaretIndex = tb.Text.Length;
                SetError(tb, $"{fieldTitle}: максимум {maxLen} символов");
                return;
            }

            // если пусто — не ругаемся сразу (как и было)
            if (string.IsNullOrWhiteSpace(t))
            {
                ClearError(tb);
                return;
            }

            ClearError(tb);
        }

        // =====================================================================
        // ===== Name (Документы / Организации / Подразделения / Продукты) =====
        // =====================================================================

        // Разрешаем: буквы+пробел, длина <= 20
        public static void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!NameAllowedRegex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            EnforceMaxLen_PreviewTextInput(sender, e, MaxNameLen);
        }

        public static void Name_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox tb) return;

            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = (e.DataObject.GetData(DataFormats.UnicodeText) as string) ?? "";
            text = text.Trim();

            if (!NameAllowedRegex.IsMatch(text))
            {
                e.CancelCommand();
                SetError(tb, "Только русские/английские буквы и пробел");
                return;
            }

            EnforceMaxLen_OnPaste(sender, e, MaxNameLen);
        }

        public static void Name_TextChanged(TextBox tb, string fieldTitle = "Название")
        {
            var t = tb.Text ?? "";

            if (!NameAllowedRegex.IsMatch(t))
            {
                SetError(tb, "Только русские/английские буквы и пробел");
                return;
            }

            EnforceMaxLen_TextChanged(tb, MaxNameLen, fieldTitle);
        }

        // =====================================================================
        // ===== Comment (до 20, без ограничения по символам) =====
        // =====================================================================

        public static void Comment_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => EnforceMaxLen_PreviewTextInput(sender, e, MaxCommentLen);

        public static void Comment_OnPaste(object sender, DataObjectPastingEventArgs e)
            => EnforceMaxLen_OnPaste(sender, e, MaxCommentLen);

        public static void Comment_TextChanged(TextBox tb)
            => EnforceMaxLen_TextChanged(tb, MaxCommentLen, "Комментарий");

        // =====================================================================
        // ===== Organisation Account (20 digits) =====
        // =====================================================================

        public static void OrgAccount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!DigitsOnlyRegex.IsMatch(e.Text))
            {
                e.Handled = true;
                if (sender is Control c) SetError(c, "Только цифры");
                return;
            }

            EnforceMaxLen_PreviewTextInput(sender, e, OrgAccountLen);
        }

        public static void OrgAccount_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox tb) return;

            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText)) ?? "";
            text = text.Trim();

            if (!Regex.IsMatch(text, @"^\d*$"))
            {
                e.CancelCommand();
                SetError(tb, "Только цифры");
                return;
            }

            EnforceMaxLen_OnPaste(sender, e, OrgAccountLen);
        }

        /// <summary>
        /// Валидация по факту (например на LostFocus): строго 20 цифр.
        /// </summary>
        public static void OrgAccount_LostFocus(TextBox tb)
        {
            var t = (tb.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(t))
            {
                SetError(tb, "Введите счёт организации (20 цифр)");
                return;
            }

            if (!Regex.IsMatch(t, @"^\d+$"))
            {
                SetError(tb, "Счёт должен содержать только цифры");
                return;
            }

            if (t.Length != OrgAccountLen)
            {
                SetError(tb, $"Счёт должен быть длиной ровно {OrgAccountLen} цифр");
                return;
            }

            ClearError(tb);
        }

        // =====================================================================
        // ===== Amount =====
        // =====================================================================

        public static void Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        public static void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            var current = tb.Text ?? "";
            var selectionStart = tb.SelectionStart;
            var selectionLength = tb.SelectionLength;
            var next = current.Remove(selectionStart, selectionLength).Insert(selectionStart, e.Text);
            e.Handled = !AmountAllowedRegex.IsMatch(next);
        }

        public static void Amount_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox tb) return;

            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText)) ?? "";
            text = text.Trim();
            if (!AmountAllowedRegex.IsMatch(text)) e.CancelCommand();
        }

        public const decimal MaxPlannedCost = 1_000_000m;                 // плановая стоимость (за единицу)
        public const decimal MaxSaleTotalAmount = 1_000_000_000_000_000m; // общая сумма реализации (итог документа)

        public static bool TryParseDecimalStrict(string? text, out decimal value)
        {
            value = 0m;
            var t = (text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(t)) return false;

            t = t.Replace(',', '.');
            if (!AmountAllowedRegex.IsMatch(t)) return false;

            return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Плановая стоимость: 0 < cost <= 1_000_000
        /// </summary>
        public static bool ValidatePlannedCost(TextBox tb)
        {
            if (!TryParseDecimalStrict(tb.Text, out var val) || val <= 0m)
            {
                SetError(tb, "Плановая стоимость должна быть числом больше 0");
                return false;
            }

            if (val > MaxPlannedCost)
            {
                SetError(tb, "Сумма слишком велика (плановая стоимость не может превышать 1 000 000)");
                return false;
            }

            ClearError(tb);
            tb.Text = val.ToString("0.##", CultureInfo.InvariantCulture);
            return true;
        }

        /// <summary>
        /// Общая сумма реализации: 0 < total <= 1_000_000_000_000_000
        /// </summary>
        public static bool ValidateSaleTotalAmount(decimal total, out string userMessage)
        {
            userMessage = "";

            if (total <= 0m)
            {
                userMessage = "Сумма реализации должна быть больше 0";
                return false;
            }

            if (total > MaxSaleTotalAmount)
            {
                userMessage =
                    "Система не предназначена для таких сумм. " +
                    "Сделайте несколько операций реализации меньшими суммами.";
                return false;
            }

            return true;
        }

        public static void Amount_LostFocus(TextBox tb)
        {
            var txt = (tb.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(txt))
            {
                SetError(tb, "Введите сумму > 0");
                return;
            }

            txt = txt.Replace(',', '.');
            if (!decimal.TryParse(txt, NumberStyles.Number, CultureInfo.InvariantCulture, out var val) || val <= 0m)
            {
                SetError(tb, "Сумма должна быть числом > 0");
                return;
            }

            ClearError(tb);
            tb.Text = val.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public static void ClearErrorOnTyping(TextBox tb)
        {
            if (!string.IsNullOrWhiteSpace(tb.Text)) ClearError(tb);
        }

        // =====================================================================
        // ===== Time (HH:mm) with correct caret =====
        // =====================================================================

        public static void Time_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        public static void Time_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        public static void Time_OnPasteDigitsOnly(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = (e.DataObject.GetData(DataFormats.UnicodeText) as string) ?? "";
            if (text.Any(ch => !char.IsDigit(ch))) e.CancelCommand();
        }

        public static void Time_TextChanged(TextBox tb)
        {
            var caret = tb.CaretIndex;
            var beforeCaretDigits = (tb.Text ?? "").Take(caret).Count(char.IsDigit);

            var raw = new string((tb.Text ?? "").Where(char.IsDigit).ToArray());
            if (raw.Length > 4) raw = raw[..4];

            var formatted = raw.Length switch
            {
                0 => "",
                1 => raw,
                2 => raw,
                3 => raw[..2] + ":" + raw[2..],
                4 => raw[..2] + ":" + raw[2..],
                _ => raw
            };

            if (tb.Text != formatted) tb.Text = formatted;

            tb.CaretIndex = GetCaretIndexByDigits(tb.Text ?? "", beforeCaretDigits);

            if (!string.IsNullOrWhiteSpace(tb.Text)) ClearError(tb);
        }

        private static int GetCaretIndexByDigits(string text, int digitsCount)
        {
            if (digitsCount <= 0) return 0;

            var seen = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i])) seen++;
                if (seen >= digitsCount) return i + 1;
            }

            return text.Length;
        }

        public static void Time_LostFocus(TextBox tb)
        {
            var t = (tb.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(t))
            {
                tb.Text = "00:00";
                ClearError(tb);
                return;
            }

            if (!TimeStrictRegex.IsMatch(t))
            {
                SetError(tb, "Время в формате HH:mm (например 12:00)");
                return;
            }

            ClearError(tb);
        }

        // =====================================================================
        // ===== Date ограничение: не позже Today+7 =====
        // =====================================================================

        /// <summary>
        /// Вызывать при изменении даты (SelectedDateChanged) или при подтверждении формы.
        /// </summary>
        public static bool ValidateDateNotLaterThanWeek(DatePicker dp, string fieldTitle = "Дата")
        {
            var d = dp.SelectedDate;

            if (d == null)
            {
                SetError(dp, $"{fieldTitle}: выберите дату");
                return false;
            }

            var max = DateTime.Today.AddDays(MaxFutureDays);
            if (d.Value.Date > max.Date)
            {
                SetError(dp, $"{fieldTitle}: нельзя выбрать дату позже {max:dd.MM.yyyy}");
                return false;
            }

            ClearError(dp);
            return true;
        }
    }
}
