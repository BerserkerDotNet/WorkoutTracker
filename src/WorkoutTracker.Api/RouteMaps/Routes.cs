using Mediator;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Api.Services;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.RouteMaps;

public static class Routes
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapGroup("/api")
            .MapApiRoutes()
            .RequireAuthorization();
    }

    public static RouteGroupBuilder MapApiRoutes(this RouteGroupBuilder group)
    {
        group.MapGroup("/exercises")
            .MapExerciseRoutes();

        group.MapGroup("/muscles")
            .MapMuscleRoutes();

        group.MapGroup("/exerciselogs")
            .MapExerciseLogRoutes();

        group.MapGroup("/workoutprograms")
            .MapProgramsRoutes();

        group.MapGroup("/profile")
            .MapUserRoutes();

        group.MapGet("/GetPreviousWorkoutStatsByExercise/{id:guid}", async (IMediator mediator, Guid id) => await mediator.Send(new GetPreviousWorkoutStatsByExercise(id)));
        group.MapPost("/UploadImage", async (IMediator mediator, IFormFileCollection files) =>
        {
            await mediator.Send(new UploadImage(files.First()));
            return Results.NoContent();
        }).DisableAntiforgery();

        return group;
    }

    public static RouteGroupBuilder MapExerciseRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) => await mediator.Send(new GetAllExercises()));
        group.MapPost("/", async (IMediator mediator, ExerciseViewModel model) => await mediator.Send(new CreateOrUpdateExercise(model)));
        group.MapDelete("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            await mediator.Send(new DeleteExerciseById(id));
            return Results.NoContent();
        });

        return group;
    }

    public static RouteGroupBuilder MapMuscleRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) => await mediator.Send(new GetAllMuscles()));
        group.MapPost("/", async (IMediator mediator, MuscleViewModel model) => await mediator.Send(new CreateOrUpdateMuscle(model)));
        group.MapDelete("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            await mediator.Send(new DeleteMuscleById(id));
            return Results.NoContent();
        });

        return group;
    }

    public static RouteGroupBuilder MapExerciseLogRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (IMediator mediator, Guid id) => await mediator.Send(new GetExerciseLogById(id)));
        group.MapGet("/{date:datetime}", async (IMediator mediator, DateTime date) => await mediator.Send(new GetExerciseLogByDateRange(date, date.AddDays(1))));
        group.MapGet("/", async (IMediator mediator, [FromQuery] DateTime from, [FromQuery] DateTime to) => await mediator.Send(new GetExerciseLogByDateRange(from, to)));
        group.MapPost("/", async (IMediator mediator, LogEntryViewModel model) => await mediator.Send(new CreateOrUpdateLog(model)));
        group.MapDelete("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            await mediator.Send(new DeleteLogById(id));
            return Results.NoContent();
        });

        return group;
    }

    public static RouteGroupBuilder MapProgramsRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) => await mediator.Send(new GetAllWorkoutPrograms()));
        group.MapPost("/", async (IMediator mediator, WorkoutProgram model) => await mediator.Send(new CreateOrUpdateWorkoutProgram(model)));
        group.MapDelete("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            await mediator.Send(new DeleteWorkoutProgramById(id));
            return Results.NoContent();
        });
        return group;
    }

    public static RouteGroupBuilder MapUserRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) => await mediator.Send(new GetCurrentProfile()));
        group.MapPost("/setCurrentWorkout", async (IMediator mediator, SetCurrentWorkout command) => await mediator.Send(command));
        return group;
    }
}