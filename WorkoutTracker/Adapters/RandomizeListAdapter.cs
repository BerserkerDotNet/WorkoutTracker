using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class RandomizeListAdapter : BaseAdapter<Exercise>
    {
        private readonly Activity _context;
        private readonly Exercise[] _items;

        public RandomizeListAdapter(Activity context, Exercise[] items)
        {
            _context = context;
            _items = items;
        }

        public override Exercise this[int position] => _items[position];

        public override int Count => _items.Length;

        public override long GetItemId(int position)
        {
            return _items[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Resource.Layout.exercises_random_listview, null);
            }

            var exercise = _items[position];
            var iconField = convertView.FindViewById<ImageView>(Resource.Id.exercise_random_list_icon);
            var nameField = convertView.FindViewById<TextView>(Resource.Id.exercise_random_list_name);
            var musclesField = convertView.FindViewById<TextView>(Resource.Id.exercise_random_list_muscles);
            var iconBitmap = BitmapFactory.DecodeByteArray(exercise.Icon, 0, exercise.Icon.Length);
            iconField.SetImageBitmap(iconBitmap);
            nameField.Text = exercise.Name;
            var musclesGroup = exercise.Muscles == null ? "N/A" : string.Join(", ", exercise.Muscles);
            musclesField.Text = $"Muscles: {musclesGroup}";

            return convertView;
        }
    }
}

