using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Azure.WebJobs.Host;
using System.Threading;

namespace WorkoutTracker.Functions;

public class AuthorizeAttribute : FunctionInvocationFilterAttribute
{
    public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {
        var request = (HttpRequest)executingContext.Arguments["request"];
        var allowedUser = Environment.GetEnvironmentVariable("AllowedUser"); // TODO: Temporary
        var email = request.HttpContext.User.FindFirst("preferred_username");
        if (email is null || !string.Equals(email.Value, allowedUser, StringComparison.OrdinalIgnoreCase)) 
        {
            request.HttpContext.Response.StatusCode = 401;
            await request.HttpContext.Response.Body.FlushAsync();
            request.HttpContext.Response.Body.Close();
            return;
        }

        await base.OnExecutingAsync(executingContext, cancellationToken);
    }
}
