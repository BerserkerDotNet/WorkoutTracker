using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.RouteMaps;
using WorkoutTracker.Api.Utils;

Mappings.Configure();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediator();
builder.Services.AddCors();

var dbEndpoint = builder.Configuration.GetValue<string>("DbEndpoint");
var cosmosClient = new CosmosClient(dbEndpoint, new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true }));

Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("WorkoutTrackerData");

var exerciseContainer = await ExercisesContainer.Create(database);
builder.Services.AddSingleton(exerciseContainer);

var musclesContainer = await MusclesContainer.Create(database);
builder.Services.AddSingleton(musclesContainer);

var logEntriesContainer = await ExerciseLogsContainer.Create(database);
builder.Services.AddSingleton(logEntriesContainer);

var accountName = builder.Configuration.GetValue<string>("CDNAccountName");
var containerEndpoint = string.Format("https://{0}.blob.core.windows.net/images", accountName);
var containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true }));
await containerClient.CreateIfNotExistsAsync();

builder.Services.AddSingleton(containerClient);

var app = builder.Build();

app.UseCors(config =>
{
    config.AllowAnyOrigin();
});
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapRoutes();

app.Run();