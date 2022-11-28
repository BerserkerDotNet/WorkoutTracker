namespace WorkoutTracker.Data.Actions;

public record RemoveExerciseFromScheduleRequest(IEnumerable<WorkoutViewModel> CurrentSchedule, Guid ScheduleToRemove);
public class RemoveExerciseFromSchedule : TrackableAction<RemoveExerciseFromScheduleRequest>
{
    public RemoveExerciseFromSchedule(ApplicationContext<RemoveExerciseFromSchedule> context)
        : base(context, "Updating exercises list")
    {
    }

    protected override Task Execute(IDispatcher dispatcher, RemoveExerciseFromScheduleRequest request, Dictionary<string, string> trackableProperties)
    {
        Console.WriteLine($"Current sschedule {request.CurrentSchedule.Count()}");
        var newSchedule = request.CurrentSchedule.Where(s => s.Id != request.ScheduleToRemove).ToArray();
        Console.WriteLine($"New sschedule {newSchedule.Count()}");
        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(newSchedule));
        return Task.CompletedTask;
    }
}

public record ReplaceExerciseInScheduleRequest(Guid ScheduleId, ExerciseViewModel Exercise);

public class ReplaceExerciseInSchedule : TrackableAction<ReplaceExerciseInScheduleRequest>
{
    private readonly WorkoutSetsService _workoutSetsService;

    public ReplaceExerciseInSchedule(WorkoutSetsService workoutSetsService, ApplicationContext<RemoveExerciseFromSchedule> context)
        : base(context, "Replacing exercise")
    {
        this._workoutSetsService = workoutSetsService;
    }

    protected override async Task Execute(IDispatcher dispatcher, ReplaceExerciseInScheduleRequest request, Dictionary<string, string> trackableProperties)
    {
        var requestExercise = request.Exercise;
        var sets = await _workoutSetsService.GenerateSets(requestExercise.Id, ExerciseProfile.DefaultNumberOfSets);
        var exercise = new WorkoutExerciseViewModel(requestExercise.Id, requestExercise.Name, requestExercise.ImagePath, sets);
        dispatcher.Dispatch(new ReplaceScheduleExercise(request.ScheduleId, exercise));
    }
}