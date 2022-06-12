using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class WorkoutStatisticsConnected : SafeConnectedComponent<WorkoutStatistics, RootState, WorkoutSummaryProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToPropsSafe(RootState state, WorkoutSummaryProps props)
    {
        var history = state.SelectSummaries();
        props.Summaries = history;
        props.Exercises = state.SelectExercises().OrderBy(e => e.Name);
    }

    protected override void MapDispatchToProps(IStore<RootState> store, WorkoutSummaryProps props)
    {
    }

    protected override async Task Init(IStore<RootState> store)
    {
        var history = store.State.SelectSummaries();
        if (history.Any())
        {
            return;
        }

        await store.Dispatch<FetchWorkoutStatsAction, WorkoutStatsRequest>(new WorkoutStatsRequest(DateTime.Today.AddMonths(-6), DateTime.Today.AddDays(1)));
        await store.Dispatch<FetchExercisesAction>();
    }
}