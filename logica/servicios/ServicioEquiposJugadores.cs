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
        Equipo GenerarEquipo(string nombreEquipo = "", int nJugadores = 14);
        string GenerarNombreEquipo();

        Jugador GenerarJugador();
        List<Jugador> GenerarJugadores(int nJugadores);
        string GenerarNombreJugador();
        Dictionary<int, string> GenerarIdentificadoresJugadores(int nIdentificadores = 1);
    }

    public class EquipoJugadoresServicioImpl : EquipoJugadoresServicio
    {
        private const int nCamisetaMin = 1;
        private const int nCamisetaMax = 30;

        /// <summary>
        /// Genera un nuevo equipo
        /// </summary>
        /// <param name="nombreEquipo">Nombre del equipo a generar</param>
        /// <param name="nJugadores">Número de jugadores a generar (por defecto 14, el nro inicial de jugadores)</param>
        /// <returns>Objeto <c>Equipo</c></returns>
        public Equipo GenerarEquipo(string nombreEquipo = "", int nJugadores = 14)
        {   
            try
            {
                Equipo nuevoEquipo = new Equipo
                {
                    Nombre = string.IsNullOrWhiteSpace(nombreEquipo) ? GenerarNombreEquipo() : nombreEquipo,
                    Jugadores = GenerarJugadores(nJugadores)
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
        public string GenerarNombreEquipo()
        {
            string nuevoNombre = "Equipo rival";

            try
            {
                // Verifico si se han cargado las configuraciones necesarias
                if (string.IsNullOrWhiteSpace(Config.ApiSportsUrl))
                    throw new ApiInaccesibleException("La URL de Api Sports es nula o está vacía");
                if (string.IsNullOrWhiteSpace(Config.ApiSportsKey))
                    throw new ApiInaccesibleException("La key de Api Sports es nula o está vacía");

                // Instancio un consumidor y le agrego los parámetros necesarios
                var consumidor = new Consumidor<TeamsRaiz>(Config.ApiSportsUrl);
                consumidor.AgregarSubUrl("teams");
                consumidor.AgregarParametro("country_id", "1");
                consumidor.AgregarCabecera("x-apisports-key", Config.ApiSportsKey);

                // Consumo la api de forma asincrónica
                var tareaConsumir = Task.Run(consumidor.ConsumirAsync);
                tareaConsumir.Wait();

                // Obtengo el resultado de la API
                var listaEquipos = tareaConsumir.Result;

                // Si la respuesta es nula, lanzo la excepción porque puede tratarse de un error
                if (listaEquipos.response == null)
                    throw new ApiInaccesibleException("La respuesta de Api Sports es nula", listaEquipos);

                // Filtro equipos no nacionales y que no tengan el mismo nombre que el equipo del usuario (por las dudas)
                var equiposFiltrados = listaEquipos.response.Where(equipo => !equipo.national)
                                                            .Where(equipo => equipo.name != null && !equipo.name.Equals(Config.NombreEquipoUsuario));

                // Devuelvo el nombre del equipo random filtrado, si el nombre fuese null o la filtración hubiese dado
                // como resultado una lista vacía, entonces retorno simplemente "Equipo rival" para continuar con el juego
                nuevoNombre = equiposFiltrados.Any() ? equiposFiltrados.ElementAt(new Random().Next(equiposFiltrados.Count())).name ?? "Equipo rival"
                                                     : "Equipo rival";
            }
            catch (Exception ex)
            {
                // Disperso el problema para poder ser mostrado por pantalla
                ErroresIgnorablesHandler.ObtenerInstancia().Errores.TryAdd("Generar nombre equipo", new Exception("Se creará un nombre genérico para el nuevo equipo", ex));
            }

            return nuevoNombre;
        }

        /// <summary>
        /// Genera un nuevo jugador
        /// </summary>
        /// <returns>Objeto <c>Jugador</c></returns>
        public Jugador GenerarJugador()
        {
            var rnd = new Random();

            try
            {   
                // Genero el numero de camiseta
                var numeroCamiseta = rnd.Next(nCamisetaMin, nCamisetaMax);

                // Selecciono el tipo de jugador aleatoriamente desde el Enum
                var nTipoJugadores = Enum.GetValues(typeof(TipoJugador));
                var tipoJugador = nTipoJugadores.GetValue(rnd.Next(nTipoJugadores.Length));

                JugadorFabrica fabricaJugador = tipoJugador switch
                {
                    TipoJugador.LIBERO => new JugadorLiberoFabrica(),
                    TipoJugador.ARMADOR => new JugadorArmadorFabrica(),
                    TipoJugador.CENTRAL => new JugadorCentralFabrica(),
                    TipoJugador.OPUESTO => new JugadorOpuestoFabrica(),
                    _ => new JugadorPuntaFabrica()
                };

                // Genero los atributos del jugador
                Jugador nuevoJugador = fabricaJugador.CrearJugador();
                nuevoJugador.NumeroCamiseta = numeroCamiseta;
                nuevoJugador.Experiencia = 0.00f;

                return nuevoJugador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Genera una lista de jugadores evitando que se repitan sus numeros de camisetas
        /// </summary>
        /// <param name="nJugadores">Numero de jugadores a generar (por defecto 1)</param>
        /// <returns><c>List</c> de <c>Jugador</c></returns>
        public List<Jugador> GenerarJugadores(int nJugadores = 1)
        {
            try
            {
                var rnd = new Random();
                var listaJugadores = new List<Jugador>();
                var listaIdentificadores = GenerarIdentificadoresJugadores(nJugadores);
                var tipoJugadores = Enum.GetValues(typeof(TipoJugador));

                for (int i=0 ; i < nJugadores ; i++)
                {
                    var tipoJugador = (TipoJugador) tipoJugadores.GetValue(rnd.Next(tipoJugadores.Length))!;

                    JugadorFabrica fabricaJugador = tipoJugador switch
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
        public string GenerarNombreJugador()
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

                var tareaConsumir = Task.Run(consumidor.ConsumirAsync);
                tareaConsumir.Wait();

                var listaNombres = tareaConsumir.Result;

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
        public Dictionary<int, string> GenerarIdentificadoresJugadores(int nIdentificadores = 1)
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

                var tareaConsumir = Task.Run(consumidor.ConsumirAsync);
                tareaConsumir.Wait();

                var respuestaNombres = tareaConsumir.Result;

                // Si la respuesta es nula o vacía, lanzo la excepción porque puede tratarse de un error
                if (respuestaNombres.Results == null)
                    throw new ApiInaccesibleException("La respuesta de Random User API es nula", consumidor.ApiFullUrl);

                if (!respuestaNombres.Results.Any()) 
                    throw new ApiInaccesibleException("La respuesta de Random User API está vacía");

                // Mapeo la lista de resultados a una lista de strings con los nombres, o el nombre "Jugador" en caso de que Name sea null
                var listaNombres = respuestaNombres.Results.Select(x => $"{x.Name?.First ?? "Jugador"} {x.Name?.Last ?? string.Empty}").ToList();

                // Genero n identificadores
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
                
            }
            catch (Exception ex)
            {
                // Si hubieron errores comunicándose con la API, genero n identificadores genéricos
                for (int i=0 ; i<nIdentificadores ; i++)
                {
                    // Genero el numero de camiseta controlando que no sea duplicada
                    var numeroCamiseta = random.Next(nCamisetaMin, nCamisetaMax);
                    while (!numerosOcupados.Add(numeroCamiseta))
                    {
                        numeroCamiseta = random.Next(nCamisetaMin, nCamisetaMax);
                    }
                    
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