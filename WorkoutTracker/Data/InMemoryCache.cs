using System.Collections.Generic;

namespace WorkoutTracker.Data
{
    public class InMemoryCache
    {
        private static InMemoryCache _instance = new InMemoryCache();

        public static InMemoryCache Instance => _instance;

        Dictionary<string, object> _data = new Dictionary<string, object>();

        public IEnumerable<T> GetCollection<T>(string key)
        {
            return (IEnumerable<T>)_data[key];
        }

        public void SetCollection<T>(string key, IEnumerable<T> collection)
        {
            _data[key] = collection;
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }
    }
}