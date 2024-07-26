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
    private Color colorConsola;

    public Equipo() 
    {
        nombre = "Nombre sin especificar";
        jugadores = new List<Jugador>();
        nJugadores = 0;
        EsEquipoJugador = false;
        colorConsola = Color.White;
    }

    // Propiedades
    
    [JsonProperty("NombreEquipo")]
    public string Nombre { get => nombre; set => nombre = value; }
    public int TotalJugadores { get => nJugadores; set => nJugadores = value; }
    public bool EsEquipoJugador { get => esEquipoJugador; set => esEquipoJugador = value; }
    public List<Jugador> Jugadores 
    { 
        get => jugadores; 
        set {
            jugadores = value;
            nJugadores = jugadores.Count(); 
        }
    }
    public Formacion? FormacionPartido { get => formacionPartido; set => formacionPartido = value; }
    public Color ColorConsola { get => colorConsola; set => colorConsola = value; }
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