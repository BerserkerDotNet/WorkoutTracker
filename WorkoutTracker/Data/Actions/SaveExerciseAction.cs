namespace WorkoutTracker.Data.Actions;

public class SaveExerciseAction : TrackableAction<EditExerciseViewModel>
{
    private readonly IWorkoutRepository _repository;

    public SaveExerciseAction(IWorkoutRepository repository, ApplicationContext<SaveExerciseAction> context)
        : base(context)
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, EditExerciseViewModel exercise, Dictionary<string, string> trackableProperties)
    {
        trackableProperties.Add(nameof(exercise.Id), exercise.Id.ToString());
        trackableProperties.Add(nameof(exercise.Name), exercise.Name);

        await _repository.UpdateExercise(exercise);
        await dispatcher.Dispatch<ReFetchExercisesAction>();
        Context.ShowToast("Exercise updated.");
    }
}