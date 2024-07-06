using Logica.Excepciones;

namespace Persistencia.Util
{
    public static class RecursosUtil
    {
        /// <summary>
        /// Verifica si un directorio existe, si no existe lo crea
        /// </summary>
        /// <param name="path">Path del directorio a verificar</param>
        /// <exception cref="DirectorioInvalidoException">Cuando el path es null o vacío</exception>
        public static void VerificarDirectorio(string path)
        {
            // Verifico si el path enviado por parámetro es válido
            if (String.IsNullOrWhiteSpace(path))
                throw new PathInvalidoException("El path '"+path+"' no puede ser nulo o estar vacio");

            // Si el directorio no existe, lo creo
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Verifica si un archivo existe, caso contrario lanzo una excepción
        /// </summary>
        /// <param name="path">Path del archivo</param>
        /// <exception cref="PathInvalidoException">En caso de que el archivo no existe</exception>
        public static void VerificarArchivo(string path)
        {
            VerificarDirectorio(Path.GetDirectoryName(path) ?? string.Empty);

            // No todos los tipos de archivos pueden ser creados, por ejemplo, los de imagen
            if (!File.Exists(path))
                throw new PathInvalidoException("El archivo del path '"+path+"' no existe");
        }
    }
}