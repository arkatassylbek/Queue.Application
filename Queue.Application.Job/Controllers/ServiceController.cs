using Microsoft.AspNetCore.Mvc;

namespace Queue.Application.Job.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ServiceController : ControllerBase
{
    [HttpGet]
    [ActionName(nameof(CheckHealth))]
    public string CheckHealth() => "OK";
}