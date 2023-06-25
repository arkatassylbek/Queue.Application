using System.Collections.Generic;
using System.Threading.Tasks;

namespace Queue.Application.Job.DbServices;

public interface IDbProcessQueueService
{
    Task<List<string>> GetToProcessAsync(int batchSize, bool sortByAttempt, string eventName = null);
    Task SetErrorProcessAsync(string processId, string error);
    Task RemoveProcessAsync(string processId);
}