﻿using Mediator;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Api.Services;

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

        group.MapGet("/GetPreviousWorkoutStatsByExercise/{id:guid}", async (IMediator mediator, Guid id) => await mediator.Send(new GetPreviousWorkoutStatsByExercise(id)));
        group.MapPost("/ExerciseImage", async (IMediator mediator, IFormFileCollection files) =>
        {
            await mediator.Send(new UploadExerciseImage(files.First()));
            return Results.NoContent();
        });

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
}