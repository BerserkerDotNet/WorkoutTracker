using System;
using System.Linq;
using System.Threading.Tasks;
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
        private CacheManager _cache;
        Exercise[] _items;

        public ExerciseSelector(Activity context)
        {
            _context = context;
            _cache = new CacheManager(context);
            _items = Array.Empty<Exercise>();

            LoadData()
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
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

        private Task LoadData()
        {
            return _cache.GetAll(ApiRepository.Instance.GetAll<Exercise>).ContinueWith(task =>
            {
                _items = task.Result.OrderBy(e => e.Name).ToArray();
            });
        }
    }
}