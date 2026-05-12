using ManuHub.IndexedDB.Metadata;
using ManuHub.IndexedDB.Queries;
using ManuHub.IndexedDB.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace ManuHub.IndexedDB.Context;

public abstract class IndexedDbContext
{
    private readonly IJSRuntime _js;
    private readonly IndexedDbOptions _options;

    private IJSObjectReference? _module;
    private bool _initialized;
    private readonly SemaphoreSlim _lock = new(1, 1);

    protected IndexedDbContext(IJSRuntime js, IndexedDbOptions options, ILogger? logger = null)
    {
        _js = js;
        _options = options;
    }

    protected abstract IEnumerable<Type> GetEntityTypes();

    // -----------------------------------------------------
    // AUTO INITIALIZATION (SAFE)
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

            _module = await _js.InvokeAsync<IJSObjectReference>(
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

    private async Task<IJSObjectReference> Module()
    {
        await EnsureReadyAsync();
        return _module!;
    }

    // -----------------------------------------------------
    // STORE ACCESS (FIXED - NO ERRORS)
    // -----------------------------------------------------

    protected IndexedSet<T> Set<T>(string storeName)
    {
        // IMPORTANT: DO NOT block UI
        _ = EnsureReadyAsync();

        return new IndexedSet<T>(
            Module, // pass function, not object
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