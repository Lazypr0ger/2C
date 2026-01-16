using Contracts;
using Microsoft.Extensions.Configuration;

namespace MainServer;

public class ConfigurationDatabase(IConfiguration configuration) : IConfigurationDatabase
{
    private readonly Lazy<DataBaseSettings> _dataBaseSettings = new(() =>
    {
        return configuration.GetSection("DataBaseSettings").Get<DataBaseSettings>()
               ?? throw new InvalidDataException(nameof(DataBaseSettings));
    });

    public string ConnectionString => _dataBaseSettings.Value.ConnectionString;
}
