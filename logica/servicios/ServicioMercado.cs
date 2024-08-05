using Logica.Excepciones;
using Logica.Modelo;
using Persistencia;
using Persistencia.Infraestructura;
using Persistencia.Repositorios;

namespace Logica.Servicios;

/// <summary>
/// Servicio para la gestión del mercado
/// </summary>
public interface IMercadoServicio
{
    void CrearMercado(Mercado mercado);
    Task<Mercado> RegenerarMercadoAsync();
    Task<List<Jugador>> GenerarJugadoresMercadoAsync();
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
    private readonly IUsuarioServicio usuarioServicio;

    public MercadoServicioImpl()
    {
        repositorio = new MercadoRepositorio();
        jugadoresServicio = new EquipoJugadoresServicioImpl();
        usuarioServicio = new UsuarioServicioImpl();
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
            var nuevoMercado = repositorio.ObtenerActual();

            nuevoMercado.Jugadores = await GenerarJugadoresMercadoAsync();
            nuevoMercado.UltimaActualizacion = DateTime.Now;

            // Guardo el nuevo mercado
            GuardarMercado(nuevoMercado);

            return nuevoMercado;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Genera una lista de nuevos jugadores para el mercado
    /// </summary>
    /// <returns>Lista de <see cref="Jugador"/></returns>
    public async Task<List<Jugador>> GenerarJugadoresMercadoAsync()
    {
        try
        {
            return await jugadoresServicio.GenerarJugadoresAsync(Config.LimiteJugadoresMercado);
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
    /// <exception cref="DineroInsuficienteException">Si el usuario no tiene suficiente dinero para la compra de <paramref name="jugador"/></exception>
    public void RealizarCompraJugador(Jugador jugador)
    {
        try
        {
            var usuarioActual = usuarioServicio.ObtenerDatosUsuario();

            // Verifico si el usuario posee dinero para la compra
            if (usuarioActual.Dinero < jugador.Precio)
                throw new DineroInsuficienteException("No tienes la cantidad de dinero suficiente para realizar la compra");
            // Verifico si el usuario tiene la plantilla llena
            if (usuarioActual.Equipo.Jugadores.Count() >= Config.LimiteJugadoresPlantilla)
                throw new PlantillaLlenaException("Ya has alcanzado el límite de jugadores en tu plantel");

            jugador.NumeroCamiseta = jugadoresServicio.GenerarIdentificadorUnico(usuarioActual.Equipo.Jugadores.Select(j => j.NumeroCamiseta).ToList());
            usuarioActual.Equipo.AgregarJugador(jugador);
            usuarioActual.Dinero -= jugador.Precio;
        }
        catch (Exception)
        {
            throw;
        }
    }
}