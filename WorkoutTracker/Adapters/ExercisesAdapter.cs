using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExercisesAdapter : BaseAdapter<Exercise>
    {
        private readonly Activity _context;
        private readonly SwipeRefreshLayout _refreshLayout;
        private readonly IRepository _repository;
        private Exercise[] _items;

        public ExercisesAdapter(Activity context, SwipeRefreshLayout refreshLayout)
        {
            _context = context;
            _refreshLayout = refreshLayout;
            _refreshLayout.Refresh += OnRefresh;
            _repository = ApiRepository.Instance;
            _items = Array.Empty<Exercise>();

            ReloadData();
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
                convertView = _context.LayoutInflater.Inflate(Resource.Layout.exercises_listview, null);
            }

            var exercise = _items[position];
            var iconField = convertView.FindViewById<ImageView>(Resource.Id.exercise_list_icon);
            var nameField = convertView.FindViewById<TextView>(Resource.Id.exercise_list_name);
            var musclesField = convertView.FindViewById<TextView>(Resource.Id.exercise_list_muscles);
            var icon = _context.Resources.GetIdentifier(exercise.Icon, "drawable", _context.PackageName);
            iconField.SetImageResource(icon);
            nameField.Text = exercise.Name;
            var musclesGroup = exercise.Muscles == null ? "N/A" : string.Join(", ", exercise.Muscles);
            musclesField.Text = $"Muscles: {musclesGroup}";

            return convertView;
        }

        public Task ReloadData()
        {
            _refreshLayout.Refreshing = true;

            return _repository.GetAll<Exercise>().ContinueWith(itemsTask =>
            {
                _items = itemsTask.Result.OrderBy(e => e.Name).ToArray();
                _context.RunOnUiThread(() =>
                {
                    NotifyDataSetChanged();
                    _refreshLayout.Refreshing = false;
                });
            });
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            ReloadData();
        }
    }
}

