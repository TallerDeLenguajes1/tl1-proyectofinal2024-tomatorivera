namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena información del usuario
    /// </summary>
    public class Usuario
    {
        private string nombreUsuario;
        private List<Jugador> equipo;

        /// <summary>
        /// Constructor de la clase Usuario
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        public Usuario(string nombreUsuario)
        {
            this.nombreUsuario = nombreUsuario;
            this.equipo = new List<Jugador>();
        }

        // Propiedades
        public string NombreUsuario { get => nombreUsuario; set => nombreUsuario = value; }
        public List<Jugador> Equipo { get => equipo; set => equipo = value; }

        // Métodos

    }
}