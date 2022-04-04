using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.MuscleSelectors;

namespace WorkoutTracker.Pages
{
    public partial class EditMuscle
    {
        private MudForm _form;
        private IBrowserFile _imageFile;

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Parameter]
        public Guid MuscleId { get; set; }

        protected override void MapStateToProps(RootState state, EditMuiscleProps props)
        {
            Props.Muscle = SelectMuscleById(state, MuscleId);
        }

        protected override void MapDispatchToProps(IStore<RootState> store, EditMuiscleProps props)
        {
            Props.Cancel = CreateCallback(() => Navigation.NavigateTo("/muscles"));
            Props.Save = CreateCallback<MuscleViewModel>(async muscle => 
            {
                await store.Dispatch<SaveMuscleAction, SaveMuscleModel>(new SaveMuscleModel(muscle, _imageFile)); // NOTE: references class level variable as there is no good way to pass second parameter to the action
                Navigation.NavigateTo("/muscles");
            });
        }

        private void UploadFiles(InputFileChangeEventArgs e)
        {
            _imageFile = e.File;
            Props.Muscle.ImagePath = $"muscles/{e.File.Name}";
        }
    }
}
