using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Queue.Application.Receiver.Models;

public class Process
{
    [BsonId]
    public string Id { get; set; }
    public bool Processed { get; set; }
    public DateTime InsertDate { get; set; }
}