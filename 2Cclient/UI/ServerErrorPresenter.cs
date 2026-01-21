using System;
using System.Net.Http;
using System.Text.Json;

namespace _2Cclient.UI
{
    public static class ServerErrorPresenter
    {
        public static string ToUserMessage(Exception ex)
        {
            // Http-level
            if (ex is TaskCanceledException)
                return "Запрос отменён или превышено время ожидания.";

            if (ex is HttpRequestException)
                return "Не удалось подключиться к серверу. Проверьте соединение и попробуйте снова.";

            // часто ApiClient кидает Exception с текстом из сервера:
            var msg = ex.Message ?? "";

            // 1) попробуем распарсить json { message: "..."} если сервер так отдаёт
            var parsed = TryExtractMessageFromJson(msg);
            if (!string.IsNullOrWhiteSpace(parsed))
                msg = parsed;

            // 2) маппинг типовых серверных ошибок в нормальные
            msg = MapKnownMessages(msg);

            // 3) fallback
            if (string.IsNullOrWhiteSpace(msg))
                return "Произошла ошибка. Попробуйте ещё раз.";

            return msg;
        }

        private static string? TryExtractMessageFromJson(string raw)
        {
            try
            {
                // если сырой ответ уже в message
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("message", out var m))
                    return m.GetString();
                if (doc.RootElement.TryGetProperty("error", out var e))
                    return e.GetString();
                return null;
            }
            catch { return null; }
        }

        private static string MapKnownMessages(string msg)
        {
            // делай список расширяемым
            if (msg.Contains("NameDocument is empty", StringComparison.OrdinalIgnoreCase))
                return "Заполните название документа.";

            if (msg.Contains("DepartamentId is empty", StringComparison.OrdinalIgnoreCase))
                return "Выберите подразделение.";

            if (msg.Contains("OrganisationId is empty", StringComparison.OrdinalIgnoreCase))
                return "Выберите организацию.";

            if (msg.Contains("Elements are empty", StringComparison.OrdinalIgnoreCase))
                return "Добавьте хотя бы одну строку.";

            if (msg.Contains("Element.CountElement must be > 0", StringComparison.OrdinalIgnoreCase))
                return "Количество должно быть больше нуля.";

            if (msg.Contains("Element.Price must be > 0", StringComparison.OrdinalIgnoreCase))
                return "Цена должна быть больше нуля.";

            if (msg.Contains("Недостаточно материалов", StringComparison.OrdinalIgnoreCase))
                return msg; // уже человеческий текст из BL

            if (msg.Contains("Недостаточно остатка", StringComparison.OrdinalIgnoreCase))
                return msg; // уже человеческий текст из BL

            if (msg.Contains("Операция распределения (4) уже проведена", StringComparison.OrdinalIgnoreCase))
                return msg;

            if (msg.Contains("Нельзя выполнить списание отклонений (5)", StringComparison.OrdinalIgnoreCase))
                return msg;

            if (msg.Contains("Операция списания отклонений (5) уже проведена", StringComparison.OrdinalIgnoreCase))
                return msg;

            // общие
            if (msg.Contains("ValidationException", StringComparison.OrdinalIgnoreCase))
                return "Проверьте введённые данные.";

            return msg;
        }
    }
}
