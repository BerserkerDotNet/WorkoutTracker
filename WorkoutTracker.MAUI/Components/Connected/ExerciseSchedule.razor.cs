using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class ExerciseScheduleConnected : ConnectedComponent<ExerciseSchedule, RootState, ExerciseScheduleProps>
    {
        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = EventCallback.Factory.Create<ExerciseWithCategoryViewModel>(this, item => Navigation.NavigateTo($"/editexerciselog/{item.Category}/{item.Exercise.Id}"));
            props.OpenLog = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/log"));
            props.Rebuild = EventCallback.Factory.Create<ExerciseProfile>(this, async profile =>
            {
                var exercises = store.State.Exercises.List.Values;
                // TODO: Should this be here?
                if (profile == ExerciseProfile.UpperBody)
                {
                    await store.Dispatch<BuildUpperBodyExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(exercises);
                }
                else 
                {
                    await store.Dispatch<BuildLowerBodyExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(exercises);
                }
            });

            props.Previous = EventCallback.Factory.Create<string>(this, async category =>
            {
                await store.Dispatch<MoveToPreviousExerciseAction, ScheduleViewModel>(store.State.Exercises.Schedule[category]);
            });

            props.Next = EventCallback.Factory.Create<string>(this, async category =>
            {
                await store.Dispatch<MoveToNextExerciseAction, ScheduleViewModel>(store.State.Exercises.Schedule[category]);
            });
        }

        protected override void MapStateToProps(RootState state, ExerciseScheduleProps props)
        {
            props.Schedule = Enumerable.Empty<ExerciseWithCategoryViewModel>();
            if (state?.Exercises?.Schedule is not null)
            {
                props.Schedule = state.Exercises.Schedule.Select(s => new ExerciseWithCategoryViewModel(s.Key, s.Value.Exercises.ElementAt(s.Value.CurrentIndex))).ToArray();
            }

            if (state is object && state.Exercises is object && state.Exercises.Log is object)
            {
                props.ExerciseCount = state.Exercises.Log.ToDictionary(k => k.Exercise.Id, v => v.Sets.Count());
            }
            else 
            {
                props.ExerciseCount = new Dictionary<Guid, int>();
            }
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (!ConfigurationService.IsConfigurationAvailable()) 
            {
                Navigation.NavigateTo("/configuration");
                return;
            }

            var state = store.State?.Exercises;
            if (state?.Schedule is object && state.Schedule.Any())
            {
                return;
            }

            if (state?.List is null)
            {
                await store.Dispatch<FetchExercisesAction>();
            }

            await store.Dispatch<BuildUpperBodyExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(store.State.Exercises.List.Values);
        }
    }
}
