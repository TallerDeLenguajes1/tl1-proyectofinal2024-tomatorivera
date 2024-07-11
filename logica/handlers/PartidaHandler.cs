using Gui.Controladores;
using Gui.Vistas;
using Logica.Modelo;

namespace Logica.Handlers
{
    public class PartidaHandler
    {
        private Partida partidaActual;

        public PartidaHandler(Partida partidaActual)
        {
            this.partidaActual = partidaActual;
        }
        
        /// <summary>
        /// Inicia la lógica de una partida
        /// </summary>
        public void IniciarPartida() {
            // Falta implementar la lógica
            
            // Muestro el dashboard
            var vistaDashboard = new Dashboard(partidaActual);
            var controladorDashboard = new DashboardControlador(vistaDashboard);

            controladorDashboard.MostrarVista();
        }
    }
}