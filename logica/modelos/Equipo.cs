using Newtonsoft.Json;
using Persistencia.Infraestructura;
using Spectre.Console;

namespace Logica.Modelo;

/// <summary>
/// Clase que representa un equipo compuesto por x jugadores
/// </summary>
public class Equipo
{
    private string nombre;
    private int nJugadores;
    private List<Jugador> jugadores;
    private Formacion? formacionPartido;
    private bool esEquipoJugador;

    public Equipo() 
    {
        nombre = "Nombre sin especificar";
        jugadores = new List<Jugador>();
        nJugadores = 0;
        EsEquipoJugador = false;
    }

    // Propiedades
    
    [JsonProperty("nombre_equipo")]
    public string Nombre { get => nombre; set => nombre = value; }

    [JsonProperty("total_jugadores")]
    public int TotalJugadores { get => nJugadores; set => nJugadores = value; }

    [JsonProperty("es_equipo_jugador")]
    public bool EsEquipoJugador { get => esEquipoJugador; set => esEquipoJugador = value; }

    [JsonProperty("jugadores")]
    public List<Jugador> Jugadores 
    { 
        get => jugadores; 
        set {
            jugadores = value;
            nJugadores = jugadores.Count(); 
        }
    }

    [JsonIgnore]
    public Formacion? FormacionPartido { get => formacionPartido; set => formacionPartido = value; }

    [JsonProperty("jugadores_convocados")]
    public List<string> JugadoresConvocados { get => (formacionPartido != null) ? formacionPartido.ObtenerListaJugadores().Select(j => $"{j.Nombre} ({j.NumeroCamiseta})").ToList() : new List<string>(); }

    // Métodos

    /// <summary>
    /// Agrega un jugador al equipo incrementando el número de jugadores
    /// </summary>
    /// <param name="jugador">Jugador a agregar</param>
    public void AgregarJugador(Jugador jugador)
    {
        jugadores.Add(jugador);
        nJugadores++;
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