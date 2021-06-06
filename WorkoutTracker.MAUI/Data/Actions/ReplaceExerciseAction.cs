using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class ReplaceExerciseAction : IAsyncAction<ReplaceExerciseRequest>
    {
        private readonly Random _random = new Random();

        public Task Execute(IDispatcher dispatcher, ReplaceExerciseRequest request)
        {
            if (request.AllExercises is null)
            {
                return Task.CompletedTask;
            }

            var exercise = request.ExerciseToReplace;
            var categories = exercise.Muscles.Split(";");
            var exercises = request.AllExercises.Where(e => e.Id != exercise.Id && e.Tags.Contains("GoodForHome") && categories.Any(c => e.Muscles.Contains(c)));
            var newExercise = exercises.ElementAt(_random.Next(0, exercises.Count()));

            var currentSchedule = request.CurrentSchedule.ToArray();
            var idx = Array.IndexOf(currentSchedule, exercise);
            currentSchedule[idx] = newExercise;

            dispatcher.Dispatch(new ReceiveExerciseScheduleAction(currentSchedule));

            return Task.CompletedTask;
        }
    }
}
