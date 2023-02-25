using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.Services;

public class CachedWorkoutRepositoryDecorator : IWorkoutRepository
{
    const string ExercisesKey = "exercises";
    const string MusclesKey = "muscles";
    const string WorkoutSummariesKey = "WorkoutSummaries";
    const string WorkoutLogKey = "WorkoutLog";

    private readonly IWorkoutRepository _decoratedRepository;
    private readonly ICacheService _cacheService;
    private readonly ApplicationContext<CachedWorkoutRepositoryDecorator> _context;

    public CachedWorkoutRepositoryDecorator(IWorkoutRepository decoratedRepository, ICacheService cacheService, ApplicationContext<CachedWorkoutRepositoryDecorator> context)
    {
        _decoratedRepository = decoratedRepository;
        _cacheService = cacheService;
        _context = context;
    }

    public async Task<IEnumerable<ExerciseViewModel>> GetExercises()
    {
        if (await _cacheService.HasKey(ExercisesKey))
        {
            return await _cacheService.GetAsync<IEnumerable<ExerciseViewModel>>(ExercisesKey);
        }

        _context.LogInformation($"Cache miss for {nameof(GetExercises)}");
        var exercises = await _decoratedRepository.GetExercises();
        await _cacheService.SetAsync(ExercisesKey, exercises);

        return exercises;
    }

    public async Task<IEnumerable<MuscleViewModel>> GetMuscles()
    {
        if (await _cacheService.HasKey(MusclesKey))
        {
            return await _cacheService.GetAsync<IEnumerable<MuscleViewModel>>(MusclesKey);
        }

        _context.LogInformation($"Cache miss for {nameof(GetMuscles)}");
        var muscles = await _decoratedRepository.GetMuscles();
        await _cacheService.SetAsync(MusclesKey, muscles);

        return muscles;
    }

    public async Task<IEnumerable<WorkoutSummary>> GetWorkoutSummaries(DateTime from, DateTime to)
    {
        var isCached = await _cacheService.HasKey(WorkoutSummariesKey);
        var summaries = Enumerable.Empty<WorkoutSummary>();
        if (isCached)
        {
            summaries = await _cacheService.GetAsync<IEnumerable<WorkoutSummary>>(WorkoutSummariesKey);
            from = summaries.MaxBy(s => s.Date).Date.Date.AddDays(1); // TODO: Store date with summaries
        }

        var newSummaries = await _decoratedRepository.GetWorkoutSummaries(from, to);

        summaries = summaries.Union(newSummaries).ToArray();
        await _cacheService.SetAsync(WorkoutSummariesKey, summaries);

        return summaries;
    }

    public async Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date)
    {
        var isCached = await _cacheService.HasKey(WorkoutLogKey);
        var logCache = isCached ? await _cacheService.GetAsync<Dictionary<DateTime, IEnumerable<LogEntryViewModel>>>(WorkoutLogKey) : new Dictionary<DateTime, IEnumerable<LogEntryViewModel>>();

        if (logCache.ContainsKey(date))
        {
            return logCache[date];
        }

        _context.LogInformation($"Cache miss for {nameof(GetLogs)}");
        var logs = await _decoratedRepository.GetLogs(date);
        logCache[date] = logs;

        await _cacheService.SetAsync(WorkoutLogKey, logCache);

        return logs;
    }

    public Task AddLogRecord(LogEntryViewModel model) => _decoratedRepository.AddLogRecord(model);

    public Task DeleteExercise(Guid id) => _decoratedRepository.DeleteExercise(id);

    public Task DeleteLog(Guid id) => _decoratedRepository.DeleteLog(id);

    public Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime from, DateTime to) => _decoratedRepository.GetLogs(from, to);

    public Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId) => _decoratedRepository.GetPreviousWorkoutStatsBy(exerciseId);

    public Task UpdateExercise(ExerciseViewModel exercise) => _decoratedRepository.UpdateExercise(exercise);

    public Task UpdateMuscle(MuscleViewModel muscle) => _decoratedRepository.UpdateMuscle(muscle);

    public Task<bool> UploadImage(FileResult file, string imagePath) => _decoratedRepository.UploadImage(file, imagePath);

    public Task UpdateProgram(WorkoutProgram program) => _decoratedRepository.UpdateProgram(program);

    public Task<IEnumerable<WorkoutProgram>> GetWorkoutPrograms() => _decoratedRepository.GetWorkoutPrograms();

    public Task DeleteWorkoutProgram(Guid id) => _decoratedRepository.DeleteWorkoutProgram(id);
}