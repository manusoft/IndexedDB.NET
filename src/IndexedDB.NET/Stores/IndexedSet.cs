using ManuHub.IndexedDB.Queries;
using Microsoft.JSInterop;

namespace ManuHub.IndexedDB.Stores;

public sealed class IndexedSet<T>
{
    private readonly Func<Task<IJSObjectReference>> _moduleFactory;

    private readonly string _databaseName;

    private readonly string _storeName;

    public IndexedSet(Func<Task<IJSObjectReference>> moduleFactory, string databaseName, string storeName)
    {
        _moduleFactory = moduleFactory;
        _databaseName = databaseName;
        _storeName = storeName;
    }

    private async Task<IJSObjectReference> JS()
    {
        var module = await _moduleFactory();

        if (module is null)
            throw new InvalidOperationException("JS module not ready");

        return module;
    }

    public IndexedQuery<T> Query()
    {
        return new IndexedQuery<T>(
            JS,
            _databaseName,
            _storeName);
    }

    public async ValueTask AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        await js.InvokeVoidAsync(
            "add",
            cancellationToken,
            _databaseName,
            _storeName,
            entity);
    }

    public async ValueTask AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        await js.InvokeVoidAsync(
            "addRange",
            cancellationToken,
            _databaseName,
            _storeName,
            entities);
    }

    public async ValueTask UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        await js.InvokeVoidAsync(
            "put",
            cancellationToken,
            _databaseName,
            _storeName,
            entity);
    }

    public async ValueTask UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        await js.InvokeVoidAsync(
            "putRange",
            cancellationToken,
            _databaseName,
            _storeName,
            entities);
    }

    public async ValueTask DeleteAsync(object key, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        await js.InvokeVoidAsync(
            "remove",
            cancellationToken,
            _databaseName,
            _storeName,
            key);
    }

    public async ValueTask DeleteRangeAsync<TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        await js.InvokeVoidAsync(
            "removeRange",
            cancellationToken,
            _databaseName,
            _storeName,
            keys);
    }

    public async ValueTask<T?> GetAsync(object key, CancellationToken cancellationToken = default)
    {
        var js = await JS();

        return await js.InvokeAsync<T?>(
            "get",
            cancellationToken,
            _databaseName,
            _storeName,
            key);
    }

    public async ValueTask<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var js = await JS();

        return await js.InvokeAsync<List<T>>(
            "getAll",
            cancellationToken,
            _databaseName,
            _storeName);
    }

    public async IAsyncEnumerable<T> AsAsyncEnumerable()
    {
        var js = await JS();

        var data = await js.InvokeAsync<List<T>>(
            "getAll",
            _databaseName,
            _storeName);

        foreach (var item in data)
            yield return item;
    }
}