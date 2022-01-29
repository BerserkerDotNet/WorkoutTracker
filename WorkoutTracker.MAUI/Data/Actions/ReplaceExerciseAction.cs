using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class MoveToPreviousExerciseAction : IAsyncAction<ScheduleViewModel>
    {
        public Task Execute(IDispatcher dispatcher, ScheduleViewModel request)
        {
            var newRequest = request with { CurrentIndex = request.CurrentIndex == 0 ? request.Exercises.Count() - 1 : request.CurrentIndex - 1 };
            dispatcher.Dispatch(new ReceiveNewExerciseScheduleItemAction(newRequest));

            return Task.CompletedTask;
        }
    }

    public class MoveToNextExerciseAction : IAsyncAction<ScheduleViewModel>
    {
        public Task Execute(IDispatcher dispatcher, ScheduleViewModel request)
        {
            var newRequest = request with { CurrentIndex = request.CurrentIndex == request.Exercises.Count() - 1 ? 0 : request.CurrentIndex + 1 };
            dispatcher.Dispatch(new ReceiveNewExerciseScheduleItemAction(newRequest));

            return Task.CompletedTask;
        }
    }
}
