namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena toda la información necesaria
    /// de los partidos que juegue el usuario
    /// </summary>
    public class Partido
    {
         private string local;
        private string visitante;
        private TipoPartido tipoPartido;
        private Dictionary<int /* nro set */, (int, int) /* L, V */> resultadoSets;
        private int setsLocal;
        private int setsVisitante;
        private string nombreGanador;

        // Propiedades
        public string Local { get => local; set => local = value; }
        public string Visitante { get => visitante; set => visitante = value; }
        public TipoPartido TipoPartido { get => tipoPartido; set => tipoPartido = value; }
        public Dictionary<int, (int, int)> ResultadoSets { get => resultadoSets; set => resultadoSets = value; }
        public string NombreGanador { get => nombreGanador; set => nombreGanador = value; }
        public int SetsLocal { get => setsLocal; set => setsLocal = value; }
        public int SetsVisitante { get => setsVisitante; set => setsVisitante = value; }

        // Métodos
        public override string ToString()
        {
            return $"Partido {TipoPartido} - {local} (local) VS {visitante} (visitante) - Ganador: {nombreGanador}";
        }
    }
}