namespace Logica.Excepciones
{
    public class UsuarioNoEspecificadoException : Exception
    {
        public UsuarioNoEspecificadoException(string message) : base(message)
        {}
    }

    public class UsernameInvalidoException : Exception
    {
        public UsernameInvalidoException (string message) : base(message)
        {}
    }
}