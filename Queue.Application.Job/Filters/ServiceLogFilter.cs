using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Queue.Application.Job.Filters;

public class ServiceLogFilter : Attribute, IActionFilter
{
    private readonly ILogger _logger;
    private readonly string _methodName;
    
    public ServiceLogFilter(ILoggerFactory loggerFactory, string className, string methodName)
    {
        _logger = loggerFactory.CreateLogger(className);
        _methodName = methodName;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("{Method}|{Method}|Request: {Request}", _methodName, context.HttpContext.Request.Method, JsonSerializer.Serialize(context.ActionArguments));
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var objectResult = (ObjectResult)context.Result;
        var response = JsonSerializer.Serialize(objectResult?.Value ?? "");
        _logger.LogInformation("{Method}|{StatusCode}|Response: {Response}", _methodName, context.HttpContext.Response.StatusCode, response);
    }
}