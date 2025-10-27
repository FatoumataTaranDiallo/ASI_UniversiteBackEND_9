namespace UniversiteDomain.Exceptions.UeExceptions;

[Serializable]
public class InvalidUeException : Exception
{
    public InvalidUeException() : base() { }
    public InvalidUeException(string message) : base() { }
    public InvalidUeException(string message, Exception inner) : base() { }  

}