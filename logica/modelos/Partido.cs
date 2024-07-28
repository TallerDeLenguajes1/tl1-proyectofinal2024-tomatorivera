using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Persistencia.Infraestructura;

namespace Logica.Modelo;

/// <summary>
/// Clase modelo que almacena toda la información necesaria
/// de los partidos que juegue el usuario
/// </summary>
public class Partido
{
    public const int PuntosParaSet = 5;

    private Equipo local;
    private Equipo visitante;
    private Equipo equipoEnSaque;
    private TipoPartido tipoPartido;
    private Set setActual;
    private Dictionary<int, ResultadoSet> resultadoSets;
    private int scoreLocal;
    private int scoreVisitante;
    private int setMaximos;
    private string nombreGanador;

    public Partido(Equipo local, Equipo visitante, TipoPartido tipoPartido)
    {
        this.local = local;
        this.visitante = visitante;
        this.tipoPartido = tipoPartido;

        // Valores por defecto
        setActual = new Set();
        resultadoSets = new Dictionary<int, ResultadoSet>();
        scoreLocal = 0;
        scoreVisitante = 0;
        nombreGanador = string.Empty;
        equipoEnSaque = local;
    }

    // Propiedades

    [JsonProperty("tipo_de_partido")]
    [JsonConverter(typeof(StringEnumConverter))]
    public TipoPartido TipoPartido { get => tipoPartido; set => tipoPartido = value; }

    [JsonProperty("nombre_ganador")]
    public string NombreGanador { get => nombreGanador; set => nombreGanador = value; }

    [JsonProperty("al_mejor_de")]
    public int SetMaximos { get => setMaximos ; set => setMaximos = value; }

    [JsonProperty("equipo_local")]
    public Equipo Local { get => local; set => local = value; }

    [JsonProperty("equipo_visitante")]
    public Equipo Visitante { get => visitante; set => visitante = value; }

    [JsonProperty("score_local")]
    public int ScoreLocal { get => scoreLocal; set => scoreLocal = value; }

    [JsonProperty("score_visitante")]
    public int ScoreVisitante { get => scoreVisitante; set => scoreVisitante = value; }

    [JsonProperty("resultados_sets")]
    public Dictionary<int, ResultadoSet> ResultadoSets { get => resultadoSets; set => resultadoSets = value; }

    [JsonIgnore]
    public Set SetActual { get => setActual; set => setActual = value; }

    [JsonIgnore]
    public Equipo EquipoEnSaque { get => equipoEnSaque; set => equipoEnSaque = value; }

    // Métodos
    public Equipo ObtenerEquipoJugador()
    {
        return local.EsEquipoJugador ? local : visitante;
    }

    public Equipo ObtenerEquipoConsola()
    {
        return (!local.EsEquipoJugador) ? local : visitante;
    }

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