using System.Text.Json;
using System.Text.RegularExpressions;
using Logica.Excepciones;
using Logica.Modelo;
using Persistencia.Infraestructura;

namespace Persistencia.Repositorios
{
    public class PartidaRepositorioImpl : IRepositorio<Partida>
    {
        public Partida? partidaActual { get; set; }

        /// <summary>
        /// Crea un nuevo archivo de persistencia para <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Objeto <c>Partida</c></param>
        /// <exception cref="PartidaDuplicadaException">Excepción lanzada cuando ya existe una carpeta con el mismo nombre de la que se va a crear</exception>
        public void Crear(Partida obj)
        {
            verificarDirectorioPartidas();

            string nuevaPartidaDir = Config.DirectorioPartidas + @"\partida-" + obj.Id + "-" + obj.FechaGuardado.ToString("ddMMyyyy") + "-" + obj.Usuario.NombreUsuario;

            // Verifico si por alguna razón ya existe un directorio con el nombre de la nueva partida
            if (Directory.Exists(nuevaPartidaDir))
            {
                throw new PartidaDuplicadaException("Ya existe una partida con este ID", nuevaPartidaDir);
            }
            
            // Creo el directorio de la nueva partida
            Directory.CreateDirectory(nuevaPartidaDir);

            // Creo el archivo de la partida
            using (FileStream partidaJson = new FileStream(nuevaPartidaDir + @"\" + Config.NombreJsonPartida, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(partidaJson))
                {
                    string partidaSerializada = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                    writer.WriteLine(partidaSerializada);
                }
            }

            // Actualizo el directorio de la partida actual en el archivo de configuración
            Config.DirectorioPartidaActual = nuevaPartidaDir;
            // Actualizo la instancia de la partida actual para manejar los datos desde el programa
            this.partidaActual = obj;
        }

        public void Guardar(Partida obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene la instancia de la partida actual cargada o creada por el usuario
        /// </summary>
        /// <returns>Objeto <c>Partida</c> con los datos de la partida actual</returns>
        /// <exception cref="PartidaInvalidaException">En caso de que se solicite una partida cuando la instancia aún sea null</exception>
        public Partida ObtenerActual()
        {
            if (partidaActual == null) 
                throw new PartidaInvalidaException("No existe una instancia de la partida actual en el repositorio");

            return partidaActual;
        }

        /// <summary>
        /// Obtiene una lista de string con los nombres válidos de directorios de partidas
        /// </summary>
        /// <returns><c>List</c> de <c>string</c> con los nombres de los directorios</returns>
        public List<string> ObtenerDirectoriosPartidas()
        {
            verificarDirectorioPartidas();

            // Regex para los nombres de carpetas de las partidas
            string nombreDirectorioRegex = @"^partida-(\d+)-(\d{2}\d{2}\d{4})-([a-zA-Z0-9]{3,15})$";
            Regex rgx = new Regex(nombreDirectorioRegex);

            // El atributo "DirectorioPartidas" no es null, si lo fuese, la excepción 
            // sería lanzada desde el método 'verificarDirectorioPartidas'. Aún así
            // realizo una doble verificación para no tener advertencias del vs code
            return (Config.DirectorioPartidas != null) ? Directory.GetDirectories(Config.DirectorioPartidas, Config.DirectorioPartidasPrefix + "*")
                                                                  .Select(dir => Path.GetFileName(dir))
                                                                  .Where(dir => rgx.IsMatch(dir))
                                                                  .ToList()

                                                       : new List<string>();
        }

        public Partida Cargar(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica si el directorio de partidas existe, si no existiese lo crea
        /// </summary>
        /// <exception cref="ConfiguracionInexistenteException">Cuando por alguna razón no se ha cargado el directorio de partidas en la configuración</exception>
        private void verificarDirectorioPartidas()
        {
            // Verifico si se cargó la configuración para poder usar los directorios
            // de no haber sido cargada, lanzo una excepción
            if (String.IsNullOrWhiteSpace(Config.DirectorioPartidas))
            {
                throw new ConfiguracionInexistenteException("No se ha configurado el directorio de partidas");
            }

            if (!Directory.Exists(Config.DirectorioPartidas))
            {
                Directory.CreateDirectory(Config.DirectorioPartidas);
            }
        }
    }
}