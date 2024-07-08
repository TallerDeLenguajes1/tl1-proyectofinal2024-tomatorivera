using Logica.Util;
using Newtonsoft.Json;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena información de la partida actual
    /// </summary>
    public class Partida
    {
        private int id;
        private DateTime fechaGuardado;
        private Usuario usuario;

        public Partida(int id)
        {
            this.id = id;

            // Valores por defecto
            this.fechaGuardado = DateTime.Now;
            this.usuario = new Usuario("No especificado");
        }

        public Partida(int id, DateTime fechaGuardado, Usuario usuario)
        {
            this.id = id;
            this.fechaGuardado = fechaGuardado;
            this.usuario = usuario;
        }

        // Propiedades
        public int Id { get => id; set => id = value; }
        public DateTime FechaGuardado { get => fechaGuardado; set => fechaGuardado = value; }

        [JsonProperty("NombreDT")] // En el json de la partida solo mostraré el nombre del usuario
        public string NombreUsuario => Usuario.Nombre;

        [JsonIgnore] // El usuario será mostrado en otro JSON
        public Usuario Usuario { get => usuario; set => usuario = value; }

        // Métodos

        public override string ToString()
        {
            if (id == -1) return "Volver al menú anterior";

            return (usuario != null) ? "Partida ID " + id + " - Creada el: " + fechaGuardado.ToString("dd/MM/yyyy") + " - DT: " + usuario.Nombre 
                                     : "Partida ID " + id;
        }
    }
}