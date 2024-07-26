namespace Logica.Modelo;

/// <summary>
/// Representa un set del partido, es decir, desde que los equipos
/// tienen 0 puntos hasta que llegan a la cantidad indicada en PuntosParaSet
/// </summary>
public class Set
{
    public const int PuntosParaSet = 5;
    public const int SustitucionesPorSet = 12;

    public int NumeroSet { get; set; }
    public int SustitucionesLocal { get; set; }
    public int SustitucionesVisitantes { get; set; }
    public ResultadoSet Resultado { get; set; }
    public Sustituciones Sustituciones { get; set; }

    public Set()
    {
        NumeroSet = 1;
        Resultado = new ResultadoSet();
        Sustituciones = new Sustituciones();
        SustitucionesLocal = 0;
        SustitucionesVisitantes = 0;
    }

    /// <summary>
    /// Determina si un equipo ya sea local o visitante ha ganado el set
    /// </summary>
    /// <returns><c>True</c> si un equipo ya ha ganado el set, <c>False</c> en caso contrario</returns>
    public bool HayGanadorSet()
    {
        return (Resultado.PuntosLocal >= PuntosParaSet && Resultado.PuntosLocal - Resultado.PuntosVisitante >= 2) ||
               (Resultado.PuntosVisitante >= PuntosParaSet && Resultado.PuntosVisitante - Resultado.PuntosLocal >= 2);
    }

    /// <summary>
    /// Obtiene las sustituciones que le quedan en el set al equipo local o visitante
    /// </summary>
    /// <param name="tipoEquipo">Tipo de equipo a revisar (Local o visitante)</param>
    /// <returns><c>int</c> sustituciones restantes del equipo <paramref name="tipoEquipo"/></returns>
    public int ObtenerSustitucionesRestantes(TipoEquipo tipoEquipo)
    {
        return SustitucionesPorSet - ((tipoEquipo == TipoEquipo.LOCAL) ? SustitucionesLocal 
                                                                       : SustitucionesVisitantes);
    }

    /// <summary>
    /// Actualiza la información del set para pasar al siguiente en el partido
    /// </summary>
    public void SiguienteSet()
    {
        NumeroSet++;
        Resultado = new ResultadoSet();
        Sustituciones = new Sustituciones();
        SustitucionesLocal = 0;
        SustitucionesVisitantes = 0;
    }
}

/// <summary>
/// Clase que almacena datos de un set
/// </summary>
public class ResultadoSet
{
    public int PuntosLocal { get; set; }
    public int PuntosVisitante { get; set; }

    public ResultadoSet()
    {
        PuntosLocal = 0;
        PuntosVisitante = 0;
    }

    /// <summary>
    /// Incrementa un punto a alguno de los equipos
    /// </summary>
    /// <param name="equipo">Equipo al cual incrementar un punto. Ver <see cref="TipoEquipo"/></param>
    public void IncrementarPuntos(TipoEquipo equipo)
    {
        if (equipo == TipoEquipo.LOCAL) 
            PuntosLocal++;
        else 
            PuntosVisitante++;
    }
}