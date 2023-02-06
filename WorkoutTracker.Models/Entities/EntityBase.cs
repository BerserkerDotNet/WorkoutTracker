using System;
using System.Text.Json.Serialization;

namespace WorkoutTracker.Models.Entities;

public abstract class EntityBase
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}
