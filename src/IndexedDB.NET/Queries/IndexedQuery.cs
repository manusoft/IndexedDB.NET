using Microsoft.JSInterop;

namespace ManuHub.IndexedDB.Queries;

public sealed class IndexedQuery<T>
{
    private readonly Func<Task<IJSObjectReference>> _moduleFactory;

    private readonly string _databaseName;

    private readonly string _storeName;

    private string? _indexName;

    private object? _value;

    public IndexedQuery(Func<Task<IJSObjectReference>> moduleFactory, string databaseName, string storeName)
    {
        _moduleFactory = moduleFactory;
        _databaseName = databaseName;
        _storeName = storeName;
    }

    private async Task<IJSObjectReference> JS()
    {
        var js = await _moduleFactory();

        if (js is null)
            throw new InvalidOperationException("JS module not initialized");

        return js;
    }

    public IndexedQuery<T> WhereEquals(string indexName, object value)
    {
        _indexName = indexName;
        _value = value;

        return this;
    }

    public async ValueTask<List<T>> ToListAsync()
    {
        var js = await JS();

        return await js.InvokeAsync<List<T>>(
            "queryWhereEquals",
            _databaseName,
            _storeName,
            _indexName,
            _value);
    }

    public async ValueTask<T?> FirstOrDefaultAsync()
    {
        var result = await ToListAsync();

        return result.FirstOrDefault();
    }
}