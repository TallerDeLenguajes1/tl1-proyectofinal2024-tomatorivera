using System.Text.RegularExpressions;
using Logica.Excepciones;
using Logica.Modelo;
using Newtonsoft.Json;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Persistencia.Repositorios
{
    public class PartidaRepositorioImpl : IRepositorioNavegable<Partida>
    {
        private static Partida? partidaActual;

        /// <summary>
        /// Crea un nuevo archivo de persistencia para <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Objeto <c>Partida</c></param>
        /// <exception cref="PartidaDuplicadaException">Excepción lanzada cuando ya existe una carpeta con el mismo nombre de la que se va a crear</exception>
        public void Crear(Partida obj)
        {
            // Verifica si el directorio de partidas existe, caso contrario lo crea
            // Lanza una excepción si por alguna razón no se ha cargado el directorio de partidas en la configuración
            RecursosUtil.VerificarDirectorio(Config.DirectorioPartidas ?? string.Empty);

            string nuevaPartidaDir = Config.DirectorioPartidas + @"\partida-" + obj.Id + "-" + obj.FechaGuardado.ToString("ddMMyyyy") + "-" + obj.Usuario.Nombre;

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
                    string partidaSerializada = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml, Formatting = Formatting.Indented });
                    writer.WriteLine(partidaSerializada);
                }
            }

            // Actualizo el directorio de la partida actual en el archivo de configuración
            Config.DirectorioPartidaActual = nuevaPartidaDir;
            // Actualizo la instancia de la partida actual para manejar los datos desde el programa
            partidaActual = obj;
        }

        /// <summary>
        /// Sobreescribe el archivo de persistencia de la partida
        /// </summary>
        /// <param name="obj">Objeto <c>Partida</c> con datos actualizados</param>
        /// <exception cref="PartidaInvalidaException">En caso de que no se haya cargado la ruta de la partida actual a la configuración</exception>
        public void Guardar(Partida obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
            
            string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonPartida}";

            // Creo el archivo del historial
            using (FileStream partidaJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter partidaWriter = new StreamWriter(partidaJson))
                {
                    string partidaSerializada = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml, Formatting = Formatting.Indented });
                    partidaWriter.WriteLine(partidaSerializada);
                }
            }
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
        /// Carga los datos de una partida desde los archivos de persistencia
        /// </summary>
        /// <param name="id">ID de la partida a cargar</param>
        /// <returns>Objeto <c>Partida</c> con los datos de la partida de ID <paramref name="id"/></returns>
        /// <exception cref="PartidaInvalidaException">En caso de que la deserealización del archivo partida sea NULL</exception>
        /// <exception cref="UsuarioInvalidoException">En caso de que la deserealización del archivo usuario sea NULL</exception>
        public Partida Cargar(int id)
        {
            // Verifica si el directorio de partidas existe, caso contrario lo crea
            // Lanza una excepción si por alguna razón no se ha cargado el directorio de partidas en la configuración
            RecursosUtil.VerificarDirectorio(Config.DirectorioPartidas ?? string.Empty);

            var coincidenciasDirPartida = Directory.GetDirectories(Config.DirectorioPartidas!)
                                                   .Select(dir => Path.GetFileName(dir))
                                                   .Where(dir => dir.StartsWith($"partida-{id}-"))
                                                   .ToList();
            
            if (!coincidenciasDirPartida.Any())
                throw new PartidaInvalidaException($"No se ha encontrado el directorio de una partida con el ID {id}");
            
            var dirPartida = coincidenciasDirPartida.First();

            // Verifico que los archivos de la partida existan, en caso de que no, el método 'VerificarArchivo()' lanzará una excepción
            var partidaJsonPath = @$"{Config.DirectorioPartidas}\{dirPartida}\{Config.NombreJsonPartida}";

            RecursosUtil.VerificarArchivo(partidaJsonPath);

            // Leo el archivo de la partida, si no se pudiese deserealizar se lanzará una excepción
            Partida? partida;
            using (FileStream lectorArchivos = new FileStream(partidaJsonPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(partidaJsonPath))
                {
                    string partidaJsonTxt = reader.ReadToEnd();
                    partida = JsonConvert.DeserializeObject<Partida>(partidaJsonTxt);

                    if (partida == null)
                        throw new PartidaInvalidaException($"No se pudieron leer del JSON los datos de la partida de ID {id}");
                }
            }

            // Actualizo la instancia de la partida actual en este repositorio
            partidaActual = partida;
            // Actualizo los datos de la partida actual en la config
            Config.DirectorioPartidaActual = @$"{Config.DirectorioPartidas}\{dirPartida}";

            // Retorno la partida obtenida
            return partida;
        }

        /// <summary>
        /// Elimina el directorio y el archivo de la partida actual de la persistencia
        /// </summary>
        public void Eliminar()
        {
            RecursosUtil.EliminarArchivo(Config.DirectorioPartidaActual + @"\" + Config.NombreJsonPartida);
            RecursosUtil.EliminarDirectorio(Config.DirectorioPartidaActual!);

            // Actualizo la instancia de la partida actual y el directorio
            partidaActual = null;
            Config.DirectorioPartidaActual = null;
        }

        /// <summary>
        /// Obtiene una lista de string con los nombres válidos de directorios de partidas
        /// </summary>
        /// <returns><c>List</c> de <c>string</c> con los nombres de los directorios</returns>
        public List<string> ObtenerDirectorios()
        {
            RecursosUtil.VerificarDirectorio(Config.DirectorioPartidas ?? string.Empty);

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
    }
}