using Microsoft.Extensions.Logging;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Data.Actions;

public record InsertNewExerciseRequest(IExerciseSelector ExerciseSelector, IEnumerable<WorkoutViewModel> CurrentSchedule);
public class InsertNewExerciseAction : TrackableAction<InsertNewExerciseRequest>
{
    private readonly IWorkoutRepository _repository;
    private readonly WorkoutSetsService _workoutSetsService;

    public InsertNewExerciseAction(IWorkoutRepository repository, WorkoutSetsService workoutSetsService, ApplicationContext<InsertNewExerciseAction> context)
        : base(context, "Updating exercises list")
    {
        _repository = repository;
        _workoutSetsService = workoutSetsService;
    }

    protected override async Task Execute(IDispatcher dispatcher, InsertNewExerciseRequest request, Dictionary<string, string> trackableProperties) // Pass a current schedule to amend
    {
        var allExercises = await _repository.GetExercises();
        var descriptor = request.ExerciseSelector.Select(allExercises);
        if (descriptor is null)
        {
            Context.LogWarning("Didn't find any exercises matching {Selector} to insert into schedule.", request.ExerciseSelector.GetType().Name);
            return;
        }

        var exercises = descriptor.MatchedExercises.ToArray();
        var restTime = descriptor.TargetRestTime ?? ExerciseProfile.DefaultRestTime;
        var targetSets = descriptor.TargetSets ?? ExerciseProfile.DefaultNumberOfSets;
        var index = Random.Shared.Next(0, exercises.Length);
        var exercise = exercises.ElementAt(index);
        var sets = await _workoutSetsService.GenerateSets(exercise.Id, targetSets);
        var exerciseViewModel = new WorkoutExerciseViewModel(exercise.Id, exercise.Name, exercise.ImagePath, sets);
        var scheduleModel = new WorkoutViewModel(Guid.NewGuid(), restTime, exerciseViewModel);
        var currentScheduleList = request.CurrentSchedule.ToList();
        currentScheduleList.Add(scheduleModel);
        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(currentScheduleList.ToArray()));
    }
}


public class UpsertWorkoutProgramAction : TrackableAction<WorkoutProgram>
{
    private readonly IWorkoutRepository _repository;

    public UpsertWorkoutProgramAction(IWorkoutRepository repository, ApplicationContext<UpsertWorkoutProgramAction> context)
        : base(context, "Updating exercises list")
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, WorkoutProgram request, Dictionary<string, string> trackableProperties) // Pass a current schedule to amend
    {
        await _repository.UpdateProgram(request);
        await dispatcher.Dispatch<FetchWorkoutProgramAction>();
    }
}

public class FetchWorkoutProgramAction : TrackableAction
{
    private readonly IWorkoutRepository _repository;

    public FetchWorkoutProgramAction(IWorkoutRepository repository, ApplicationContext<FetchWorkoutProgramAction> context)
        : base(context, "Fetching programs")
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, Dictionary<string, string> trackableProperties)
    {
        var programs = await _repository.GetWorkoutPrograms();
        dispatcher.Dispatch(new ReceiveWorkoutProgramsAction(programs));
    }
}
