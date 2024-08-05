namespace Logica.Excepciones;

public class MercadoInvalidoException : VoleyballManagerRuntimeException
{
    public MercadoInvalidoException(string message) : base(message)
    {}
}

public class DineroInsuficienteException : VoleyballManagerRuntimeException
{
    public DineroInsuficienteException(string message) : base(message)
    {}
}