using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record GetCurrentProfile() : IRequest<Profile>;
public sealed class GetCurrentProfileHandler : IRequestHandler<GetCurrentProfile, Profile>
{
    private readonly UsersContainer _container;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<GetCurrentProfileHandler> _logger;

    public GetCurrentProfileHandler(UsersContainer container, IHttpContextAccessor contextAccessor, ILogger<GetCurrentProfileHandler> logger)
    {
        _container = container;
        _contextAccessor = contextAccessor;
        _logger = logger;
    }
    
    public ValueTask<Profile> Handle(GetCurrentProfile request, CancellationToken cancellationToken)
    {
        var email = _contextAccessor.HttpContext.User.Claims.Single(c=>c.Type == "preferred_username").Value;
        _logger.LogInformation($"Current user email is: {email}");
        
        var user = _container.GetItemLinqQueryable<Profile>(allowSynchronousQueryExecution: true)
            .Where(u => u.Email == email)
            .Take(1)
            .ToArray()
            .FirstOrDefault();

        return new ValueTask<Profile>(Task.FromResult(user));
    }
}

public sealed record SetCurrentWorkout(Guid WorkoutId) : ICommand<Profile>;

public sealed class SetCurrentWorkoutHandler : ICommandHandler<SetCurrentWorkout, Profile>
{
    private readonly IMediator _mediator;
    private readonly UsersContainer _container;
    private readonly ILogger<SetCurrentWorkoutHandler> _logger;

    public SetCurrentWorkoutHandler(IMediator mediator, UsersContainer container, ILogger<SetCurrentWorkoutHandler> logger)
    {
        _mediator = mediator;
        _container = container;
        _logger = logger;
    }
    
    public async ValueTask<Profile> Handle(SetCurrentWorkout command, CancellationToken cancellationToken)
    {
        var profile = await _mediator.Send(new GetCurrentProfile());
        profile.CurrentWorkout = command.WorkoutId;
        return await _container.UpsertEntity(profile, _logger);
    }
}