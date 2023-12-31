using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Queue.Application.Receiver.Models;

namespace Queue.Application.Receiver.DbServices;

public class DbProcessService : IDbProcessService
{
    private readonly IMongoCollection<Process> _collection;

    public DbProcessService(string mongoUrl)
    {
        _collection = new MongoClient(mongoUrl).GetDatabase("local").GetCollection<Process>("Process");
    }

    public async Task InsertAsync(string processId)
    { 
        await _collection.InsertOneAsync(new Process
        {
            Id = processId,
            Processed = true,
            InsertDate = DateTime.Now
        });
    }
}