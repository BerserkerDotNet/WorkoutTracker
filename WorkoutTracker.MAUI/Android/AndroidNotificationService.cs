using Widget = Android.Widget;
using AndroidApp = Android.App;

namespace WorkoutTracker.MAUI.Android
{
    public class AndroidNotificationService : INotificationService
    {
        public void ShowToast(string message)
        {
            Widget.Toast.MakeText(AndroidApp.Application.Context, "Foo", Widget.ToastLength.Short);
        }
    }
}
