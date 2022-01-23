using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
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
        const string DefaultDialogStyle = "min-height:auto;min-width:auto;width:auto";
        const string KG = "KG";
        const string LB = "LB";
        private Stopwatch _timeTracker;
        private bool _isRunning = false;
        private CancellationTokenSource _source;
        private string _weightUnits = "KG";
        private int _currentRestTime = 0;
        private int _weight = 0;

        private TimeSpan duration = TimeSpan.Zero;

        [Parameter]
        public EditExerciseLogProps Props { get; set; }

        [Inject]
		public EditExerciseLogStyles Styles { get; set; }

        [Inject]
        public FieldSetItemStyles FieldSetStyles { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        private bool DisablePrevious => !Props.PreviousExerciseId.HasValue || _isRunning;

        private bool DisableNext => !Props.NextExerciseId.HasValue || _isRunning;

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
            var details = await ShowSetCompletionDialog(exerciseDuration, restTime);
            if (details is null) 
            {
                return;
            }

            try
            {
                ShowMessageDialog(string.Empty, "Saving, please wait...");
                await SaveAndNew(details);
            }
            catch (Exception ex)
            {
                // TODO: Need to close saving and open error dialog
                ShowMessageDialog("Error saving the record", ex.Message, closeOnDismiss: true);
            }
            DialogService.Close();
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

        private void ShowMessageDialog(string title, string message, bool closeOnDismiss = false) 
        {
            var showHeader = !string.IsNullOrEmpty(title);
            DialogService.Open(title, ds =>
            {
                var seq = 0;
                RenderFragment content = b =>
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "row");
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "col-md-12");
                    b.AddContent(seq++, message);
                    b.CloseElement();
                    b.CloseElement();
                };

                return content;
            }, new DialogOptions() { ShowTitle = showHeader, ShowClose = showHeader, CloseDialogOnOverlayClick = closeOnDismiss, Style = DefaultDialogStyle });
        }

        private async Task<Set> ShowSetCompletionDialog(int duration, int restTime) 
        {
            var weightInKG = _weightUnits == "LB" ? Math.Ceiling(_weight * 0.453592d) : _weight;
            return await DialogService.OpenAsync($"Details of set {Props.SetNumber}", ds =>
            {
                var seq = 0;
                RenderFragment content = b =>
                {
                    b.OpenComponent<SetCompletionForm>(seq++);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.SetNumber), Props.SetNumber);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.CurrentDuration), duration);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.CurrentRestTime), restTime);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.CurrentWeight), weightInKG);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.SaveSet), EventCallback.Factory.Create<Set>(this, d => ds.Close(d)));
                    b.CloseComponent();
                };

                return content;
            }, new DialogOptions() { Style = DefaultDialogStyle });
        }
    }

    public class EditExerciseLogConnected : ConnectedComponent<EditExerciseLog, RootState, EditExerciseLogProps>
    {
        private EditExerciseLogProps _props; // TODO: workaround

        [Parameter]
        public Guid Id { get; set; }

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

            props.Next = EventCallback.Factory.Create<Guid>(this, id =>
            { 
                Navigation.NavigateTo($"/editexerciselog/{Guid.Empty}/{id}");
            });

			props.Previous = EventCallback.Factory.Create<Guid>(this, id =>
            {
                Navigation.NavigateTo($"/editexerciselog/{Guid.Empty}/{id}");
            });
        }

        protected override void MapStateToProps(RootState state, EditExerciseLogProps props)
        {
            var nextId = state.Exercises.Schedule.SkipWhile(s => s.Id != ExerciseId).Take(2).Last().Id;
            props.NextExerciseId = nextId == ExerciseId ? null : nextId;

            var prevId = state.Exercises.Schedule.TakeWhile(s => s.Id != ExerciseId).LastOrDefault()?.Id;
            props.PreviousExerciseId = prevId;
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

    public class EditExerciseLogStylesModule : IStylesModule
    {
        public void Configure(SharpCssConfigurator configurator)
        {
            configurator.RegisterStyles<EditExerciseLogStyles>(new
            {
                InputWithButton = new StyleSet
                {
                    MarginRight = 10
                },
                Input = new StyleSet
                {
                    Width = "100%"
                },
                WeightContainer = new StyleSet
                {
                    MinWidth = 112,
                    Width = "100%",
                    Display = "flex",
                    JustifyContent = "space-evenly"
                },
                WeightButton = new StyleSet
                {
                    Padding = "0 1em",
                    Width = "50%",
                    TextAlign= "center"
                },
                TimerButton = new StyleSet
                {
                    MinWidth = 95
                }
            });
        }
    }
}
