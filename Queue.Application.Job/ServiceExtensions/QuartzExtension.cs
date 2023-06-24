using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Queue.Application.Job.Configs;
using Queue.Application.Job.Jobs;
#pragma warning disable CS0618


namespace Queue.Application.Job.ServiceExtensions;

public static class QuartzExtension
{
    public static void AddQuartzServices(this IServiceCollection services, JobsConfig jobsConfig)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Background";
            
            q.UseMicrosoftDependencyInjectionJobFactory(options =>
            {
                options.AllowDefaultConstructor = true;
            });
            
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 101;
            });
                
            foreach (var (name, jobConfig) in jobsConfig.Jobs)
            {
                if (jobConfig.Enabled is false) continue;

                //здесь может быть любой класс вместо ProcessQueue1Job, главное класс из папки Jobs
                var jobType = Type.GetType($"{typeof(ProcessQueueJob).Namespace}.{name}Job"); 

                if (jobType is null) continue;

                var jobKey = new JobKey(name, "BackgroundTasks");
                q.AddJob(jobType, jobKey, j => j
                    .StoreDurably()
                    .UsingJobData("BatchSize", jobConfig.BatchSize)
                    .UsingJobData("EventName", jobConfig.EventName)
                    .UsingJobData("SortByAttempt", jobConfig.SortByAttempt)
                );
                    
                q.AddTrigger(t => t
                    .WithIdentity(new TriggerKey(name, "BackgroundTasks"))
                    .ForJob(jobKey)
                    .WithCronSchedule(jobConfig.CronSchedule)
                );
            }
        });
            
        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        services.Scan(scan => scan.FromAssemblyOf<ProcessQueueJob>()
            .AddClasses(classes => classes.AssignableTo<IJob>())
            .AsSelfWithInterfaces()
            .WithTransientLifetime());
    }
}