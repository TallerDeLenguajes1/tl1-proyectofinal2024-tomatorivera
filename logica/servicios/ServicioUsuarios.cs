using Logica.Modelo;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de usuarios en el juego.
    /// </summary>
    public interface UsuarioServicio
    {
        /// <summary>
        /// Crea un nuevo usuario a partir del nombre ingresado por el jugador
        /// </summary>
        /// <param name="nombreUsuario">Nombre del nuevo usuario.</param>
        void CrearUsuario(string nombreUsuario);

        /// <summary>
        /// Obtiene los datos del usuario actual
        /// </summary>
        /// <returns>Objeto 'Usuario' con los datos del jugador actual</returns>
        Usuario ObtenerDatosUsuario();
    }

    public class UsuarioServicioImpl : UsuarioServicio
    {
        public void CrearUsuario(string nombreUsuario)
        {
            // Falta implementar la persistencia
        }

        public Usuario ObtenerDatosUsuario()
        {
            // Falta implementar la obtención de datos de la persistencia
            return new Usuario("Usuario");
        }
    }
}