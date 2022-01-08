using Newtonsoft.Json;
using System;

namespace WorkoutTracker.Models
{
    public abstract class EntityBase
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}
