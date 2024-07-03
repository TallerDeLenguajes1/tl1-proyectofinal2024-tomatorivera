using Gui.Controladores;
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
            
            // Inicio el juego
            mostrarMenuPrincipal();
        }

        private static void mostrarMenuPrincipal()
        {
            // Muestro el titulo del juego
            Inicio tituloInicio = new Inicio();
            Controlador<Inicio> tituloInicioControlador = new InicioControlador(tituloInicio);

            tituloInicioControlador.MostrarVista();

            // Muestro el menú principal
            var opcionesMenuPrincipal = new List<IComando>()
            {
                new ComandoNuevaPartida(),
                new ComandoCargarPartida(),
                new ComandoSalir(TipoMenu.PRINCIPAL)
            };

            Menu menuPrincipal = new Menu(opcionesMenuPrincipal);
            Controlador<Menu> menuPrincipalControlador = new MenuControlador(menuPrincipal);

            menuPrincipalControlador.MostrarVista();
        }
    }
}
