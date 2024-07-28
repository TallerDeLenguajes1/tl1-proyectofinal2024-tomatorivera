using Newtonsoft.Json;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo encargada de gestionar la información
    /// relacionada al historial de partidos
    /// </summary>
    public class Historial
    {
        private List<Partido> historialPartidos;
        private int nPartidosJugados;

        public Historial()
        {
            historialPartidos = new List<Partido>();
        }

        // Propiedades

        [JsonProperty("total_partidos_jugados")]
        public int TotalPartidosJugados { get => nPartidosJugados; set => nPartidosJugados = value;}

        [JsonProperty("historial_partidos")]
        public List<Partido> HistorialPartidos { get => historialPartidos; set => HistorialPartidos = value; }  

        // Métodos 
        public void AgregarPartido(Partido partido)
        {
            historialPartidos.Add(partido);
            nPartidosJugados++;
        }
    }
}