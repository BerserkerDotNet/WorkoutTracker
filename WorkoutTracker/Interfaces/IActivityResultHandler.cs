using Android.App;
using Android.Content;

namespace WorkoutTracker.Interfaces
{
    public interface IActivityResultHandler
    {
        void HandleActivityResult(int requestCode, Result resultCode, Intent data);
    }
}