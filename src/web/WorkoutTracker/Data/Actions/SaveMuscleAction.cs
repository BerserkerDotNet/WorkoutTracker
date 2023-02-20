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

        var isImageUploaded = await _repository.UploadImage(viewModel.ImageFile, model.ImagePath);
        if (!isImageUploaded)
        {
            throw new Exception("Not able to upload image. Aborting muscle update");
        }

        await _repository.UpdateMuscle(model);
        await dispatcher.Dispatch<FetchMusclesAction>();
        Context.ShowToast("Muscle updated.");
    }
}
