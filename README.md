# Common.Caching

A lightweight and extensible Redis-based caching library for .NET, designed for clean integration and reusable application architecture.

## 🚀 Features

- Simple `ICacheService` interface for consistent caching
- Supports generic `Get`, `Set`, `GetOrSet`, and `Remove` operations
- JSON serialization with camelCase formatting
- Configurable default expiry via options pattern
- Built-in logging using `ILogger`
- Fully compatible with `IDistributedCache` (e.g., Redis via StackExchange.Redis)

## 🛠️ Installation

To install from a local NuGet source:

```bash
dotnet add package Common.Caching --source LocalCachingLibrary
```

To add the package from GitHub/NuGet (once published):

```bash
dotnet add package Common.Caching
```

## 📦 Usage

### 1. Register in `Startup.cs` or `Program.cs`

```csharp
services.Configure<RedisCacheOptions>(Configuration.GetSection("RedisCacheOptions"));
services.AddScoped<ICacheService, RedisCacheService>();
```

### 2. Configuration (`appsettings.json`)

```json
"RedisCacheOptions": {
  "DefaultExpiry": "00:30:00"
}
```

### 3. Inject and Use

```csharp
public class MyService
{
    private readonly ICacheService _cache;

    public MyService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<string> GetCachedValueAsync()
    {
        return await _cache.GetOrSetAsync("sample:key", async () =>
        {
            // Simulate DB call or expensive operation
            await Task.Delay(100);
            return "Cached result";
        });
    }
}
```

## 🧪 Interface

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
```

## ✅ Requirements

- .NET 6 or later
- Microsoft.Extensions.Caching.StackExchangeRedis
- Redis server

## 📄 License

MIT License

---

Made with ❤️ by [Kalule Dison](mailto:kaluledison@gmail.com)
