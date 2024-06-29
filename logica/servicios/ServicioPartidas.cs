using Logica.Modelo;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de la partida
    /// </summary>
    public interface PartidaServicio
    {
        /// <summary>
        /// Crea una nueva partida y todos los archivos necesarios
        /// </summary>
        void CrearPartida();

        /// <summary>
        /// Obtiene los datos de la partida actual
        /// </summary>
        /// <returns>Objeto <c>Partida</c></returns>
        Partida ObtenerDatosPartida();

        /// <summary>
        /// Obtiene una lista con todas las partidas guardadas
        /// </summary>
        /// <returns>Objeto <c>List</c> de <c>Partida</c></returns>
        List<Partida> ObtenerPartidas();
    }

    public class PartidaServicioImpl : PartidaServicio
    {
        public void CrearPartida()
        {
            // Falta implementar la persistencia
        }

        public Partida ObtenerDatosPartida()
        {
            // Falta implementar la persistencia
            return new Partida(1, DateTime.Now, new UsuarioServicioImpl().ObtenerDatosUsuario());
        }

        public List<Partida> ObtenerPartidas()
        {
            // Falta implementar la persistencia
            return new List<Partida> { ObtenerDatosPartida(), new Partida(2, DateTime.Now, new Usuario("Otro usuario")) };
        }
    }
}