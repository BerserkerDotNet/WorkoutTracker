using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public partial class EditExerciseLog 
    {
        const string KG = "KG";
        const string LB = "LB";
        private bool isSaving = false;
        private bool _isRunning = false;
        private CancellationTokenSource _source;
        private string _weightUnits = "KG";
        private ExerciseLogEntry logEntry = new ExerciseLogEntry();
        private int duration = 0;

        [Parameter]
        public EditExerciseLogProps Props { get; set; }

        [Inject]
		public EditExerciseLogStyles Styles { get; set; }

        [Inject]
        public FieldSetItemStyles FieldSetStyles { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        private async Task SaveAndNew(SetDetail details)
        {
            var weightInKG = _weightUnits == "LB" ? Math.Ceiling((double)logEntry.Weight * 0.453592d) : logEntry.Weight;
            isSaving = true;
            await Props.Save.InvokeAsync(new ExerciseLogEntry
            {
                Id = Guid.NewGuid(),
                ExerciseId = Props.Exercise.Id,
                Duration = TimeSpan.FromSeconds(details.Duration),
                Weight = weightInKG,
                Repetitions = details.Repetitions,
                Score = details.Rating,
                Note = details.Notes

            });
            isSaving = false;
            logEntry.Repetitions = 0;
            logEntry.Note = string.Empty;
            duration = 0;
        }

        private void StartSet()
        {
            if (!_isRunning)
            {
                _source = new CancellationTokenSource();
                Task.Factory.StartNew(async () => await StartCounting(_source.Token));
                _isRunning = true;
            }
        }

        private async Task EndSet()
        {
            if (_isRunning)
            {
                _source.Cancel();
                _source = null;
                _isRunning = false;
            }

            var details = await DialogService.OpenAsync($"Details of set {Props.ExerciseSetNumber}", ds => 
            {
                var seq = 0;
                RenderFragment content = b =>
                {
                    b.OpenComponent<SetCompletionForm>(seq++);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.SetNumber), Props.ExerciseSetNumber);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.CurrentDuration), duration);
                    b.AddAttribute(seq++, nameof(SetCompletionForm.SaveDetails), EventCallback.Factory.Create<SetDetail>(this, d => ds.Close(d)));
                    b.CloseComponent();
                };

                return content;
            }, new DialogOptions() { Style = "min-height:auto;min-width:auto;width:auto" });

            if (details is null) 
            {
                return;
            }

            DialogService.Open("", ds =>
            {
                var seq = 0;
                RenderFragment content = b =>
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "row");
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "col-md-12");
                    b.AddContent(seq++, "Saving, please wait...");
                    b.CloseElement();
                    b.CloseElement();
                };

                return content;
            }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto" });

            await SaveAndNew(details);

            DialogService.Close();
        }

        private async Task StartCounting(CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            int currentDuration = 0;
            while (await timer.WaitForNextTickAsync())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                duration = ++currentDuration;
            }
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
            props.Save = EventCallback.Factory.Create<ExerciseLogEntry>(this, async e =>
            {
                await store.Dispatch<SaveExerciseLogEntryAction, ExerciseLogEntry>(e);
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
            props.Exercise = state.Exercises.List[ExerciseId];
            var nextId = state.Exercises.Schedule.SkipWhile(s => s.Id != ExerciseId).Take(2).Last().Id;
            props.NextExerciseId = nextId == ExerciseId ? null : nextId;

            var prevId = state.Exercises.Schedule.TakeWhile(s => s.Id != ExerciseId).LastOrDefault()?.Id;
            props.PreviousExerciseId = prevId;
            if (state is object && state.Exercises is object && state.Exercises.Log is object)
            {
                props.Sets = state.Exercises.Log.Where(g => g.ExerciseId == ExerciseId).OrderByDescending(s => s.Date).ToArray();
                props.ExerciseSetNumber = props.Sets.Count() + 1;
            }
            else
            {
                props.Sets = Enumerable.Empty<ExerciseLogEntry>();
                props.ExerciseSetNumber = 1;
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
