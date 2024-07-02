using Logica.Excepciones;
using Logica.Modelo;
using Persistencia.Infraestructura;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de usuarios en el juego.
    /// </summary>
    public interface UsuarioServicio
    {
        /// <summary>
        /// Crea un nuevo usuario y todos los archivos necesarios
        /// </summary>
        void CrearUsuario();

        /// <summary>
        /// Almacena temporalmente el nombre del usuario en los archivos de configuración
        /// </summary>
        /// <param name="nombreUsuario">Nombre del nuevo usuario</param>
        void AlmacenarUsuario(string nombreUsuario);

        /// <summary>
        /// Obtiene los datos del usuario actual
        /// </summary>
        /// <returns>Objeto <c>Usuario</c> con los datos del jugador actual</returns>
        Usuario ObtenerDatosUsuario();
    }

    public class UsuarioServicioImpl : UsuarioServicio
    {
        public void CrearUsuario()
        {
            // Falta implementar la persistencia
            Usuario nuevoUsuario;

            try
            {
                if (Config.NombreUsuarioActual == null)
                {
                    throw new UsuarioNoEspecificadoException("No se ha especificado un nombre de usuario");
                }
                
                nuevoUsuario = new Usuario(Config.NombreUsuarioActual);
                
                // Falta implementar la persistencia
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AlmacenarUsuario(string nombreUsuario)
        {
            Config.NombreUsuarioActual = nombreUsuario;
        }

        public Usuario ObtenerDatosUsuario()
        {
            // Falta implementar la obtención de datos de la persistencia
            return new Usuario("Usuario");
        }
    }
}