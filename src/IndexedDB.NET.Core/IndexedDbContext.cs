using Microsoft.JSInterop;

namespace IndexedDB.NET.Core;

public abstract class IndexedDbContext
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IndexedDbOptions _options;

    private IJSObjectReference? _module;

    protected IndexedDbContext(IJSRuntime jsRuntime, IndexedDbOptions options)
    {
        _jsRuntime = jsRuntime;
        _options = options;
    }

    protected async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        if (_module is not null) return _module;

        _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/IndexedDB.NET.Blazor/indexeddb.js");

        return _module;
    }

    public async ValueTask InitializeAsync()
    {
        var module = await GetModuleAsync();

        await module.InvokeVoidAsync("initializeDatabase", _options.DatabaseName, _options.Version, GetStores());
    }

    protected abstract List<StoreDefinition> GetStores();

    protected IndexedSet<T> Set<T>(string storeName)
    {
        if (_module is null)
            throw new InvalidOperationException("InitializeAsync must be called first.");

        return new IndexedSet<T>(_module, _options.DatabaseName, storeName);
    }
}
