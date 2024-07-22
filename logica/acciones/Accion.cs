using Logica.Modelo;

namespace Logica.Acciones
{
    /// <summary>
    /// Clase que representa una acción específica de un partido
    /// </summary>
    public abstract class Accion
    {
        protected Rally rally;
        protected Jugador realizador;
        protected Random rnd;

        protected Accion(Rally rally)
        {
            this.rally = rally;
            this.realizador = rally.JugadorActual;
            this.rnd = new Random();
        }

        /// <summary>
        /// Realiza la acción
        /// </summary>
        /// <returns>Objeto <c>ResultadoAccion</c> con datos del resultado de la accion que determinarán cómo continúa el partido</returns>
        public abstract ResultadoAccion Realizar();
    }
}