using System;

namespace WorkoutTracker.Models.Entities;

[PluralName(EndpointNames.UserPluralName)]
public class Profile : EntityBase
{
    public string Name { get; set; }

    public string Email { get; set; }

    public Guid? CurrentWorkout { get; set; }
}