using Widget = Android.Widget;
using AndroidApp = Android.App;
using WorkoutTracker.Interfaces;

namespace WorkoutTracker.MAUI.Android
{
    public class AndroidNotificationService : INotificationService
    {
        public void ShowToast(string message)
        {
            Widget.Toast.MakeText(AndroidApp.Application.Context, message, Widget.ToastLength.Long).Show();
        }
    }
}
