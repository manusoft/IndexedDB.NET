namespace ManuHub.IndexedDB.Core;

public sealed class IndexedDbException : Exception
{
    public IndexedDbException(string message) : base(message) { }
}