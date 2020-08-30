using System;
using System.Linq;

using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseSelector : BaseAdapter<Exercise>
    {
        private readonly Activity _context;
        Exercise[] _items;

        public ExerciseSelector(Activity context)
        {
            var exercises = InMemoryCache.Instance.GetCollection<Exercise>(nameof(Exercise));
            _items = exercises.OrderBy(e => e.Name).ToArray();

            _context = context;
        }

        public override Exercise this[int position] => _items[position];

        public override int Count => _items.Length;

        public override long GetItemId(int position)
        {
            return _items[position].Id.GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var layout = _context.LayoutInflater.Inflate(Resource.Layout.exercise_selector, null);

            var img = layout.FindViewById<ImageView>(Resource.Id.exercise_selector_image);
            var name = layout.FindViewById<TextView>(Resource.Id.exercise_selector_name);
            var iconBitmap = BitmapFactory.DecodeByteArray(item.Icon, 0, item.Icon.Length);
            img.SetImageBitmap(iconBitmap);

            name.Text = item.Name;

            return layout;
        }

        public int GetPositionById(Guid id)
        {
            var item = _items.Single(i => i.Id == id);
            return Array.IndexOf(_items, item);
        }
    }
}