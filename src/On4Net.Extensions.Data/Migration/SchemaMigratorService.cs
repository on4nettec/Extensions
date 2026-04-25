using Microsoft.Extensions.Hosting;

namespace On4Net.Extensions.Data.Migration;

public class SchemaMigratorService<T> : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private readonly DataSchemaMigrator _dataSchemaMigrator;

    public SchemaMigratorService(IHostApplicationLifetime hostApplicationLifetime, DataSchemaMigrator dataSchemaMigrator)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _dataSchemaMigrator = dataSchemaMigrator;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _dataSchemaMigrator.UpdateSchemas<T>();
            return Task.CompletedTask;
        }
        catch
        {
            _hostApplicationLifetime.StopApplication();
            throw;
        }
    }
}