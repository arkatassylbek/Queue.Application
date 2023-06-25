using Microsoft.Extensions.DependencyInjection;
using Queue.Application.Receiver.DbServices;

namespace Queue.Application.Receiver.ServiceExtensions;

public static class MongoDbExtension
{
    public static void AddMongoDbServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbProcessService, DbProcessService>();
        services.AddSingleton<IDbProcessQueueService, DbProcessQueueService>();
    }
}