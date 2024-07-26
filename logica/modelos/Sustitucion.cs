using Logica.Excepciones;

namespace Logica.Modelo;

public class Sustitucion
{
    public Jugador Entrante { get; }
    public Jugador Saliente { get; }
    public bool EstaCicloSustitucionCumplido { get; set; }

    public Sustitucion(Jugador entrante, Jugador saliente)
    {
        Entrante = entrante;
        Saliente = saliente;
        EstaCicloSustitucionCumplido = false;
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

    /// <summary>
    /// Devuelve la lista de sustituciones correspondientes a <paramref name="equipo"/>.
    /// Solo aquellas cuyo ciclo de sustitución no se haya cumplido
    /// </summary>
    /// <param name="equipo">Tipo de equipo a obtener (Local o visitante)</param>
    /// <returns><c>List</c> de <c>Sustitucion</c></returns>
    public List<Sustitucion> ObtenerListaSustituciones(TipoEquipo equipo)
    {
        return ((equipo == TipoEquipo.LOCAL) ? SustitucionesLocal : SustitucionesVisitante)
                    .Where(sustitucion => !sustitucion.EstaCicloSustitucionCumplido)
                    .ToList();
    }

    /// <summary>
    /// Verifica si una sustitución es válida, es decir, si el jugador <paramref name="entrante"/> ha sido
    /// sustituido antes por <paramref name="saliente"/> o viceversa
    /// </summary>
    /// <param name="equipo">Tipo de equipo que realiza la sustitución (local o visitante)</param>
    /// <param name="entrante">Jugador que entra al campo de juego</param>
    /// <param name="saliente">Jugador que es sustituido</param>
    /// <exception cref="SustitucionInvalidaException">En caso de que la sustitución no pueda cumplirse</exception>
    public void VerificarSustitucion(TipoEquipo equipo, Jugador entrante, Jugador saliente)
    {
        var sustituciones = ObtenerListaSustituciones(equipo);

        // Si el equipo no ha hecho sustituciones aún, entonces se puede proceder con la actual
        if (!sustituciones.Any()) return;

        // Si el jugador que sale entró en lugar de otro jugador, debe ser reemplazado únicamente por aquel jugador
        var sustitucionesSaliente = sustituciones.Where(s => s.Entrante.Equals(saliente));
        if (sustitucionesSaliente.Any())
        {
            var sustitucionAnterior = sustitucionesSaliente.First();

            if (!sustitucionAnterior.Saliente.Equals(entrante))
                throw new SustitucionInvalidaException($"El jugador {saliente.Nombre} entró en lugar de {sustitucionAnterior.Saliente.Nombre} y solo puede ser sustituido por él");
        }
        
        // Si el jugador que entra ya ha salido anteriormente, debe ingresar unicamente por el jugador que lo reemplazó
        var sustitucionesEntrante = sustituciones.Where(sustitucion => sustitucion.Saliente.Equals(entrante));
        if (sustitucionesEntrante.Any())
        {
            var sustitucionAnterior = sustitucionesEntrante.First();

            if (!sustitucionAnterior.Entrante.Equals(saliente))
                throw new SustitucionInvalidaException($"El jugador {entrante.Nombre} fue sustituido por {sustitucionAnterior.Entrante.Nombre} y solo puede reingresar en su lugar");
        }
    }

    /// <summary>
    /// Verifica si una sustitución ha cumplido su ciclo, es decir, si el jugador <paramref name="entrante"/>
    /// ha sido reemplazado antes por <paramref name="saliente"/> y ha vuelto a ingresar en su lugar, es entonces
    /// cuando <paramref name="entrante"/> puede volver a ser reemplazado por cualquier otro jugador
    /// </summary>
    /// <param name="equipo">Tipo de equipo que realiza la sustitución (local o visitante)</param>
    /// <param name="entrante">Jugador que ingresa al campo de juego</param>
    /// <param name="saliente">Jugador que fue sustituido</param>
    public void VerificarCicloSustitucionCumplido(TipoEquipo equipo, Jugador entrante, Jugador saliente)
    {
        var sustituciones = ObtenerListaSustituciones(equipo);

        // Si el equipo no tiene sustituciones, no hay ningún ciclo que verificar
        if (!sustituciones.Any()) return;

        // Verifico si se ha realizado la primera sustitución, donde el jugador entrante ha sido reemplazado por saliente
        var primeraSustitucion = sustituciones.Where(s => s.Saliente.Equals(entrante) && s.Entrante.Equals(saliente));
        if (!primeraSustitucion.Any()) return;
        
        // Verifico si por alguna razón no existe la segunda sustitución, donde entrante reingresa en lugar de saliente
        var segundaSustitucion = sustituciones.Where(s => s.Entrante.Equals(entrante) && s.Saliente.Equals(saliente));
        if (!segundaSustitucion.Any()) return;

        // Marco el ciclo cumplido en las sustituciones para no filtrarlas nuevamente
        primeraSustitucion.First().EstaCicloSustitucionCumplido = true;
        segundaSustitucion.First().EstaCicloSustitucionCumplido = true;
    }
}