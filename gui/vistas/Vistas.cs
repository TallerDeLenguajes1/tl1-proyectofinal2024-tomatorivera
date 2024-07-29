using System.Text;
using Gui.Util;
using Logica.Comandos;
using Logica.Modelo;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Gui.Vistas;

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
        Console.CursorVisible = false;
        AnsiConsole.Write(layout);
    }
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
                VistasUtil.MostrarCentrado("► " + comando.Titulo + " ◄");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.Write(new string(' ', Console.WindowWidth - comando.Titulo.Length));
                Console.CursorLeft = 0;
                VistasUtil.MostrarCentrado(comando.Titulo);
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

    /// <summary>
    /// Dibuja el titulo del juego para el layout
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
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

    /// <summary>
    /// Dibuja un encabezado con una bienvenida al jugador
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
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

    /// <summary>
    /// Dibuja información del equipo y sus jugadores
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
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

    /// <summary>
    /// Dibuja la información del usuario y su historial de partidos
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
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
                columnas.Add(new Markup($"[yellow]{partido.Local.Nombre}[/]"));
                columnas.Add(new Markup($"[yellow]{partido.ScoreLocal}[/][gray] (L) - [/][yellow]{partido.ScoreVisitante}[/] [gray](V)[/]"));
                columnas.Add(new Markup($"[yellow]{partido.Visitante.Nombre}[/]"));

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

    /// <summary>
    /// Dibuja las últimas novedades del mundo del voleyball
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
    private Panel obtenerNovedades()
    {
        IRenderable novedades;

        if (informacionNovedades.Item1 == null ||
            informacionNovedades.Item2 == null ||
            informacionNovedades.Item1.Id == -1 ||
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
                    case "AW":
                        sb.Append($"[orange3]Partido galardonado[/] :face_screaming_in_fear: :1st_place_medal: [gray]-[/] [orange1] ${partido.Teams.Home.Name} :vs_button: {partido.Teams.Away.Name}[/]");
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

/// <summary>
/// Clase Vista que se encarga de mostrar los datos de un partido por pantalla
/// </summary>
public class PanelPartido : Vista
{
    private const string nombrePanelAcciones = "info_acciones";

    public Partido? InformacionPartido { get; set; }
    public Layout LayoutInformacion { get; set; }

    public PanelPartido()
    {
        LayoutInformacion = generarLayout();
    }

    // Métodos
    public override void Dibujar()
    {
        AnsiConsole.Clear();

        LayoutInformacion["info_partido"].Update(dibujarInformacionPartido());
        LayoutInformacion["info_equipos"].Update(dibujarInformacionEquipos());
        LayoutInformacion["info_adicional"].Update(dibujarInformacionAdicional());
        LayoutInformacion[nombrePanelAcciones].Update(dibujarAcciones());

        //AnsiConsole.Write(LayoutInformacion);
    }

    /// <summary>
    /// Genera el layout sobre el cual se desplegará la información
    /// </summary>
    /// <returns>Objeto <c>Layout</c></returns>
    private Layout generarLayout()
    {
        var layout = new Layout("raiz")
                        .SplitColumns(
                            new Layout("info_columna")
                                .SplitRows(
                                    new Layout("info_fila")
                                        .SplitColumns(
                                            new Layout("info_partido"),
                                            new Layout(nombrePanelAcciones)
                                        ),
                                    new Layout("info_adicional")
                                ),
                            new Layout("info_equipos")
                        );
        
        layout["info_columna"].Ratio(2);
        layout["info_equipos"].Ratio(1);

        layout["info_fila"].Ratio(2);
        layout["info_adicional"].Ratio(1);

        return layout;
    }

    /// <summary>
    /// Dibuja el panel con información del partido
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
    private Panel dibujarInformacionPartido()
    {
        // Si por alguna razón la información del partido es nula, no muestro nada por pantalla
        if (InformacionPartido == null)
        {
            return 
                new Panel(Align.Left(
                    new Markup(":warning: [red]Ha ocurrido algún error obteniendo la información[/]")
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 bold underline]Información del partido[/][dim] [/]")
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }
        else
        {
            var tablaResultado = new Table()
            {
                Title = new TableTitle($"[red]{InformacionPartido.Local.Nombre} :vs_button: {InformacionPartido.Visitante.Nombre}[/]")
            }
            .Border(TableBorder.Square)
            .BorderColor(Color.Orange1)
            .ShowRowSeparators()
            .Centered();

            tablaResultado.AddColumn(new TableColumn(new Markup(":t_shirt: [orange1]Equipo[/]")).Centered());
            tablaResultado.AddColumn(new TableColumn(new Markup(":star: [orange1]Sets[/]")).Centered());
            tablaResultado.AddColumn(new TableColumn(new Markup(":volleyball: [orange1]Puntaje[/]")).Centered());

            tablaResultado.AddRow($"[tan]{InformacionPartido.Local.Nombre}[/]", $"[tan]{InformacionPartido.ScoreLocal}[/]", $"[tan]{InformacionPartido.SetActual.Resultado.PuntosLocal}[/]");
            tablaResultado.AddRow($"[tan]{InformacionPartido.Visitante.Nombre}[/]", $"[tan]{InformacionPartido.ScoreVisitante}[/]", $"[tan]{InformacionPartido.SetActual.Resultado.PuntosVisitante}[/]");
            
            var filasInformacion = new Rows(
                new Text(""), // separador
                tablaResultado,
                new Text(""), // separador
                new Markup($":backhand_index_pointing_right: [tan]Set actual:[/] [yellow bold]{InformacionPartido.SetActual.NumeroSet}[/]"),
                new Markup($":backhand_index_pointing_right: [tan]Sets a jugar:[/] [yellow bold]{InformacionPartido.SetMaximos}[/]"),
                new Markup($":backhand_index_pointing_right: [tan]Posesión del saque:[/] [yellow bold]{InformacionPartido.EquipoEnSaque.Nombre}[/]"),
                new Markup($":backhand_index_pointing_right: [tan]Tipo de partido:[/] [yellow bold]{InformacionPartido.TipoPartido}[/]"),
                new Text(""), // separador
                new Markup($"[yellow]▮ Acciones de {InformacionPartido.Local.Nombre}[/]"),
                new Markup($"[red]▮ Acciones de {InformacionPartido.Visitante.Nombre}[/]")
            );

            return
                new Panel(Align.Left(
                    filasInformacion
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 bold underline]Información del partido[/][dim] [/]")
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }
    }

    /// <summary>
    /// Dibuja el panel con información de los equipos
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
    private Panel dibujarInformacionEquipos()
    {
        // Si por alguna razón la información del partido es nula, no muestro detalles
        if (InformacionPartido == null)
        {
            return 
                new Panel(Align.Left(
                    new Markup(":warning: [red]Ha ocurrido algún error obteniendo la información[/]")
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 bold underline]Información de los equipos[/][dim] [/]")
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }
        else
        {
            /******************
                * EQUIPO JUGADOR *
                *****************/
            var equipoJugadorSeparador = new Rule("[orange1]Su equipo[/]");
            equipoJugadorSeparador.RuleStyle(Style.Parse("gray bold"));
            equipoJugadorSeparador.LeftJustified();

            var arbolSuplentes = new Tree("\n:small_orange_diamond: [orange3]Jugadores suplentes:[/]")
            {
                Style = Style.Parse("orange3")
            };
            var formacionJugador = InformacionPartido.ObtenerEquipoJugador().FormacionPartido;
            if (formacionJugador == null)
            {
                arbolSuplentes.AddNode(":warning: [red]Ha ocurrido un error obteniendo los suplentes[/]");
            }
            else
            {
                foreach (var jugador in formacionJugador.JugadoresSuplentes)
                {
                    var nodoJugador = arbolSuplentes.AddNode(
                        $"[yellow]{jugador.Nombre}[/] [orange3]({jugador.NumeroCamiseta})[/][gray] - [/][orange1]{jugador.TipoJugador}[/][gray] - [/][orange1]Cansancio: [/][orange3]{jugador.Cansancio}[/]"
                    );

                    nodoJugador.AddNode(
                        $"[gray]SAQ:[/] {jugador.HabilidadSaque} [gray]REM:[/] {jugador.HabilidadRemate} [gray]REC:[/] {jugador.HabilidadRecepcion} [gray]COL:[/] {jugador.HabilidadColocacion} [gray]BLO:[/] {jugador.HabilidadBloqueo}"
                    );
                }
            }

            /******************
                * EQUIPO CONSOLA *
                *****************/
            var equipoConsolaSeparador = new Rule("[orange1]Equipo rival[/]");
            equipoConsolaSeparador.RuleStyle(Style.Parse("gray bold"));
            equipoConsolaSeparador.LeftJustified();

            var arbolTitulares = new Tree("\n:small_orange_diamond: [orange3]Jugadores en cancha:[/]")
            {
                Style = Style.Parse("orange3")
            };
            var formacionConsola = InformacionPartido.ObtenerEquipoConsola().FormacionPartido;
            if (formacionConsola == null)
            {
                arbolTitulares.AddNode(":warning: [red]Ha ocurrido un error obteniendo los titulares[/]");
            }
            else
            {
                for (int i=0 ; i<formacionConsola.JugadoresCancha.Count() ; i++)
                {
                    var jugador = formacionConsola.JugadoresCancha.ElementAt(i);

                    var nodoJugador = arbolTitulares.AddNode(
                        $"[red]ZONA {i+1}:[/] [yellow]{jugador.Nombre}[/] [orange3]({jugador.NumeroCamiseta})[/]"
                    );
                    nodoJugador.AddNode(
                        $"[gray]SAQ:[/] {jugador.HabilidadSaque} [gray]REM:[/] {jugador.HabilidadRemate} [gray]REC:[/] {jugador.HabilidadRecepcion} [gray]COL:[/] {jugador.HabilidadColocacion} [gray]BLO:[/] {jugador.HabilidadBloqueo}"
                    );
                }
            }

            // Junto las filas con el componente Rows
            var filasInformacion = new Rows(
                new Text(""), // separador
                equipoJugadorSeparador,
                arbolSuplentes,
                new Text(""), // separador
                equipoConsolaSeparador,
                arbolTitulares
            );

            // Retorno el panel con la información
            return
                new Panel(Align.Left(
                    filasInformacion
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 bold underline]Información de los equipos[/][dim] [/]")
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }
    }

    /// <summary>
    /// Dibuja el panel con información adicional del partido, como
    /// las formaciones de los equipos y quién realiza un punto
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
    private Panel dibujarInformacionAdicional()
    {
        // Si por alguna razón la información del partido es nula, no muestro detalles
        if (InformacionPartido == null)
        {
            return 
                new Panel(Align.Left(
                    new Markup(":warning: [red]Ha ocurrido algún error obteniendo la información[/]")
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 bold underline]Estado del partido[/][dim] [/]")
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }
        else
        {
            // Armo las formaciones en una matriz
            var formacionLocal = InformacionPartido.Local.FormacionPartido!;
            Markup[,] locales = 
            {
                {
                    new Markup($"\n[orange1]ZONA 5[/]\n[yellow]{formacionLocal.ObtenerJugadorZona(5).Nombre}[/]"), 
                    new Markup($"\n[orange1]ZONA 4[/]\n[yellow]{formacionLocal.ObtenerJugadorZona(4).Nombre}[/]")
                },{
                    new Markup($"\n[orange1]ZONA 6[/]\n[yellow]{formacionLocal.ObtenerJugadorZona(6).Nombre}[/]"),
                    new Markup($"\n[orange1]ZONA 3[/]\n[yellow]{formacionLocal.ObtenerJugadorZona(3).Nombre}[/]")
                },{
                    new Markup($"\n[orange1]ZONA 1[/]\n[yellow]{formacionLocal.ObtenerJugadorZona(1).Nombre}[/]\n"),
                    new Markup($"\n[orange1]ZONA 2[/]\n[yellow]{formacionLocal.ObtenerJugadorZona(2).Nombre}[/]\n")
                }
            };

            var formacionVisitante = InformacionPartido.Visitante.FormacionPartido!;
            Markup[,] visitantes = 
            {
                {
                    new Markup($"\n[red]ZONA 2[/]\n[red1]{formacionVisitante.ObtenerJugadorZona(2).Nombre}[/]"),
                    new Markup($"\n[red]ZONA 1[/]\n[red1]{formacionVisitante.ObtenerJugadorZona(1).Nombre}[/]")
                },{
                    new Markup($"\n[red]ZONA 3[/]\n[red1]{formacionVisitante.ObtenerJugadorZona(3).Nombre}[/]"),
                    new Markup($"\n[red]ZONA 6[/]\n[red1]{formacionVisitante.ObtenerJugadorZona(6).Nombre}[/]")
                },{
                    new Markup($"\n[red]ZONA 4[/]\n[red1]{formacionVisitante.ObtenerJugadorZona(4).Nombre}[/]\n"),
                    new Markup($"\n[red]ZONA 5[/]\n[red1]{formacionVisitante.ObtenerJugadorZona(5).Nombre}[/]\n")
                }
            };

            var tablaFormaciones = new Table()
            {
                Title = new TableTitle("[tan](( Formacion actual de los equipos ))[/]")
            };
            tablaFormaciones.AddColumn(new TableColumn("local").Centered());
            tablaFormaciones.AddColumn(new TableColumn("visitante").Centered());
            tablaFormaciones.HideHeaders();
            tablaFormaciones.Border(TableBorder.Heavy);
            tablaFormaciones.BorderColor(Color.LightSkyBlue1);
            
            // Genero dos tablas para almacenar las formaciones, una de local y otra para visitantes
            var tablaLocal = new Table();
            tablaLocal.AddColumn(new TableColumn("d").Centered());
            tablaLocal.AddColumn(new TableColumn("o").Centered());
            tablaLocal.HideHeaders();
            tablaLocal.Border(TableBorder.Minimal);
            tablaLocal.BorderColor(Color.DarkSlateGray1);
            tablaLocal.AddRow(locales[0,0], locales[0,1]);
            tablaLocal.AddRow(locales[1,0], locales[1,1]);
            tablaLocal.AddRow(locales[2,0], locales[2,1]);

            var tablaVisitante = new Table();
            tablaVisitante.AddColumn(new TableColumn("o").Centered());
            tablaVisitante.AddColumn(new TableColumn("d").Centered());
            tablaVisitante.HideHeaders();
            tablaVisitante.Border(TableBorder.Minimal);
            tablaVisitante.BorderColor(Color.DarkSlateGray1);
            tablaVisitante.AddRow(visitantes[0,0], visitantes[0,1]);
            tablaVisitante.AddRow(visitantes[1,0], visitantes[1,1]);
            tablaVisitante.AddRow(visitantes[2,0], visitantes[2,1]);

            // Las dos tablas se muestran dentro de otra tabla para dar la forma de la cancha
            tablaFormaciones.AddRow(tablaLocal, tablaVisitante);

            return
                new Panel(Align.Center(
                    tablaFormaciones,
                    VerticalAlignment.Middle
                ))
                {
                    Header = new PanelHeader("[dim] [/][orange1 bold underline]Estado del partido[/][dim] [/]")
                }
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Orange1)
                .Expand();
        }

    }

    /// <summary>
    /// Dibuja el panel con la información de las acciones de un partido
    /// </summary>
    /// <returns>Objeto <c>Panel</c></returns>
    private Panel dibujarAcciones(string accion = "")
    {
        return
            new Panel(Align.Center(
                new Markup(accion)
            ))
            {
                Header = new PanelHeader("[dim] [/][orange1 bold underline]Desarrollo del partido[/][dim] [/]")
            }
            .Expand()
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Orange1);
    }

    /// <summary>
    /// Actualiza el panel de acciones de modo que se vayan mostrando todas las acciones
    /// una debajo de la otra con un delay de <paramref name="segundosDelay"/>
    /// </summary>
    /// <param name="ctx">Contexto de la actualización del panel en tiempo real</param>
    /// <param name="acciones">Lista de acciones a mostrar</param>
    /// <param name="segundosDelay">Segundos de delay entre la aparición de cada acción en pantalla (por defecto, 0 segs)</param>
    public void ActualizarAcciones(LiveDisplayContext ctx, List<string> acciones, int segundosDelay = 0)
    {
        int maxLineasVentana = 20 /*Console.WindowHeight / 3 * 2 - 8*/;

        // En esta variable acumulo las acciones que van mostrandose para realizar la animación
        // de que lo que pasa va dibujandose una línea debajo de otra
        string accionesSucedidas = string.Empty;

        foreach (var accion in acciones)
        {
            // Controlo si las lineas de las acciones superan la cantidad máxima de líneas para la ventana.
            // De ser así, borro las primeras líneas para no desbordar
            if (accionesSucedidas.Split('\n', StringSplitOptions.None).Length >= maxLineasVentana)
            {
                var lineas = accionesSucedidas.Split('\n').ToList();
                lineas.RemoveAt(1);
                if (!lineas[1].Contains("►")) lineas.RemoveAt(1);

                accionesSucedidas = string.Join('\n', lineas);
            }

            accionesSucedidas += $"\n{accion}";
            LayoutInformacion[nombrePanelAcciones].Update(dibujarAcciones(accionesSucedidas));

            VistasUtil.PausarVistas(segundosDelay);
            ctx.Refresh();
        }
    }

    /// <summary>
    /// Actualiza el panel de 'Estado del partido' para mostrar qué equipo ha marcado un punto
    /// </summary>
    /// <param name="ctx">Contexto de la actualización del panel en tiempo real</param>
    /// <param name="equipo">Equipo que ha marcado el punto (Local o Visitante)</param>
    public void MarcarPunto(LiveDisplayContext ctx, TipoEquipo equipo)
    {
        (Color figlet, Color subtitulo) color = (equipo == TipoEquipo.LOCAL) ? (Color.Orange1, Color.Yellow) : (Color.Red, Color.Red1);
        var figletPunto = @"   _ ___  __  ___  ____________  __
(_) _ \/ / / / |/ /_  __/ __ \/ /
/ / ___/ /_/ /    / / / / /_/ /_/ 
/_/_/   \____/_/|_/ /_/  \____(_)  ";
        var subtitulo = (equipo == TipoEquipo.LOCAL) ? InformacionPartido!.Local.Nombre : InformacionPartido!.Visitante.Nombre;

        var filas = new Rows(
            new Markup($"[{color.figlet.ToMarkup()}]{figletPunto}[/]"),
            new Text(""), // separador
            new Markup($"[{color.figlet.ToMarkup()}]Punto para el equipo [/][{color.subtitulo.ToMarkup()}]{subtitulo}[/]")
        );

        LayoutInformacion["info_adicional"].Update(
            new Panel(Align.Center(
                filas,
                VerticalAlignment.Middle
            ))
            {
                Header = new PanelHeader("[dim] [/][orange1 bold underline]Estado del partido[/][dim] [/]")
            }
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Orange1)
            .Expand()
        );

        ctx.Refresh();
    }

    /// <summary>
    /// Actualiza el panel 'Información del partido' para actualizar el marcador
    /// </summary>
    public void ActualizarMarcador()
    {
        LayoutInformacion["info_partido"].Update(dibujarInformacionPartido());
    }

    /// <summary>
    /// Actualiza el layout de información para únicamente desplegar una pantalla final en
    /// la que se le indica al usuario si ha ganado o ha perdido
    /// </summary>
    /// <param name="nombreEquipoJugador">Nombre del equipo del jugador</param>
    /// <param name="tipoEquipoJugador">Tipo de equipo del jugador (Local o visitante)</param>
    /// <param name="nombreEquipoRival">Nombre del equipo rival</param>
    /// <param name="usuarioEsGanador">Indica si el usuario ha ganado o no la partida</param>
    /// <param name="fontTitulo">Fuente utilizada para desplegar el titulo de la pantalla final</param>
    /// <param name="partidoAbandonado">Indica si el partido ha sido abandonado, en dado caso muestro un mensaje diferente</param>
    public void MostrarPantallaFinal(string nombreEquipoJugador, TipoEquipo tipoEquipoJugador, string nombreEquipoRival, bool usuarioEsGanador, FigletFont? fontTitulo, bool partidoAbandonado)
    {
        // Solicito al usuario una tecla para luego mostrar la pantalla final
        AnsiConsole.Write(new Markup("\n[grey70 italic](( El partido ha finalizado, presione una tecla para continuar al dashboard... ))[/]").Centered());
        Console.ReadKey(true);

        AnsiConsole.Clear();

        // Según si el usuario ha ganado o no el partido, algunos mensajes y colores cambian
        Color colorTitulo;
        string colorSeparador;
        string colorNombreEquipos;
        string colorTipoEquipo;
        string colorTexto;
        string descripcion;
        string encabezadoTitulo;

        if (partidoAbandonado)
        {
            encabezadoTitulo = "\nAbandonaste";
            colorSeparador = "red3";
            colorTitulo = Color.Red;
            colorNombreEquipos = "red1";
            colorTipoEquipo = "red3_1";
            colorTexto = "indianred_1";
            descripcion = "ha abandonado";
        }
        else if (usuarioEsGanador)
        {
            encabezadoTitulo = "\nYou win!";
            colorSeparador = "orange1";
            colorTitulo = Color.Yellow;
            colorNombreEquipos = "gold1";
            colorTipoEquipo = "lightgoldenrod2_1";
            colorTexto = "lightgoldenrod2_2";
            descripcion = "le ha ganado como";
        }
        else
        {
            encabezadoTitulo = "\nGame over :(";
            colorSeparador = "red3";
            colorTitulo = Color.Red;
            colorNombreEquipos = "red1";
            colorTipoEquipo = "red3_1";
            colorTexto = "indianred_1";
            descripcion = "ha caido como";
        }

        var separador = new Rule().RuleStyle(Style.Parse(colorSeparador)).Border(BoxBorder.Double);
        var titulo = (fontTitulo != null) ? new FigletText(fontTitulo, encabezadoTitulo) : new FigletText("\n" + encabezadoTitulo);
        titulo.Color(colorTitulo);
        var texto = new Markup($"[{colorNombreEquipos} underline]{nombreEquipoJugador}[/] [{colorTexto}]{descripcion}[/][{colorTipoEquipo}]{(partidoAbandonado ? "" : $" {tipoEquipoJugador}")}[/] [{colorTexto}]ante el equipo[/] [{colorNombreEquipos}]{nombreEquipoRival}[/]\n\n");

        var layoutPantallaFinal = new Layout("raiz");
        layoutPantallaFinal["raiz"].Update(
            new Panel(Align.Center(
                new Rows(
                    separador,
                    new Text(""), // separador
                    new Text(""), // separador
                    titulo,
                    texto,
                    separador
                ),
                VerticalAlignment.Middle
            ))
            .Expand()
            .Border(BoxBorder.None)
        );
        AnsiConsole.Write(layoutPantallaFinal);
    }

    /// <summary>
    /// Muestra una animación para el encabezado del partido
    /// </summary>
    /// <param name="figletLocal">Figlet text a mostrar del equipo local</param>
    /// <param name="figletVisitante">Figlet text a mostrar del equipo visitante</param>
    /// <param name="tipoPartido">Tipo de partido a disputarse</param>
    public void MostrarEncabezadoPartido(FigletText figletLocal, FigletText figletVisitante, TipoPartido tipoPartido)
    {
        AnsiConsole.Clear(); 

        var layout = new Layout("raiz")
            .SplitRows(new Layout("arriba"));
        
        // Textos a mostrarse por pantalla
        var textos = new IRenderable[] {
            new Markup("Se va a disputar..."),
            figletLocal,
            new Rule("[orange1 bold]VS[/]") { Style = Style.Parse("bold gray"), Justification = Justify.Center },
            figletVisitante,
            new Markup($"En un partido [orange1]{tipoPartido}[/]")
        };

        // Muestro el layout con el componente Live para que se vaya actualizando de a poco
        AnsiConsole.Live(layout)
            .Start(controladorDisplay => {
                // En esta lista almaceno los objetos que voy mostrando por pantalla para dar el efecto
                // de que van apareciendo uno por uno
                var textosMostrados = new List<IRenderable>();

                foreach (var linea in textos)
                {
                    textosMostrados.Add(linea);
                    layout["arriba"].Update(
                        new Panel(Align.Center(
                            new Rows(textosMostrados),
                            VerticalAlignment.Middle
                        ))
                        .Expand()
                        .Border(BoxBorder.None)
                    );
                    
                    textosMostrados.Add(new Text(""));
                    controladorDisplay.Refresh();
                    VistasUtil.PausarVistas(1);
                }

                VistasUtil.PausarVistas(3);
            });
    }
}

/// <summary>
/// Clase vista que se encarga de mostrar los datos del historial de un equipo
/// </summary>
public class PanelHistorial : Vista
{
    private string nombreEquipo;
    private Historial informacionHistorial;

    public PanelHistorial(Historial informacionHistorial, string nombreEquipo)
    {
        this.informacionHistorial = informacionHistorial;
        this.nombreEquipo = nombreEquipo;
    }

    public override void Dibujar()
    {
        if (!informacionHistorial.HistorialPartidos.Any())
        {
            AnsiConsole.Write($":warning: [tan]El equipo {nombreEquipo} no ha jugado partidos aún[/]");
        }
        else
        {
            var separador = new Rule().RuleStyle(Style.Parse("gray"));
            var arbolPartidos = new Tree($":newspaper: [red bold]Historial de {nombreEquipo}[/]").Style(Style.Parse("gray"));
            var indicaciones = new Markup("\n[gray italic](( Presione una tecla para volver al dashboard))[/]");

            foreach (var partido in informacionHistorial.HistorialPartidos)
            {
                var resultado = partido.NombreGanador.Equals("ABANDONADO") ? "[red3_1]ABANDONADO[/]"
                                                                           : nombreEquipo.Equals(partido.NombreGanador) ? "[greenyellow]GANADO[/]" : "[red3_1]PERDIDO[/]";

                var nodoPartido = arbolPartidos.AddNode(
                    $"{resultado} [navajowhite1]• Partido[/] [lightgoldenrod2_2]{partido.TipoPartido}[/][navajowhite1]:[/] [cornsilk1]{partido.Local.Nombre}[/] [grey78](LOCAL)[/] :vs_button: [cornsilk1]{partido.Visitante.Nombre}[/] [grey78](VISITANTE)[/]"
                );

                nodoPartido.AddNode(
                    $"[grey70]Al mejor de:[/] [gold1]{partido.SetMaximos}[/]"
                );
                nodoPartido.AddNode(
                    $"[grey70]Resultado final:[/] [gold1]{partido.ScoreLocal} - {partido.ScoreVisitante}[/] [grey78](L - V)[/]"
                );

                var nodoResultados = nodoPartido.AddNode(
                    $"[grey70]Resultados de los sets:[/] [grey78](L - V)[/]"
                );
                foreach (var resultadoSet in partido.ResultadoSets)
                {
                    nodoResultados.AddNode(
                        $"[grey70]Set[/] [white]N.{resultadoSet.Key}[/][grey70]:[/] [gold1]{resultadoSet.Value.PuntosLocal} - {resultadoSet.Value.PuntosVisitante}[/]"
                    );
                }

                /* No se muestra nada en los jugadores, para revisar
                var nodoJugadores = nodoPartido.AddNode($"[grey70]Jugadores convocados:[/]");

                var jugadores = $"[gold1]{partido.Local.Nombre}:[/] ";
                foreach (var jugadorLocal in partido.Local.JugadoresConvocados)
                {
                    if (partido.Local.JugadoresConvocados.Last().Equals(jugadorLocal))
                    {
                        jugadores += $"[grey78]{jugadorLocal}[/]";
                        continue;
                    }

                    jugadores += $"[grey78]{jugadorLocal}[/] [grey70], [/]";
                }
                nodoJugadores.AddNode(jugadores);


                jugadores = $"[gold1]{partido.Visitante.Nombre}:[/] ";
                foreach (var jugadorVisitante in partido.Visitante.JugadoresConvocados)
                {
                    if (partido.Visitante.JugadoresConvocados.Last().Equals(jugadorVisitante))
                    {
                        jugadores += $"[grey78]{jugadorVisitante}[/]";
                        continue;
                    }

                    jugadores += $"[grey78]{jugadorVisitante}[/] [grey70], [/]";
                }
                nodoJugadores.AddNode(jugadores);
                */
            }

            AnsiConsole.Write(
                new Rows(
                    separador,
                    arbolPartidos,
                    indicaciones,
                    separador
                )
            );
        }        
    }
}

/// <summary>
/// Clase vista que se encarga de mostrar los datos de la plantilla de jugadores de un equipo
/// </summary>
public class PanelPlantilla : Vista
{
    private string nombreEquipo;
    private List<Jugador> jugadores;

    public PanelPlantilla(List<Jugador> jugadores, string nombreEquipo)
    {
        this.jugadores = jugadores;
        this.nombreEquipo = nombreEquipo;
    }

    public override void Dibujar()
    {
        if (!jugadores.Any())
        {
            AnsiConsole.Write($":warning: [navajowhite1]El equipo[/] [cornsilk1]{nombreEquipo}[/] [navajowhite1]no tiene jugadores[/]");
        }
        else
        {
            var separador = new Rule().RuleStyle(Style.Parse("gray"));
            var arbolJugadores = new Tree($":newspaper: [red bold]Jugadores del equipo {nombreEquipo}[/]").Style(Style.Parse("gray"));
            var indicaciones = new Markup("\n[gray italic](( Presione una tecla para volver al dashboard))[/]");

            foreach (var jugador in jugadores)
            {
                var nodoJugador = arbolJugadores.AddNode(
                    $"[cornsilk1]{jugador.NumeroCamiseta}[/] :t_shirt: [navajowhite1]{jugador.Nombre}[/]"
                );

                nodoJugador.AddNode($"[grey70]Posición de preferencia:[/] [gold1]{jugador.TipoJugador}[/]");
                nodoJugador.AddNode($"[grey70]Experiencia en juego:[/] [gold1]{jugador.Experiencia} pts.[/]");

                var nodoHabilidades = nodoJugador.AddNode($"[grey70]Habilidades:[/]");
                nodoHabilidades.AddNode(
                    $"[gray]SAQUE:[/] {jugador.HabilidadSaque}" +
                    $"[gray], REMATE:[/] {jugador.HabilidadRemate}" +
                    $"[gray], RECEPCION:[/] {jugador.HabilidadRecepcion}" +
                    $"[gray], COLOCACION:[/] {jugador.HabilidadColocacion}" +
                    $"[gray], BLOQUEO:[/] {jugador.HabilidadBloqueo}"
                );
            }

            AnsiConsole.Write(
                new Rows(
                    separador,
                    arbolJugadores,
                    indicaciones,
                    separador
                )
            );
        }
    }
}