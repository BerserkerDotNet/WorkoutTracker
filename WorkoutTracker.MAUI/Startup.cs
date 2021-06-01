using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using WorkoutTracker.MAUI.Android;
using Xamarin.Android.Net;

namespace WorkoutTracker.MAUI
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .UseFormsCompatibility()
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureServices(services =>
                {
                    services.AddBlazorWebView();
                    var url = /*"http://192.168.0.19:7071/api/"*/  "https://workouttrackerfunctions.azurewebsites.net/api/";
                    services.AddHttpClient("api", cfg =>
                    {
                        cfg.BaseAddress = new Uri(url);
                        cfg.DefaultRequestHeaders.Add("x-functions-key", "tEtzNOPdCPqpWIakJBMf3gHKhVpX9cssTLT6O0rRygD1r3ohys0N7A==");
                    }).ConfigurePrimaryHttpMessageHandler(() => new AndroidClientHandler());

                    services.AddSingleton<IRepository, ApiRepository>();
                    services.AddSingleton<IExerciseLogRepository, ApiRepository>();
                    services.AddSingleton<AppState>();
                    services.AddSingleton<INotificationService, AndroidNotificationService>();
                });
        }
    }
}