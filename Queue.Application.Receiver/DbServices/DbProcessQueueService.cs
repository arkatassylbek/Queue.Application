using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Queue.Application.Receiver.Models;

namespace Queue.Application.Receiver.DbServices;

public class DbProcessQueueService : IDbProcessQueueService
{
    private readonly IMongoCollection<ProcessQueueItem> _collection;

    public DbProcessQueueService(string mongoUrl)
    {
        _collection = new MongoClient(mongoUrl).GetDatabase("local").GetCollection<ProcessQueueItem>("ProcessQueue");
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
                NextProcessingDate = DateTime.Now.AddMinutes(3),
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