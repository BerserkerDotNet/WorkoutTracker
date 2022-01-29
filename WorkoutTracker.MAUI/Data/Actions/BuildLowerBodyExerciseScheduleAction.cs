using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class BuildLowerBodyExerciseScheduleAction : IAsyncAction<IEnumerable<ExerciseViewModel>>
    {
        private readonly Random _random = new Random();

        public Task Execute(IDispatcher dispatcher, IEnumerable<ExerciseViewModel> allExercises)
        {
            if (allExercises is null)
            {
                return Task.CompletedTask;
            }

            var musclesToPick = new[] { 
                Guid.Parse("326bbce1-b660-4559-8fb0-67a7c59e1a4a") /* Gluteus maximus */, 
                Guid.Parse("78f7f381-80cd-4a48-ab89-5d58a54a8b50") /* Quadriceps */,
                Guid.Parse("e322933a-a4ca-42f9-adbd-cb9340cc4d89") /* Hamstring */,
                Guid.Parse("58ebbca6-1dea-464f-9b94-1b528452564b") /* Gastrocnemius */ 
            };
            _random.Shuffle(musclesToPick);

            var randomSet = new Dictionary<string, ScheduleViewModel>(musclesToPick.Length);
            var legExercises = allExercises.Where(e => e.Muscles.Any(m => m.MuscleGroup == "Legs"));
            foreach (var muscle in musclesToPick)
            {
                var exercisesToPickFrom = legExercises.Where(e => e.Muscles.Any(m => m.Id == muscle));
                var index = _random.Next(0, exercisesToPickFrom.Count());

                randomSet.Add(muscle.ToString(), new ScheduleViewModel(muscle.ToString(), index, exercisesToPickFrom));
            }

            dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet));

            return Task.CompletedTask;
        }
    }
}
