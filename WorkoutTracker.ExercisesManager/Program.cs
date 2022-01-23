using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkoutTracker.Models;

// TODO: Build muscles list
// TODO: Update exercises list to reflect muscles correctly

//await UpdateMuscleDb();
await UpdateExerciseDb();

static async Task UpdateExerciseDb()
{
    const string exercisesFileName = @"exercises.json";
    const string musclesFileName = @"muscles.json";

    var exercisesString = await File.ReadAllTextAsync(Path.Combine("Assets", exercisesFileName));
    var musclesString = await File.ReadAllTextAsync(Path.Combine("Assets", musclesFileName));

    var muscles = JsonConvert.DeserializeObject<Muscle[]>(musclesString);
    var exercises = JsonConvert.DeserializeObject<Exercise[]>(exercisesString);
    var distinctExercises = new HashSet<Guid>();

    foreach (var exercise in exercises)
    {
        if (distinctExercises.Contains(exercise.Id))
        {
            throw new Exception("Duplicate exercise ID detected");
        }

        distinctExercises.Add(exercise.Id);
    }

    var exercisesWithoutMuscleGroups = exercises.Where(e => e.Muscles.Length == 0 || e.Muscles.Any(m => !muscles.Any(mu => mu.Id == m)));
    if (exercisesWithoutMuscleGroups.Any())
    {
        throw new Exception("Invalid muscle groups detected");
    }

    foreach (var exercise in exercises)
    {
        Console.WriteLine($"Processing exercise '{exercise.Name}'");
        var imagePath = $"exercises/{exercise.Name.Replace(" ", "_")}.jpg";
        if (!File.Exists(Path.Combine("Assets", imagePath)))
        {
            Console.WriteLine($"Image not found for '{exercise.Name}'");
            continue;
        }

        exercise.ImagePath = imagePath;

        using (var client = new HttpClient())
        {

            var json = JsonConvert.SerializeObject(exercise);
            var response = await client.PostAsync("http://localhost:7071/api/Exercises", new StringContent(json, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Response does not indicate success. {response.StatusCode} {response.ReasonPhrase}");
            }
        }
    }

}


static async Task UpdateMuscleDb()
{
    const string musclesFileName = @"muscles.json";

    var musclesString = await File.ReadAllTextAsync(Path.Combine("Assets", musclesFileName));

    var muscles = JsonConvert.DeserializeObject<Muscle[]>(musclesString);
    var distinctMuscles = new HashSet<Guid>();

    foreach (var muscle in muscles)
    {
        if (distinctMuscles.Contains(muscle.Id))
        {
            throw new Exception("Duplicate muscle ID detected");
        }

        distinctMuscles.Add(muscle.Id);
    }

    foreach (var muscle in muscles)
    {
        Console.WriteLine($"Processing muscle '{muscle.Name}'");
        var imagePath = Path.Combine("Assets", $"{muscle.ImagePath}").Replace(" ", "_");
        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Image not found for '{muscle.Name}'");
            continue;
        }

        muscle.ImagePath = muscle.ImagePath.Replace(" ", "_");

        using (var client = new HttpClient())
        {
            var json = JsonConvert.SerializeObject(muscle);
            var response = await client.PostAsync("http://localhost:7071/api/Muscles", new StringContent(json, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Response does not indicate success. {response.StatusCode} {response.ReasonPhrase}");
            }
        }
    }

}

static async Task TestExerciseLogsUpsert() 
{
    var log = new ExerciseLogEntry
    {
        Date = DateTime.UtcNow,
        Id = Guid.NewGuid(),
        ExerciseId = Guid.NewGuid(),
        Sets = new Set[] { }
    };

    using (var client = new HttpClient())
    {
        var json = JsonConvert.SerializeObject(log);
        var response = await client.PostAsync("http://localhost:7071/api/ExerciseLog", new StringContent(json, Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Response does not indicate success. {response.StatusCode} {response.ReasonPhrase}");
        }

        var logUpdate = new ExerciseLogEntry
        {
            Id = log.Id,
            Sets = new Set[] 
            {
                new Set 
                {
                    CompletionTime = DateTime.UtcNow,
                    Duration = TimeSpan.FromSeconds(60),
                    Note = "Test",
                    Repetitions = 10,
                    RestTime = TimeSpan.FromSeconds(20),
                    Weight = 10
                }
            }
        };

        var response2 = await client.PostAsync("http://localhost:7071/api/ExerciseLog", new StringContent(JsonConvert.SerializeObject(logUpdate), Encoding.UTF8, "application/json"));
        if (!response2.IsSuccessStatusCode)
        {
            Console.WriteLine($"Response does not indicate success. {response.StatusCode} {response.ReasonPhrase}");
        }

        var logUpdate2 = new ExerciseLogEntry
        {
            Id = log.Id,
            Sets = new Set[]
            {
                new Set
                {
                    CompletionTime = DateTime.UtcNow,
                    Duration = TimeSpan.FromSeconds(45),
                    Note = "Test 2",
                    Repetitions = 5,
                    RestTime = TimeSpan.FromSeconds(10),
                    Weight = 5
                }
            }
        };

        var response3 = await client.PostAsync("http://localhost:7071/api/ExerciseLog", new StringContent(JsonConvert.SerializeObject(logUpdate2), Encoding.UTF8, "application/json"));
        if (!response3.IsSuccessStatusCode)
        {
            Console.WriteLine($"Response does not indicate success. {response.StatusCode} {response.ReasonPhrase}");
        }
    }
}