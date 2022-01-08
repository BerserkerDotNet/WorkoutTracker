using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Models;

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
            var categories = exercise.Muscles;
            var exercises = request.AllExercises.Where(e => e.Id != exercise.Id && categories.Any(c => e.Muscles.Contains(c)));
            Exercise newExercise = null;
            while (true) 
            {
                newExercise = exercises.ElementAt(_random.Next(0, exercises.Count()));
                if (!request.CurrentSchedule.Any(s => s.Id == newExercise.Id)) 
                {
                    break;
                }
            }

            var currentSchedule = request.CurrentSchedule.ToArray();
            var idx = Array.IndexOf(currentSchedule, exercise);
            currentSchedule[idx] = newExercise;

            dispatcher.Dispatch(new ReceiveExerciseScheduleAction(currentSchedule));

            return Task.CompletedTask;
        }
    }
}
