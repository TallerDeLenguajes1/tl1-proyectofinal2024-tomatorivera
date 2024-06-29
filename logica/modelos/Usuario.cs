namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena información del jugador
    /// </summary>
    public class Usuario
    {
        private string nombreUsuario;

        /// <summary>
        /// Constructor de la clase Usuario
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        public Usuario(string nombreUsuario)
        {
            this.nombreUsuario = nombreUsuario;
        }

        // Propiedades
        public string? NombreUsuario { get => nombreUsuario; set => nombreUsuario = value; }

        // Métodos

    }
}