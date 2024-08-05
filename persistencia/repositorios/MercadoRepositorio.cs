using Logica.Excepciones;
using Logica.Modelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Persistencia.Repositorios;

public class MercadoRepositorio : IRepositorio<Mercado>
{
    private static Mercado? mercadoActual;

    /// <value>Contiene las propiedades que se excluirán del JSON correspondiente al mercado</value>
    private DefaultContractResolver mercadoContractResolver = new ExclusionPropiedadesJson(["numero_camiseta"]);

    /// <summary>
    /// Crea un nuevo mercado en los archivos de persistencia
    /// </summary>
    /// <param name="obj">Instancia de <see cref="Mercado"/> con los datos a persistir</param>
    /// <exception cref="PartidaInvalidaException">En caso de que no haya un directorio de la partida actual donde persistir</exception>
    public void Crear(Mercado obj)
    {
        // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
        // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
        if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
            throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
        
        string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonMercado}";

        // Creo el archivo del mercado
        using (FileStream mercadoJson = new FileStream(nombreArchivo, FileMode.Create))
        {
            using (StreamWriter mercadoWriter = new StreamWriter(mercadoJson))
            {
                string mercadoSerializado = JsonConvert.SerializeObject(obj,
                                                                        new JsonSerializerSettings {
                                                                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                                                                            Formatting = Formatting.Indented,
                                                                            ContractResolver = mercadoContractResolver
                                                                        });
                mercadoWriter.WriteLine(mercadoSerializado);
            }
        }

        // Actualizo la instancia del mercado actual
        mercadoActual = obj;
    }

    /// <summary>
    /// Carga los datos del archivo de persistencia del mercado correspondiente
    /// a la partida con el ID <paramref name="id"/>
    /// </summary>
    /// <param name="id">ID de la partida</param>
    /// <returns>Instancia de <see cref="Mercado"/></returns>
    /// <exception cref="PartidaInvalidaException">En caso de que no haya una partida actual de la cual obtener archivos de persistencia</exception>
    /// <exception cref="MercadoInvalidoException">En caso de que la respuesta de la deserealización del mercado sea nula</exception>
    public Mercado Cargar(int id)
    {
        if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
            throw new PartidaInvalidaException("No se pudieron cargar los datos del mercado. El directorio de la partida a cargar es nulo o está vacío");

        var mercadoJsonPath = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonMercado}";

        RecursosUtil.VerificarArchivo(mercadoJsonPath);

        // Leo y deserealizo el archivo del mercado
        Mercado? mercado;
        using (FileStream fileMercado = new FileStream(mercadoJsonPath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader readerMercado = new StreamReader(fileMercado))
            {
                string mercadoJsonTxt = readerMercado.ReadToEnd();
                mercado = JsonConvert.DeserializeObject<Mercado>(mercadoJsonTxt);

                if (mercado == null)
                    throw new MercadoInvalidoException("No se pudo deserealizar el archivo JSON del mercado, la respuesta ha sido nula");
            }
        }

        // Actualizo la instancia del mercado actual en el repositorio
        mercadoActual = mercado;

        return mercado;
    }

    /// <summary>
    /// Elimina el archivo del mercado actual de la persitencia
    /// </summary>
    public void Eliminar()
    {
        RecursosUtil.EliminarArchivo(Config.DirectorioPartidaActual + @"\" + Config.NombreJsonMercado);

        // Actualizo la instancia del mercado actual
        mercadoActual = null;
    }

    /// <summary>
    /// Persiste los datos del mercado
    /// </summary>
    /// <param name="obj">Instancia de <see cref="Mercado"/> con los datos a almacenar</param>
    /// <exception cref="PartidaInvalidaException">En caso de que no haya una partida actual de la cual obtener archivos de persistencia</exception>
    public void Guardar(Mercado obj)
    {
        // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
        // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
        if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
            throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
        
        string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonMercado}";

        // Creo el archivo del mercado
        using (FileStream mercadoJson = new FileStream(nombreArchivo, FileMode.Create))
        {
            using (StreamWriter mercadoWriter = new StreamWriter(mercadoJson))
            {
                string mercadoSerializado = JsonConvert.SerializeObject(obj,
                                                                        new JsonSerializerSettings {
                                                                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                                                                            Formatting = Formatting.Indented,
                                                                            ContractResolver = mercadoContractResolver
                                                                        });
                mercadoWriter.WriteLine(mercadoSerializado);
            }
        }
    }

    /// <summary>
    /// Obtiene la instancia del mercado correspondiente a la partida actual
    /// </summary>
    /// <returns>Instancia del <see cref="Mercado"/></returns>
    /// <exception cref="MercadoInvalidoException">Cuando la instancia del mercado sea nula</exception>
    public Mercado ObtenerActual()
    {
        if (mercadoActual == null)
            throw new MercadoInvalidoException("No existe una instancia del mercado actual en el repositorio");

        return mercadoActual;
    }
}
