using System.Threading.Tasks;

namespace Queue.Application.Receiver.DbServices;

public interface IDbProcessQueueService
{
    Task CreateProcessesAsync(int amount);
}