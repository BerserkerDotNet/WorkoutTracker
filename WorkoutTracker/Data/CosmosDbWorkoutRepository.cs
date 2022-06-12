using Mapster;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WorkoutTracker.Exceptions;
using WorkoutTracker.Models;

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

        var musclesDictionary = muscles.ToDictionary(k => k.Id, m => m);
        return exerciseDtos.Select(e => MapExercise(e, musclesDictionary)).ToArray();
    }

    public async Task<IEnumerable<MuscleViewModel>> GetMuscles()
    {
        var muscleDtos = await GetMultiple<Muscle>(EndpointNames.MusclePluralName);
        return muscleDtos.Select(MapMuscle).ToArray();
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

    public virtual async Task UpdateExercise(EditExerciseViewModel exercise)
    {
        var isImageUploaded = await UploadImage(exercise.ImageFile, exercise.ImagePath);
        if (!isImageUploaded) 
        {
            throw new Exception("Not able to upload image. Aborting exercise update");
        }

        var exerciseDto = exercise.Adapt<Exercise>();

        await Create(exerciseDto, EndpointNames.ExercisePluralName); // Works by upserting the record
    }

    public async Task UpdateMuscle(MuscleViewModel muscle, IBrowserFile imageFile)
    {
        var isImageUploaded = await UploadImage(imageFile, muscle.ImagePath);
        if (!isImageUploaded)
        {
            throw new Exception("Not able to upload image. Aborting muscle update");
        }

        var muscleDto = muscle.Adapt<Muscle>();
        await Create(muscleDto, EndpointNames.MusclePluralName); // Works by upserting the record
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

    private async Task<bool> UploadImage(IBrowserFile file, string imagePath) 
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