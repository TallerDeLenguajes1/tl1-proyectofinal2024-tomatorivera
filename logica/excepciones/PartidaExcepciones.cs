namespace Logica.Excepciones
{
    public class PartidaDuplicadaException : VoleyballManagerRuntimeException
    {
        public string PartidaDirectorio { get; set; }

        public PartidaDuplicadaException(string msg, string PartidaDirectorio) : base(msg)
        {
            this.PartidaDirectorio = PartidaDirectorio;
        }
    }

    public class PartidaInvalidaException : VoleyballManagerRuntimeException
    {
        public PartidaInvalidaException(string msg) : base(msg)
        {}
    }

    public class DirectorioPartidaInvalidoException : VoleyballManagerRuntimeException
    {
        public DirectorioPartidaInvalidoException(string msg) : base(msg)
        {}
    }

    public class MaximoPartidasAlcanzadoException : VoleyballManagerRuntimeException
    {
        public MaximoPartidasAlcanzadoException(string msg) : base(msg)
        {}
    }
}