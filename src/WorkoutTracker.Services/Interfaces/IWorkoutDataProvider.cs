using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Services.Interfaces;

public interface IWorkoutDataProvider
{
    IExerciseSet GetMaxWeightLiftedOnExercise(Guid id);

    LogEntryViewModel GetLastEntryForExercise(Guid id);

    IEnumerable<MuscleViewModel> GetMuscles();

    IEnumerable<ExerciseViewModel> GetExercises();

    void UpdateViewModel<T>(T model);
    
    void DeleteViewModel<T>(T model);

    void SetCurrentWorkout(Guid id);

    Profile GetProfile();

    IEnumerable<WorkoutProgram> GetPrograms();

    IEnumerable<LogEntryViewModel> GetTodaysSchedule();
}