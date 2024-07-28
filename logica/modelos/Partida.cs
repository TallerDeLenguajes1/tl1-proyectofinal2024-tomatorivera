using Logica.Util;
using Newtonsoft.Json;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena toda la información de una partida
    /// </summary>
    public class Partida
    {
        private int id;
        private DateTime fechaGuardado;
        private Usuario usuario;
        private Historial historial;

        public Partida()
        {
            // Valores por defecto
            this.id = -1;
            this.fechaGuardado = DateTime.Now;
            this.usuario = new Usuario("No especificado");
            this.historial = new Historial();
        }

        public Partida(int id)
        {
            this.id = id;

            // Valores por defecto
            this.fechaGuardado = DateTime.Now;
            this.usuario = new Usuario("No especificado");
            this.historial = new Historial();
        }

        public Partida(int id, DateTime fechaGuardado, Usuario usuario)
        {
            this.id = id;
            this.fechaGuardado = fechaGuardado;
            this.usuario = usuario;

            // Valores por defecto
            this.historial = new Historial();
        }

        public Partida(int id, DateTime fechaGuardado, Usuario usuario, Historial historial)
        {
            this.id = id;
            this.fechaGuardado = fechaGuardado;
            this.usuario = usuario;
            this.historial = historial;
        }

        // Propiedades

        [JsonProperty("id_partida")]
        public int Id { get => id; set => id = value; }

        [JsonProperty("ultimo_guardado")]
        public DateTime FechaGuardado { get => fechaGuardado; set => fechaGuardado = value; }

        [JsonProperty("nombre_dt")] // En el json de la partida solo mostraré el nombre del usuario
        public string NombreUsuario => Usuario.Nombre;

        [JsonIgnore] // El usuario será mostrado en otro JSON
        public Usuario Usuario { get => usuario; set => usuario = value; }

        [JsonIgnore] // El historial será mostrado en otro JSON
        public Historial Historial { get => historial; set => historial = value; }

        // Métodos

        public override string ToString()
        {
            if (id == -1) return "Volver al menú anterior";

            return (usuario != null) ? "Partida ID " + id + " - Creada el: " + fechaGuardado.ToString("dd/MM/yyyy") + " - DT: " + usuario.Nombre 
                                     : "Partida ID " + id;
        }
    }
}