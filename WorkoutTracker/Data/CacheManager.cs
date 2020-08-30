using Android.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data
{
    public class CacheManager
    {
        private readonly Context _context;

        public CacheManager(Context context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<T>> GetAll<T>(Func<Task<IEnumerable<T>>> factory)
            where T : EntityBase
        {
            if (IsCachedInMemory<T>())
            {
                return InMemoryCache.Instance.GetCollection<T>(typeof(T).Name);
            }

            IEnumerable<T> items = null;
            var fileName = $"{typeof(T).Name}.cache";

            if (IsCached(fileName))
            {
                var cachedJson = ReadFromFile(fileName);
                items = JsonConvert.DeserializeObject<IEnumerable<T>>(cachedJson);
                CacheInMemory(items);
                return items;
            }

            items = await factory();

            CacheInMemory(items);
            var json = JsonConvert.SerializeObject(items);
            WriteToCache(fileName, json);

            return items;
        }

        private void WriteToCache(string fileName, string data)
        {
            var path = GetFilePath(fileName);
            File.WriteAllText(path, data);
        }

        private string ReadFromFile(string fileName)
        {
            var path = GetFilePath(fileName);
            return File.ReadAllText(path);
        }

        private bool IsCachedInMemory<T>()
        {
            return InMemoryCache.Instance.ContainsKey(typeof(T).Name);
        }

        private bool IsCached(string fileName)
        {
            return File.Exists(GetFilePath(fileName));
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(_context.CacheDir.AbsolutePath, fileName);
        }

        private void CacheInMemory<T>(IEnumerable<T> items)
             where T : EntityBase
        {
            InMemoryCache.Instance.SetCollection(typeof(T).Name, items);
        }
    }
}