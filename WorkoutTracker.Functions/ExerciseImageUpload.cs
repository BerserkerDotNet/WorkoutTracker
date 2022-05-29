using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Models;
using Azure.Storage.Blobs;
using Azure.Identity;
using System;
using Azure;
using Azure.Storage.Blobs.Models;

namespace WorkoutTracker.Functions;

public static class ExerciseImageUpload
{
    [Authorize]
    [FunctionName(EndpointNames.ExerciseImageUpload)]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request, ILogger log)
    {
        var file = request.Form.Files[0];
        log.LogInformation("Uploading {File} of size {FileSize} to blob", file.FileName, file.Length);

        var accountName = Environment.GetEnvironmentVariable("ContentAccount");
        var containerEndpoint = string.Format("https://{0}.blob.core.windows.net/images", accountName);
        var containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());

        try
        {
            await containerClient.CreateIfNotExistsAsync();

            using var fileStream = file.OpenReadStream();
            var blobClient = containerClient.GetBlobClient(file.Name);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
        }
        catch (RequestFailedException e)
        {
            log.LogError(e, "Failed to upload file with message: {Message}", e.Message);
            throw;
        }

        return new OkResult();
    }
}
