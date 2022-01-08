using BlazorState.Redux.Extensions;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using WorkoutTracker.MAUI.Android;
using WorkoutTracker.MAUI.Components;
using WorkoutTracker.MAUI.Components.Connected;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.MAUI.Data.Reducers;
using Xamarin.Android.Net;

namespace WorkoutTracker.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddBlazorWebView();
            builder.Services.AddHttpClient("api", (services, cfg) =>
            {
                var configurationService = services.GetService<IConfigurationService>();
                var config = configurationService.GetEndpointConfiguration();
                cfg.BaseAddress = new Uri(config.Url);
                cfg.DefaultRequestHeaders.Add("x-functions-key", config.Secret);
            }).ConfigurePrimaryHttpMessageHandler(() => new AndroidClientHandler());

            builder.Services.AddSingleton<IRepository, ApiRepository>();
            builder.Services.AddSingleton<IConfigurationService, LocalConfigurationService>();
            builder.Services.AddSingleton<INotificationService, AndroidNotificationService>();
            builder.Services.AddSingleton<ICacheService, AndroidCacheService>();
            builder.Services.AddSharpCss(cfg =>
            {
                cfg.RegisterStylesModule<EditExerciseLogStylesModule>();
                cfg.RegisterStylesModule<FieldSetItemStylesModule>();
            });
            builder.Services.AddReduxStore<RootState>(cfg =>
            {
                cfg.RegisterActionsFromAssemblyContaining<FetchExercisesAction>();
                cfg.Map<ExercisesReducer, ExercisesState>(s => s.Exercises);
            });

            builder.Services.AddScoped<Radzen.DialogService>();

            return builder.Build();
        }
    }
}