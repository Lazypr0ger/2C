using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contracts.BindingModels;
using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class OperationApi
    {
        private readonly ApiClient _api;

        public OperationApi(ApiClient api)
        {
            _api = api;
        }

        // =========================
        // GET
        // =========================

        /// <summary>
        /// Получить все операции (без фильтра — как в 1С "Журнал операций")
        /// </summary>
        public Task<List<OperationVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<OperationVM>>("/ms/api/Operation", ct);

        /// <summary>
        /// Получить операцию по Id (для открытия документа)
        /// </summary>
        public Task<OperationVM> GetByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Operation id is empty", nameof(id));

            return _api.GetAsync<OperationVM>($"/ms/api/Operation/id/{id}", ct);
        }

        // =========================
        // CREATE / UPDATE
        // =========================

        /// <summary>
        /// Создать и провести операцию
        /// </summary>
        public Task CreateAsync(OperationBM bm, CancellationToken ct = default)
        {
            if (bm is null)
                throw new ArgumentNullException(nameof(bm));

            return _api.PostAsync("/ms/api/Operation", bm, ct);
        }

        /// <summary>
        /// Обновить и перепровести операцию
        /// </summary>
        public Task UpdateAsync(OperationBM bm, CancellationToken ct = default)
        {
            if (bm is null)
                throw new ArgumentNullException(nameof(bm));

            if (string.IsNullOrWhiteSpace(bm.Id))
                throw new ArgumentException("Operation id is empty", nameof(bm));

            return _api.PutAsync("/ms/api/Operation", bm, ct);
        }

        // =========================
        // DELETE / RECOVERY
        // =========================

        /// <summary>
        /// Пометить операцию как удалённую (soft delete)
        /// </summary>
        public Task SoftDeleteAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Operation id is empty", nameof(id));

            // DELETE с body — по твоему ApiClient
            return _api.DeleteAsync($"/ms/api/Operation/{id}", new { }, ct);
        }

        /// <summary>
        /// Восстановить операцию
        /// </summary>
        public Task RestoreAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Operation id is empty", nameof(id));

            // PATCH с пустым body
            return _api.PatchAsync($"/ms/api/Operation/{id}", new { }, ct);
        }
    }
}
