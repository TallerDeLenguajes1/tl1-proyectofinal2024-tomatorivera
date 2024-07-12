namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena información del usuario
    /// </summary>
    public class Usuario
    {
        private string nombre;
        private float dinero;
        private Equipo equipo;
        
        public Usuario() {
            this.nombre = string.Empty;
            this.dinero = 0f;
            this.equipo = new Equipo();
        }

        /// <summary>
        /// Constructor de la clase Usuario
        /// </summary>
        /// <param name="nombre">Nombre de usuario</param>
        public Usuario(string nombre)
        {
            this.nombre = nombre;
            this.equipo = new Equipo();
            this.dinero = 0;
        }

        public Usuario(string nombre, Equipo equipo)
        {
            this.nombre = nombre;
            this.equipo = equipo;
            this.dinero = 0;
        }

        // Propiedades
        public string Nombre { get => nombre; set => nombre = value; }
        public float Dinero { get => dinero; set => dinero = value; } 
        public Equipo Equipo { get => equipo; set => equipo = value; }

        // Métodos

    }
}