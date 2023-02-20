using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Azure.WebJobs.Host;
using System.Threading;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Functions;

public class AuthorizeAttribute : FunctionInvocationFilterAttribute
{
    public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {
        var logger = executingContext.Logger;
        var request = (HttpRequest)executingContext.Arguments["request"];

        if (string.Equals(request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Skipping auth for localhost");
            return;
        }

        try
        {
            var idClaim = request.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            if (idClaim is null)
            {
                logger.LogWarning("Id claim is missing in the token.");
                await WriteForbiddenResponse(request,"Id claim is missing in the token.");
                return;
            }

            var id = idClaim.Value;
            var userContainer = await CosmosUtils.GetContainer<User>();

            // TODO: cache
            var itemResponse = await userContainer.ReadItemAsync<User>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
            if (itemResponse.StatusCode != System.Net.HttpStatusCode.OK || itemResponse.Resource is null)
            {
                logger.LogWarning("User is not allowed in the app. Response code '{StatusCode}'", itemResponse.StatusCode);
                await WriteForbiddenResponse(request, "User is not allowed in the app.");
                return;
            }

            logger.LogInformation("User '{User}' is authenticated.", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error authenticating user. {Message}", ex.Message);

            request.HttpContext.Response.StatusCode = 500;
            await request.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(ex.Message));
            await request.HttpContext.Response.Body.FlushAsync();
            request.HttpContext.Response.Body.Close();
        }
    }

    private async Task WriteForbiddenResponse(HttpRequest request, string message)
    {
        var errorResponse = new ErrorResponse("Forbidden", message);
        var responseJson = JsonSerializer.Serialize(errorResponse);

        request.HttpContext.Response.StatusCode = 403;
        await request.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(responseJson));
        await request.HttpContext.Response.Body.FlushAsync();
        request.HttpContext.Response.Body.Close();
    }
}

public record ErrorResponse(string StatusCode, string Message);
