using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Azure.WebJobs.Host;
using System.Threading;
using WorkoutTracker.Models;
using System.Text.Json;
using System.Text;
using System.Linq;

namespace WorkoutTracker.Functions;

public class AuthorizeAttribute : FunctionInvocationFilterAttribute
{
    public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {
        var request = (HttpRequest)executingContext.Arguments["request"];
        try
        {
            var idClaim = request.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            if (idClaim is null)
            {
                await WriteForbiddenResponse(request, $"Id claim is missing in the token.");
                return;
            }

            var id = idClaim.Value;
            var userContainer = await CosmosUtils.GetContainer<User>();

            // TODO: cache
            var itemResponse = await userContainer.ReadItemAsync<User>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
            if (itemResponse.StatusCode != System.Net.HttpStatusCode.OK || itemResponse.Resource is null)
            {
                await WriteForbiddenResponse(request, "User is not allowed in the app.");
                return;
            }

            await base.OnExecutingAsync(executingContext, cancellationToken);
        }
        catch (Exception ex)
        {
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
