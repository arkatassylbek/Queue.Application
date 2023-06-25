using System.Threading.Tasks;

namespace Queue.Application.Receiver.DbServices;

public interface IDbProcessService
{
    Task InsertAsync(string processId);
}