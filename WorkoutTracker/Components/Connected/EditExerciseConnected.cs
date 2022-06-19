using Mapster;
using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class EditExerciseConnected : SafeConnectedComponent<EditExercise, RootState, EditExerciseProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    [Parameter]
    [EditorRequired]
    public Guid ExerciseId { get; set; }

    protected override void MapStateToPropsSafe(RootState state, EditExerciseProps props)
    {
        var exercise = state.SelectExerciseById(ExerciseId);
        if (exercise is null)
        {
            props.Exercise = new EditExerciseViewModel
            {
                Id = ExerciseId,
                Muscles = Enumerable.Empty<MuscleViewModel>(),
                Tags = Enumerable.Empty<string>()
            };
        }
        else
        {
            props.Exercise = exercise.Adapt<EditExerciseViewModel>();
        }
        props.Muscles = state.SelectMuscles();
        props.Tags = state.SelectTags();
    }

    protected override void MapDispatchToProps(IStore<RootState> store, EditExerciseProps props)
    {
        props.Save = CallbackAsync<EditExerciseViewModel>(async exercise =>
        {
            await store.Dispatch<SaveExerciseAction, EditExerciseViewModel>(exercise);
            Navigation.NavigateTo($"/library");
        });

        props.Cancel = Callback(() => Navigation.NavigateTo($"/library"));
    }

    protected override async Task Init(IStore<RootState> store)
    {
        if (!store.State.SelectMuscles().Any())
        {
            await store.Dispatch<FetchMusclesAction>();
        }
    }
}
