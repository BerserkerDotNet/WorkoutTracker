using Mapster;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using UnitsNet;
using WorkoutTracker.Exceptions;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Mappings;

namespace WorkoutTracker.Data;

public class CosmosDbWorkoutRepository : IWorkoutRepository
{
    private readonly HttpClient _client;
    private readonly IAccessTokenProvider _accessTokenProvider;

    protected ApplicationContext<IWorkoutRepository> Context { get; }

    public CosmosDbWorkoutRepository(IHttpClientFactory clientFactory, IAccessTokenProvider accessTokenProvider, ApplicationContext<IWorkoutRepository> context)
    {
        _client = clientFactory.CreateClient("api");
        _accessTokenProvider = accessTokenProvider;
        Context = context;
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
        var muscles = await GetMuscles();

        return exerciseDtos.AdaptToViewModel(muscles);
    }

    public async Task<IEnumerable<MuscleViewModel>> GetMuscles()
    {
        var muscleDtos = await GetMultiple<Muscle>(EndpointNames.MusclePluralName);
        return muscleDtos.Adapt<IEnumerable<MuscleViewModel>>();
    }

    public virtual async Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date)
    {
        var exercisesDictionary = await GetExercisesLookup();
        var dateString = date.ToString("O");
        var exerciseLogsDtos = await GetMultiple<ExerciseLogEntry>($"{EndpointNames.ExerciseLogEntryPluralName}?date={dateString}"); // TODO, make a parameter on a Get function

        return exerciseLogsDtos.Select(l => MapLog(l, exercisesDictionary)).ToArray();
    }

    public virtual async Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime from, DateTime to)
    {
        var exercisesDictionary = await GetExercisesLookup();
        var fromString = from.ToString("O");
        var toString = to.ToString("O");
        var exerciseLogsDtos = await GetMultiple<ExerciseLogEntry>($"{EndpointNames.ExerciseLogEntryPluralName}?from={fromString}&to={toString}"); // TODO, make a parameter on a Get function

        return exerciseLogsDtos.Select(l => MapLog(l, exercisesDictionary)).ToArray();
    }

    public virtual async Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId)
    {
        var exercisesDictionary = await GetExercisesLookup();
        var exerciseLog = await Get<ExerciseLogEntry>($"{EndpointNames.GetPreviousWorkoutStatsByExercise}?exerciseId={exerciseId}");
        return MapLog(exerciseLog, exercisesDictionary);
    }

    public virtual async Task UpdateExercise(ExerciseViewModel exercise)
    {
        var exerciseDto = exercise.Adapt<Exercise>();
        await Create(exerciseDto, EndpointNames.ExercisePluralName); // Works by upserting the record
    }

    public async Task UpdateMuscle(MuscleViewModel muscle)
    {
        var muscleDto = muscle.Adapt<Muscle>();
        await Create(muscleDto, EndpointNames.MusclePluralName); // Works by upserting the record
    }

    public virtual async Task<IEnumerable<WorkoutSummary>> GetWorkoutSummaries(DateTime from, DateTime to)
    {
        var logs = await GetLogs(from.ToUniversalTime(), to.ToUniversalTime());
        var summaries = logs.Where(log => log.Sets.Any()).Select(log =>
        {
            var sets = log.Sets;
            var maxSet = sets.MaxBy(s => s.WeightLB);
            var minSet = sets.MinBy(s => s.WeightLB);
            var avgWeightLb = sets.Aggregate(0.0d, (acc, s) => acc + s.WeightLB ?? 0) / sets.Count();
            var avgReps = sets.Aggregate(0, (acc, s) => acc + s.Repetitions) / sets.Count();
            var avgDuration = sets.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.Duration) / sets.Count();
            var avgRest = sets.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.RestTime) / sets.Count();

            var max = new WorkoutSetSummary(maxSet.WeightKG ?? 0, Math.Ceiling(maxSet.WeightLB ?? 0), maxSet.Repetitions, maxSet.Duration, maxSet.RestTime, sets);
            var min = new WorkoutSetSummary(minSet.WeightKG ?? 0, Math.Ceiling(minSet.WeightLB ?? 0), minSet.Repetitions, minSet.Duration, minSet.RestTime, sets);
            var avg = new WorkoutSetSummary(Mass.FromPounds(avgWeightLb).Kilograms, Math.Ceiling(avgWeightLb), avgReps, avgDuration, avgRest, sets);
            var total = new WorkoutSetSummary(
                sets.Sum(s => s.WeightKG ?? 0),
                sets.Sum(s => s.WeightLB ?? 0),
                sets.Sum(s => s.Repetitions),
                TimeSpan.FromSeconds(sets.Sum(s => s.Duration.TotalSeconds)),
                TimeSpan.FromSeconds(sets.Sum(s => s.RestTime.TotalSeconds)), Enumerable.Empty<Set>());

            return new WorkoutSummary(log.Date, max, min, avg, total, sets.Count(), log.Exercise.Id);
        }).ToArray();

        return summaries;
    }

    public Task DeleteExercise(Guid id)
    {
        return Delete<Exercise>(id, EndpointNames.ExercisePluralName);
    }

    private async Task<T> Get<T>(string endpoint)
        where T : EntityBase
    {
        var client = await GetAuthenticatedClient();
        var response = await client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            Context.LogError("Unable to fetch from '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
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
        var client = await GetAuthenticatedClient();
        var response = await client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            Context.LogError("Unable to fetch from '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataFetchException($"Failed to fetch {endpoint}. Reason: {response.StatusCode} - {response.ReasonPhrase}.");
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
    }

    private async Task Create<T>(T entity, string endpoint)
        where T : EntityBase
    {
        var client = await GetAuthenticatedClient();
        var content = JsonConvert.SerializeObject(entity);
        var response = await client.PostAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            Context.LogError("Unable to create record '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataPersistanceException($"Failed to create {endpoint} record. Reason: {response.StatusCode} - {response.ReasonPhrase}. {responseContent}");
        }
    }

    public async Task Delete<T>(Guid id, string endpoint)
        where T : EntityBase
    {
        var client = await GetAuthenticatedClient();
        var response = await client.DeleteAsync($"{endpoint}?id={id}");
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            Context.LogError("Unable to delete record '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataPersistanceException($"Failed to delete {id} record. Reason: {response.StatusCode} - {response.ReasonPhrase}. {responseContent}");
        }
    }

    public async Task<bool> UploadImage(IBrowserFile file, string imagePath)
    {
        if (file is null)
        {
            return true;
        }

        var fileContent = new StreamContent(file.OpenReadStream());

        var client = await GetAuthenticatedClient();
        using var content = new MultipartFormDataContent();

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(
            content: fileContent,
            name: imagePath,
            fileName: file.Name);

        var response = await client.PostAsync(EndpointNames.ExerciseImageUpload, content);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            Context.LogError("Unable to upload image '{Path}'. Server responded with {StatusCode} {Content}", imagePath, response.StatusCode, responseContent);
        }

        return response.IsSuccessStatusCode;
    }

    private async Task<HttpClient> GetAuthenticatedClient()
    {
        var tokenResult = await _accessTokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            // TODO: Ugly
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Value}");
        }

        return _client;
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
            Name = exercise.Name,
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
            Exercise = exercisesDictionary[logEntry.ExerciseId],
            Date = logEntry.Date,
            Sets = logEntry.Sets
        };
    }

    public Task UpdateProgram(WorkoutProgram program)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<WorkoutProgram>> GetWorkoutPrograms()
    {
        throw new NotImplementedException();
    }

    public Task DeleteWorkoutProgram(Guid id)
    {
        throw new NotImplementedException();
    }
}