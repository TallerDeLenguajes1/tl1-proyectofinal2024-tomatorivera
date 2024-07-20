using Logica.Acciones;

namespace Logica.Modelo
{
    /// <summary>
    /// Esta clase representa el resultado de una acción de partido
    /// </summary>
    public class ResultadoAccion
    {
        public Accion? AccionSiguiente { get; set; }
        public string MensajeAccion { get; set; }

        public ResultadoAccion(string MensajeAccion)
        {
            this.AccionSiguiente = null;
            this.MensajeAccion = MensajeAccion;
        }

        public ResultadoAccion(Accion AccionSiguiente, string MensajeAccion)
        {
            this.AccionSiguiente = AccionSiguiente;
            this.MensajeAccion = MensajeAccion;
        }
    }
}