namespace Logica.Excepciones
{
    public class UsuarioNoEspecificadoException : Exception
    {
        public UsuarioNoEspecificadoException(string message) : base(message)
        {}
    }

    public class NombreInvalidoException : Exception
    {
        public NombreInvalidoException (string message) : base(message)
        {}
    }

    public class UsuarioInvalidoException : Exception
    {
        public UsuarioInvalidoException (string message) : base(message)
        {}
    }
}