namespace Logica.Excepciones;

public abstract class VoleyballManagerRuntimeException : Exception
{
    public VoleyballManagerRuntimeException(string message) : base(message)
    {}
}