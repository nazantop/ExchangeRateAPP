using StackExchange.Redis;
using System.Text.Json;

namespace ExchangeRateAPI.Repositories { 
    public class RedisRepository : IRedisRepository { 
        private readonly IDatabase _database;

        public RedisRepository(IConnectionMultiplexer connectionMultiplexer) {
            _database = connectionMultiplexer.GetDatabase();
        }
        
        public async Task SaveAsync<T>(string key, T data, TimeSpan expiry){
            var json = JsonSerializer.Serialize(data);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<T> GetAsync<T>(string key){
            var json = await _database.StringGetAsync(key);
            return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}