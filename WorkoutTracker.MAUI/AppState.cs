using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI
{
    internal class ExerciseLogRecord
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public ExerciseLogEntry LogEntry { get; set; }

    }

    internal class AppState
    {
        private readonly Random _random = new Random();
        private readonly IExerciseLogRepository _repository;
        private readonly INotificationService _notification;
        private Dictionary<Guid, Exercise> _exercises;
        private Exercise[] _currentSchedule = Array.Empty<Exercise>();

        public AppState(IExerciseLogRepository repository, INotificationService notification)
        {
            _repository = repository;
            _notification = notification;
        }

        public Dictionary<Guid, Exercise> Exercises => _exercises;

        public IEnumerable<Exercise> Schedule => _currentSchedule;

        public async Task ReloadExercises()
        {
            var exercises = await _repository.GetAll<Exercise>();
            _exercises = exercises.ToDictionary(k => k.Id);
        }

        public async ValueTask<ItemsProviderResult<ExerciseLogRecord>> LoadExerciseLog(ItemsProviderRequest request)
        {
            if (_exercises is null)
            {
                await ReloadExercises();
            }

            var availableDatesString = await _repository.GetDates();
            var availableDates = availableDatesString
                .Select(d => DateTime.ParseExact(d, "dd-MM-yyyy", null))
                .OrderByDescending(d => d)
                .ToArray();

            if (availableDates.Length == 0)
            {
                return new ItemsProviderResult<ExerciseLogRecord>(Enumerable.Empty<ExerciseLogRecord>(), 0);
            }

            var logEntryChunk = await _repository.GetByDate(availableDates[0]);

            var records = logEntryChunk.Select(i => new ExerciseLogRecord
            {
                Name = _exercises[i.ExerciseId].Name,
                Icon = Convert.ToBase64String(_exercises[i.ExerciseId].Icon),
                LogEntry = i
            }).OrderByDescending(r => r.LogEntry.Date);

            return new ItemsProviderResult<ExerciseLogRecord>(records, records.Count());
        }

        public async Task BuildNewSchedule()
        {
            if (_exercises is null)
            {
                await ReloadExercises();
            }

            var exercises = Exercises.Values.Where(e => e.Tags.Contains("GoodForHome"));

            var categoriesToPick = new[] { "Chest", "Back", "Shoulders", "Triceps", "Biceps", "Abdominals" };
            _random.Shuffle(categoriesToPick);

            var randomSet = new List<Exercise>(categoriesToPick.Length);

            foreach (var category in categoriesToPick)
            {
                var exercisesByCategory = exercises.Where(e => e.Muscles.Contains(category));
                var count = exercisesByCategory.Count();

                var pickedExercise = exercisesByCategory.ElementAt(_random.Next(0, count));
                randomSet.Add(pickedExercise);
            }

            _currentSchedule = randomSet.ToArray();
        }

        public void ReplaceExercise(Exercise exercise)
        {
            var categories = exercise.Muscles.Split(";");
            var exercises = Exercises.Values.Where(e => e.Id != exercise.Id && e.Tags.Contains("GoodForHome") && categories.Any(c => e.Muscles.Contains(c)));
            var newExercise = exercises.ElementAt(_random.Next(0, exercises.Count()));

            var idx = Array.IndexOf(_currentSchedule, exercise);
            _currentSchedule[idx] = newExercise;
        }

        public async Task LogExercise(ExerciseLogEntry entry)
        {
            await _repository.Create(entry);
            _notification.ShowToast("Exercise saved.");
            // _exercisesLog.Insert(0, entry);
        }

        public async Task DeleteExerciseLog(ExerciseLogEntry entry)
        {
            await _repository.Delete<ExerciseLogEntry>(entry.Id);
            _notification.ShowToast("Exercise deleted.");
            // _exercisesLog.Remove(entry);
        }
    }
}
