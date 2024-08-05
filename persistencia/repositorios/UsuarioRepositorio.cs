using Logica.Excepciones;
using Logica.Modelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Persistencia.Repositorios
{
    public class UsuarioRepositorioImpl : IRepositorio<Usuario>
    {
        private static Usuario? usuarioActual;

        /// <value>Contiene las propiedades que se excluirán del JSON correspondiente al usuario</value>
        private DefaultContractResolver usuarioContractResolver = new ExclusionPropiedadesJson(["jugadores_convocados", "es_equipo_jugador", "precio_mercado"]);

        /// <summary>
        /// Crea un nuevo archivo de persistencia para <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Objeto <c>Usuario</c> con los datos a persistir</param>
        /// <exception cref="PartidaInvalidaException">En caso de que no se haya creado el directorio de la partida donde persistir el usuario</exception>
        public void Crear(Usuario obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");

            // Guardo el nombre del archivo
            string nombreArchivo = Config.DirectorioPartidaActual + @"\" + Config.NombreJsonUsuario;

            // Creo el archivo del usuario
            using (FileStream usuarioJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(usuarioJson))
                {
                    string usuarioSerializado = JsonConvert.SerializeObject(obj, 
                                                                            new JsonSerializerSettings {
                                                                                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                                                                                Formatting = Formatting.Indented,
                                                                                ContractResolver = usuarioContractResolver
                                                                            });
                    writer.WriteLine(usuarioSerializado);
                }
            }

            // Actualizo la instancia del usuario actual para manejar sus datos desde el programa
            usuarioActual = obj;
            // Almaceno el nombre del usuario actual
            Config.NombreUsuarioActual = usuarioActual.Nombre;
            // Almaceno el nombre de su equipo
            Config.NombreEquipoUsuario = usuarioActual.Equipo.Nombre;
        }

        /// <summary>
        /// Carga los datos de un usuario desde los archivos de persistencia correspondientes a una partida de ID <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID de la partida</param>
        /// <returns>Objeto <c>Usuario</c></returns>
        /// <exception cref="PartidaInvalidaException">Cuando no se haya cargado el directorio de la partida donde buscar el usuario</exception>
        /// <exception cref="UsuarioInvalidoException">Cuando no se pueda leer o deserealizar el JSON correspondiente al usuario</exception>
        public Usuario Cargar(int id)
        {
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudieron cargar los datos del usuario. El directorio de la partida actual es nulo o está vacío");

            var usuarioJsonPath = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonUsuario}";

            RecursosUtil.VerificarArchivo(usuarioJsonPath);

            // Leo el archivo del usuario, si no se pudiese deserealizar se lanzará una excepción
            Usuario? usuario;
            using (FileStream lectorArchivos = new FileStream(usuarioJsonPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(usuarioJsonPath))
                {
                    string usuarioJsonTxt = reader.ReadToEnd();
                    usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJsonTxt);

                    if (usuario == null)
                        throw new UsuarioInvalidoException($"No se pudieron leer del JSON los datos del usuario de la partida solicitada");
                }
            }

            // Actualizo la instancia del usuario actual en el repositorio
            usuarioActual = usuario;
            // Modifico los datos de configuración vinculados al usuario
            Config.NombreUsuarioActual = usuario.Nombre;
            Config.NombreEquipoUsuario = usuarioActual.Equipo.Nombre;

            return usuario;
        }

        /// <summary>
        /// Sobreescribe los datos del usuario en los archivos de persistencia
        /// </summary>
        /// <param name="obj">Objeto <c>Usuario</c> con los datos del usuario actualizados</param>
        /// <exception cref="PartidaInvalidaException">En caso de que no se haya cargado la ruta de la carpeta a la configuración</exception>S
        public void Guardar(Usuario obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
            
            string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonUsuario}";

            // Creo el archivo del historial
            using (FileStream usuarioJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter usuarioWriter = new StreamWriter(usuarioJson))
                {
                    string usuarioSerializado = JsonConvert.SerializeObject(obj, 
                                                                            new JsonSerializerSettings {
                                                                                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                                                                                Formatting = Formatting.Indented,
                                                                                ContractResolver = usuarioContractResolver
                                                                            });
                    usuarioWriter.WriteLine(usuarioSerializado);
                }
            }
        }

        /// <summary>
        /// Elimina el archivo del usuario actual de la persistencia
        /// </summary>
        public void Eliminar()
        {
            RecursosUtil.EliminarArchivo(Config.DirectorioPartidaActual + @"\" + Config.NombreJsonUsuario);

            // Actualizo la instancia del usuario actual y su nombre en la configuración
            usuarioActual = null;
            Config.NombreUsuarioActual = null;
            Config.NombreEquipoUsuario = null;
        }

        /// <summary>
        /// Obtiene la instancia del usuario actual correspondiente a la partida que se está jugando
        /// </summary>
        /// <returns>Objeto <c>Usuario</c> con los datos del usuario actual</returns>
        /// <exception cref="UsuarioInvalidoException">En caso de que se solicite el usuario actual cuando la instancia aún sea null</exception>
        public Usuario ObtenerActual()
        {
            if (usuarioActual == null)
                throw new UsuarioInvalidoException("No se ha cargado una instancia del usuario actual en el repositorio");

            return usuarioActual;
        }
    }
}