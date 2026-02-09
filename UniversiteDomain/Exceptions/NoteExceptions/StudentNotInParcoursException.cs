namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class StudentNotInParcoursException : Exception
{
    public StudentNotInParcoursException() { }

    public StudentNotInParcoursException(string message) : base(message) { }

    public StudentNotInParcoursException(string message, Exception inner) : base(message, inner) { }

}
