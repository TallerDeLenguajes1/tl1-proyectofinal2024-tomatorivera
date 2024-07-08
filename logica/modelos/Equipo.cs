using Persistencia.Infraestructura;

namespace Logica.Modelo
{
    public class Equipo
    {
        private string nombre;
        private int nJugadores;
        private List<Jugador> jugadores;

        public Equipo() 
        {
            this.nombre = "Nombre sin especificar";
            this.jugadores = new List<Jugador>();
            this.nJugadores = 0;
        }
        public Equipo(string nombre)
        {
            this.nombre = nombre;
            this.jugadores = new List<Jugador>();
            this.nJugadores = 0;
        }

        // Propiedades
        public string Nombre { get => nombre; set => nombre = value; }
        public int TotalJugadores { get => nJugadores; set => nJugadores = value; }
        public List<Jugador> Jugadores 
        { 
            get => jugadores; 
            set {
                jugadores = value;
                nJugadores = jugadores.Count(); 
            }
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