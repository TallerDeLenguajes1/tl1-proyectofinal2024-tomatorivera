using Gui.Modelo;
using Gui.Vistas;

namespace Gui.Controladores
{
    /// <summary>
    /// Representa una clase controladora. Dichas clases
    /// se encargan de controlar el funcionamiento de una vista
    /// </summary>
    public abstract class Controlador
    {
        protected Vista vista;

        /// <summary>
        /// Constructor de una clase controladora
        /// </summary>
        /// <param name="vista">Instancia de la clase vista a controlar</param>
        public Controlador(Vista vista)
        {
            this.vista = vista;
        }
    }

    public class InicioControlador : Controlador
    {
        public InicioControlador(Vista vista) : base(vista)
        {
            MostrarEncabezado();
        }

        /// <summary>
        /// Muestra la vista de inicio y solicita al usuario su nombre de DT
        /// </summary>
        public void MostrarEncabezado()
        {
            ((Inicio) vista).Dibujar();

            // Controles pendientes: largo, null, vacio, clase que lo almacenará
            string? nombreUsuario = Console.ReadLine();
        }
    }

    public class MenuControlador : Controlador
    {

        private bool estaSeleccionando;
        private int indiceSeleccionado;

        public MenuControlador(Vista vista) : base(vista)
        {
            this.indiceSeleccionado = 0;
            this.estaSeleccionando = true;

            MostrarMenu();
        }

        /// <summary>
        /// Muestra lo necesario del menu y controla su funcionamiento
        /// </summary>
        public void MostrarMenu()
        {
            ConsoleKeyInfo teclaPresionada;
            Menu menu = (Menu) vista;
            menu.IndiceSeleccionado = this.indiceSeleccionado;
            menu.Dibujar();

            // El menú se mostrará mientras no se seleccione la opción Salir
            while (estaSeleccionando)
            {
                // Este while mantiene al usuario en el menú hasta que presione enter
                while ((teclaPresionada = Console.ReadKey(true)).Key != ConsoleKey.Enter)
                {
                    switch (teclaPresionada.Key)
                    {
                        case ConsoleKey.DownArrow:
                            if (indiceSeleccionado == (menu.Comandos.Count() - 1))
                                continue;
                            indiceSeleccionado++;
                            break;

                        case ConsoleKey.UpArrow:
                            if (indiceSeleccionado == 0) 
                                continue;
                            indiceSeleccionado--;
                            break;

                        default:
                            break;
                    }

                    // Actualizo el indice seleccionado y vuelvo a dibujar el menu
                    menu.IndiceSeleccionado = this.indiceSeleccionado;
                    menu.Dibujar();
                }

                // Ejecuto el comando seleccionado
                menu.Comandos.ElementAt(indiceSeleccionado).ejecutar();

                if (menu.Comandos.ElementAt(indiceSeleccionado) is ComandoSalir)
                {
                    this.estaSeleccionando = false;
                }
            }
        }

    }
}