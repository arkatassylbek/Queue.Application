using Microsoft.Extensions.DependencyInjection;
using Queue.Application.Job.DbServices;

namespace Queue.Application.Job.ServiceExtensions;

public static class MongoDbExtension
{
    public static void AddMongoDbServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbProcessQueueService, DbProcessQueueService>();
    }
}