using Logica.Excepciones;
using Logica.Modelo;
using Newtonsoft.Json;
using Persistencia.Infraestructura;

namespace Persistencia.Repositorios
{
    public class HistorialRepositorio : IRepositorio<Historial>
    {
        public Historial? HistorialActual { get; set; }

        public Historial Cargar(int id)
        {
            throw new NotImplementedException();
        }

        public void Crear(Historial obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");
            
            string nombreArchivo = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonHistorial}";

            // Creo el archivo del historial
            using (FileStream historialJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter historialWriter = new StreamWriter(historialJson))
                {
                    string historialSerializado = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml, Formatting = Formatting.Indented });
                    historialWriter.WriteLine(historialSerializado);
                }
            }

            // Actualizo la instancia del historial actual
            this.HistorialActual = obj;
        }

        public void Guardar(Historial obj)
        {
            throw new NotImplementedException();
        }

        public Historial ObtenerActual()
        {
            throw new NotImplementedException();
        }
    }
}