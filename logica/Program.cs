using Gui.Modelo;
using Gui.Vistas;

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
            // Muestro el titulo del juego
            Vista encabezado = new Inicio();
            encabezado.Mostrar();
            
            // Muestro el menú principal
            List<IComando> comandos = new List<IComando>()
            {
                new prueba1(),
                new prueba2(),
                new ComandoSalir(TipoMenu.PRINCIPAL)
            };
            Vista menuPrincipal = new Menu(comandos);
            menuPrincipal.Mostrar();
        }
    }
}
