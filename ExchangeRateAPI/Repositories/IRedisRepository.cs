namespace ExchangeRateAPI.Repositories { 
    public interface IRedisRepository { 
       Task SaveAsync<T>(string key, T data, TimeSpan expiry);
       Task<T> GetAsync<T>(string key);
    }
}