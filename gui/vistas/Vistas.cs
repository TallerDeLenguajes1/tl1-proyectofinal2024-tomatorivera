using Gui.Modelo;
using Gui.Util;
using Logica.Handlers;
using Logica.Modelo;
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
                        new Layout("abajo")
                            .SplitColumns(
                                new Layout("abajo_izq"),
                                new Layout("abajo_der")
                            )
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
            // Coloco la imagen renderizada en el layout a la izquierda
            layout["abajo_izq"].Update(
                new Panel(
                    Align.Center(
                        // falta obtener la ruta mediante una capa de servicio
                        (Logo != null) ? (IRenderable) Logo : new Markup("[red strikethrough]---[/] [red underline bold] VBM [/][red] * By: [/][bold red reverse]Ramiro Tomas Rivera Octtaviano[/] [red strikethrough]---[/]"),
                        VerticalAlignment.Middle
                    )
                )
                .Expand()
                .Border(BoxBorder.None) 
            );
            // Coloco el mensaje para continuar en el layout de abajo a la derecha
            layout["abajo_der"].Update(
                new Panel(
                    Align.Center(
                        new Rows(
                            new Markup("[red strikethrough]---[/] [red underline bold] VBM [/][red] * By: [/][bold red reverse]Ramiro Tomas Rivera Octtaviano[/] [red strikethrough]---[/]"),
                            new Text(""),
                            new Markup("[italic underline red]Presione una tecla para iniciar...[/]")
                        ),
                        VerticalAlignment.Middle
                    )
                )
                .Border(BoxBorder.None)
                .Expand()
            );

            // Configuro los tamaños de cada fila
            layout["arriba"].Ratio(1);
            layout["abajo"].Ratio(3);

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

    /// <summary>
    /// Clase Vista que representa un dashboard con información
    /// </summary>
    public class Dashboard : Vista
    {
        private Partida informacionPartida;

        public Dashboard(Partida informacionPartida)
        {
            this.informacionPartida = informacionPartida;
        }

        public override void Dibujar()
        {
            // Limpio la consola
            AnsiConsole.Clear();
            
            // Layout del dashboard
            // -------------
            // |     1     |
            // -------------
            // |     |  3  |
            // |  2  |-----|
            // |     |  4  |
            // -------------

            var layout = new Layout("raiz")
                .SplitRows(
                    new Layout("arriba"),
                    new Layout("centro"),
                    new Layout("abajo")
                        .SplitColumns(
                            new Layout("abajo_izq"),
                            new Layout("abajo_der")
                                .SplitRows(
                                    new Layout("abajo_der_arr"),
                                    new Layout("abajo_der_aba")
                                )
                        )
                );

            layout["arriba"].Ratio = 1;
            layout["centro"].Ratio = 1;
            layout["abajo"].Ratio = 3;

            /******************
             * SECCION TITULO *
             ******************/

            var titulo = @"
██╗   ██╗██████╗ ███╗   ███╗
██║   ██║██╔══██╗████╗ ████║
██║   ██║██████╔╝██╔████╔██║
╚██╗ ██╔╝██╔══██╗██║╚██╔╝██║
 ╚████╔╝ ██████╔╝██║ ╚═╝ ██║
  ╚═══╝  ╚═════╝ ╚═╝     ╚═╝
";
            var subtitulo = ":backhand_index_pointing_right: [yellow]By:[/][bold yellow1] Ramiro Tomas Rivera Octtaviano[/] :backhand_index_pointing_left:";

            // Uso el componente Rows para mostrar el titulo justo arriba del subtitulo
            var filasCabecera = new Rows(
                Align.Center(new Markup($"[yellow]{titulo}[/]")),
                Align.Center(new Markup(subtitulo))
            );
            
            // Agrego y alineo las filas layout
            layout["arriba"].Update(
                new Panel(Align.Center(
                        filasCabecera,
                        VerticalAlignment.Middle
                    )
                )
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Yellow)
                .Expand()
            );

            /******************
             * SECCION HEADER * 
             ******************/

            var mensajeHeader = $":volleyball: [orange1]¡Bienvenido DT [/][yellow]{informacionPartida.Usuario.Nombre}[/][orange1]! Suerte dirigiendo a [/][yellow]{informacionPartida.Usuario.Equipo.Nombre}[/] :volleyball:";

            // Muestro el mensaje del header centrado en el layout
            layout["centro"].Update(
                new Panel(Align.Center(
                        new Markup(mensajeHeader), 
                        VerticalAlignment.Middle
                    )
                )
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange3)
            ).Size(3);

            /************************
             * SECCION INFO. EQUIPO *
             ************************/

            // Información del equipo del jugador
            var nombreEquipoTxt = new Markup($"\n:small_orange_diamond: [orange1]Nombre del equipo: [/][yellow]{informacionPartida.Usuario.Equipo.Nombre}[/]");
            var nJugadoresTxt = new Markup($":small_orange_diamond: [orange1]Cantidad de jugadores actuales: [/][yellow]{informacionPartida.Usuario.Equipo.TotalJugadores}[/]");

            // Arbol que desplegará la información de la plantilla indexada a una tabla
            var arbolPlantilla = new Tree(":small_orange_diamond: [orange1]Plantilla actual del equipo:[/]")
            {
                Style = Style.Parse("orange3")
            };

            // En esta tabla voy a mostrar los datos de los jugadores
            var tablaJugadores = new Table()
            {
                Caption = new TableTitle("[grey italic](( Puede consultar su plantilla detallada más abajo ))[/]")
            }
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Orange3);

            tablaJugadores.AddColumn(new TableColumn(new Markup("[orange3]Nombre[/]")).Centered());
            tablaJugadores.AddColumn(new TableColumn(new Markup("[orange3]Nro. :t_shirt:[/]")).Centered());
            tablaJugadores.AddColumn(new TableColumn(new Markup("[orange3]Posición de juego :volleyball:[/]")).Centered());
            tablaJugadores.AddColumn(new TableColumn(new Markup("[orange3]Experiencia :timer_clock:[/]")).Centered());

            var columnas = new List<Markup>();
            var nJugadoresMostrar = informacionPartida.Usuario.Equipo.Jugadores.Count();
            var equipoJugador = informacionPartida.Usuario.Equipo.Jugadores;

            for (int i=0 ; i<nJugadoresMostrar ; i++)
            {
                columnas.Add(new Markup($"[yellow]{equipoJugador[i].Nombre}[/]"));
                columnas.Add(new Markup($"[yellow]{equipoJugador[i].NumeroCamiseta}[/]"));
                columnas.Add(new Markup($"[yellow]{equipoJugador[i].TipoJugador}[/]"));
                columnas.Add(new Markup($"[yellow]{equipoJugador[i].Experiencia} pts.[/]"));

                tablaJugadores.AddRow(columnas);
                columnas.Clear();
            }
            arbolPlantilla.AddNode(tablaJugadores);

            // Uso el componente rows para mostrar la información del equipo una arriba de otra
            var filasInformacionEquipo = new Rows(
                nombreEquipoTxt,
                nJugadoresTxt,
                arbolPlantilla
            );

            // Muestro la info anterior en el layout del dashboard
            layout["abajo_izq"].Update(
                new Panel(Align.Left(filasInformacionEquipo))
                {
                    Header = new PanelHeader("[dim] [/][orange1 underline bold]Información del equipo[/][dim] [/]").LeftJustified()
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange3)
                .Expand()
            );

            /**** Muestro el dashboard ****/
            AnsiConsole.Write(layout);
        }
    }
}