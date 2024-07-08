using Persistencia.Infraestructura;

namespace Logica.Modelo
{
    public class Equipo
    {
        private string nombre;
        private List<Jugador> jugadores;

        public Equipo() 
        {
            this.nombre = "Nombre sin especificar";
            this.jugadores = new List<Jugador>();
        }
        public Equipo(string nombre)
        {
            this.nombre = nombre;
            this.jugadores = new List<Jugador>();
        }

        // Propiedades
        public string Nombre { get => nombre; set => nombre = value; }
        public List<Jugador> Jugadores { get => jugadores; set => jugadores = value; }
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