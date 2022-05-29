﻿using BlazorStorage.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Web;

public class LocalStorageCacheService : ICacheService
{
    private const string ExercisesKey = "WT_EXERCISES";
    private const string TokenKey = "WT_TOKEN";
    private readonly ILocalStorage _storage;

    public LocalStorageCacheService(ILocalStorage storage)
    {
        _storage = storage;
    }

    public async Task<IEnumerable<ExerciseViewModel>> GetExercises()
    {
        return await _storage.GetItem<IEnumerable<ExerciseViewModel>>(ExercisesKey);
    }

    public async Task<bool> IsExercisesCached()
    {
        var exercises = await GetExercises();
        return exercises is object;
    }

    public async Task ResetExercisesCache()
    {
        await _storage.RemoveItem(ExercisesKey);
    }

    public async Task SaveExercises(IEnumerable<ExerciseViewModel> exercises)
    {
        await _storage.SetItem(ExercisesKey, exercises);
    }

    public async Task<AccessToken> GetToken()
    {
        return await _storage.GetItem<AccessToken>(TokenKey);
    }

    public async Task SaveToken(AccessToken token)
    {
        await _storage.SetItem(TokenKey, token);
    }
}