using Logica.Excepciones;
using Logica.Modelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Persistencia.Repositorios
{
    public class HistorialRepositorio : IRepositorio<Historial>
    {
        private static Historial? historialActual { get; set; }

        /// <value>Contiene las propiedades que se excluirán del JSON correspondiente al usuario</value>
        private DefaultContractResolver historialContractResolver = new ExclusionPropiedadesJson(["jugadores", "total_jugadores"]);

        /// <summary>
        /// Crea un nuevo historial en los archivos de persistencia
        /// </summary>
        /// <param name="obj">Objeto <c>Historial</c></param>
        /// <exception cref="PartidaInvalidaException">En caso de que no haya un directorio de la partida actual donde persistir</exception>
        public void Crear(Historial obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
            
            string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonHistorial}";

            // Creo el archivo del historial
            using (FileStream historialJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter historialWriter = new StreamWriter(historialJson))
                {
                    string historialSerializado = JsonConvert.SerializeObject(obj,
                                                                              new JsonSerializerSettings {
                                                                                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                                                                                Formatting = Formatting.Indented,
                                                                                ContractResolver = historialContractResolver
                                                                              });
                    historialWriter.WriteLine(historialSerializado);
                }
            }

            // Actualizo la instancia del historial actual
            historialActual = obj;
        }
        
        /// <summary>
        /// Carga los datos del archivo de persistencia de historial correspondiente
        /// a la partida con el ID <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID de la partida</param>
        /// <returns>Objeto <c>Historial</c></returns>
        /// <exception cref="PartidaInvalidaException">En caso de que no haya una partida actual de la cual obtener archivos de persistencia</exception>
        /// <exception cref="HistorialInvalidoException">En caso de que la respuesta de la deserealización del historial sea nula</exception>
        public Historial Cargar(int id)
        {
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudieron cargar los datos del historial. El directorio de la partida a cargar es nulo o está vacío");

            var historialJsonPath = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonHistorial}";

            RecursosUtil.VerificarArchivo(historialJsonPath);

            // Leo y deserealizo el archivo del historial
            Historial? historial;
            using (FileStream fileHistorial = new FileStream(historialJsonPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader readerHistorial = new StreamReader(fileHistorial))
                {
                    string historialJsonTxt = readerHistorial.ReadToEnd();
                    historial = JsonConvert.DeserializeObject<Historial>(historialJsonTxt);

                    if (historial == null)
                        throw new HistorialInvalidoException("No se pudo deserealizar el archivo JSON del historial, la respuesta ha sido nula");
                }
            }

            // Actualizo la instancia del historial actual en el repositorio
            historialActual = historial;

            return historial;
        }

        /// <summary>
        /// Guarda el historial en los archivos de persistencia
        /// </summary>
        /// <param name="obj">Objeto <c>Historial</c> a almacenar</param>
        /// <exception cref="PartidaInvalidaException">En caso de que no haya una partida actual de la cual obtener archivos de persistencia</exception>
        public void Guardar(Historial obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
            
            string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonHistorial}";

            // Creo el archivo del historial
            using (FileStream historialJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter historialWriter = new StreamWriter(historialJson))
                {
                    string historialSerializado = JsonConvert.SerializeObject(obj,
                                                                              new JsonSerializerSettings {
                                                                                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                                                                                Formatting = Formatting.Indented,
                                                                                ContractResolver = historialContractResolver
                                                                              });
                    historialWriter.WriteLine(historialSerializado);
                }
            }
        }

        /// <summary>
        /// Obtiene la instancia del historial correspondiente a la partida actual
        /// </summary>
        /// <returns>Objeto <c>Historial</c></returns>
        /// <exception cref="HistorialInvalidoException">Cuando la instancia del historial sea nula</exception>
        public Historial ObtenerActual()
        {
            if (historialActual == null)
                throw new HistorialInvalidoException("No existe una instancia del historial actual en el repositorio");

            return historialActual;
        }
    }
}