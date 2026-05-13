namespace ManuHub.IndexedDB.Exceptions;

public sealed class DuplicateKeyException : IndexedDbException
{
    public DuplicateKeyException(string message) : base(message) { }
}