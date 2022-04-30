using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class ExerciseProfileSelectorConnected : ConnectedComponent<ExerciseProfileSelector, RootState, ExerciseProfileSelectorProps>
{
    protected override void MapStateToProps(RootState state, ExerciseProfileSelectorProps props)
    {
        props.SelectedProfile = state.SelectCurrentProfile();
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ExerciseProfileSelectorProps props)
    {
        props.GenerateSchedule = CallbackAsync(async () => await GenerateSchedule(store, props.SelectedProfile));
        props.SetProfile = CallbackAsync<ExerciseProfile>(async profile => await SetProfile(store, profile));
    }

    private async Task SetProfile(IStore<RootState> store, ExerciseProfile profile) 
    {
        store.Dispatch(new ExerciseProfileSelected(profile));
        await GenerateSchedule(store, profile);
    }

    private async Task GenerateSchedule(IStore<RootState> store, ExerciseProfile profile)
    {
        await store.Dispatch<BuildExerciseScheduleAction, ExerciseProfile>(profile);
    }
}
