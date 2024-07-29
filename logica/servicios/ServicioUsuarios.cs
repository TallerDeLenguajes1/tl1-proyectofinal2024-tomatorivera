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
        void GuardarUsuario(Usuario usuario);
        void EliminarUsuario();
    }

    public class UsuarioServicioImpl : IUsuarioServicio
    {
        private readonly IRepositorio<Usuario> repositorio;

        public UsuarioServicioImpl()
        {
            repositorio = new UsuarioRepositorioImpl();
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

        /// <summary>
        /// Guarda un usuario en los archivos de persistencia de la partida actual
        /// </summary>
        /// <param name="usuario">Datos del Usuario a guardar</param>
        public void GuardarUsuario(Usuario usuario)
        {
            try
            {
                repositorio.Guardar(usuario);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Elimina el usuario actual
        /// </summary>
        public void EliminarUsuario()
        {
            try
            {
                repositorio.Eliminar();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}