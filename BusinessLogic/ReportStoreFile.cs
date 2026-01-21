using System.Text.Json;
using Contracts.DTO.Reports;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;

namespace BusinessLogic;

public class ReportStoreFile : IReportStore
{
    private readonly string _dir;
    private static readonly object _lock = new();

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public ReportStoreFile()
    {
        _dir = Path.Combine(AppContext.BaseDirectory, "ReportsJson");
        Directory.CreateDirectory(_dir);
    }

    public void Save(ReportResultDto report)
    {
        if (report is null) throw new ArgumentNullException(nameof(report));
        if (string.IsNullOrWhiteSpace(report.Id)) throw new ValidationException("Report id is empty");

        lock (_lock)
        {
            var path = Path.Combine(_dir, $"{report.Id}.json");
            WriteAtomic(path, JsonSerializer.Serialize(report, _json));
        }
    }

    public ReportResultDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Id is empty");

        var path = Path.Combine(_dir, $"{id}.json");

        if (!File.Exists(path))
            throw new StorageException(new FileNotFoundException($"Report '{id}' not found", path));

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ReportResultDto>(json, _json)
                   ?? throw new StorageException(new InvalidDataException($"Report '{id}' is corrupted"));
        }
        catch (StorageException) { throw; }
        catch (Exception ex) { throw new StorageException(ex); }
    }

    // ✅ читаем все файлы, фильтруем по TypeCode
    public List<ReportListItemDto> GetList(ReportTypeCodes? typeCode = null)
    {
        lock (_lock)
        {
            var files = Directory.EnumerateFiles(_dir, "*.json", SearchOption.TopDirectoryOnly);

            var result = new List<ReportListItemDto>();

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var r = JsonSerializer.Deserialize<ReportResultDto>(json, _json);
                    if (r == null) continue;

                    if (typeCode.HasValue && r.TypeCode != typeCode.Value)
                        continue;

                    result.Add(new ReportListItemDto
                    {
                        Id = r.Id,
                        TypeCode = r.TypeCode,
                        Name = r.Name,
                        From = r.From,
                        To = r.To,
                        BuildDate = r.BuildDate,
                        TotalActualCosts = r.TotalActualCosts,
                        Total1 = r.Total1,
                        Total2 = r.Total2,
                        Total3 = r.Total3
                    });
                }
                catch
                {
                   
                }
            }

            // последние сверху
            return result
                .OrderByDescending(x => x.BuildDate)
                .ToList();
        }
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Id is empty");

        lock (_lock)
        {
            var path = Path.Combine(_dir, $"{id}.json");
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private static void WriteAtomic(string path, string content)
    {
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, content);
        File.Copy(tmp, path, overwrite: true);
        File.Delete(tmp);
    }
}
