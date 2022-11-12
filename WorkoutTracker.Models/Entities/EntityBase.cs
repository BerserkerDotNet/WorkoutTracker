using Newtonsoft.Json;
using System;

namespace WorkoutTracker.Models.Entities;

public abstract class EntityBase
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}
