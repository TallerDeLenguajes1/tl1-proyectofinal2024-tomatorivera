using System.Text.RegularExpressions;
using Logica.Excepciones;
using Logica.Modelo;
using Persistencia;
using Persistencia.Infraestructura;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de usuarios en el juego.
    /// </summary>
    public interface UsuarioServicio
    {
        void CrearUsuario(Usuario usuario);
        Usuario ObtenerDatosUsuario();
        bool ValidarNombreUsuario(string username);
    }

    public class UsuarioServicioImpl : UsuarioServicio
    {
        private readonly IRepositorio<Usuario> repositorio;
        private readonly EquipoJugadoresServicio equipoServicio;

        public UsuarioServicioImpl()
        {
            this.repositorio = new UsuarioRepositorioImpl();
            this.equipoServicio = new EquipoJugadoresServicioImpl();
        }

        /// <summary>
        /// Crea un nuevo usuario y todos los archivos necesarios
        /// </summary>
        public void CrearUsuario(Usuario usuario)
        {
            try
            {
                repositorio.Crear(usuario);
            }
            catch (Exception)
            {
                throw;
            }
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
        /// <exception cref="NombreInvalidoException">Cuando el nombre de usuario no cumpla con los requerimentos</exception>
        public bool ValidarNombreUsuario(string nombreUsuario)
        {
            // Valido la longitud del nombre
            if (!(nombreUsuario.Length >= 3 && nombreUsuario.Length <= 15))
                throw new NombreInvalidoException("El nombre de usuario debe tener de 3 a 15 caracteres");

            // Regex para validar si contiene solo carácteres alfanuméricos
            Regex rgx = new Regex("^[a-zA-Z]+$");
            
            if (!rgx.IsMatch(nombreUsuario))
                throw new NombreInvalidoException("El nombre de usuario debe tener solo caracteres alfabeticos");

            // Si pasó las validaciones anteriores, el usuario es válido
            return true;
        }
    }
}