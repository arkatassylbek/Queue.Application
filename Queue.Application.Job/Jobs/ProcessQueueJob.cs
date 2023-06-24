using System;
using System.Threading.Tasks;
using Quartz;
using Queue.Application.Job.DbServices;

namespace Queue.Application.Job.Jobs;

public class ProcessQueueJob : IJob
{
    private readonly IDbProcessQueueService _dbProcessQueueService;

    public ProcessQueueJob(IDbProcessQueueService dbProcessQueueService)
    {
        _dbProcessQueueService = dbProcessQueueService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var processIds = await _dbProcessQueueService.GetToProcessAsync(
            context.JobDetail.JobDataMap.GetIntValue("BatchSize"),
            context.JobDetail.JobDataMap.GetBooleanValue("SortByAttempt"),
            context.JobDetail.JobDataMap.GetString("EventName")
        );
        foreach (var processId in processIds)
        {
            Console.WriteLine(processId + " in work");
        }
    }
}