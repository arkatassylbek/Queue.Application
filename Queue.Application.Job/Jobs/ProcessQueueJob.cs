using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog;
using Quartz;
using Queue.Application.Job.DbServices;
using Queue.Application.Job.HttpServices;

namespace Queue.Application.Job.Jobs;

public class ProcessQueueJob : IJob
{
    private readonly IDbProcessQueueService _dbProcessQueueService;
    private readonly IReceiverService _receiverService;
    private readonly ILogger<ProcessQueueJob> _logger;
    
    public ProcessQueueJob(IDbProcessQueueService dbProcessQueueService, ILogger<ProcessQueueJob> logger, IReceiverService receiverService)
    {
        _dbProcessQueueService = dbProcessQueueService;
        _logger = logger;
        _receiverService = receiverService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        ScopeContext.PushProperty("jobName", context.JobDetail.Key.Name);
        ScopeContext.PushProperty("fireInstanceId", context.FireInstanceId);
        
        var processIds = await _dbProcessQueueService.GetToProcessAsync(
            context.JobDetail.JobDataMap.GetIntValue("BatchSize"),
            context.JobDetail.JobDataMap.GetBooleanValue("SortByAttempt"),
            context.JobDetail.JobDataMap.GetString("EventName")
        );
        
        _logger.LogInformation("{Count} to process", processIds.Count);

        if (processIds.Count == 0) return;

        var success = 0; var failed = 0;
        foreach (var processId in processIds)
        {
            try
            {
                var response = await _receiverService.Send(processId);
                if (response.IsSuccess is false) throw new Exception(response.Error);
                
                _logger.LogInformation("{ProcessId} successfully processed", processId);
                await _dbProcessQueueService.RemoveProcessAsync(processId);
                success += 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ProcessId} failed to process", processId);
                await _dbProcessQueueService.SetErrorProcessAsync(processId, e.Message);
                failed += 1;
            }
        }
        _logger.LogInformation("{Success} processed successfully, {Failed} failed.", success, failed);
    }
}