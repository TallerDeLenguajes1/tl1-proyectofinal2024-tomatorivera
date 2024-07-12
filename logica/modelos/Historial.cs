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
            this.historialPartidos = new List<Partido>();
        }

        // Propiedades
        public int TotalPartidosJugados { get => nPartidosJugados; set => nPartidosJugados = value;}
        public List<Partido> HistorialPartidos { get => historialPartidos; set => HistorialPartidos = value; }  

        // Métodos 
        public void AgregarPartido(Partido partido)
        {
            this.historialPartidos.Add(partido);
            nPartidosJugados++;
        }
    }
}