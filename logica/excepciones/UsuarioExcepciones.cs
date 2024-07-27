namespace Logica.Excepciones
{
    public class UsuarioNoEspecificadoException : VoleyballManagerRuntimeException
    {
        public UsuarioNoEspecificadoException(string message) : base(message)
        {}
    }

    public class NombreInvalidoException : VoleyballManagerRuntimeException
    {
        public NombreInvalidoException (string message) : base(message)
        {}
    }

    public class UsuarioInvalidoException : VoleyballManagerRuntimeException
    {
        public UsuarioInvalidoException (string message) : base(message)
        {}
    }
}