using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Logica.Servicios
{
    public interface RecursoServicio
    {
        string ObtenerLogo();
    }

    public class RecursoServicioImpl : RecursoServicio
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
    }
}