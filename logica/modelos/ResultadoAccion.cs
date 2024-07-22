using Logica.Acciones;

namespace Logica.Modelo
{
    /// <summary>
    /// Esta clase representa el resultado de una acción de partido
    /// </summary>
    public class ResultadoAccion
    {
        public Accion? AccionSiguiente { get; }
        public string MensajeAccion { get; }
        public Jugador Realizador { get; }

        public ResultadoAccion(string MensajeAccion, Jugador Realizador)
        {
            this.AccionSiguiente = null;
            this.MensajeAccion = MensajeAccion;
            this.Realizador = Realizador;
        }

        public ResultadoAccion(Accion AccionSiguiente, string MensajeAccion, Jugador Realizador)
        {
            this.AccionSiguiente = AccionSiguiente;
            this.MensajeAccion = MensajeAccion;
            this.Realizador = Realizador;
        }
    }
}