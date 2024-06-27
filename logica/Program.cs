using Gui.Controladores;
using Gui.Modelo;
using Gui.Vistas;

namespace Logica
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Vista encabezado = new Inicio();
            
            List<IComando> comandos = new List<IComando>()
            {
                new prueba1(),
                new prueba2(),
                new ComandoSalir(TipoMenu.PRINCIPAL)
            };

            Vista menuPrincipal = new Menu(comandos);
        }
    }
}
