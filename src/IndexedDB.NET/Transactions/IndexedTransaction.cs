using Microsoft.JSInterop;

namespace ManuHub.IndexedDB.Transactions;

public sealed class IndexedTransaction : IAsyncDisposable
{
    private readonly IJSObjectReference _module;

    public string TransactionId { get; }

    internal IndexedTransaction(IJSObjectReference module, string transactionId)
    {
        _module = module;
        TransactionId = transactionId;
    }

    public async ValueTask CommitAsync()
    {
        await _module.InvokeVoidAsync("commitTransaction", TransactionId);
    }

    public async ValueTask AbortAsync()
    {
        await _module.InvokeVoidAsync("abortTransaction", TransactionId);
    }

    public async ValueTask DisposeAsync()
    {
        await AbortAsync();
    }
}