using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Queue.Application.Receiver.Models;

namespace Queue.Application.Receiver.DbServices;

public class DbProcessQueueService : IDbProcessQueueService
{
    private readonly IMongoCollection<ProcessQueueItem> _collection;

    public DbProcessQueueService()
    {
        _collection = new MongoClient("mongodb://localhost:27017/?retryWrites=true&serverSelectionTimeoutMS=5000&connectTimeoutMS=10000")
            .GetDatabase("local").GetCollection<ProcessQueueItem>("ProcessQueue");
    }

    public async Task CreateProcessesAsync(int amount)
    {
        var queueItems = new List<WriteModel<ProcessQueueItem>>();
        var random = new Random();
        
        for (var i = 0; i < amount; i++)
        {
            var stepNumber = random.Next(1, 10);
            var queueItem = new ProcessQueueItem
            {
                ProcessId = Guid.NewGuid().ToString(),
                EventName = $"Step{stepNumber}",
                InsertDate = DateTime.Now,
                NextProcessingDate = DateTime.Now.AddHours(10),
                ModifyDate = DateTime.Now,
                Processing = false,
                Attempt = 0,
                Error = null
            };
            queueItems.Add(new InsertOneModel<ProcessQueueItem>(queueItem));
        }

        await _collection.BulkWriteAsync(queueItems, new BulkWriteOptions { IsOrdered = false });
    }
}