using Logica.Excepciones;
using Logica.Fabricas;
using Logica.Handlers;
using Logica.Modelo;
using Persistencia.Infraestructura;

namespace Logica.Servicios
{
    /// <summary>
    /// Servicio para la gestión de tareas relacionadas a un Equipo
    /// </summary>
    public interface EquipoJugadoresServicio
    {
        Equipo GenerarEquipo(string nombreEquipo, int nJugadores = 14);
        string GenerarNombreEquipo();

        Jugador GenerarJugador();
        List<Jugador> GenerarJugadores(int nJugadores);
        string GenerarNombreJugador();
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
        public Equipo GenerarEquipo(string nombreEquipo, int nJugadores = 14)
        {   
            try
            {
                Equipo nuevoEquipo = new Equipo();
                nuevoEquipo.Nombre = nombreEquipo;
                nuevoEquipo.Jugadores = GenerarJugadores(nJugadores);

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
                new ErroresIgnorablesHandler().ManejarError($"{ex.Message}. Se creará un nombre genérico para el nuevo equipo");
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
                int numeroCamiseta = rnd.Next(nCamisetaMin, nCamisetaMax);
                string nombre = GenerarNombreJugador();

                // Si el nombre del jugador es el genérico, quiere decir que hubo un error con la api
                // En dicho caso le agrego el número de camiseta al nombre para distinguirlo
                if (nombre.Equals("Jugador"))
                    nombre += " " + numeroCamiseta;

                var tipoJugadores = Enum.GetValues(typeof(TipoJugador));
                var tipoJugador = tipoJugadores.GetValue(rnd.Next(tipoJugadores.Length));

                JugadorFabrica fabricaJugador = tipoJugador switch
                {
                    TipoJugador.LIBERO => new JugadorLiberoFabrica(),
                    TipoJugador.ARMADOR => new JugadorArmadorFabrica(),
                    TipoJugador.CENTRAL => new JugadorCentralFabrica(),
                    TipoJugador.REMATADOR => new JugadorRematadorFabrica(),
                    TipoJugador.SERVIDOR => new JugadorServidorFabrica(),
                    _ => new JugadorCentralFabrica() // Default
                };

                Jugador nuevoJugador = fabricaJugador.CrearJugador();
                nuevoJugador.NumeroCamiseta = numeroCamiseta;
                nuevoJugador.Nombre = nombre;
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
        /// <param name="nJugadores">Numero de jugadores a generar</param>
        /// <returns><c>List</c> de <c>Jugador</c></returns>
        public List<Jugador> GenerarJugadores(int nJugadores)
        {
            try
            {
                var rnd = new Random();
                var listaJugadores = new List<Jugador>();
                var numerosOcupados = new HashSet<int>();
                var nombresOcupados = new HashSet<string>();

                // Genero nJugadores
                for (int i=0 ; i<nJugadores ; i++)
                {
                    Jugador nuevoJugador = GenerarJugador();

                    // Reviso si el jugador tiene un numero de camiseta duplicado
                    // en dicho caso, se lo modifico
                    while (numerosOcupados.Contains(nuevoJugador.NumeroCamiseta))
                        nuevoJugador.NumeroCamiseta = rnd.Next(nCamisetaMin, nCamisetaMax);

                    while (nombresOcupados.Contains(nuevoJugador.Nombre ?? "Jugador " + nuevoJugador.NumeroCamiseta))
                        nuevoJugador.Nombre = GenerarNombreJugador();

                    numerosOcupados.Add(nuevoJugador.NumeroCamiseta);
                    nombresOcupados.Add(nuevoJugador.Nombre ?? "Jugador " + nuevoJugador.NumeroCamiseta);
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
        /// Genera el nombre para un jugador desde una API, en caso de no poder traer un
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
                    throw new ApiInaccesibleException("La respuesta de Api Sports es nula", listaNombres);
                
                if (!listaNombres.Results.Any()) 
                    return nombreJugador;

                var nombreAleatorio = listaNombres.Results.ElementAt(new Random().Next(listaNombres.Results.Count()));

                nombreJugador = (nombreAleatorio.Name != null) ? nombreAleatorio.Name.First + " " + nombreAleatorio.Name.Last
                                                               : nombreJugador;
            }
            catch (Exception ex)
            {
                // Disperso el problema para poder ser mostrado por pantalla
                new ErroresIgnorablesHandler().ManejarError($"{ex.Message}. Se creará un nombre genérico para el jugador");
            }

            return nombreJugador;
        }
    }
}