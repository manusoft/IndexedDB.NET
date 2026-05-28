namespace ManuHub.IndexedDB.Exceptions;

public class IndexedDbException : Exception
{
    public IndexedDbException(string message, Exception innerException) : base(message, innerException) { }
}
