using Newtonsoft.Json;

namespace Logica.Modelo;

/// <summary>
/// Representa la formación en un partido de un equipo, almacenando en una
/// <c>Lista Circular</c> los jugadores en cancha y los jugadores suplentes
/// </summary>
public class Formacion
{
    public ListaCircular<Jugador> JugadoresCancha { get; set; }
    public ListaCircular<Jugador> JugadoresSuplentes { get; set; }

    public Formacion(ListaCircular<Jugador> jugadoresCancha, ListaCircular<Jugador> jugadoresSuplentes)
    {
        JugadoresCancha = jugadoresCancha;
        JugadoresSuplentes = jugadoresSuplentes;
    }

    /// <summary>
    /// Obtiene los jugadores en la línea de defensa de un equipo
    /// </summary>
    /// <returns>Lista de <c>Jugador</c> con los jugadores en zona 1, 5 y 6</returns>
    public List<Jugador> ObtenerDefensas()
    {
        return new List<Jugador>()
        {
            ObtenerJugadorZona(1), // Jugador en zona de servicio (1)
            ObtenerJugadorZona(5), // Jugador en zona de zaguero lateral (5)
            ObtenerJugadorZona(6)  // Jugador en zona de zaguero medio (6)
        };
    }

    /// <summary>
    /// Obtiene los jugadores en la línea de ataque de un equipo
    /// </summary>
    /// <returns>Lista de <c>Jugador</c> con los jugadores en zona 2, 3 y 4</returns>
    public List<Jugador> ObtenerAtacantes()
    {
        return new List<Jugador>()
        {
            ObtenerJugadorZona(2), // Jugador en zona de opuesto (2)
            ObtenerJugadorZona(3), // Jugador en zona de armador (3)
            ObtenerJugadorZona(4)  // Jugador en zona de lateral (4)
        };
    }

    /// <summary>
    /// Obtiene el jugador que se encuentra en la zona <paramref name="nZona"/>
    /// </summary>
    /// <param name="nZona">Zona a filtrar</param>
    /// <returns>Objeto <c>Jugador</c></returns>
    /// <exception cref="InvalidOperationException">En caso de que la formación tenga menos zonas de la que se solicita</exception>
    public Jugador ObtenerJugadorZona(int nZona)
    {
        if (JugadoresCancha.Count() < nZona)
            throw new InvalidOperationException("No hay suficientes jugadores en cancha");

        return JugadoresCancha.ElementAt(nZona - 1);
    }

    /// <summary>
    /// Dado un jugador, se ubica la zona de la cancha en la que se encuentra
    /// </summary>
    /// <param name="jugador">Jugador a buscar</param>
    /// <returns>Número entero que representa la zona en la que se encuentra <paramref name="jugador"/></returns>
    /// <exception cref="InvalidOperationException">Cuando el jugador buscado no está en la lista de jugadores en cancha</exception>
    public int DeterminarZonaJugador(Jugador jugador)
    {
        if (!JugadoresCancha.Contiene(jugador))
        {
            throw new InvalidOperationException($"El jugador {jugador.Nombre} no está en la cancha");
        }

        int zona;
        for (zona = 0 ; zona<JugadoresCancha.Count() ; zona++)
        {
            if (JugadoresCancha.ElementAt(zona).NumeroCamiseta == jugador.NumeroCamiseta) break;
        }

        return zona + 1;
    }

    /// <summary>
    /// Genera una lista con todos los jugadores de la formación, tanto titulares como suplentes
    /// </summary>
    /// <returns><c>List</c> de <c>Jugador</c></returns>
    public List<Jugador> ObtenerListaJugadores()
    {
        var lista = new List<Jugador>();

        // Agrego los titulares
        foreach (var jugador in JugadoresCancha)
            lista.Add(jugador);

        // Agrego los suplentes
        foreach (var jugador in JugadoresSuplentes)
            lista.Add(jugador);

        return lista;
    }
}