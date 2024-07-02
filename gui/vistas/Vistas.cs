using Gui.Controladores;
using Gui.Modelo;
using Gui.Util;
using Logica;

namespace Gui.Vistas
{
    /// <summary>
    /// Clase abstracta que representa una Vista
    /// </summary>
    public abstract class Vista
    {   
        /// <summary>
        /// Método encargado de mostrar por pantalla la vista en cuestión
        /// </summary>
        public abstract void Dibujar();

        /// <summary>
        /// Se encarga de mostrar la vista desde su respectivo controlador para poder
        /// separar la instancia del objeto vista de la acción de mostrarlo, y para
        /// que el usuario no tenga que preocuparse de la instancia de controladores
        /// </summary>
        public abstract void Mostrar();
    }

    /// <summary>
    /// Clase del tipo Vista que representa el encabezado de inicio
    /// </summary>
    public class Inicio : Vista
    {
        private InicioControlador controlador;

        /// <summary>
        /// Constructor de la vista de Inicio
        /// </summary>
        public Inicio()
        {
            // Instancio el controlador
            this.controlador = new InicioControlador(this);
        }

        public override void Dibujar()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;

            string logo = @"
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
                                        ██████████                                    ░░
                                    ██████▓▓▓▓▓▓▓▓████                                ░░
░░      ░░    ░░      ░░      ░░  ██▓▓██    ████▓▓▓▓▓▓██  ░░      ░░      ░░    ░░    ░░
        ░░      ░░    ░░      ░░████▓▓▓▓██      ████▓▓▓▓██  ░░    ░░      ░░      ░░  ░░
                                ██  ██▓▓▓▓██        ██▓▓██                            ░░
░░                            ██    ██▓▓▓▓▓▓████      ██▓▓██                          ░░
░░      ░░    ░░░░    ░░      ██      ████▓▓▓▓▓▓██      ████░░    ░░      ░░    ░░░░  ░░
        ░░      ░░    ░░      ██    ████▓▓██▓▓▓▓▓▓██    ████      ░░      ░░      ░░  ░░
                              ██  ██▓▓▓▓▓▓▓▓██▓▓▓▓▓▓████  ██                          ░░
                              ██  ██▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓██████                          ░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████▓▓▓▓▓▓██  ██▓▓▓▓██  ██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░      ░░      ░░    ░░      ░░██████▓▓██      ████  ████  ░░    ░░      ░░      ░░  ░░
                                  ████▓▓████        ████                                
░░                                  ████▓▓██      ████                                  
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ██████████      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░

";

            VistasUtil.MostrarCentrado(VistasUtil.ObtenerLineasSeparadas(logo));

            string titulo = @"
 _    __      ____           __          ____
| |  / /___  / / /__  __  __/ /_  ____ _/ / /
| | / / __ \/ / / _ \/ / / / __ \/ __ `/ / / 
| |/ / /_/ / / /  __/ /_/ / /_/ / /_/ / / /  
|___/\____/_/_/\___/\__, /_.___/\__,_/_/_/   
   /  |/  /___ ____/____/_ _____ ____  _____ 
  / /|_/ / __ `/ __ \/ __ `/ __ `/ _ \/ ___/ 
 / /  / / /_/ / / / / /_/ / /_/ /  __/ /     
/_/  /_/\__,_/_/ /_/\__,_/\__, /\___/_/      
                         /____/              
";
            VistasUtil.MostrarCentrado(VistasUtil.ObtenerLineasSeparadas(titulo));

            System.Console.WriteLine();
            VistasUtil.MostrarCentrado("* Presione una tecla para iniciar... *");
        }

        /// <summary>
        /// Solicita el nombre al usuario en la vista, es una "sub-vista" aislada
        /// del encabezado principal
        /// </summary>
        public void SolicitarNombre()
        {
            System.Console.WriteLine();
            VistasUtil.MostrarCentradoSinSalto("► Ingrese su nombre de DT: ");
        }

        public override void Mostrar()
        {
            controlador.MostrarEncabezado();
        }
    }

    /// <summary>
    /// Clase del tipo Vista que representa un Menú
    /// </summary>
    public class Menu : Vista
    {
        private MenuControlador controlador;
        private List<IComando> comandos;
        private int x;
        private int y;
        private int indiceSeleccionado;

        /// <summary>
        /// Constructor de la clase Menu
        /// </summary>
        /// <param name="comandos">Lista de comandos seleccionables en el menú</param>
        public Menu(List<IComando> comandos)
        {
            this.comandos = comandos;

            // Instancio el controlador
            this.controlador = new MenuControlador(this);
        }

        /* Propiedades */

        public List<IComando> Comandos { get => comandos; set => comandos = value; }
        public int IndiceSeleccionado { get => indiceSeleccionado; set => indiceSeleccionado = value; }

        /* Métodos */

        /// <summary>
        /// Muestra el título del menú separado de las opciones seleccionables
        /// </summary>
        public void mostrarTitulo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

            string lineasEncabezado = new string('─', Console.WindowWidth - 3);
            VistasUtil.MostrarCentrado("┌" + lineasEncabezado + "┐");
            VistasUtil.MostrarCentrado("¡Bienvenido " + controlador.ObtenerNombreUsuario() + "! - Juego desarrollado por: Tomas Rivera");
            VistasUtil.MostrarCentrado("└" + lineasEncabezado + "┘");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            
            string[] tituloMenu = {
                @"╭════════════ *-* ════════════╮",
                @"・Seleccione una opcion・",
                @"╰════════════ *-* ════════════╯"
            };

            VistasUtil.MostrarCentrado(tituloMenu);

            // Espacio en blanco para separar el título de las opciones
            System.Console.WriteLine("");
        }

        public override void Dibujar()
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;

            int indiceRecorriendo = 0;

            this.comandos.ForEach(comando => 
            {
                if (indiceRecorriendo == indiceSeleccionado)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    VistasUtil.MostrarCentrado("► " + comando.titulo + " ◄");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.CursorLeft = 0;
                    VistasUtil.MostrarCentrado(comando.titulo);
                }

                indiceRecorriendo++;
            });
        }

        public override void Mostrar()
        {
            // Oculto el cursor y muestro el titulo del menú
            Console.CursorVisible = false;
            mostrarTitulo();

            // Guardo las coordenadas de inicio del menú
            this.x = Console.CursorLeft;
            this.y = Console.CursorTop;

            // Muestro el menú
            controlador.MostrarMenu();
        }
    }
}