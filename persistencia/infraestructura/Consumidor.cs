using Logica.Excepciones;
using Newtonsoft.Json;

namespace Persistencia.Infraestructura
{
    /// <summary>
    /// Clase encargada de consumir API's y deserealizar en el tipo de dato provisto
    /// </summary>
    /// <typeparam name="T">Tipo del objeto sobre el que se deserealizará</typeparam>
    public class Consumidor<T> where T : IConsumido
    {
        private string apiUrl;
        private string subUrl;
        private string parametros;
        private int nParametros;
        private Dictionary<string, string> cabeceras;

        public Consumidor(string apiUrl)
        {
            this.apiUrl = apiUrl;
            this.subUrl = string.Empty;
            this.parametros = string.Empty;
            this.nParametros = 0;
            this.cabeceras = new Dictionary<string, string>();
        }

        /// <summary>
        /// Agrega un parámetro a la url
        /// </summary>
        /// <param name="parametro">Nombre del parámetro</param>
        /// <param name="valor">Valor del parámetro (opcional, puesto que hay parametros que no son acompañados por valores)</param>
        public void AgregarParametro(string parametro, string valor = "")
        {
            // Si no hay parámetros aún, se inicializa con '&', caso contrario se agrega el parámetro con '?'
            this.parametros += ((nParametros > 0) ? "&" : "?") + parametro + ((!string.IsNullOrWhiteSpace(valor)) ? "=" + valor : "");
            this.nParametros++;
        }

        /// <summary>
        /// Agrega una cabecera para ser insertada en la petición HTTP
        /// </summary>
        /// <param name="parametro">Nombre de la cabecera</param>
        /// <param name="valor">Valor de la cabecera</param>
        public void AgregarCabecera(string parametro, string valor)
        {
            this.cabeceras.Add(parametro, valor);
        }

        public void AgregarSubUrl(string subUrl)
        {
            this.subUrl += "/" + subUrl;
        }

        /// <summary>
        /// Consume una API de manera asincrónica
        /// </summary>
        /// <returns>Respuesta de la API deserealizada en un objeto del tipo <c>T<c></returns>
        /// <exception cref="DeserealizacionNulaException"></exception>
        public async Task<T> ConsumirAsync()
        {
            // Junto la url de la api con los parametros agregados
            string apiUrlFull = apiUrl + subUrl + parametros;

            using (HttpClient clienteHttp = new HttpClient())
            {
                // Agrego las cabeceras si es que hay
                foreach (var cabecera in cabeceras)
                    clienteHttp.DefaultRequestHeaders.Add(cabecera.Key, cabecera.Value);

                HttpResponseMessage apiRespuesta = await clienteHttp.GetAsync(apiUrlFull);
                apiRespuesta.EnsureSuccessStatusCode();
                
                var cuerpoRespuesta = await apiRespuesta.Content.ReadAsStringAsync();
                var respuestaDeserealizada = JsonConvert.DeserializeObject<T>(cuerpoRespuesta);
                
                return (respuestaDeserealizada != null) ? respuestaDeserealizada 
                                                        : throw new DeserealizacionNulaException("El resultado de la deserealizacion es nulo");
            }
        }
    }

    /// <summary>
    /// Interfaz que representa un tipo de datos que es consumido desde un servicio externo.
    /// </summary>
    public interface IConsumido {}
}
