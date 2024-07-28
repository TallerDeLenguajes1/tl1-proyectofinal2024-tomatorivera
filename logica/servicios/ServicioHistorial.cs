using Logica.Modelo;
using Persistencia.Repositorios;

namespace Logica.Servicios;

public interface IHistorialServicio
{
    void CrearHistorial(Historial historial);
    Historial ObtenerDatosHistorial();
    Historial ObtenerDatosHistorial(int id);
    void GuardarPartido(Partido partido);
}

public class HistorialServicioImpl : IHistorialServicio
{
    private readonly HistorialRepositorio repositorio;

    public HistorialServicioImpl() 
    {
        repositorio = new HistorialRepositorio();
    }

    /// <summary>
    /// Genera un nuevo historial para la partida actual
    /// </summary>
    /// <param name="historial">Datos del <c>Historial</c> a almacenar</param>
    public void CrearHistorial(Historial historial)
    {
        try 
        {
            repositorio.Crear(historial);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Obtiene los datos del historial correspondiente a la partida actual
    /// </summary>
    /// <returns>Objeto <c>Historial</c></returns>
    public Historial ObtenerDatosHistorial()
    {
        try
        {
            return repositorio.ObtenerActual();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Obtiene el historial de partidos de una partida en específico
    /// </summary>
    /// <param name="id">ID de la partida</param>
    /// <returns>Objeto <c>Historial</c></returns>
    public Historial ObtenerDatosHistorial(int id)
    {
        try
        {
            return repositorio.Cargar(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Guarda un partido en el historial
    /// </summary>
    /// <param name="partido">Objeto <c>Partido</c> a insertar en el historial</param>
    public void GuardarPartido(Partido partido)
    {
        try
        {
            var historialPartidaActual = repositorio.ObtenerActual();
            historialPartidaActual.AgregarPartido(partido);

            repositorio.Guardar(historialPartidaActual);
        }
        catch (Exception)
        {
            throw;
        }
    }
}