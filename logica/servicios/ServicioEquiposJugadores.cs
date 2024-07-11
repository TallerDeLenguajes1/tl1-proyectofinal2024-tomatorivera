using Logica.Excepciones;
using Logica.Fabricas;
using Logica.Handlers;
using Logica.Modelo;
using Persistencia.Infraestructura;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de tareas relacionadas a un Equipo y sus jugadores
    /// </summary>
    public interface EquipoJugadoresServicio
    {
        Task<Equipo> GenerarEquipoAsync(string nombreEquipo = "", int nJugadores = 14);
        Task<string> GenerarNombreEquipoAsync();
        void AlmacenarNombreEquipoUsuario(string nombreEquipo);

        Task<List<Jugador>> GenerarJugadoresAsync(int nJugadores);
        Task<string> GenerarNombreJugadorAsync();
        Task<Dictionary<int, string>> GenerarIdentificadoresJugadoresAsync(int nIdentificadores = 1);
    }

    public class EquipoJugadoresServicioImpl : EquipoJugadoresServicio
    {
        private const int nCamisetaMin = 1;
        private const int nCamisetaMax = 50;

        /// <summary>
        /// Genera un nuevo equipo
        /// </summary>
        /// <param name="nombreEquipo">Nombre del equipo a generar (por defecto vacío, en cuyo caso se genera automáticamente desde una API)</param>
        /// <param name="nJugadores">Número de jugadores a generar (por defecto 14, el nro inicial de jugadores)</param>
        /// <returns>Objeto <c>Equipo</c></returns>
        public async Task<Equipo> GenerarEquipoAsync(string nombreEquipo = "", int nJugadores = 14)
        {   
            try
            {
                Equipo nuevoEquipo = new Equipo
                {
                    Nombre = string.IsNullOrWhiteSpace(nombreEquipo) ? await GenerarNombreEquipoAsync() : nombreEquipo,
                    Jugadores = await GenerarJugadoresAsync(nJugadores)
                };

                return nuevoEquipo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Genera un nombre aleatorio para un equipo a partir de la api API-Sports, en caso de problemas
        /// de comunicación o parámetros enviados a la API, devuelve "Equipo Rival"
        /// </summary>
        /// <returns><c>string</c> con el nombre del equipo generado</returns>
        public async Task<string> GenerarNombreEquipoAsync()
        {
            string nuevoNombre = "Nuevo equipo";

            try
            {
                // Verifico si se han cargado las configuraciones necesarias
                if (string.IsNullOrWhiteSpace(Config.ApiSportsUrl))
                    throw new ApiInaccesibleException("La URL de Api Sports es nula o está vacía");
                if (string.IsNullOrWhiteSpace(Config.ApiSportsKey))
                    throw new ApiInaccesibleException("No se pudo cargar la KEY de Api Sports, revise la configuración");

                // Instancio un consumidor y le agrego los parámetros necesarios
                var consumidor = new Consumidor<TeamsRaiz>(Config.ApiSportsUrl);
                consumidor.AgregarSubUrl("teams");
                consumidor.AgregarParametro("country_id", "1");
                consumidor.AgregarCabecera("x-apisports-key", Config.ApiSportsKey);

                // Consumo la api de forma asincrónica
                var tareaConsumir = await consumidor.ConsumirAsync();

                // Obtengo el resultado de la API
                var listaEquipos = tareaConsumir;

                // Si la respuesta es nula, lanzo la excepción porque puede tratarse de un error
                if (listaEquipos.response == null)
                    throw new ApiInaccesibleException("La respuesta de Api Sports es nula", listaEquipos);
                if (!listaEquipos.response.Any())
                    throw new ApiInaccesibleException("La respuesta de Api Sports está vacía");

                // Filtro equipos no nacionales y que no tengan el mismo nombre que el equipo del usuario (por las dudas)
                var equiposFiltrados = listaEquipos.response.Where(equipo => !equipo.national)
                                                            .Where(equipo => equipo.name != null && !equipo.name.Equals(Config.NombreEquipoUsuario));

                // Devuelvo el nombre del equipo random filtrado, si el nombre fuese null o la filtración hubiese dado
                // como resultado una lista vacía, entonces retorno simplemente "Equipo rival" para continuar con el juego
                nuevoNombre = equiposFiltrados.Any() ? equiposFiltrados.ElementAt(new Random().Next(equiposFiltrados.Count())).name ?? nuevoNombre
                                                     : nuevoNombre;
            }
            catch (Exception ex)
            {
                // Disperso el problema para poder ser mostrado por pantalla
                ErroresIgnorablesHandler.ObtenerInstancia().Errores.TryAdd("Generar nombre equipo", new Exception("Se creará un nombre genérico para el nuevo equipo", ex));
            }

            return nuevoNombre;
        }

        /// <summary>
        /// Guarda en los archivos de configuracion el nombre del equipo del usuario
        /// </summary>
        /// <param name="nombreEquipo">Nombre de equipo del usuario</param>
        public void AlmacenarNombreEquipoUsuario(string nombreEquipo)
        {
            Config.NombreEquipoUsuario = nombreEquipo;
        }

        /// <summary>
        /// Genera una lista de jugadores evitando que se repitan sus numeros de camisetas
        /// </summary>
        /// <param name="nJugadores">Numero de jugadores a generar (por defecto 1)</param>
        /// <returns><c>List</c> de <c>Jugador</c></returns>
        public async Task<List<Jugador>> GenerarJugadoresAsync(int nJugadores = 1)
        {
            try
            {
                var rnd = new Random();
                var listaJugadores = new List<Jugador>();
                var listaIdentificadores = await GenerarIdentificadoresJugadoresAsync(nJugadores);
                var tipoJugadores = Enum.GetValues(typeof(TipoJugador));
                
                TipoJugador tipoJugador;
                JugadorFabrica fabricaJugador;

                for (int i=0 ; i < nJugadores ; i++)
                {
                    tipoJugador = (TipoJugador) tipoJugadores.GetValue(rnd.Next(tipoJugadores.Length))!;

                    fabricaJugador = tipoJugador switch
                    {
                        TipoJugador.LIBERO => new JugadorLiberoFabrica(),
                        TipoJugador.ARMADOR => new JugadorArmadorFabrica(),
                        TipoJugador.CENTRAL => new JugadorCentralFabrica(),
                        TipoJugador.OPUESTO => new JugadorOpuestoFabrica(),
                        _ => new JugadorPuntaFabrica()
                    };

                    // Genero los atributos del jugador
                    Jugador nuevoJugador = fabricaJugador.CrearJugador();
                    nuevoJugador.NumeroCamiseta = listaIdentificadores.ElementAt(i).Key;
                    nuevoJugador.Nombre = listaIdentificadores.ElementAt(i).Value;
                    nuevoJugador.TipoJugador = tipoJugador;
                    nuevoJugador.Experiencia = 0.00f;

                    listaJugadores.Add(nuevoJugador);
                }

                return listaJugadores;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Genera un nombre para un jugador desde una API, en caso de no poder traer un
        /// dato de allí, devuelve un nombre genérico
        /// </summary>
        /// <returns>Nombre generado para un jugador</returns>
        public async Task<string> GenerarNombreJugadorAsync()
        {
            string nombreJugador = "Jugador";

            try
            {
                if (string.IsNullOrWhiteSpace(Config.ApiRandomUserUrl))
                    throw new ApiInaccesibleException("La url de Api Random User es nula o está vacía");

                var consumidor = new Consumidor<NameRaiz>(Config.ApiRandomUserUrl);
                consumidor.AgregarParametro("inc", "name,nat");
                consumidor.AgregarParametro("nat", "es,us,mx");
                consumidor.AgregarParametro("gender", "male");
                consumidor.AgregarParametro("results", "1");
                consumidor.AgregarParametro("noinfo");

                var tareaConsumir = await consumidor.ConsumirAsync();

                var listaNombres = tareaConsumir;

                // Si la respuesta es nula, lanzo la excepción porque puede tratarse de un error
                if (listaNombres.Results == null)
                    throw new ApiInaccesibleException("La respuesta de Random User API es nula", listaNombres);
                
                if (!listaNombres.Results.Any()) 
                    throw new ApiInaccesibleException("La respuesta de Random User API está vacía");

                var nombreAleatorio = listaNombres.Results.ElementAt(new Random().Next(listaNombres.Results.Count()));

                nombreJugador = (nombreAleatorio.Name != null) ? nombreAleatorio.Name.First + " " + nombreAleatorio.Name.Last
                                                               : nombreJugador;
            }
            catch (Exception ex)
            {
                // Disperso el problema para poder ser mostrado por pantalla
                ErroresIgnorablesHandler.ObtenerInstancia().Errores.TryAdd("Generar nombre jugador", new Exception("Se creará un nombre genérico para los jugadores", ex));
            }

            return nombreJugador;
        }

        /// <summary>
        /// Genera un par clave valor para identificar a los jugadores de manera única, que contiene
        /// su número de camiseta y su nombre obtenido a partir de una api
        /// </summary>
        /// <param name="nIdentificadores">Números de identificadores a generar (por defecto, uno)</param>
        /// <returns>Diccionario de valores: <c>número de camiseta, nombre de jugador</c></returns>
        public async Task<Dictionary<int, string>> GenerarIdentificadoresJugadoresAsync(int nIdentificadores = 1)
        {
            var listaIdentificadores = new Dictionary<int, string>();
            var numerosOcupados = new HashSet<int>();
            var nombresOcupados = new HashSet<string>();
            var random = new Random();

            try
            {
                if (string.IsNullOrWhiteSpace(Config.ApiRandomUserUrl))
                    throw new ApiInaccesibleException("La url de Api Random User es nula o está vacía");

                var consumidor = new Consumidor<NameRaiz>(Config.ApiRandomUserUrl);
                consumidor.AgregarParametro("inc", "name");
                consumidor.AgregarParametro("nat", "es,us,mx");
                consumidor.AgregarParametro("gender", "male");
                consumidor.AgregarParametro("results", nIdentificadores.ToString());
                consumidor.AgregarParametro("noinfo");

                var tareaConsumir = await consumidor.ConsumirAsync();

                var respuestaNombres = tareaConsumir;

                // Si la respuesta es nula o vacía, lanzo la excepción porque puede tratarse de un error
                if (respuestaNombres.Results == null)
                    throw new ApiInaccesibleException("La respuesta de Random User API es nula", consumidor.ApiFullUrl);

                if (!respuestaNombres.Results.Any()) 
                    throw new ApiInaccesibleException("La respuesta de Random User API está vacía");

                // Mapeo la lista de resultados a una lista de strings con los nombres, o el nombre "Jugador" en caso de que Name sea null
                var listaNombres = respuestaNombres.Results.Select(x => $"{x.Name?.First ?? "Jugador"} {x.Name?.Last ?? string.Empty}").ToList();

                // Genero n identificadores
                while (listaIdentificadores.Count() < nIdentificadores)
                {
                    var numeroCamiseta = random.Next(nCamisetaMin, nCamisetaMax);

                    if (!numerosOcupados.Add(numeroCamiseta)) 
                        continue;

                    var nombreJugador = listaNombres.First();
                    listaNombres.RemoveAt(0);

                    // En caso de que se devuelva "Jugador", quiere decir que hubo problemas conectándose con la API
                    // y en dicho caso se crea un nombre genérico
                    if (nombreJugador.Contains("Jugador"))
                        nombreJugador = $"Jugador {numeroCamiseta}";

                    listaIdentificadores.Add(numeroCamiseta, nombreJugador);
                }
                /*
                for (int i=0 ; i<nIdentificadores ; i++)
                {
                    // Genero el numero de camiseta controlando que no sea duplicada
                    var numeroCamiseta = random.Next(nCamisetaMin, nCamisetaMax);
                    while (!numerosOcupados.Add(numeroCamiseta))
                    {
                        numeroCamiseta = random.Next(nCamisetaMin, nCamisetaMax);
                    }
                    
                    // Obtengo un nombre random de la lista controlando que no sea duplicado
                    var nombreJugador = listaNombres.ElementAt(random.Next(listaNombres.Count()));
                    while (!nombresOcupados.Add(nombreJugador))
                    {
                        nombreJugador = GenerarNombreJugador();

                        // En caso de que se devuelva "Jugador", quiere decir que hubo problemas conectándose con la API
                        // y en dicho caso se crea un nombre genérico
                        if (nombreJugador.Contains("Jugador"))
                            nombreJugador += $" {numeroCamiseta}";
                    }

                    listaIdentificadores.Add(numeroCamiseta, nombreJugador);
                }
                */
                
            }
            catch (Exception ex)
            {
                // Si hubieron errores comunicándose con la API, genero n identificadores genéricos
                while (listaIdentificadores.Count() < nIdentificadores)
                {
                    // Genero el numero de camiseta controlando que no sea duplicada
                    var numeroCamiseta = random.Next(nCamisetaMin, nCamisetaMax);
                    if (!numerosOcupados.Add(numeroCamiseta)) continue;
                    
                    // Agrego a la lista un nombre genérico
                    listaIdentificadores.Add(numeroCamiseta, $"Jugador {numeroCamiseta}");
                }

                
                // Disperso el problema para poder ser mostrado por pantalla
                ErroresIgnorablesHandler.ObtenerInstancia().Errores.TryAdd("Generar nombres jugadores", new Exception("Se crearán nombres genéricos para los jugadores", ex));
            }

            return listaIdentificadores;
        }
    }
}