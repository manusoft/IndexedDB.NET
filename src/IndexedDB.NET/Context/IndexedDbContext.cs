using ManuHub.IndexedDB.Metadata;
using ManuHub.IndexedDB.Queries;
using ManuHub.IndexedDB.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace ManuHub.IndexedDB.Context;

public abstract class IndexedDbContext
{
    private readonly IndexedDbOptions _options;
    private IJSRuntime? _js;
    private IJSObjectReference? _module;
    private bool _initialized;
    private readonly SemaphoreSlim _lock = new(1, 1);

    internal IServiceProvider Services { get; set; } = default!;

    protected IndexedDbContext(IndexedDbOptions options, ILogger? logger = null)
    {
        _options = options;
    }

    protected abstract IEnumerable<Type> GetEntityTypes();

    // -----------------------------------------------------
    // JS RUNTIME RESOLUTION
    // -----------------------------------------------------
    private IJSRuntime JS => _js ??= Services.GetRequiredService<IJSRuntime>();

    // -----------------------------------------------------
    // AUTO INITIALIZATION
    // -----------------------------------------------------
    private async Task EnsureReadyAsync()
    {
        if (_initialized)
            return;

        await _lock.WaitAsync();

        try
        {
            if (_initialized)
                return;

            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/ManuHub.IndexedDB/indexeddb.js");

            var stores = GetEntityTypes()
                .Select(StoreDefinition.FromType)
                .ToList();

            await _module.InvokeVoidAsync(
                "initializeDatabase",
                _options.DatabaseName,
                _options.Version,
                stores);

            _initialized = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    // -----------------------------------------------------
    // SAFE MODULE ACCESS
    // -----------------------------------------------------
    internal async Task<IJSObjectReference> Module()
    {
        await EnsureReadyAsync();
        return _module!;
    }

    // -----------------------------------------------------
    // STORE ACCESS
    // -----------------------------------------------------
    protected IndexedSet<T> Set<T>(string storeName)
    {
        _ = EnsureReadyAsync();

        return new IndexedSet<T>(
            Module,
            _options.DatabaseName,
            storeName);
    }

    // -----------------------------------------------------
    // QUERY
    // -----------------------------------------------------
    protected IndexedQuery<T> Query<T>(string storeName)
    {
        _ = EnsureReadyAsync();

        return new IndexedQuery<T>(
            Module,
            _options.DatabaseName,
            storeName);
    }
}