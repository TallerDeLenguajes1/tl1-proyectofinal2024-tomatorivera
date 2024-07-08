namespace Logica.Modelo
{
    /// <summary>
    /// Clase modelo que almacena información del usuario
    /// </summary>
    public class Usuario
    {
        private string nombre;
        private Equipo equipo;
        private float puntos;

        /// <summary>
        /// Constructor de la clase Usuario
        /// </summary>
        /// <param name="nombre">Nombre de usuario</param>
        public Usuario(string nombre)
        {
            this.nombre = nombre;
            this.equipo = new Equipo();
            this.puntos = 0;
        }

        public Usuario(string nombre, Equipo equipo)
        {
            this.nombre = nombre;
            this.equipo = equipo;
            this.puntos = 0;
        }

        // Propiedades
        public string Nombre { get => nombre; set => nombre = value; }
        public Equipo Equipo { get => equipo; set => equipo = value; }
        public float Puntos { get => puntos; set => puntos = value; } 

        // Métodos

    }
}