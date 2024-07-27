namespace Logica.Excepciones
{
    public class ConfiguracionInexistenteException : VoleyballManagerRuntimeException
    {
        public ConfiguracionInexistenteException(string message) : base(message)
        {}
    }

    public class DeserealizacionNulaException : VoleyballManagerRuntimeException
    {
        public DeserealizacionNulaException(string message) : base(message)
        {}
    }

    public class ApiInaccesibleException : VoleyballManagerRuntimeException
    {
        public object? RespuestaApi { get; set; }

        public ApiInaccesibleException (string message) : base(message)
        {
            this.RespuestaApi = null;
        }

        public ApiInaccesibleException (string message, object RespuestaApi) : base(message)
        {
            this.RespuestaApi = RespuestaApi;
        }
    }

    public class RespuestaApiInvalidaException : VoleyballManagerRuntimeException
    {
        public RespuestaApiInvalidaException(string message) : base(message)
        {}
    }
}