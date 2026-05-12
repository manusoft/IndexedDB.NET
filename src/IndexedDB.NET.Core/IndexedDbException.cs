namespace IndexedDB.NET.Core;

public sealed class IndexedDbException : Exception
{
    public IndexedDbException(string message) : base(message) { }
}