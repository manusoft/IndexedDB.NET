namespace ManuHub.IndexedDB.Abstractions;

public interface IIndexedDbContext
{
    ValueTask InitializeAsync();
}