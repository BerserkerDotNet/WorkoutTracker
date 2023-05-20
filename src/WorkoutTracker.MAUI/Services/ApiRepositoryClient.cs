using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using UnitsNet;
using WorkoutTracker.MAUI.Exceptions;
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;

namespace WorkoutTracker.MAUI.Services;

public class ApiRepositoryClient : IWorkoutRepository
{
    private static readonly JsonSerializerOptions _options;
    private readonly HttpClient _client;
    private readonly ApplicationContext<ApiRepositoryClient> _context;

    static ApiRepositoryClient()
    {
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _options.Converters.Add(new IExerciseSetPolymorphicJsonConverter());
    }

    public ApiRepositoryClient(HttpClient client, ApplicationContext<ApiRepositoryClient> context)
    {
        _client = client;
        _context = context;
    }

    public Task AddLogRecord(LogEntryViewModel model)
    {
        return Create(model, "exerciselogs");
    }

    public Task DeleteExercise(Guid id)
    {
        return Delete(id, EndpointNames.ExercisePluralName);
    }

    public Task DeleteLog(Guid id)
    {
        return Delete(id, "exerciselogs");
    }

    public Task UpdateExercise(ExerciseViewModel exercise)
    {
        return Create(exercise, EndpointNames.ExercisePluralName);
    }

    public Task UpdateMuscle(MuscleViewModel muscle)
    {
        return Create(muscle, EndpointNames.MusclePluralName);
    }

    public Task UpdateProgram(WorkoutProgram program)
    {
        return Create(program, "workoutprograms");
    }

    public Task<Profile> SetCurrentWorkoutProgram(Guid programId)
    {
        return Create<Profile>(new { WorkoutId = programId }, "profile/setCurrentWorkout");
    }

    public Task<IEnumerable<WorkoutProgram>> GetWorkoutPrograms()
    {
        return GetMultiple<WorkoutProgram>("workoutprograms");
    }

    public Task<Profile> GetProfile()
    {
        return Get<Profile>("profile");
    }

    public Task DeleteWorkoutProgram(Guid id)
    {
        return Delete(id, "workoutprograms");
    }

    public Task<IEnumerable<ExerciseViewModel>> GetExercises()
    {
        return GetMultiple<ExerciseViewModel>(EndpointNames.ExercisePluralName);
    }

    public Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date)
    {
        var dateString = date.ToString("O");
        return GetMultiple<LogEntryViewModel>($"exerciselogs/{dateString}");
    }

    public Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime from, DateTime to)
    {
        var fromString = from.ToString("O");
        var toString = to.ToString("O");
        return GetMultiple<LogEntryViewModel>($"exerciselogs?from={fromString}&to={toString}"); // TODO, make a parameter on a Get function
    }

    public Task<IEnumerable<MuscleViewModel>> GetMuscles()
    {
        return GetMultiple<MuscleViewModel>(EndpointNames.MusclePluralName);
    }

    public Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId)
    {
        return Get<LogEntryViewModel>($"{EndpointNames.GetPreviousWorkoutStatsByExercise}?exerciseId={exerciseId}");
    }

    public async Task<IEnumerable<WorkoutSummary>> GetWorkoutSummaries(DateTime from, DateTime to)
    {
        var logs = await GetLogs(from.ToUniversalTime(), to.ToUniversalTime());
        var summaries = logs.Where(log => log.Sets.OfType<LegacySet>().Any()).Select(log =>
        {
            var sets = log.Sets.OfType<LegacySet>();
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
                TimeSpan.FromSeconds(sets.Sum(s => s.RestTime.TotalSeconds)), Enumerable.Empty<IExerciseSet>());

            return new WorkoutSummary(log.Date, max, min, avg, total, sets.Count(), log.Exercise.Id);
        }).ToArray();

        return summaries;
    }

    public async Task<bool> UploadImage(FileResult file, string imagePath)
    {
        if (file is null)
        {
            return true;
        }
        var stream = await file.OpenReadAsync();
        var fileContent = new StreamContent(stream);

        using var content = new MultipartFormDataContent();

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(
            content: fileContent,
            name: imagePath,
            fileName: file.FileName);

        var response = await _client.PostAsync("UploadImage", content);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            _context.LogError("Unable to upload image '{Path}'. Server responded with {StatusCode} {Content}", imagePath, response.StatusCode, responseContent);
        }

        return response.IsSuccessStatusCode;
    }

    private async Task<IEnumerable<T>> GetMultiple<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            _context.LogError("Unable to fetch from '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataFetchException($"Failed to fetch {endpoint}. Reason: {response.StatusCode} - {response.ReasonPhrase}.");
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<T>>(content, _options);
    }

    private async Task<T> Get<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            _context.LogError("Unable to fetch from '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataFetchException($"Failed to fetch {endpoint}. Reason: {response.StatusCode} - {response.ReasonPhrase}.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return default;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, _options);
    }

    private async Task Delete(Guid id, string endpoint)
    {
        var response = await _client.DeleteAsync($"{endpoint}/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            _context.LogError("Unable to delete record '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataPersistanceException($"Failed to delete {id} record. Reason: {response.StatusCode} - {response.ReasonPhrase}. {responseContent}");
        }
    }

    private async Task<TReturn> Create<TReturn>(object entity, string endpoint)
    {
        var content = JsonSerializer.Serialize(entity, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var response = await _client.PostAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync();
            _context.LogError("Unable to create record '{Path}'. Server responded with {StatusCode} {Content}", endpoint, response.StatusCode, responseContent);
            throw new DataPersistanceException($"Failed to create {endpoint} record. Reason: {response.StatusCode} - {response.ReasonPhrase}. {responseContent}");
        }

        return await response.Content.ReadFromJsonAsync<TReturn>();
    }

    private Task<T> Create<T>(T entity, string endpoint)
    {
        return Create<T>((object)entity, endpoint);
    }
}