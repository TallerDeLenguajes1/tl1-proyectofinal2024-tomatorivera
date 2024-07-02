using Gui.Modelo;
using Gui.Vistas;
using Persistencia.Infraestructura;

namespace Logica
{
    internal class Program
    {
        /// <summary>
        /// Hilo de ejecución principal
        /// </summary>
        /// <param name="args">Argumentos de consola</param>
        private static void Main(string[] args)
        {
            // Cargo la configuración general del juego
            Config.CargarConfiguracion();
            
            // Muestro el titulo del juego
            Vista encabezado = new Inicio();
            encabezado.Mostrar();
            
            // Muestro el menú principal
            List<IComando> comandos = new List<IComando>()
            {
                new NuevaPartida(),
                new CargarPartida(),
                new ComandoSalir(TipoMenu.PRINCIPAL)
            };
            Vista menuPrincipal = new Menu(comandos);
            menuPrincipal.Mostrar();
        }
    }
}
