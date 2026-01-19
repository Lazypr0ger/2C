using Contracts.ViewModels;
using System.Globalization;

namespace _2Cclient.Services.Api;

public class TransactionLogApi
{
    private readonly ApiClient _api;
    public TransactionLogApi(ApiClient api) => _api = api;

    public Task<List<TransactionLogVM>> GetViewAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var path = "/ms/api/TransactionLog/view";

        if (from.HasValue || to.HasValue)
        {
            var q = new List<string>();
            if (from.HasValue) q.Add($"from={Uri.EscapeDataString(from.Value.ToString("O", CultureInfo.InvariantCulture))}");
            if (to.HasValue) q.Add($"to={Uri.EscapeDataString(to.Value.ToString("O", CultureInfo.InvariantCulture))}");
            path += "?" + string.Join("&", q);
        }

        return _api.GetAsync<List<TransactionLogVM>>(path, ct);
    }
}
