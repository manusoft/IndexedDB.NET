namespace ManuHub.IndexedDB.Exceptions;

public sealed class QuotaExceededException : IndexedDbException
{
    public QuotaExceededException(string message, Exception innerException) : base(message, innerException) { }
}