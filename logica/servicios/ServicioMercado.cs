using Logica.Modelo;
using Persistencia;
using Persistencia.Repositorios;

namespace Logica.Servicios;

/// <summary>
/// Servicio para la gestión del mercado
/// </summary>
public interface IMercadoServicio
{
    void CrearMercado(Mercado mercado);
    Task<Mercado> RegenerarMercadoAsync();
    Mercado ObtenerDatosMercado();
    Mercado ObtenerDatosMercado(int id);
    void GuardarMercado(Mercado mercado);
    void EliminarMercado();
    void RealizarCompraJugador(Jugador jugador);
}

public class MercadoServicioImpl : IMercadoServicio
{
    private readonly IRepositorio<Mercado> repositorio;
    private readonly IEquipoJugadoresServicio jugadoresServicio;

    public MercadoServicioImpl()
    {
        repositorio = new MercadoRepositorio();
        jugadoresServicio = new EquipoJugadoresServicioImpl();
    }

    /// <summary>
    /// Crea un nuevo Mercado en los archivos de persistencia
    /// </summary>
    /// <param name="mercado">Instancia de <see cref="Mercado"/> a almacenar</param>
    public void CrearMercado(Mercado mercado)
    {
        try
        {
            repositorio.Crear(mercado);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Genera los jugadores para el mercado y actualiza la fecha de última actualización
    /// </summary>
    /// <returns><see cref="Task"/> de la tarea asincrónica</returns>
    public async Task<Mercado> RegenerarMercadoAsync()
    {
        try
        {
            Mercado nuevoMercado;

            // Si la instancia del mercado en el repositorio es nula, genero una nueva
            // porque puedo necesitar generar el mercado por primera vez
            try
            {
                nuevoMercado = repositorio.ObtenerActual();
            }
            catch (Exception)
            {
                nuevoMercado = new Mercado();
            }
            
            var nuevosJugadores = await jugadoresServicio.GenerarJugadoresAsync(nuevoMercado.MaximoJugadoresPorMercado);

            nuevoMercado.Jugadores = nuevosJugadores;
            nuevoMercado.UltimaActualizacion = DateTime.Now;
            repositorio.Guardar(nuevoMercado);

            return nuevoMercado;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Obtiene la instancia del Mercado correspondiente
    /// a la partida actual 
    /// </summary>
    /// <returns>Instancia de <see cref="Mercado"/></returns>
    public Mercado ObtenerDatosMercado()
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
    /// Obtiene los datos de un mercado correspondiente a la partida
    /// del <paramref name="id"/> provisto
    /// </summary>
    /// <param name="id">ID de la partida de la cual cargar</param>
    /// <returns>Instancia de <see cref="Mercado"/></returns>
    public Mercado ObtenerDatosMercado(int id)
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
    /// Persiste los datos del mercado en la partida actual
    /// </summary>
    /// <param name="mercado">Instancia de <see cref="Mercado"/> con los datos a persistir</param>
    public void GuardarMercado(Mercado mercado)
    {
        try
        {
            repositorio.Guardar(mercado);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Elimina la persistencia del mercado de la partida actual
    /// </summary>
    public void EliminarMercado()
    {
        try
        {
            repositorio.Eliminar();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Realiza la compra de un jugador para el usuario
    /// </summary>
    /// <param name="jugador">Jugador a comprar</param>
    public void RealizarCompraJugador(Jugador jugador)
    {

    }
}