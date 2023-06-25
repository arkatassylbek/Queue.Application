using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Queue.Application.Job.Models;

[BsonIgnoreExtraElements]
public class ProcessQueueItem
{
    public string Key { get; set; }
    public string EventName { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime ModifyDate { get; set; }
    public bool Processing { get; set; }
    public long Attempt { get; set; }
    public string Error { get; set; }
}