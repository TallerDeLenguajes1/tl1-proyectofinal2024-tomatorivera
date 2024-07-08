namespace Logica.Excepciones
{
    public class ConfiguracionInexistenteException : Exception
    {
        public ConfiguracionInexistenteException(string message) : base(message)
        {}
    }

    public class DeserealizacionNulaException : Exception
    {
        public DeserealizacionNulaException(string message) : base(message)
        {}
    }

    public class ApiInaccesibleException : Exception
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
}