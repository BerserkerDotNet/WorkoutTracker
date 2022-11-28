﻿namespace WorkoutTracker.Models.Entities;

[PluralName(EndpointNames.UserPluralName)]
public class User : EntityBase
{
    public string Name { get; set; }

    public string Email { get; set; }
}