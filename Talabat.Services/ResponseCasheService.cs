using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Services
{
    public class ResponseCasheService : IResponseCasheService
    {
        private readonly IDatabase _database;

        public ResponseCasheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task CasheResponseAsync(string casheKey, object response, TimeSpan timeToLive)
        {
            if (response is null) return;
            var serializedOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            var serializesResponse = JsonSerializer.Serialize(response , serializedOptions);
            await _database.StringSetAsync(casheKey, serializesResponse,timeToLive);
        }

        public async Task<string?> GetCashedResponseAsync(string casheKey)
        {
            var cashedResponse = await _database.StringGetAsync(casheKey);
            if (cashedResponse.IsNullOrEmpty) return null;
            return cashedResponse;
        }
    }
}
