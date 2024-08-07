using Logica.Handlers;
using Logica.Modelo;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Logica.Servicios
{
    public interface IRecursoServicio
    {
        string ObtenerLogo();
        AudioHandler ObtenerManejadorAudio();
    }

    public class RecursoServicioImpl : IRecursoServicio
    {
        /// <summary>
        /// Obtiene el Path completo del archivo 'Logo' o lanza una excepción en caso de que haya problemas con el path
        /// </summary>
        /// <returns>Path del archivo logo.png</returns>
        public string ObtenerLogo()
        {
            try 
            {
                string pathArchivoLogo = Config.DirectorioImagenes + @"\" + Config.NombreImgLogo;

                // Si el path no existe ya sea por problemas con el directorio o el archivo, este método lanzará una excepción
                RecursosUtil.VerificarArchivo(pathArchivoLogo);
                
                return pathArchivoLogo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retorna la instancia del manejador de audio utilizado
        /// </summary>
        /// <returns>Instancia de <c>AudioHandler</c></returns>
        public AudioHandler ObtenerManejadorAudio()
        {
            return AudioHandler.Instancia;
        }
    }
}