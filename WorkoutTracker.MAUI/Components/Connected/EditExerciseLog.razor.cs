using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public partial class EditExerciseLog : IDisposable
    {
        const string KG = "KG";
        const string LB = "LB";
        private Stopwatch _timeTracker;
        private bool _isRunning = false;
        private bool isSavingData = false;
        private CancellationTokenSource _source;
        private string _weightUnits = KG;
        private int _currentRestTime = 0;
        private int _weight = 0;

        private TimeSpan duration = TimeSpan.Zero;

        [Parameter]
        public EditExerciseLogProps Props { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        private bool DisablePrevious => Props.PreviousExerciseId is null || _isRunning;

        private bool DisableNext => Props.NextExerciseId is null || _isRunning;

        public void Dispose()
        {
            _source.Cancel();
            _source = null;
        }

        protected override void OnInitialized()
        {
            _timeTracker = Stopwatch.StartNew();
            _source = new CancellationTokenSource();
            Task.Factory.StartNew(async () => await StartCounting(_source.Token));
        }

        private async Task SaveAndNew(Set set)
        {
            Props.Log.Sets = Props.Log.Sets.Union(new[] { set }); // TODO: Change this!
            
            await Props.Save.InvokeAsync(Props.Log);
        }

        private void StartSet()
        {
            _currentRestTime = (int)_timeTracker.Elapsed.TotalSeconds;
            _timeTracker.Restart();
            _isRunning = true;
        }

        private async Task EndSet()
        {
            _isRunning = false;
            var restTime = _currentRestTime;
            var exerciseDuration = (int)_timeTracker.Elapsed.TotalSeconds;
            _currentRestTime = 0;
            _timeTracker.Restart();
            var saveDialog = ShowSetCompletionDialog(exerciseDuration, restTime);
            var result = await saveDialog.Result;
            if (result.Cancelled) 
            {
                return;
            }

            try
            {
                isSavingData = true;
                await SaveAndNew(result.Data as Set);

                Snackbar.Add("Exercise logged.", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error saving exercise {ex.Message}", Severity.Error, cfg =>
                {
                    cfg.VisibleStateDuration = 10000;
                });
            }
            finally 
            {
                isSavingData = false;
            }
        }

        private async Task StartCounting(CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (await timer.WaitForNextTickAsync())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                await InvokeAsync(() => StateHasChanged());
            }
        }

        private IDialogReference ShowSetCompletionDialog(int duration, int restTime) 
        {
            var weightInKG = _weightUnits == LB ? Math.Ceiling(_weight * 0.453592d) : _weight;
            var parameters = new DialogParameters
            {
                { nameof(SetCompletionForm.CurrentDuration), duration},
                { nameof(SetCompletionForm.CurrentRestTime), restTime},
                { nameof(SetCompletionForm.CurrentWeight), weightInKG},
            };
            return DialogService.Show<SetCompletionForm>($"Details of set {Props.SetNumber}", parameters);
        }
    }

    public class EditExerciseLogConnected : ConnectedComponent<EditExerciseLog, RootState, EditExerciseLogProps>
    {
        private EditExerciseLogProps _props; // TODO: workaround

        [Parameter]
        public string CurrentCategory { get; set; }

        [Parameter]
        public Guid ExerciseId { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, EditExerciseLogProps props)
        {
            props.Save = EventCallback.Factory.Create<LogEntryViewModel>(this, async e =>
            {
                await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
            });

            props.Cancel = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/"));

            props.Next = EventCallback.Factory.Create<ExerciseWithCategoryViewModel>(this, item =>
            { 
                Navigation.NavigateTo($"/editexerciselog/{item.Category}/{item.Exercise.Id}");
            });

			props.Previous = EventCallback.Factory.Create<ExerciseWithCategoryViewModel>(this, item =>
            {
                Navigation.NavigateTo($"/editexerciselog/{item.Category}/{item.Exercise.Id}");
            });
        }

        protected override void MapStateToProps(RootState state, EditExerciseLogProps props)
        {
            var nextSet = state.Exercises.Schedule.SkipWhile(s => s.Key != CurrentCategory).Take(2).Last().Value;
            props.NextExerciseId = nextSet.Category == CurrentCategory ? null : new ExerciseWithCategoryViewModel(nextSet.Category, nextSet.Exercises.ElementAt(nextSet.CurrentIndex));

            var prevSet = state.Exercises.Schedule.TakeWhile(s => s.Key != CurrentCategory).LastOrDefault().Value;
            props.PreviousExerciseId = prevSet is null || prevSet.Category == CurrentCategory ? null : new ExerciseWithCategoryViewModel(prevSet.Category, prevSet.Exercises.ElementAt(prevSet.CurrentIndex));

            var log = state?.Exercises?.Log?.SingleOrDefault(g => g.Date.Date == DateTime.UtcNow.Date && g.Exercise.Id == ExerciseId);
            if (log is object)
            {
                props.Log = log;
                props.SetNumber = log.Sets.Count() + 1;
            }
            else
            {
                props.Log = new LogEntryViewModel
                {
                    Id = Guid.NewGuid(),
                    Exercise = state.Exercises.List[ExerciseId],
                    Sets = Enumerable.Empty<Set>(),
                    Date = DateTime.UtcNow
                };
                props.SetNumber = 1;
            }

            _props = props;
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var state = store.State?.Exercises;
            if (state?.List is null)
            {
                await store.Dispatch<FetchExercisesAction>();
            }
        }

        protected override void OnParametersSet()
        {
            MapStateToProps(Store.State, _props);
            this.StateHasChanged();
        }
    }
}
