using Logica.Modelo;

namespace Logica.Acciones
{
    /// <summary>
    /// Clase que representa una acción específica de un partido
    /// </summary>
    public abstract class Accion
    {
        /// <summary>
        /// Realiza la acción
        /// </summary>
        /// <returns>Objeto <c>ResultadoAccion</c> con datos del resultado de la accion que determinarán cómo continúa el partido</returns>
        public abstract ResultadoAccion realizar();
    }

    public class Saque : Accion
    {
        public override ResultadoAccion realizar()
        {
            throw new NotImplementedException();
        }
    }
}