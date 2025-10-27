namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class DuplicateUeDansParcoursException : Exception
{
    public DuplicateUeDansParcoursException() : base() { }
    public DuplicateUeDansParcoursException(string message) : base() { }
    public DuplicateUeDansParcoursException(string message, Exception inner) : base() { } 
}