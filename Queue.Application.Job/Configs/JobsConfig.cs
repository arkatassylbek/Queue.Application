using System.Collections.Generic;

namespace Queue.Application.Job.Configs;

public class JobsConfig
{
    public Dictionary<string, JobConfig> Jobs { get; set; }
}

public class JobConfig
{
    public int BatchSize { get; set; }
    public bool Enabled { get; set; }
    public string CronSchedule { get; set; }
}