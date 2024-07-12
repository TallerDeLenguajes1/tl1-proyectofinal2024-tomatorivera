using Logica.Excepciones;
using Logica.Modelo;
using Newtonsoft.Json;
using Persistencia.Infraestructura;
using Persistencia.Util;

namespace Persistencia.Repositorios
{
    public class HistorialRepositorio : IRepositorio<Historial>
    {
        public Historial? HistorialActual { get; set; }


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

        public Historial Cargar(int id)
        {
            if (string.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudieron cargar los datos del historial. El directorio de la partida a cargar es nulo o está vacío");

            var historialJsonPath = @$"{Config.DirectorioPartidaActual}\{Config.NombreJsonHistorial}";

            RecursosUtil.VerificarArchivo(historialJsonPath);

            // Leo y deserealizo el archivo del historial
            Historial? historial;
            using (FileStream fileHistorial = new FileStream(historialJsonPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader readerHistorial = new StreamReader(fileHistorial))
                {
                    string historialJsonTxt = readerHistorial.ReadToEnd();
                    historial = JsonConvert.DeserializeObject<Historial>(historialJsonTxt);

                    if (historial == null)
                        throw new HistorialInvalidoException("No se pudo deserealizar el archivo JSON del historial, la respuesta ha sido nula");
                }
            }

            return historial;
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