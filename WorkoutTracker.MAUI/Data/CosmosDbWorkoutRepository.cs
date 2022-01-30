using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Exceptions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data
{
    public class CosmosDbWorkoutRepository : IWorkoutRepository
    {
        private readonly HttpClient _client;

        public CosmosDbWorkoutRepository(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }

        public virtual async Task AddLogRecord(LogEntryViewModel model)
        {
            var logEntry = new ExerciseLogEntry
            {
                Id = model.Id,
                ExerciseId = model.Exercise.Id,
                Date = model.Date,
                Sets = model.Sets.ToArray()
            };
            await Create(logEntry, EndpointNames.ExerciseLogEntryPluralName); // Works by upserting the record
        }

        public virtual Task DeleteLog(Guid id)
        {
            return Delete<ExerciseLogEntry>(id, EndpointNames.ExerciseLogEntryPluralName);
        }

        public virtual async Task<IEnumerable<ExerciseViewModel>> GetExercises()
        {
            var exerciseDtos = await GetMultiple<Exercise>(EndpointNames.ExercisePluralName);
            var muscleDtos = await GetMultiple<Muscle>(EndpointNames.MusclePluralName);

            var musclesDictionary = muscleDtos.ToDictionary(k => k.Id, MapMuscle);
            return exerciseDtos.Select(e => MapExercise(e, musclesDictionary)).ToArray();
        }

        public virtual async Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date)
        {
            var exercisesDictionary = await GetExercisesLookup();
            var exerciseLogsDtos = await GetMultiple<ExerciseLogEntry>($"{EndpointNames.ExerciseLogEntryPluralName}?date={date.ToString("yyyy-MM-dd")}"); // TODO, make a parameter on a Get function

            return exerciseLogsDtos.Select(l => MapLog(l, exercisesDictionary)).ToArray();
        }

        public virtual async Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId)
        {
            var exercisesDictionary = await GetExercisesLookup();
            var exerciseLog = await Get<ExerciseLogEntry>($"{EndpointNames.GetPreviousWorkoutStatsByExercise}?exerciseId={exerciseId}");
            return MapLog(exerciseLog, exercisesDictionary);
        }

        public virtual Task UpdateExercise(ExerciseViewModel exercise)
        {
            throw new NotImplementedException();
        }

        private async Task<T> Get<T>(string endpoint)
            where T : EntityBase
        {
            var response = await _client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new DataFetchException($"Failed to fetch {endpoint}. Reason: {response.StatusCode} - {response.ReasonPhrase}.");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) 
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        private async Task<IEnumerable<T>> GetMultiple<T>(string endpoint)
             where T : EntityBase
        {
            var response = await _client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new DataFetchException($"Failed to fetch {endpoint}. Reason: {response.StatusCode} - {response.ReasonPhrase}.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
        }

        private async Task Create<T>(T entity, string endpoint)
            where T : EntityBase
        {
            var content = JsonConvert.SerializeObject(entity);
            var response = await _client.PostAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new DataPersistanceException($"Failed to create {endpoint} record. Reason: {response.StatusCode} - {response.ReasonPhrase}. {message}");
            }
        }

        public Task Delete<T>(Guid id, string endpoint)
            where T : EntityBase
        {
            return _client.DeleteAsync($"{endpoint}?id={id}");
        }

        private async Task<Dictionary<Guid, ExerciseViewModel>> GetExercisesLookup() 
        {
            var exercises = await GetExercises();
            return exercises.ToDictionary(k => k.Id, v => v);
        }

        private MuscleViewModel MapMuscle(Muscle muscle) 
        {
            return new MuscleViewModel
            {
                Id = muscle.Id,
                Name = muscle.Name,
                MuscleGroup = muscle.MuscleGroup,
                ImagePath = muscle.ImagePath
            };
        }

        private ExerciseViewModel MapExercise(Exercise exercise, Dictionary<Guid, MuscleViewModel> musclesDictionary)
        {
            return new ExerciseViewModel
            {
                Id = exercise.Id,
                Description = exercise.Description,
                Name= exercise.Name,
                ImagePath = exercise.ImagePath,
                Steps = exercise.Steps,
                Tags = exercise.Tags,
                TutorialUrl = exercise.TutorialUrl,
                Muscles = exercise.Muscles.Select(m => musclesDictionary[m]).ToArray() // Assume all muscles are present in the DB.
            };
        }

        private LogEntryViewModel MapLog(ExerciseLogEntry logEntry, Dictionary<Guid, ExerciseViewModel> exercisesDictionary)
        {
            if (logEntry is null) 
            {
                return null;
            }

            return new LogEntryViewModel
            {
                Id = logEntry.Id,
                Date = logEntry.Date,
                Sets = logEntry.Sets,
                Exercise = exercisesDictionary[logEntry.ExerciseId]
            };
        }
    }
}