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
    public interface IUsuarioServicio
    {
        void CrearUsuario(Usuario usuario);
        Usuario ObtenerDatosUsuario();
    }

    public class UsuarioServicioImpl : IUsuarioServicio
    {
        private readonly IRepositorio<Usuario> repositorio;

        public UsuarioServicioImpl()
        {
            this.repositorio = new UsuarioRepositorioImpl();
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
    }
}