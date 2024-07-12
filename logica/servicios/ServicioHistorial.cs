using Logica.Modelo;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
    public interface IHistorialServicio
    {
        void CrearHistorial(Historial historial);
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
    }
}