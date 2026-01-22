using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace _2Cclient.Services.Api
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<T> GetAsync<T>(string path, CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync(path, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {path} -> {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");

            return JsonConvert.DeserializeObject<T>(body)
                   ?? throw new InvalidOperationException($"Empty JSON for GET {path}");
        }

        public async Task PostAsync<T>(string path, T payload, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsync(path, ToJson(payload), ct);
            await EnsureSuccess(resp, path, "POST", ct);
        }

        public async Task PutAsync<T>(string path, T payload, CancellationToken ct = default)
        {
            using var resp = await _http.PutAsync(path, ToJson(payload), ct);
            await EnsureSuccess(resp, path, "PUT", ct);
        }

        public async Task PatchAsync<T>(string path, T payload, CancellationToken ct = default)
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, path)
            {
                Content = ToJson(payload)
            };
            using var resp = await _http.SendAsync(req, ct);
            await EnsureSuccess(resp, path, "PATCH", ct);
        }

        public async Task DeleteAsync<T>(string path, T payload, CancellationToken ct = default)
        {
      
            var req = new HttpRequestMessage(HttpMethod.Delete, path)
            {
                Content = ToJson(payload)
            };
            using var resp = await _http.SendAsync(req, ct);
            await EnsureSuccess(resp, path, "DELETE", ct);
        }

        private static StringContent ToJson<T>(T payload)
            => new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        private static async Task EnsureSuccess(HttpResponseMessage resp, string path, string method, CancellationToken ct)
        {
            if (resp.IsSuccessStatusCode) return;

            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"{method} {path} -> {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");
        }
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        public async Task<TResponse> PostAsync<TResponse, TPayload>(string path, TPayload payload, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsync(path, ToJson(payload), ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"POST {path} -> {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");

            return JsonConvert.DeserializeObject<TResponse>(body)
                   ?? throw new InvalidOperationException($"Empty JSON for POST {path}");
        }
    }
}
