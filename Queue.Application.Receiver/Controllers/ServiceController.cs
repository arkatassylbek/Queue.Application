using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Queue.Application.Receiver.DbServices;
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

    [HttpPost]
    [ActionName(nameof(Save))]
    public async Task<SaveResponse> Save(SaveRequest request)
    {
        try
        {
            await _dbProcessService.InsertAsync(request.ProcessId);
            _logger.LogInformation($"{request.ProcessId} saved");
            return new SaveResponse { IsSuccess = true };
        }
        catch (Exception e)
        {
            return new SaveResponse { IsSuccess = false, Error = e.Message };
        }
    }
    
    [HttpGet]
    [ActionName(nameof(GenerateProcesses))]
    public async Task<IActionResult> GenerateProcesses([FromQuery] int amount)
    {
        try
        {
            await _dbProcessQueueService.CreateProcessesAsync(amount);
            _logger.LogInformation($"{amount} generated");
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}