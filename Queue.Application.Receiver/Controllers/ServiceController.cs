using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Queue.Application.Receiver.DbServices;
using Queue.Application.Receiver.Filters;
using Queue.Application.Receiver.Models;

namespace Queue.Application.Receiver.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ServiceController : ControllerBase
{
    private readonly IDbProcessService _dbProcessService;
    private readonly IDbProcessQueueService _dbProcessQueueService;
    private readonly ILogger<ServiceController> _logger;

    public ServiceController(ILogger<ServiceController> logger, IDbProcessService dbProcessService, IDbProcessQueueService dbProcessQueueService)
    {
        _logger = logger;
        _dbProcessService = dbProcessService;
        _dbProcessQueueService = dbProcessQueueService;
    }

    [HttpGet]
    [ActionName(nameof(CheckHealth))]
    [TypeFilter(typeof(ServiceLogFilter<ServiceController>), Arguments = new object[] { nameof(CheckHealth) })]
    public string CheckHealth() => "OK";

    [HttpPost]
    [ActionName(nameof(SaveProcessId))]
    [TypeFilter(typeof(ServiceLogFilter<ServiceController>), Arguments = new object[] { nameof(SaveProcessId) })]
    public async Task<SaveResponse> SaveProcessId(SaveRequest request)
    {
        try
        {
            await _dbProcessService.InsertAsync(request.ProcessId);
            _logger.LogInformation("{ProcessId} saved", request.ProcessId);
            return new SaveResponse { IsSuccess = true };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Filed to save {ProcessId}", request.ProcessId);
            return new SaveResponse { IsSuccess = false, Error = e.Message };
        }
    }
    
    [HttpGet]
    [ActionName(nameof(GenerateProcesses))]
    [TypeFilter(typeof(ServiceLogFilter<ServiceController>), Arguments = new object[] { nameof(GenerateProcesses) })]
    public async Task<IActionResult> GenerateProcesses([FromQuery] int amount)
    {
        try
        {
            await _dbProcessQueueService.CreateProcessesAsync(amount);
            _logger.LogInformation("{Amount} generated", amount);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate");
            return StatusCode(500, e);
        }
    }
}