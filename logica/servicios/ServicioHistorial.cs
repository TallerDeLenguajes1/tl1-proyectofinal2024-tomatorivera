using Logica.Modelo;
using Persistencia.Repositorios;

namespace Logica.Servicios;

public interface IHistorialServicio
{
    void CrearHistorial(Historial historial);
    Historial ObtenerDatosHistorial(int id);
}

public class HistorialServicioImpl : IHistorialServicio
{
    private HistorialRepositorio repositorio;

    public HistorialServicioImpl() 
    {
        this.repositorio = new HistorialRepositorio();
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
}