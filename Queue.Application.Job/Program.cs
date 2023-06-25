using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Queue.Application.Job.Configs;
using Queue.Application.Job.ServiceExtensions;
using NLog;
using NLog.Web;
using Queue.Application.Job.HttpServices;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
    builder.Host.UseNLog();
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<AppSettingsConfig>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.Configure<JobsConfig>(builder.Configuration.GetSection("Jobs"));
    builder.Services.AddQuartzServices(builder.Configuration.Get<JobsConfig>());
    builder.Services.AddMongoDbServices();

    //Register services
    builder.Services.AddSingleton<IReceiverService>(sp => new ReceiverService(sp.GetService<IOptionsMonitor<AppSettingsConfig>>().CurrentValue.ReceiverUrl));
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}