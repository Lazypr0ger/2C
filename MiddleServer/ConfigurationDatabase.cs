using Contracts;

namespace MiddleServer;

public class ConfigurationDatabase(IConfiguration configuration) : IConfigurationDatabase
{
    private readonly Lazy<DataBaseSettings> _dataBaseSettings = new(() =>
    {
        return configuration.GetValue<DataBaseSettings>("DataBaseSettings") ?? throw new InvalidDataException(nameof(DataBaseSettings));
    });
    public string ConnectionString => _dataBaseSettings.Value.ConnectionString;
}
