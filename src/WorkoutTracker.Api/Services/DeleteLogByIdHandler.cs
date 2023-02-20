using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record DeleteLogById(Guid Id) : ICommand;

public sealed class DeleteLogByIdHandler : ICommandHandler<DeleteLogById>
{
    private readonly ExerciseLogsContainer _container;
    private readonly ILogger<DeleteExerciseByIdHandler> _logger;

    public DeleteLogByIdHandler(ExerciseLogsContainer container, ILogger<DeleteExerciseByIdHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DeleteLogById command, CancellationToken cancellationToken)
    {
        await _container.DeleteEntity<ExerciseLogEntry>(command.Id, _logger);
        return Unit.Value;
    }
}

public sealed record DeleteWorkoutProgramById(Guid Id) : ICommand;

public sealed class DeleteWorkoutProgramByIdHandler : ICommandHandler<DeleteWorkoutProgramById>
{
    private readonly WorkoutProgramsContainer _container;
    private readonly ILogger<DeleteWorkoutProgramByIdHandler> _logger;

    public DeleteWorkoutProgramByIdHandler(WorkoutProgramsContainer container, ILogger<DeleteWorkoutProgramByIdHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DeleteWorkoutProgramById command, CancellationToken cancellationToken)
    {
        await _container.DeleteEntity<WorkoutProgram>(command.Id, _logger);
        return Unit.Value;
    }
}

public sealed record UploadImage(IFormFile File) : ICommand;

public sealed class UploadmageHandler : ICommandHandler<UploadImage>
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<UploadmageHandler> _logger;

    public UploadmageHandler(BlobContainerClient containerClient, ILogger<UploadmageHandler> logger)
    {
        _containerClient = containerClient;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(UploadImage command, CancellationToken cancellationToken)
    {
        var file = command.File;
        _logger.LogInformation("Uploading {File} of size {FileSize} to blob", file.FileName, file.Length);

        try
        {
            using var fileStream = file.OpenReadStream();
            var blobClient = _containerClient.GetBlobClient(file.Name);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
        }
        catch (RequestFailedException e)
        {
            _logger.LogError(e, "Failed to upload file with message: {Message}", e.Message);
            throw;
        }


        return Unit.Value;
    }
}
