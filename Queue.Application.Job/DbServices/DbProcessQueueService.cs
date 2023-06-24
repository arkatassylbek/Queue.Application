using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Queue.Application.Job.Models;

namespace Queue.Application.Job.DbServices;

public class DbProcessQueueService : IDbProcessQueueService
{
    private readonly IMongoCollection<Process> _collection;

    public DbProcessQueueService()
    {
        _collection = new MongoClient("mongodb://localhost:27017/?retryWrites=true&serverSelectionTimeoutMS=5000&connectTimeoutMS=10000")
            .GetDatabase("local").GetCollection<Process>("processQueue");
    }

    public async Task<IEnumerable<string>> GetToProcessAsync(int batchSize)
    {
        var filter = Builders<Process>.Filter.Eq(i => i.Processing, false);
        var update = Builders<Process>.Update.Set(i => i.Processing, true).Set(i => i.ModifyDate, DateTime.Now).Inc(i => i.Attempt, 1);
        var options = new FindOneAndUpdateOptions<Process> { Projection = Builders<Process>.Projection.Include(i => i.Key) };

        var tasks = new List<Task<string>>();
        for (var i = 0; i < batchSize; i++)
        {
            tasks.Add(GetProcess(filter, update, options));
        }

        var output = await Task.WhenAll(tasks);
        return output.Where(r => r is not null);
    }

    private async Task<string> GetProcess(FilterDefinition<Process> filter, UpdateDefinition<Process> update, FindOneAndUpdateOptions<Process> options)
    {
        var result = await _collection.FindOneAndUpdateAsync(filter, update, options);
        return result?.Key;
    }
    
    public async Task CreateProcessesAsync(int amount)
    {
        var processes = new List<Process>();
        
        for (int i = 0; i < amount; i++)
        {
            processes.Add(new Process
            {
                Key = Guid.NewGuid().ToString(),
                EventName = "Step1",
                InsertDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Processing = false,
                Attempt = 0,
                Error = null
            });
        }
        
        await _collection.InsertManyAsync(processes);
    }
}