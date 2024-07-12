using Logica.Excepciones;
using Logica.Handlers;
using Logica.Modelo;
using Persistencia.Infraestructura;

namespace Logica.Servicios
{
    public interface INovedadesServicio
    {
        Task<List<LeagueResponse>> ObtenerLigasAsync(int temporada = 0);
        Task<List<GamesResponse>> ObtenerPartidosAsync(int competicionId, int temporada = 0);
    }

    public class NovedadesServicioImpl : INovedadesServicio
    {
        /// <summary>
        /// Obtiene datos de ligas de volley de una cierta temporada desde API Sports
        /// </summary>
        /// <param name="temporada">Año de los datos de las ligas a traer (por defecto, el año actual)</param>
        /// <returns>Lista de <c>LeagueResponse</c> con datos de las ligas obtenidas</returns>
        public async Task<List<LeagueResponse>> ObtenerLigasAsync(int temporada)
        {
            List<LeagueResponse> ligas;

            // Si no se especifica la temporada, se usa la actual
            if (temporada == 0)
                temporada = DateTime.Now.Year;

            try
            {
                // Verifico si se han cargado las configuraciones necesarias para acceder a la api
                if (string.IsNullOrWhiteSpace(Config.ApiSportsUrl))
                    throw new ApiInaccesibleException("La URL de Api Sports es nula o está vacía");
                
                if (string.IsNullOrWhiteSpace(Config.ApiSportsKey))
                    throw new ApiInaccesibleException("No se pudo cargar la KEY de Api Sports, revise la configuración");

                var consumidor = new Consumidor<LeaguesRaiz>(Config.ApiSportsUrl);
                consumidor.AgregarSubUrl("leagues");
                consumidor.AgregarCabecera("x-apisports-key", Config.ApiSportsKey);
                consumidor.AgregarParametro("season", temporada.ToString());

                var respuesta = await consumidor.ConsumirAsync();

                if (respuesta == null || respuesta.Response == null || !respuesta.Response.Any())
                    throw new RespuestaApiInvalidaException("La respuesta de API Sports es nula o está vacía");

                ligas = respuesta.Response;
            } 
            catch (Exception ex) 
            {
                ErroresIgnorablesHandler.ObtenerInstancia().Errores.TryAdd("Obtener Ligas", ex);

                // En caso de error obteniendo las ligas, retorno una lista vacía
                ligas = new List<LeagueResponse>();
            }

            return ligas;
        }

        /// <summary>
        /// Obtiene una lista con detalle de partidos de cierta competición de voley en determinada temporada
        /// </summary>
        /// <param name="competicionId">ID de la competición a filtrar</param>
        /// <param name="temporada">Temporada de los datos a buscar (por defecto, el año actual)</param>
        /// <returns>Lista de <c>GamesResponse</c> con datos de los partidos obtenidos</returns>
        public async Task<List<GamesResponse>> ObtenerPartidosAsync(int competicionId, int temporada = 0)
        {
            List<GamesResponse> partidos;

            // Si no se especifica la temporada, se usa la actual
            if (temporada == 0)
                temporada = DateTime.Now.Year;

            try
            {
                // Verifico si se han cargado las configuraciones necesarias para acceder a la api
                if (string.IsNullOrWhiteSpace(Config.ApiSportsUrl))
                    throw new ApiInaccesibleException("La URL de Api Sports es nula o está vacía");
                
                if (string.IsNullOrWhiteSpace(Config.ApiSportsKey))
                    throw new ApiInaccesibleException("No se pudo cargar la KEY de Api Sports, revise la configuración");

                var consumidor = new Consumidor<GamesRaiz>(Config.ApiSportsUrl);
                consumidor.AgregarSubUrl("games");
                consumidor.AgregarCabecera("x-apisports-key", Config.ApiSportsKey);
                consumidor.AgregarParametro("league", competicionId.ToString());
                consumidor.AgregarParametro("season", temporada.ToString());

                var respuesta = await consumidor.ConsumirAsync();

                if (respuesta.Response == null || !respuesta.Response.Any())
                    throw new RespuestaApiInvalidaException("La respuesta de API Sports es nula o está vacía");

                partidos = respuesta.Response;
            }
            catch (Exception ex)
            {
                ErroresIgnorablesHandler.ObtenerInstancia().Errores.TryAdd("Obtener partidos", ex);

                // En caso de error obteniendo los partidos, retorno una lista vacía
                partidos = new List<GamesResponse>();
            }

            return partidos;
        }
    }
}