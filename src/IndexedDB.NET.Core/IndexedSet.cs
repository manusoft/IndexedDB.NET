using Microsoft.JSInterop;

namespace IndexedDB.NET.Core;

public sealed class IndexedSet<T>
{
    private readonly IJSObjectReference _module;
    private readonly string _databaseName;
    private readonly string _storeName;

    public IndexedSet(IJSObjectReference module, string databaseName, string storeName)
    {
        _module = module;
        _databaseName = databaseName;
        _storeName = storeName;
    }

    public async ValueTask AddAsync(T entity)
    {
        await _module.InvokeVoidAsync("add", _databaseName, _storeName, entity);
    }

    public async ValueTask PutAsync(T entity)
    {
        await _module.InvokeVoidAsync("put", _databaseName, _storeName, entity);
    }

    public async ValueTask<T?> GetAsync(object key)
    {
        return await _module.InvokeAsync<T?>("get", _databaseName, _storeName, key);
    }

    public async ValueTask<List<T>> GetAllAsync()
    {
        return await _module.InvokeAsync<List<T>>("getAll", _databaseName, _storeName);
    }

    public async ValueTask DeleteAsync(object key)
    {
        await _module.InvokeVoidAsync("deleteRecord", _databaseName, _storeName, key);
    }

    public async ValueTask ClearAsync()
    {
        await _module.InvokeVoidAsync("clear", _databaseName, _storeName);
    }

    public async ValueTask<int> CountAsync()
    {
        return await _module.InvokeAsync<int>("count", _databaseName, _storeName);
    }
}
