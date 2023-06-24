using System.Collections.Generic;
using System.Threading.Tasks;

namespace Queue.Application.Job.DbServices;

public interface IDbProcessQueueService
{
    Task<IEnumerable<string>> GetToProcessAsync(int batchSize);
    Task CreateProcessesAsync(int amount);
}