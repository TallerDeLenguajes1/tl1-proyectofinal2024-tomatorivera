using Logica.Modelo;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
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
}