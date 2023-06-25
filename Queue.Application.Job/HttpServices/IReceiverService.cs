using System.Threading.Tasks;
using Queue.Application.Job.Models;

namespace Queue.Application.Job.HttpServices;

public interface IReceiverService
{
    Task<ReceiverResponse> Send(string processId);
}