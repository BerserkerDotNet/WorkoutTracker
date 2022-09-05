namespace WorkoutTracker.Data.Actions;

public record RemoveExerciseFromScheduleRequest(IEnumerable<ScheduleViewModel> CurrentSchedule, Guid ScheduleToRemove);
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