using System;
using WorkoutTracker.MAUI.Views;
using Application = Microsoft.Maui.Controls.Application;

namespace WorkoutTracker.MAUI
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new MainPage();
            ServiceProvider = serviceProvider;
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider ServiceProvider { get; }
    }
}
