namespace WorkoutTracker.Data.Actions;

public class MoveToPreviousExerciseAction : IAsyncAction<ScheduleViewModel>
{
    public Task Execute(IDispatcher dispatcher, ScheduleViewModel request)
    {
        var index = request.CurrentIndex == 0 ? request.Exercises.Count() - 1 : request.CurrentIndex - 1;
        dispatcher.Dispatch(new ReceiveExerciseCurrentIndexAction(request.Id, index));

        return Task.CompletedTask;
    }
}

public class MoveToNextExerciseAction : IAsyncAction<ScheduleViewModel>
{
    public Task Execute(IDispatcher dispatcher, ScheduleViewModel request)
    {
        var index = request.CurrentIndex == request.Exercises.Count() - 1 ? 0 : request.CurrentIndex + 1;
        dispatcher.Dispatch(new ReceiveExerciseCurrentIndexAction(request.Id, index));

        return Task.CompletedTask;
    }
}