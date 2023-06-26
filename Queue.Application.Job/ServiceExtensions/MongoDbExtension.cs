using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Queue.Application.Job.Configs;
using Queue.Application.Job.DbServices;

namespace Queue.Application.Job.ServiceExtensions;

public static class MongoDbExtension
{
    public static void AddMongoDbServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbProcessQueueService>(sp => new DbProcessQueueService(sp.GetService<IOptionsMonitor<AppSettingsConfig>>().CurrentValue.MongoUrl));
    }
}