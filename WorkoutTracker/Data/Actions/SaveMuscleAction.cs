using Microsoft.AspNetCore.Components.Forms;

namespace WorkoutTracker.Data.Actions;

public record struct SaveMuscleModel(MuscleViewModel Model, IBrowserFile ImageFile);

public class SaveMuscleAction : TrackableAction<SaveMuscleModel>
{
    private readonly IWorkoutRepository _repository;

    public SaveMuscleAction(IWorkoutRepository repository, ApplicationContext<SaveMuscleAction> context)
        : base(context)
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, SaveMuscleModel viewModel, Dictionary<string, string> trackableProperties)
    {
        var model = viewModel.Model;
        trackableProperties.Add(nameof(model.Id), model.Id.ToString());
        trackableProperties.Add(nameof(model.Name), model.Name);
        
        await _repository.UpdateMuscle(model, viewModel.ImageFile);
        await dispatcher.Dispatch<FetchMusclesAction>();
        Context.ShowToast("Muscle updated.");
    }
}
