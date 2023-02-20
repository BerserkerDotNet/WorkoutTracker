namespace WorkoutTracker.MAUI.Views
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EditExercisePage), typeof(EditExercisePage));
        }
    }
}
