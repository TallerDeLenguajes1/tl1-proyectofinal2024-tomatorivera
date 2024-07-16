using Newtonsoft.Json;
using Persistencia.Infraestructura;

namespace Logica.Modelo
{
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

    public class Formacion
    {
        public ListaCircular<Jugador> JugadoresCancha { get; set; }
        public ListaCircular<Jugador> JugadoresSuplentes { get; set; }

        public Formacion(ListaCircular<Jugador> jugadoresCancha, ListaCircular<Jugador> jugadoresSuplentes)
        {
            JugadoresCancha = jugadoresCancha;
            JugadoresSuplentes = jugadoresSuplentes;
        }
    }

    public enum TipoEquipo
    {
        LOCAL,
        VISITANTE
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