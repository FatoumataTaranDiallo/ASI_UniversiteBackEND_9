namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class InvalidNoteException : Exception
{
    public InvalidNoteException() { }
    public InvalidNoteException(string message) : base(message) { }
    public InvalidNoteException(string message, Exception inner) : base(message, inner) { }
}



















