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
        Usuario ObtenerDatosUsuario(int id);
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

        /// <summary>
        /// Obtiene los datos del usuario perteneciente a la partida de ID: <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID de la partida donde buscaré al usuario</param>
        /// <returns>Objeto <c>Usuario</c></returns>
        public Usuario ObtenerDatosUsuario(int id)
        {
            try
            {
                return repositorio.Cargar(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}