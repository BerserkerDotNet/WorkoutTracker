using BlazorState.Redux.Blazor;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.MuscleSelectors;
using static WorkoutTracker.Data.Selectors.ExerciseSelectors;

namespace WorkoutTracker.Components.Connected
{
    public class EditExerciseConnected : ConnectedComponent<EditExercise, RootState, EditExerciseProps>
    {
        [Parameter]
        public Guid ExerciseId { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, EditExerciseProps props)
        {
            props.Save = EventCallback.Factory.Create<EditExerciseViewModel>(this, async exercise =>
            {
                await store.Dispatch<SaveExerciseAction, EditExerciseViewModel>(exercise);
                Navigation.NavigateTo($"/library");
            });

            props.Cancel = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/library"));
        }

        protected override void MapStateToProps(RootState state, EditExerciseProps props)
        {
            var exercise = SelectExerciseById(state, ExerciseId);
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
            props.Muscles = SelectMusclesLookup(state);
            props.Tags = SelectTags(state);
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (!SelectMuscles(store.State).Any()) 
            {
                await store.Dispatch<FetchMusclesAction>();
            }
        }
    }

    public partial class EditExercise 
    {
        [Parameter]
        public EditExerciseProps Props { get; set; }

        private IEnumerable<string> SelectedMuscles { get; set; } = new HashSet<string>();
        private MudForm form;

        protected override void OnParametersSet()
        {
            if (!Props.IsLoading)
            {
                SelectedMuscles = new HashSet<string>(Props.Exercise.Muscles.OrderBy(m => m.MuscleGroup).Select(m => m.Id.ToString()));
            }
        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return $"Selected muscle{(selectedValues.Count > 1 ? "s" : "")}: {string.Join(", ", selectedValues.Select(x => Props.Muscles[Guid.Parse(x)].Name))}";
        }

        private async Task Save()
        {
            await form.Validate();
            if (form.IsValid)
            {
                Props.Exercise.Muscles = SelectedMuscles.Select(x => Props.Muscles[Guid.Parse(x)]);
                await Props.Save.InvokeAsync(Props.Exercise);
            }
        }

        private async Task Cancel()
        {
            await Props.Cancel.InvokeAsync();
        }

        private void UploadFiles(InputFileChangeEventArgs e)
        {
            Props.Exercise.ImageFile = e.File;
            Props.Exercise.ImagePath = $"exercises/{e.File.Name}";
        }
    }
}
