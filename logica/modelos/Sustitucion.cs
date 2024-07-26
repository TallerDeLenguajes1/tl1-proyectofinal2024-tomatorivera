namespace Logica.Modelo;

public class Sustitucion
{
    public Jugador Entrante { get; }
    public Jugador Saliente { get; }

    public Sustitucion(Jugador entrante, Jugador saliente)
    {
        Entrante = entrante;
        Saliente = saliente;
    }   
}

public class Sustituciones
{
    public List<Sustitucion> SustitucionesLocal { get; }
    public List<Sustitucion> SustitucionesVisitante { get; }

    public Sustituciones()
    {
        SustitucionesLocal = new List<Sustitucion>();
        SustitucionesVisitante = new List<Sustitucion>();
    }
}