using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class EditMuscleFormConnected : ConnectedComponent<EditMuscleForm, RootState, EditMuscleFormProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    [Parameter]
    [EditorRequired]
    public Guid MuscleId { get; set; }

    protected override void MapStateToProps(RootState state, EditMuscleFormProps props)
    {
        props.Muscle = state.SelectMuscleById(MuscleId);
    }

    protected override void MapDispatchToProps(IStore<RootState> store, EditMuscleFormProps props)
    {
        props.Cancel = Callback(() => Navigation.NavigateTo("/muscles"));
        props.Save = CallbackAsync<SaveMuscleModel>(async saveModel =>
        {
            await store.Dispatch<SaveMuscleAction, SaveMuscleModel>(saveModel);
            Navigation.NavigateTo("/muscles");
        });
    }
}
