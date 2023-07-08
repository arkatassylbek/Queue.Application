using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Queue.Application.Receiver.Filters;

public class ServiceLogFilter<T> : Attribute, IActionFilter
{
    private readonly ILogger<T> _logger;
    private readonly string _methodName;
    
    public ServiceLogFilter(ILoggerFactory loggerFactory, string methodName)
    {
        _logger = loggerFactory.CreateLogger<T>();
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