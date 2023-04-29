using System;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Services.Data.Entities
{
    public class ProfileDbEntity : BaseDbEntity
    {
        public string Name { get; set; }
    
        public string Email { get; set; }

        public Guid? CurrentWorkout { get; set; }

        public Profile ToViewModel() => new Profile
        {
            Id = Id, Name = Name, Email = Email, CurrentWorkout = CurrentWorkout
        };

        public static ProfileDbEntity FromViewModel(Profile vm) => new ProfileDbEntity
        {
            Id = vm.Id,
            Name = vm.Name,
            Email = vm.Email,
            CurrentWorkout = vm.CurrentWorkout
        };
    }
}