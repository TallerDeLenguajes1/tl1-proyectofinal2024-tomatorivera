using Newtonsoft.Json;
using Persistencia.Infraestructura;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena toda la información necesaria
    /// de los partidos que juegue el usuario
    /// </summary>
    public class Partido
    {
        private const int puntosParaGanar = 5;

        private Equipo local;
        private Equipo visitante;
        private TipoPartido tipoPartido;
        private Dictionary<int /* nro set */, (int, int) /* L, V */> resultadoSets;
        private int scoreLocal;
        private int scoreVisitante;
        private string nombreGanador;

        public Partido(Equipo local, Equipo visitante, TipoPartido tipoPartido)
        {
            this.local = local;
            this.visitante = visitante;
            this.tipoPartido = tipoPartido;

            // Valores por defecto
            resultadoSets = new Dictionary<int, (int, int)>();
            scoreLocal = 0;
            scoreVisitante = 0;
            nombreGanador = string.Empty;
        }

        // Propiedades
        public Equipo Local { get => local; set => local = value; }
        public Equipo Visitante { get => visitante; set => visitante = value; }
        public TipoPartido TipoPartido { get => tipoPartido; set => tipoPartido = value; }
        public Dictionary<int, (int, int)> ResultadoSets { get => resultadoSets; set => resultadoSets = value; }
        public string NombreGanador { get => nombreGanador; set => nombreGanador = value; }
        public int ScoreLocal { get => scoreLocal; set => scoreLocal = value; }
        public int ScoreVistante { get => scoreVisitante; set => scoreVisitante = value; }
        public static int PuntosParaGanar => puntosParaGanar;

        // Métodos
        public override string ToString()
        {
            return $"Partido {TipoPartido} - {local} (local) VS {visitante} (visitante) - Ganador: {nombreGanador}";
        }
    }

    // Modelos para API's relacionadas a los partidos
    public class GamesRaiz : IConsumido
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public List<GamesResponse>? Response { get; set; }
    }

    public class GamesResponse
    {

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Date { get; set; }

        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string? Timezone { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public GameLeagueStatus Status { get; set; }

        [JsonProperty("teams", NullValueHandling = NullValueHandling.Ignore)]
        public Teams Teams { get; set; }

        [JsonProperty("scores", NullValueHandling = NullValueHandling.Ignore)]
        public GameScore Scores { get; set; }
    }


    public class GameTeam
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("logo", NullValueHandling = NullValueHandling.Ignore)]
        public string? Logo { get; set; }
    }


    public class GameLeagueStatus
    {
        [JsonProperty("long", NullValueHandling = NullValueHandling.Ignore)]
        public string? Long { get; set; }
        
        [JsonProperty("short", NullValueHandling = NullValueHandling.Ignore)]
        public string? Short { get; set; }
    }

    public class GameScore
    {
        [JsonProperty("home", NullValueHandling = NullValueHandling.Ignore)]
        public int Home { get; set; }

        [JsonProperty("away", NullValueHandling = NullValueHandling.Ignore)]
        public int Away { get; set; }
    }

    public class Teams
    {
        [JsonProperty("home", NullValueHandling = NullValueHandling.Ignore)]
        public GameTeam Home { get; set; }

        [JsonProperty("away", NullValueHandling = NullValueHandling.Ignore)]
        public GameTeam Away { get; set; }
    }

    public class LeaguesRaiz : IConsumido
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public List<LeagueResponse>? Response { get; set; }
    }

    public class LeagueResponse
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        
        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public LeagueCountry Country { get; set; }

        public LeagueResponse()
        {
            this.Id = -1;
            this.Name = "Indefinido";
            this.Country = new LeagueCountry();
        }
    }

    public class LeagueCountry
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        public LeagueCountry()
        {
            this.Name = "Indefinido";
        }
    }
}