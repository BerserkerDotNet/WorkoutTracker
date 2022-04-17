using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.MuscleSelectors;
using static WorkoutTracker.Data.Selectors.ExerciseSelectors;

namespace WorkoutTracker.Pages
{
    public partial class EditExercise
    {
        private MudForm form;

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
            props.Muscles = SelectMuscles(state);
            props.Tags = SelectTags(state);
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (!SelectMuscles(store.State).Any()) 
            {
                await store.Dispatch<FetchMusclesAction>();
            }
        }

        private async Task Save()
        {
            await form.Validate();
            if (form.IsValid)
            {
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
