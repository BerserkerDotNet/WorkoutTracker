using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data
{
    public class ApiRepository
    {
        private readonly HttpClient _client;

        public ApiRepository(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }

        public async Task Create<T>(T entity)
            where T : EntityBase
        {
            var endpoint = GetApiEndpointFor<T>();
            var content = JsonConvert.SerializeObject(entity);
            var response = await _client.PostAsync(endpoint, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Something went wrong. Response code: {response}. Message: {message}.");
            }
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

        private string GetApiEndpointFor<T>()
            where T : EntityBase
        {
            switch (typeof(T).Name) 
            {
                case nameof(Exercise):
                    return nameof(Exercise);
                case nameof(ExerciseLogEntry):
                    return "ExerciseLog";
                default:
                    throw new NotSupportedException("Endpoint for the entity is not supported.");
            }
        }
    }
}