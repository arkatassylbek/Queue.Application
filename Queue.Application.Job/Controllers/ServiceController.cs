using Microsoft.AspNetCore.Mvc;
using Queue.Application.Job.Filters;

namespace Queue.Application.Job.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ServiceController : ControllerBase
{
    [HttpGet]
    [ActionName(nameof(CheckHealth))]
    [TypeFilter(typeof(ServiceLogFilter<ServiceController>), Arguments = new object[] { nameof(CheckHealth) })]
    public string CheckHealth() => "OK";
}