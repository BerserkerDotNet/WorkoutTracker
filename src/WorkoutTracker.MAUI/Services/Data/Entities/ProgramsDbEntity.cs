using SQLiteNetExtensions.Attributes;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class ProgramsDbEntity : BaseDbEntity
{
    public string Name { get; set; }

    [TextBlob(nameof(ScheduleBlobbed))]
    public Schedule Schedule { get; set; }

    public string ScheduleBlobbed { get; set; }

    public WorkoutProgram ToViewModel()
    {
        return new WorkoutProgram
        {
            Id = Id,
            Name = Name,
            Schedule = Schedule,
        };
    }

    public static ProgramsDbEntity FromViewModel(WorkoutProgram workoutProgram)
    {
        return new ProgramsDbEntity
        {
            Id = workoutProgram.Id,
            Name = workoutProgram.Name,
            Schedule = workoutProgram.Schedule
        };
    }
}