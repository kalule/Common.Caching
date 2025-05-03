namespace Common.Caching.Interfaces
{
    namespace Common.Caching.Interfaces
    {
        /// <summary>
        /// Defines methods for a distributed caching service.
        /// </summary>
        public interface ICacheService
        {
            /// <summary>
            /// Retrieves a value from the cache by key.
            /// </summary>
            /// <typeparam name="T">The type of the cached value.</typeparam>
            /// <param name="key">The cache key.</param>
            /// <param name="cancellationToken">Optional cancellation token.</param>
            /// <returns>The cached value if found; otherwise, default.</returns>
            Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

            /// <summary>
            /// Stores a value in the cache with an optional expiration.
            /// </summary>
            /// <typeparam name="T">The type of the value to store.</typeparam>
            /// <param name="key">The cache key.</param>
            /// <param name="value">The value to store.</param>
            /// <param name="expiry">Optional expiration time. Defaults to a configured value if null.</param>
            /// <param name="cancellationToken">Optional cancellation token.</param>
            Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

            /// <summary>
            /// Retrieves a value from the cache or creates and stores it if not found.
            /// </summary>
            /// <typeparam name="T">The type of the value.</typeparam>
            /// <param name="key">The cache key.</param>
            /// <param name="factory">A function to create the value if not found in cache.</param>
            /// <param name="expiry">Optional expiration time.</param>
            /// <param name="cancellationToken">Optional cancellation token.</param>
            /// <returns>The cached or newly created value.</returns>
            Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

            /// <summary>
            /// Removes a value from the cache by key.
            /// </summary>
            /// <param name="key">The cache key.</param>
            /// <param name="cancellationToken">Optional cancellation token.</param>
            Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        }
    }


}
