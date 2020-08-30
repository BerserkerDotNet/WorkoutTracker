using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using WorkoutTracker.Models;

const string exercisesFileName = @"exercises.csv";

var lines = await File.ReadAllLinesAsync(Path.Combine("Assets", exercisesFileName));

foreach (var line in lines)
{
    var properties = line.Split(",", StringSplitOptions.RemoveEmptyEntries);

    Console.WriteLine($"Processing '{properties[1]}'");
    var imagePath = Path.Combine("Assets", $"{properties[1]}.jpg");
    if (!File.Exists(imagePath))
    {
        Console.WriteLine($"Image not found for '{properties[1]}'");
        continue;
    }

    var imageBytes = await File.ReadAllBytesAsync(imagePath);

    var exercise = new Exercise
    {
        Id = Guid.Parse(properties[0]),
        Name = properties[1],
        Icon = imageBytes,
        Muscles = properties[2]
    };

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

