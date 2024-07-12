namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena toda la información necesaria
    /// de los partidos que juegue el usuario
    /// </summary>
    public class Partido
    {
        private Equipo local;
        private Equipo visitante;
        private TipoPartido tipoPartido;
        private Dictionary<int /* nro set */, (int, int) /* L, V */> resultadoSets;
        private int scoreLocal;
        private int scoreVisitante;
        private string nombreGanador;

        // Propiedades
        public Equipo Local { get => local; set => local = value; }
        public Equipo Visitante { get => visitante; set => visitante = value; }
        public TipoPartido TipoPartido { get => tipoPartido; set => tipoPartido = value; }
        public Dictionary<int, (int, int)> ResultadoSets { get => resultadoSets; set => resultadoSets = value; }
        public string NombreGanador { get => nombreGanador; set => nombreGanador = value; }
        public int ScoreLocal { get => scoreLocal; set => scoreLocal = value; }
        public int ScoreVistante { get => scoreVisitante; set => scoreVisitante = value; }

        // Métodos
        public override string ToString()
        {
            return $"Partido {TipoPartido} - {local} (local) VS {visitante} (visitante) - Ganador: {nombreGanador}";
        }
    }
}