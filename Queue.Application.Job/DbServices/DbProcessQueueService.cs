using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Queue.Application.Job.Models;

namespace Queue.Application.Job.DbServices;

public class DbProcessQueueService : IDbProcessQueueService
{
    private readonly IMongoCollection<ProcessQueueItem> _collection;

    public DbProcessQueueService()
    {
        _collection = new MongoClient("mongodb://localhost:27017/?retryWrites=true&serverSelectionTimeoutMS=5000&connectTimeoutMS=10000")
            .GetDatabase("local").GetCollection<ProcessQueueItem>("ProcessQueue");
    }

    public async Task<List<string>> GetToProcessAsync(int batchSize, bool sortByAttempt, string eventName = null)
    {
        var update = Builders<ProcessQueueItem>.Update
            .Set(i => i.Processing, true)
            .Set(i => i.ModifyDate, DateTime.Now)
            .Inc(i => i.Attempt, 1);
        
        var options = new FindOneAndUpdateOptions<ProcessQueueItem>
        {
            Projection = Builders<ProcessQueueItem>.Projection.Include(i => i.Key)
        };
        
        FilterDefinition<ProcessQueueItem> filter;
        if (string.IsNullOrWhiteSpace(eventName))
        {
            filter = Builders<ProcessQueueItem>.Filter.Eq(i => i.Processing, false);
        }
        else
        {
            filter = Builders<ProcessQueueItem>.Filter.And(
                Builders<ProcessQueueItem>.Filter.Eq(i => i.Processing, false), 
                Builders<ProcessQueueItem>.Filter.Eq(i => i.EventName, eventName));
        }

        if (sortByAttempt)
        {
            options.Sort = Builders<ProcessQueueItem>.Sort.Descending(i => i.Attempt);
        }
        
        var tasks = new List<Task<string>>();
        for (var i = 0; i < batchSize; i++)
        {
            tasks.Add(GetProcessAsync(filter, update, options));
        }

        var processIds = await Task.WhenAll(tasks);
        return processIds.Where(p => p is not null).ToList();
    }

    private async Task<string> GetProcessAsync(FilterDefinition<ProcessQueueItem> filter, UpdateDefinition<ProcessQueueItem> update, FindOneAndUpdateOptions<ProcessQueueItem> options)
    {
        var result = await _collection.FindOneAndUpdateAsync(filter, update, options);
        return result?.Key;
    }

    public async Task SetErrorProcessAsync(string processId, string error)
    {
        var filter = Builders<ProcessQueueItem>.Filter.Eq(i => i.Key, processId);
        var update = Builders<ProcessQueueItem>.Update
            .Set(i => i.Processing, true)
            .Set(i => i.ModifyDate, DateTime.Now)
            .Inc(i => i.Error, error);
        
        await _collection.UpdateOneAsync(filter, update);
    }
    
    public async Task RemoveProcessAsync(string processId)
    {
        await _collection.DeleteOneAsync(Builders<ProcessQueueItem>.Filter.Eq(i => i.Key, processId));
    }
}