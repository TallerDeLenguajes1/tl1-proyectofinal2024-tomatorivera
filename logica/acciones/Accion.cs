using Logica.Modelo;
using Logica.Util;

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
            realizador = rally.JugadorActual;
            rnd = new Random();
        }

        /// <summary>
        /// Realiza la acción
        /// </summary>
        /// <returns>Objeto <c>ResultadoAccion</c> con datos del resultado de la accion que determinarán cómo continúa el partido</returns>
        public abstract ResultadoAccion Realizar();

        /// <summary>
        /// Realiza un incremento del cansancio en el realizador de la acción
        /// </summary>
        protected void IncrementarCansancio(float min, float max)
        {
            realizador.AumentarCansancio(ProbabilidadesUtil.ValorAleatorioEntre(min, max));
        }
    }
}