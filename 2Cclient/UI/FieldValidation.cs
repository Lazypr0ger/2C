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
        // ===== Regex =====
        public static readonly Regex AmountAllowedRegex =
            new(@"^[0-9]*([.,][0-9]*)?$", RegexOptions.Compiled);

        public static readonly Regex TimeStrictRegex =
            new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

        public static readonly Regex NameAllowedRegex =
            new(@"^[A-Za-zА-Яа-яЁё\s]*$", RegexOptions.Compiled);

        // ===== Validation (без Binding) =====
        public static void SetError(Control control, string message)
        {
            // Делаем dummy binding на Tag, чтобы получить BindingExpression.
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

        // ===== Name =====
        public static void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !NameAllowedRegex.IsMatch(e.Text);
        }

        public static void Name_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = (e.DataObject.GetData(DataFormats.UnicodeText) as string) ?? "";
            if (!NameAllowedRegex.IsMatch(text)) e.CancelCommand();
        }

        public static void Name_TextChanged(TextBox tb)
        {
            var t = tb.Text ?? "";

            if (!NameAllowedRegex.IsMatch(t))
            {
                SetError(tb, "Только русские/английские буквы и пробел");
                return;
            }

            // пока печатает — не ругаемся за пустоту
            if (string.IsNullOrWhiteSpace(t))
            {
                ClearError(tb);
                return;
            }

            ClearError(tb);
        }

        // ===== Amount =====
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
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.CancelCommand();
                return;
            }

            var text = ((string?)e.DataObject.GetData(DataFormats.UnicodeText)) ?? "";
            text = text.Trim();

            if (!AmountAllowedRegex.IsMatch(text)) e.CancelCommand();
        }

        public static bool TryParseDecimalStrict(string? text, out decimal value)
        {
            value = 0m;
            var t = (text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(t)) return false;

            t = t.Replace(',', '.');
            if (!AmountAllowedRegex.IsMatch(t)) return false;

            return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
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
            if (!string.IsNullOrWhiteSpace(tb.Text))
                ClearError(tb);
        }

        // ===== Time (HH:mm) with correct caret =====
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
            // число цифр до каретки
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

            if (tb.Text != formatted)
                tb.Text = formatted;

            // восстановить каретку по количеству цифр
            tb.CaretIndex = GetCaretIndexByDigits(tb.Text ?? "", beforeCaretDigits);

            // пока печатает — снять ошибку
            if (!string.IsNullOrWhiteSpace(tb.Text))
                ClearError(tb);
        }

        private static int GetCaretIndexByDigits(string text, int digitsCount)
        {
            if (digitsCount <= 0) return 0;

            var seen = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]))
                    seen++;

                if (seen >= digitsCount)
                    return i + 1;
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
    }
}
