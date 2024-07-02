using Logica.Excepciones;
using Logica.Modelo;
using Persistencia;
using Persistencia.Infraestructura;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de la partida
    /// </summary>
    public interface PartidaServicio
    {
        void CrearPartida(Partida partida);
        Partida ObtenerDatosPartida();
        List<Partida> ObtenerPartidas();
        int ObtenerNuevoIdPartida();
    }

    public class PartidaServicioImpl : PartidaServicio
    {
        private readonly IRepositorio<Partida> repositorio;
        private readonly UsuarioServicio usuarioServicio;

        public PartidaServicioImpl()
        {
            this.repositorio = new PartidaRepositorioImpl();
            this.usuarioServicio = new UsuarioServicioImpl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partida"></param>
        public void CrearPartida(Partida partida)
        {
            try 
            {
                repositorio.Crear(partida);
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
            // Falta implementar la persistencia
            return new Partida(1, DateTime.Now, new UsuarioServicioImpl().ObtenerDatosUsuario());
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
                // Obtengo una lista de directorios
                var partidasDirs = ((PartidaRepositorioImpl) repositorio).ObtenerDirectoriosPartidas();
                if (!partidasDirs.Any())
                {
                    return 1;
                }
                else
                {
                    // A partir de una lista de strings con nombres de los subdirectorios de la carpeta partidas
                    var partidasIds = partidasDirs.Select(nombreCarpeta => nombreCarpeta.Split('-')[1])         
                                                  // Obtengo el ID de la partida
                                                  .Select(idStr => int.TryParse(idStr, out int id) ? id : -1)
                                                  // Mapeo la lista de strings a una lista de int con los ID's casteables, a los que tengan errores los dejo como -1
                                                  .Where(id => id != -1)
                                                  .ToList();

                    int nuevoId = 1;
                    // Si no hubiesen carpetas con ID's válidos, se retorna '1'
                    // caso contrario se retorna el ID máximo más 1
                    if (partidasIds.Any())
                    {
                        nuevoId = partidasIds.Max() + 1;
                    }

                    return nuevoId;
                }
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
        /// <returns>Objeto <c>List</c> de <c>Partida</c></returns>
        public List<Partida> ObtenerPartidas()
        {
            // Falta implementar la persistencia
            return new List<Partida> { ObtenerDatosPartida(), new Partida(2, DateTime.Now, new Usuario("Otro usuario")) };
        }
    }
}