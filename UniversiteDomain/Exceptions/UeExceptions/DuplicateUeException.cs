namespace UniversiteDomain.Exceptions.UeExceptions;

[Serializable]
public class DuplicateUeException : Exception
{
    public DuplicateUeException() : base() { }
    public DuplicateUeException(string message) : base() { }
    public DuplicateUeException(string message, Exception inner) : base() { }  

 
}