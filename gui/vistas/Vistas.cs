using Gui.Modelo;
using Gui.Util;
using Logica.Handlers;
using Spectre.Console;
using Spectre.Console.Rendering;

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
    }

    /// <summary>
    /// Clase del tipo Vista que representa el encabezado de inicio
    /// </summary>
    public class Inicio : Vista
    {
        public object? Logo { get; set; }

        public override void Dibujar()
        {
            string titulo = @" _    __      ____           __          ____
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

            // Creo un layout de tres filas en la vista del cmd
            var layout = new Layout("raiz")
                    .SplitRows(
                        new Layout("arriba"),
                        new Layout("centro"),
                        new Layout("abajo")
                    );

            // Coloco el titulo en el layout de arriba
            layout["arriba"].Update(
                new Panel(
                    Align.Center(
                        new Markup("[bold red]" + titulo + "[/]")
                    )
                )
                .Border(BoxBorder.Heavy)
                .BorderColor(Color.Red)
            );
            // Coloco la imagen renderizada en el layout del medio
            layout["centro"].Update(
                new Panel(
                    Align.Center(
                        // falta obtener la ruta mediante una capa de servicio
                        (Logo != null) ? (IRenderable) Logo : new Markup(""),
                        VerticalAlignment.Top
                    )
                )
                .Border(BoxBorder.None) 
                .Padding(new Padding(0, 2, 0, 0))
            );
            // Coloco el mensaje para continuar en el layout de abajo
            layout["abajo"].Update(
                new Panel(
                    Align.Center(
                        new Markup("[bold underline red]Presione una tecla para continuar...[/]"),
                        VerticalAlignment.Middle
                    )
                )
                .Border(BoxBorder.None)
                .Padding(new Padding(0,3))
            );

            // Configuro los tamaños de cada fila
            layout["arriba"].Ratio(1);
            layout["centro"].Ratio(3);
            layout["abajo"].Ratio(1);

            // Y muestro por pantalla el layout
            AnsiConsole.Write(layout);
        }

        /* Vieja vista de inicio
                public override void Dibujar()
                {
                    Console.CursorVisible = false;
                    Console.Clear();
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
        */
    }

    /// <summary>
    /// Clase del tipo Vista que representa un Menú
    /// </summary>
    public class Menu : Vista
    {
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
        }

        /* Propiedades */

        public List<IComando> Comandos { get => comandos; set => comandos = value; }
        public int IndiceSeleccionado { get => indiceSeleccionado; set => indiceSeleccionado = value; }

        /* Métodos */

        /// <summary>
        /// Muestra el título del menú separado de las opciones seleccionables
        /// </summary>
        public void MostrarTitulo()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            AnsiConsole.Write(
                new Panel(
                    Align.Center(
                        new Markup("[red]¡Bienvenido! - Juego desarrollado por: [/][bold red]Tomas Rivera[/]"),
                        VerticalAlignment.Middle
                    )
                )
                .Expand()
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Red)
            );

            //string lineasEncabezado = new string('─', Console.WindowWidth - 3);
            //VistasUtil.MostrarCentrado("┌" + lineasEncabezado + "┐");
            //VistasUtil.MostrarCentrado("¡Bienvenido! - Juego desarrollado por: Tomas Rivera");
            //VistasUtil.MostrarCentrado("└" + lineasEncabezado + "┘");

            Console.ResetColor();
            
            string[] tituloMenu = {
                @"-════════════ *-* ════════════-",
                @"> Seleccione una opcion <",
                @"-════════════ *-* ════════════-"
            };

            VistasUtil.MostrarCentrado(tituloMenu);

            // Espacio en blanco para separar el título de las opciones
            System.Console.WriteLine("");

            // Almaceno las coordenadas del cursor luego de mostrar el titulo para
            // mostrar las opciones del menú a partir de aquí
            this.x = Console.CursorLeft;
            this.y = Console.CursorTop;
        }

        public override void Dibujar()
        {
            Console.SetCursorPosition(x, y);

            int indiceRecorriendo = 0;

            this.comandos.ForEach(comando => 
            {
                if (indiceRecorriendo == indiceSeleccionado)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    VistasUtil.MostrarCentrado("► " + comando.titulo + " ◄");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(new string(' ', Console.WindowWidth - comando.titulo.Length));
                    Console.CursorLeft = 0;
                    VistasUtil.MostrarCentrado(comando.titulo);
                }

                indiceRecorriendo++;
            });
        }
    }
}