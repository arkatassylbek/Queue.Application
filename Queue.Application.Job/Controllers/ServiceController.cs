using Microsoft.AspNetCore.Mvc;
using Queue.Application.Job.Filters;

namespace Queue.Application.Job.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ServiceController : ControllerBase
{
    [HttpGet]
    [ActionName(nameof(CheckHealth))]
    [TypeFilter(typeof(ServiceLogFilter), Arguments = new object[] { nameof(ServiceController), nameof(CheckHealth) })]
    public string CheckHealth() => "OK";
}