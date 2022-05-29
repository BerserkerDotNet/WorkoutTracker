namespace WorkoutTracker.Data.Actions;

public class MoveToPreviousExerciseAction : TrackableAction<ScheduleViewModel>
{
    public MoveToPreviousExerciseAction(ApplicationContext<MoveToPreviousExerciseAction> context)
        : base(context)
    {
    }

    protected override Task Execute(IDispatcher dispatcher, ScheduleViewModel request, Dictionary<string, string> trackableProperties)
    {
        var index = request.CurrentIndex == 0 ? request.Exercises.Count() - 1 : request.CurrentIndex - 1;
        trackableProperties.Add(nameof(request.CurrentIndex), request.CurrentIndex.ToString());
        trackableProperties.Add("NewIndex", index.ToString());
        trackableProperties.Add("ScheduleId", request.Id.ToString());

        dispatcher.Dispatch(new ReceiveExerciseCurrentIndexAction(request.Id, index));

        return Task.CompletedTask;
    }
}

public class MoveToNextExerciseAction : TrackableAction<ScheduleViewModel>
{
    public MoveToNextExerciseAction(ApplicationContext<MoveToPreviousExerciseAction> context)
    : base(context)
    {
    }

    protected override Task Execute(IDispatcher dispatcher, ScheduleViewModel request, Dictionary<string, string> trackableProperties)
    {
        var index = request.CurrentIndex == request.Exercises.Count() - 1 ? 0 : request.CurrentIndex + 1;
        trackableProperties.Add(nameof(request.CurrentIndex), request.CurrentIndex.ToString());
        trackableProperties.Add("NewIndex", index.ToString());
        trackableProperties.Add("ScheduleId", request.Id.ToString());

        dispatcher.Dispatch(new ReceiveExerciseCurrentIndexAction(request.Id, index));

        return Task.CompletedTask;
    }
}