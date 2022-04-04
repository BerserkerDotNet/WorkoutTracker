using Microsoft.AspNetCore.Components.Forms;

namespace WorkoutTracker.Data.Actions;

public record struct SaveMuscleModel(MuscleViewModel Model, IBrowserFile ImageFile);
public class SaveMuscleAction : IAsyncAction<SaveMuscleModel>
{
    private readonly IWorkoutRepository _repository;
    private readonly INotificationService _notificationService;

    public SaveMuscleAction(IWorkoutRepository repository, INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task Execute(IDispatcher dispatcher, SaveMuscleModel context)
    {
        await _repository.UpdateMuscle(context.Model, context.ImageFile);
        await dispatcher.Dispatch<FetchMusclesAction>();
        _notificationService.ShowToast("Muscle updated.");
    }
}
