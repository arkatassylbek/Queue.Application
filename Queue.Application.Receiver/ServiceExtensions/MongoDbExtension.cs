using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Queue.Application.Receiver.Configs;
using Queue.Application.Receiver.DbServices;

namespace Queue.Application.Receiver.ServiceExtensions;

public static class MongoDbExtension
{
    public static void AddMongoDbServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbProcessService>(sp => new DbProcessService(sp.GetService<IOptionsMonitor<AppSettingsConfig>>().CurrentValue.MongoUrl));
        services.AddSingleton<IDbProcessQueueService>(sp => new DbProcessQueueService(sp.GetService<IOptionsMonitor<AppSettingsConfig>>().CurrentValue.MongoUrl));
    }
}