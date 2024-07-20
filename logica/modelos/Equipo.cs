using Newtonsoft.Json;
using Persistencia.Infraestructura;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase que representa un equipo compuesto por x jugadores
    /// </summary>
    public class Equipo
    {
        private string nombre;
        private int nJugadores;
        private List<Jugador> jugadores;
        private Formacion? formacionPartido;
        private bool esEquipoJugador;

        public Equipo() 
        {
            this.nombre = "Nombre sin especificar";
            this.jugadores = new List<Jugador>();
            this.nJugadores = 0;
            this.EsEquipoJugador = false;
        }

        // Propiedades
        
        [JsonProperty("NombreEquipo")]
        public string Nombre { get => nombre; set => nombre = value; }
        public int TotalJugadores { get => nJugadores; set => nJugadores = value; }
        public bool EsEquipoJugador { get => esEquipoJugador; set => esEquipoJugador = value; }
        public List<Jugador> Jugadores 
        { 
            get => jugadores; 
            set {
                jugadores = value;
                nJugadores = jugadores.Count(); 
            }
        }
        public Formacion? FormacionPartido { get => formacionPartido ; set => formacionPartido = value; }
    }

    /// <summary>
    /// Representa la formación en un partido de un equipo, almacenando en una
    /// <c>Lista Circular</c> los jugadores en cancha y los jugadores suplentes
    /// </summary>
    public class Formacion
    {
        public ListaCircular<Jugador> JugadoresCancha { get; set; }
        public ListaCircular<Jugador> JugadoresSuplentes { get; set; }

        public Formacion(ListaCircular<Jugador> jugadoresCancha, ListaCircular<Jugador> jugadoresSuplentes)
        {
            JugadoresCancha = jugadoresCancha;
            JugadoresSuplentes = jugadoresSuplentes;
        }

        /// <summary>
        /// Obtiene los jugadores en la línea de defensa de un equipo
        /// </summary>
        /// <returns>Lista de <c>Jugador</c> con los jugadores en zona 1, 5 y 6</returns>
        public List<Jugador> ObtenerDefensas()
        {
            return new List<Jugador>()
            {
                JugadoresCancha.ElementAt(0), // Jugador en zona de servicio (1)
                JugadoresCancha.ElementAt(4), // Jugador en zona de zaguero lateral (5)
                JugadoresCancha.ElementAt(5)  // Jugador en zona de zaguero medio (6)
            };
        }

        /// <summary>
        /// Obtiene los jugadores en la línea de ataque de un equipo
        /// </summary>
        /// <returns>Lista de <c>Jugador</c> con los jugadores en zona 2, 3 y 4</returns>
        public List<Jugador> ObtenerAtacantes()
        {
            return new List<Jugador>()
            {
                JugadoresCancha.ElementAt(1), // Jugador en zona de opuesto (2)
                JugadoresCancha.ElementAt(2), // Jugador en zona de armador (3)
                JugadoresCancha.ElementAt(3)  // Jugador en zona de lateral (4)
            };
        }

        /// <summary>
        /// Obtiene el jugador que se encuentra en la zona <paramref name="nZona"/>
        /// </summary>
        /// <param name="nZona">Zona a filtrar</param>
        /// <returns>Objeto <c>Jugador</c></returns>
        /// <exception cref="Exception">En caso de que la formación tenga menos zonas de la que se solicita</exception>
        public Jugador ObtenerJugadorZona(int nZona)
        {
            if (JugadoresCancha.Count() < nZona)
            {
                throw new Exception("No hay suficientes jugadores en cancha");
            }

            return JugadoresCancha.ElementAt(nZona - 1);
        }

        /// <summary>
        /// Dado un jugador, se ubica la zona de la cancha en la que se encuentra
        /// </summary>
        /// <param name="jugador">Jugador a buscar</param>
        /// <returns>Número entero que representa la zona en la que se encuentra <paramref name="jugador"/></returns>
        public int DeterminarZonaJugador(Jugador jugador)
        {
            int zona;
            for (zona = 0 ; zona<JugadoresCancha.Count() ; zona++)
            {
                if (JugadoresCancha.ElementAt(zona).NumeroCamiseta == jugador.NumeroCamiseta) break;
            }

            return zona + 1;
        }
    }

    // Modelos para API's relacionadas al equipo
    public class Team
    {
        public string? name { get; set; }
        public bool national { get; set; }
    }

    public class TeamsRaiz : IConsumido
    {
        public List<Team>? response { get; set; }
    }
}