using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data
{
    public class ApiRepository : IRepository, IExerciseLogRepository
    {
        private static ApiRepository _instance = new ApiRepository();
        private readonly HttpClient _client;

        private ApiRepository()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://192.168.0.19:7071/api/" /* "https://workouttrackerfunctions.azurewebsites.net/api/" */);
            _client.DefaultRequestHeaders.Add("x-functions-key", "tEtzNOPdCPqpWIakJBMf3gHKhVpX9cssTLT6O0rRygD1r3ohys0N7A==");
        }

        public static ApiRepository Instance => _instance;

        public Task Create<T>(T entity)
            where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var content = JsonConvert.SerializeObject(entity);
            return _client.PostAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
        }

        public async Task<T> GetById<T>(Guid id) where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var response = await _client.GetAsync($"{endpoint}?id={id}").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<IEnumerable<T>>(content);

            return items.FirstOrDefault();
        }

        public Task Update<T>(T entity) where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var content = JsonConvert.SerializeObject(entity);
            return _client.PatchAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
        }

        private string GetApiEndpointFor<T>()
            where T : EntityBase
        {
            var type = typeof(T);
            if (type == typeof(Exercise))
            {
                return "Exercises";
            }
            else if (type == typeof(ExerciseLogEntry))
            {
                return "ExerciseLogs";
            }

            throw new NotSupportedException($"Type of {type} is not supported.");
        }

        async Task<IEnumerable<ExerciseLogEntry>> IExerciseLogRepository.GetByDate(DateTime date)
        {
            var endpoint = GetApiEndpointFor<ExerciseLogEntry>();
            var response = await _client.GetAsync($"{endpoint}?date={date.Date.ToString("dd-MM-yyyy")}").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<IEnumerable<ExerciseLogEntry>>(content);

            return items;
        }

        async Task<IEnumerable<string>> IExerciseLogRepository.GetDates()
        {
            var response = await _client.GetAsync("ExerciseLogsDates");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(content);

            return items;
        }
    }
}