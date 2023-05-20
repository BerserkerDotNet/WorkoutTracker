using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.MAUI.Views;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.Extensions;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder AddViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<WorkoutViewModel>();
        builder.Services.AddSingleton<WorkoutProgramsViewModel>();
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<EditExercisePageViewModel>();
        builder.Services.AddSingleton<EditWorkoutProgramViewModel>();
        builder.Services.AddSingleton<EditWorkoutDefinitionViewModel>();
        return builder;
    }

    public static MauiAppBuilder AddViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<Workout>();
        builder.Services.AddSingleton<Library>();
        builder.Services.AddSingleton<WorkoutPrograms>();
        builder.Services.AddSingleton<EditExercisePage>();
        builder.Services.AddSingleton<EditWorkoutProgram>();
        builder.Services.AddSingleton<EditWorkoutDefinition>();
        return builder;
    }
}