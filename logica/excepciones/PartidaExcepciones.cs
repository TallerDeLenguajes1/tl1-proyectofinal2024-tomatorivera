namespace Logica.Excepciones
{
    public class PartidaDuplicadaException : Exception
    {
        public string PartidaDirectorio { get; set; }

        public PartidaDuplicadaException(string msg, string PartidaDirectorio) : base(msg)
        {
            this.PartidaDirectorio = PartidaDirectorio;
        }
    }

    public class DirectorioPartidaInvalidoException : Exception
    {
        public DirectorioPartidaInvalidoException(string msg) : base(msg)
        {}
    }
}