using Newtonsoft.Json;

namespace Logica.Modelo;

/// <summary>
/// Clase modelo encargada de gestionar la información del
/// Mercado de jugadores en el juego
/// </summary>
public class Mercado
{
    private const int maximoJugadoresPorMercado = 4;

    private DateTime ultimaActualizacion;
    private List<Jugador> jugadores;

    public Mercado()
    {
        jugadores = new List<Jugador>();
    }

    public Mercado(List<Jugador> jugadores)
    {
        this.jugadores = jugadores;
        ultimaActualizacion = DateTime.Now;
    }

    // Propiedades

    [JsonProperty("ultima_actualizacion")]
    public DateTime UltimaActualizacion { get => ultimaActualizacion; set => ultimaActualizacion = value; }

    [JsonProperty("jugadores_mercado")]
    public List<Jugador> Jugadores { get => jugadores; set => jugadores = value; }

    [JsonIgnore]
    public int MaximoJugadoresPorMercado { get => maximoJugadoresPorMercado; }
}