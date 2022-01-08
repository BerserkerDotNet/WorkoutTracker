using Application = Microsoft.Maui.Controls.Application;

namespace WorkoutTracker.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
