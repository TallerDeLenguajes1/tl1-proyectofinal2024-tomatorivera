using Gui.Controladores;
using Gui.Modelo;
using Gui.Vistas;
using Logica.Comandos;
using Persistencia.Infraestructura;
using Spectre.Console;

namespace Logica
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Cargo la configuración general del juego
            Config.CargarConfiguracion();
            
            // Inicio el juego
            mostrarMenuPrincipal();
        }

        private static void mostrarMenuPrincipal()
        {
            // Configuro el UTF de la consola y la limpio para mostrar las vistas
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            AnsiConsole.Clear();

            // Muestro el titulo del juego
            var tituloInicio = new Inicio();
            Controlador<Inicio> tituloInicioControlador = new InicioControlador(tituloInicio);

            tituloInicioControlador.MostrarVista();

            // Muestro el menú principal
            var opcionesMenuPrincipal = new List<IComando>()
            {
                new ComandoNuevaPartida(),
                new ComandoCargarPartida(),
                new ComandoSalir(TipoMenu.PRINCIPAL)
            };

            var menuPrincipal = new Menu(opcionesMenuPrincipal);
            Controlador<Menu> menuPrincipalControlador = new MenuControlador(menuPrincipal);

            menuPrincipalControlador.MostrarVista();
        }
    }
}
