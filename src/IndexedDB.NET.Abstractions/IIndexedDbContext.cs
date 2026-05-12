namespace IndexedDB.NET.Abstractions;

public interface IIndexedDbContext
{
    ValueTask InitializeAsync();
}