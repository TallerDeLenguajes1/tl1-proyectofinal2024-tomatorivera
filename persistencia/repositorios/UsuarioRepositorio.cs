using System.Text.Json;
using Logica.Excepciones;
using Logica.Modelo;
using Persistencia.Infraestructura;

namespace Persistencia.Repositorios
{
    public class UsuarioRepositorioImpl : IRepositorio<Usuario>
    {
        public Usuario? usuarioActual { get; set; }

        public void Crear(Usuario obj)
        {
            // Si por alguna razón no se ha configurado el directorio de la partida actual al cargar/crear
            // la partida, entonces se lanzará una excepción puesto que no hay carpeta donde persistir
            if (String.IsNullOrWhiteSpace(Config.DirectorioPartidaActual))
                throw new PartidaInvalidaException("No se pudo cargar el directorio de la partida actual de la configuración");

            // Guardo el nombre del archivo
            string nombreArchivo = Config.DirectorioPartidaActual + @"\" + Config.NombreJsonUsuario;

            // Creo el archivo del usuario
            using (FileStream usuarioJson = new FileStream(nombreArchivo, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(usuarioJson))
                {
                    string usuarioSerializado = JsonSerializer.Serialize(obj, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true });
                    writer.WriteLine(usuarioSerializado);
                }
            }

            // Actualizo la instancia del usuario actual para manejar sus datos desde el programa
            this.usuarioActual = obj;
        }

        public Usuario Cargar(int id)
        {
            throw new NotImplementedException();
        }

        public void Guardar(Usuario obj)
        {
            throw new NotImplementedException();
        }

        public Usuario ObtenerActual()
        {
            throw new NotImplementedException();
        }
    }
}