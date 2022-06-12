using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using System.Reflection;
using WorkoutTracker.Extensions;
using WorkoutTracker.MAUI.Android;
using Xamarin.Android.Net;

namespace WorkoutTracker.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureEssentials()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });


            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("WorkoutTracker.MAUI.appsettings.json");

            var config = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();

            builder.Configuration.AddConfiguration(config);

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddWorkoutTracker(config, cfg =>
            {
                cfg.WithCacheService<AndroidCacheService>();
                cfg.WithMessageHandler<AndroidMessageHandler>();
                cfg.WithConfigurationService<LocalConfigurationService>();
                cfg.WithAuthenticationRedirectUrl("https://0.0.0.0");
            });

            builder.Services.AddRemoteAuthentication<RemoteAuthenticationState, RemoteUserAccount, EmptyOptions>();

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IRemoteAuthenticationService<RemoteAuthenticationState>>(s => s.GetService<AuthService>());
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetService<AuthService>());
            builder.Services.AddScoped<IAccessTokenProvider>(s => s.GetService<AuthService>());

            return builder.Build();
        }
    }
}