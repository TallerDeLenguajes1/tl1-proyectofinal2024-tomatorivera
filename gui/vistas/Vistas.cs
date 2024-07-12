using System.Text;
using Gui.Modelo;
using Gui.Util;
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
        private readonly Partida informacionPartida;
        private (LeagueResponse, List<GamesResponse>) informacionNovedades;

        public Dashboard(Partida informacionPartida)
        {
            this.informacionPartida = informacionPartida;
        }

        // Propiedades
        public (LeagueResponse, List<GamesResponse>) InformacionNovedades { get => informacionNovedades; set => informacionNovedades = value; }

        // Métodos
        public override void Dibujar()
        {
            // Layout del dashboard
            // -------------
            // |     1     |
            // |-----------|
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

            // Agrego el titulo al layout
            layout["arriba"].Update(obtenerTitulo());
            // Agrego el header al layout
            layout["centro"].Update(obtenerHeader()).Size(3);
            // Agrego la info del equipo al layout
            layout["abajo_izq"].Update(obtenerInformacionEquipo());
            // Muestro la informacion del usuario
            layout["abajo_der_arr"].Update(obtenerInformacionUsuario());
            // Muestro las nuevas novedades
            layout["abajo_der_aba"].Update(obtenerNovedades());

            /**** Muestro el dashboard ****/
            AnsiConsole.Write(layout);
        }

        private Panel obtenerTitulo()
        {
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

            return 
                new Panel(Align.Center(
                        filasCabecera,
                        VerticalAlignment.Middle
                    )
                )
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Yellow)
                .Expand();
        }

        private Panel obtenerHeader()
        {
            /******************
             * SECCION HEADER * 
             ******************/

            var mensajeHeader = $":volleyball: [orange1]¡Bienvenido DT [/][yellow]{informacionPartida.Usuario.Nombre}[/][orange1]! Suerte dirigiendo a [/][yellow]{informacionPartida.Usuario.Equipo.Nombre}[/] :volleyball:";

            return
                new Panel(Align.Center(
                        new Markup(mensajeHeader), 
                        VerticalAlignment.Middle
                    )
                )
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange3);
        }

        private Panel obtenerInformacionEquipo()
        {
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

            return 
                new Panel(Align.Left(filasInformacionEquipo))
                {
                    Header = new PanelHeader("[dim] [/][orange1 underline bold]Información del equipo[/][dim] [/]").LeftJustified()
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange3)
                .Expand();
        }
    
        private Panel obtenerInformacionUsuario()
        {
            /*********************
             * SECCIÓN HISTORIAL *
             *********************/

            var dineroClub = new Markup($"\n:small_orange_diamond: [orange1]Dinero actual:[/] [yellow]$ {informacionPartida.Usuario.Dinero}[/]");
            var arbolHistorial = new Tree(":small_orange_diamond: [orange1]Últimos partidos jugados:[/]")
            {
                Style = Style.Parse("orange3")
            };

            // Si el historial aún no contiene partidos, lo indico por pantalla
            if (!informacionPartida.Historial.HistorialPartidos.Any())
            {
                arbolHistorial.AddNode($"[red]{informacionPartida.Usuario.Equipo.Nombre} [/][orange3]aún no ha jugado partidos[/]");
            }
            else
            {
                // En esta tabla mostraré los últimos partidos del historial
                var tablaPartidos = new Table()
                {
                    Caption = new TableTitle("[italic grey](( Puede consultar el historial detallado más abajo ))[/]")
                }
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Orange3);

                tablaPartidos.AddColumn(new TableColumn(new Markup("[orange3]Partido[/]")).Centered());
                tablaPartidos.AddColumn(new TableColumn(new Markup("[orange3]Local[/]")).Centered());
                tablaPartidos.AddColumn(new TableColumn(new Markup("[orange3]Resultado[/]")).Centered());
                tablaPartidos.AddColumn(new TableColumn(new Markup("[orange3]Visitante[/]")).Centered());
    
                // Como máximo se muestran 8 partidos, si el historial tiene menos de 8, se muestran lo que hayan
                var columnas = new List<Markup>();
                var nPartidosMostrar = (informacionPartida.Historial.TotalPartidosJugados > 8) ? 8 : informacionPartida.Historial.TotalPartidosJugados;
                foreach (var partido in informacionPartida.Historial.HistorialPartidos.TakeLast(nPartidosMostrar)) 
                {
                    columnas.Add(new Markup($"[yellow]{partido.TipoPartido}[/]"));
                    columnas.Add(new Markup($"[yellow]{partido.Local}[/]"));
                    columnas.Add(new Markup($"[yellow]{partido.ScoreLocal}[/][gray] (L) - [/][yellow]{partido.ScoreVistante}[/] [gray](V)[/]"));
                    columnas.Add(new Markup($"[yellow]{partido.Visitante}[/]"));

                    tablaPartidos.AddRow(columnas);
                    columnas.Clear();
                }

                arbolHistorial.AddNode(tablaPartidos);
            }

            // Uso el componente Rows para mostrar el dinero arriba del historial
            var filasHistorial = new Rows(
                dineroClub,
                arbolHistorial
            );

            // Muestro la info anterior en el layout
            return
                new Panel(Align.Left(
                    filasHistorial,
                    VerticalAlignment.Top
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 underline bold]Información del jugador[/][dim] [/]").LeftJustified()
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange3)
                .Expand();
        }

        private Panel obtenerNovedades()
        {
            IRenderable novedades;

            if (informacionNovedades.Item1.Id == -1 ||
                !informacionNovedades.Item2.Any())
            {
                novedades = new Markup("\n:see_no_evil_monkey: [orange1]No hay nuevas novedades para mostrar[/]");
            }
            else
            {
                var competicion = informacionNovedades.Item1;

                var sb = new StringBuilder();
                var arbolPartidos = new Tree($"\n:collision: [orange1]¡Novedades de la competición [/][red]{competicion.Name}[/] {(!competicion.Country.Name.Equals("World") ? $"[orange1]de [/][red]{competicion.Country.Name} [/]" : string.Empty)}[orange4](TEMPORADA 23/24)[/][orange1]![/]")
                {
                    Style = Style.Parse("orange3")
                };

                // Muestro los ultimos 8 partidos de la competicion seleccionada, si tiene menos de 8, muestro los que tenga
                var nPartidosMostrar = (informacionNovedades.Item2.Count() <= 8) ? informacionNovedades.Item2.Count() : 8;
                foreach (var partido in informacionNovedades.Item2.TakeLast(nPartidosMostrar))
                {
                    // Según el estado del partido, muestro los datos correspondientes

                    /*
                     * Estados posibles de los partidos según datos de API Sports
                     *
                     * NS : Not Started
                     * S1 : Set 1 (In Play)
                     * S2 : Set 2 (In Play)
                     * S3 : Set 3 (In Play)
                     * S4 : Set 4 (In Play)
                     * S5 : Set 5 (In Play)
                     * AW : Awarded
                     * POST : Postponed
                     * CANC : Cancelled
                     * INTR : Interrupted
                     * ABD : Abandoned
                     * FT : Finished (Game Finished)
                     *
                     */
                    switch (partido.Status.Short)
                    {
                        case "NS":
                            string fecha = partido.Date.HasValue ? "(" + partido.Date.Value.ToString("dd/MM/yyyy hh:mm") + $" {partido.Timezone}" + ")" : string.Empty;
                            sb.Append($"[orange3]PROXIMAMENTE [/][grey]{fecha}[/][orange3]: [/][yellow]{partido.Teams.Home.Name} :vs_button: {partido.Teams.Away.Name}[/]");
                            break;
                        case "S1":
                        case "S2":
                        case "S3":
                        case "S4":
                        case "S5":
                            sb.Append($"[orange3]Se está disputando [/][yellow]{partido.Teams.Home.Name}[/] [orange1]({partido.Scores.Home})[/] :vs_button: [yellow]{partido.Teams.Away.Name}[/][orange1] ({partido.Scores.Away})[/][orange3] - {partido.Status.Long}[/]");
                            break;
                        case "POST":
                            sb.Append($"[orange1]¡Habrá que esperar![/][orange3] Se ha pospuesto el partido de [/][yellow]${partido.Teams.Home.Name} :vs_button: {partido.Teams.Away.Name}[/]");
                            break;
                        case "CANC":
                            sb.Append($"[yellow]¡PARTIDO CANCELADO![/][orange1] ${partido.Teams.Home.Name} :vs_button: {partido.Teams.Away.Name}[/]");
                            break;
                        case "INTR":
                            sb.Append($"[yellow]¡INTERRUMPIDO POR INCONVENIENTES![/][orange1] ${partido.Teams.Home.Name} :vs_button: {partido.Teams.Away.Name}[/]");
                            break;
                        case "ABD":
                            sb.Append($"[orange3]El partido entre [/][orange1]${partido.Teams.Home.Name} y {partido.Teams.Away.Name}[/][orange3] se ha abandonado por algún motivo[/]");
                            break;
                        case "FT":
                            sb.Append($"[orange3]FINALIZADO[/][grey]{(partido.Date.HasValue ? " (" + partido.Date.Value.ToString("dd/MM/yyyy") + ")" : string.Empty)}[/][orange3]: [/][yellow]{partido.Teams.Home.Name}[/]");
                            sb.Append((partido.Scores.Home > partido.Scores.Away) ? "[orange1] le ha ganado a [/]" : "[orange1] perdió contra [/]");
                            sb.Append($"[yellow]{partido.Teams.Away.Name}[/]");
                            sb.Append($"[orange1] por [/][yellow]{partido.Scores.Home} - {partido.Scores.Away}[/]");
                            break;
                    }

                    arbolPartidos.AddNode(new Markup(sb.ToString()));
                    sb.Clear();
                }

                novedades = arbolPartidos;
            }

            return 
                new Panel(Align.Left(
                    novedades,
                    VerticalAlignment.Top
                ))
                {
                    Header = new PanelHeader("[red] [/][orange1 bold underline]Noticias internacionales[/][red] [/]").LeftJustified()
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }
    }
}