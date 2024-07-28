using Logica.Excepciones;
using Logica.Handlers;
using Logica.Modelo;
using Persistencia;
using Persistencia.Repositorios;
using SixLabors.ImageSharp.ColorSpaces;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de la partida
    /// </summary>
    public interface IPartidaServicio
    {
        void CrearPartida(Partida partida);
        Partida ObtenerDatosPartida();
        Partida ObtenerDatosPartida(int id);
        List<Partida> ObtenerPartidas();
        int ObtenerNuevoIdPartida();
        PartidaHandler ObtenerManejadorPartida(Partida partida);
        void GuardarPartida(Partida partida);
    }

    public class PartidaServicioImpl : IPartidaServicio
    {
        private readonly IRepositorioNavegable<Partida> repositorio;
        private readonly IUsuarioServicio usuarioServicio;
        private readonly IHistorialServicio historialServicio;

        public PartidaServicioImpl()
        {
            repositorio = new PartidaRepositorioImpl();
            usuarioServicio = new UsuarioServicioImpl();
            historialServicio = new HistorialServicioImpl();
        }

        /// <summary>
        /// Crea una nueva partida en los archivos de persistencia
        /// </summary>
        /// <param name="partida">Datos de la partida a guardar</param>
        public void CrearPartida(Partida partida)
        {
            try 
            {
                repositorio.Crear(partida);
                usuarioServicio.CrearUsuario(partida.Usuario);
                historialServicio.CrearHistorial(partida.Historial);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene los datos de la partida actual
        /// </summary>
        /// <returns>Objeto <c>Partida</c></returns>
        public Partida ObtenerDatosPartida()
        {
            try
            {
                return repositorio.ObtenerActual();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los datos de una partida según un ID provisto
        /// </summary>
        /// <param name="id">ID de la partida a cargar</param>
        /// <returns>Objeto <c>Partida</c></returns>
        public Partida ObtenerDatosPartida(int id)
        {
            try
            {
                // Cargo los datos de la partida
                var partida = repositorio.Cargar(id);
                var usuario = usuarioServicio.ObtenerDatosUsuario(id);
                var historial = historialServicio.ObtenerDatosHistorial(id);

                // Vinculo los datos a la partida
                partida.Usuario = usuario;
                partida.Historial = historial;

                return partida;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Genera un nuevo ID para una nueva partida
        /// </summary>
        /// <returns><c>int</c> con el ID generado</returns>
        /// <exception cref="Exception">Cuando ocurre algún error en la creación de directorios</exception>
        /// <exception cref="ConfiguracionInexistenteException">Cuando las configuraciones necesarias no están cargadas</exception>
        public int ObtenerNuevoIdPartida()
        {
            try
            {
                int nuevoId = 1;

                // Obtengo una lista de directorios
                var partidasDirs = repositorio.ObtenerDirectorios();
                if (partidasDirs.Any())
                {
                    // A partir de una lista de strings con nombres de los subdirectorios de la carpeta partidas
                    var partidasIds = partidasDirs.Select(nombreCarpeta => nombreCarpeta.Split('-')[1])         
                                                  // Obtengo el ID de la partida
                                                  .Select(idStr => int.TryParse(idStr, out int id) ? id : -1)
                                                  // Mapeo la lista de strings a una lista de int con los ID's casteables, a los que tengan errores los dejo como -1
                                                  .Where(id => id != -1)
                                                  .ToList();

                    // Si no hubiesen carpetas con ID's válidos, se retorna '1'
                    // caso contrario se retorna el ID máximo más 1
                    if (partidasIds.Any())
                    {
                        nuevoId = partidasIds.Max() + 1;
                    }
                }

                return nuevoId;
            }
            catch (ConfiguracionInexistenteException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error inesperado: " + e.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista con el ID, fecha de guardado y usuario asociado
        /// de todas las partidas guardadas
        /// </summary>
        /// <returns>Objeto <c>List</c> de <c>string</c> con los nombres de los directorios de las partidas</returns>
        public List<Partida> ObtenerPartidas()
        {
            try
            {
                // Mapeo la lista de string con directorios a una lista de objetos Partida
                // Tengo en cuenta que los Parse nunca darán error, ya que 'ObtenerDirectorios' se encarga
                // de obtener solo los directorios correctos utilizando una expresión regular
                return repositorio.ObtenerDirectorios()
                    .Select(nombreDirectorio => {
                        string[] datos = nombreDirectorio.Split("-");

                        int idPartida = int.Parse(datos[1]);
                        DateTime fechaCreacion = DateTime.ParseExact(datos[2], "ddMMyyyy", null);
                        string nombreUsuario = datos[3];

                        return new Partida(idPartida, fechaCreacion, new Usuario(nombreUsuario)); 
                    })
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo manejador de partida para una partida en específico
        /// </summary>
        /// <param name="partida">Partida mediante la cual generar</param>
        /// <returns>Nueva instancia de <c>PartidaHandler</c></returns>
        public PartidaHandler ObtenerManejadorPartida(Partida partida)
        {
            return new PartidaHandler(partida);
        }

        /// <summary>
        /// Guarda los datos de una partida en los archivos de persistencia
        /// </summary>
        /// <param name="partida">Datos de la partida</param>
        public void GuardarPartida(Partida partida)
        {
            try
            {
                // Actualizo la fecha de guardado de la partida
                partida.FechaGuardado = DateTime.Now;

                repositorio.Guardar(partida);
                usuarioServicio.GuardarUsuario(partida.Usuario);
                historialServicio.GuardarHistorial(partida.Historial);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}