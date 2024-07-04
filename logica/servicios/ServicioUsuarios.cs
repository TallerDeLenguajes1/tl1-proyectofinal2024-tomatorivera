using System.Text.RegularExpressions;
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
        void CrearUsuario();
        void AlmacenarUsuario(string nombreUsuario);
        Usuario ObtenerDatosUsuario();
        bool ValidarNombreUsuario(string username);
    }

    public class UsuarioServicioImpl : UsuarioServicio
    {
        /// <summary>
        /// Crea un nuevo usuario y todos los archivos necesarios
        /// </summary>
        /// <exception cref="UsuarioNoEspecificadoException">Si el usuario de algún modo saltado las verificaciones de nombre o el nombre ingresado no se hubiese guardado en la configuración</exception>
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

        /// <summary>
        /// Almacena temporalmente el nombre del usuario en los archivos de configuración
        /// </summary>
        /// <param name="nombreUsuario">Nombre del nuevo usuario</param>
        public void AlmacenarUsuario(string nombreUsuario)
        {
            Config.NombreUsuarioActual = nombreUsuario;
        }

        /// <summary>
        /// Obtiene los datos del usuario actual
        /// </summary>
        /// <returns>Objeto <c>Usuario</c> con los datos del jugador actual</returns>
        public Usuario ObtenerDatosUsuario()
        {
            // Falta implementar la obtención de datos de la persistencia
            return new Usuario("Usuario");
        }

        /// <summary>
        /// Valida si un nombre de usuario es correcto
        /// </summary>
        /// <param name="username">Nombre de usuario a validar</param>
        /// <returns><c>True</c> si el nombre de usuario es válido, <c>False</c> en caso contrario</returns>
        /// <exception cref="UsernameInvalidoException">Cuando el nombre de usuario no cumpla con los requerimentos</exception>
        public bool ValidarNombreUsuario(string nombreUsuario)
        {
            // Valido la longitud del nombre
            if (!(nombreUsuario.Length >= 3 && nombreUsuario.Length <= 15))
                throw new UsernameInvalidoException("El nombre de usuario debe tener de 3 a 15 caracteres");

            // Regex para validar si contiene solo carácteres alfanuméricos
            Regex rgx = new Regex("^[a-zA-Z]+$");
            
            if (!rgx.IsMatch(nombreUsuario))
                throw new UsernameInvalidoException("El nombre de usuario debe tener solo caracteres alfabeticos");

            // Si pasó las validaciones anteriores, el usuario es válido
            return true;
        }
    }
}