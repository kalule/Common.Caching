using Common.Caching.Interfaces.Common.Caching.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Common.Caching.Interfaces.Implementations
{
    public class RedisCacheOptions
    {
        public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(30);
        ///public JsonSerializerOptions JsonSerializerOptions { get; set; }
    }
    public class RedisCacheService : ICacheService
    {
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IDistributedCache _cache;
        private readonly RedisCacheOptions _options;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger, IOptions<RedisCacheOptions> options)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await _cache.GetStringAsync(key, cancellationToken);
                if (data == null)
                {
                    _logger.LogDebug("Cache miss for key '{Key}'", key);
                    return default;
                }

                if (typeof(T) == typeof(string))
                {
                    return (T)(object)data!;
                }

                return JsonSerializer.Deserialize<T>(data, _serializerOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for key '{Key}' with value that could not be parsed to type {Type}", key, typeof(T));
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving key '{Key}' from cache", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? _options.DefaultExpiry
                };

                var json = JsonSerializer.Serialize(value, _serializerOptions);
                await _cache.SetStringAsync(key, json, options, cancellationToken);
                _logger.LogDebug("Cache set for key '{Key}'", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set key '{Key}' in cache", key);
            }
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await GetAsync<T>(key, cancellationToken);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var newValue = await factory();
                if (newValue != null)
                {
                    await SetAsync(key, newValue, expiry, cancellationToken);
                }

                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetOrSetAsync for key '{Key}'", key);
                return default;
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
                _logger.LogDebug("Cache removed for key '{Key}'", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove key '{Key}' from cache", key);
            }
        }
    }
}
