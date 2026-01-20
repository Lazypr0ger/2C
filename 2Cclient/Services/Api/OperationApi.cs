// Services/Api/OperationApi.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class OperationApi
    {
        private readonly ApiClient _api;

        // Важно: это путь MiddleServer (Ocelot): /ms/api/Operation
        private const string BasePath = "/ms/api/Operation";

        public OperationApi(ApiClient api)
        {
            _api = api;
        }

        /// <summary>
        /// Получить все операции (опционально по периоду).
        /// Сервер сортирует по DateOperation desc/Id desc (у тебя в storage так).
        /// </summary>
        public Task<List<OperationVM>> GetAllAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var path = BasePath + BuildPeriodQuery(fromUtc, toUtc);
            return _api.GetAsync<List<OperationVM>>(path, ct);
        }

        /// <summary>
        /// Получить операции конкретного типа (фильтр делаем на клиенте, т.к. эндпоинта type=... у сервера сейчас нет).
        /// </summary>
        public async Task<List<OperationVM>> GetByTypeAsync(OperationType type, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var all = await GetAllAsync(fromUtc, toUtc, ct);
            return all
                .Where(x => x.Type == type)
                .OrderByDescending(x => x.DateOperation)
                .ThenByDescending(x => x.Id)
                .ToList();
        }

        /// <summary>
        /// Получить операцию по Id: GET /ms/api/Operation/id/{id}
        /// </summary>
        public Task<OperationVM> GetByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Operation id is empty", nameof(id));

            return _api.GetAsync<OperationVM>($"{BasePath}/id/{id}", ct);
        }

        /// <summary>
        /// Создать: POST /ms/api/Operation
        /// </summary>
        public Task CreateAsync(OperationBM bm, CancellationToken ct = default)
        {
            if (bm is null) throw new ArgumentNullException(nameof(bm));
            return _api.PostAsync(BasePath, bm, ct);
        }

        /// <summary>
        /// Обновить: PUT /ms/api/Operation
        /// </summary>
        public Task UpdateAsync(OperationBM bm, CancellationToken ct = default)
        {
            if (bm is null) throw new ArgumentNullException(nameof(bm));
            return _api.PutAsync(BasePath, bm, ct);
        }

        /// <summary>
        /// Удалить (пометка): DELETE /ms/api/Operation/{id}
        /// В твоем ApiClient DeleteAsync требует body — отправим пустой объект.
        /// </summary>
        public Task DeleteAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Operation id is empty", nameof(id));

            return _api.DeleteAsync($"{BasePath}/{id}", new { }, ct);
        }

        /// <summary>
        /// Восстановить: PATCH /ms/api/Operation/{id}
        /// PATCH в ApiClient тоже с body — отправим пустой объект.
        /// </summary>
        public Task RecoveryAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Operation id is empty", nameof(id));

            return _api.PatchAsync($"{BasePath}/{id}", new { }, ct);
        }

        private static string BuildPeriodQuery(DateTime? fromUtc, DateTime? toUtc)
        {
            // Передаем в ISO 8601, желательно UTC с 'Z'
            // Сервер принимает DateTime? из query.
            var parts = new List<string>();

            if (fromUtc.HasValue)
                parts.Add("from=" + Uri.EscapeDataString(ToIso(fromUtc.Value)));

            if (toUtc.HasValue)
                parts.Add("to=" + Uri.EscapeDataString(ToIso(toUtc.Value)));

            return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
        }

        private static string ToIso(DateTime dt)
        {
            // нормализуем в UTC, чтобы не было Kind=Unspecified
            if (dt.Kind == DateTimeKind.Unspecified)
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            else if (dt.Kind == DateTimeKind.Local)
                dt = dt.ToUniversalTime();

            return dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
        }
    }
}
