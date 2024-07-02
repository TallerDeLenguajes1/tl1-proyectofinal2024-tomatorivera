using System.Text.Json.Serialization;
using Logica.Helpers;

namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena información de la partida actual
    /// </summary>
    public class Partida
    {
        private int id;
        private DateTime fechaGuardado;
        
        [JsonConverter(typeof(UsuarioConverter))]
        private Usuario usuario;

        public Partida(int id, DateTime fechaGuardado, Usuario usuario)
        {
            this.id = id;
            this.fechaGuardado = fechaGuardado;
            this.usuario = usuario;
        }

        // Propiedades
        public int Id { get => id; set => id = value; }
        public DateTime FechaGuardado { get => fechaGuardado; set => fechaGuardado = value; }
        public Usuario Usuario { get => usuario; set => usuario = value; }

        // Métodos

        /// <summary>
        /// Inicia la lógica de una partida
        /// </summary>
        public void Iniciar() {
            // Falta implementar la lógica
            System.Console.WriteLine("Iniciando partida...");
        }

        public override string ToString()
        {
            return "Partida ID " + id + " - " + fechaGuardado + " - DT: " + usuario.NombreUsuario;
        }
    }
}