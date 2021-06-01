using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data
{
    public class ApiRepository : IRepository, IExerciseLogRepository
    {
        private readonly HttpClient _client;

        public ApiRepository(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }

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
            var response = await _client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
        }

        public async Task<T> GetById<T>(Guid id) where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var response = await _client.GetAsync($"{endpoint}?id={id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<IEnumerable<T>>(content);

            return items.FirstOrDefault();
        }

        public Task Update<T>(T entity) where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var content = JsonConvert.SerializeObject(entity);
            return _client.PatchAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
        }

        public Task Delete<T>(Guid id)
            where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            return _client.DeleteAsync($"{endpoint}?id={id}");
        }

        async Task<IEnumerable<ExerciseLogEntry>> IExerciseLogRepository.GetByDate(DateTime date)
        {
            var endpoint = GetApiEndpointFor<ExerciseLogEntry>();
            var response = await _client.GetAsync($"{endpoint}?date={date.Date.ToString("dd-MM-yyyy")}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("TODO: change this!");
            }

            var content = await response.Content.ReadAsStringAsync();
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

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(content);

            return items;
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
    }
}